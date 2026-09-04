using Microsoft.UI.Xaml.Controls;

namespace NexusApp;

/// <summary>
/// Base class for all Nexus peripheral screens.
///
/// Every screen in the Nexus gallery should extend NexusPage rather than Page
/// directly. This gives us a central place to add shared infrastructure as the
/// sprint progresses — theme awareness, navigation helpers, screenshot hooks,
/// token audit utilities, etc.
///
/// Current capabilities (scaffold):
///   - Inherits from Page (WinUI3 Frame navigation works as normal)
///   - No additional behavior yet — this class is intentionally thin at scaffold
///     time and will grow as patterns emerge from the pilot
///
/// Planned additions (add here as needed during the sprint):
///   - OnThemeChanged() override slot for screens that need to swap assets on
///     Light ↔ Dark (e.g. background textures, device illustrations)
///   - Screenshot capture helper (end-of-session workflow)
///   - PeripheralName / ScreenState properties for gallery metadata
/// </summary>
public class NexusPage : Page
{
    // Intentionally empty at scaffold time.
    // Do not add behavior here without confirming it is needed by multiple screens.
}
