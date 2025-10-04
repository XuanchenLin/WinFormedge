# 高级特性

本页面介绍 WinFormedge 的高级功能和特性，包括性能优化、安全配置、自定义扩展等内容。

## 目录

- [性能优化](#性能优化)
- [安全配置](#安全配置)
- [自定义扩展](#自定义扩展)
- [调试和开发工具](#调试和开发工具)
- [部署和分发](#部署和分发)
- [与原生 API 集成](#与原生-api-集成)
- [多线程和异步处理](#多线程和异步处理)

---

## 性能优化

### WebView2 性能调优

#### 浏览器参数优化

```csharp
public class OptimizedApp : AppStartup
{
    protected override void ConfigureAdditionalBrowserArgs(NameValueCollection args)
    {
        // 启用 GPU 加速
        args.Add("enable-gpu", null);
        args.Add("enable-gpu-compositing", null);
        
        // 优化内存使用
        args.Add("max-old-space-size", "512");
        args.Add("memory-pressure-off", null);
        
        // 禁用不必要的功能
        args.Add("disable-background-timer-throttling", null);
        args.Add("disable-renderer-backgrounding", null);
        args.Add("disable-backgrounding-occluded-windows", null);
        
        // 网络优化
        args.Add("enable-quic", null);
        args.Add("enable-tcp-fast-open", null);
    }
}
```

#### 资源预加载和缓存

```csharp
public class PreloadingResourceHandler : WebResourceHandler
{
    private readonly Dictionary<string, byte[]> _preloadedResources = new();
    private readonly SemaphoreSlim _preloadSemaphore = new(1, 1);

    public async Task PreloadCriticalResources()
    {
        await _preloadSemaphore.WaitAsync();
        try
        {
            var criticalResources = new[]
            {
                "/css/app.css",
                "/js/app.js",
                "/images/logo.png"
            };

            var tasks = criticalResources.Select(PreloadResource);
            await Task.WhenAll(tasks);
        }
        finally
        {
            _preloadSemaphore.Release();
        }
    }

    private async Task PreloadResource(string path)
    {
        try
        {
            var content = await LoadResourceFromAssembly(path);
            if (content != null)
            {
                _preloadedResources[path] = content;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"预加载资源失败 {path}: {ex.Message}");
        }
    }

    protected override async Task<WebResourceResponse?> ProcessRequestAsync(WebResourceRequest request)
    {
        var uri = new Uri(request.Uri);
        var path = uri.AbsolutePath;

        // 优先使用预加载的资源
        if (_preloadedResources.TryGetValue(path, out var preloadedContent))
        {
            return CreateResponse(preloadedContent, GetMimeType(path));
        }

        return await base.ProcessRequestAsync(request);
    }
}
```

#### 内存管理

```csharp
public class MemoryOptimizedWindow : Formedge
{
    private Timer? _memoryCleanupTimer;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        // 定期清理内存
        _memoryCleanupTimer = new Timer(CleanupMemory, null, 
            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    private void CleanupMemory(object? state)
    {
        // 清理 JavaScript 内存
        _ = ExecuteScriptAsync("""
            if (window.gc) {
                window.gc();
            }
            
            // 清理不必要的缓存
            if ('caches' in window) {
                caches.keys().then(names => {
                    names.forEach(name => {
                        if (name.includes('temp')) {
                            caches.delete(name);
                        }
                    });
                });
            }
        """);

        // 强制 .NET 垃圾回收
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _memoryCleanupTimer?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

---

## 安全配置

### 内容安全策略 (CSP)

```csharp
public class SecureResourceHandler : WebResourceHandler
{
    protected override async Task<WebResourceResponse?> ProcessRequestAsync(WebResourceRequest request)
    {
        var response = await base.ProcessRequestAsync(request);
        
        if (response != null && IsHtmlContent(request.Uri))
        {
            // 添加安全头
            response.Headers["Content-Security-Policy"] = 
                "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:;";
            
            response.Headers["X-Frame-Options"] = "DENY";
            response.Headers["X-Content-Type-Options"] = "nosniff";
            response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        }
        
        return response;
    }

    private bool IsHtmlContent(string uri)
    {
        return uri.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
               uri.EndsWith("/", StringComparison.OrdinalIgnoreCase);
    }
}
```

### JavaScript 对象安全暴露

```csharp
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class SecureApiService
{
    private readonly HashSet<string> _allowedOrigins = new()
    {
        "https://app.local",
        "https://secure.local"
    };

    public string? GetUserData(string origin)
    {
        if (!_allowedOrigins.Contains(origin))
        {
            throw new UnauthorizedAccessException("不允许的来源");
        }

        return JsonSerializer.Serialize(new
        {
            Name = "用户",
            Role = "Admin"
        });
    }

    public bool ExecuteSecureAction(string action, string origin, string signature)
    {
        // 验证来源
        if (!_allowedOrigins.Contains(origin))
        {
            return false;
        }

        // 验证签名
        if (!VerifySignature(action, signature))
        {
            return false;
        }

        // 执行安全操作
        return PerformAction(action);
    }

    private bool VerifySignature(string data, string signature)
    {
        // 实现签名验证逻辑
        // 可以使用 HMAC、JWT 或其他加密方法
        return true; // 简化示例
    }

    private bool PerformAction(string action)
    {
        // 实现具体的业务逻辑
        return true;
    }
}
```

### 文件访问控制

```csharp
public class SecureFileService
{
    private readonly string _allowedBasePath;
    private readonly HashSet<string> _allowedExtensions = new()
    {
        ".txt", ".json", ".xml", ".csv", ".log"
    };

    public SecureFileService()
    {
        _allowedBasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "MyApp"
        );
        
        // 确保目录存在
        Directory.CreateDirectory(_allowedBasePath);
    }

    public string? ReadFile(string relativePath)
    {
        var fullPath = GetSecurePath(relativePath);
        if (fullPath == null || !File.Exists(fullPath))
        {
            return null;
        }

        try
        {
            return File.ReadAllText(fullPath);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"文件读取失败: {ex.Message}");
            return null;
        }
    }

    public bool WriteFile(string relativePath, string content)
    {
        var fullPath = GetSecurePath(relativePath);
        if (fullPath == null)
        {
            return false;
        }

        try
        {
            var directory = Path.GetDirectoryName(fullPath);
            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(fullPath, content);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"文件写入失败: {ex.Message}");
            return false;
        }
    }

    private string? GetSecurePath(string relativePath)
    {
        try
        {
            // 标准化路径
            var normalizedPath = Path.GetFullPath(
                Path.Combine(_allowedBasePath, relativePath)
            );

            // 检查路径是否在允许的基础路径内
            if (!normalizedPath.StartsWith(_allowedBasePath))
            {
                Debug.WriteLine($"路径访问被拒绝: {normalizedPath}");
                return null;
            }

            // 检查文件扩展名
            var extension = Path.GetExtension(normalizedPath);
            if (!_allowedExtensions.Contains(extension.ToLower()))
            {
                Debug.WriteLine($"文件类型不被允许: {extension}");
                return null;
            }

            return normalizedPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"路径验证失败: {ex.Message}");
            return null;
        }
    }
}
```

---

## 自定义扩展

### 插件系统

```csharp
public interface IWinFormedgePlugin
{
    string Name { get; }
    string Version { get; }
    void Initialize(IPluginContext context);
    void Shutdown();
}

public interface IPluginContext
{
    void RegisterService<T>(T service) where T : class;
    T? GetService<T>() where T : class;
    void RegisterResourceHandler(string scheme, string hostname, WebResourceHandler handler);
}

public class PluginManager
{
    private readonly List<IWinFormedgePlugin> _plugins = new();
    private readonly Dictionary<Type, object> _services = new();
    private readonly PluginContext _context;

    public PluginManager()
    {
        _context = new PluginContext(this);
    }

    public void LoadPlugins(string pluginDirectory)
    {
        if (!Directory.Exists(pluginDirectory))
        {
            return;
        }

        var dllFiles = Directory.GetFiles(pluginDirectory, "*.dll");
        
        foreach (var dllFile in dllFiles)
        {
            try
            {
                LoadPluginFromAssembly(dllFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"插件加载失败 {dllFile}: {ex.Message}");
            }
        }
    }

    private void LoadPluginFromAssembly(string assemblyPath)
    {
        var assembly = Assembly.LoadFrom(assemblyPath);
        var pluginTypes = assembly.GetTypes()
            .Where(t => typeof(IWinFormedgePlugin).IsAssignableFrom(t) && !t.IsInterface)
            .ToList();

        foreach (var pluginType in pluginTypes)
        {
            if (Activator.CreateInstance(pluginType) is IWinFormedgePlugin plugin)
            {
                plugin.Initialize(_context);
                _plugins.Add(plugin);
                
                Debug.WriteLine($"插件已加载: {plugin.Name} v{plugin.Version}");
            }
        }
    }

    public void RegisterService<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    public T? GetService<T>() where T : class
    {
        return _services.TryGetValue(typeof(T), out var service) ? service as T : null;
    }

    public void Shutdown()
    {
        foreach (var plugin in _plugins)
        {
            try
            {
                plugin.Shutdown();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"插件关闭失败 {plugin.Name}: {ex.Message}");
            }
        }
        
        _plugins.Clear();
        _services.Clear();
    }

    private class PluginContext : IPluginContext
    {
        private readonly PluginManager _manager;

        public PluginContext(PluginManager manager)
        {
            _manager = manager;
        }

        public void RegisterService<T>(T service) where T : class
        {
            _manager.RegisterService(service);
        }

        public T? GetService<T>() where T : class
        {
            return _manager.GetService<T>();
        }

        public void RegisterResourceHandler(string scheme, string hostname, WebResourceHandler handler)
        {
            // 实现资源处理器注册逻辑
        }
    }
}
```

### 自定义主题系统

```csharp
public class ThemeManager
{
    private readonly Dictionary<string, Theme> _themes = new();
    private Theme? _currentTheme;
    private readonly List<Formedge> _windows = new();

    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public ThemeManager()
    {
        LoadBuiltInThemes();
    }

    private void LoadBuiltInThemes()
    {
        _themes["light"] = new Theme
        {
            Name = "浅色主题",
            BackgroundColor = "#FFFFFF",
            TextColor = "#000000",
            AccentColor = "#0078D4",
            Variables = new Dictionary<string, string>
            {
                ["--primary-bg"] = "#FFFFFF",
                ["--secondary-bg"] = "#F5F5F5",
                ["--text-primary"] = "#000000",
                ["--text-secondary"] = "#666666",
                ["--accent"] = "#0078D4",
                ["--border"] = "#E5E5E5"
            }
        };

        _themes["dark"] = new Theme
        {
            Name = "深色主题",
            BackgroundColor = "#1E1E1E",
            TextColor = "#FFFFFF",
            AccentColor = "#60A5FA",
            Variables = new Dictionary<string, string>
            {
                ["--primary-bg"] = "#1E1E1E",
                ["--secondary-bg"] = "#2D2D2D",
                ["--text-primary"] = "#FFFFFF",
                ["--text-secondary"] = "#CCCCCC",
                ["--accent"] = "#60A5FA",
                ["--border"] = "#404040"
            }
        };
    }

    public void RegisterWindow(Formedge window)
    {
        _windows.Add(window);
        window.FormClosed += (s, e) => _windows.Remove(window);
        
        // 应用当前主题
        if (_currentTheme != null)
        {
            ApplyThemeToWindow(window, _currentTheme);
        }
    }

    public async void SetTheme(string themeName)
    {
        if (!_themes.TryGetValue(themeName, out var theme))
        {
            return;
        }

        _currentTheme = theme;
        
        // 应用到所有窗口
        var tasks = _windows.Select(window => ApplyThemeToWindow(window, theme));
        await Task.WhenAll(tasks);
        
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme));
    }

    private async Task ApplyThemeToWindow(Formedge window, Theme theme)
    {
        try
        {
            // 生成 CSS 变量
            var cssVariables = string.Join("\n", 
                theme.Variables.Select(kv => $"    {kv.Key}: {kv.Value};"));

            var themeCSS = $"""
                :root {{
                {cssVariables}
                }}
                
                body {{
                    background-color: var(--primary-bg);
                    color: var(--text-primary);
                    transition: background-color 0.3s, color 0.3s;
                }}
                """;

            // 注入主题样式
            await window.ExecuteScriptAsync($"""
                (function() {{
                    let themeStyle = document.getElementById('dynamic-theme');
                    if (!themeStyle) {{
                        themeStyle = document.createElement('style');
                        themeStyle.id = 'dynamic-theme';
                        document.head.appendChild(themeStyle);
                    }}
                    themeStyle.textContent = `{themeCSS}`;
                }})();
            """);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"主题应用失败: {ex.Message}");
        }
    }

    public void LoadCustomTheme(string themePath)
    {
        try
        {
            var themeJson = File.ReadAllText(themePath);
            var theme = JsonSerializer.Deserialize<Theme>(themeJson);
            
            if (theme != null && !string.IsNullOrEmpty(theme.Name))
            {
                _themes[theme.Name.ToLower()] = theme;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"自定义主题加载失败: {ex.Message}");
        }
    }
}

public class Theme
{
    public string Name { get; set; } = "";
    public string BackgroundColor { get; set; } = "";
    public string TextColor { get; set; } = "";
    public string AccentColor { get; set; } = "";
    public Dictionary<string, string> Variables { get; set; } = new();
}

public class ThemeChangedEventArgs : EventArgs
{
    public Theme Theme { get; }
    
    public ThemeChangedEventArgs(Theme theme)
    {
        Theme = theme;
    }
}
```

---

## 调试和开发工具

### 高级调试功能

```csharp
public class DebugWindow : Formedge
{
    private bool _debugMode;

    public DebugWindow()
    {
        #if DEBUG
        _debugMode = true;
        #endif
        
        if (_debugMode)
        {
            SetupDebugFeatures();
        }
    }

    private void SetupDebugFeatures()
    {
        // 启用右键菜单
        Load += async (s, e) =>
        {
            if (CoreWebView2 != null)
            {
                CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                CoreWebView2.Settings.AreDevToolsEnabled = true;
                
                // 添加调试快捷键
                await ExecuteScriptAsync("""
                    document.addEventListener('keydown', function(e) {
                        // F12 - 开发者工具
                        if (e.key === 'F12') {
                            window.chrome.webview.hostObjects.debug.openDevTools();
                        }
                        
                        // Ctrl+Shift+I - 开发者工具
                        if (e.ctrlKey && e.shiftKey && e.key === 'I') {
                            window.chrome.webview.hostObjects.debug.openDevTools();
                        }
                        
                        // Ctrl+R - 刷新
                        if (e.ctrlKey && e.key === 'r') {
                            window.location.reload();
                        }
                    });
                """);
                
                // 注册调试对象
                CoreWebView2.AddHostObjectToScript("debug", new DebugService());
            }
        };
    }
}

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class DebugService
{
    public void OpenDevTools()
    {
        // 在实际实现中打开开发者工具
        Debug.WriteLine("开发者工具请求");
    }

    public void LogMessage(string level, string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        Debug.WriteLine($"[{timestamp}] [{level.ToUpper()}] {message}");
    }

    public string GetSystemInfo()
    {
        return JsonSerializer.Serialize(new
        {
            OS = Environment.OSVersion.ToString(),
            CLR = Environment.Version.ToString(),
            WorkingSet = Environment.WorkingSet,
            ProcessorCount = Environment.ProcessorCount,
            MachineName = Environment.MachineName,
            UserName = Environment.UserName
        });
    }
}
```

### 性能监控

```csharp
public class PerformanceMonitor
{
    private readonly PerformanceCounter _cpuCounter = new("Processor", "% Processor Time", "_Total");
    private readonly PerformanceCounter _ramCounter = new("Memory", "Available MBytes");
    private readonly Timer _monitorTimer;
    private readonly List<PerformanceSnapshot> _snapshots = new();

    public event EventHandler<PerformanceSnapshot>? PerformanceUpdated;

    public PerformanceMonitor()
    {
        _monitorTimer = new Timer(CollectMetrics, null, 
            TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    private void CollectMetrics(object? state)
    {
        try
        {
            var snapshot = new PerformanceSnapshot
            {
                Timestamp = DateTime.Now,
                CpuUsage = _cpuCounter.NextValue(),
                AvailableMemoryMB = _ramCounter.NextValue(),
                WorkingSetMB = Environment.WorkingSet / 1024 / 1024,
                GCMemoryMB = GC.GetTotalMemory(false) / 1024 / 1024
            };

            _snapshots.Add(snapshot);
            
            // 只保留最近100个快照
            if (_snapshots.Count > 100)
            {
                _snapshots.RemoveAt(0);
            }

            PerformanceUpdated?.Invoke(this, snapshot);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"性能监控错误: {ex.Message}");
        }
    }

    public IReadOnlyList<PerformanceSnapshot> GetSnapshots() => _snapshots.AsReadOnly();

    public void Dispose()
    {
        _monitorTimer?.Dispose();
        _cpuCounter?.Dispose();
        _ramCounter?.Dispose();
    }
}

public class PerformanceSnapshot
{
    public DateTime Timestamp { get; set; }
    public float CpuUsage { get; set; }
    public float AvailableMemoryMB { get; set; }
    public long WorkingSetMB { get; set; }
    public long GCMemoryMB { get; set; }
}
```

---

## 部署和分发

### 应用程序打包

```csharp
// 在项目文件中配置发布设置
/*
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>true</SelfContained>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <PublishTrimmed>true</PublishTrimmed>
    <TrimMode>link</TrimMode>
  </PropertyGroup>
</Project>
*/

public class DeploymentHelper
{
    public static void CheckWebView2Runtime()
    {
        try
        {
            var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            if (string.IsNullOrEmpty(version))
            {
                PromptWebView2Installation();
            }
        }
        catch
        {
            PromptWebView2Installation();
        }
    }

    private static void PromptWebView2Installation()
    {
        var result = MessageBox.Show(
            "此应用程序需要 Microsoft Edge WebView2 Runtime。\n是否要下载并安装？",
            "缺少必需组件",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        if (result == DialogResult.Yes)
        {
            OpenWebView2DownloadPage();
        }
        else
        {
            Environment.Exit(1);
        }
    }

    private static void OpenWebView2DownloadPage()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://developer.microsoft.com/en-us/microsoft-edge/webview2/",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"无法打开下载页面: {ex.Message}", "错误", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
```

---

## 与原生 API 集成

### Windows API 调用

```csharp
public class NativeApiService
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("kernel32.dll")]
    private static extern bool Beep(uint dwFreq, uint dwDuration);

    public string GetActiveWindowTitle()
    {
        var handle = GetForegroundWindow();
        var text = new StringBuilder(256);
        GetWindowText(handle, text, text.Capacity);
        return text.ToString();
    }

    public void PlaySystemBeep(uint frequency = 800, uint duration = 200)
    {
        Beep(frequency, duration);
    }

    public void MinimizeAllWindows()
    {
        // 发送 Win+M 快捷键
        SendKeys.SendWait("^{ESC}m");
    }
}
```

### 系统托盘集成

```csharp
public class SystemTrayManager
{
    private NotifyIcon? _notifyIcon;
    private ContextMenuStrip? _contextMenu;

    public void Initialize()
    {
        _contextMenu = new ContextMenuStrip();
        _contextMenu.Items.Add("显示", null, OnShow);
        _contextMenu.Items.Add("隐藏", null, OnHide);
        _contextMenu.Items.Add("-");
        _contextMenu.Items.Add("退出", null, OnExit);

        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "WinFormedge 应用",
            ContextMenuStrip = _contextMenu,
            Visible = true
        };

        _notifyIcon.DoubleClick += OnDoubleClick;
    }

    private void OnShow(object? sender, EventArgs e)
    {
        // 显示主窗口
        Application.OpenForms.Cast<Form>().FirstOrDefault()?.Show();
    }

    private void OnHide(object? sender, EventArgs e)
    {
        // 隐藏主窗口
        Application.OpenForms.Cast<Form>().FirstOrDefault()?.Hide();
    }

    private void OnDoubleClick(object? sender, EventArgs e)
    {
        OnShow(sender, e);
    }

    private void OnExit(object? sender, EventArgs e)
    {
        Application.Exit();
    }

    public void Dispose()
    {
        _notifyIcon?.Dispose();
        _contextMenu?.Dispose();
    }
}
```

---

## 多线程和异步处理

### 后台任务管理

```csharp
public class BackgroundTaskManager
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly List<Task> _backgroundTasks = new();
    private readonly SemaphoreSlim _taskSemaphore = new(10); // 最多10个并发任务

    public async Task<T> RunBackgroundTask<T>(Func<CancellationToken, Task<T>> taskFunc)
    {
        await _taskSemaphore.WaitAsync(_cancellationTokenSource.Token);

        try
        {
            var task = taskFunc(_cancellationTokenSource.Token);
            _backgroundTasks.Add(task);
            
            var result = await task;
            
            _backgroundTasks.Remove(task);
            return result;
        }
        finally
        {
            _taskSemaphore.Release();
        }
    }

    public void StartPeriodicTask(Func<CancellationToken, Task> taskFunc, TimeSpan interval)
    {
        var task = Task.Run(async () =>
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    await taskFunc(_cancellationTokenSource.Token);
                    await Task.Delay(interval, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"定期任务错误: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(5), _cancellationTokenSource.Token);
                }
            }
        }, _cancellationTokenSource.Token);

        _backgroundTasks.Add(task);
    }

    public async Task StopAllTasks()
    {
        _cancellationTokenSource.Cancel();
        
        try
        {
            await Task.WhenAll(_backgroundTasks);
        }
        catch (OperationCanceledException)
        {
            // 预期的取消异常
        }
    }

    public void Dispose()
    {
        StopAllTasks().Wait(TimeSpan.FromSeconds(5));
        _cancellationTokenSource.Dispose();
        _taskSemaphore.Dispose();
    }
}
```

### 线程安全的 UI 更新

```csharp
public class ThreadSafeUIUpdater
{
    private readonly Formedge _window;
    private readonly SynchronizationContext _uiContext;

    public ThreadSafeUIUpdater(Formedge window)
    {
        _window = window;
        _uiContext = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();
    }

    public async Task UpdateUIAsync(string script)
    {
        if (_window.InvokeRequired)
        {
            await Task.Run(() =>
            {
                _uiContext.Post(async _ =>
                {
                    await _window.ExecuteScriptAsync(script);
                }, null);
            });
        }
        else
        {
            await _window.ExecuteScriptAsync(script);
        }
    }

    public void UpdateUI(Action uiAction)
    {
        if (_window.InvokeRequired)
        {
            _window.Invoke(uiAction);
        }
        else
        {
            uiAction();
        }
    }

    public async Task<T> GetFromUIAsync<T>(Func<Task<T>> uiFunc)
    {
        if (_window.InvokeRequired)
        {
            return await Task.Run(async () =>
            {
                var tcs = new TaskCompletionSource<T>();
                _uiContext.Post(async _ =>
                {
                    try
                    {
                        var result = await uiFunc();
                        tcs.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }, null);
                return await tcs.Task;
            });
        }
        else
        {
            return await uiFunc();
        }
    }
}
```

这些高级特性为 WinFormedge 应用程序提供了强大的扩展能力和优化选项，帮助开发者创建更加专业和高性能的桌面应用程序。