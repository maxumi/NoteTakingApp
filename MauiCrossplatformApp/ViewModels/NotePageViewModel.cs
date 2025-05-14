using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Markdig;
using MauiCrossplatformApp.Data.Interfaces;
using MauiCrossplatformApp.Models;

namespace MauiCrossplatformApp.ViewModels
{
    public partial class NotePageViewModel : ObservableObject
    {
        private readonly INoteRepository _repository;
        private Note _currentNote = new();

        public NotePageViewModel(INoteRepository repository)
        {
            _repository = repository;
        }

        // 1) The ID that Shell passes in
        [ObservableProperty]
        private int noteId;

        // 2) When it changes, kick off load
        partial void OnNoteIdChanged(int value)
        {
            // fire-and-forget is OK here
            _ = LoadNoteAsync(value);
        }

        // 3) Load a single note by ID
        private async Task LoadNoteAsync(int id)
        {
            if (id <= 0) return;
            _currentNote = await _repository.GetNoteAsync(id)
                            .ConfigureAwait(false)
                         ?? new Note();

            // split _currentNote.Path → FolderPath + FileName
            if (!string.IsNullOrWhiteSpace(_currentNote.Path))
            {
                var i = _currentNote.Path.LastIndexOfAny(new[] { '/', '\\' });
                FolderPath = i > 0
                    ? _currentNote.Path[..i]
                    : "/";
                FileName = i >= 0
                    ? _currentNote.Path[(i + 1)..]
                    : _currentNote.Path;
            }
            else
            {
                FolderPath = "/";
                FileName = _currentNote.Name;
            }

            NoteContent = _currentNote.Content;
        }

        // … the rest of your properties & commands stay exactly the same …
        public string Title
        {
            get
            {
                var folder = FolderPath?.Trim().TrimEnd('/', '\\');
                return string.IsNullOrEmpty(folder)
                    ? FileName
                    : $"{folder}/{FileName}";
            }
        }

        [ObservableProperty] private string fileName = "Untitled.md";
        [ObservableProperty] private string folderPath = "/";
        [ObservableProperty] private bool isReading;
        [ObservableProperty] private string renderedHtml = string.Empty;
        [ObservableProperty] private string noteContent = string.Empty;
        partial void OnFolderPathChanged(string value) => OnPropertyChanged(nameof(Title));
        partial void OnFileNameChanged(string value) => OnPropertyChanged(nameof(Title));

        [RelayCommand]
        private void ToggleReading()
        {
            IsReading = !IsReading;
            if (IsReading)
                RenderedHtml = Markdown.ToHtml(NoteContent ?? string.Empty);
        }

        [ObservableProperty] private bool isSaving;
        [ObservableProperty] private bool saveCompleted;

        [RelayCommand]
        private async Task SaveAsync()
        {
            var folder = FolderPath?.Trim().TrimEnd('/', '\\');
            var fullPath = string.IsNullOrEmpty(folder)
                ? FileName.Trim()
                : $"{folder}/{FileName.Trim()}";

            _currentNote.Name = FileName.Trim();
            _currentNote.Path = fullPath;
            _currentNote.Content = NoteContent;
            _currentNote.Touch();

            await _repository.SaveNoteAsync(_currentNote)
                             .ConfigureAwait(false);
            SaveCompleted = true;
        }
    }
}
