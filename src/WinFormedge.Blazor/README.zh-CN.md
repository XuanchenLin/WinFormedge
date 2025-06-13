# WinFormedge 的 Blazor Hybrid 插件


## ✨ 关于

Blazor Hybrid 插件使 WinFormedge 基于 Windows Forms 的应用程序能够无缝集成 Blazor 组件。通过这个插件，开发者可以利用 Blazor 的现代 Web UI 框架与传统的 WinForms 相结合，使用 C#、HTML 和 CSS 构建交互式、基于组件的用户界面。这种混合方法将 Web 技术的灵活性与原生 Windows 应用程序的性能和能力相结合，使得构建丰富的跨技术桌面解决方案变得更加容易。

## 📖 使用方法

要使用 Blazor Hybrid 插件，请按照以下步骤操作：

**安装 NuGet 包**：将 `WinFormedge.Blazor` 包添加到你的 WinFormedge 项目中。

在 **解决方案资源管理器** 中，右键点击项目名称，选择“编辑项目文件”以打开项目文件。

在项目文件的顶部，将 SDK 更改为 `Microsoft.NET.Sdk.Razor`：

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
```


在项目根目录下添加一个 `_Imports.razor` 文件，并添加如下 @using 指令以引用 Microsoft.AspNetCore.Components.Web。

```razor
@using Microsoft.AspNetCore.Components.Web
```

添加一个 `wwwroot` 文件夹到项目的更目录。创建一个 `index.html` 文件到 `wwwroot` 文件夹，并使用以下标记。

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>WinFormsBlazor</title>
    <base href="/" />
    <link href="css/bootstrap/bootstrap.min.css" rel="stylesheet" />
    <link href="css/app.css" rel="stylesheet" />
</head>

<body>

    <div id="app">Loading...</div>

    <div id="blazor-error-ui" data-nosnippet>
        An unhandled error has occurred.
        <a href="" class="reload">Reload</a>
        <a class="dismiss">🗙</a>
    </div>

    <script src="_framework/blazor.webview.js"></script>

</body>
</html>
```

在 `wwwroot` 文件夹内创建一个 `css` 文件夹来存放样式表。

添加一个 `app.css` 样式表到 `wwwroot/css` 文件夹，并使用以下内容。

```css
html, body {
    font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
}

h1:focus {
    outline: none;
}

a, .btn-link {
    color: #0071c1;
}

.btn-primary {
    color: #fff;
    background-color: #1b6ec2;
    border-color: #1861ac;
}

.valid.modified:not([type=checkbox]) {
    outline: 1px solid #26b050;
}

.invalid {
    outline: 1px solid red;
}

.validation-message {
    color: red;
}

#blazor-error-ui {
    background: lightyellow;
    bottom: 0;
    box-shadow: 0 -1px 2px rgba(0, 0, 0, 0.2);
    display: none;
    left: 0;
    padding: 0.6rem 1.25rem 0.7rem 1.25rem;
    position: fixed;
    width: 100%;
    z-index: 1000;
}

    #blazor-error-ui .dismiss {
        cursor: pointer;
        position: absolute;
        right: 0.75rem;
        top: 0.5rem;
    }
```


在 `wwwroot/css` 文件夹内创建一个 `bootstrap` 文件夹。在 `bootstrap` 文件夹内，放置一份 `bootstrap.min.css` 的副本。你可以从 Bootstrap 官网获取最新版本的 `bootstrap.min.css`。

将以下 Counter 组件添加到项目根目录，这就是 Blazor 项目模板中默认的 Counter 组件。


```razor
<h1>Counter</h1>

<p>Current count: @currentCount</p>

<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

打开你想要托管 Blazor 组件的窗口的源文件，并添加以下代码：

```csharp
using WinFormedge;
using WinFormedge.Blazor;

namespace BlazorHybridExampleApp;

internal class MainWindow : Formedge
{
    public MainWindow()
    {
        Url = "https://blazorapp.local/";
        Load += MainWindow_Load;
    }

    private void MainWindow_Load(object? sender, EventArgs e)
    {
        this.SetVirtualHostNameToBlazorHybrid(new BlazorHybridOptions
        {
            Scheme = "https",
            HostName = "blazorapp.local",
            RootComponent = typeof(Counter),
            HostPath = "wwwroot/index.html"
        });
    }
}
```

以上代码设置了 Blazor Hybrid 环境，用 `SetVirtualHostNameToBlazorHybrid` 方法指定 Blazor 应用的 URL 和要渲染的根组件。`BlazorHybridOptions` 中的 `Scheme`、`HostName`、`RootComponent` 和 `HostPath` 属性分别定义了协议、主机名、根组件类型和主机路径。

## 🎶 运行

运行应用程序，你应该能看到 Blazor Counter 组件在 WinFormedge 窗口中渲染。你可以像在标准 Blazor 应用程序中一样与组件进行交互。

