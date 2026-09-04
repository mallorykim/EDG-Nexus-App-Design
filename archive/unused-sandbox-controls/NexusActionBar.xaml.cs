using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Controls;

/// <summary>
/// Nexus pattern — bottom action bar. The four action buttons are fixed; the
/// battery chip is shown only when <see cref="ShowBattery"/> is true, and
/// <see cref="BatteryPercent"/> feeds the chip.
/// </summary>
public sealed partial class NexusActionBar : UserControl
{
    public NexusActionBar()
    {
        this.InitializeComponent();
        ApplyValues();
    }

    public bool ShowBattery
    {
        get => (bool)GetValue(ShowBatteryProperty);
        set => SetValue(ShowBatteryProperty, value);
    }

    public static readonly DependencyProperty ShowBatteryProperty =
        DependencyProperty.Register(
            nameof(ShowBattery),
            typeof(bool),
            typeof(NexusActionBar),
            new PropertyMetadata(true, OnValueChanged));

    public string BatteryPercent
    {
        get => (string)GetValue(BatteryPercentProperty);
        set => SetValue(BatteryPercentProperty, value);
    }

    public static readonly DependencyProperty BatteryPercentProperty =
        DependencyProperty.Register(
            nameof(BatteryPercent),
            typeof(string),
            typeof(NexusActionBar),
            new PropertyMetadata("84%", OnValueChanged));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NexusActionBar bar)
        {
            bar.ApplyValues();
        }
    }

    private void ApplyValues()
    {
        if (BatteryChip is null)
        {
            return;
        }

        BatteryChip.Visibility = ShowBattery ? Visibility.Visible : Visibility.Collapsed;
        PercentText.Text = BatteryPercent;
    }
}
