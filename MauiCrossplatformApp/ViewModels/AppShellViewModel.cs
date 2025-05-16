using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiCrossplatformApp.Data;
using MauiCrossplatformApp.Data.Interfaces;
using MauiCrossplatformApp.Models;
using MauiCrossplatformApp.Services;
using MauiCrossplatformApp.ViewModels;
using MauiCrossplatformApp.Views;

public partial class AppShellViewModel : ObservableObject
{
    private readonly INoteService _service;
    // Master List, it holds all currently loaded notes and folders
    private readonly List<FileSystemItemViewModel> _roots = new();

    // Filtered List, it holds the currently displayed notes and folders
    public ObservableCollection<FileSystemItemViewModel> TreeItems { get; } = new();

    [ObservableProperty] private string? searchText;
    [ObservableProperty] private bool expandNext = true;
    [ObservableProperty] private bool sortAscending = true;
    [ObservableProperty] private FileSystemItemViewModel? selected;
    public IAsyncRelayCommand ReloadCommand { get; }

    public AppShellViewModel(INoteService service)
    {
        _service = service;
        // Only uses this for the initial load. Otherwise i need to declare Constructor async.
        ReloadCommand = new AsyncRelayCommand(LoadAsync);
        _ = ReloadCommand.ExecuteAsync(null);
    }

    // Load all notes and folders from the repository
    private async Task LoadAsync()
    {
        _roots.Clear();
        TreeItems.Clear();

        // pull the already‐built tree from the API
        var entries = await _service.GetTreeAsync();
        foreach (var dto in entries)
            _roots.Add(new FileSystemItemViewModel(dto, _service));

        RefreshTree();
    }

    /* ───────────────────────── search / sort ───────────────────── */
    partial void OnSearchTextChanged(string _, string __) => RefreshTree();

    private void RefreshTree()
    {
        var query = _roots.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
            query = query.Where(r => ContainsRecursive(r, SearchText!, StringComparison.OrdinalIgnoreCase));

        query = sortAscending
            ? query.OrderByDescending(n => n.IsFolder).ThenBy(n => n.Name)
            : query.OrderByDescending(n => n.IsFolder).ThenByDescending(n => n.Name);

        TreeItems.ReplaceAll(query);
    }

    private static bool ContainsRecursive(FileSystemItemViewModel n, string term, StringComparison cmp)
        => n.Name.Contains(term, cmp) || n.Children.Any(c => ContainsRecursive(c, term, cmp));

    #region Commands

    [RelayCommand]
    private async Task NewNoteAsync(int? parentFolderId = null)
    {
        var dto = new NoteDto
        {
            Name = "Untitled.md",
            Content = "",
            ParentId = parentFolderId
        };

        var created = await _service.CreateNoteAsync(dto);
        await Shell.Current.GoToAsync($"{nameof(NotePage)}?noteId={created.Id}");
        Shell.Current.FlyoutIsPresented = false;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task EditAsync()
    {
        // WIP
    }

    [RelayCommand]
    private void ToggleSort()
    {
        sortAscending = !sortAscending;
        RefreshTree();
    }

    [RelayCommand]
    private void ToggleExpandCollapse()
    {
        foreach (var r in _roots) SetExpandedRecursive(r, expandNext);
        expandNext = !expandNext;
        RefreshTree();
    }

    #endregion

    private static void SetExpandedRecursive(FileSystemItemViewModel n, bool val)
    {
        if (!n.IsFolder) return;
        n.IsExpanded = val;
        foreach (var c in n.Children) SetExpandedRecursive(c, val);
    }
}
public static class ObservableCollectionExtensions
{
    public static void ReplaceAll<T>(this ObservableCollection<T> col, IEnumerable<T> items)
    {
        col.Clear();
        foreach (var i in items) col.Add(i);
    }
}
