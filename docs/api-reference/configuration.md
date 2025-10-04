# 配置选项

本页面介绍了 WinFormedge 中各种配置选项和枚举类型的详细信息。

## StartupSettings

`StartupSettings` 类用于配置应用程序的启动行为。

### 方法

#### UseMainWindow
```csharp
public AppCreationAction UseMainWindow(Formedge mainWindow)
```
指定应用程序的主窗口。

**参数:**
- `mainWindow`: 主窗口实例

**返回值:**
- `AppCreationAction`: 应用程序创建动作

### 示例

```csharp
protected override AppCreationAction? OnApplicationStartup(StartupSettings options)
{
    var mainWindow = new MainWindow();
    return options.UseMainWindow(mainWindow);
}
```

---

## SystemBackdropType

`SystemBackdropType` 枚举定义了窗口的系统背景效果类型。

### 枚举值

#### Auto
```csharp
Auto = 0
```
自动选择背景类型（默认值）。

#### None
```csharp
None = 1
```
无背景效果，使用纯色背景。

#### BlurBehind
```csharp
BlurBehind = 2
```
模糊背景效果。
- **最低要求**: Windows 10 版本 2004

#### Acrylic
```csharp
Acrylic = 3
```
亚克力背景效果。
- **最低要求**: Windows 10 版本 2004

#### Mica
```csharp
Mica = 4
```
Mica 背景效果。
- **最低要求**: Windows 11

#### Transient
```csharp
Transient = 5
```
临时窗口的 Mica 效果。
- **最低要求**: Windows 11

#### MicaAlt
```csharp
MicaAlt = 6
```
替代 Mica 背景效果。
- **最低要求**: Windows 11

### 使用示例

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    
    // 设置不同的背景效果
    if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
    {
        // Windows 11 - 使用 Mica
        settings.SystemBackdropType = SystemBackdropType.Mica;
    }
    else if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041))
    {
        // Windows 10 2004+ - 使用模糊效果
        settings.SystemBackdropType = SystemBackdropType.BlurBehind;
    }
    else
    {
        // 较旧版本 - 无效果
        settings.SystemBackdropType = SystemBackdropType.None;
    }
    
    return settings;
}
```

### 背景效果对比

| 效果类型 | 外观特征 | 性能影响 | 支持版本 |
|----------|----------|----------|----------|
| None | 纯色背景 | 最佳 | 所有版本 |
| BlurBehind | 模糊透明 | 中等 | Win10 2004+ |
| Acrylic | 亚克力质感 | 较高 | Win10 2004+ |
| Mica | 自然材质感 | 中等 | Win11+ |
| Transient | 临时窗口材质 | 中等 | Win11+ |
| MicaAlt | 深色替代材质 | 中等 | Win11+ |

---

## SystemColorMode

`SystemColorMode` 枚举定义了应用程序的颜色模式。

### 枚举值

#### Auto
```csharp
Auto = 0
```
自动跟随系统颜色模式（默认值）。

#### Light
```csharp
Light = 1
```
强制使用浅色模式。

#### Dark
```csharp
Dark = 2
```
强制使用深色模式。

### 使用示例

```csharp
// 在 AppBuilder 中设置
var app = WinFormedgeApp.CreateAppBuilder()
    .SetColorMode(SystemColorMode.Dark)  // 强制深色模式
    .UseWinFormedgeApp<MyApp>()
    .Build();

// 在运行时检查颜色模式
var isDarkMode = WinFormedgeApp.Current.IsDarkMode;
if (isDarkMode)
{
    // 应用深色主题相关逻辑
}
```

---

## KisokWindowSettings

`KisokWindowSettings` 类用于配置 Kiosk 模式窗口的特殊设置。

### 属性

通常继承自 `WindowSettings` 的所有属性，并针对 Kiosk 模式进行了特殊配置：

- 窗口无法被最小化
- 窗口无法被关闭（通过标准按钮）
- 窗口占据全屏
- 阻止 Alt+Tab 等系统快捷键

### 使用示例

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    // 创建 Kiosk 模式窗口
    var settings = opts.UseKisokWindow();
    
    // Kiosk 模式的附加配置
    settings.Resizable = false;
    
    return settings;
}
```

### 注意事项

- Kiosk 模式窗口通常用于公共展示或安全环境
- 用户需要使用 `Alt+F4` 或程序内部逻辑来退出
- 建议在创建 Kiosk 窗口前向用户显示提示信息

---

## DefaultWindowSettings

`DefaultWindowSettings` 类提供标准窗口的默认配置选项。

### 属性示例

```csharp
public class DefaultWindowSettings : WindowSettings
{
    public bool ExtendsContentIntoTitleBar { get; set; } = false;
    public bool ShowWindowDecorators { get; set; } = true;
    public bool Resizable { get; set; } = true;
    public SystemBackdropType SystemBackdropType { get; set; } = SystemBackdropType.Auto;
    public Padding WindowEdgeOffsets { get; set; } = Padding.Empty;
}
```

### 常用配置组合

#### 标准窗口
```csharp
var settings = opts.UseDefaultWindow();
settings.ExtendsContentIntoTitleBar = false;
settings.ShowWindowDecorators = true;
settings.SystemBackdropType = SystemBackdropType.None;
```

#### 无边框窗口
```csharp
var settings = opts.UseDefaultWindow();
settings.ExtendsContentIntoTitleBar = true;
settings.ShowWindowDecorators = true;
settings.SystemBackdropType = SystemBackdropType.Mica;
```

#### 完全自定义窗口
```csharp
var settings = opts.UseDefaultWindow();
settings.ExtendsContentIntoTitleBar = true;
settings.ShowWindowDecorators = false;
settings.Resizable = true;
settings.WindowEdgeOffsets = new Padding(20); // 为拖拽调整大小留出边距
settings.SystemBackdropType = SystemBackdropType.Acrylic;
```

---

## 配置最佳实践

### 1. 版本兼容性检查

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    
    // 根据操作系统版本选择合适的背景效果
    if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
    {
        settings.SystemBackdropType = SystemBackdropType.Mica;
    }
    else if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041))
    {
        settings.SystemBackdropType = SystemBackdropType.BlurBehind;
    }
    else
    {
        settings.SystemBackdropType = SystemBackdropType.None;
        BackColor = Color.White; // 设置回退颜色
    }
    
    return settings;
}
```

### 2. 响应式配置

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    
    // 根据屏幕尺寸调整窗口配置
    var screen = Screen.PrimaryScreen;
    if (screen.Bounds.Width >= 1920)
    {
        // 高分辨率屏幕 - 使用现代效果
        settings.SystemBackdropType = SystemBackdropType.Mica;
        settings.ExtendsContentIntoTitleBar = true;
    }
    else
    {
        // 低分辨率屏幕 - 使用简单配置以提高性能
        settings.SystemBackdropType = SystemBackdropType.None;
        settings.ExtendsContentIntoTitleBar = false;
    }
    
    return settings;
}
```

### 3. 主题适配

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    
    // 根据系统主题调整窗口外观
    var isDarkMode = WinFormedgeApp.Current.IsDarkMode;
    
    if (isDarkMode)
    {
        settings.SystemBackdropType = SystemBackdropType.MicaAlt;
        BackColor = Color.FromArgb(32, 32, 32);
    }
    else
    {
        settings.SystemBackdropType = SystemBackdropType.Mica;
        BackColor = Color.FromArgb(248, 248, 248);
    }
    
    return settings;
}
```