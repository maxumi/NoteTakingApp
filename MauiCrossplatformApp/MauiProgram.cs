using MauiCrossplatformApp.Interfaces;
using MauiCrossplatformApp.Repositories;
using MauiCrossplatformApp.ViewModels;
using Microsoft.Extensions.Logging;

namespace MauiCrossplatformApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.Services.AddSingleton<INoteRepository, InMemoryNoteRepository>();
            builder.Services.AddTransient<NotePageViewModel>();
            builder.Services.AddTransient<MainPageViewModel>();


            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
