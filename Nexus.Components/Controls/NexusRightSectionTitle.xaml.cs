using System;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Controls;

/// <summary>
/// Nexus pattern — right panel section title bar with a back chevron, a primary
/// title label, and an optional breadcrumb segment that appears when the user
/// has drilled into a sub-level.
/// </summary>
/// <remarks>
/// MVVM usage:
/// <code>
///   &lt;nexus:NexusRightSectionTitle
///       TitleName="{x:Bind ViewModel.SectionTitle}"
///       BreadcrumbTitle="{x:Bind ViewModel.BreadcrumbTitle}"
///       BackCommand="{x:Bind ViewModel.NavigateBackCommand}"/&gt;
/// </code>
/// When <see cref="BreadcrumbTitle"/> is null or empty the "/" separator and the
/// crumb label are both collapsed and <see cref="TitleName"/> renders in
/// <c>text/primary</c>. When non-empty, the title dims to <c>text/secondary</c>
/// and the crumb row becomes visible — driven by the VSM <c>BreadcrumbStates</c>
/// group so that theme-change re-evaluation happens automatically.
/// </remarks>
public sealed partial class NexusRightSectionTitle : UserControl
{
    public NexusRightSectionTitle()
    {
        this.InitializeComponent();
    }

    // ── State properties ──────────────────────────────────────────────────────

    /// <summary>
    /// Primary label displayed to the right of the back chevron. Default: "Section".
    /// </summary>
    public string TitleName
    {
        get => (string)GetValue(TitleNameProperty);
        set => SetValue(TitleNameProperty, value);
    }

    /// <summary>Default: "Section".</summary>
    public static readonly DependencyProperty TitleNameProperty =
        DependencyProperty.Register(
            nameof(TitleName),
            typeof(string),
            typeof(NexusRightSectionTitle),
            new PropertyMetadata("Section"));

    /// <summary>
    /// Secondary crumb label shown after "/" when the user has navigated into a
    /// sub-level. When null or empty, both the "/" separator and this label are
    /// collapsed, and <see cref="TitleName"/> renders at full <c>text/primary</c>
    /// weight. When non-empty, <see cref="TitleName"/> dims to <c>text/secondary</c>
    /// and the crumb row becomes visible. Default: "".
    /// </summary>
    public string BreadcrumbTitle
    {
        get => (string)GetValue(BreadcrumbTitleProperty);
        set => SetValue(BreadcrumbTitleProperty, value);
    }

    /// <summary>Default: "" (NoBreadcrumb state).</summary>
    public static readonly DependencyProperty BreadcrumbTitleProperty =
        DependencyProperty.Register(
            nameof(BreadcrumbTitle),
            typeof(string),
            typeof(NexusRightSectionTitle),
            new PropertyMetadata("", OnBreadcrumbTitleChanged));

    private static void OnBreadcrumbTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((NexusRightSectionTitle)d).UpdateBreadcrumbVisibility();
    }

    private void UpdateBreadcrumbVisibility()
    {
        // Drive the VSM BreadcrumbStates group declared in the XAML.
        // HasBreadcrumb: dims TitleText to text/secondary and reveals "/" + crumb.
        // NoBreadcrumb:  TitleText stays text/primary; crumb row collapsed.
        // Using GoToState (not direct property mutation) so the VSM Setter's
        // ThemeResource values re-evaluate automatically on theme change.
        string state = string.IsNullOrEmpty(BreadcrumbTitle) ? "NoBreadcrumb" : "HasBreadcrumb";
        VisualStateManager.GoToState(this, state, useTransitions: false);
    }

    // ── Command properties ─────────────────────────────────────────────────────

    /// <summary>
    /// Invoked when the user clicks the chevron-left back button.
    /// CommandParameter: <see langword="null"/>.
    /// Bind to an <see cref="ICommand"/> on your ViewModel
    /// (e.g. a CommunityToolkit <c>RelayCommand</c>).
    /// </summary>
    public ICommand? BackCommand
    {
        get => (ICommand?)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    /// <summary>Default: <see langword="null"/>.</summary>
    public static readonly DependencyProperty BackCommandProperty =
        DependencyProperty.Register(
            nameof(BackCommand),
            typeof(ICommand),
            typeof(NexusRightSectionTitle),
            new PropertyMetadata(null));

    // ── Events ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Raised when the user clicks the chevron-left back button.
    /// For ViewModel consumers, prefer binding <see cref="BackCommand"/>.
    /// This event is provided for code-behind consumers and x:Bind event syntax.
    /// </summary>
    public event EventHandler? BackRequested;

    // ── Interaction handlers ───────────────────────────────────────────────────

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke(this, EventArgs.Empty);

        if (BackCommand?.CanExecute(null) is true)
        {
            BackCommand.Execute(null);
        }
    }
}
