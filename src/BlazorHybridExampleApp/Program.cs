// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using WinFormedge;

namespace BlazorHybridExampleApp;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        var app = WinFormedgeApp.CreateAppBuilder()
            .UseCulture(Application.CurrentCulture.Name)
            .UseDevTools()
            .UseModernStyleScrollbar()
            .UseWinFormedgeApp<MyFormedgeApp>()
            .Build();

        app.Run();
    }
}