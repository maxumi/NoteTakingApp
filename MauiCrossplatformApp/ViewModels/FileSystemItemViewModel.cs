// ViewModels/FileSystemItemViewModel.cs
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiCrossplatformApp.Data;
using MauiCrossplatformApp.Models;
using MauiCrossplatformApp.Repositories;
using MauiCrossplatformApp.Views;

namespace MauiCrossplatformApp.ViewModels;
public partial class FileSystemItemViewModel : ObservableObject
{
    public int Id { get; }
    [ObservableProperty] 
    private bool isExpanded = true;
    public string Name { get; }
    public bool IsFolder => Source is Folder;

    public ObservableCollection<FileSystemItemViewModel> Children { get; }
        = new();

    public FileSystemEntry Source { get; }

    [ObservableProperty]
    int depth;


    [RelayCommand]
    private async Task OpenAsync()
    {
        if (!IsFolder)
            await Shell.Current.GoToAsync($"{nameof(NotePage)}?noteId={Id}");
        Shell.Current.FlyoutIsPresented = false;
    }

    public FileSystemItemViewModel(FileSystemEntry src)
    {
        Source = src;
        Id = src.Id;
        Name = src.Name;
    }
}
