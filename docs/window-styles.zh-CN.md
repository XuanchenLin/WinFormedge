# 窗口样式

## 概述

要使用 WinFormedge 窗体，您需要创建一个继承自 `Formedge` 的窗体类。WinFormedge 提供了多种窗口样式，您可以根据需要选择合适的样式。重载 `Formedge` 的 `ConfigureWindowSettings` 方法，可以自定义窗口的外观和行为。

## 默认窗体样式

`HostWindowBuilder` 的 `UseDefaultWindow` 方法提供了 WinFormedge 最基本的窗口样式，您可以在该方法返回的 `DefaultWindowSettings` 对象上对这种样式的窗体进行进一步的自定义。

```csharp
using WinFormedge;

namespace MyApp;

internal class MyCustomForm : Formedge
{
    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
      // opts 提供了多种预定义的窗口样式方法，您可以选择其中之一
      // 该方法将返回该窗体样式的默认设置，您可以在此基础上进行修改
      var windowSettings = opts.UseDefaultWindow();

      // 例如，您可以修改标题栏的外观，使内容扩展到标题栏区域
      windowSettings.ExtendsContentIntoTitleBar = true;

      // 您还可以在此重载方法中自定义标准窗口属性（也可以根据需要在构造函数设置这些窗口标准属性），例如：
      MinimumSize = new Size(1024, 640);
      Size = new Size(1280, 800);

      // 在设置完成后返回自定义的窗口设置
      return windowSettings;
    }
}
```

`DefaultWindowSettings` 对象包含以下属性，允许您配置窗口的各种行为和外观：

| 属性                       | 类型                 | 说明                                                                                                                                                                                                                                                                                                                  |
| -------------------------- | -------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ExtendsContentIntoTitleBar | `bool`               | 指示内容是否应扩展到标题栏区域。设置为 `true` 可实现无边框窗口效果。                                                                                                                                                                                                                                                  |
| Fullscreenable             | `bool`               | 指示窗口是否是全屏显示。                                                                                                                                                                                                                                                                                              |
| Resizable                  | `bool`               | 指示窗口是否可以调整大小。                                                                                                                                                                                                                                                                                            |
| SystemMenu                 | `bool`               | 指示窗口是否显示系统菜单（右键点击标题栏时显示的菜单）。                                                                                                                                                                                                                                                              |
| ShowWindowDecorators       | `bool`               | 指示是否显示窗口装饰（如标题栏、边框和系统阴影）。                                                                                                                                                                                                                                                                    |
| SystemBackdropType         | `SystemBackdropType` | 设置窗口的背景效果：<br>`Auto` - 系统默认背景效果。<br>`None` - 无背景效果（完全透明）。<br>`BlurBehind` - 毛玻璃效果。 <br>`Acrylic` - 亚克力效果。 <br>`Mica` - Windows 11 云母效果。 <br>`MicaAlt` - Windows 11 云母 Alt 效果。 <br>`Transient` - Windows 11 模糊效果                                              |
| WindowEdgeOffsets          | `Padding`            | 设置窗口边缘的偏移量，用于调整窗口内容与边框之间的间距。此属性主要用于 `SystemBackdropType` 设置为完全透明且 `ShowWindowDecorators` 设置为 `false` 的情况下，此时窗体将完全透明，您可以完全使用 html 和 css 来绘制窗体细节，包括：窗体边框、阴影等。因此您需要使用 `WindowEdgeOffsets` 手动指定窗体边框位置的偏移量。 |
|                            |

## KIOSK 模式窗体样式

`HostWindowBuilder` 的 `UseKioskWindow` 方法提供了一个适用于信息展示类应用程序的窗口样式。该样式通常用于需要全屏显示且不允许用户关闭或最小化窗口的场景，该方法返回的 `KioskWindowSettings` 对象，您可以使用它来进一步自定义 KIOSK 窗体样式。

```csharp
using WinFormedge;
namespace MyApp;
internal class MyKioskForm : Formedge
{
    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
      // 使用 KIOSK 窗口样式作为基础
      var windowSettings = opts.UseKioskWindow();

      // 返回自定义的窗口设置
      return windowSettings;
    }
}
```

`KioskWindowSettings` 对象包含以下属性，允许您配置 KIOSK 窗体的各种行为和外观：
| 属性 | 类型 | 说明 |
| ------------------ | ------ | ------------------------------------ |
| Fullscreen | `bool` | 指示窗口是否应全屏显示。 |
| TargetScreen | `Screen` | 指定窗口应显示在哪个屏幕上。默认为主屏幕。 |

## 相关内容

- [使用不同的窗体样式](using-different-window-styles.zh-CN.md)
