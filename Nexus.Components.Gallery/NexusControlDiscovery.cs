using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml.Controls;
using NexusApp.Gallery.Views;

namespace NexusApp.Gallery;

/// <summary>
/// Reflection-based fallback so a Nexus control is never invisible to the Gallery, even
/// before anyone writes it a hand-written page. Scans the Nexus.Components assembly for
/// every public <c>NexusApp.Controls.*</c> UserControl not already referenced by a
/// <see cref="ComponentCatalog"/> entry's <see cref="ComponentEntry.ControlType"/>, and
/// gives each one a generic entry pointing at the single shared <see cref="AutoControlPage"/>
/// (with the control's <see cref="Type"/> threaded through as the navigation parameter —
/// see <see cref="ComponentEntry.NavigationParameter"/>).
///
/// Hand-written pages always win: this only fills the gap for controls nobody has written a
/// real Gallery page for yet.
/// </summary>
internal static class NexusControlDiscovery
{
    public static IReadOnlyList<ComponentEntry> DiscoverUncataloged(IReadOnlySet<Type> alreadyCataloged)
    {
        var assembly = Assembly.Load("Nexus.Components");

        return assembly.GetTypes()
            .Where(type => type.IsPublic && !type.IsAbstract)
            .Where(type => type.Namespace == "NexusApp.Controls")
            .Where(type => typeof(UserControl).IsAssignableFrom(type))
            .Where(type => !alreadyCataloged.Contains(type))
            .OrderBy(type => type.Name)
            .Select(type => new ComponentEntry(
                Name: SplitPascalCase(type.Name),
                PageType: typeof(AutoControlPage),
                ControlType: type,
                NavigationParameter: type,
                IsAuto: true))
            .ToList();
    }

    /// <summary>"NexusFeatureListRow" → "Nexus Feature List Row", for a readable nav label.
    /// Internal (not private): reused by <see cref="AutoControlPage"/> for the page Title.</summary>
    internal static string SplitPascalCase(string name) =>
        Regex.Replace(name, "(?<!^)([A-Z])", " $1");
}
