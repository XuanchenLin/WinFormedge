// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentUIBlazorExampleApp.Components;
using Microsoft.FluentUI.AspNetCore.Components;

using Microsoft.Extensions.DependencyInjection;

using WinFormedge;
using WinFormedge.Blazor;
using Microsoft.AspNetCore.Components.Web;

namespace FluentUIBlazorExampleApp;
internal class MainWindow : Formedge
{
    public MainWindow()
    {
        Icon = new System.Drawing.Icon(new MemoryStream(Properties.Resources.WinFormiumBlazor));

        Url = "https://blazorapp.local/";

        Load += MainWindow_Load;

        BackColor = System.Drawing.Color.Transparent;

        WindowTitle = "FluentUI Blazor";

    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var settings = opts.UseDefaultWindow();
        settings.ExtendsContentIntoTitleBar = true;

        // Windows 11 or later
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
        {
            settings.SystemBackdropType = SystemBackdropType.Transient;
        }

        return settings;
    }



    private void MainWindow_Load(object? sender, EventArgs e)
    {
        var opts = new BlazorHybridOptions
        {
            Scheme = "https",
            HostName = "blazorapp.local",
            HostPage = "wwwroot/index.html",
            ConfigureServices = ConfigureServices
        };

        opts.RootComponents.Add<App>("#app");
        opts.RootComponents.Add<HeadOutlet>("head::after");

        opts.StaticResources.Add(new BlazorHybridAssemblyResources(typeof(MainWindow).Assembly));

        this.SetVirtualHostNameToBlazorHybrid(opts);

        
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddFluentUIComponents(options =>
        {
            options.ValidateClassNames = false;

        });

        services.AddHttpClient();
    }
}
