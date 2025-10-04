# Web 资源管理

本页面详细介绍 WinFormedge 中 Web 资源管理的相关类和方法，包括嵌入式资源处理、自定义协议和资源映射。

## WebResourceManager

`WebResourceManager` 负责管理应用程序中的 Web 资源，包括嵌入式文件、自定义协议处理等。

### 主要功能

- 嵌入式资源文件服务
- 自定义协议注册和处理
- 资源请求拦截和重定向
- MIME 类型自动识别
- 缓存控制

---

## EmbeddedFileResourceOptions

`EmbeddedFileResourceOptions` 类用于配置嵌入式文件资源的映射选项。

### 属性

#### Scheme
```csharp
public string Scheme { get; set; }
```
获取或设置协议方案（如 "https", "http"）。

**默认值**: `"https"`

#### HostName
```csharp
public string HostName { get; set; }
```
获取或设置虚拟主机名。

**示例**: `"app.local"`, `"embedded.resources"`

#### ResourceAssembly
```csharp
public Assembly? ResourceAssembly { get; set; }
```
获取或设置包含嵌入式资源的程序集。

#### DefaultFolderName
```csharp
public string? DefaultFolderName { get; set; }
```
获取或设置默认的资源文件夹名称。

**示例**: `"wwwroot"`, `"Resources/Web"`

### 使用示例

```csharp
public class MyWindow : Formedge
{
    public MyWindow()
    {
        Url = "https://myapp.local/index.html";
        
        // 配置嵌入式资源映射
        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "myapp.local",
            ResourceAssembly = Assembly.GetExecutingAssembly(),
            DefaultFolderName = "wwwroot"
        });
    }
}
```

### 项目配置

在 `.csproj` 文件中配置嵌入式资源：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <!-- 其他配置 -->
  
  <ItemGroup>
    <!-- 将 wwwroot 文件夹下的所有文件设为嵌入式资源 -->
    <EmbeddedResource Include="wwwroot\**\*" />
  </ItemGroup>
  
  <!-- 或者更精确的配置 -->
  <ItemGroup>
    <EmbeddedResource Include="wwwroot\*.html" />
    <EmbeddedResource Include="wwwroot\css\*.css" />
    <EmbeddedResource Include="wwwroot\js\*.js" />
    <EmbeddedResource Include="wwwroot\images\*.*" />
  </ItemGroup>
</Project>
```

---

## WebResourceHandler

`WebResourceHandler` 是处理 Web 资源请求的基类，可以通过继承来实现自定义资源处理逻辑。

### 虚拟方法

#### ProcessRequestAsync
```csharp
protected virtual Task<WebResourceResponse?> ProcessRequestAsync(WebResourceRequest request)
```
处理资源请求的核心方法。

**参数:**
- `request`: Web 资源请求对象

**返回值:**
- `WebResourceResponse?`: 资源响应，返回 `null` 表示未处理

### 自定义资源处理器示例

```csharp
public class CustomResourceHandler : WebResourceHandler
{
    protected override async Task<WebResourceResponse?> ProcessRequestAsync(WebResourceRequest request)
    {
        var uri = new Uri(request.Uri);
        
        // 处理 API 请求
        if (uri.AbsolutePath.StartsWith("/api/"))
        {
            return await HandleApiRequest(uri.AbsolutePath, request);
        }
        
        // 处理静态文件
        if (uri.AbsolutePath.StartsWith("/assets/"))
        {
            return await HandleStaticFile(uri.AbsolutePath);
        }
        
        // 返回 null 让其他处理器处理
        return null;
    }
    
    private async Task<WebResourceResponse> HandleApiRequest(string path, WebResourceRequest request)
    {
        var response = new WebResourceResponse();
        
        try
        {
            string jsonContent;
            
            switch (path)
            {
                case "/api/user":
                    jsonContent = JsonSerializer.Serialize(new { Name = "用户", Id = 1 });
                    break;
                    
                case "/api/settings":
                    jsonContent = JsonSerializer.Serialize(GetAppSettings());
                    break;
                    
                default:
                    response.StatusCode = 404;
                    response.ReasonPhrase = "Not Found";
                    return response;
            }
            
            response.Content = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.StatusCode = 200;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.ReasonPhrase = "Internal Server Error";
            response.Content = new MemoryStream(Encoding.UTF8.GetBytes(ex.Message));
        }
        
        return response;
    }
    
    private async Task<WebResourceResponse?> HandleStaticFile(string path)
    {
        // 从文件系统或其他位置加载静态文件
        var filePath = Path.Combine(Application.StartupPath, "assets", path.TrimStart('/'));
        
        if (!File.Exists(filePath))
        {
            return null;
        }
        
        var response = new WebResourceResponse
        {
            Content = new FileStream(filePath, FileMode.Open, FileAccess.Read),
            StatusCode = 200
        };
        
        // 设置正确的 MIME 类型
        var extension = Path.GetExtension(filePath);
        response.Headers.Add("Content-Type", GetMimeType(extension));
        
        return response;
    }
    
    private object GetAppSettings()
    {
        return new
        {
            Theme = "dark",
            Language = "zh-CN",
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString()
        };
    }
    
    private string GetMimeType(string extension)
    {
        return extension.ToLower() switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".ico" => "image/x-icon",
            _ => "application/octet-stream"
        };
    }
}
```

---

## 高级资源管理

### 多域名映射

支持多个虚拟主机名映射到不同的资源集：

```csharp
public class MultiDomainWindow : Formedge
{
    public MultiDomainWindow()
    {
        // 主应用程序资源
        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "app.local",
            ResourceAssembly = Assembly.GetExecutingAssembly(),
            DefaultFolderName = "wwwroot"
        });
        
        // 静态资源 CDN
        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "cdn.app.local",
            ResourceAssembly = Assembly.GetExecutingAssembly(),
            DefaultFolderName = "assets"
        });
        
        // API 服务
        RegisterCustomResourceHandler("https", "api.app.local", new ApiResourceHandler());
    }
    
    private void RegisterCustomResourceHandler(string scheme, string hostname, WebResourceHandler handler)
    {
        // 这里需要在实际实现中注册自定义处理器
        // WinFormedge 框架会在未来版本中提供此功能
    }
}
```

### 动态资源生成

运行时动态生成 Web 资源：

```csharp
public class DynamicResourceHandler : WebResourceHandler
{
    protected override async Task<WebResourceResponse?> ProcessRequestAsync(WebResourceRequest request)
    {
        var uri = new Uri(request.Uri);
        
        if (uri.AbsolutePath == "/dynamic/theme.css")
        {
            return await GenerateThemeCSS();
        }
        
        if (uri.AbsolutePath == "/dynamic/config.js")
        {
            return await GenerateConfigJS();
        }
        
        return null;
    }
    
    private async Task<WebResourceResponse> GenerateThemeCSS()
    {
        var isDarkMode = WinFormedgeApp.Current.IsDarkMode;
        
        var css = isDarkMode ? """
            :root {
                --bg-color: #2d2d2d;
                --text-color: #ffffff;
                --accent-color: #0078d4;
            }
            """ : """
            :root {
                --bg-color: #ffffff;
                --text-color: #000000;
                --accent-color: #106ebe;
            }
            """;
        
        var response = new WebResourceResponse
        {
            Content = new MemoryStream(Encoding.UTF8.GetBytes(css)),
            StatusCode = 200
        };
        
        response.Headers.Add("Content-Type", "text/css; charset=utf-8");
        response.Headers.Add("Cache-Control", "no-cache");
        
        return response;
    }
    
    private async Task<WebResourceResponse> GenerateConfigJS()
    {
        var config = new
        {
            AppVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
            Platform = Environment.OSVersion.Platform.ToString(),
            Culture = CultureInfo.CurrentCulture.Name,
            IsDarkMode = WinFormedgeApp.Current.IsDarkMode,
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };
        
        var js = $"window.APP_CONFIG = {JsonSerializer.Serialize(config, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })};";
        
        var response = new WebResourceResponse
        {
            Content = new MemoryStream(Encoding.UTF8.GetBytes(js)),
            StatusCode = 200
        };
        
        response.Headers.Add("Content-Type", "application/javascript; charset=utf-8");
        response.Headers.Add("Cache-Control", "no-cache");
        
        return response;
    }
}
```

### 资源缓存策略

实现智能的资源缓存策略：

```csharp
public class CachingResourceHandler : WebResourceHandler
{
    private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions
    {
        SizeLimit = 100 // 最多缓存 100 个资源
    });
    
    protected override async Task<WebResourceResponse?> ProcessRequestAsync(WebResourceRequest request)
    {
        var cacheKey = request.Uri;
        
        // 尝试从缓存获取
        if (_cache.TryGetValue(cacheKey, out byte[] cachedContent))
        {
            return CreateResponse(cachedContent, GetMimeType(request.Uri));
        }
        
        // 生成或加载资源
        var content = await LoadResourceContent(request.Uri);
        if (content != null)
        {
            // 缓存资源（根据资源类型设置不同的过期时间）
            var cacheOptions = new MemoryCacheEntryOptions
            {
                Size = 1,
                SlidingExpiration = GetCacheExpiration(request.Uri)
            };
            
            _cache.Set(cacheKey, content, cacheOptions);
            
            return CreateResponse(content, GetMimeType(request.Uri));
        }
        
        return null;
    }
    
    private TimeSpan GetCacheExpiration(string uri)
    {
        var extension = Path.GetExtension(new Uri(uri).AbsolutePath);
        
        return extension.ToLower() switch
        {
            ".html" => TimeSpan.FromMinutes(5),     // HTML 文件短期缓存
            ".css" or ".js" => TimeSpan.FromHours(1), // 样式和脚本中期缓存
            ".png" or ".jpg" or ".gif" => TimeSpan.FromDays(1), // 图片长期缓存
            _ => TimeSpan.FromMinutes(10)
        };
    }
    
    private async Task<byte[]?> LoadResourceContent(string uri)
    {
        // 实现资源加载逻辑
        // 这里可以从文件系统、数据库或网络加载资源
        return null;
    }
    
    private WebResourceResponse CreateResponse(byte[] content, string mimeType)
    {
        return new WebResourceResponse
        {
            Content = new MemoryStream(content),
            StatusCode = 200,
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = mimeType,
                ["Cache-Control"] = "max-age=3600"
            }
        };
    }
    
    private string GetMimeType(string uri)
    {
        var extension = Path.GetExtension(new Uri(uri).AbsolutePath);
        // 实现 MIME 类型映射
        return "application/octet-stream";
    }
}
```

---

## 最佳实践

### 1. 资源组织结构

推荐的项目资源组织结构：

```
MyApp/
├── wwwroot/                 # Web 资源根目录
│   ├── index.html          # 主页面
│   ├── css/                # 样式文件
│   │   ├── app.css
│   │   └── themes/
│   │       ├── light.css
│   │       └── dark.css
│   ├── js/                 # JavaScript 文件
│   │   ├── app.js
│   │   └── components/
│   ├── images/             # 图片资源
│   └── fonts/              # 字体文件
├── Resources/              # 其他嵌入式资源
└── Handlers/               # 自定义资源处理器
```

### 2. 性能优化

- 使用适当的缓存策略
- 压缩静态资源
- 实现资源懒加载
- 合理设置 MIME 类型

### 3. 安全考虑

- 验证资源请求路径，防止路径遍历攻击
- 实施适当的访问控制
- 避免暴露敏感文件

```csharp
private bool IsValidResourcePath(string path)
{
    // 防止路径遍历攻击
    if (path.Contains("..") || path.Contains("//"))
    {
        return false;
    }
    
    // 检查文件扩展名白名单
    var allowedExtensions = new[] { ".html", ".css", ".js", ".png", ".jpg", ".gif", ".svg" };
    var extension = Path.GetExtension(path);
    
    return allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
}
```