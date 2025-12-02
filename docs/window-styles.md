# Window Styles

## Overview

To use a WinFormedge window, create a form class that inherits from `Formedge`. WinFormedge provides multiple window styles you can choose from. Override `Formedge.ConfigureWindowSettings` to customize the window’s appearance and behavior.

## Default Window Style

The `HostWindowBuilder.UseDefaultWindow` method provides the most basic WinFormedge window style. You can further customize this style using the returned `DefaultWindowSettings` object.

```csharp
using WinFormedge;

namespace MyApp;

internal class MyCustomForm : Formedge
{
    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
      // opts provides several predefined window style methods; choose one
      // The method returns the default settings for that style, which you can modify
      var windowSettings = opts.UseDefaultWindow();

      // For example, extend content into the title bar area
      windowSettings.ExtendsContentIntoTitleBar = true;

      // You can also customize standard window properties here (or in the constructor), e.g.:
      MinimumSize = new Size(1024, 640);
      Size = new Size(1280, 800);

      // Return the customized window settings
      return windowSettings;
    }
}
```

The `DefaultWindowSettings` object includes the following properties to configure various behaviors and visual aspects:

| Property                   | Type                 | Description                                                                                                                                                                                                                                                                                                                                                           |
| -------------------------- | -------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ExtendsContentIntoTitleBar | `bool`               | Indicates whether content should extend into the title bar area. Set to `true` to achieve a borderless window effect.                                                                                                                                                                                                                                                 |
| Fullscreenable             | `bool`               | Indicates whether the window supports fullscreen.                                                                                                                                                                                                                                                                                                                     |
| Resizable                  | `bool`               | Indicates whether the window can be resized.                                                                                                                                                                                                                                                                                                                          |
| SystemMenu                 | `bool`               | Indicates whether the window shows the system menu (the menu shown when right-clicking the title bar).                                                                                                                                                                                                                                                                |
| ShowWindowDecorators       | `bool`               | Indicates whether to show window decorations (such as title bar, border, and system shadow).                                                                                                                                                                                                                                                                          |
| SystemBackdropType         | `SystemBackdropType` | Sets the window backdrop effect: <br>`Auto` - System default backdrop. <br>`None` - No backdrop (fully transparent). <br>`BlurBehind` - Blur/glass effect. <br>`Acrylic` - Acrylic effect. <br>`Mica` - Windows 11 Mica effect. <br>`MicaAlt` - Windows 11 Mica Alt effect. <br>`Transient` - Windows 11 transient blur effect                                        |
| WindowEdgeOffsets          | `Padding`            | Sets the offsets of the window edges to adjust spacing between content and the border. Primarily used when `SystemBackdropType` is fully transparent and `ShowWindowDecorators` is `false`. In this case the form is fully transparent and you can draw all details using HTML/CSS (e.g., border, shadows). Use `WindowEdgeOffsets` to manually specify edge offsets. |

## Kiosk Window Style

The `HostWindowBuilder.UseKioskWindow` method provides a window style suitable for information-display applications. It is typically used for scenarios that require fullscreen and disallow users from closing or minimizing the window. The method returns a `KioskWindowSettings` object that you can use to further customize the kiosk window style.

```csharp
using WinFormedge;
namespace MyApp;
internal class MyKioskForm : Formedge
{
    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
      // Use the kiosk window style as the base
      var windowSettings = opts.UseKioskWindow();

      // Return the customized window settings
      return windowSettings;
    }
}
```

The `KioskWindowSettings` object includes the following properties to configure the kiosk window’s behavior and appearance:
| Property | Type | Description |
| ------------ | ------- | ------------------------------------------- |
| Fullscreen | `bool` | Indicates whether the window should be fullscreen. |
| TargetScreen | `Screen`| Specifies which screen the window should appear on. Defaults to the primary screen. |

## Related Content

- [Using Different Window Styles](using-different-window-styles.md)
