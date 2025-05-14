using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiCrossplatformApp.Data;
using MauiCrossplatformApp.Data.Interfaces;
using MauiCrossplatformApp.Models;
using MauiCrossplatformApp.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    private readonly INoteRepository _noteRepo;
    private readonly List<FileSystemItemViewModel> _snapshot = new();

    public ObservableCollection<FileSystemItemViewModel> TreeItems { get; } = new();

    [ObservableProperty]
    private string? searchText;

    public IAsyncRelayCommand ReloadCommand { get; }

    public AppShellViewModel(INoteRepository noteRepo)
    {
        _noteRepo = noteRepo;
        ReloadCommand = new AsyncRelayCommand(BuildTreeAsync);

        _ = ReloadCommand.ExecuteAsync(null);          // initial load
    }

    /* ───────────────────────── LOAD ───────────────────────── */
    private async Task BuildTreeAsync()
    {
        TreeItems.Clear();

        var folders = (await _noteRepo.GetAllFoldersAsync()).ToList();
        var notes = (await _noteRepo.GetAllNotesAsync()).ToList();

        // one VM per row
        var map = folders.Cast<FileSystemEntry>()
                         .Concat(notes)
                         .ToDictionary(x => x.Id, x => new FileSystemItemViewModel(x));

        // wire parents
        foreach (var vm in map.Values.Where(v => v.Source.ParentId is int))
        {
            var parent = map[vm.Source.ParentId!.Value];
            vm.Depth = parent.Depth + 1;
            parent.Children.Add(vm);
        }

        // root = no parent  (folders after loose notes)
        var roots = notes.Where(n => n.ParentId == null).Select(n => map[n.Id])
                   .Concat(folders.Where(f => f.ParentId == null).Select(f => map[f.Id]));

        foreach (var r in roots.OrderBy(r => r.Name))
            TreeItems.Add(r);

        // keep an immutable snapshot for search
        _snapshot.Clear();
        _snapshot.AddRange(TreeItems.Select(CloneDeep));

        ApplyFilter();                   // show first time (SearchText may already be set)
    }

    /* ───────────────────────── SEARCH ─────────────────────── */
    partial void OnSearchTextChanged(string _, string __) => ApplyFilter();

    private void ApplyFilter()
    {
        TreeItems.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var root in _snapshot)
                TreeItems.Add(root);
            return;
        }

        var term = SearchText!;

        var filtered = _snapshot
            .Select(root =>
            {
                var clone = FilterBranch(root, term, out var keep);
                return (clone, keep);
            })
            .Where(t => t.keep && t.clone is not null)
            .Select(t => t.clone!);

        foreach (var r in filtered) TreeItems.Add(r);

        // local iterator that clones only matching sub‑branches
        static FileSystemItemViewModel? FilterBranch(
            FileSystemItemViewModel src,
            string term,
            out bool keep)
        {
            keep = src.Name.Contains(term, StringComparison.OrdinalIgnoreCase);
            var clone = new FileSystemItemViewModel(src.Source!) { Depth = src.Depth };

            foreach (var c in src.Children)
                if (FilterBranch(c, term, out var ck) is { } child)
                {
                    keep |= ck;
                    if (ck) clone.Children.Add(child);
                }

            return keep ? clone : null;
        }
    }

    /* ──────────────────────── helpers ─────────────────────── */
    private static FileSystemItemViewModel CloneDeep(FileSystemItemViewModel src)
    {
        var copy = new FileSystemItemViewModel(src.Source!) { Depth = src.Depth };
        foreach (var c in src.Children) copy.Children.Add(CloneDeep(c));
        return copy;
    }
}
