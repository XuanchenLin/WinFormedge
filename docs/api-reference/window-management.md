# 窗口管理

本页面详细介绍 WinFormedge 中窗口创建、配置和管理的相关类和方法。

## WindowSettings

`WindowSettings` 是所有窗口设置类的基类，定义了窗口的基本行为和外观。

### 主要属性

#### ExtendsContentIntoTitleBar
```csharp
public bool ExtendsContentIntoTitleBar { get; set; }
```
获取或设置是否将内容扩展到标题栏区域。
- `true`: 创建无边框窗口，内容占据整个窗口区域
- `false`: 使用标准窗口边框和标题栏

#### ShowWindowDecorators
```csharp
public bool ShowWindowDecorators { get; set; }
```
获取或设置是否显示窗口装饰器（边框、阴影等）。

#### Resizable
```csharp
public bool Resizable { get; set; }
```
获取或设置窗口是否可调整大小。

#### SystemBackdropType
```csharp
public SystemBackdropType SystemBackdropType { get; set; }
```
获取或设置系统背景效果类型。

#### WindowEdgeOffsets
```csharp
public Padding WindowEdgeOffsets { get; set; }
```
获取或设置窗口边缘偏移量，用于无边框窗口的拖拽调整大小。

### 使用示例

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    
    // 创建现代化无边框窗口
    settings.ExtendsContentIntoTitleBar = true;
    settings.ShowWindowDecorators = true;
    settings.Resizable = true;
    settings.SystemBackdropType = SystemBackdropType.Mica;
    
    // 设置窗口基本属性
    Size = new Size(1200, 800);
    MinimumSize = new Size(800, 600);
    StartPosition = FormStartPosition.CenterScreen;
    
    return settings;
}
```

---

## HostWindowBuilder

`HostWindowBuilder` 提供了创建不同类型窗口设置的工厂方法。

### 方法

#### UseDefaultWindow
```csharp
public DefaultWindowSettings UseDefaultWindow()
```
创建标准窗口设置。

**返回值:**
- `DefaultWindowSettings`: 默认窗口设置实例

#### UseKisokWindow
```csharp
public KisokWindowSettings UseKisokWindow()
```
创建 Kiosk 模式窗口设置。

**返回值:**
- `KisokWindowSettings`: Kiosk 窗口设置实例

### 使用示例

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    // 根据应用程序模式选择窗口类型
    if (IsKioskMode)
    {
        return opts.UseKisokWindow();
    }
    else
    {
        return opts.UseDefaultWindow();
    }
}
```

---

## 窗口样式配置

### 标准窗口样式

创建带有完整窗口边框和标题栏的传统窗口：

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    
    // 标准窗口配置
    settings.ExtendsContentIntoTitleBar = false;
    settings.ShowWindowDecorators = true;
    settings.Resizable = true;
    settings.SystemBackdropType = SystemBackdropType.None;
    
    return settings;
}
```

### 无边框窗口样式

创建现代化的无边框窗口，内容扩展到整个窗口区域：

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    
    // 无边框窗口配置
    settings.ExtendsContentIntoTitleBar = true;
    settings.ShowWindowDecorators = true;  // 保留阴影等装饰
    settings.Resizable = true;
    settings.SystemBackdropType = SystemBackdropType.Mica;
    
    return settings;
}
```

### 完全自定义窗口样式

创建完全自定义的窗口，移除所有系统装饰：

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    
    // 完全自定义窗口配置
    settings.ExtendsContentIntoTitleBar = true;
    settings.ShowWindowDecorators = false;  // 移除所有装饰
    settings.Resizable = true;
    settings.SystemBackdropType = SystemBackdropType.BlurBehind;
    
    // 设置边缘偏移以支持拖拽调整大小
    settings.WindowEdgeOffsets = new Padding(15, 15, 15, 15);
    
    return settings;
}
```

### Kiosk 模式窗口

创建用于公共展示的 Kiosk 模式窗口：

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseKisokWindow();
    
    // Kiosk 模式通常不允许调整大小
    settings.Resizable = false;
    
    // 设置为全屏黑色背景
    BackColor = Color.Black;
    WindowState = FormWindowState.Maximized;
    
    return settings;
}
```

---

## 高级窗口特性

### 自适应窗口配置

根据系统环境和硬件能力自动调整窗口配置：

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    
    // 获取系统信息
    var screen = Screen.FromPoint(MousePosition);
    var isHighDPI = DeviceDpi > 96;
    var isWindows11 = OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000);
    
    // 根据屏幕尺寸调整窗口大小
    if (screen.Bounds.Width >= 2560)
    {
        Size = new Size(1600, 1000);
        MinimumSize = new Size(1200, 800);
    }
    else if (screen.Bounds.Width >= 1920)
    {
        Size = new Size(1200, 800);
        MinimumSize = new Size(1000, 600);
    }
    else
    {
        Size = new Size(1000, 600);
        MinimumSize = new Size(800, 500);
    }
    
    // 根据系统版本选择背景效果
    if (isWindows11)
    {
        settings.SystemBackdropType = SystemBackdropType.Mica;
        settings.ExtendsContentIntoTitleBar = true;
    }
    else if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041))
    {
        settings.SystemBackdropType = SystemBackdropType.BlurBehind;
        settings.ExtendsContentIntoTitleBar = true;
    }
    else
    {
        settings.SystemBackdropType = SystemBackdropType.None;
        settings.ExtendsContentIntoTitleBar = false;
    }
    
    return settings;
}
```

### 动态窗口样式切换

在运行时动态改变窗口样式：

```csharp
public class AdaptiveWindow : Formedge
{
    private bool _isCompactMode = false;
    
    public void ToggleCompactMode()
    {
        _isCompactMode = !_isCompactMode;
        
        if (_isCompactMode)
        {
            // 切换到紧凑模式
            Size = new Size(400, 300);
            MinimumSize = new Size(300, 200);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }
        else
        {
            // 切换到正常模式
            Size = new Size(1200, 800);
            MinimumSize = new Size(800, 600);
            FormBorderStyle = FormBorderStyle.Sizable;
        }
    }
    
    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var settings = opts.UseDefaultWindow();
        
        if (_isCompactMode)
        {
            settings.Resizable = false;
            settings.SystemBackdropType = SystemBackdropType.None;
        }
        else
        {
            settings.Resizable = true;
            settings.SystemBackdropType = SystemBackdropType.Mica;
        }
        
        return settings;
    }
}
```

### 多窗口管理

管理多个窗口实例：

```csharp
public class WindowManager
{
    private readonly List<Formedge> _openWindows = new();
    private Formedge? _mainWindow;
    
    public void CreateMainWindow()
    {
        _mainWindow = new MainWindow();
        _mainWindow.FormClosed += OnWindowClosed;
        _openWindows.Add(_mainWindow);
        _mainWindow.Show();
    }
    
    public void CreateChildWindow(IWin32Window? parent = null)
    {
        var childWindow = new ChildWindow();
        childWindow.FormClosed += OnWindowClosed;
        _openWindows.Add(childWindow);
        
        if (parent != null)
        {
            childWindow.Show(parent);
        }
        else
        {
            childWindow.Show();
        }
    }
    
    public void CreateDialogWindow(IWin32Window parent)
    {
        var dialogWindow = new DialogWindow();
        dialogWindow.FormClosed += OnWindowClosed;
        _openWindows.Add(dialogWindow);
        dialogWindow.ShowDialog(parent);
    }
    
    private void OnWindowClosed(object? sender, FormClosedEventArgs e)
    {
        if (sender is Formedge window)
        {
            _openWindows.Remove(window);
            
            // 如果主窗口关闭，关闭所有子窗口
            if (window == _mainWindow)
            {
                CloseAllWindows();
            }
        }
    }
    
    public void CloseAllWindows()
    {
        var windows = _openWindows.ToList();
        foreach (var window in windows)
        {
            window.Close();
        }
        _openWindows.Clear();
    }
}
```

---

## 窗口事件和生命周期

### 窗口状态管理

```csharp
public class StatefulWindow : Formedge
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        // 恢复窗口状态
        RestoreWindowState();
    }
    
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // 保存窗口状态
        SaveWindowState();
        
        base.OnFormClosing(e);
    }
    
    private void SaveWindowState()
    {
        var settings = Properties.Settings.Default;
        settings.WindowLocation = Location;
        settings.WindowSize = Size;
        settings.WindowState = WindowState.ToString();
        settings.Save();
    }
    
    private void RestoreWindowState()
    {
        var settings = Properties.Settings.Default;
        
        if (!settings.WindowLocation.IsEmpty)
        {
            Location = settings.WindowLocation;
        }
        
        if (!settings.WindowSize.IsEmpty)
        {
            Size = settings.WindowSize;
        }
        
        if (Enum.TryParse<FormWindowState>(settings.WindowState, out var state))
        {
            WindowState = state;
        }
    }
}
```

### 窗口焦点管理

```csharp
public class FocusAwareWindow : Formedge
{
    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        
        // 窗口获得焦点时的处理
        UpdateTitleBarStyle(true);
    }
    
    protected override void OnDeactivate(EventArgs e)
    {
        base.OnDeactivate(e);
        
        // 窗口失去焦点时的处理
        UpdateTitleBarStyle(false);
    }
    
    private async void UpdateTitleBarStyle(bool isActive)
    {
        if (CoreWebView2 != null)
        {
            await ExecuteScriptAsync($"""
                document.body.classList.toggle('window-active', {isActive.ToString().ToLower()});
            """);
        }
    }
}
```