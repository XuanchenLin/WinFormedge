// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using WinFormedge;
using WinFormedge.Blazor;

namespace BlazorHybridExampleApp;
internal class MainWindow : Formedge
{
    public MainWindow()
    {
        Icon = new Icon(new MemoryStream(Properties.Resources.WinFormiumBlazor));

        Url = "https://blazorapp.local/";

        Load += MainWindow_Load;
    }

    private void MainWindow_Load(object? sender, EventArgs e)
    {
        var opts = new BlazorHybridOptions
        {
            Scheme = "https",
            HostName = "blazorapp.local",
            HostPage = "wwwroot/index.html",
        };
        
        opts.RootComponents.Add<Counter>("#app");

        opts.StaticResources.Add(new BlazorHybridAssemblyResources(typeof(MainWindow).Assembly));

        this.SetVirtualHostNameToBlazorHybrid(opts);
    }
}
