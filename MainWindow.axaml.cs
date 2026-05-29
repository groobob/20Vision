using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Win32;

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

        public bool runOnStartup { get; set; } = false;
        public bool displayAlertTimer { get; set; } = false;
        public bool intrusiveAlert { get; set; } = false;
        public bool buh { get; set; } = false;

        public bool isFirstLaunch { get; set; } = true;
    }

    public string settingsPath;
    public Settings settings = new Settings();

    private int tutorialStage = 0;
    private Button[] frequencyButtons;
    private Button[] durationButtons;

    public MainWindow()
    {
        Instance = this;

        InitializeComponent();
        SetupSettingsInteraction();
        LoadSettings();
        WelcomeTutorial();
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
            settings.runOnStartup = loadedSettings.runOnStartup;
            settings.displayAlertTimer = loadedSettings.displayAlertTimer;
            settings.intrusiveAlert = loadedSettings.intrusiveAlert;
            settings.buh = loadedSettings.buh;
            settings.isFirstLaunch = loadedSettings.isFirstLaunch;
        }

        switch (settings.alertFrequency)
        {
            case 15:
                frequencyButtons[0].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
            case 20:
                frequencyButtons[1].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
            case 30:
                frequencyButtons[2].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
        }
        switch (settings.alertDuration)
        {
            case 5:
                durationButtons[0].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
            case 10:
                durationButtons[1].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
            case 20:
                durationButtons[2].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
        }
        alertTimerText.IsVisible = false;
        runOnStartUpCheck.IsChecked = IsStartupEnabled();
        displayAlertTimerCheck.IsChecked = settings.displayAlertTimer;
        intrusiveAlertCheck.IsChecked = settings.intrusiveAlert;
        buhCheck.IsChecked = settings.buh;
    }

    private void WelcomeTutorial()
    {
        if(settings.isFirstLaunch)
        {
            settings.isFirstLaunch = false;

            SetClickThrough(false);

            welcomeTutorial.IsVisible = true;
            tutorialBody1.IsVisible = true;

            tutorialButton1.Click += (s, e) => { ProceedTutorial(); };
            tutorialButton2.Click += (s, e) => { ProceedTutorial(); };
            tutorialButton3.Click += (s, e) => { ProceedTutorial(); };
        }
    }

    private void ProceedTutorial()
    {
        switch (tutorialStage)
        {
            case 0:
            {
                tutorialBody1.IsVisible = false;
                tutorialBody2.IsVisible = true;
                tutorialStage++;
                break;
            }
            case 1:
            {
                tutorialBody2.IsVisible = false;
                tutorialBody3.IsVisible = true;
                tutorialStage++;
                break;
            }
            case 2:
            {
                tutorialBody3.IsVisible = false;
                welcomeTutorial.IsVisible = false;
                SetClickThrough(true);
                SaveSettings();
                break;
            }
        }
    }

    private void SetupSettingsInteraction()
    {
        settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "20Vision", "settings.json");

        frequencyButtons = new Button[] { frequencySettingButton1, frequencySettingButton2, frequencySettingButton3 };
        frequencyButtons[0].Content = "15min";
        frequencyButtons[1].Content = "20min";
        frequencyButtons[2].Content = "30min";
        frequencyButtons[0].Click += (s, e) => SetAlertFrequency(15);
        frequencyButtons[1].Click += (s, e) => SetAlertFrequency(20);
        frequencyButtons[2].Click += (s, e) => SetAlertFrequency(30);

        durationButtons = new Button[] { durationSettingButton1, durationSettingButton2, durationSettingButton3 };
        durationButtons[0].Content = "5sec";
        durationButtons[1].Content = "10sec";
        durationButtons[2].Content = "20sec";
        durationButtons[0].Click += (s, e) => SetAlertDuration(5);
        durationButtons[1].Click += (s, e) => SetAlertDuration(10);
        durationButtons[2].Click += (s, e) => SetAlertDuration(20);

        runOnStartUpCheck.IsCheckedChanged += (s, e) => { UpdateStartup(runOnStartUpCheck.IsChecked == true); };
        displayAlertTimerCheck.IsCheckedChanged += (s, e) => { UpdateAlertTimer(displayAlertTimerCheck.IsChecked == true); };
        intrusiveAlertCheck.IsCheckedChanged += (s, e) => { UpdateIntrusiveAlert(intrusiveAlertCheck.IsChecked == true); };
        buhCheck.IsCheckedChanged += (s, e) => { UpdateBuh(buhCheck.IsChecked == true); };

        SetClickThrough(true);
        settingsMenu.IsVisible = false;
        versionText.IsVisible = false;
    }

    private void UpdateStartup(bool check)
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", writable: true);

        if (key == null) return;

        settings.runOnStartup = check;

        if(check)
        {
            string exePath = Environment.ProcessPath ?? System.Reflection.Assembly.GetExecutingAssembly().Location; key.SetValue("20Vision", $"\"{exePath}\"");
        }
        else
        {
            key.DeleteValue("20Vision", throwOnMissingValue: false);
        }
    }

    public static bool IsStartupEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

        return key?.GetValue("20Vision") != null;
    }

    private void UpdateAlertTimer(bool check)
    {
        settings.displayAlertTimer = check;

        if (check) alertTimerText.IsVisible = true;
        else alertTimerText.IsVisible = false;

        SaveSettings();
    }

    private void UpdateIntrusiveAlert(bool check)
    {
        settings.intrusiveAlert = check;

        var screen = Screens.Primary;

        if (check)
        {
            alert.Width = screen != null ? screen.WorkingArea.Width * 0.9 : 750;
            alert.Height = screen != null ? screen.WorkingArea.Height * 0.9 : 750;
            alert.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            alert.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            alertText.FontSize = screen != null ? screen.WorkingArea.Height * 0.9 : 750;
            alertTimerText.FontSize = screen != null ? screen.WorkingArea.Height * 0.15 : 112.5;
            alertTimerText.Margin = new Avalonia.Thickness(50, 20);

        }
        else
        {
            alert.Width = 100;
            alert.Height = 100;
            alert.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
            alert.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            alertText.FontSize = 100;
            alertTimerText.FontSize = 15;
            alertTimerText.Margin = new Avalonia.Thickness(5, 5);
        }

        SaveSettings();
    }

    private void UpdateBuh(bool check)
    {
        settings.buh = check;

        if (check)
        {
            var uri = new Uri("avares://20Vision/Assets/buh.png");
            alert.Background = new ImageBrush
            {
                Source = new Bitmap(AssetLoader.Open(uri)),
                Stretch = Stretch.Fill,
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center
            };
        }
        else
        {
            alert.Background = new SolidColorBrush(Color.Parse("#fc5656"));
        }

        SaveSettings();
    }

    private void SetAlertFrequency(int num)
    {
        switch (settings.alertFrequency)
        {
            case 15:
                frequencyButtons[0].Background = new SolidColorBrush(Color.Parse("#33000000"));
                break;
            case 20:
                frequencyButtons[1].Background = new SolidColorBrush(Color.Parse("#33000000"));
                break;
            case 30:
                frequencyButtons[2].Background = new SolidColorBrush(Color.Parse("#33000000"));
                break;
        }
        settings.alertFrequency = num;
        switch (num)
        {
            case 15:
                frequencyButtons[0].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
            case 20:
                frequencyButtons[1].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
            case 30:
                frequencyButtons[2].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
        }

        SaveSettings();
    }

    private void SetAlertDuration(int num)
    {
        switch (settings.alertDuration)
        {
            case 5:
                durationButtons[0].Background = new SolidColorBrush(Color.Parse("#33000000"));
                break;
            case 10:
                durationButtons[1].Background = new SolidColorBrush(Color.Parse("#33000000"));
                break;
            case 20:
                durationButtons[2].Background = new SolidColorBrush(Color.Parse("#33000000"));
                break;
        }
        settings.alertDuration = num;
        switch (num)
        {
            case 5:
                durationButtons[0].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
            case 10:
                durationButtons[1].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
            case 20:
                durationButtons[2].Background = new SolidColorBrush(Color.Parse("#FF000000"));
                break;
        }

        SaveSettings();
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
            if (DateTime.Now.Minute % settings.alertFrequency == 0 && DateTime.Now.Second < settings.alertDuration)
            {
                alertTimerText.Text = (settings.alertDuration - DateTime.Now.Second).ToString();
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
        settingsMenu.IsVisible = false;
        versionText.IsVisible = false;
    }

    public void EnableSettingsMenu()
    {
        SetClickThrough(false);
        settingsMenu.IsVisible = true;
        versionText.IsVisible = true;
    }

    private void Exit(object? sender, RoutedEventArgs e)
    {
        SaveSettings();
        Environment.Exit(0);
    }
}