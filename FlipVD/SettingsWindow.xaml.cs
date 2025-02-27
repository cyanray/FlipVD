using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlipVD
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private const string AppName = "FlipVD";
        private readonly string AppPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private bool EnableAutoStart()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (key == null) return false;

                key.SetValue(AppName, $"\"{AppPath}\"");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool DisableAutoStart()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (key == null) return false;

                key.DeleteValue(AppName, false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsAutoStartEnabled()
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false);
            if (key == null) return false;
            return key.GetValue(AppName) != null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox_LaunchAtStartup.IsChecked = IsAutoStartEnabled();
        }

        private void CheckBox_LaunchAtStartup_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBox_LaunchAtStartup.IsChecked == true)
            {
                bool result = EnableAutoStart();
            }
            else
            {
                bool result = DisableAutoStart();
            }
        }
    }
}
