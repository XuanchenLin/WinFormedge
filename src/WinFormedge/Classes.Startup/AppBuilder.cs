// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;

namespace WinFormedge;

public class AppBuilder
{
    private string? _browserExecutablePath;
    private string? _appDataDirectory;
    private string _cultureName = $"{Application.CurrentCulture.Name}".ToLower();
    private bool _enableDevTools = false;
    private bool _shouldCleanupCache = false;
    private AppStartup? _startup;
    private SystemColorMode _colorMode = SystemColorMode.Auto;
    private bool _scrollbarUsingFluentOverlay = false;

    internal AppBuilder()
    {
        
    }


    public AppBuilder UseCustomBrowserExecutablePath(string path)
    {
        // Set the custom browser executable path
        // This is where you can set the path to the custom browser executable
        // for your application.
        _browserExecutablePath = path;
        return this;
    }

    public AppBuilder UseCustomAppDataFolder(string appDataFolder)
    {
        // Set the custom app data folder
        // This is where you can set the path to the custom app data folder
        // for your application.
        _appDataDirectory = appDataFolder;
        return this;
    }

    public AppBuilder UseCulture(string cultureName)
    {
        // Set the culture name
        _cultureName = cultureName;
        return this;
    }

    public AppBuilder UseDevTools()
    {
        // Enable DevTools
        // This is where you can enable the DevTools for your application.
        _enableDevTools = true;
        return this;
    }

    //public AppBuilder ConfigureAdditionalBrowserArguments(Action<NameValueCollection> configureAdditionalBrowserArgs)
    //{
    //    // Configure additional browser arguments
    //    _configureAdditionalBrowserArgs += configureAdditionalBrowserArgs;
    //    return this;
    //}

    public AppBuilder SetColorMode(SystemColorMode colorMode)
    {
        _colorMode = colorMode;
        return this;
    }

    public AppBuilder CacheCleanup()
    {
        // Perform cache cleanup
        // This is where you can perform any necessary cache cleanup for your application.
        _shouldCleanupCache = true;
        return this;
    }

    public AppBuilder UseWinFormedgeApp<TApp>()
        where TApp : AppStartup, new()
    {
        _startup = Activator.CreateInstance<TApp>();
        return this;
    }

    public AppBuilder UseModernStyleScrollbar()
    {
        // Use modern scrollbar style
        // This is where you can set the modern scrollbar style for your application.
        _scrollbarUsingFluentOverlay = true;
        return this;
    }

    public WinFormedgeApp Build()
    {
        return new WinFormedgeApp()
        {
            EnableDevTools = _enableDevTools,
            ShouldCleanupCache = _shouldCleanupCache,
            CultureName = _cultureName,
            BrowserExecutablePath = _browserExecutablePath,
            CustomAppDataDirectory = _appDataDirectory,
            Startup = _startup,
            SystemColorMode = _colorMode,
            FluentOverlayStyleScrollbar = _scrollbarUsingFluentOverlay,
        };
    }

}
