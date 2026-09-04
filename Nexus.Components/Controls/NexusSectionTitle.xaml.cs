// NexusSectionTitle.xaml.cs — Nexus Pattern — Left section header control.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Controls;

/// <summary>
/// Nexus left section title — displays a static back chevron alongside a
/// two-line device header: a prominent title and a subdued model name with
/// an inline info icon.
/// </summary>
/// <remarks>
/// Both text properties are OneWay (read from the device/VM, never edited by
/// the user directly). The chevron and info icon are static visual elements.
///
/// MVVM usage:
/// <code>
///   &lt;nexus:NexusSectionTitle
///       TitleName="{x:Bind ViewModel.DeviceTitle}"
///       ModelName="{x:Bind ViewModel.DeviceModel}"/&gt;
/// </code>
/// </remarks>
public sealed partial class NexusSectionTitle : UserControl
{
    public NexusSectionTitle()
    {
        this.InitializeComponent();
    }

    // ── State properties ──────────────────────────────────────────────────────

    /// <summary>
    /// Device or section title displayed in <c>headers/h4</c> style.
    /// Populated from the connected device; not edited by the user directly.
    /// Default: <c>""</c>.
    /// </summary>
    public string TitleName
    {
        get => (string)GetValue(TitleNameProperty);
        set => SetValue(TitleNameProperty, value);
    }

    /// <summary>Backing DependencyProperty for <see cref="TitleName"/>.</summary>
    public static readonly DependencyProperty TitleNameProperty =
        DependencyProperty.Register(
            nameof(TitleName),
            typeof(string),
            typeof(NexusSectionTitle),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Device model identifier displayed in <c>body/m-regular</c> style below
    /// <see cref="TitleName"/>. Populated from the connected device; not edited
    /// by the user directly. Default: <c>""</c>.
    /// </summary>
    public string ModelName
    {
        get => (string)GetValue(ModelNameProperty);
        set => SetValue(ModelNameProperty, value);
    }

    /// <summary>Backing DependencyProperty for <see cref="ModelName"/>.</summary>
    public static readonly DependencyProperty ModelNameProperty =
        DependencyProperty.Register(
            nameof(ModelName),
            typeof(string),
            typeof(NexusSectionTitle),
            new PropertyMetadata(string.Empty));
}
