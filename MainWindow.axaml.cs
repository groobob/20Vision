using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace _20Vision;

public partial class MainWindow : Window
{
    // Win32 API imports
    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int index);
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int WS_EX_LAYERED = 0x00080000;

    public MainWindow()
    {
        InitializeComponent();
    }

    private bool MakeClickThrough()
    {
        var handle = TryGetPlatformHandle()?.Handle;
        if (handle == null) return false;

        int style = GetWindowLong(handle.Value, GWL_EXSTYLE);
        SetWindowLong(handle.Value, GWL_EXSTYLE, style | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        return true;
    }

    private void buh(object? sender, RoutedEventArgs e)
    {
        MyTextBlock.Text = "Clicked!";
        MakeClickThrough();
    }
}