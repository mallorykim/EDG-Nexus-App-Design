using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Gallery;

public partial class App : Application
{
    public static Window? MainWindow { get; private set; }

    private Window? _window;

    public App()
    {
        this.InitializeComponent();
        this.UnhandledException += OnUnhandledException;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        MainWindow = _window;
        _window.Activate();
    }

    private static async void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        e.Handled = true;   // prevent crash

        var dialog = new ContentDialog
        {
            Title = e.Exception?.GetType().Name ?? "Unknown exception",
            Content = new ScrollViewer
            {
                MaxHeight = 400,
                Content = new TextBlock
                {
                    Text = $"{e.Message}\n\n{e.Exception?.StackTrace}",
                    TextWrapping = TextWrapping.Wrap,
                    IsTextSelectionEnabled = true,
                    FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas"),
                    FontSize = 11,
                }
            },
            CloseButtonText = "OK",
            XamlRoot = MainWindow?.Content?.XamlRoot,
        };

        await dialog.ShowAsync();
    }
}
