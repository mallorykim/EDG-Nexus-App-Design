using Microsoft.UI.Xaml;

namespace NexusApp;

public partial class App : Application
{
    public static Window? MainWindow { get; private set; }

    private Window? _window;

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        MainWindow = _window;
        _window.Activate();
    }
}
