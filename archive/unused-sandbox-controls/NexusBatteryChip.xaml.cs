using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Controls;

/// <summary>
/// Nexus pattern — battery indicator chip. <see cref="Percent"/> is the only
/// per-device value. Visibility is controlled by the parent action bar.
/// </summary>
public sealed partial class NexusBatteryChip : UserControl
{
    public NexusBatteryChip()
    {
        this.InitializeComponent();
        ApplyValues();
    }

    public string Percent
    {
        get => (string)GetValue(PercentProperty);
        set => SetValue(PercentProperty, value);
    }

    public static readonly DependencyProperty PercentProperty =
        DependencyProperty.Register(
            nameof(Percent),
            typeof(string),
            typeof(NexusBatteryChip),
            new PropertyMetadata("84%", OnValueChanged));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NexusBatteryChip chip)
        {
            chip.ApplyValues();
        }
    }

    private void ApplyValues()
    {
        if (PercentText is null)
        {
            return;
        }

        PercentText.Text = Percent;
    }
}
