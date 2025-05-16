using MauiCrossplatformApp.ViewModels;
namespace MauiCrossplatformApp.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}