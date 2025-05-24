# The WinFormedge Project

## ✨ About

**WinFormedge** is an open-source .NET library built on **Microsoft WebView2**, enabling developers to create modern, visually appealing WinForms applications using **HTML, CSS, and JavaScript**. With WinFormedge, you can seamlessly integrate web technologies into your WinForm projects, allowing for rich and interactive user interfaces.  


## 🖥️ Requirements

**Development Environment:**
- Visual Studio 2022
- .NET 8.0 or higher

**Deployment Environment:**
- Windows 10 1903 or higher

This is a **Windows ONLY** library. It is not compatible with other operating systems.

The minimum supported operating system is Windows 10 version 1903 (May 2019 Update). And some fetures such as SystemBackdrop and SystemColorMode are only available on Windows 11.

## 🪄 Getting Started

### Create a WinForm Application by using default project template

**1. Replace initialization code by using WinFormedge application initialization procedure.**

You should use `FormedgeApp` instead of `Application` class to initialize your WinForm application in the `program.cs` file. The `FormedgeApp` class is a builder for creating a WinFormedge application. It provides methods for configuring the application and running it.

```csharp
using WinFormedge;

namespace MinimalExampleApp;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var app = FormedgeApp.CreateBuilder()
            .UseDevTools()
            .UseWinFormedgeApp<MyFormedgeApp>()
            .Build();

        app.Run();
    }
}
```

When the `FormedgeApp` class is created, it will automatically initialize the WebView2 environment and run the message loop.


**2. Create a AppStartup class.**

The `AppStartup` class is the entry point of your WinFormedge application. It provides methods for configuring the application. You can override the `OnApplicationLaunched` method to perform any initialization tasks before the application starts.

And you must override the `OnApplicationStartup` method to create the main window of your application. If the `OnApplicationStartup` method returns values created by `StartupSettings` class, the `FormedgeApp` class will create the main window of your application. Otherwise if the `OnApplicationStartup` method returns `null` the application will be closed.

```csharp
using WinFormedge;

namespace MinimalExampleApp;

internal class MyFormedgeApp : AppStartup
{
    protected override AppCreationAction? OnApplicationStartup(StartupSettings options)
    {
        return options.UseMainWindow(new MyWindow());
    }
}
```

You can do some staffs like User Login, User Settings, etc. in the `OnApplicationStartup` method to determine using which window to start the application. Andalso you can end the application by returning `null` if conditions are not met.

**3. Create a MainWindow class.**

The `MainWindow` class is the main window of your application. It inherits from the `Formedge` class. You can use the `Formedge` class to create a window with a WebView2 control.
```csharp
using WinFormedge;
using Microsoft.Web.WebView2.Core;

namespace MinimalExampleApp;
internal class MyWindow : Formedge
{
    public MyWindow()
    {
        Url = "https://embedded.appresource.local/";
        Size = new Size(1440, 900);
        AllowFullscreen = true;
        BackColor = Color.Transparent;

        // If you have embedded resources, you can set the virtual host name to access them.
        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions { 
            Scheme="https", 
            HostName="embedded.appresource.local", 
            ResourceAssembly=Assembly.GetExecutingAssembly(),
            DefaultFolderName="Resources\\wwwroot"
        });

        Load += MyWindow_Load;
        DOMContentLoaded += MyWindow_DOMContentLoaded;
    }

    // Configure the window settings for the Formedge window.
    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var win = opts.UseDefaultWindow();

        // Extends the content of the WebView into the non-client area of the window.
        win.ExtendsContentIntoTitleBar = true;
        // Sets the backdrop type of the window to blur behind or something else.
        win.SystemBackdropType = SystemBackdropType.BlurBehind;

        return win;
    }

    private void MyWindow_Load(object? sender, EventArgs e)
    {
        // Window and WebView2 are ready to use here.
    }

    private void MyWindow_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
    {
        // The DOM content is loaded and ready to use here.
        ExecuteScriptAsync(""""
        (()=>{
            alert("Hello from WinFormedge!");
        })();
        """");
    }
}
```

Codes above creates a `Formedge` window. By using `Url` property, you can set the initial URL of the window. Sets the `ExtendsContentIntoTitleBar` property to `true` to extend the WebView into the non-client area of the window to create a borderless window. You can also set the `Size` property to set the initial size of the window.

The `Load` event is raised when the window and WebView2 are ready to use. You can use this event to perform any initialization tasks that require the WebView2 control to be ready.

The `DOMContentLoaded` event is raised when the DOM content is loaded and ready to use. You can use this event to perform any tasks that require the DOM content to be loaded.

**4. Run the application.**

You can run the application by pressing `F5` in Visual Studio. The application will create a window with a WebView2 control that displays the Bing homepage. 

You can drag the window by clicking and dragging the header element of the page and resize the window by dragging the edges of the window. 
