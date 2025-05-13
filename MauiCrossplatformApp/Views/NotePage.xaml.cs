using MauiCrossplatformApp.ViewModels;

namespace MauiCrossplatformApp.Views;

public partial class NotePage : ContentPage
{
	public NotePage(NotePageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

    }
}