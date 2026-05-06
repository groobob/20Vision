using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.IO;

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

    public class Settings
    {
        public int alertFrequency { get; set; } = 20;
        public int alertDuration { get; set; } = 10;
    }

    public string settingsPath;
    public Settings settings = new Settings();

    private Button[] frequencyButtons;
    private Button[] durationButtons;

    public MainWindow()
    {
        Instance = this;

        InitializeComponent();
        SetupSettingsInteraction();
        LoadSettings();
        HideFromEverything();
        StartCounter();
    }
    
    public void SaveSettings()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(settingsPath)!);
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions{ WriteIndented = true });
        File.WriteAllText(settingsPath, json);
    }

    private void LoadSettings()
    {
        if (File.Exists(settingsPath))
        {
            string json = File.ReadAllText(settingsPath);
            Settings loadedSettings = JsonSerializer.Deserialize<Settings>(json) ?? new Settings();

            settings.alertFrequency = loadedSettings.alertFrequency;
            settings.alertDuration = loadedSettings.alertDuration;
        }
    }

    private void SetupSettingsInteraction()
    {
        settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "20Vision", "settings.json");

        frequencyButtons = new Button[] { frequencySettingButton1, frequencySettingButton2, frequencySettingButton3 };
        frequencyButtons[0].Click += (s, e) => SetAlertFrequency(15);
        frequencyButtons[1].Click += (s, e) => SetAlertFrequency(20);
        frequencyButtons[2].Click += (s, e) => SetAlertFrequency(30);

        durationButtons = new Button[] { durationSettingButton1, durationSettingButton2, durationSettingButton3 };
        durationButtons[0].Click += (s, e) => SetAlertDuration(5);
        durationButtons[1].Click += (s, e) => SetAlertDuration(10);
        durationButtons[2].Click += (s, e) => SetAlertDuration(20);
    }

    private void SetAlertFrequency(int num)
    {
        settings.alertFrequency = num;
    }

    private void SetAlertDuration(int num)
    {
        settings.alertDuration = num;
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
            if (DateTime.Now.Minute % settings.alertFrequency == 0 && DateTime.Now.Second <= settings.alertDuration)
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
        SaveSettings();
        Environment.Exit(0);
    }
}