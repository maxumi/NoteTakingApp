using CommunityToolkit.Mvvm.DependencyInjection;
using MauiCrossplatformApp.ViewModels;

namespace MauiCrossplatformApp.Views;

[QueryProperty(nameof(NoteId), "noteId")]
public partial class NotePage : ContentPage
{
    private readonly NotePageViewModel _vm;
    public NotePage(NotePageViewModel viewModel)
    {
        InitializeComponent();

        // resolve VM via DI (or use constructor injection if you prefer)
        _vm = viewModel;
        BindingContext = _vm;
    }

    public int NoteId
    {
        get => _vm.NoteId;
        set => _vm.NoteId = value;
    }
}