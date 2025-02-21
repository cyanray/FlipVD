using Microsoft.Win32;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Controls;

namespace FlipVD;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private NotifyIcon? m_TrayIcon;


    protected override void OnStartup(StartupEventArgs e)
    {
        bool Debug_DoNotMoveToTaskbar = false;

        base.OnStartup(e);

        m_TrayIcon = new NotifyIcon
        {
            Icon = new Icon("Resources/Icon/Icon.ico"),
            Visible = true,
            Text = "FlipVD"
        };


        ContextMenuStrip menu = new ContextMenuStrip();
        menu.Items.Add("Exit", null, ExitApplication);
        m_TrayIcon.ContextMenuStrip = menu;


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


    private void ExitApplication(object? sender, EventArgs e)
    {
        m_TrayIcon?.Dispose();
        Current.Shutdown();
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

