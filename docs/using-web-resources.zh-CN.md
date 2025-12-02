# 使用 Web 资源

`WinFormedge` 基于 `WebView2`，因此您可以使用 `WebView2` 支持的所有 Web 资源管理功能，例如加载本地文件、远程 URL 以及使用自定义协议等。除此之外在 `WinFormedge` 中，您可以将 Web 资源（如 HTML、CSS、JavaScript 文件等）嵌入到应用程序中，并通过虚拟主机名映射的方式进行访问和管理。

## 使用远程 URL

在 `WinFormedge` 中，您可以直接通过设置 `Url` 属性来加载远程 URL。例如：

```csharp
using WinFormedge;
namespace MyApp;
internal class MyWindow : Formedge
{
  public MyWindow()
  {
    Url = "https://www.bing.com";
  }
}
```

您可以直接为 `Url` 属性设置任何有效的 URL，`WinFormedge` 会自动处理加载和显示该 URL 的内容。

## 使用本地文件

您也可以加载本地文件作为 Web 资源，使用 `SetVirtualHostNameToFolderMapping` 方法将本地文件夹映射到虚拟主机名。例如：

```csharp
using WinFormedge;
namespace MyApp;
internal class MyWindow : Formedge
{
  public MyWindow()
  {
    // 设置 URL 为映射的虚拟主机名
    Url = "https://appassets.local/index.html";

    Load += (s, e) =>
    {
      // 将本地文件夹映射到虚拟主机名
      SetVirtualHostNameToFolderMapping("https://appassets.local", "C:\\Path\\To\\Your\\WebAssets");
    };
  }
}
```

> [!NOTE]
> 您需要在 `Load` 事件中调用 `SetVirtualHostNameToFolderMapping` 方法，以确保映射在 `WebView2` 初始化之后完成。

在上述示例中，我们将本地文件夹 `C:\Path\To\Your\WebAssets` 映射到虚拟主机名 `https://appassets.local`，然后将 `Url` 属性设置为该虚拟主机名下的 `index.html` 文件。

### SetVirtualHostNameToFolderMapping 方法

#### 方法签名

```csharp
public void SetVirtualHostNameToFolderMapping(string hostName, string folderPath, CoreWebView2HostResourceAccessKind accessKind = CoreWebView2HostResourceAccessKind.Allow);
```

#### 参数说明

| 参数名称   | 类型                               | 描述                                                                                                                                 |
| ---------- | ---------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| hostName   | string                             | 要映射的虚拟主机名，例如 `https://appassets.local`。                                                                                 |
| folderPath | string                             | 本地文件夹的路径，例如 `C:\Path\To\Your\WebAssets`。                                                                                 |
| accessKind | CoreWebView2HostResourceAccessKind | 指定访问权限，默认为 `Allow`，表示允许访问该文件夹中的资源。其他取值请参考 `WebView2` 的 `CoreWebView2HostResourceAccessKind` 枚举。 |

## 使用嵌入资源

`WinFormedge` 还支持将 Web 资源嵌入到应用程序的任意程序集中，并通过虚拟主机名映射的方式进行访问。您可以使用 `SetVirtualHostNameToEmbeddedResourcesMapping` 方法将嵌入资源注册到 `WebView2` 中。例如：

```csharp
using WinFormedge;
namespace MyApp;
internal class MyWindow : Formedge
{
  public MyWindow()
  {
    // 设置 URL 为映射的虚拟主机名
    Url = "https://appresources.local/index.html";

    Load += (s, e) =>
    {
      // 将嵌入资源映射到虚拟主机名
      SetVirtualHostNameToEmbeddedResourcesMapping(new WinFormedge.WebResource.EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "appresources.local",
            ResourceAssembly = typeof(MyWindow).Assembly,
            DefaultFolderName = "wwwroot"
        });
    };
  }
}
```

> [!NOTE]
> 您需要在 `Load` 事件中调用 `SetVirtualHostNameToEmbeddedResourcesMapping` 方法，以确保映射在 `WebView2` 初始化之后完成。

在上述示例中，我们将嵌入资源映射到虚拟主机名 `https://appresources.local`，然后将 `Url` 属性设置为该虚拟主机名下的 `index.html` 文件。嵌入资源位于应用程序的 `wwwroot` 文件夹中。

> [!NOTE]
> 您可以忽略 `Url` 属性中的 `index.html`，对于嵌入资源 `WinFormedge` 会自动查找默认的首页文件。

另外，别忘了将您的 Web 资源文件设置为嵌入资源。在 Visual Studio 中，右键点击资源文件，选择**属性**，然后将**生成操作**设置为**嵌入的资源**。

如果您使用的是 SDK 样式的 `.csproj` 文件，可以双击项目文件并添加如下内容，以通配符的方式将整个文件夹内的文件设置为嵌入资源：

```xml
<ItemGroup>
  <EmbeddedResource Include="wwwroot\**\*.*" />
</ItemGroup>
```

### SetVirtualHostNameToEmbeddedResourcesMapping 方法

#### 方法签名

```csharp
public void SetVirtualHostNameToEmbeddedResourcesMapping(EmbeddedFileResourceOptions options);
```

#### 参数说明

| 参数名称 | 类型                        | 描述                                                             |
| -------- | --------------------------- | ---------------------------------------------------------------- |
| options  | EmbeddedFileResourceOptions | 包含嵌入资源映射配置的选项对象，您可以参考下表了解该对象的属性。 |

#### EmbeddedFileResourceOptions 属性说明

| 属性名称          | 类型                        | 描述                                                                                                                                                                                                                                                                                   |
| ----------------- | --------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Scheme            | string                      | 指定虚拟主机名的协议部分，例如 `https` 或 `http`。                                                                                                                                                                                                                                     |
| HostName          | string                      | 指定虚拟主机名的主机部分，例如 `appresources.local`。                                                                                                                                                                                                                                  |
| ResourceAssembly  | Assembly                    | 指定包含嵌入资源的程序集，通常使用 `typeof(YourClass).Assembly` 来获取当前程序集。                                                                                                                                                                                                     |
| DefaultFolderName | string                      | 指定嵌入资源在程序集中的默认文件夹名称，例如 `wwwroot`。                                                                                                                                                                                                                               |
| DefaultNamespace  | string                      | 指定嵌入资源在程序集中的默认命名空间前缀，通常您可以忽略这个参数，但如果您的程序集名称与默认命名空间不一致时，您需要手动将此属性指定为资源文件所在的程序集的命名空间。                                                                                                                 |
| OnFallback        | WebResourceFallbackDelegate | 指定一个回调委托，当请求的资源未找到时调用该委托以处理回退逻辑。该委托接受一个 `requestUrl` 字符串参数，该参数表示当前请求的的 URL，您可以在回调中实现自定义的资源查找或错误处理逻辑，该委托需要返回一个 `string` 类型的返回值，该返回值表示回退的资源内容，需要是一个有效的资源路径。 |

## 相关内容

- [快速入门](./getting-started.zh-CN.md)
