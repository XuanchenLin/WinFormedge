# API 参考

WinFormedge 提供了丰富的 API 来创建现代化的桌面应用程序。本节详细介绍了所有可用的类、方法和属性。

## 核心组件

### 应用程序管理
- [WinFormedgeApp](core-classes.md#winformedgeapp) - 应用程序主类
- [AppBuilder](core-classes.md#appbuilder) - 应用程序构建器
- [AppStartup](core-classes.md#appstartup) - 应用程序启动类

### 窗口管理
- [Formedge](core-classes.md#formedge) - 主窗口基类
- [WindowSettings](window-management.md#windowsettings) - 窗口配置
- [HostWindowBuilder](window-management.md#hostwindowbuilder) - 窗口构建器

### 配置和设置
- [StartupSettings](configuration.md#startupsettings) - 启动配置
- [SystemBackdropType](configuration.md#systembackdroptype) - 系统背景类型
- [SystemColorMode](configuration.md#systemcolormode) - 系统颜色模式

### Web 资源管理
- [WebResourceManager](web-resources.md#webresourcemanager) - Web 资源管理器
- [EmbeddedFileResourceOptions](web-resources.md#embeddedfileresourceoptions) - 嵌入式文件资源选项
- [WebResourceHandler](web-resources.md#webresourcehandler) - Web 资源处理器

## 快速导航

| 类别 | 页面 | 描述 |
|------|------|------|
| 核心类 | [core-classes.md](core-classes.md) | 主要的核心类和接口 |
| 配置选项 | [configuration.md](configuration.md) | 应用程序和窗口配置选项 |
| 窗口管理 | [window-management.md](window-management.md) | 窗口创建、样式和行为管理 |
| Web资源 | [web-resources.md](web-resources.md) | 嵌入式资源和自定义协议处理 |

## 常用代码示例

### 基本应用程序设置

```csharp
var app = WinFormedgeApp.CreateAppBuilder()
    .UseDevTools()
    .SetColorMode(SystemColorMode.Dark)
    .UseModernStyleScrollbar()
    .UseWinFormedgeApp<MyApp>()
    .Build();
```

### 窗口配置

```csharp
protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
{
    var settings = opts.UseDefaultWindow();
    settings.ExtendsContentIntoTitleBar = true;
    settings.SystemBackdropType = SystemBackdropType.Mica;
    settings.Resizable = true;
    return settings;
}
```

### 嵌入式资源设置

```csharp
SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
{
    Scheme = "https",
    HostName = "app.local",
    ResourceAssembly = Assembly.GetExecutingAssembly(),
    DefaultFolderName = "wwwroot"
});
```

## 版本兼容性

| 功能 | 最低 Windows 版本 | 备注 |
|------|------------------|------|
| 基本功能 | Windows 10 1903 | 支持所有基本特性 |
| 系统背景效果 | Windows 10 2004 | BlurBehind, Acrylic |
| Mica 效果 | Windows 11 | Mica, MicaAlt, Transient |
| 现代滚动条 | Windows 11 | FluentOverlay 样式 |