using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace NexusApp;

public sealed partial class MainWindow : Window
{
    private bool _isDark = false;
    private bool _isSyncingSelection = false;

    public MainWindow()
    {
        this.InitializeComponent();
        ResizeToDesignCanvas();
        BuildNavigation();
        ContentFrame.Navigate(ScreenCatalog.Home.PageType);
    }

    // ── Design canvas size ───────────────────────────────────────────────────

    /// <summary>
    /// Design canvas the gallery mirrors — 1440 × 900 matches a standard laptop display.
    /// Adjust if the Nexus Phase I Figma uses a different artboard size.
    /// </summary>
    private const double DesignCanvasWidth = 1440;
    private const double DesignCanvasHeight = 900;

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(nint hwnd);

    private void ResizeToDesignCanvas()
    {
        var handle = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var dpi = GetDpiForWindow(handle);
        var scale = dpi == 0 ? 1d : dpi / 96d;

        AppWindow.ResizeClient(new Windows.Graphics.SizeInt32(
            (int)Math.Round(DesignCanvasWidth * scale),
            (int)Math.Round(DesignCanvasHeight * scale)));
    }

    // ── Navigation build ─────────────────────────────────────────────────────

    /// <summary>
    /// Builds the NavigationView pane from ScreenCatalog. Structure:
    ///   Home (icon item)
    ///   ── KEYBOARD (header)
    ///      Base — Connected (leaf item)
    ///      Base — Disconnected (leaf item)
    ///   ── MOUSE (header)
    ///      ...
    /// Only peripheral groups with at least one screen are shown.
    /// </summary>
    private void BuildNavigation()
    {
        // Home item
        var homeItem = new NavigationViewItem
        {
            Content = ScreenCatalog.Home.Name,
            Tag = ScreenCatalog.Home.PageType,
            Icon = new SymbolIcon(Symbol.Home),
        };
        NavView.MenuItems.Add(homeItem);

        // One header + leaf items per active peripheral group
        foreach (var group in ScreenCatalog.ActivePeripherals)
        {
            NavView.MenuItems.Add(new NavigationViewItemHeader
            {
                Content = group.Peripheral.ToUpperInvariant(),
                FontSize = 11,
                CharacterSpacing = 80,
                Margin = new Thickness(0, 12, 0, 0),
                Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextBrand"],
            });

            foreach (var screen in group.Screens)
            {
                NavView.MenuItems.Add(new NavigationViewItem
                {
                    Content = screen.Name,
                    Tag = screen.PageType,
                });
            }
        }

        NavView.SelectedItem = homeItem;
    }

    // ── Selection / navigation sync ──────────────────────────────────────────

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (_isSyncingSelection) return;
        if (args.SelectedItem is NavigationViewItem { Tag: Type pageType } &&
            ContentFrame.CurrentSourcePageType != pageType)
        {
            ContentFrame.Navigate(pageType);
        }
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        foreach (var item in NavView.MenuItems)
        {
            if (item is NavigationViewItem { Tag: Type pageType } navItem && pageType == e.SourcePageType)
            {
                if (!ReferenceEquals(NavView.SelectedItem, navItem))
                {
                    _isSyncingSelection = true;
                    NavView.SelectedItem = navItem;
                    _isSyncingSelection = false;
                }
                break;
            }
        }
    }

    // ── Theme toggle ─────────────────────────────────────────────────────────

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        _isDark = !_isDark;
        RootGrid.RequestedTheme = _isDark ? ElementTheme.Dark : ElementTheme.Light;
        ThemeToggle.Content = _isDark ? "🌙 Dark" : "☀ Light";
    }

    /// <summary>
    /// Called by GalleryHomePage cards to navigate the shell's Frame without
    /// going through the NavigationView selection handler.
    /// </summary>
    public void NavigateTo(Type pageType) => ContentFrame.Navigate(pageType);

    private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception($"Navigation failed to page '{e.SourcePageType.FullName}': {e.Exception.Message}");
    }
}
