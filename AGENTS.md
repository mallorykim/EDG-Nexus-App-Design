# Agent Guidelines — EDG Nexus App Design

## Repo structure

```
EDG Nexus App Design/
├── EDGNexusApp.csproj        ← WinUI 3 app shell (unpackaged, x64)
├── EDGNexusApp.slnx          ← Solution (app + Nexus.Components + Gallery)
├── Nexus.Components/         ← Nexus-specific WinUI class library
│   ├── Nexus.Components.csproj
│   └── Controls/             ← Nexus UserControls (empty — see archive/ note below)
├── Nexus.Components.Gallery/ ← Standalone WinUI app: browsable gallery of every
│   │                           Nexus control (NavigationView shell + live specimens).
│   │                           Currently empty (no controls to show yet — see
│   │                           "Nexus component Gallery" below); auto-discovers
│   │                           whatever lands in Nexus.Components/Controls/.
│   ├── Nexus.Components.Gallery.csproj
│   ├── ComponentCatalog.cs   ← nav tree: category → component name → page Type
│   └── Views/                ← shared ComponentPage/ControlExample chrome +
│                                AutoControlPage (reflection-driven fallback page)
├── src/
│   ├── App/                  ← App.xaml + MainWindow.xaml
│   ├── Themes/Generic.xaml   ← Nexus-only resource shell (thin)
│   ├── Resources/
│   │   ├── Foundations/      ← EtherLabel.xaml (not yet in Ether pkg)
│   │   └── Visuals/          ← EtherPageTitleGradient, EtherDataGraphics
│   └── Views/                ← App pages (Camera/, Keyboard/, etc. — currently empty
│                                placeholder folders; real pages TBD, see archive/ note)
├── Fonts/                    ← Instrument Sans + Inter variable fonts
├── Assets/                   ← Textures + device images
└── archive/                  ← Prototype controls + pages (not compiled) — see below
```

## Ether design system dependency

The app and `Nexus.Components` both reference the real Ether library via
**local ProjectReference** (sibling repo at `C:\Code\EtherDesignLibrary`):

- `Ether.DesignSystem.Foundation` — tokens (colours, spacing, typography, primitives)
- `Ether.DesignSystem.Controls` — all Ether controls + `DesignSystem.xaml` entry point

**When Ether is published to a NuGet feed**, swap both ProjectReferences in
`EDGNexusApp.csproj` and `Nexus.Components\Nexus.Components.csproj` for
`PackageReference` items:

```xml
<PackageReference Include="Ether.DesignSystem.Foundation" Version="x.y.z" />
<PackageReference Include="Ether.DesignSystem.Controls"   Version="x.y.z" />
```

## Build

```powershell
# Restore + build (x64 only — Ether only ships x64 for now)
dotnet build EDGNexusApp.csproj -p:Platform=x64

# Build + run the component Gallery
dotnet build Nexus.Components.Gallery\Nexus.Components.Gallery.csproj -p:Platform=x64
dotnet run --project Nexus.Components.Gallery\Nexus.Components.Gallery.csproj -p:Platform=x64

# Clean build
Remove-Item obj,bin -Recurse -Force
Remove-Item Nexus.Components\obj,Nexus.Components\bin -Recurse -Force
Remove-Item Nexus.Components.Gallery\obj,Nexus.Components.Gallery\bin -Recurse -Force
dotnet build EDGNexusApp.csproj -p:Platform=x64
```

The 2 SourceLink warnings from the EtherDesignLibrary projects are pre-existing
and harmless — they appear in local builds because SourceLink requires a git
remote to generate source link metadata.

### WMC9999 — use VS MSBuild, not `dotnet build`, for Nexus.Components

`dotnet build` sets `MSBuildRuntimeType=Core`, which forces the XAML build pipeline
to spawn `XamlCompiler.exe` as a child process. On this machine that process crashes
immediately with:

```
Xaml Internal Error error WMC9999: Could not find any resources appropriate for
the specified culture or the neutral culture.
```

**Fix:** use the Visual Studio MSBuild binary instead (which runs in-process):

```powershell
$msbuild = &"${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
    -latest -requires Microsoft.Component.MSBuild `
    -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
& $msbuild EDGNexusApp.csproj /p:Platform=x64 /p:Configuration=Debug /v:minimal
```

This produces **0 errors** (2 pre-existing SourceLink warnings only). The error is a
build-toolchain environment issue, not a XAML code error. Confirmed pre-existing: it
reproduces on an empty `Nexus.Components` project (no XAML files).

## Nexus component Gallery

`Nexus.Components.Gallery` is a second, standalone WinUI app (own `OutputType>WinExe`,
own `App.xaml`/`MainWindow.xaml`) whose only job is to browse every Nexus control in
isolation — a NavigationView pane generated from `ComponentCatalog.cs`, live specimen +
copy-paste XAML source per control (via the shared `ComponentPage`/`ControlExample`
chrome). It was bootstrapped from the `EtherGalleryScaffold` shell and re-pointed at
Nexus.

**Currently shows nothing** (beyond Home): `Nexus.Components/Controls/` is empty right
now (see `archive/` note below) — the "Nexus Controls" nav category only appears once
there's at least one control to show it for.

It has a single `ProjectReference` — `Nexus.Components.csproj` — no direct Ether
ProjectReference of its own. `Ether.DesignSystem.Foundation`/`.Controls` come in
transitively through `Nexus.Components` (which already needs them for every real
Nexus control), and that's enough for the Gallery's shared chrome (`EtherButton`/
`EtherCheckbox`) and `DesignSystem.xaml` tokens to resolve too. The Gallery also
**links in** — rather than copies — the app's own `src\Themes\Generic.xaml` and the
Foundations/Visuals dictionaries it merges, so a Nexus control that depends on
those thin Nexus-only resources renders identically in the Gallery and in
`EDGNexusApp`.

### New controls show up automatically — no Gallery step required

`NexusControlDiscovery.cs` reflects over the `Nexus.Components` assembly at startup and
gives every public `NexusApp.Controls.*` UserControl **not already listed** in
`ComponentCatalog.ManualNexusControlEntries` a generic fallback page
(`Views/AutoControlPage.xaml(.cs)`): it instantiates the control, live-edits every
`string`/`bool`/`int` `DependencyProperty` the control declares itself, and shows a
best-effort copy-paste snippet. The nav label gets an " (auto)" suffix so it's obvious no
one has written a real page for it yet. **A brand-new Nexus control is never invisible to
the Gallery, even with zero Gallery work.**

### Writing a real page for a control (optional, but recommended)

The auto page is a floor, not a ceiling — it can't show `ICommand`/enum/complex-typed
properties, curated descriptions, or side-by-side STATES swatches. Give a control a real
page in the same PR when you want a proper demo:

1. Create a page under `Nexus.Components.Gallery\Views\NexusControls\<Control>Page.xaml(.cs)`
   (create the `NexusControls` folder if it doesn't exist yet — it was removed along
   with the last hand-written pages when their controls were archived).
2. Root the page in a `views:ComponentPage` (`Title`, `Description`), wrap the live
   specimen in a `views:ControlExample` (`SourceXaml` = copy-paste snippet), and wire
   any settable properties to plain WinUI inputs (`TextBox`/`CheckBox`) in
   `ComponentPage.InteractiveControls` so they're actually adjustable at runtime —
   no Ether-specific input controls required.
3. Register the page's `Type` **and** `ControlType:` (the control's own `Type`, so
   discovery knows to skip it) as one line in `ComponentCatalog.ManualNexusControlEntries`
   under the
   "Nexus Controls" category.
4. Build+run the Gallery (commands above) and confirm the page renders and the
   interactive controls actually update the specimen.

## Adding Nexus controls

> **Read `Nexus.Components/NEXUS_CONTROL_TEMPLATE.md` before writing a new control.**
> It contains the canonical MVVM-compatible pattern, a copy-paste skeleton, and a
> pre-commit checklist.

### MVVM requirements (enforced by template)
Every Nexus control must be usable from a ViewModel with zero code-behind in the
consuming page:
- All state the VM drives → `DependencyProperty` (supports `{x:Bind}` / `{Binding}`)
- All actions the VM triggers → `ICommand` DependencyProperty (null-safe execute,
  same as WinUI `Button.Command`)
- No business logic in the control — only visual/interaction behavior

### Mechanics
1. Create `Nexus.Components/Controls/YourControl.xaml` and `.xaml.cs`
   — namespace must be `NexusApp.Controls`
   — `x:Class` must be `NexusApp.Controls.YourControl`
2. The SDK auto-includes XAML files in `Nexus.Components/Controls/`. No
   explicit `<Page>` entry is needed in `Nexus.Components.csproj`.
3. **Do NOT** reference other Nexus UserControls inside a Nexus UserControl's
   XAML via `<nexus:OtherControl>`. The WinUI 3 toolchain cannot handle
   intra-library UserControl cross-references in a ProjectReference context
   (WMC0610 / XBF error). Inline the visual or compose in code-behind instead.
4. **Do NOT** wire plain C# events (`EventHandler`/custom delegates) on a cross-assembly
   `UserControl` via XAML attribute syntax (e.g. `CloseButtonClick="Handler"`).
   WMC omits plain CLR events from `XamlTypeInfo.g.cs`, so the consuming project's
   XAML compiler emits `WMC0011: Unknown member` and fails to generate
   `InitializeComponent`. **Subscribe in code-behind instead:**
   ```csharp
   public MyPage() {
       this.InitializeComponent();
       Specimen.SomeEvent += Handler;   // ← correct
   }
   ```
5. **Do NOT** rely on `x:Name` to reach elements nested inside `object`-typed
   `DependencyProperty` values of another `UserControl` in the same XAML file
   (e.g. `ComponentPage.InteractiveContent`, `ControlExample.Example`). The WinUI 3
   XBF loader routes `Connect()` calls to the inner control rather than the host
   page, leaving every such named field **null at runtime** even though the build
   succeeds. For hand-written Gallery pages, build the full content tree in
   code-behind (in `OnNavigatedTo`) instead of XAML — exactly as `AutoControlPage`
   and `NexusPopupPage` do.
6. The app project automatically excludes `Nexus.Components/**` from its own
   SDK glob (via `<Page Remove>` / `<Compile Remove>` in `EDGNexusApp.csproj`).
   Do not add Nexus control files to the app project directly.

## Namespace map

| What                    | Namespace / xmlns alias          |
|-------------------------|----------------------------------|
| Nexus controls          | `using:NexusApp.Controls`        |
| Ether controls          | `using:Ether.DesignSystem.Controls` |
| Ether Foundation types  | `using:Ether.DesignSystem.Foundation` |

## Resource loading order (App.xaml)

1. `ms-appx:///Ether.DesignSystem.Controls/Themes/DesignSystem.xaml`
   → loads Foundation tokens + all Ether control templates
2. `ms-appx:///Generic.xaml`
   → thin Nexus shell: EtherLabel, EtherPageTitleGradient, EtherDataGraphics

Never merge the Ether Foundation token files (`EtherColors.xaml`,
`EtherPrimitives.xaml`, etc.) directly — `DesignSystem.xaml` handles them.

## archive/ folder

`archive/unused-sandbox-controls/` holds prototype controls that were sketch-only
tests, none of them compiled into the current build:
- Nav/surface prototypes: EtherDeviceMenu, EtherNavRail, EtherRightPanel,
  EtherWideNav, EtherAIAssistant
- Early Nexus test controls (predate the MVVM template/rubric — do not use as
  reference examples, and do not restore as-is; controls will be rebuilt against
  `NEXUS_CONTROL_TEMPLATE.md`): NexusActionBar, NexusBatteryChip, NexusDeviceHeader,
  NexusDevicePreview, NexusFeatureListRow, NexusTopBar

`archive/unused-sandbox-views/` holds the pages that consumed those early Nexus
controls (Camera/CameraColorImagePage, Keyboard/KeyboardBasePage,
_Template/PeripheralDetailFeatureDetailPage, _Template/PeripheralDetailTemplatePage).
`ScreenCatalog.cs` was updated with placeholder stubs in their place.

Everything under `archive/` lives in git history for reference. Do not move it back
into `src/` or `Nexus.Components/` as-is.
