using System.Linq;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace NexusApp.Gallery.Views;

/// <summary>
/// Generic fallback page for any Nexus control that doesn't have a hand-written page yet
/// (see <see cref="NexusControlDiscovery"/>). Instantiates the control via its public
/// parameterless constructor, exposes every DependencyProperty it declares itself (not
/// inherited from UserControl/FrameworkElement) whose type is <see cref="string"/>,
/// <see cref="bool"/>, or <see cref="int"/> as a live-editable input, and shows a
/// best-effort copy-paste XAML snippet. One control per navigation — the target
/// <see cref="Type"/> arrives as the Frame.Navigate parameter, not a compile-time page.
/// </summary>
public sealed partial class AutoControlPage : Page
{
    public AutoControlPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is Type controlType)
        {
            try
            {
                Content = Build(controlType);
            }
            catch (Exception ex)
            {
                // Surface the exception as readable text so we can diagnose without crashing.
                Content = new ScrollViewer
                {
                    Padding = new Microsoft.UI.Xaml.Thickness(24),
                    Content = new TextBlock
                    {
                        Text = $"Failed to build auto-page for {controlType.Name}.\n\n"
                             + $"{ex.GetType().FullName}: {ex.Message}\n\n"
                             + ex.StackTrace,
                        TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
                        IsTextSelectionEnabled = true,
                        FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas"),
                        FontSize = 11,
                    }
                };
            }
        }
    }

    private static UIElement Build(Type controlType)
    {
        var instance = (FrameworkElement)Activator.CreateInstance(controlType)!;

        // DeclaredOnly: only the control's own properties, not Width/Height/Name/... that
        // every FrameworkElement already has.
        var editableProperties = controlType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(property => property.CanRead && property.CanWrite && IsEditableType(property.PropertyType))
            .Where(property => controlType.GetField(property.Name + "Property", BindingFlags.Public | BindingFlags.Static) is not null)
            .ToList();

        var interactiveControls = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, VerticalAlignment = VerticalAlignment.Center };
        foreach (var property in editableProperties)
            interactiveControls.Children.Add(BuildEditor(instance, property));

        return new ComponentPage
        {
            Title = NexusControlDiscovery.SplitPascalCase(controlType.Name),
            Description = $"Auto-generated fallback page — {controlType.Name} doesn't have a hand-written Gallery page "
                         + "yet. Add one under Nexus.Components.Gallery\\Views\\NexusControls\\ for a richer demo "
                         + "(see AGENTS.md \u201cNexus component Gallery\u201d).",
            InteractiveContent = new ControlExample
            {
                Example = instance,
                SourceXaml = BuildSourceXaml(controlType, editableProperties),
            },
            InteractiveControls = editableProperties.Count > 0 ? interactiveControls : null,
        };
    }

    private static bool IsEditableType(Type type) =>
        type == typeof(string) || type == typeof(bool) || type == typeof(int);

    private static FrameworkElement BuildEditor(FrameworkElement instance, PropertyInfo property)
    {
        var label = new TextBlock
        {
            Text = property.Name,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 12,
            Foreground = (Brush)Application.Current.Resources["TextSecondary"],
        };

        FrameworkElement input = property.PropertyType == typeof(bool)
            ? BuildCheckBoxEditor(instance, property)
            : BuildTextBoxEditor(instance, property);

        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            VerticalAlignment = VerticalAlignment.Center,
            Children = { label, input },
        };
    }

    private static CheckBox BuildCheckBoxEditor(FrameworkElement instance, PropertyInfo property)
    {
        var check = new CheckBox { IsChecked = (bool)(property.GetValue(instance) ?? false) };
        check.Checked += (_, _) => property.SetValue(instance, true);
        check.Unchecked += (_, _) => property.SetValue(instance, false);
        return check;
    }

    private static TextBox BuildTextBoxEditor(FrameworkElement instance, PropertyInfo property)
    {
        var textBox = new TextBox { Text = property.GetValue(instance)?.ToString() ?? string.Empty, Width = 100 };
        textBox.TextChanged += (_, _) =>
        {
            if (property.PropertyType == typeof(int))
            {
                if (int.TryParse(textBox.Text, out var parsed))
                    property.SetValue(instance, parsed);
            }
            else
            {
                property.SetValue(instance, textBox.Text);
            }
        };
        return textBox;
    }

    private static string BuildSourceXaml(Type controlType, IReadOnlyList<PropertyInfo> properties)
    {
        if (properties.Count == 0)
            return $"<nexus:{controlType.Name}/>";

        var attributes = string.Join(" ", properties.Select(property => $"{property.Name}=\"...\""));
        return $"<nexus:{controlType.Name} {attributes}/>";
    }
}
