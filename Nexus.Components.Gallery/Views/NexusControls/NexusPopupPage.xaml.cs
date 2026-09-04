using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using NexusApp.Controls;

namespace NexusApp.Gallery.Views.NexusControls;

/// <summary>
/// Hand-written Gallery page for <see cref="NexusPopup"/>. All content is built
/// programmatically to avoid the WinUI 3 x:Name-scope issue with nested UserControl
/// content properties (see NexusPopupPage.xaml comment for details).
/// </summary>
public sealed partial class NexusPopupPage : Page
{
    public NexusPopupPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        Content = Build();
    }

    private UIElement Build()
    {
        // ── Specimen ──────────────────────────────────────────────────────────
        var specimen = new NexusPopup
        {
            Width = 420,
            HorizontalAlignment = HorizontalAlignment.Left,
            Title = "Popup title",
            PrimaryButtonText = "Primary action",
            SecondaryButtonText = "Secondary action",
            IsPrimaryButtonVisible = true,
            IsSecondaryButtonVisible = true,
            BodyContent = new TextBlock
            {
                Text = "This middle region is read-only content supplied by the consumer — "
                     + "anything from a single line of copy to a full stack of rows.",
                Style = (Style)Application.Current.Resources["body/m-regular"],
                Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["text/secondary"],
                TextWrapping = TextWrapping.Wrap,
            },
        };

        // Plain C# events cannot be wired from XAML on cross-assembly controls;
        // subscribe in code-behind.
        specimen.CloseButtonClick += async (_, _) =>
            await ShowFeedbackAsync("Close (X) clicked.", specimen);
        specimen.PrimaryButtonClick += async (_, _) =>
            await ShowFeedbackAsync("Primary button clicked.", specimen);
        specimen.SecondaryButtonClick += async (_, _) =>
            await ShowFeedbackAsync("Secondary button clicked.", specimen);

        // ── Interactive controls ───────────────────────────────────────────────
        var titleBox = MakeTextBox("Title", specimen.Title, 140,
            v => specimen.Title = v);
        var primaryTextBox = MakeTextBox("Primary text", specimen.PrimaryButtonText, 120,
            v => specimen.PrimaryButtonText = v);
        var secondaryTextBox = MakeTextBox("Secondary text", specimen.SecondaryButtonText, 120,
            v => specimen.SecondaryButtonText = v);

        var primaryVisibleCheck = new CheckBox
        {
            Content = "Primary visible",
            IsChecked = specimen.IsPrimaryButtonVisible,
        };
        primaryVisibleCheck.Checked += (_, _) => specimen.IsPrimaryButtonVisible = true;
        primaryVisibleCheck.Unchecked += (_, _) => specimen.IsPrimaryButtonVisible = false;

        var secondaryVisibleCheck = new CheckBox
        {
            Content = "Secondary visible",
            IsChecked = specimen.IsSecondaryButtonVisible,
        };
        secondaryVisibleCheck.Checked += (_, _) => specimen.IsSecondaryButtonVisible = true;
        secondaryVisibleCheck.Unchecked += (_, _) => specimen.IsSecondaryButtonVisible = false;

        var interactiveControls = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 16,
            VerticalAlignment = VerticalAlignment.Center,
        };
        interactiveControls.Children.Add(titleBox);
        interactiveControls.Children.Add(primaryTextBox);
        interactiveControls.Children.Add(secondaryTextBox);
        interactiveControls.Children.Add(primaryVisibleCheck);
        interactiveControls.Children.Add(secondaryVisibleCheck);

        // ── Page chrome ────────────────────────────────────────────────────────
        return new ComponentPage
        {
            Title = "Nexus Popup",
            Description = "An in-app popup card: a title bar with a dismiss (X), "
                        + "a read-only middle content region shaped by the consumer, "
                        + "and an optional primary/secondary action bar.",
            InteractiveContent = new ControlExample
            {
                Example = specimen,
                SourceXaml =
                    "<nexus:NexusPopup\n" +
                    "    Title=\"{x:Bind ViewModel.Title}\"\n" +
                    "    BodyContent=\"{x:Bind ViewModel.BodyContent}\"\n" +
                    "    PrimaryButtonText=\"{x:Bind ViewModel.PrimaryButtonText}\"\n" +
                    "    PrimaryButtonCommand=\"{x:Bind ViewModel.PrimaryCommand}\"\n" +
                    "    SecondaryButtonText=\"{x:Bind ViewModel.SecondaryButtonText}\"\n" +
                    "    SecondaryButtonCommand=\"{x:Bind ViewModel.SecondaryCommand}\"\n" +
                    "    CloseCommand=\"{x:Bind ViewModel.CloseCommand}\"/>",
            },
            InteractiveControls = interactiveControls,
        };
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static StackPanel MakeTextBox(string label, string initial, double width,
        Action<string> onChange)
    {
        var box = new TextBox { Width = width, Text = initial };
        box.TextChanged += (s, _) => onChange(((TextBox)s).Text);

        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            VerticalAlignment = VerticalAlignment.Center,
        };
        panel.Children.Add(new TextBlock
        {
            Text = label,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 12,
        });
        panel.Children.Add(box);
        return panel;
    }

    private async System.Threading.Tasks.Task ShowFeedbackAsync(string message, NexusPopup specimen)
    {
        var dialog = new ContentDialog
        {
            Title = "Nexus Popup",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = specimen.XamlRoot ?? this.XamlRoot,
        };
        await dialog.ShowAsync();
    }
}
