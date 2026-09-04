using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Controls;

/// <summary>
/// Nexus pattern — fixed global masthead action strip (Updates / Add / Settings /
/// window caption). The only dynamic value is <see cref="UpdateCount"/>.
/// </summary>
public sealed partial class NexusTopBar : UserControl
{
    public NexusTopBar()
    {
        this.InitializeComponent();
        UpdateBadge();
    }

    public int UpdateCount
    {
        get => (int)GetValue(UpdateCountProperty);
        set => SetValue(UpdateCountProperty, value);
    }

    public static readonly DependencyProperty UpdateCountProperty =
        DependencyProperty.Register(
            nameof(UpdateCount),
            typeof(int),
            typeof(NexusTopBar),
            new PropertyMetadata(3, OnUpdateCountChanged));

    private static void OnUpdateCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NexusTopBar bar)
        {
            bar.UpdateBadge();
        }
    }

    private void UpdateBadge()
    {
        if (UpdatesBadge is null)
        {
            return;
        }

        UpdatesBadge.Visibility = UpdateCount > 0 ? Visibility.Visible : Visibility.Collapsed;
        UpdatesBadgeText.Text = UpdateCount.ToString();
    }
}
