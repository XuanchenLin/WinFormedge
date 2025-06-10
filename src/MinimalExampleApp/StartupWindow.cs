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

namespace MinimalExampleApp;
internal class StartupWindow : Formedge
{
    public StartupWindow()
    {
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.Transparent;
        Maximizable = false;
        Size = new Size(620, 620);
        Url = "https://localresources.app/startup/";
        Icon = new Icon(new MemoryStream(Properties.Resources.ColorsIcon));


        SetVirtualHostNameToEmbeddedResourcesMapping(new WinFormedge.WebResource.EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "localresources.app",
            ResourceAssembly = typeof(StartupWindow).Assembly,
            DefaultFolderName = "wwwroot"
        });
    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var settings = opts.UseDefaultWindow();

        settings.ExtendsContentIntoTitleBar = true;
        settings.ShowWindowDecorators = false;
        settings.SystemBackdropType = SystemBackdropType.None;
        settings.Resizable = false;

        return settings;
    }
}
