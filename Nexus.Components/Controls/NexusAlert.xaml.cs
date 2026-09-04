// NexusAlert.xaml.cs — Nexus Pattern — Status alert banner with dismiss button.
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Controls;

/// <summary>Alert severity variants for <see cref="NexusAlert"/>.</summary>
public enum NexusAlertType
{
    /// <summary>Informational message — blue info icon.</summary>
    Information,

    /// <summary>Caution/warning — orange warning icon.</summary>
    Warning,

    /// <summary>Error/critical — red danger icon.</summary>
    Error,
}

/// <summary>
/// Nexus status alert banner — a surface-coloured container carrying a type-adaptive
/// status icon, a message, and a dismiss button. The container always uses the neutral
/// <c>background/surface-raised</c> token; alert severity is communicated exclusively
/// through the icon (colour and shape), not through background tinting.
/// </summary>
/// <remarks>
/// MVVM usage:
/// <code>
///   &lt;nexus:NexusAlert
///       AlertType="{x:Bind ViewModel.AlertType}"
///       Message="{x:Bind ViewModel.AlertMessage}"
///       DismissCommand="{x:Bind ViewModel.DismissAlertCommand}"/&gt;
/// </code>
///
/// The three icon PathIcon elements (InfoIcon, WarningIcon, ErrorIcon) are declared in
/// XAML with their Styles set via <c>{StaticResource}</c>. This is the only correct
/// pattern for PathIcon Data resolution in this SDK version — see the header note in
/// EtherIconGeometries.xaml on Geometry coercion. Code-based Style assignment is
/// deliberately avoided. AlertType changes are implemented as simple Visibility toggles.
/// </remarks>
public sealed partial class NexusAlert : UserControl
{
    public NexusAlert()
    {
        this.InitializeComponent();
    }

    // ── State properties ──────────────────────────────────────────────────────

    /// <summary>
    /// Alert severity that selects which icon is shown.
    /// OneWay — set by the VM or the containing page; not edited by the user directly.
    /// Default: <see cref="NexusAlertType.Error"/>.
    /// </summary>
    /// <remarks>
    /// The container background and border do not change with this property —
    /// they remain <c>background/surface-raised</c> and <c>border/subtle</c> for
    /// all types. Only the icon switches (InfoIcon / WarningIcon / ErrorIcon).
    /// </remarks>
    public NexusAlertType AlertType
    {
        get => (NexusAlertType)GetValue(AlertTypeProperty);
        set => SetValue(AlertTypeProperty, value);
    }

    /// <summary>Backing DependencyProperty for <see cref="AlertType"/>.</summary>
    public static readonly DependencyProperty AlertTypeProperty =
        DependencyProperty.Register(
            nameof(AlertType),
            typeof(NexusAlertType),
            typeof(NexusAlert),
            new PropertyMetadata(NexusAlertType.Error, OnAlertTypeChanged));

    private static void OnAlertTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((NexusAlert)d).ApplyAlertType((NexusAlertType)e.NewValue);

    /// <summary>
    /// Alert message text displayed in the center of the banner.
    /// OneWay — set by the VM. Default: <c>""</c>.
    /// </summary>
    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>Backing DependencyProperty for <see cref="Message"/>.</summary>
    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(
            nameof(Message),
            typeof(string),
            typeof(NexusAlert),
            new PropertyMetadata(string.Empty));

    // ── Command properties ────────────────────────────────────────────────────

    /// <summary>
    /// Invoked when the user clicks the dismiss (×) button.
    /// CommandParameter: <c>null</c>.
    /// Bind to an <see cref="ICommand"/> on your ViewModel (e.g. a CommunityToolkit
    /// RelayCommand). Default: <c>null</c>.
    /// </summary>
    public ICommand? DismissCommand
    {
        get => (ICommand?)GetValue(DismissCommandProperty);
        set => SetValue(DismissCommandProperty, value);
    }

    /// <summary>Backing DependencyProperty for <see cref="DismissCommand"/>.</summary>
    public static readonly DependencyProperty DismissCommandProperty =
        DependencyProperty.Register(
            nameof(DismissCommand),
            typeof(ICommand),
            typeof(NexusAlert),
            new PropertyMetadata(null));

    // ── Events ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Raised when the user clicks the dismiss (×) button.
    /// For ViewModel consumers, prefer binding <see cref="DismissCommand"/>.
    /// This event is provided for code-behind consumers and x:Bind event syntax.
    /// </summary>
    public event EventHandler? Dismissed;

    // ── Interaction handlers ──────────────────────────────────────────────────

    private void DismissButton_Click(object sender, RoutedEventArgs e)
    {
        Dismissed?.Invoke(this, EventArgs.Empty);

        if (DismissCommand?.CanExecute(null) is true)
            DismissCommand.Execute(null);
    }

    // ── Visual update helpers ─────────────────────────────────────────────────

    /// <summary>
    /// Shows the icon matching <paramref name="type"/> and collapses the other two.
    /// The container background and border are always neutral and are not touched here.
    /// </summary>
    private void ApplyAlertType(NexusAlertType type)
    {
        // Guard: x:Name elements are not yet set if called before InitializeComponent.
        if (InfoIcon is null) return;

        InfoIcon.Visibility    = type == NexusAlertType.Information ? Visibility.Visible : Visibility.Collapsed;
        WarningIcon.Visibility = type == NexusAlertType.Warning     ? Visibility.Visible : Visibility.Collapsed;
        ErrorIcon.Visibility   = type == NexusAlertType.Error       ? Visibility.Visible : Visibility.Collapsed;
    }
}
