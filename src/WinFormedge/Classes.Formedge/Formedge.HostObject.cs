// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

public abstract partial class Formedge
{
    /// <summary>
    /// Exposes window and browser control methods and properties to external consumers, such as JavaScript via WebView2.
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class FormedgeHostObject
    {
        private readonly Formedge _formedge;

        /// <summary>
        /// Gets or sets a value indicating whether the window is currently activated (focused).
        /// </summary>
        public bool Activated { get; set; }

        /// <summary>
        /// Gets the current window state as a string ("fullscreen", "minimized", "maximized", or "normal").
        /// </summary>
        public string WindowState => _formedge.Fullscreen ? "fullscreen" : _formedge.WindowState.ToString().ToLower();

        /// <summary>
        /// Gets a value indicating whether the window has a system title bar.
        /// </summary>
        public bool HasTitleBar => _formedge.HasSystemTitlebar;

        /// <summary>
        /// Gets the version of the Formedge library.
        /// </summary>
        public string FormedgeVersion => typeof(Formedge).Assembly.GetName().Version?.ToString() ?? string.Empty;

        /// <summary>
        /// Gets the version string of the embedded Chromium browser.
        /// </summary>
        public string ChromiumVersion => _formedge.WebView.Browser?.Environment.BrowserVersionString ?? string.Empty;

        /// <summary>
        /// Gets or sets the left position of the window in pixels.
        /// </summary>
        public int Left
        {
            get => _formedge.Left;
            set => _formedge.Left = value;
        }

        /// <summary>
        /// Gets or sets the top position of the window in pixels.
        /// </summary>
        public int Top
        {
            get => _formedge.Top;
            set => _formedge.Top = value;
        }

        /// <summary>
        /// Gets or sets the width of the window in pixels.
        /// </summary>
        public int Width
        {
            get => _formedge.Width;
            set => _formedge.Width = value;
        }

        /// <summary>
        /// Gets or sets the height of the window in pixels.
        /// </summary>
        public int Height
        {
            get => _formedge.Height;
            set => _formedge.Height = value;
        }

        /// <summary>
        /// Minimizes the window.
        /// </summary>
        public void Minimize()
        {
            _formedge.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Maximizes the window.
        /// </summary>
        public void Maximize()
        {
            _formedge.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Restores the window from minimized, maximized, or fullscreen state to normal.
        /// </summary>
        public void Restore()
        {
            if (_formedge.Fullscreen)
            {
                _formedge.Fullscreen = false;
            }
            else
            {
                _formedge.WindowState = FormWindowState.Normal;
            }
        }

        /// <summary>
        /// Sets the window to fullscreen mode.
        /// </summary>
        public void Fullscreen()
        {
            _formedge.Fullscreen = true;
        }

        /// <summary>
        /// Toggles the fullscreen state of the window.
        /// </summary>
        public void ToggleFullscreen()
        {
            _formedge.Fullscreen = !_formedge.Fullscreen;
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void Close()
        {
            _formedge.Close();
        }

        /// <summary>
        /// Activates (focuses) the window if it is not in fullscreen mode.
        /// </summary>
        public void Activate()
        {
            if (_formedge.Fullscreen) return;

            _formedge.Activate();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormedgeHostObject"/> class.
        /// </summary>
        /// <param name="formedge">The parent <see cref="Formedge"/> instance.</param>
        internal FormedgeHostObject(Formedge formedge)
        {
            _formedge = formedge;

            Activated = _formedge.HostWindow.Focused;

            _formedge.Activated += (_, _) => Activated = true;
            _formedge.Deactivate += (_, _) => Activated = false;
        }
    }
}
