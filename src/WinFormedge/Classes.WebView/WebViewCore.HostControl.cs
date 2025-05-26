// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace WinFormedge;
partial class WebViewCore
{

    /// <summary>
    /// Represents a fullscreen window used to display the WebViewCore instance in fullscreen mode.
    /// Handles the transition of the WebView between its container and the fullscreen window,
    /// as well as system color mode and non-client region support.
    /// </summary>
    class FullscreenWindow : Form
    {
        /// <summary>
        /// Stores the original value of non-client region support to restore after exiting fullscreen.
        /// </summary>
        private bool _isNonClientRegionSupportEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="FullscreenWindow"/> class.
        /// </summary>
        /// <param name="webview">The <see cref="WebViewCore"/> instance to display in fullscreen.</param>
        public FullscreenWindow(WebViewCore webview)
        {
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            AutoScaleMode = AutoScaleMode.Dpi;
            ShowInTaskbar = false;
            WebView = webview;
            StartPosition = FormStartPosition.Manual;

            Location = webview.Container.Location;
            Size = webview.Container.Size;

            _isNonClientRegionSupportEnabled = webview.Browser?.Settings.IsNonClientRegionSupportEnabled ?? false;
        }

        /// <summary>
        /// Gets the <see cref="WebViewCore"/> instance associated with this fullscreen window.
        /// </summary>
        public WebViewCore WebView { get; }

        /// <summary>
        /// Handles the creation of the window handle, sets up the WebView controller's parent and bounds,
        /// and disables non-client region support for fullscreen.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            var screen = Screen.FromHandle(WebView.Container.Handle);

            WebView.Controller.Bounds = ClientRectangle;
            WebView.Controller.ParentWindow = Handle;

            WebView.Browser!.Settings.IsNonClientRegionSupportEnabled = false;
        }

        /// <summary>
        /// Handles window resize events, updates the WebView controller's bounds,
        /// and restores non-client region support if needed.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            WebView.Controller.Bounds = ClientRectangle;
            WebView.Browser!.Settings.IsNonClientRegionSupportEnabled = _isNonClientRegionSupportEnabled;
        }

        /// <summary>
        /// Handles the window shown event, hides the original container, maximizes the window,
        /// and activates the fullscreen window.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnShown(EventArgs e)
        {
            WebView.Container.Hide();

            base.OnShown(e);

            WindowState = FormWindowState.Maximized;

            Activate();
        }

        /// <summary>
        /// Handles the window closing event, restores the WebView controller's parent and bounds
        /// to the original container, and shows the container.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            WebView.Controller.ParentWindow = WebView.Container.Handle;

            WebView.Controller.Bounds = WebView.Container.ClientRectangle;

            WebView.Container.Show();

            base.OnClosing(e);
        }

        /// <summary>
        /// Processes Windows messages for the fullscreen window.
        /// Ignores system command messages to prevent unwanted system menu actions.
        /// </summary>
        /// <param name="m">A <see cref="Message"/> that represents the Windows message to process.</param>
        protected override void WndProc(ref Message m)
        {
            var msg = (uint)m.Msg;

            switch (msg)
            {
                case WM_SYSCOMMAND:
                    return;
            }

            base.WndProc(ref m);
        }

    }


    /// <summary>
    /// The host control for the WebViewCore instance.
    /// </summary>
    internal Control _hostControl;

    /// <summary>
    /// Gets the container control for the WebViewCore instance.
    /// Returns the top-level control if available, otherwise returns the host control itself.
    /// </summary>
    public Control Container
    {
        get
        {
            return _hostControl.TopLevelControl ?? _hostControl;
        }
    }

    /// <summary>
    /// Temporary container control used during handle recreation.
    /// </summary>
    private Control? _temporaryContainerControl;

    /// <summary>
    /// Handles the creation of the host control's handle.
    /// Sets up the WebView controller's parent window and manages temporary containers as needed.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    private void HostHandleCreated(object? sender, EventArgs e)
    {
        if (Container.RecreatingHandle)
        {
            if (_temporaryContainerControl == null) throw new NullReferenceException("Temporary container control is null.");

            Controller.ParentWindow = Container.Handle;

            _temporaryContainerControl.Dispose();
            _temporaryContainerControl = null;
        }
        else
        {
            CreateWebView2();
        }

        HandleSystemColorMode();
    }

    /// <summary>
    /// Applies the system color mode (dark or light) to the container window using DWM attributes.
    /// </summary>
    private void HandleSystemColorMode()
    {
        BOOL mode = WinFormedgeApp.Current.GetSystemColorMode() == SystemColorMode.Dark ? true : false;

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
        {
            unsafe
            {
                DwmSetWindowAttribute((HWND)Container.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &mode, (uint)sizeof(BOOL));
            }
        }
    }

    /// <summary>
    /// Handles the destruction of the host control's handle.
    /// Creates a temporary container control to hold the WebView controller during handle recreation.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    private void HostHandleDestroyed(object? sender, EventArgs e)
    {
        if (Container.RecreatingHandle)
        {
            _temporaryContainerControl = new Control();
            _temporaryContainerControl.CreateControl();
            Controller.ParentWindow = _temporaryContainerControl.Handle;
        }
    }

    /// <summary>
    /// Processes Windows messages for the host control.
    /// Handles system setting changes and background erase requests.
    /// </summary>
    /// <param name="m">A <see cref="Message"/> that represents the Windows message to process.</param>
    /// <returns>True if the message was handled; otherwise, false.</returns>
    internal bool HostWndProc(ref Message m)
    {
        var msg = (uint)m.Msg;
        switch (msg)
        {
            case WM_SETTINGCHANGE when m.LParam != 0:
                {
                    OnWmSettingChangeWithImmersiveColorSet(m.LParam);
                }
                break;
            case WM_ERASEBKGND:
                {
                    return Initialized;
                }
        }


        return false;
    }

    /// <summary>
    /// Handles the WM_SETTINGCHANGE message with the ImmersiveColorSet parameter.
    /// Updates the system color mode if the immersive color set has changed.
    /// </summary>
    /// <param name="lParam">The LPARAM value from the Windows message.</param>
    private void OnWmSettingChangeWithImmersiveColorSet(nint lParam)
    {
        const string IMMERSIVE_COLOR_SET = "ImmersiveColorSet";

        const int strlen = 255;

        var buffer = new byte[strlen];

        Marshal.Copy(lParam, buffer, 0, buffer.Length);


        var setting = Encoding.Unicode.GetString(buffer);

        setting = setting.Substring(0, setting.IndexOf('\0'));

        if (setting == IMMERSIVE_COLOR_SET)
        {
            HandleSystemColorMode();
        }

    }
}
