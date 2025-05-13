using CommunityToolkit.Mvvm.ComponentModel;
using MauiCrossplatformApp.ViewModels;

namespace MauiCrossplatformApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }

}
