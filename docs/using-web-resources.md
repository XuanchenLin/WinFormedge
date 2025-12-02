# Using Web Resources

`WinFormedge` is based on `WebView2`, so you can use all the web resource management features supported by `WebView2`, such as loading local files, remote URLs, and using custom protocols. Additionally, in `WinFormedge`, you can embed web resources (such as HTML, CSS, JavaScript files, etc.) into your application and access and manage them through virtual hostname mapping.

## Using Remote URLs

In `WinFormedge`, you can directly load remote URLs by setting the `Url` property. For example:

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

You can directly set any valid URL for the `Url` property, and `WinFormedge` will automatically handle loading and displaying the content of that URL.

## Using Local Files

You can also load local files as web resources by using the `SetVirtualHostNameToFolderMapping` method to map a local folder to a virtual hostname. For example:

```csharp
using WinFormedge;
namespace MyApp;
internal class MyWindow : Formedge
{
  public MyWindow()
  {
    // Set URL to the mapped virtual hostname
    Url = "https://appassets.local/index.html";

    Load += (s, e) =>
    {
      // Map local folder to virtual hostname
      SetVirtualHostNameToFolderMapping("https://appassets.local", "C:\\Path\\To\\Your\\WebAssets");
    };
  }
}
```

> [!NOTE]
> You need to call the `SetVirtualHostNameToFolderMapping` method in the `Load` event to ensure the mapping is completed after `WebView2` initialization.

In the example above, we map the local folder `C:\Path\To\Your\WebAssets` to the virtual hostname `https://appassets.local`, and then set the `Url` property to the `index.html` file under that virtual hostname.

### SetVirtualHostNameToFolderMapping Method

#### Method Signature

```csharp
public void SetVirtualHostNameToFolderMapping(string hostName, string folderPath, CoreWebView2HostResourceAccessKind accessKind = CoreWebView2HostResourceAccessKind.Allow);
```

#### Parameter Description

| Parameter Name | Type                               | Description                                                                                                                                                                           |
| -------------- | ---------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| hostName       | string                             | The virtual hostname to map, for example `https://appassets.local`.                                                                                                                   |
| folderPath     | string                             | The path to the local folder, for example `C:\Path\To\Your\WebAssets`.                                                                                                                |
| accessKind     | CoreWebView2HostResourceAccessKind | Specifies access permissions, defaults to `Allow`, which allows access to resources in the folder. For other values, refer to `WebView2`'s `CoreWebView2HostResourceAccessKind` enum. |

## Using Embedded Resources

`WinFormedge` also supports embedding web resources into any assembly of your application and accessing them through virtual hostname mapping. You can use the `SetVirtualHostNameToEmbeddedResourcesMapping` method to register embedded resources in `WebView2`. For example:

```csharp
using WinFormedge;
namespace MyApp;
internal class MyWindow : Formedge
{
  public MyWindow()
  {
    // Set URL to the mapped virtual hostname
    Url = "https://appresources.local/index.html";

    Load += (s, e) =>
    {
      // Map embedded resources to virtual hostname
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
> You need to call the `SetVirtualHostNameToEmbeddedResourcesMapping` method in the `Load` event to ensure the mapping is completed after `WebView2` initialization.

In the example above, we map embedded resources to the virtual hostname `https://appresources.local`, and then set the `Url` property to the `index.html` file under that virtual hostname. The embedded resources are located in the application's `wwwroot` folder.

> [!NOTE]
> You can omit `index.html` in the `Url` property; for embedded resources, `WinFormedge` will automatically look for the default homepage file.

Additionally, don't forget to set your web resource files as embedded resources. In Visual Studio, right-click on the resource file, select **Properties**, and then set **Build Action** to **Embedded Resource**.

If you're using an SDK-style `.csproj` file, you can double-click the project file and add the following content to set all files in a folder as embedded resources using wildcards:

```xml
<ItemGroup>
  <EmbeddedResource Include="wwwroot\**\*.*" />
</ItemGroup>
```

### SetVirtualHostNameToEmbeddedResourcesMapping Method

#### Method Signature

```csharp
public void SetVirtualHostNameToEmbeddedResourcesMapping(EmbeddedFileResourceOptions options);
```

#### Parameter Description

| Parameter Name | Type                        | Description                                                                                                                       |
| -------------- | --------------------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| options        | EmbeddedFileResourceOptions | An options object containing embedded resource mapping configuration. Refer to the table below for the properties of this object. |

#### EmbeddedFileResourceOptions Property Description

| Property Name     | Type                        | Description                                                                                                                                                                                                                                                                                                                                                                                                                       |
| ----------------- | --------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Scheme            | string                      | Specifies the protocol part of the virtual hostname, such as `https` or `http`.                                                                                                                                                                                                                                                                                                                                                   |
| HostName          | string                      | Specifies the host part of the virtual hostname, such as `appresources.local`.                                                                                                                                                                                                                                                                                                                                                    |
| ResourceAssembly  | Assembly                    | Specifies the assembly containing embedded resources, typically obtained using `typeof(YourClass).Assembly` to get the current assembly.                                                                                                                                                                                                                                                                                          |
| DefaultFolderName | string                      | Specifies the default folder name for embedded resources in the assembly, such as `wwwroot`.                                                                                                                                                                                                                                                                                                                                      |
| DefaultNamespace  | string                      | Specifies the default namespace prefix for embedded resources in the assembly. You can usually ignore this parameter, but if your assembly name is inconsistent with the default namespace, you need to manually set this property to the namespace of the assembly where the resource files are located.                                                                                                                         |
| OnFallback        | WebResourceFallbackDelegate | Specifies a callback delegate that is called when the requested resource is not found to handle fallback logic. This delegate accepts a `requestUrl` string parameter representing the URL of the current request. You can implement custom resource lookup or error handling logic in the callback. The delegate should return a `string` value representing the fallback resource content, which must be a valid resource path. |

## Related Content

- [Getting Started](./getting-started.md)
