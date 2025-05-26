// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

/// <summary>
/// Delegate for creating a host window.
/// </summary>
public delegate Form HostWindowCreationDelegate();

//public delegate bool RequireChangeFullscreenStateDelegate(bool fullscreen);

/// <summary>
/// Represents the base class for window settings in WinFormedge.
/// Provides properties and methods for configuring window appearance and behavior.
/// </summary>
public abstract class WindowSettings
{
    /// <summary>
    /// Gets a value indicating whether the window has a system title bar.
    /// </summary>
    internal protected abstract bool HasSystemTitlebar { get; }

    /// <summary>
    /// Gets or sets the custom window procedure (WndProc) for the window.
    /// </summary>
    internal protected abstract WindowProc? WndProc { get; set; }

    /// <summary>
    /// Gets or sets the default window procedure (DefWndProc) for the window.
    /// </summary>
    internal protected abstract WindowProc? DefWndProc { get; set; }

    //public string WindowCaption { get; set; } = nameof(WinFormedge);
    //public Size Size { get => _size ?? Size.Empty; set => _size = value; }
    //public Point Location { get => _location ?? Point.Empty; set => _location = value; }
    //public FormStartPosition StartPosition { get; set; } = FormStartPosition.WindowsDefaultLocation;
    //public Size? MaximumSize { get; set; }
    //public Size? MinimumSize { get; set; }
    //public Icon? Icon { get; set; }
    //public bool Resizable { get; set; } = true;
    //public bool Maximizable { get; set; } = true;
    //public bool Minimizable { get; set; } = true;
    //public bool TopMost { get; set; } = false;
    //public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
    //public bool Enabled { get; set; } = true;
    //public bool AllowFullScreen { get; set; } = false;
    //public bool ShowInTaskbar { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the window is in fullscreen mode.
    /// </summary>
    public abstract bool Fullscreen { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the window is resizable.
    /// </summary>
    public bool Resizable { get; set; } = true;

    //public bool AllowSystemMenuOnNonClientRegion { get; set; } = true;
    //public Color BackColor { get; set; } = FormedgeApp.Current.IsDarkMode ? Color.DimGray : Color.White;

    //internal Color SolidBackColor =>
    //    Color.FromArgb(255, BackColor.R, BackColor.G, BackColor.B);

    /// <summary>
    /// Creates the host window instance.
    /// </summary>
    /// <returns>The created <see cref="Form"/> instance.</returns>
    internal protected abstract Form CreateHostWindow();

    /// <summary>
    /// Configures additional WinForms properties for the specified form.
    /// </summary>
    /// <param name="form">The form to configure.</param>
    internal protected virtual void ConfigureWinFormProps(Form form)
    {

    }

    /// <summary>
    /// Gets the JavaScript code specified for the window, if any.
    /// </summary>
    internal protected virtual string? WindowSpecifiedJavaScript => null;
}
