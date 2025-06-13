# Blazor Hybrid Plugin for WinFormedge

## ✨ About

The Blazor Hybrid Plugin for WinFormedge enables seamless integration of Blazor components into WinFormedge-based Windows Forms applications. With this plugin, developers can leverage the power of Blazor's modern web UI framework alongside traditional WinForms, allowing for interactive, component-based user interfaces using C#, HTML, and CSS. This hybrid approach combines the flexibility of web technologies with the performance and capabilities of native Windows applications, making it easy to build rich, cross-technology desktop solutions.

## 📖 Usage

To use the Blazor Hybrid Plugin, you need to follow these steps:

**Install the NuGet Package**: Add the `WinFormedge.Blazor` package to your WinFormedge project.

In **Solution Explorer**, right-click the project's name, and select Edit Project File to open the project file.

At the top of the project file, change the SDK to `Microsoft.NET.Sdk.Razor`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
```

Add an `_Imports.razor` file to the root of the project with an @using directive for Microsoft.AspNetCore.Components.Web.

```razor
@using Microsoft.AspNetCore.Components.Web
```

Add a `wwwroot` folder to the project.

Add an `index.html` file to the wwwroot folder with the following markup.

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

Inside the `wwwroot` folder, create a `css` folder to hold stylesheets.

Add an `app.css` stylesheet to the `wwwroot/css` folder with the following content.

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

Inside the `wwwroot/css` folder, create a `bootstrap` folder. Inside the `bootstrap` folder, place a copy of `bootstrap.min.css`. You can obtain the latest version of `bootstrap.min.css` from the Bootstrap website. Because all of the content at the site is versioned in the URL, a direct link can't be provided here. Therefore, follow navigation bar links to Docs > Download to obtain bootstrap.min.css.

Add the following Counter component to the root of the project, which is the default Counter component found in Blazor project templates.

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

Open the source file for the window that you want to host the Blazor component in, and add the following code:
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

This code sets up the Blazor Hybrid environment for the `MainWindow` class, using the `SetVirtualHostNameToBlazorHybrid` method to specify the Blazor app's URL and the root component to be rendered.

## 🎶 Run the app

Run the application, and you should see the Blazor Counter component rendered within the WinFormedge window. You can interact with the component as you would in a standard Blazor application.
