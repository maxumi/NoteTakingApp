// ViewModels/FileSystemItemViewModel.cs
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiCrossplatformApp.Models;   // where FileSystemEntryDto lives
using MauiCrossplatformApp.Services;
using MauiCrossplatformApp.Views;

namespace MauiCrossplatformApp.ViewModels
{
    public partial class FileSystemItemViewModel : ObservableObject
    {
        public int Id { get; }
        [ObservableProperty] private bool isExpanded = true;
        public string Name { get; }
        public bool IsFolder => Source.Type == "Folder";

        public ObservableCollection<FileSystemItemViewModel> Children { get; }
            = new();

        public FileSystemEntryDto Source { get; }

        [ObservableProperty] private int depth;

        private readonly INoteService _noteService;
        public FileSystemItemViewModel(FileSystemEntryDto src, int depth = 0)
        {
            Source = src;
            Id = src.Id;
            Name = src.Name;
            Depth = depth;
            IsExpanded = true;

            // recurse into children
            foreach (var childDto in src.Children ?? Enumerable.Empty<FileSystemEntryDto>())
            {
                Children.Add(new FileSystemItemViewModel(childDto, depth + 1));
            }
        }

        [RelayCommand]
        private async Task ShowMoreAsync()
        {
            var page = Application.Current!.MainPage;
            string? choice = await page.DisplayActionSheet(
                title: Name,
                cancel: "Cancel",
                destruction: "Delete",
                buttons: new[] { "Rename", "Share" });

            switch (choice)
            {
                case "Rename": await RenameAsync(); break;
                case "Delete": await DeleteAsync(); break;
                case "Share": await ShareAsync(); break;
            }
        }

        private async Task RenameAsync()
        {
            // TODO: call service to rename, then update Name/Source.Name
            await Shell.Current.DisplayAlert("Rename", $"TODO rename {Name}", "OK");
        }

        private async Task DeleteAsync()
        {
            if (IsFolder)
            {
                // TODO: delete folder via service
                await Shell.Current.DisplayAlert("Delete", $"TODO delete folder {Name}", "OK");
            }
            else
            {
                await _noteService.DeleteNoteAsync(Id);
            }
        }

        private Task ShareAsync()
        {
            // TODO: implement share
            return Shell.Current.DisplayAlert("Share", $"TODO share {Name}", "OK");
        }

        [RelayCommand]
        private async Task OpenAsync()
        {
            if (!IsFolder)
                await Shell.Current.GoToAsync($"{nameof(NotePage)}?noteId={Id}");
            Shell.Current.FlyoutIsPresented = false;
        }
    }
}
