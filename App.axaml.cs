using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace _20Vision;

public partial class App : Application
{

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void Exit(object? sender, EventArgs e)
    {
        MainWindow.Instance?.SaveSettings();
        Environment.Exit(0);
    }

    private void EnableSettingsMenu(object? sender, EventArgs e)
    {
        MainWindow.Instance?.EnableSettingsMenu();
    }
}