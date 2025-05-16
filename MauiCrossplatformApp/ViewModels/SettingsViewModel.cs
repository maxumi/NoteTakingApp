using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;      // Preferences

namespace MauiCrossplatformApp.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private const string ThemeKey = "Theme";

    public IReadOnlyList<string> AvailableThemes { get; } = new[] { "System", "Light", "Dark" };

    [ObservableProperty]
    private string selectedTheme = Preferences.Get(ThemeKey, "System");

    public SettingsViewModel()
    {
        // Applies theme on startup
        ApplyTheme(selectedTheme);
    }

    partial void OnSelectedThemeChanged(string value)
    {
        ApplyTheme(value);
        Preferences.Set(ThemeKey, value);
    }
    private static void ApplyTheme(string themeName)
    {
        var appTheme = themeName switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified // "System" or any unknown string
        };

        Application.Current!.UserAppTheme = appTheme;
    }

}
