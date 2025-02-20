using Microsoft.Win32;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FlipVD;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        bool Debug_DoNotMoveToTaskbar = false;

        base.OnStartup(e);

        SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        ApplyTheme();

        MainWindow mainWindow = new MainWindow();
        this.MainWindow = mainWindow;
        mainWindow.Show();


        if (!Debug_DoNotMoveToTaskbar)
        {
            mainWindow.MoveToTaskbar();
        }

    }

    private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category == UserPreferenceCategory.General)
        {
            ApplyTheme();
        }
    }

    private void ApplyTheme()
    {
        bool isLightMode = IsLightTheme();

        string themePath = isLightMode ? "Resources/Themes/LightTheme.xaml" : "Resources/Themes/DarkTheme.xaml";

        Resources.MergedDictionaries.Clear();
        Resources.MergedDictionaries.Add(new ResourceDictionary()
        {
            Source = new Uri(themePath, UriKind.Relative)
        });
    }

    private static bool IsLightTheme()
    {
        const string registryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        using var key = Registry.CurrentUser.OpenSubKey(registryKey);
        if (key != null && key.GetValue("AppsUseLightTheme") is int lightTheme)
        {
            return lightTheme == 1;
        }
        return false;
    }
}

