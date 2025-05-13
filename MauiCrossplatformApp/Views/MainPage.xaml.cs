using CommunityToolkit.Mvvm.ComponentModel;
using MauiCrossplatformApp.Interfaces;
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
