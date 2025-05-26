// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

/// <summary>
/// Represents an action for creating and configuring the startup form of the application.
/// Allows specifying either a standard <see cref="Form"/> or a <see cref="Formedge"/> as the main window.
/// </summary>
public sealed class AppCreationAction
{
    /// <summary>
    /// Gets or sets the standard WinForms <see cref="Form"/> to be used as the startup form.
    /// </summary>
    internal Form? Form { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Formedge"/> instance to be used as the startup form.
    /// </summary>
    internal Formedge? EdgeForm { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppCreationAction"/> class.
    /// </summary>
    public AppCreationAction()
    {
    }

    /// <summary>
    /// Configures the specified <see cref="StartupApplicationContext"/> to use the appropriate startup form.
    /// Prefers <see cref="EdgeForm"/> if set; otherwise, uses <see cref="Form"/> if available.
    /// </summary>
    /// <param name="appContext">The application context to configure.</param>
    internal void ConfigureAppContext(StartupApplicationContext appContext)
    {
        if (EdgeForm != null)
        {
            appContext.UseStartupForm(EdgeForm);
        }
        else if (Form != null)
        {
            appContext.UseStartupForm(Form);
        }
    }
}
