# Nexus Control Template — MVVM-Compatible WinUI 3 UserControl

Copy this template when creating a new Nexus control. Replace every occurrence of
`NexusMyControl` with your control name. Delete sections that don't apply.

---

## Why these rules?

Nexus controls are consumed by ViewModels via data binding. A control is
**MVVM-compatible** when:

- All state the VM drives is a **`DependencyProperty`** (supports `{Binding}` and `x:Bind`)
- All actions the VM triggers are **`ICommand` DependencyProperties** (null-safe `.Execute()`,
  same pattern as WinUI's built-in `Button.Command`)
- **No business logic** lives in the control — only visual/interaction behavior
- The control works equally well with a ViewModel or with plain literal values
  in a design-time/gallery page

---

## File: `Controls/NexusMyControl.xaml`

```xml
<!--
    NexusMyControl.xaml — Nexus Pattern — [One-sentence description]

    BINDABLE STATE
      Prop1  — [what it controls visually]
      Prop2  — [what it controls visually]

    COMMANDS
      PrimaryCommand   — invoked when the user triggers the primary action;
                         CommandParameter = [what value is passed]

    EVENTS (optional, for two-way / data-surfacing scenarios)
      Prop2Changed — raised when the user changes Prop2; args carry the new value

    Figma: [file key] node [node id]
-->
<UserControl
    x:Class="NexusApp.Controls.NexusMyControl"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!--
        IMPORTANT: Do NOT reference other NexusApp.Controls UserControls here
        via <nexus:OtherControl>. The WinUI3 XAML toolchain cannot handle
        intra-library UserControl cross-references in a ProjectReference context
        (produces WMC0610). Inline shared visuals or compose in code-behind.
    -->

    <Grid>
        <!-- Visual tree here. Use only ThemeResource / StaticResource tokens
             from the Ether design system (EtherColors, EtherSpacing, etc.).
             Never hardcode colours or spacing values. -->

        <Button x:Name="PrimaryButton"
                Click="PrimaryButton_Click"
                Content="{x:Bind Prop1, Mode=OneWay}"
                AutomationProperties.Name="[Accessible name]"/>
    </Grid>
</UserControl>
```

---

## File: `Controls/NexusMyControl.xaml.cs`

```csharp
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Controls;

/// <summary>
/// Nexus pattern — [one-sentence summary].
/// </summary>
/// <remarks>
/// MVVM usage:
/// <code>
///   &lt;nexus:NexusMyControl
///       Prop1="{x:Bind ViewModel.Value1}"
///       Prop2="{x:Bind ViewModel.Value2, Mode=TwoWay}"
///       PrimaryCommand="{x:Bind ViewModel.PrimaryCommand}"/&gt;
/// </code>
/// </remarks>
public sealed partial class NexusMyControl : UserControl
{
    public NexusMyControl()
    {
        this.InitializeComponent();
    }

    // ── State properties ─────────────────────────────────────────────────────

    /// <summary>[Description of what this state drives visually.]</summary>
    public string Prop1
    {
        get => (string)GetValue(Prop1Property);
        set => SetValue(Prop1Property, value);
    }

    public static readonly DependencyProperty Prop1Property =
        DependencyProperty.Register(
            nameof(Prop1),
            typeof(string),
            typeof(NexusMyControl),
            new PropertyMetadata("Default value", OnProp1Changed));

    private static void OnProp1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Update any visual elements that can't be handled by x:Bind alone.
        // Keep this purely visual — no business logic.
    }

    // ── Command properties ────────────────────────────────────────────────────

    /// <summary>
    /// Invoked when the user triggers the primary action.
    /// CommandParameter: [describe what value is passed, e.g. the current Prop2 value].
    /// Bind to an ICommand on your ViewModel (e.g. a CommunityToolkit RelayCommand).
    /// </summary>
    public ICommand? PrimaryCommand
    {
        get => (ICommand?)GetValue(PrimaryCommandProperty);
        set => SetValue(PrimaryCommandProperty, value);
    }

    public static readonly DependencyProperty PrimaryCommandProperty =
        DependencyProperty.Register(
            nameof(PrimaryCommand),
            typeof(ICommand),
            typeof(NexusMyControl),
            new PropertyMetadata(null));

    // ── Events (optional — add when the control surfaces data back) ───────────

    /// <summary>
    /// Raised when the user changes <see cref="Prop2"/> interactively.
    /// For ViewModel consumers, prefer binding <see cref="Prop2"/> two-way.
    /// This event is provided for code-behind consumers and x:Bind event syntax.
    /// </summary>
    public event EventHandler<string>? Prop2Changed;

    // ── Interaction handlers ──────────────────────────────────────────────────

    private void PrimaryButton_Click(object sender, RoutedEventArgs e)
    {
        // Execute the command if one is bound; pass a meaningful parameter.
        // The null-safe pattern matches WinUI's own Button.Command behavior.
        if (PrimaryCommand?.CanExecute(Prop1) is true)
        {
            PrimaryCommand.Execute(Prop1);
        }
    }
}
```

---

## ViewModel side (consumer example — lives in the app, not this library)

```csharp
// Using CommunityToolkit.Mvvm (already available via the app project)
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MyPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _value1 = "Hello";

    [RelayCommand]
    private void Primary(string parameter)
    {
        // Handle the action — no UI code here.
    }
}
```

---

## Checklist before committing a new control

- [ ] Every piece of state the VM could set is a `DependencyProperty`
- [ ] Every action the VM triggers is an `ICommand` DependencyProperty
- [ ] Command invocation uses the null-safe `?.CanExecute() / .Execute()` pattern
- [ ] No `EtherSandbox.*` namespace references — use `Ether.DesignSystem.Controls`
- [ ] No other `NexusApp.Controls` UserControl referenced inline via XAML (inline the visual)
- [ ] All colours/spacing/typography use Ether `{ThemeResource}` or `{StaticResource}` tokens
- [ ] XML doc comment on the class shows a binding usage example
- [ ] Control works with literal values (no VM required) for gallery/design-time use
- [ ] Shows up in the Gallery — new controls appear automatically via reflection
      (`NexusControlDiscovery.cs`), but consider adding a real page under
      `Nexus.Components.Gallery\Views\NexusControls\` (see
      `.claude\skills\build-nexus-control\SKILL.md` Step 6) once the API is stable
