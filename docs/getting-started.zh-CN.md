# 快速入门

本指南将帮助您快速上手 WinFormedge ，创建您的第一个基于 WinFormedge 的现代化 WinForm 应用程序。

## 安装

### NuGet 包管理器

```bash
Install-Package WinFormedge
```

### .NET CLI

```bash
dotnet add package WinFormedge
```

### PackageReference

在您的 `.csproj` 文件中添加：

```xml
<PackageReference Include="WinFormedge" Version="1.1.*" />
```

## 创建第一个应用程序

使用 Visual Studio 创建一个新的 Windows Forms 应用程序项目，然后按照以下步骤集成 WinFormedge。

> [!NOTE]
> 确保您的项目目标框架为 `.NET 8.0` 或更高版本，并且本机已安装 WebView2 运行时（通常由 Windows 更新自动安装）。

### 1. 修改程序入口点

将传统的 `Application` 初始化替换为使用 WinFormedge 的构建器 `WinFormedgeApp`：

```csharp
using WinFormedge;

namespace MyApp;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var app = WinFormedgeApp.CreateAppBuilder()
            .UseDevTools()              // 启用开发者工具
            .UseWinFormedgeApp<MyApp>() // 指定应用程序启动类
            .Build();

        app.Run();
    }
}
```

### 2. 创建应用程序启动类

WinFormedge 启动类将负责主窗体的创建和应用程序生命周期管理，您可以选择使用 WinFormedge 的 Formedge 窗口或 传统 WinForm 的 Form 作为启动窗体。

```csharp
using WinFormedge;

namespace MyApp;

internal class MyApp : AppStartup
{
    protected override AppCreationAction? OnApplicationStartup(StartupSettings options)
    {
        // 创建并返回主窗口
        return options.UseMainWindow(new MainWindow());
    }

    // 可选：应用程序启动前的初始化
    protected override bool OnApplicationLaunched(string[] args)
    {
        // 执行启动前的初始化逻辑
        // 返回 false 将阻止应用程序启动
        return true;
    }

    // 可选：应用程序异常处理
    protected override void OnApplicationException(Exception? exception)
    {
        // 处理未捕获的异常
        MessageBox.Show($"应用程序发生错误: {exception?.Message}", "错误",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // 可选：应用程序终止处理
    protected override void OnApplicationTerminated()
    {
        // 执行清理工作
    }
}
```

### 3. 创建主窗口

```csharp
using WinFormedge;
using Microsoft.Web.WebView2.Core;

namespace MyApp;

internal class MainWindow : Formedge
{
    public MainWindow()
    {
        // 设置初始 URL
        Url = "https://embedded.appresource.local/";

        // 配置窗口属性
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;
        WindowTitle = "我的 WinFormedge 应用";

        // 允许全屏
        AllowFullscreen = true;

        // 设置透明背景
        BackColor = Color.Transparent;

        // 配置嵌入式资源映射
        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "embedded.appresource.local",
            ResourceAssembly = Assembly.GetExecutingAssembly(),
            DefaultFolderName = "wwwroot" // 嵌入式资源文件夹
        });

        // 绑定事件
        Load += MainWindow_Load;
        DOMContentLoaded += MainWindow_DOMContentLoaded;
    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        //在此方法中您可以自定义窗口设置

        // 使用默认窗口设置作为基础
        var settings = opts.UseDefaultWindow();

        // 将内容扩展到标题栏区域
        settings.ExtendsContentIntoTitleBar = true;

        // 设置窗口背景效果
        settings.SystemBackdropType = SystemBackdropType.Mica;

        // 设置最小尺寸
        MinimumSize = new Size(800, 600);

        return settings;
    }

    private void MainWindow_Load(object? sender, EventArgs e)
    {
        // 窗口和 WebView2 准备就绪
        Console.WriteLine("窗口已加载");
    }

    private void MainWindow_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
    {
        // DOM 内容已加载完成
        ExecuteScriptAsync("""
            (function() {
                console.log('Hello from WinFormedge!');
                document.body.style.fontFamily = 'Microsoft YaHei, sans-serif';
            })();
        """);
    }
}
```

### 4. 创建 Web 内容

本示例中使用嵌入式资源来托管 Web 内容，您也可以根据需要直接访问 Web 或使用文件系统资源等其他托管方式。

在项目中创建 `wwwroot` 文件夹，并添加您的 HTML、CSS 和 JavaScript 文件。

**wwwroot/index.html:**

```html
<!DOCTYPE html>
<html lang="zh-CN">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>WinFormedge 应用</title>
    <link rel="stylesheet" href="styles.css" />
  </head>
  <body>
    <div class="header" data-drag-region>
      <h1>我的 WinFormedge 应用</h1>
      <div class="window-controls">
        <button
          onclick="window.chrome.webview.hostObjects.windowControls.minimize()"
        >
          ─
        </button>
        <button
          onclick="window.chrome.webview.hostObjects.windowControls.maximize()"
        >
          □
        </button>
        <button
          onclick="window.chrome.webview.hostObjects.windowControls.close()"
        >
          ✕
        </button>
      </div>
    </div>

    <main>
      <p>欢迎使用 WinFormedge！</p>
      <p>这是一个集成了现代 Web 技术的桌面应用程序。</p>
    </main>

    <script src="app.js"></script>
  </body>
</html>
```

**wwwroot/styles.css:**

```css
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: "Microsoft YaHei", sans-serif;
  background: rgba(255, 255, 255, 0.1);
  color: #333;
  height: 100vh;
  overflow: hidden;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 20px;
  background: rgba(0, 0, 0, 0.1);
  backdrop-filter: blur(10px);
  app-region: drag; /* 使用这个 CSS 属性来允许拖动元素区域来移动窗口 */
}

.header h1 {
  font-size: 16px;
  font-weight: normal;
}

.window-controls button {
  background: none;
  border: none;
  color: #666;
  font-size: 14px;
  padding: 5px 10px;
  cursor: pointer;
  margin-left: 5px;
  app-region: no-drag; /* 防止按钮区域被拖动 */
}

.window-controls button:hover {
  background: rgba(0, 0, 0, 0.1);
}

main {
  padding: 40px;
  text-align: center;
}

main p {
  margin-bottom: 20px;
  font-size: 18px;
}
```

**wwwroot/app.js:**

```javascript
// WinFormedge 内置很多可控制窗体的 JavaScript API，以下简单展示一些用法

window.addEventListener("DOMContentLoaded", () => {
  window.addEventListener("windowresize", (e) => {
    const { width, height } = e.detail;
    console.log(`窗口已调整大小: ${width}x${height}`);
  });

  window.addEventListener("windowmove", (e) => {
    const { x, y } = e.detail;
    console.log(`窗口已移动到: X:${x} Y:${y}`);
  });
});
```

### 5. 配置项目文件

确保在 `.csproj` 文件中正确配置嵌入式资源：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <!-- ... -->

  <ItemGroup>
    <EmbeddedResource Include="wwwroot\**\*" />
  </ItemGroup>
</Project>
```

## 运行应用程序

按 `F5` 在 Visual Studio 中运行应用程序，您将看到一个现代化的窗口，具有：

- 透明的 Mica 背景效果
- 自定义的标题栏
- 响应式的 Web 内容
- 现代化的用户界面
