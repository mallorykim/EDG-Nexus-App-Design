using System;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Controls;

/// <summary>
/// Nexus pattern — an in-app popup card with a title bar (title + dismiss),
/// a read-only middle content region, and an optional primary/secondary action bar.
/// </summary>
/// <remarks>
/// MVVM usage:
/// <code>
///   &lt;nexus:NexusPopup
///       Title="{x:Bind ViewModel.Title}"
///       BodyContent="{x:Bind ViewModel.BodyContent}"
///       PrimaryButtonText="{x:Bind ViewModel.PrimaryButtonText}"
///       PrimaryButtonCommand="{x:Bind ViewModel.PrimaryCommand}"
///       SecondaryButtonText="{x:Bind ViewModel.SecondaryButtonText}"
///       SecondaryButtonCommand="{x:Bind ViewModel.SecondaryCommand}"
///       CloseCommand="{x:Bind ViewModel.CloseCommand}"/&gt;
/// </code>
/// The popup itself only renders the card surface — placement/visibility (a
/// <c>ContentDialog</c>, a flyout, or an overlay <c>Grid</c>) is owned by the
/// consuming page, not by this control.
/// </remarks>
public sealed partial class NexusPopup : UserControl
{
    public NexusPopup()
    {
        this.InitializeComponent();
        UpdateActionBarVisibility();
    }

    // ── State properties ─────────────────────────────────────────────────────

    /// <summary>Popup title text shown at the top-left of the title bar. Default: "Popup title".</summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(NexusPopup),
            new PropertyMetadata("Popup title"));

    /// <summary>
    /// Arbitrary read-only content hosted in the popup's middle region — shaped
    /// entirely by the consumer (e.g. a TextBlock, a StackPanel of rows). Default: <see langword="null"/>.
    /// </summary>
    public object? BodyContent
    {
        get => GetValue(BodyContentProperty);
        set => SetValue(BodyContentProperty, value);
    }

    public static readonly DependencyProperty BodyContentProperty =
        DependencyProperty.Register(
            nameof(BodyContent),
            typeof(object),
            typeof(NexusPopup),
            new PropertyMetadata(null));

    /// <summary>Label for the right-hand primary action button. Default: "Primary action".</summary>
    public string PrimaryButtonText
    {
        get => (string)GetValue(PrimaryButtonTextProperty);
        set => SetValue(PrimaryButtonTextProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonTextProperty =
        DependencyProperty.Register(
            nameof(PrimaryButtonText),
            typeof(string),
            typeof(NexusPopup),
            new PropertyMetadata("Primary action"));

    /// <summary>Shows or hides the primary action button. Default: <see langword="true"/>.</summary>
    public bool IsPrimaryButtonVisible
    {
        get => (bool)GetValue(IsPrimaryButtonVisibleProperty);
        set => SetValue(IsPrimaryButtonVisibleProperty, value);
    }

    public static readonly DependencyProperty IsPrimaryButtonVisibleProperty =
        DependencyProperty.Register(
            nameof(IsPrimaryButtonVisible),
            typeof(bool),
            typeof(NexusPopup),
            new PropertyMetadata(true, OnButtonVisibilityChanged));

    /// <summary>Label for the left-hand secondary action button. Default: "Secondary action".</summary>
    public string SecondaryButtonText
    {
        get => (string)GetValue(SecondaryButtonTextProperty);
        set => SetValue(SecondaryButtonTextProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonTextProperty =
        DependencyProperty.Register(
            nameof(SecondaryButtonText),
            typeof(string),
            typeof(NexusPopup),
            new PropertyMetadata("Secondary action"));

    /// <summary>Shows or hides the secondary action button. Default: <see langword="true"/>.</summary>
    public bool IsSecondaryButtonVisible
    {
        get => (bool)GetValue(IsSecondaryButtonVisibleProperty);
        set => SetValue(IsSecondaryButtonVisibleProperty, value);
    }

    public static readonly DependencyProperty IsSecondaryButtonVisibleProperty =
        DependencyProperty.Register(
            nameof(IsSecondaryButtonVisible),
            typeof(bool),
            typeof(NexusPopup),
            new PropertyMetadata(true, OnButtonVisibilityChanged));

    private static void OnButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Collapse the whole action bar (not just the individual buttons) when neither
        // is visible, so a purely informational popup doesn't reserve an empty action row.
        ((NexusPopup)d).UpdateActionBarVisibility();
    }

    private void UpdateActionBarVisibility()
    {
        if (ActionBar is null) return;
        ActionBar.Visibility = (IsPrimaryButtonVisible || IsSecondaryButtonVisible)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    // ── Command properties ────────────────────────────────────────────────────

    /// <summary>
    /// Invoked when the user clicks the primary action button.
    /// CommandParameter: <see cref="PrimaryButtonCommandParameter"/>.
    /// Bind to an ICommand on your ViewModel (e.g. a CommunityToolkit RelayCommand).
    /// </summary>
    public ICommand? PrimaryButtonCommand
    {
        get => (ICommand?)GetValue(PrimaryButtonCommandProperty);
        set => SetValue(PrimaryButtonCommandProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonCommandProperty =
        DependencyProperty.Register(
            nameof(PrimaryButtonCommand),
            typeof(ICommand),
            typeof(NexusPopup),
            new PropertyMetadata(null));

    /// <summary>Optional parameter passed to <see cref="PrimaryButtonCommand"/>. Default: <see langword="null"/>.</summary>
    public object? PrimaryButtonCommandParameter
    {
        get => GetValue(PrimaryButtonCommandParameterProperty);
        set => SetValue(PrimaryButtonCommandParameterProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonCommandParameterProperty =
        DependencyProperty.Register(
            nameof(PrimaryButtonCommandParameter),
            typeof(object),
            typeof(NexusPopup),
            new PropertyMetadata(null));

    /// <summary>
    /// Invoked when the user clicks the secondary action button.
    /// CommandParameter: <see cref="SecondaryButtonCommandParameter"/>.
    /// Bind to an ICommand on your ViewModel (e.g. a CommunityToolkit RelayCommand).
    /// </summary>
    public ICommand? SecondaryButtonCommand
    {
        get => (ICommand?)GetValue(SecondaryButtonCommandProperty);
        set => SetValue(SecondaryButtonCommandProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonCommandProperty =
        DependencyProperty.Register(
            nameof(SecondaryButtonCommand),
            typeof(ICommand),
            typeof(NexusPopup),
            new PropertyMetadata(null));

    /// <summary>Optional parameter passed to <see cref="SecondaryButtonCommand"/>. Default: <see langword="null"/>.</summary>
    public object? SecondaryButtonCommandParameter
    {
        get => GetValue(SecondaryButtonCommandParameterProperty);
        set => SetValue(SecondaryButtonCommandParameterProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonCommandParameterProperty =
        DependencyProperty.Register(
            nameof(SecondaryButtonCommandParameter),
            typeof(object),
            typeof(NexusPopup),
            new PropertyMetadata(null));

    /// <summary>
    /// Invoked when the user clicks the title bar's dismiss (X) button.
    /// Bind to an ICommand on your ViewModel (e.g. a CommunityToolkit RelayCommand)
    /// that closes or hides the popup's host (a ContentDialog, flyout, or overlay).
    /// </summary>
    public ICommand? CloseCommand
    {
        get => (ICommand?)GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    public static readonly DependencyProperty CloseCommandProperty =
        DependencyProperty.Register(
            nameof(CloseCommand),
            typeof(ICommand),
            typeof(NexusPopup),
            new PropertyMetadata(null));

    // ── Events (for code-behind consumers; ViewModels should prefer the commands above) ──

    /// <summary>Raised when the user clicks the primary action button.</summary>
    public event EventHandler? PrimaryButtonClick;

    /// <summary>Raised when the user clicks the secondary action button.</summary>
    public event EventHandler? SecondaryButtonClick;

    /// <summary>Raised when the user clicks the title bar's dismiss (X) button.</summary>
    public event EventHandler? CloseButtonClick;

    // ── Interaction handlers ──────────────────────────────────────────────────

    private void PrimaryButton_Click(object sender, RoutedEventArgs e)
    {
        PrimaryButtonClick?.Invoke(this, EventArgs.Empty);

        if (PrimaryButtonCommand?.CanExecute(PrimaryButtonCommandParameter) is true)
        {
            PrimaryButtonCommand.Execute(PrimaryButtonCommandParameter);
        }
    }

    private void SecondaryButton_Click(object sender, RoutedEventArgs e)
    {
        SecondaryButtonClick?.Invoke(this, EventArgs.Empty);

        if (SecondaryButtonCommand?.CanExecute(SecondaryButtonCommandParameter) is true)
        {
            SecondaryButtonCommand.Execute(SecondaryButtonCommandParameter);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        CloseButtonClick?.Invoke(this, EventArgs.Empty);

        if (CloseCommand?.CanExecute(null) is true)
        {
            CloseCommand.Execute(null);
        }
    }
}
