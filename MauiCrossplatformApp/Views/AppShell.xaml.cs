using MauiCrossplatformApp.Views;
using MauiCrossplatformApp.ViewModels;
using MauiCrossplatformApp.Converters;
namespace MauiCrossplatformApp
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            // Register routes for navigation
            Routing.RegisterRoute(nameof(NotePage), typeof(NotePage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));


        }
    }
}
