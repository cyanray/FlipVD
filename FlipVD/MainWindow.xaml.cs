using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Win32;
using Windows.Win32.Foundation;
using GWL = Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX;
using Window_Style = Windows.Win32.UI.WindowsAndMessaging.WINDOW_STYLE;
using Window_Ex_Style = Windows.Win32.UI.WindowsAndMessaging.WINDOW_EX_STYLE;
using SWP = Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS;
using static FlipVD.KeyboardSimulation;
using WindowsVirtualDesktopHelper.VirtualDesktopAPI;
using System.Windows.Media.Animation;

namespace FlipVD;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private IVirtualDesktopManager? VirtualDesktopManager = null;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        string name = Loader.GetImplementationForOS();
        VirtualDesktopManager = Loader.LoadImplementationWithFallback(name);

        NameOfVD.Text = VirtualDesktopManager?.CurrentDisplayName();
    }

    public bool MoveToTaskbar()
    {
        // Get Win32 handle of this WPF window
        HWND thisHandle = (HWND)new WindowInteropHelper(this).Handle;

        // Find taskbar handle
        HWND taskbarHandle = PInvoke.FindWindow("Shell_TrayWnd", null);
        if (taskbarHandle == IntPtr.Zero)
        {
            return false;
        }

        PInvoke.SetParent(thisHandle, taskbarHandle);

        // Change window style to WS_CHILD
        int style = PInvoke.GetWindowLong(thisHandle, GWL.GWL_STYLE);
        style &= ~unchecked((int)Window_Style.WS_POPUP);
        style |= (int)Window_Style.WS_CHILD;
        PInvoke.SetWindowLong(thisHandle, GWL.GWL_STYLE, style);

        // Change window style to ToolWindow
        int exStyle = PInvoke.GetWindowLong(thisHandle, GWL.GWL_EXSTYLE);
        exStyle |= (int)Window_Ex_Style.WS_EX_TOOLWINDOW;
        PInvoke.SetWindowLong(thisHandle, GWL.GWL_EXSTYLE, exStyle);

        // Get rect of taskbar
        if (!PInvoke.GetWindowRect(taskbarHandle, out RECT taskbarRect))
        {
            return false;
        }
        int taskbarWidth = taskbarRect.right - taskbarRect.left;
        int taskbarHeight = taskbarRect.bottom - taskbarRect.top;


        int windowWidth = (int)this.Width;
        int windowHeight = (int)taskbarHeight - 24;


        int posX_screen = (taskbarWidth - windowWidth) / 2;
        int posY_screen = (taskbarHeight - windowHeight) / 2;

        System.Drawing.Point pt = new() { X = taskbarRect.left, Y = taskbarRect.top };
        PInvoke.ScreenToClient(taskbarHandle, ref pt);

        int posX = posX_screen + pt.X;
        int posY = posY_screen + pt.Y;

        PInvoke.SetWindowPos(thisHandle, HWND.Null, posX, posY, windowWidth, windowHeight, SWP.SWP_NOZORDER | SWP.SWP_NOACTIVATE);
        return true;
    }

    private void MainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        int delta = e.Delta;
        if (delta > 0)
        {
            // KeyPress([KeyDef.Ctrl, KeyDef.LeftWin, KeyDef.LeftArrow]);
            VirtualDesktopManager?.SwitchBackward();

        }
        else
        {
            // KeyPress([KeyDef.Ctrl, KeyDef.LeftWin, KeyDef.RightArrow]);
            VirtualDesktopManager?.SwitchForward();
        }
        NameOfVD.Text = VirtualDesktopManager?.CurrentDisplayName();
    }

    private void MainGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        KeyPress([KeyDef.LeftWin, KeyDef.Tab]);
    }

}