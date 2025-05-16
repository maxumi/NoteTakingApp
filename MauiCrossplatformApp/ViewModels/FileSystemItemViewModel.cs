﻿// ViewModels/FileSystemItemViewModel.cs
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiCrossplatformApp.Messenger;
using MauiCrossplatformApp.Models;   // where FileSystemEntryDto lives
using MauiCrossplatformApp.Services;
using MauiCrossplatformApp.Views;

namespace MauiCrossplatformApp.ViewModels
{
    public partial class FileSystemItemViewModel : ObservableObject
    {
        public int Id { get; }
        [ObservableProperty] private bool isExpanded = true;
        public string Name { get; set; }
        public bool IsFolder => Source.Type == "Folder";

        public ObservableCollection<FileSystemItemViewModel> Children { get; }
            = new();

        public FileSystemEntryDto Source { get; }

        [ObservableProperty]
        public  int depth;



        private readonly INoteService _noteService;
        public FileSystemItemViewModel(FileSystemEntryDto src, INoteService noteService, int depth = 0)
        {
            Source = src;
            Id = src.Id;
            Name = src.Name;
            Depth = depth;
            _noteService = noteService;
            foreach (var childDto in src.Children ?? Enumerable.Empty<FileSystemEntryDto>())
            {
                Children.Add(new FileSystemItemViewModel(childDto, noteService, depth + 1));
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
                buttons: new[] { "Open", "Rename" });

            switch (choice)
            {
                case "Open": await OpenAsync(); break;
                case "Rename": await RenameAsync(); break;
                case "Delete": await DeleteAsync(); break;
            }
        }

        [RelayCommand]
        private async Task RenameAsync()
        {
            // ask user for the new file name
            var newName = await Shell.Current.DisplayPromptAsync(
                              "Rename note",
                              "Enter a new name:",
                              initialValue: Name);

            if (string.IsNullOrWhiteSpace(newName) || newName == Name)
                return;

            try
            {
                // 1. server-side rename
                await _noteService.RenameNoteAsync(Source.Id, newName);

                // 2. update the local model / UI
                Source.Name = Name = newName;

                // keep Path in sync if it mirrors the file name
                if (!string.IsNullOrEmpty(Source.Path))
                {
                    var lastSlash = Source.Path.LastIndexOf('/');
                    Source.Path = lastSlash >= 0
                        ? $"{Source.Path[..(lastSlash + 1)]}{newName}"
                        : newName;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error",
                                                 $"Could not rename note: {ex.Message}",
                                                 "OK");
            }
            UpdateShell();
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            // Confirm with user (optional)
            var confirm = await Shell.Current.DisplayAlert(
                              "Delete",
                              $"Delete note “{Name}”?",
                              "Delete", "Cancel");
            if (!confirm)
                return;

            try
            {
                await _noteService.DeleteNoteAsync(Id);
                UpdateShell();

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Could not delete note: {ex.Message}",
                    "OK");
            }
        }

        [RelayCommand]
        private async Task OpenAsync()
        {
            if (!IsFolder)
                await Shell.Current.GoToAsync($"{nameof(NotePage)}?noteId={Id}");
            Shell.Current.FlyoutIsPresented = false;
        }

        private void UpdateShell()
        {
            WeakReferenceMessenger.Default.Send(new RefreshMessenger("refresh"));

        }
    }
}