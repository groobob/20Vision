using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Reflection.Metadata;
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
    [DllImport("user32.dll")]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WS_EX_APPWINDOW = 0x00040000;

    public MainWindow()
    {
        Instance = this;

        InitializeComponent();
        HideFromEverything();
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
            alert.IsVisible = false;
            if (DateTime.Now.Minute % 52 == 0 && DateTime.Now.Second <= 5)
            {
                alert.IsVisible = true;
            }
        };
        timer.Start();
        alert.IsVisible = false;
    }

    public bool SetClickThrough(bool value)
    {
        var handle = TryGetPlatformHandle()?.Handle;
        if (handle == null) return false;

        int style = GetWindowLong(handle.Value, GWL_EXSTYLE);
        int flags = WS_EX_TRANSPARENT | WS_EX_LAYERED;

        if (value) SetWindowLong(handle.Value, GWL_EXSTYLE, style | flags);
        else SetWindowLong(handle.Value, GWL_EXSTYLE, style & ~(flags));
        return true;
    }

    private void HideFromEverything()
    {
        this.Opened += (s, e) =>
        {
            var handle = TryGetPlatformHandle()?.Handle;
            if (handle == null) return;

            // Get the current style
            int style = GetWindowLong(handle.Value, GWL_EXSTYLE);

            // WS_EX_TOOLWINDOW removes from alt+tab and taskbar
            // & ~WS_EX_APPWINDOW makes sure it doesn't appear in taskbar
            SetWindowLong(handle.Value, GWL_EXSTYLE, style | WS_EX_TOOLWINDOW);
        };
    }

    private void DisableSettingsMenu(object? sender, RoutedEventArgs e)
    {
        SetClickThrough(true);
        SettingsMenu.IsVisible = false;
    }

    public void EnableSettingsMenu()
    {
        SetClickThrough(false);
        SettingsMenu.IsVisible = true;
    }

    private void Exit(object? sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }
}