using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiCrossplatformApp.Interfaces;
using MauiCrossplatformApp.Models;
using MauiCrossplatformApp.Views;

namespace MauiCrossplatformApp.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly INoteRepository _repo;

    public MainPageViewModel(INoteRepository repo)
    {
        _repo = repo;
        _ = InitialiseAsync();
    }

    private async Task InitialiseAsync()
    {
        var first = (await _repo.GetAllNotesAsync().ConfigureAwait(false))
                    .FirstOrDefault();

        if (first is not null)
        {
            await Shell.Current.GoToAsync(nameof(NotePage),
                                          new() { { "NoteId", first.Id } });
            return;
        }

        await NewNoteAsync();
    }

    [RelayCommand]
    private async Task NewNoteAsync()
    {
        var note = new Note
        {
            FolderPath = "/",
            FileName = "Untitled.md",
            Content = "# New Note\n\nStart writing…"
        };

        await _repo.SaveNoteAsync(note).ConfigureAwait(false);

        await Shell.Current.GoToAsync(nameof(NotePage),
                                      new() { { "NoteId", note.Id } });
    }
}
