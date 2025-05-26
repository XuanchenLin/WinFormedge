// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

/// <summary>
/// Provides startup configuration options for the WinFormedge application.
/// Allows specifying the main window or starting without a window.
/// </summary>
public sealed class StartupSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupSettings"/> class.
    /// </summary>
    public StartupSettings()
    {

    }

    /// <summary>
    /// Specifies a <see cref="Formedge"/> instance to use as the main window at application startup.
    /// </summary>
    /// <param name="formium">The <see cref="Formedge"/> instance to use as the main window.</param>
    /// <returns>
    /// An <see cref="AppCreationAction"/> configured to use the specified <see cref="Formedge"/> as the main window.
    /// </returns>
    public AppCreationAction UseMainWindow(Formedge formium)
    {
        return new AppCreationAction()
        {
            EdgeForm = formium
        };
    }

    /// <summary>
    /// Specifies a standard <see cref="Form"/> to use as the main window at application startup.
    /// </summary>
    /// <param name="form">The <see cref="Form"/> instance to use as the main window.</param>
    /// <returns>
    /// An <see cref="AppCreationAction"/> configured to use the specified <see cref="Form"/> as the main window.
    /// </returns>
    public AppCreationAction UseMainWindow(Form form)
    {
        return new AppCreationAction()
        {
            Form = form
        };
    }

    /// <summary>
    /// Configures the application to start without displaying any main window.
    /// </summary>
    /// <returns>
    /// An <see cref="AppCreationAction"/> configured to start the application without a main window.
    /// </returns>
    public AppCreationAction StartWitoutWindow()
    {
        return new AppCreationAction() { };
    }
}
