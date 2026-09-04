using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Controls;

/// <summary>
/// Nexus pattern — device identity block (back chevron + title, model + info).
/// Fixed layout; <see cref="DeviceName"/> and <see cref="ModelText"/> are the
/// only per-device values. <see cref="ShowBackButton"/> hides the chevron on
/// root screens.
/// </summary>
public sealed partial class NexusDeviceHeader : UserControl
{
    public NexusDeviceHeader()
    {
        this.InitializeComponent();
        ApplyValues();
    }

    public string DeviceName
    {
        get => (string)GetValue(DeviceNameProperty);
        set => SetValue(DeviceNameProperty, value);
    }

    public static readonly DependencyProperty DeviceNameProperty =
        DependencyProperty.Register(
            nameof(DeviceName),
            typeof(string),
            typeof(NexusDeviceHeader),
            new PropertyMetadata("Dell Peripherals name", OnValueChanged));

    public string ModelText
    {
        get => (string)GetValue(ModelTextProperty);
        set => SetValue(ModelTextProperty, value);
    }

    public static readonly DependencyProperty ModelTextProperty =
        DependencyProperty.Register(
            nameof(ModelText),
            typeof(string),
            typeof(NexusDeviceHeader),
            new PropertyMetadata("Peripherals model", OnValueChanged));

    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    public static readonly DependencyProperty ShowBackButtonProperty =
        DependencyProperty.Register(
            nameof(ShowBackButton),
            typeof(bool),
            typeof(NexusDeviceHeader),
            new PropertyMetadata(true, OnValueChanged));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NexusDeviceHeader header)
        {
            header.ApplyValues();
        }
    }

    private void ApplyValues()
    {
        if (TitleText is null)
        {
            return;
        }

        TitleText.Text = DeviceName;
        ModelTextBlock.Text = ModelText;
        BackButton.Visibility = ShowBackButton ? Visibility.Visible : Visibility.Collapsed;
    }
}
