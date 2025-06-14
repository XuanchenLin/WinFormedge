// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.FileProviders;

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
        this.SetVirtualHostNameToBlazorHybrid(new BlazorHybridOptions
        {
            Scheme = "https",
            HostName = "blazorapp.local",
            RootComponent = typeof(Counter),
            HostPage = "wwwroot/index.html",
            StaticResources = typeof(MainWindow).Assembly,
        });
    }
}
