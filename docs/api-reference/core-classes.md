# 核心类

本页面详细介绍了 WinFormedge 的核心类和它们的主要功能。

## WinFormedgeApp

`WinFormedgeApp` 是应用程序的主类，管理 WebView2 环境、应用程序设置和生命周期。

### 属性

#### WebView2Environment
```csharp
public CoreWebView2Environment WebView2Environment { get; }
```
获取已初始化的 WebView2 环境。

#### Culture
```csharp
public CultureInfo Culture { get; }
```
获取应用程序使用的文化信息。

#### AppDataFolder
```csharp
public string AppDataFolder { get; }
```
获取应用程序数据文件夹路径。

#### UserDataFolder
```csharp
public string UserDataFolder { get; }
```
获取浏览器数据的用户数据文件夹路径。

#### IsDarkMode
```csharp
internal bool IsDarkMode { get; }
```
获取应用程序当前是否处于暗色模式。

### 静态属性

#### Current
```csharp
public static WinFormedgeApp Current { get; }
```
获取当前的 `WinFormedgeApp` 实例。

### 静态方法

#### CreateAppBuilder
```csharp
public static AppBuilder CreateAppBuilder()
```
创建一个新的 `AppBuilder` 用于配置和构建 `WinFormedgeApp` 实例。

### 方法

#### Run
```csharp
public void Run()
```
运行 WinFormedge 应用程序，初始化环境并启动消息循环。

### 示例

```csharp
// 创建和配置应用程序
var app = WinFormedgeApp.CreateAppBuilder()
    .UseDevTools()
    .SetColorMode(SystemColorMode.Auto)
    .UseWinFormedgeApp<MyStartup>()
    .Build();

// 运行应用程序
app.Run();

// 在其他地方访问当前应用程序实例
var currentApp = WinFormedgeApp.Current;
Console.WriteLine($"应用数据目录: {currentApp.AppDataFolder}");
```

---

## AppBuilder

`AppBuilder` 提供了流畅的 API 来配置 `WinFormedgeApp` 实例。

### 方法

#### UseCustomBrowserExecutablePath
```csharp
public AppBuilder UseCustomBrowserExecutablePath(string path)
```
设置自定义的浏览器可执行文件路径。

**参数:**
- `path`: 自定义浏览器可执行文件的路径

#### UseCustomAppDataFolder
```csharp
public AppBuilder UseCustomAppDataFolder(string appDataFolder)
```
设置自定义的应用程序数据文件夹。

**参数:**
- `appDataFolder`: 自定义应用数据文件夹路径

#### UseCulture
```csharp
public AppBuilder UseCulture(string cultureName)
```
设置应用程序的文化信息。

**参数:**
- `cultureName`: 文化名称，如 "zh-CN", "en-US"

#### UseDevTools
```csharp
public AppBuilder UseDevTools()
```
启用开发者工具支持。

#### SetColorMode
```csharp
public AppBuilder SetColorMode(SystemColorMode colorMode)
```
设置系统颜色模式。

**参数:**
- `colorMode`: 系统颜色模式枚举值

#### CacheCleanup
```csharp
public AppBuilder CacheCleanup()
```
启用启动时的缓存清理。

#### UseWinFormedgeApp<TApp>
```csharp
public AppBuilder UseWinFormedgeApp<TApp>() where TApp : AppStartup, new()
```
设置应用程序启动逻辑类型。

**泛型参数:**
- `TApp`: 继承自 `AppStartup` 的类型

#### UseModernStyleScrollbar
```csharp
public AppBuilder UseModernStyleScrollbar()
```
启用现代样式（Fluent Overlay）滚动条。

#### Build
```csharp
public WinFormedgeApp Build()
```
构建并返回配置好的 `WinFormedgeApp` 实例。

### 示例

```csharp
var app = WinFormedgeApp.CreateAppBuilder()
    .UseCulture("zh-CN")                    // 设置中文文化
    .UseDevTools()                          // 启用开发工具
    .SetColorMode(SystemColorMode.Dark)     // 设置暗色模式
    .UseModernStyleScrollbar()              // 使用现代滚动条
    .CacheCleanup()                         // 启动时清理缓存
    .UseCustomAppDataFolder(@"C:\MyApp\Data") // 自定义数据目录
    .UseWinFormedgeApp<MyStartupClass>()    // 设置启动类
    .Build();
```

---

## AppStartup

`AppStartup` 是应用程序启动逻辑的基类，定义了应用程序生命周期的关键事件。

### 虚拟方法

#### OnApplicationLaunched
```csharp
protected virtual bool OnApplicationLaunched(string[] args)
```
应用程序启动前调用，可用于早期初始化。

**参数:**
- `args`: 命令行参数

**返回值:**
- `bool`: 返回 `false` 将阻止应用程序继续启动

#### OnApplicationStartup
```csharp
protected abstract AppCreationAction? OnApplicationStartup(StartupSettings options)
```
**必须重写**。定义应用程序启动时的行为。

**参数:**
- `options`: 启动设置对象

**返回值:**
- `AppCreationAction?`: 应用程序创建动作，返回 `null` 将退出应用程序

#### OnApplicationException
```csharp
protected virtual void OnApplicationException(Exception? exception)
```
未捕获异常时调用。

**参数:**
- `exception`: 异常对象

#### OnApplicationTerminated
```csharp
protected virtual void OnApplicationTerminated()
```
应用程序终止时调用，用于清理资源。

#### ConfigureAdditionalBrowserArgs
```csharp
protected virtual void ConfigureAdditionalBrowserArgs(NameValueCollection args)
```
配置额外的浏览器命令行参数。

**参数:**
- `args`: 参数集合

#### ConfigureSchemeRegistrations
```csharp
protected virtual void ConfigureSchemeRegistrations(IList<CoreWebView2CustomSchemeRegistration> schemeRegistrations)
```
配置自定义协议注册。

**参数:**
- `schemeRegistrations`: 协议注册列表

### 示例

```csharp
public class MyAppStartup : AppStartup
{
    protected override bool OnApplicationLaunched(string[] args)
    {
        // 检查是否已有实例运行
        if (IsAnotherInstanceRunning())
        {
            MessageBox.Show("应用程序已在运行");
            return false; // 阻止启动
        }
        
        return true;
    }

    protected override AppCreationAction? OnApplicationStartup(StartupSettings options)
    {
        // 可以在这里做用户登录验证等
        if (!AuthenticateUser())
        {
            return null; // 退出应用程序
        }

        // 创建主窗口
        return options.UseMainWindow(new MainWindow());
    }

    protected override void OnApplicationException(Exception? exception)
    {
        // 记录异常日志
        LogException(exception);
        
        // 显示用户友好的错误消息
        MessageBox.Show($"应用程序遇到错误: {exception?.Message}", 
                       "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    protected override void OnApplicationTerminated()
    {
        // 保存用户设置
        SaveUserSettings();
        
        // 清理临时文件
        CleanupTempFiles();
    }

    protected override void ConfigureAdditionalBrowserArgs(NameValueCollection args)
    {
        // 添加自定义浏览器参数
        args.Add("disable-web-security", null);
        args.Add("allow-running-insecure-content", null);
    }

    private bool IsAnotherInstanceRunning() { /* 实现 */ }
    private bool AuthenticateUser() { /* 实现 */ }
    private void LogException(Exception? ex) { /* 实现 */ }
    private void SaveUserSettings() { /* 实现 */ }
    private void CleanupTempFiles() { /* 实现 */ }
}
```

---

## Formedge

`Formedge` 是所有窗口的基类，结合了 Windows Forms 和 WebView2 的功能。

### 继承关系
```
Form -> FormBase -> Formedge
```

### 主要属性

#### Url
```csharp
public string? Url { get; set; }
```
获取或设置 WebView2 的初始 URL。

#### WindowTitle
```csharp
public string? WindowTitle { get; set; }
```
获取或设置窗口标题。

#### AllowFullscreen
```csharp
public bool AllowFullscreen { get; set; }
```
获取或设置是否允许全屏模式。

#### ShowDocumentTitle
```csharp
public bool ShowDocumentTitle { get; set; }
```
获取或设置是否显示文档标题作为窗口标题。

#### CoreWebView2
```csharp
public CoreWebView2? CoreWebView2 { get; }
```
获取底层的 CoreWebView2 实例。

### 主要方法

#### SetVirtualHostNameToEmbeddedResourcesMapping
```csharp
public void SetVirtualHostNameToEmbeddedResourcesMapping(EmbeddedFileResourceOptions options)
```
设置虚拟主机名到嵌入式资源的映射。

#### ExecuteScriptAsync
```csharp
public async Task<string> ExecuteScriptAsync(string javaScript)
```
异步执行 JavaScript 代码。

#### ConfigureWindowSettings (抽象方法)
```csharp
protected abstract WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
```
**必须重写**。配置窗口设置。

### 主要事件

#### Load
```csharp
public event EventHandler? Load
```
窗口加载完成时触发。

#### DOMContentLoaded
```csharp
public event EventHandler<CoreWebView2DOMContentLoadedEventArgs>? DOMContentLoaded
```
DOM 内容加载完成时触发。

#### NavigationStarting
```csharp
public event EventHandler<CoreWebView2NavigationStartingEventArgs>? NavigationStarting
```
导航开始时触发。

#### NavigationCompleted
```csharp
public event EventHandler<CoreWebView2NavigationCompletedEventArgs>? NavigationCompleted
```
导航完成时触发。

### 示例

```csharp
public class MyWindow : Formedge
{
    public MyWindow()
    {
        // 基本配置
        Url = "https://myapp.local/";
        WindowTitle = "我的应用";
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;
        AllowFullscreen = true;
        ShowDocumentTitle = false;

        // 设置嵌入式资源
        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "myapp.local",
            ResourceAssembly = Assembly.GetExecutingAssembly(),
            DefaultFolderName = "wwwroot"
        });

        // 绑定事件
        Load += OnLoad;
        DOMContentLoaded += OnDOMContentLoaded;
        NavigationStarting += OnNavigationStarting;
    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var settings = opts.UseDefaultWindow();
        settings.ExtendsContentIntoTitleBar = true;
        settings.SystemBackdropType = SystemBackdropType.Mica;
        settings.Resizable = true;
        
        MinimumSize = new Size(800, 600);
        
        return settings;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        Console.WriteLine("窗口已加载");
    }

    private async void OnDOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
    {
        // 注入初始化脚本
        await ExecuteScriptAsync("""
            console.log('应用程序已初始化');
            document.body.classList.add('app-ready');
        """);
    }

    private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        Console.WriteLine($"开始导航到: {e.Uri}");
        
        // 可以取消导航
        if (e.Uri.StartsWith("http://blocked.site/"))
        {
            e.Cancel = true;
        }
    }
}
```