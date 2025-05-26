// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

/// <summary>
/// Provides an application context for WinFormedge startup, allowing selection between standard Form and Formedge window types.
/// </summary>
class StartupApplicationContext : ApplicationContext
{
    /// <summary>
    /// Specifies the type of window currently running in the application context.
    /// </summary>
    internal enum RunningType
    {
        /// <summary>
        /// No window is currently running.
        /// </summary>
        None,
        /// <summary>
        /// A standard WinForms Form is running.
        /// </summary>
        Form,
        /// <summary>
        /// A Formedge window is running.
        /// </summary>
        Formium
    }

    /// <summary>
    /// Holds the current Formedge instance if running.
    /// </summary>
    Formedge? _formium;

    /// <summary>
    /// Holds the current standard Form instance if running.
    /// </summary>
    Form? _form;

    /// <summary>
    /// Gets the type of window currently running in the application context.
    /// </summary>
    internal RunningType RunningOn => _formium == null && _form == null ? RunningType.None : _formium != null ? RunningType.Formium : RunningType.Form;

    /// <summary>
    /// Sets the startup window to a Formedge instance and updates the main form reference.
    /// </summary>
    /// <param name="formium">The Formedge instance to use as the startup window.</param>
    public void UseStartupForm(Formedge formium)
    {
        _form = null;
        _formium = formium;
        MainForm = formium.HostWindow;
    }

    /// <summary>
    /// Sets the startup window to a standard Form instance and updates the main form reference.
    /// </summary>
    /// <param name="form">The Form instance to use as the startup window.</param>
    public void UseStartupForm(Form form)
    {
        _formium = null;
        _form = form;
        MainForm = form;
    }

    /// <summary>
    /// Gets the main window handle as an <see cref="IWin32Window"/> for the currently running window type.
    /// </summary>
    public IWin32Window? MainWindowHandle => RunningOn switch
    {
        RunningType.Form => _form,
        RunningType.Formium => _formium,
        _ => null
    };
}
