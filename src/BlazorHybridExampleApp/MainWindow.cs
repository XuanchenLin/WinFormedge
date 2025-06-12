// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormedge;
using WinFormedge.Blazor;

namespace BlazorHybridExampleApp;
internal class MainWindow : Formedge
{
    public MainWindow()
    {
        this.SetVirtualHostNameToBlazorHybrid(new BlazorWebViewOptions
        {
            Scheme = "https",
            HostName = "blazorapp.local",
            RootComponent = typeof(Counter),
            HostPath = "wwwroot/index.html"
        });

        Url = "https://blazorapp.local/";
    }
}
