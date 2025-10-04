# 常见问题

本页面收集了 WinFormedge 使用过程中的常见问题和解决方案。

## 目录

- [安装和环境](#安装和环境)
- [开发问题](#开发问题)
- [运行时问题](#运行时问题)
- [性能问题](#性能问题)
- [部署问题](#部署问题)
- [疑难解答](#疑难解答)

---

## 安装和环境

### Q: 支持哪些 Windows 版本？

**A:** WinFormedge 支持以下 Windows 版本：

- **最低要求**: Windows 10 版本 1903 (May 2019 Update)
- **推荐版本**: Windows 11 (获得最佳体验和所有功能)

某些高级功能有特定要求：
- 系统背景效果（Blur/Acrylic）: Windows 10 版本 2004+
- Mica 效果: Windows 11
- 现代滚动条样式: Windows 11

### Q: 如何检查是否安装了 WebView2 Runtime？

**A:** 可以通过以下方式检查：

```csharp
public static bool IsWebView2RuntimeInstalled()
{
    try
    {
        var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
        return !string.IsNullOrEmpty(version);
    }
    catch
    {
        return false;
    }
}
```

或者在命令行中运行：
```cmd
reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}" /v pv
```

### Q: 项目无法编译，提示找不到 WinFormedge 命名空间？

**A:** 请检查以下几点：

1. 确认已安装 WinFormedge NuGet 包：
   ```xml
   <PackageReference Include="WinFormedge" Version="1.0.0" />
   ```

2. 确认项目目标框架正确：
   ```xml
   <TargetFramework>net8.0-windows</TargetFramework>
   <UseWindowsForms>true</UseWindowsForms>
   ```

3. 添加必要的 using 声明：
   ```csharp
   using WinFormedge;
   using Microsoft.Web.WebView2.Core;
   ```

---

## 开发问题

### Q: 嵌入式资源无法加载，显示 404 错误？

**A:** 检查以下配置：

1. **项目文件配置**：
   ```xml
   <ItemGroup>
     <EmbeddedResource Include="wwwroot\**\*" />
   </ItemGroup>
   ```

2. **资源映射配置**：
   ```csharp
   SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
   {
       Scheme = "https",
       HostName = "app.local",  // 确保与 URL 中的主机名匹配
       ResourceAssembly = Assembly.GetExecutingAssembly(),
       DefaultFolderName = "wwwroot"  // 确保文件夹名称正确
   });
   ```

3. **URL 路径**：
   ```csharp
   Url = "https://app.local/index.html";  // 主机名必须匹配
   ```

### Q: JavaScript 无法调用 C# 方法？

**A:** 确保正确配置了 COM 可见性：

```csharp
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class MyApiService
{
    public string GetData()
    {
        return "Hello from C#";
    }
}

// 在窗口 Load 事件中注册
private void OnLoad(object? sender, EventArgs e)
{
    CoreWebView2?.AddHostObjectToScript("myApi", new MyApiService());
}
```

JavaScript 调用方式：
```javascript
// 异步调用
window.chrome.webview.hostObjects.myApi.GetData().then(result => {
    console.log(result);
});

// 或使用 sync 版本（不推荐，可能阻塞 UI）
const result = window.chrome.webview.hostObjects.sync.myApi.GetData();
```

### Q: 窗口样式不生效？

**A:** 检查窗口配置方法：

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    
    // 确保在正确的时机设置属性
    settings.ExtendsContentIntoTitleBar = true;
    settings.SystemBackdropType = SystemBackdropType.Mica;
    
    // 基本属性应该在构造函数中设置
    Size = new Size(1200, 800);
    MinimumSize = new Size(800, 600);
    
    return settings;
}
```

---

## 运行时问题

### Q: 应用程序启动后窗口一片空白？

**A:** 可能的原因和解决方案：

1. **URL 配置错误**：
   ```csharp
   // 确保 URL 协议、主机名和路径正确
   Url = "https://app.local/index.html";
   ```

2. **资源映射问题**：检查 `SetVirtualHostNameToEmbeddedResourcesMapping` 配置

3. **HTML 文件问题**：确保 `wwwroot/index.html` 存在且内容正确

4. **启用开发者工具调试**：
   ```csharp
   var app = WinFormedgeApp.CreateAppBuilder()
       .UseDevTools()  // 启用开发者工具
       .Build();
   ```
   然后按 F12 打开开发者工具查看错误信息。

### Q: 应用程序崩溃并显示 "WebView2Environment is not initialized" 错误？

**A:** 这通常是因为过早访问 WebView2 相关功能导致的：

```csharp
public class MyWindow : Formedge
{
    public MyWindow()
    {
        // 在构造函数中只设置基本属性
        Url = "https://app.local/";
        Size = new Size(800, 600);
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        // 在 Load 事件中访问 CoreWebView2
        if (CoreWebView2 != null)
        {
            CoreWebView2.AddHostObjectToScript("api", new ApiService());
        }
    }
}
```

### Q: 窗口透明背景不生效？

**A:** 检查以下配置：

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    
    // 必须设置系统背景类型
    settings.SystemBackdropType = SystemBackdropType.Acrylic; // 或其他非 None 值
    settings.ExtendsContentIntoTitleBar = true;
    
    // 设置透明背景色
    BackColor = Color.Transparent;
    
    return settings;
}
```

CSS 中也需要设置透明背景：
```css
body {
    background: rgba(255, 255, 255, 0.1);
    backdrop-filter: blur(20px);
}
```

---

## 性能问题

### Q: 应用程序内存使用过高？

**A:** 优化建议：

1. **启用内存清理**：
   ```csharp
   private Timer? _memoryTimer;
   
   private void OnLoad(object? sender, EventArgs e)
   {
       _memoryTimer = new Timer(_ => {
           GC.Collect();
           GC.WaitForPendingFinalizers();
           GC.Collect();
       }, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
   }
   ```

2. **优化 JavaScript 内存使用**：
   ```javascript
   // 定期清理不用的对象
   setInterval(() => {
       if (window.gc) {
           window.gc();
       }
   }, 300000); // 5分钟
   ```

3. **使用缓存清理选项**：
   ```csharp
   var app = WinFormedgeApp.CreateAppBuilder()
       .CacheCleanup()  // 启动时清理缓存
       .Build();
   ```

### Q: 应用程序启动缓慢？

**A:** 性能优化方案：

1. **预加载关键资源**：
   ```csharp
   public async Task PreloadCriticalResources()
   {
       var tasks = new[]
       {
           PreloadResource("/css/app.css"),
           PreloadResource("/js/app.js"),
           PreloadResource("/images/logo.png")
       };
       await Task.WhenAll(tasks);
   }
   ```

2. **优化浏览器参数**：
   ```csharp
   protected override void ConfigureAdditionalBrowserArgs(NameValueCollection args)
   {
       args.Add("disable-background-timer-throttling", null);
       args.Add("disable-renderer-backgrounding", null);
       args.Add("enable-gpu", null);
   }
   ```

3. **延迟加载非关键功能**：
   ```javascript
   // 页面加载完成后再加载次要功能
   window.addEventListener('load', () => {
       setTimeout(() => {
           loadSecondaryFeatures();
       }, 100);
   });
   ```

---

## 部署问题

### Q: 发布的应用程序在其他机器上无法运行？

**A:** 检查部署配置：

1. **自包含部署**：
   ```xml
   <PropertyGroup>
     <SelfContained>true</SelfContained>
     <RuntimeIdentifier>win-x64</RuntimeIdentifier>
     <PublishSingleFile>true</PublishSingleFile>
   </PropertyGroup>
   ```

2. **WebView2 Runtime 检查**：
   ```csharp
   static void Main()
   {
       // 检查 WebView2 Runtime
       if (!IsWebView2RuntimeInstalled())
       {
           MessageBox.Show("请先安装 Microsoft Edge WebView2 Runtime");
           return;
       }
       
       // 正常启动应用程序
       ApplicationConfiguration.Initialize();
       // ...
   }
   ```

3. **资源文件包含**：确保所有嵌入式资源都正确包含在发布输出中

### Q: 应用程序图标不显示？

**A:** 配置应用程序图标：

```xml
<PropertyGroup>
  <ApplicationIcon>app.ico</ApplicationIcon>
</PropertyGroup>

<ItemGroup>
  <Content Include="app.ico" />
</ItemGroup>
```

代码中设置窗口图标：
```csharp
public MyWindow()
{
    Icon = new Icon("app.ico");
    // 或从嵌入式资源加载
    Icon = new Icon(Assembly.GetExecutingAssembly()
        .GetManifestResourceStream("MyApp.Resources.app.ico"));
}
```

---

## 疑难解答

### Q: 如何启用详细的错误日志？

**A:** 配置日志记录：

```csharp
public class DiagnosticApp : AppStartup
{
    protected override bool OnApplicationLaunched(string[] args)
    {
        // 启用控制台窗口（Debug 模式）
        #if DEBUG
        AllocConsole();
        #endif
        
        return true;
    }

    protected override void OnApplicationException(Exception? exception)
    {
        var logFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MyApp", "error.log"
        );
        
        Directory.CreateDirectory(Path.GetDirectoryName(logFile));
        File.AppendAllText(logFile, 
            $"[{DateTime.Now}] {exception}\n");
    }

    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();
}
```

### Q: 如何调试 JavaScript 和 C# 之间的交互？

**A:** 调试技巧：

1. **JavaScript 端**：
   ```javascript
   // 使用 console.log 记录调用
   console.log('调用 C# 方法');
   window.chrome.webview.hostObjects.api.getData()
       .then(result => {
           console.log('C# 返回结果:', result);
       })
       .catch(error => {
           console.error('调用失败:', error);
       });
   ```

2. **C# 端**：
   ```csharp
   [ComVisible(true)]
   public class DebugApiService
   {
       public string GetData()
       {
           Debug.WriteLine("GetData 方法被调用");
           try
           {
               var result = "some data";
               Debug.WriteLine($"返回结果: {result}");
               return result;
           }
           catch (Exception ex)
           {
               Debug.WriteLine($"方法执行出错: {ex}");
               throw;
           }
       }
   }
   ```

### Q: 如何处理跨域问题？

**A:** WinFormedge 中的跨域处理：

```csharp
protected override void ConfigureAdditionalBrowserArgs(NameValueCollection args)
{
    // 禁用同源策略（仅在开发环境中使用）
    #if DEBUG
    args.Add("disable-web-security", null);
    args.Add("disable-features", "VizDisplayCompositor");
    #endif
}

// 或者使用自定义资源处理器
public class CorsEnabledResourceHandler : WebResourceHandler
{
    protected override async Task<WebResourceResponse?> ProcessRequestAsync(WebResourceRequest request)
    {
        var response = await base.ProcessRequestAsync(request);
        
        if (response != null)
        {
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
        }
        
        return response;
    }
}
```

### 获得更多帮助

如果以上解决方案不能解决您的问题，可以通过以下方式获得帮助：

- **GitHub Issues**: [https://github.com/XuanchenLin/WinFormedge/issues](https://github.com/XuanchenLin/WinFormedge/issues)
- **官方文档**: [docs/README.md](README.md)
- **示例项目**: [examples/README.md](examples/README.md)

提交问题时，请包含以下信息：
- Windows 版本
- .NET 版本
- WinFormedge 版本
- 完整的错误消息
- 最小重现代码示例