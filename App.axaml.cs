using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
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

    private void bye(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }

    private void settings(object? sender, EventArgs e)
    {
        MainWindow.Instance?.SetClickThrough(false);
        MainWindow.Instance?.SettingsMenu.IsVisible = true;
    }
}