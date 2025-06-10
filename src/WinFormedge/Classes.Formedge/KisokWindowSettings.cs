// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System.Runtime;

namespace WinFormedge;
/// <summary>
/// Provides window settings for a kiosk-style window, which is typically fullscreen and borderless.
/// </summary>
public class KisokWindowSettings : WindowSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the window is in fullscreen mode.
    /// </summary>
    public override bool Fullscreen { get; set; }

    /// <summary>
    /// Gets a value indicating whether the system title bar is present. Always returns false for kiosk windows.
    /// </summary>
    protected internal override bool HasSystemTitlebar => false;

    /// <summary>
    /// Gets or sets the custom window procedure delegate.
    /// </summary>
    protected internal override WindowProc? WndProc { get; set; }

    /// <summary>
    /// Gets or sets the default window procedure delegate.
    /// </summary>
    protected internal override WindowProc? DefWndProc { get; set; }

    /// <summary>
    /// Gets or sets the target screen on which the kiosk window will be displayed.
    /// </summary>
    public Screen TargetScreen { get; set; } = Screen.PrimaryScreen!;

    /// <summary>
    /// Creates the host window using the current kiosk window settings.
    /// </summary>
    /// <returns>A new <see cref="Form"/> instance configured as a kiosk window.</returns>
    protected internal override Form CreateHostWindow()
    {
        var form = new KisokWindow(this)
        {
            // Set properties for the KisokWindow
            FormBorderStyle = FormBorderStyle.None,
            StartPosition = FormStartPosition.Manual,
            WindowState = FormWindowState.Maximized,
            ShowInTaskbar = false,
            Bounds = TargetScreen.Bounds
        };

        return form;
    }

    /// <summary>
    /// Represents the actual kiosk window form, configured according to <see cref="KisokWindowSettings"/>.
    /// </summary>
    class KisokWindow : Form
    {
        private Screen _screen;

        /// <summary>
        /// Initializes a new instance of the <see cref="KisokWindow"/> class with the specified settings.
        /// </summary>
        /// <param name="settings">The <see cref="KisokWindowSettings"/> to apply to the window.</param>
        public KisokWindow(KisokWindowSettings settings)
        {
            Settings = settings;
            // Initialize the window with the settings
            _screen = settings.TargetScreen;
        }

        /// <summary>
        /// Gets the <see cref="KisokWindowSettings"/> associated with this window.
        /// </summary>
        public KisokWindowSettings Settings { get; }

        /// <inheritdoc/>
        protected override void WndProc(ref Message m)
        {
            var wndProcs = Settings.WndProc?.GetInvocationList() ?? [];

            var result = false;

            foreach (WindowProc wndProc in wndProcs)
            {
                result |= wndProc.Invoke(ref m);
            }

            if (result) return;

            base.WndProc(ref m);
        }

        /// <inheritdoc/>
        protected override void DefWndProc(ref Message m)
        {
            var wndProcs = Settings.DefWndProc?.GetInvocationList() ?? [];

            var result = false;
            foreach (WindowProc wndProc in wndProcs)
            {
                result |= wndProc.Invoke(ref m);
            }

            if (result) return;

            base.DefWndProc(ref m);
        }
    }
}

/// <summary>
/// Provides extension methods for <see cref="HostWindowBuilder"/> to support kiosk window settings.
/// </summary>
public static class KisokWindowSettingsExtension
{
    /// <summary>
    /// Configures the builder to use kiosk window settings.
    /// </summary>
    /// <param name="builder">The <see cref="HostWindowBuilder"/> instance.</param>
    /// <returns>A new <see cref="KisokWindowSettings"/> instance for further configuration.</returns>
    public static KisokWindowSettings UseKisokWindow(this HostWindowBuilder builder)
    {
        return new KisokWindowSettings();
    }

}
