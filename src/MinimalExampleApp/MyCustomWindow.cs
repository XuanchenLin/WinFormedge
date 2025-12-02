// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Web.WebView2.Core;

using WinFormedge;

namespace MinimalExampleApp;

internal class MyCustomWindow : Formedge
{
    public MyCustomWindow()
    {
        Url = "https://localresources.app/backdrop-styles.html";



        Load += MyCustomForm_Load;

    }

    private void MyCustomForm_Load(object? sender, EventArgs e)
    {
        
        

        SetVirtualHostNameToEmbeddedResourcesMapping(new WinFormedge.WebResource.EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "localresources.app",
            ResourceAssembly = typeof(StartupWindow).Assembly,
            DefaultFolderName = "wwwroot",
        });
    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var settings = opts.UseDefaultWindow();
        settings.SystemBackdropType = SystemBackdropType.Transient;
        settings.ExtendsContentIntoTitleBar = true;

        //BackColor = Color.FromArgb(50, System.Drawing.Color.Orange);


        return settings;
    }
}
