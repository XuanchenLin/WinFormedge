// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;


/// <summary>
/// Provides the default window settings for a <see cref="DefaultWindow"/> instance.
/// </summary>
public sealed class DefaultWindowSettings : WindowSettings
{
    /// <summary>
    /// Gets a value indicating whether the system title bar is present.
    /// </summary>
    internal protected override bool HasSystemTitlebar => !ExtendsContentIntoTitleBar;

    /// <summary>
    /// Gets or sets the custom window procedure delegate.
    /// </summary>
    internal protected override WindowProc? WndProc { get; set; }

    /// <summary>
    /// Gets or sets the default window procedure delegate.
    /// </summary>
    internal protected override WindowProc? DefWndProc { get; set; }

    /// <summary>
    /// Gets or sets the system backdrop type for the window.
    /// </summary>
    public SystemBackdropType SystemBackdropType
    {
        get; set;
    } = SystemBackdropType.Auto;

    /// <summary>
    /// Gets or sets a value indicating whether the system menu is enabled.
    /// </summary>
    public bool SystemMenu
    {
        get; set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether window decorators (such as borders and shadows) are shown.
    /// </summary>
    public bool ShowWindowDecorators { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the window content extends into the title bar area.
    /// </summary>
    public bool ExtendsContentIntoTitleBar { get; set; } = false;

    /// <summary>
    /// Gets or sets the padding offsets for the window edges.
    /// </summary>
    public Padding WindowEdgeOffsets { get; set; } = Padding.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the window is in fullscreen mode.
    /// </summary>
    public override bool Fullscreen
    {
        get => _form?.Fullscreen ?? false;
        set
        {
            if (_form is not null)
            {
                _form.Fullscreen = value;
            }
        }
    }

    /// <summary>
    /// Holds a reference to the created <see cref="DefaultWindow"/> instance.
    /// </summary>
    private DefaultWindow? _form = null;

    /// <summary>
    /// Creates the host window using the current settings.
    /// </summary>
    /// <returns>A new <see cref="Form"/> instance configured with these settings.</returns>
    internal protected override Form CreateHostWindow()
    {
        var form = _form = new DefaultWindow(this)
        {

        };

        return form;
    }
}
