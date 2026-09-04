using Microsoft.UI.Xaml.Controls;

namespace NexusApp;

public sealed partial class GalleryHomePage : Page
{
    public GalleryHomePage()
    {
        this.InitializeComponent();
        // Only show groups that have at least one screen registered in the catalog
        GroupsRepeater.ItemsSource = ScreenCatalog.ActivePeripherals.ToList();
    }

    /// <summary>
    /// Navigates the gallery shell's ContentFrame to the screen whose Page type
    /// is stored in the clicked card's Tag.
    /// </summary>
    private void Card_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button { Tag: Type pageType } &&
            App.MainWindow is MainWindow mainWindow)
        {
            mainWindow.NavigateTo(pageType);
        }
    }
}
