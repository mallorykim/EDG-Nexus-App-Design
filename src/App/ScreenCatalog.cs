namespace NexusApp;

/// <summary>
/// One screen entry: the nav label (e.g. "Base — Connected") and the Page type that renders it.
/// </summary>
public sealed record ScreenEntry(string Name, Type PageType);

/// <summary>
/// A peripheral group in the gallery nav: the peripheral name (e.g. "Keyboard") and
/// its list of screens (one per feature category or state).
/// </summary>
public sealed record PeripheralGroup(string Peripheral, IReadOnlyList<ScreenEntry> Screens);

/// <summary>
/// Single source of truth for the Nexus gallery navigation tree.
/// Groups screens by peripheral type, then by screen name within each peripheral.
///
/// Adding a new screen:
///   1. Create the Page class in src/Views/[Peripheral]/
///   2. Add a using at the top of this file
///   3. Add a new ScreenEntry row in the correct PeripheralGroup below
///   That's it — MainWindow and GalleryHomePage both read from this catalog.
///
/// Naming convention for ScreenEntry:
///   "Base — Connected"       → default/connected state, all feature categories
///   "Base — Disconnected"    → disconnected state variant
///   "Base — Updating"        → firmware update in progress
///   "Base — Alert"           → warning/alert state
///   "Base — Error"           → error state
/// </summary>
public static class ScreenCatalog
{
    /// <summary>The gallery home/index page. Always shown first; not part of Peripherals.</summary>
    public static ScreenEntry Home { get; } = new("Home", typeof(GalleryHomePage));

    public static IReadOnlyList<PeripheralGroup> Peripherals { get; } = new PeripheralGroup[]
    {
        new("Keyboard", new ScreenEntry[]
        {
            // Placeholder — add KeyboardBasePage when rebuilt
            // new ScreenEntry("Base — Connected", typeof(KeyboardBasePage)),
        }),

        new("Mouse", new ScreenEntry[]
        {
            // Placeholder — uncomment and add MouseBasePage when designed
            // new ScreenEntry("Base — Connected", typeof(MouseBasePage)),
        }),

        new("Display", new ScreenEntry[]
        {
            // new ScreenEntry("Base — Connected", typeof(DisplayBasePage)),
        }),

        new("Audio", new ScreenEntry[]
        {
            // new ScreenEntry("Base — Connected (Speaker)",    typeof(AudioSpeakerBasePage)),
            // new ScreenEntry("Base — Connected (Headset)",    typeof(AudioHeadsetBasePage)),
        }),

        new("Camera", new ScreenEntry[]
        {
            // Placeholder — add CameraColorImagePage when rebuilt
            // new ScreenEntry("Color and Image", typeof(CameraColorImagePage)),
        }),

        new("Dock", new ScreenEntry[]
        {
            // new ScreenEntry("Base — Connected", typeof(DockBasePage)),
        }),

        new("Travel Hub", new ScreenEntry[]
        {
            // new ScreenEntry("Base — Connected", typeof(TravelHubBasePage)),
        }),

        new("Pen", new ScreenEntry[]
        {
            // new ScreenEntry("Base — Connected", typeof(PenBasePage)),
        }),
    };

    /// <summary>
    /// All peripheral groups that have at least one screen registered.
    /// MainWindow uses this so empty groups don't appear in the nav.
    /// </summary>
    public static IEnumerable<PeripheralGroup> ActivePeripherals =>
        Peripherals.Where(g => g.Screens.Count > 0);
}
