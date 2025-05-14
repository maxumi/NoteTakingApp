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

            // 1) Prepare DB path
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");

            // 2) Use a sync connection to create & seed, then expose async
            builder.Services.AddSingleton<SQLiteAsyncConnection>(_ =>
            {
                // Synchronous connection for setup
                var syncConn = new SQLiteConnection(dbPath);
                syncConn.CreateTable<Folder>();
                syncConn.CreateTable<Note>();

                // Seed if empty
                if (syncConn.Table<Folder>().Count() == 0)
                {
                    var folder = new Folder { Name = "Samples", Path = "Samples" };
                    syncConn.Insert(folder);

                    var now = DateTime.UtcNow;
                    syncConn.Insert(new Note
                    {
                        Name = "Welcome.md",
                        Path = "Samples/Welcome.md",
                        ParentId = folder.Id,
                        Content = "# Welcome to **MauiCrossplatformApp**\n\nThis is a *seeded* note.",
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                    syncConn.Insert(new Note
                    {
                        Name = "Shortcuts.md",
                        Path = "Samples/Shortcuts.md",
                        ParentId = folder.Id,
                        Content = "## Shortcuts\n- **Save**: Ctrl+S\n- **Read**: Ctrl+R",
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }

                // Return the async connection for runtime use
                return new SQLiteAsyncConnection(dbPath);
            });

            // 3) Repository & VMs
            builder.Services.AddScoped<INoteRepository, MockRepo>();
            builder.Services.AddTransient<NotePageViewModel>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<AppShellViewModel>();
            builder.Services.AddTransient<FileSystemItemViewModel>();

            builder.Services.AddSingleton<AppShell>();

            return builder.Build();
        }
    }
}
