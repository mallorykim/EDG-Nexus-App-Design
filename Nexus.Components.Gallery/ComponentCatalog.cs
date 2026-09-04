using System.Linq;
using NexusApp.Gallery.Views;
using NexusApp.Gallery.Views.NexusControls;

namespace NexusApp.Gallery;

/// <summary>
/// One entry in the catalog: a display name, the Page type that shows it, whether it
/// has been reworked in its own branch (<see cref="IsUpdated"/>, surfaced as an "UPDATED"
/// badge in the nav, the Home cards and the component page), and whether it's a raw
/// primitive-token page rather than a component (<see cref="IsPrimitive"/>, surfaced the
/// same way as a "PRIMITIVE TOKEN" badge).
///
/// <see cref="ControlType"/> and <see cref="NavigationParameter"/> exist for the reflection
/// fallback (<see cref="NexusControlDiscovery"/>): a hand-written entry sets
/// <see cref="ControlType"/> so discovery knows the control already has a page and skips it;
/// an auto-discovered entry reuses a single generic page type
/// (<see cref="AutoControlPage"/>) for every uncataloged control and threads the target
/// control <see cref="Type"/> through as <see cref="NavigationParameter"/> since one Page
/// type now serves many nav items.
/// </summary>
public sealed record ComponentEntry(
    string Name,
    Type PageType,
    Type? ControlType = null,
    object? NavigationParameter = null,
    bool IsAiFamily = false,
    bool IsUpdated = false,
    bool IsPrimitive = false,
    bool IsAuto = false)
{
    /// <summary>Localized nav/home label. <see cref="Name"/> stays the English fallback key.
    /// Auto-discovered entries get an " (auto)" suffix so it's obvious at a glance that no
    /// hand-written Gallery page exists yet for that control.</summary>
    public string DisplayName =>
        GalleryStrings.Get(GalleryStrings.CatalogKey(Name), Name) + (IsAuto ? " (auto)" : string.Empty);
}

/// <summary>Base type for a top-level slot in the navigation tree.</summary>
public abstract record CatalogNode;

/// <summary>A category header (e.g. "Nexus Controls") with its component pages underneath it.</summary>
public sealed record CatalogCategory(string Header, IReadOnlyList<ComponentEntry> Items) : CatalogNode
{
    /// <summary>Localized category header. <see cref="Header"/> stays the English fallback key.</summary>
    public string DisplayHeader => GalleryStrings.Get(GalleryStrings.CatalogKey(Header), Header);
}

/// <summary>
/// A single top-level entry with no header of its own — it sits alongside the category
/// headers rather than inside one.
/// </summary>
public sealed record CatalogLeaf(ComponentEntry Entry) : CatalogNode;

/// <summary>
/// Single source of truth for the gallery's navigation tree: category → component name →
/// page Type. Both the NavigationView's menu items (MainWindow) and the Home page index
/// are generated from <see cref="Nodes"/>, so they cannot drift from each other.
///
/// Every Nexus control shows up here one way or another:
/// - Hand-written entries (<see cref="ManualNexusControlEntries"/>) point at a real page
///   under Views/NexusControls/ with a proper description and tuned property editors.
/// - Anything in Nexus.Components NOT listed there is picked up automatically by
///   <see cref="NexusControlDiscovery"/> and given a generic reflection-driven page — so a
///   brand-new control is never invisible to the Gallery, even before anyone writes it a
///   real page (see AGENTS.md "Nexus component Gallery").
/// </summary>
public static class ComponentCatalog
{
    /// <summary>The index/landing page. Not part of <see cref="Nodes"/> — it is always first.</summary>
    public static ComponentEntry Home { get; } = new("Home", typeof(HomePage));

    /// <summary>Hand-written Gallery pages. Add a line here (plus the page file under
    /// Views/NexusControls/) whenever a control deserves a proper demo — otherwise it still
    /// appears automatically via <see cref="NexusControlDiscovery"/>.
    ///
    /// The original six Nexus controls were archived to archive/unused-sandbox-controls/
    /// (they predate NEXUS_CONTROL_TEMPLATE.md's MVVM rubric) and will be rebuilt against
    /// the template. As soon as a rebuilt control lands in Nexus.Components/Controls/ with
    /// no entry here, it shows up automatically via <see cref="NexusControlDiscovery"/> —
    /// no catalog edit required.</summary>
    private static readonly ComponentEntry[] ManualNexusControlEntries =
    {
        new("Nexus Popup", typeof(NexusPopupPage), ControlType: typeof(NexusApp.Controls.NexusPopup)),
    };

    public static IReadOnlyList<CatalogNode> Nodes { get; } = BuildNodes();

    private static IReadOnlyList<CatalogNode> BuildNodes()
    {
        var cataloged = ManualNexusControlEntries
            .Where(entry => entry.ControlType is not null)
            .Select(entry => entry.ControlType!)
            .ToHashSet();

        var autoEntries = NexusControlDiscovery.DiscoverUncataloged(cataloged);
        var items = ManualNexusControlEntries.Concat(autoEntries).ToArray();

        // Skip the category entirely rather than show an empty "NEXUS CONTROLS" header
        // with nothing under it (e.g. right after the archive above, before anything has
        // been rebuilt against the template).
        if (items.Length == 0)
            return Array.Empty<CatalogNode>();

        return new CatalogNode[] { new CatalogCategory("Nexus Controls", items) };
    }

    /// <summary>Whether the component shown by <paramref name="pageType"/> is flagged
    /// <see cref="ComponentEntry.IsUpdated"/>. Lets a component page surface its own badge
    /// without each page hard-coding the flag — the catalog stays the single source.</summary>
    public static bool IsUpdated(Type pageType) => Find(pageType)?.IsUpdated ?? false;

    /// <summary>Whether the page shown by <paramref name="pageType"/> is flagged
    /// <see cref="ComponentEntry.IsPrimitive"/>. Same lookup as <see cref="IsUpdated"/>.</summary>
    public static bool IsPrimitive(Type pageType) => Find(pageType)?.IsPrimitive ?? false;

    private static ComponentEntry? Find(Type pageType)
    {
        if (Home.PageType == pageType) return Home;
        foreach (var node in Nodes)
        {
            switch (node)
            {
                case CatalogCategory category:
                    foreach (var entry in category.Items)
                        if (entry.PageType == pageType) return entry;
                    break;
                case CatalogLeaf leaf:
                    if (leaf.Entry.PageType == pageType) return leaf.Entry;
                    break;
                default:
                    throw new InvalidOperationException($"Unhandled catalog node '{node.GetType().FullName}'.");
            }
        }
        return null;
    }
}
