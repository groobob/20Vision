using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Runtime.InteropServices;

namespace _20Vision;

public partial class MainWindow : Window
{
    // Singleton
    public static MainWindow? Instance { get; private set; }

    // Win32 API imports
    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int index);
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int WS_EX_LAYERED = 0x00080000;

    public Button SettingsMenu => clickthrough;

    public MainWindow()
    {
        Instance = this;

        InitializeComponent();
        StartCounter();
    }

    private void StartCounter()
    {
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        timer.Tick += (s, e) =>
        {
            counter.Text = DateTime.Now.Second.ToString();
            if (DateTime.Now.Second % 20 == 0)
            {
                counter.Text = "balright";
            }
        };
        timer.Start();
        counter.Text = DateTime.Now.Second.ToString();
    }

    public bool ToggleClickThrough()
    {
        var handle = TryGetPlatformHandle()?.Handle;
        if (handle == null) return false;

        int style = GetWindowLong(handle.Value, GWL_EXSTYLE);
        SetWindowLong(handle.Value, GWL_EXSTYLE, style ^ WS_EX_TRANSPARENT ^ WS_EX_LAYERED);
        return true;
    }

    private void buh(object? sender, RoutedEventArgs e)
    {
        ToggleClickThrough();
        clickthrough.IsVisible = false;
    }
}