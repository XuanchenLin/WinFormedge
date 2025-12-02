# Using Different Window Styles

This section introduces how to use the various window styles provided by WinFormedge to create windows with specific appearance and behavior.

To use different window styles, create a form class that inherits from `Formedge` and override the `ConfigureWindowSettings` method. In this method, call the predefined window style methods provided by `HostWindowBuilder`, such as `UseDefaultWindow` and `UseKioskWindow`, to obtain the corresponding window settings object. You can then customize these settings as needed.

## Default Window Style

The `HostWindowBuilder.UseDefaultWindow` method provides the most basic WinFormedge window style. You can further customize this style using the returned `DefaultWindowSettings` object.

Here's an example showing how to create a simple WinFormedge window using the default window style:

```csharp
using WinFormedge;
namespace MyApp;
internal class MyCustomForm : Formedge
{
  public MyCustomForm()
  {
    Url = "https://www.bing.com";
  }

  protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
  {
    // opts provides several predefined window style methods; choose one
    // The method returns the default settings for that style, which you can modify
    var windowSettings = opts.UseDefaultWindow();

    // You can also customize standard window properties here (or in the constructor), e.g.:
    MinimumSize = new Size(1024, 640);
    Size = new Size(1280, 800);

    // Return the customized window settings
    return windowSettings;
  }
}
```

Using the above code, you can create a WinFormedge window with the default window style. This window will have a standard title bar, border, and system menu.

![Default Window Style](window-standard-style.png)

## Extending Content into Title Bar Area

Continuing with the `HostWindowBuilder.UseDefaultWindow` method, you can extend content into the title bar area by setting the `ExtendsContentIntoTitleBar` property, achieving a borderless window effect.

At this point, you can use HTML/CSS to design the content and styling of the title bar area, create your own close, minimize, and maximize button styles, and flexibly control their behavior. WinFormedge provides a series of UI interaction APIs to help you implement these features. Some of these APIs are based on HTML and CSS properties, while others are based on JavaScript interfaces provided by WinFormedge. You can refer to the [Window Interaction API Documentation](./window-interaction-apis.md) for more details.

Here's an example showing how to create a borderless WinFormedge window with content extended into the title bar area:

```csharp
using WinFormedge;
namespace MyApp;
internal class MyBorderlessForm : Formedge
{
  protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
  {
    var windowSettings = opts.UseDefaultWindow();

    // Extend content into the title bar area
    windowSettings.ExtendsContentIntoTitleBar = true;

    // You can also customize standard window properties here (or in the constructor), e.g.:
    MinimumSize = new Size(1024, 640);
    Size = new Size(1280, 800);

    // Return the customized window settings
    return windowSettings;
  }
}
```

![Extended Content Window Style](window-extended-style.png)

## Window Backdrop Effects

The `DefaultWindowSettings` object returned by the `HostWindowBuilder.UseDefaultWindow` method contains a property called `SystemBackdropType`, which you can use to set the window's backdrop effect. WinFormedge supports multiple backdrop effects, including blur, acrylic, and Windows 11's Mica effect. You can choose an appropriate backdrop effect to enhance your application's visual experience.

### Default Backdrop Effect

Using `SystemBackdropType.Auto` allows the window to adopt the system's default backdrop effect. This typically adjusts the backdrop effect automatically based on the operating system's settings.

```csharp
windowSettings.SystemBackdropType = SystemBackdropType.Auto;
```

![SystemBackdropType.Auto](window-backdrop-auto.png)

### No Backdrop Effect

Using `SystemBackdropType.None` creates a window without a backdrop effect, making it fully transparent. You can use HTML/CSS to design the window's appearance and style.

```csharp
windowSettings.SystemBackdropType = SystemBackdropType.None;
```

![SystemBackdropType.None](window-backdrop-none.png)

### Blur Backdrop Effect

Using `SystemBackdropType.BlurBehind` applies a blur backdrop effect to the window, similar to a glass effect. This effect enhances the visual depth of the window.

```csharp
windowSettings.SystemBackdropType = SystemBackdropType.BlurBehind;
```

![SystemBackdropType.BlurBehind](window-backdrop-blur-behind.png)

### Acrylic Backdrop Effect

Using `SystemBackdropType.Acrylic` applies an acrylic backdrop effect to the window. This effect is available in Windows 10 and later, providing a semi-transparent blurred visual effect. Compared to the `BlurBehind` effect, you can use the `BackColor` property to adjust the color and transparency of the acrylic effect.

```csharp
windowSettings.SystemBackdropType = SystemBackdropType.Acrylic;
```

![SystemBackdropType.Acrylic](window-backdrop-acrylic.png)

### Mica Backdrop Effect

Using `SystemBackdropType.Mica` applies Windows 11's Mica backdrop effect to the window. This effect dynamically adjusts the window's background color based on the desktop wallpaper colors.

```csharp
windowSettings.SystemBackdropType = SystemBackdropType.Mica;
```

![SystemBackdropType.Mica](window-backdrop-mica.png)

### Mica Alt Backdrop Effect

Using `SystemBackdropType.MicaAlt` applies Windows 11's Mica Alt backdrop effect to the window. This effect is similar to the Mica effect but has a different visual style.

```csharp
windowSettings.SystemBackdropType = SystemBackdropType.MicaAlt;
```

![SystemBackdropType.MicaAlt](window-backdrop-mica-alt.png)

### Transient Backdrop Effect

Using `SystemBackdropType.Transient` applies Windows 11's transient blur backdrop effect to the window. This effect provides a lightweight blurred visual experience.

```csharp
windowSettings.SystemBackdropType = SystemBackdropType.Transient;
```

![SystemBackdropType.Transient](window-backdrop-transient.png)

## Transparent Window with Custom Border

By setting `SystemBackdropType` to `None` and `ShowWindowDecorators` to `false`, you can create a fully transparent window. At this point, you can use HTML/CSS to design the window's appearance and style, including details such as borders and shadows. You need to use the `WindowEdgeOffsets` property to manually specify the window edge offsets to adjust the spacing between content and the border.

The sample application `MinimalExampleApp` included with the WinFormedge source code contains a transparent window example where all details are implemented using HTML/CSS. When you run it, you will see the window effect shown in the image below:

![Transparent Window](window-transparent.png)

The window's border uses the following CSS code to implement the shadow effect:

```css
box-shadow: 0px 5px 20px #333333;
```

Based on measurements, you need to exclude the shadow area from WinFormedge's mouse HitTest handling range. Therefore, set `WindowEdgeOffsets` to `new Padding(20, 15, 20, 25)` to ensure the shadow effect is fully displayed and calculate the actual window edge offsets after cropping the shadow area, so that WinFormedge can correctly handle mouse events.

```csharp
windowSettings.SystemBackdropType = SystemBackdropType.None;
windowSettings.ShowWindowDecorators = false;
windowSettings.WindowEdgeOffsets = new Padding(20, 15, 20, 25); // Set edge offsets
```

To precisely obtain the dimensions of the shadow area, you can use the element inspection feature in the browser's developer tools to measure the correct shadow size. As shown in the image, after using the developer tools and selecting the window border element, you can accurately obtain the shadow dimensions based on different color blocks. You can take a screenshot and use other image processing tools to measure the shadow size.

![Getting Shadow Size](window-transparent-get-padding.png)

## References

- [Window Styles](./window-styles.md)
- [Window Interaction APIs](./window-interaction-apis.md)
