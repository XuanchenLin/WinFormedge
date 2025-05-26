// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

/// <summary>
/// Provides a builder for configuring and creating a <see cref="WinFormedgeApp"/> instance.
/// </summary>
public class AppBuilder
{
    /// <summary>
    /// The custom browser executable path.
    /// </summary>
    private string? _browserExecutablePath;

    /// <summary>
    /// The custom application data directory.
    /// </summary>
    private string? _appDataDirectory;

    /// <summary>
    /// The culture name to use for the application.
    /// </summary>
    private string _cultureName = $"{Application.CurrentCulture.Name}".ToLower();

    /// <summary>
    /// Indicates whether DevTools should be enabled.
    /// </summary>
    private bool _enableDevTools = false;

    /// <summary>
    /// Indicates whether cache cleanup should be performed.
    /// </summary>
    private bool _shouldCleanupCache = false;

    /// <summary>
    /// The application startup logic.
    /// </summary>
    private AppStartup? _startup;

    /// <summary>
    /// The system color mode for the application.
    /// </summary>
    private SystemColorMode _colorMode = SystemColorMode.Auto;

    /// <summary>
    /// Indicates whether the modern (fluent overlay) scrollbar style should be used.
    /// </summary>
    private bool _scrollbarUsingFluentOverlay = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppBuilder"/> class.
    /// </summary>
    internal AppBuilder()
    {

    }

    /// <summary>
    /// Sets the custom browser executable path for the application.
    /// </summary>
    /// <param name="path">The path to the custom browser executable.</param>
    /// <returns>The current <see cref="AppBuilder"/> instance.</returns>
    public AppBuilder UseCustomBrowserExecutablePath(string path)
    {
        // Set the custom browser executable path
        // This is where you can set the path to the custom browser executable
        // for your application.
        _browserExecutablePath = path;
        return this;
    }

    /// <summary>
    /// Sets the custom application data folder for the application.
    /// </summary>
    /// <param name="appDataFolder">The path to the custom app data folder.</param>
    /// <returns>The current <see cref="AppBuilder"/> instance.</returns>
    public AppBuilder UseCustomAppDataFolder(string appDataFolder)
    {
        // Set the custom app data folder
        // This is where you can set the path to the custom app data folder
        // for your application.
        _appDataDirectory = appDataFolder;
        return this;
    }

    /// <summary>
    /// Sets the culture name for the application.
    /// </summary>
    /// <param name="cultureName">The culture name to use.</param>
    /// <returns>The current <see cref="AppBuilder"/> instance.</returns>
    public AppBuilder UseCulture(string cultureName)
    {
        // Set the culture name
        _cultureName = cultureName;
        return this;
    }

    /// <summary>
    /// Enables DevTools for the application.
    /// </summary>
    /// <returns>The current <see cref="AppBuilder"/> instance.</returns>
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

    /// <summary>
    /// Sets the system color mode for the application.
    /// </summary>
    /// <param name="colorMode">The <see cref="SystemColorMode"/> to use.</param>
    /// <returns>The current <see cref="AppBuilder"/> instance.</returns>
    public AppBuilder SetColorMode(SystemColorMode colorMode)
    {
        _colorMode = colorMode;
        return this;
    }

    /// <summary>
    /// Enables cache cleanup for the application.
    /// </summary>
    /// <returns>The current <see cref="AppBuilder"/> instance.</returns>
    public AppBuilder CacheCleanup()
    {
        // Perform cache cleanup
        // This is where you can perform any necessary cache cleanup for your application.
        _shouldCleanupCache = true;
        return this;
    }

    /// <summary>
    /// Sets the application startup logic using the specified <see cref="AppStartup"/> type.
    /// </summary>
    /// <typeparam name="TApp">The type of <see cref="AppStartup"/> to use.</typeparam>
    /// <returns>The current <see cref="AppBuilder"/> instance.</returns>
    public AppBuilder UseWinFormedgeApp<TApp>()
        where TApp : AppStartup, new()
    {
        _startup = Activator.CreateInstance<TApp>();
        return this;
    }

    /// <summary>
    /// Enables the modern (fluent overlay) scrollbar style for the application.
    /// </summary>
    /// <returns>The current <see cref="AppBuilder"/> instance.</returns>
    public AppBuilder UseModernStyleScrollbar()
    {
        // Use modern scrollbar style
        // This is where you can set the modern scrollbar style for your application.
        _scrollbarUsingFluentOverlay = true;
        return this;
    }

    /// <summary>
    /// Builds and returns a configured <see cref="WinFormedgeApp"/> instance.
    /// </summary>
    /// <returns>A configured <see cref="WinFormedgeApp"/> instance.</returns>
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
