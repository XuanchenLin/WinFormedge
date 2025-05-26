// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;

namespace WinFormedge;
/// <summary>
/// Provides the base class for application startup logic in WinFormedge.
/// Allows customization of application launch, startup, exception handling, termination, 
/// custom scheme registration, and additional browser arguments.
/// </summary>
public abstract class AppStartup
{
    /// <summary>
    /// Called when the application is launched.
    /// Can be overridden to perform pre-startup logic.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    /// <returns>True if the application should continue launching; otherwise, false.</returns>
    internal protected virtual bool OnApplicationLaunched(string[] args)
    {
        return true;
    }

    /// <summary>
    /// Called to configure and create the main application window or context.
    /// Must be implemented in derived classes to specify the startup action.
    /// </summary>
    /// <param name="options">The startup settings for the application.</param>
    /// <returns>An <see cref="AppCreationAction"/> describing how to create the main window, or null to abort startup.</returns>
    internal abstract protected AppCreationAction? OnApplicationStartup(StartupSettings options);

    /// <summary>
    /// Called when an unhandled exception occurs in the application.
    /// Can be overridden to provide custom exception handling logic.
    /// </summary>
    /// <param name="exception">The exception that occurred, or null if not available.</param>
    internal protected virtual void OnApplicationException(Exception? exception = null)
    {
    }

    /// <summary>
    /// Called when the application is terminating.
    /// Can be overridden to perform cleanup or shutdown logic.
    /// </summary>
    internal protected virtual void OnApplicationTerminated()
    {
    }

    /// <summary>
    /// Allows registration of custom WebView2 URI schemes before browser initialization.
    /// Can be overridden to add custom scheme registrations.
    /// </summary>
    /// <param name="customSchemeRegistrations">A list to which custom scheme registrations can be added.</param>
    internal protected virtual void ConfigureSchemeRegistrations(List<CoreWebView2CustomSchemeRegistration> customSchemeRegistrations)
    {
    }

    /// <summary>
    /// Allows configuration of additional browser command-line arguments before browser initialization.
    /// Can be overridden to add or modify browser arguments.
    /// </summary>
    /// <param name="additionalBrowserArgs">A collection to which additional browser arguments can be added.</param>
    internal protected virtual void ConfigureAdditionalBrowserArgs(NameValueCollection additionalBrowserArgs)
    {
    }

}
