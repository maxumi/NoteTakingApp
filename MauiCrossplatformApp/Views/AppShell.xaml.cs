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

            Routing.RegisterRoute(nameof(NotePage), typeof(NotePage));

        }
    }
}
