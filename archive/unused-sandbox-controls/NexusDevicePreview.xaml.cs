using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NexusApp.Controls;

/// <summary>
/// Nexus pattern — the fixed white device surface with its permanent texture
/// (wave in Light, dots in Dark). The device image and placeholder are owned
/// by the page overlay grid above this surface, not by this control.
/// </summary>
public sealed partial class NexusDevicePreview : UserControl
{
    public NexusDevicePreview()
    {
        this.InitializeComponent();
        Loaded += (_, _) => UpdateTexture();
        ActualThemeChanged += (_, _) => UpdateTexture();
    }

    private void UpdateTexture()
    {
        bool isLight = ActualTheme == ElementTheme.Light;
        TextureLight.Visibility = isLight ? Visibility.Visible : Visibility.Collapsed;
        TextureDark.Visibility = isLight ? Visibility.Collapsed : Visibility.Visible;
    }
}
