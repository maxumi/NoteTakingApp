using System.IO;
using MauiCrossplatformApp.Data;
using MauiCrossplatformApp.Data.Interfaces;
using MauiCrossplatformApp.Models;
using MauiCrossplatformApp.Repositories;
using MauiCrossplatformApp.ViewModels;
using Microsoft.Extensions.Logging;
using SQLite;
using System.Diagnostics;
using CommunityToolkit.Maui;
using MauiCrossplatformApp.Services;
namespace MauiCrossplatformApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<NotePageViewModel>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<AppShellViewModel>();
            builder.Services.AddTransient<FileSystemItemViewModel>();
            builder.Services.AddTransient<INoteService, NoteService>();

            builder.Services.AddSingleton<AppShell>();

            return builder.Build();
        }
    }
}
