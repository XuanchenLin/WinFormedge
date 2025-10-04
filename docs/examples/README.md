# 示例项目

本节提供了各种 WinFormedge 应用程序的完整示例，展示了不同的使用场景和最佳实践。

## 示例概览

| 示例 | 描述 | 复杂度 | 主要特性 |
|------|------|---------|----------|
| [最小示例](#最小示例) | 基础的 Hello World 应用 | ⭐ | 基本配置、嵌入式资源 |
| [现代化计算器](#现代化计算器) | 功能完整的计算器应用 | ⭐⭐ | 现代 UI、数据绑定 |
| [文件管理器](#文件管理器) | 简单的文件浏览器 | ⭐⭐⭐ | 文件操作、原生 API 集成 |
| [图片编辑器](#图片编辑器) | 基于 Web 的图片编辑器 | ⭐⭐⭐⭐ | Canvas 操作、文件处理 |
| [Blazor 混合应用](#blazor-混合应用) | 集成 Blazor 组件 | ⭐⭐⭐ | Blazor、组件化 |

---

## 最小示例

一个最简单的 WinFormedge 应用程序，展示基本的配置和使用方法。

### 项目结构

```
MinimalExample/
├── Program.cs
├── App.cs
├── MainWindow.cs
├── wwwroot/
│   ├── index.html
│   ├── style.css
│   └── app.js
└── MinimalExample.csproj
```

### Program.cs

```csharp
using WinFormedge;

namespace MinimalExample;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var app = WinFormedgeApp.CreateAppBuilder()
            .UseDevTools()
            .UseWinFormedgeApp<App>()
            .Build();

        app.Run();
    }
}
```

### App.cs

```csharp
using WinFormedge;

namespace MinimalExample;

internal class App : AppStartup
{
    protected override AppCreationAction? OnApplicationStartup(StartupSettings options)
    {
        return options.UseMainWindow(new MainWindow());
    }
}
```

### MainWindow.cs

```csharp
using WinFormedge;

namespace MinimalExample;

public class MainWindow : Formedge
{
    public MainWindow()
    {
        Url = "https://app.local/index.html";
        WindowTitle = "最小示例";
        Size = new Size(800, 600);
        StartPosition = FormStartPosition.CenterScreen;

        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "app.local",
            ResourceAssembly = Assembly.GetExecutingAssembly(),
            DefaultFolderName = "wwwroot"
        });
    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var settings = opts.UseDefaultWindow();
        settings.ExtendsContentIntoTitleBar = true;
        settings.SystemBackdropType = SystemBackdropType.Mica;
        return settings;
    }
}
```

### wwwroot/index.html

```html
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>最小示例</title>
    <link rel="stylesheet" href="style.css">
</head>
<body>
    <div class="container">
        <h1>欢迎使用 WinFormedge</h1>
        <p>这是一个最简单的示例应用程序。</p>
        <button onclick="showMessage()">点击我</button>
    </div>
    <script src="app.js"></script>
</body>
</html>
```

### wwwroot/style.css

```css
* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
}

body {
    font-family: 'Microsoft YaHei', sans-serif;
    background: rgba(255, 255, 255, 0.1);
    color: #333;
    height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
}

.container {
    text-align: center;
    padding: 40px;
    background: rgba(255, 255, 255, 0.8);
    border-radius: 12px;
    backdrop-filter: blur(20px);
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
}

h1 {
    margin-bottom: 20px;
    color: #2c3e50;
}

p {
    margin-bottom: 30px;
    font-size: 16px;
    color: #7f8c8d;
}

button {
    padding: 12px 24px;
    background: #3498db;
    color: white;
    border: none;
    border-radius: 6px;
    font-size: 16px;
    cursor: pointer;
    transition: background 0.3s;
}

button:hover {
    background: #2980b9;
}
```

### wwwroot/app.js

```javascript
function showMessage() {
    alert('Hello from WinFormedge!');
}

// 应用程序初始化
document.addEventListener('DOMContentLoaded', function() {
    console.log('最小示例应用已启动');
});
```

---

## 现代化计算器

一个功能完整的计算器应用，展示现代 UI 设计和数据绑定。

### 项目特点

- 现代化的毛玻璃效果界面
- 完整的计算功能
- 键盘快捷键支持
- 历史记录功能
- 主题切换

### 关键代码片段

#### CalculatorWindow.cs

```csharp
public class CalculatorWindow : Formedge
{
    private CalculatorEngine _engine = new();

    public CalculatorWindow()
    {
        Url = "https://calculator.local/index.html";
        WindowTitle = "现代计算器";
        Size = new Size(400, 600);
        StartPosition = FormStartPosition.CenterScreen;
        
        // 禁止调整大小以保持计算器的固定布局
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "calculator.local",
            ResourceAssembly = Assembly.GetExecutingAssembly(),
            DefaultFolderName = "wwwroot"
        });

        Load += OnLoad;
    }

    private async void OnLoad(object? sender, EventArgs e)
    {
        // 注册计算器对象到 JavaScript
        CoreWebView2?.AddHostObjectToScript("calculator", _engine);
        
        // 注册键盘事件处理
        await ExecuteScriptAsync("""
            document.addEventListener('keydown', function(e) {
                window.calculator.handleKeyPress(e.key);
            });
        """);
    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var settings = opts.UseDefaultWindow();
        settings.ExtendsContentIntoTitleBar = true;
        settings.SystemBackdropType = SystemBackdropType.Acrylic;
        settings.Resizable = false;
        return settings;
    }
}
```

#### CalculatorEngine.cs

```csharp
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class CalculatorEngine
{
    private string _display = "0";
    private double _memory = 0;
    private string _operation = "";
    private bool _shouldResetDisplay = false;

    public string Display => _display;

    public void InputNumber(string number)
    {
        if (_shouldResetDisplay || _display == "0")
        {
            _display = number;
            _shouldResetDisplay = false;
        }
        else
        {
            _display += number;
        }
        
        NotifyDisplayChanged();
    }

    public void InputOperation(string operation)
    {
        if (!string.IsNullOrEmpty(_operation) && !_shouldResetDisplay)
        {
            Calculate();
        }
        
        _memory = double.Parse(_display);
        _operation = operation;
        _shouldResetDisplay = true;
    }

    public void Calculate()
    {
        if (string.IsNullOrEmpty(_operation)) return;

        var current = double.Parse(_display);
        var result = _operation switch
        {
            "+" => _memory + current,
            "-" => _memory - current,
            "*" => _memory * current,
            "/" => current != 0 ? _memory / current : double.NaN,
            _ => current
        };

        _display = double.IsNaN(result) ? "错误" : result.ToString();
        _operation = "";
        _shouldResetDisplay = true;
        
        NotifyDisplayChanged();
    }

    public void Clear()
    {
        _display = "0";
        _memory = 0;
        _operation = "";
        _shouldResetDisplay = false;
        
        NotifyDisplayChanged();
    }

    public void HandleKeyPress(string key)
    {
        switch (key)
        {
            case >= "0" and <= "9":
                InputNumber(key);
                break;
            case "+":
            case "-":
            case "*":
            case "/":
                InputOperation(key);
                break;
            case "Enter":
            case "=":
                Calculate();
                break;
            case "Escape":
                Clear();
                break;
        }
    }

    private void NotifyDisplayChanged()
    {
        // 通知前端更新显示
        DisplayChanged?.Invoke(_display);
    }

    public event Action<string>? DisplayChanged;
}
```

---

## 文件管理器

一个简单的文件浏览器，展示文件操作和原生 API 集成。

### 项目特点

- 文件和文件夹浏览
- 文件操作（复制、移动、删除）
- 文件预览功能
- 上下文菜单
- 拖拽支持

### 关键代码片段

#### FileManagerWindow.cs

```csharp
public class FileManagerWindow : Formedge
{
    private FileSystemService _fileService = new();

    public FileManagerWindow()
    {
        Url = "https://filemanager.local/index.html";
        WindowTitle = "文件管理器";
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;

        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "filemanager.local",
            ResourceAssembly = Assembly.GetExecutingAssembly(),
            DefaultFolderName = "wwwroot"
        });

        Load += OnLoad;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        CoreWebView2?.AddHostObjectToScript("fileService", _fileService);
    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var settings = opts.UseDefaultWindow();
        settings.ExtendsContentIntoTitleBar = true;
        settings.SystemBackdropType = SystemBackdropType.Mica;
        return settings;
    }
}
```

#### FileSystemService.cs

```csharp
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class FileSystemService
{
    public string GetDirectoryListing(string path)
    {
        try
        {
            var directory = new DirectoryInfo(path);
            var items = new List<object>();

            // 添加文件夹
            foreach (var dir in directory.GetDirectories())
            {
                items.Add(new
                {
                    Name = dir.Name,
                    Type = "folder",
                    Size = 0,
                    Modified = dir.LastWriteTime,
                    FullPath = dir.FullName
                });
            }

            // 添加文件
            foreach (var file in directory.GetFiles())
            {
                items.Add(new
                {
                    Name = file.Name,
                    Type = "file",
                    Size = file.Length,
                    Modified = file.LastWriteTime,
                    FullPath = file.FullName
                });
            }

            return JsonSerializer.Serialize(items);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message });
        }
    }

    public bool CreateFolder(string path, string name)
    {
        try
        {
            var fullPath = Path.Combine(path, name);
            Directory.CreateDirectory(fullPath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteItem(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetFileContent(string path)
    {
        try
        {
            if (IsTextFile(path))
            {
                return File.ReadAllText(path);
            }
            return "二进制文件";
        }
        catch (Exception ex)
        {
            return $"错误: {ex.Message}";
        }
    }

    private bool IsTextFile(string path)
    {
        var textExtensions = new[] { ".txt", ".cs", ".js", ".html", ".css", ".json", ".xml" };
        return textExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
    }
}
```

---

## 图片编辑器

基于 Web Canvas 的图片编辑器，展示复杂的前端交互。

### 项目特点

- 图片加载和保存
- 基本编辑功能（裁剪、旋转、滤镜）
- 画笔工具
- 图层支持
- 撤销/重做功能

### 关键代码片段

#### ImageEditorWindow.cs

```csharp
public class ImageEditorWindow : Formedge
{
    private ImageEditingService _imageService = new();

    public ImageEditorWindow()
    {
        Url = "https://imageeditor.local/index.html";
        WindowTitle = "图片编辑器";
        Size = new Size(1400, 900);
        StartPosition = FormStartPosition.CenterScreen;

        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "imageeditor.local",
            ResourceAssembly = Assembly.GetExecutingAssembly(),
            DefaultFolderName = "wwwroot"
        });

        Load += OnLoad;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        CoreWebView2?.AddHostObjectToScript("imageService", _imageService);
    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var settings = opts.UseDefaultWindow();
        settings.ExtendsContentIntoTitleBar = true;
        settings.SystemBackdropType = SystemBackdropType.Mica;
        return settings;
    }
}
```

---

## Blazor 混合应用

展示如何在 WinFormedge 中集成 Blazor 组件。

### 项目特点

- Blazor Server 集成
- 组件化 UI
- 数据绑定
- 服务注入
- 实时更新

### 关键代码片段

#### BlazorHybridWindow.cs

```csharp
public class BlazorHybridWindow : Formedge
{
    public BlazorHybridWindow()
    {
        Url = "https://blazor.local/";
        WindowTitle = "Blazor 混合应用";
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;

        // 配置 Blazor 资源映射
        SetupBlazorResourceMapping();
    }

    private void SetupBlazorResourceMapping()
    {
        // 设置 Blazor 应用的资源映射
        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "blazor.local",
            ResourceAssembly = Assembly.GetExecutingAssembly(),
            DefaultFolderName = "wwwroot"
        });
    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var settings = opts.UseDefaultWindow();
        settings.ExtendsContentIntoTitleBar = true;
        settings.SystemBackdropType = SystemBackdropType.Mica;
        return settings;
    }
}
```

---

## 获取示例代码

所有示例的完整源代码都可以在 GitHub 仓库中找到：

- **GitHub**: [https://github.com/XuanchenLin/WinFormedge/tree/main/src](https://github.com/XuanchenLin/WinFormedge/tree/main/src)

### 运行示例

1. 克隆仓库：
   ```bash
   git clone https://github.com/XuanchenLin/WinFormedge.git
   ```

2. 打开 Visual Studio 2022

3. 打开解决方案文件 `WinFormedge.sln`

4. 选择一个示例项目作为启动项目

5. 按 F5 运行

### 创建新项目

可以使用示例项目作为模板创建新的 WinFormedge 应用程序：

1. 复制最小示例项目文件夹
2. 重命名项目和命名空间
3. 修改 `wwwroot` 文件夹中的 Web 内容
4. 根据需要添加新的功能和服务

---

## 学习路径

推荐的学习顺序：

1. **最小示例** - 了解基本概念和项目结构
2. **现代化计算器** - 学习 UI 设计和数据绑定
3. **文件管理器** - 掌握原生 API 集成
4. **图片编辑器** - 理解复杂的前后端交互
5. **Blazor 混合应用** - 探索高级集成方案

每个示例都包含详细的注释和说明，帮助您理解 WinFormedge 的各种特性和最佳实践。