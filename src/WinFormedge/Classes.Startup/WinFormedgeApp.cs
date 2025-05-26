// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;

namespace WinFormedge;

/// <summary>
/// Represents a delegate for processing Windows messages.
/// </summary>
/// <param name="m">The Windows message to process.</param>
/// <returns>True if the message was handled; otherwise, false.</returns>
public delegate bool WindowProc(ref Message m);
/// <summary>
/// Represents the main application class for WinFormedge.
/// Manages application-wide settings, WebView2 environment, culture, color mode, and startup logic.
/// </summary>
public class WinFormedgeApp
{
    private static WinFormedgeApp? _current;

    private CoreWebView2Environment? _environment;

    /// <summary>
    /// Gets the initialized WebView2 environment for the application.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the environment is not initialized.</exception>
    public CoreWebView2Environment WebView2Environment
    {
        get
        {
            if (_environment == null)
            {
                throw new InvalidOperationException("WebView2Environment is not initialized.");
            }

            return _environment!;
        }
    }

    /// <summary>
    /// Gets a value indicating whether developer tools are enabled.
    /// </summary>
    internal bool EnableDevTools { get; init; }

    /// <summary>
    /// Gets a value indicating whether the browser cache should be cleaned up on startup.
    /// </summary>
    internal bool ShouldCleanupCache { get; init; }

    /// <summary>
    /// Gets the culture name used by the application.
    /// </summary>
    internal string CultureName { get; init; } = Application.CurrentCulture.Name;

    /// <summary>
    /// Gets the custom browser executable path, if specified.
    /// </summary>
    internal string? BrowserExecutablePath { get; init; }

    /// <summary>
    /// Gets the custom application data directory, if specified.
    /// </summary>
    internal string? CustomAppDataDirectory { get; init; }

    /// <summary>
    /// Gets the system color mode for the application.
    /// </summary>
    internal SystemColorMode SystemColorMode { get; init; }

    /// <summary>
    /// Gets a value indicating whether the fluent overlay style scrollbar is enabled.
    /// </summary>
    internal bool FluentOverlayStyleScrollbar { get; init; }

    /// <summary>
    /// Determines the effective system color mode (light or dark) based on settings and OS.
    /// </summary>
    /// <returns>The effective <see cref="SystemColorMode"/>.</returns>
    internal SystemColorMode GetSystemColorMode()
    {

        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        static extern bool IsDarkMode();

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 18362))
        {
            try
            {
                if (SystemColorMode == SystemColorMode.Auto)
                {
                    return IsDarkMode() ? SystemColorMode.Dark : SystemColorMode.Light;
                }
                else
                {
                    return SystemColorMode == SystemColorMode.Dark ? SystemColorMode.Dark : SystemColorMode.Light;
                }

            }
            catch
            {

            }
        }

        return SystemColorMode.Light;
    }

    /// <summary>
    /// Gets a value indicating whether the application is currently in dark mode.
    /// </summary>
    internal bool IsDarkMode => GetSystemColorMode() == SystemColorMode.Dark;

    /// <summary>
    /// Gets the application startup logic instance.
    /// </summary>
    internal AppStartup? Startup { get; init; }

    /// <summary>
    /// Gets the default application data directory path.
    /// </summary>
    internal string DefaultAppDataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName ?? "FormedgeApp");

    /// <summary>
    /// Gets the <see cref="CultureInfo"/> used by the application.
    /// </summary>
    public CultureInfo Culture => new CultureInfo(CultureName);

    /// <summary>
    /// Gets the application data folder path, using custom or default location.
    /// </summary>
    public string AppDataFolder => CustomAppDataDirectory ?? DefaultAppDataDirectory;

    /// <summary>
    /// Gets the user data folder path for browser data.
    /// </summary>
    public string UserDataFolder => Path.Combine(AppDataFolder, "User Data");

    /// <summary>
    /// Gets the current <see cref="WinFormedgeApp"/> instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the application is not initialized.</exception>
    public static WinFormedgeApp Current
    {
        get
        {
            if (_current == null)
            {
                throw new InvalidOperationException("FormedgeApp is not initialized.");
            }
            return _current;
        }
    }

    /// <summary>
    /// Creates a new <see cref="AppBuilder"/> for configuring and building a <see cref="WinFormedgeApp"/> instance.
    /// </summary>
    /// <returns>A new <see cref="AppBuilder"/> instance.</returns>
    public static AppBuilder CreateAppBuilder()
    {
        return new AppBuilder();
    }

    /// <summary>
    /// Gets the running application context for startup, if available.
    /// </summary>
    internal StartupApplicationContext? RunningApplicationContext { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WinFormedgeApp"/> class and sets it as the current instance.
    /// </summary>
    internal WinFormedgeApp()
    {
        _current = this;
    }

    /// <summary>
    /// Builds and sets additional browser command-line arguments for the WebView2 environment.
    /// </summary>
    /// <param name="opts">The WebView2 environment options to configure.</param>
    private void BuildAdditionalBrowserArguments(CoreWebView2EnvironmentOptions opts)
    {
        var browserArgs = new NameValueCollection();
        Startup?.ConfigureAdditionalBrowserArgs(browserArgs);

        //browserArgs.Add("--enable-features", "msWebView2EnableDraggableRegions");

        var argumentDict = new Dictionary<string, string?>();

        if (browserArgs != null && browserArgs.AllKeys != null)
        {
            foreach (var key in browserArgs.AllKeys)
            {
                if (key == null) continue;

                var value = browserArgs.GetValues(key);

                argumentDict[key] = value != null ? string.Join(",", value) : null;
            }
        }

        var browserArgsArray = new List<string>();

        foreach (var key in argumentDict.Keys)
        {
            var value = argumentDict[key];


            if (value != null)
            {
                browserArgsArray.Add($"{key}={value}");
            }
            else
            {
                browserArgsArray.Add($"{key}");
            }
        }

        opts.AdditionalBrowserArguments = string.Join(" ", browserArgsArray);
    }

    /// <summary>
    /// Runs the WinFormedge application, initializing culture, WebView2 environment, and main window.
    /// </summary>
    public void Run()
    {
        var retval = Startup?.OnApplicationLaunched(Environment.GetCommandLineArgs()) ?? true;

        if (!retval) return;

        Application.CurrentCulture = Culture;
        CultureInfo.DefaultThreadCurrentCulture = Culture;
        CultureInfo.DefaultThreadCurrentUICulture = Culture;

        var opts = new CoreWebView2EnvironmentOptions()
        {
            Language = $"{Culture.Name}".ToLower(),
            AreBrowserExtensionsEnabled = false,
            ExclusiveUserDataFolderAccess = false,
            AdditionalBrowserArguments = string.Empty,
            EnableTrackingPrevention = false,
            IsCustomCrashReportingEnabled = true,
            ReleaseChannels = CoreWebView2ReleaseChannels.Stable,
            ScrollBarStyle = FluentOverlayStyleScrollbar ? CoreWebView2ScrollbarStyle.FluentOverlay : CoreWebView2ScrollbarStyle.Default,
            ChannelSearchKind = CoreWebView2ChannelSearchKind.MostStable,
            AllowSingleSignOnUsingOSPrimaryAccount = false,
        };

        BuildAdditionalBrowserArguments(opts);

        Startup?.ConfigureSchemeRegistrations(opts.CustomSchemeRegistrations);



        if (ShouldCleanupCache)
        {
            try
            {
                if (Directory.Exists(UserDataFolder))
                {
                    Directory.Delete(UserDataFolder, true);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error deleting cache directory: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error deleting cache directory: {ex.Message}");
            }
        }

        _environment = CoreWebView2Environment.CreateAsync(BrowserExecutablePath, UserDataFolder, opts).GetAwaiter().GetResult();


        var startup = Startup?.OnApplicationStartup(new StartupSettings());

        if (startup == null)
        {
            Environment.Exit(0);
            return;
        }



        RunningApplicationContext = new StartupApplicationContext();

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Startup?.OnApplicationException(e.ExceptionObject as Exception);
        };

        AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
        {
            Startup?.OnApplicationTerminated();
        };

        startup.ConfigureAppContext(RunningApplicationContext);


        Application.Run(RunningApplicationContext);
    }
}


