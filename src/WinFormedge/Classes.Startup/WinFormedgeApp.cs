// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


using Windows.Win32;

namespace WinFormedge;

public delegate bool WindowProc(ref Message m);
public class WinFormedgeApp
{
    private static WinFormedgeApp? _current;

    private CoreWebView2Environment? _environment;
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

    internal bool EnableDevTools { get; init; }
    internal bool ShouldCleanupCache { get; init; }
    internal string CultureName { get; init; } = Application.CurrentCulture.Name;
    internal string? BrowserExecutablePath { get; init; }
    internal string? CustomAppDataDirectory { get; init; }
    internal SystemColorMode SystemColorMode { get; init; }
    internal bool FluentOverlayStyleScrollbar { get; init; }

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

    internal bool IsDarkMode => GetSystemColorMode() == SystemColorMode.Dark;


    internal AppStartup? Startup { get; init; }

    internal string DefaultAppDataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName ?? "FormedgeApp");

    public CultureInfo Culture => new CultureInfo(CultureName);

    public string AppDataFolder => CustomAppDataDirectory ?? DefaultAppDataDirectory;

    public string UserDataFolder => Path.Combine(AppDataFolder, "User Data");

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


    public static AppBuilder CreateAppBuilder()
    {
        return new AppBuilder();
    }

    internal StartupApplicationContext? RunningApplicationContext { get; private set; }


    internal WinFormedgeApp()
    {
        _current = this;
    }

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


