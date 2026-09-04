using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Controls;

/// <summary>
/// Nexus pattern — a single feature-category tile (icon + label + chevron) in
/// the right-hand feature panel. <see cref="Glyph"/> and <see cref="Label"/> are
/// set per device category.
/// </summary>
public sealed partial class NexusFeatureListRow : UserControl
{
    public NexusFeatureListRow()
    {
        this.InitializeComponent();
        ApplyValues();
    }

    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public static readonly DependencyProperty GlyphProperty =
        DependencyProperty.Register(
            nameof(Glyph),
            typeof(string),
            typeof(NexusFeatureListRow),
            new PropertyMetadata("\uE707", OnValueChanged));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(NexusFeatureListRow),
            new PropertyMetadata("Feature name", OnValueChanged));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NexusFeatureListRow row)
        {
            row.ApplyValues();
        }
    }

    private void ApplyValues()
    {
        if (FeatureIcon is null)
        {
            return;
        }

        FeatureIcon.Glyph = Glyph;
        FeatureLabel.Text = Label;
    }
}
