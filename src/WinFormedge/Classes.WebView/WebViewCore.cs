// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace WinFormedge;
internal partial class WebViewCore
{
    /// <summary>
    /// The host control for the WebViewCore instance.
    /// </summary>
    internal Control _hostControl;

    /// <summary>
    /// The constant string representing the about:blank URL.
    /// </summary>
    private const string ABOUT_BLANK = "about:blank";

    /// <summary>
    /// Holds the current WebView2 controller instance.
    /// </summary>
    private CoreWebView2Controller? _controller;

    /// <summary>
    /// Stores a deferred URL to navigate to after initialization.
    /// </summary>
    private string? _defferedUrl;

    /// <summary>
    /// Indicates whether the browser is currently in fullscreen mode.
    /// </summary>
    private bool _fullscreen;

    /// <summary>
    /// Holds the current fullscreen window instance, if any.
    /// </summary>
    private FullscreenWindow? _fullscreenWindow = null;

    /// <summary>
    /// Indicates whether fullscreen mode is required after initialization.
    /// </summary>
    private bool _isFullscrrenRequired;

    /// <summary>
    /// Temporary container control used during handle recreation.
    /// </summary>
    private Control? _temporaryContainerControl;

    /// <summary>
    /// Occurs before the WebView2 settings are finalized, allowing customization of settings.
    /// </summary>
    public event Action<CoreWebView2Settings>? ConfigureSettings;

    /// <summary>
    /// Occurs when the WebView2 instance has been created and initialized.
    /// </summary>
    public event EventHandler? WebViewCreated;

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
    /// Gets or sets a value indicating whether the browser should be displayed in fullscreen mode.
    /// </summary>
    public bool Fullscreen
    {
        get => _fullscreen;
        set
        {
            if (_fullscreen == value) return;

            _fullscreen = value;

            if (Initialized)
            {
                HandleFullscreenChanged();
            }
            else
            {
                _isFullscrrenRequired = _fullscreen;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the WebView2 controller has been initialized.
    /// </summary>
    public bool Initialized => _controller != null;

    /// <summary>
    /// Gets or sets the current URL of the web browser.
    /// When getting, returns the current source URL or a blank page if the browser is not initialized.
    /// When setting, navigates the browser to the specified URL if the browser is available;
    /// otherwise, stores the URL to be navigated to when the browser is initialized.
    /// </summary>
    public string Url
    {
        get => Browser?.Source ?? ABOUT_BLANK;
        set
        {
            if (Browser != null)
            {
                Browser.Navigate(value);
            }
            else
            {
                _defferedUrl = value;
            }
        }
    }

    /// <summary>
    /// Gets the current CoreWebView2 browser instance, or null if not initialized.
    /// </summary>
    internal CoreWebView2? Browser => _controller?.CoreWebView2;

    /// <summary>
    /// Gets the current WebView2 controller instance.
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown if the controller is not initialized.</exception>
    internal CoreWebView2Controller Controller => _controller ?? throw new NullReferenceException(nameof(Controller));

    /// <summary>
    /// Gets the current WebView2 environment instance.
    /// </summary>
    internal CoreWebView2Environment WebViewEnvironment => WinFormedgeApp.Current!.WebView2Environment;

    /// <summary>
    /// Gets the web resource manager responsible for handling custom resource requests.
    /// </summary>
    private WebResourceManager WebResourceManager { get; } = new WebResourceManager();

    /// <summary>
    /// Initializes a new instance of the <see cref="WebViewCore"/> class with the specified host control.
    /// Subscribes to the <c>HandleCreated</c> and <c>HandleDestroyed</c> events of the container.
    /// </summary>
    /// <param name="hostControl">The control that will host the WebView.</param>
    public WebViewCore(Control hostControl)
    {
        _hostControl = hostControl;
        Container.HandleCreated += HostHandleCreated;
        Container.HandleDestroyed += HostHandleDestroyed;
    }

    /// <summary>
    /// Closes the WebView and releases associated resources.
    /// If the WebView is initialized, the controller is closed and set to <c>null</c>.
    /// </summary>
    public void Close()
    {
        if (Initialized)
        {
            Controller.Close();
            _controller = null;
        }
    }
    /// <summary>
    /// Registers a custom web resource handler to intercept and handle web resource requests.
    /// </summary>
    /// <param name="resourceHandler">The web resource handler to register.</param>
    public void RegisterWebResourceHander(WebResourceHandler resourceHandler)
    {
        WebResourceManager.RegisterWebResourceHander(resourceHandler);
    }

    /// <summary>
    /// Unregisters a previously registered web resource handler.
    /// </summary>
    /// <param name="scheme">The scheme of the web resource handler to unregister.</param>
    /// <param name="hostName">The host name of the web resource handler to unregister.</param>
    public void UnregisterWebResourceHander(string scheme, string hostName)
    {
        WebResourceManager.UnregisterWebResourceHander(scheme, hostName);
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
    /// Asynchronously creates and initializes the WebView2 controller and browser instance.
    /// Configures settings, event handlers, and resource management.
    /// </summary>
    private async void CreateWebView2()
    {
        var opts = WebViewEnvironment.CreateCoreWebView2ControllerOptions();

        Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--autoplay-policy=no-user-gesture-required");

        opts.ScriptLocale = WinFormedgeApp.Current.CultureName;
        opts.ProfileName = Application.ProductName;

        var controller = _controller = await WebViewEnvironment.CreateCoreWebView2ControllerAsync(Container.Handle);

        if (controller == null || controller.CoreWebView2 == null)
        {
            var ex = new InvalidOperationException("Failed to create WebView2 controller.");
            throw ex;
        }

        controller.ShouldDetectMonitorScaleChanges = true;
        controller.Bounds = Container.ClientRectangle;
        controller.DefaultBackgroundColor = Color.Transparent;

        var webview = controller!.CoreWebView2;

        webview.ProcessFailed += (_, args) =>
        {
            webview.Reload();
        };

        webview.Settings.AreBrowserAcceleratorKeysEnabled = false;
        webview.Settings.AreDefaultScriptDialogsEnabled = true;
        webview.Settings.IsGeneralAutofillEnabled = false;
        webview.Settings.IsPasswordAutosaveEnabled = false;
        webview.Settings.IsZoomControlEnabled = false;
        webview.Settings.IsStatusBarEnabled = false;
        webview.Settings.IsSwipeNavigationEnabled = false;
        webview.Settings.IsReputationCheckingRequired = false;
        webview.Settings.IsPinchZoomEnabled = false;
        webview.Settings.IsNonClientRegionSupportEnabled = true;

        ConfigureSettings?.Invoke(webview.Settings);


        webview.Profile.PreferredColorScheme = WinFormedgeApp.Current.SystemColorMode switch
        {
            SystemColorMode.Dark => CoreWebView2PreferredColorScheme.Dark,
            SystemColorMode.Auto => CoreWebView2PreferredColorScheme.Auto,
            _ => CoreWebView2PreferredColorScheme.Light,
        };

        //Container.VisibleChanged += (_, _) =>
        //{
        //    Controller.IsVisible = Container.Visible;
        //};

        Container.Move += (_, _) =>
        {
            if (Fullscreen) return;
            controller.NotifyParentWindowPositionChanged();
        };

        Container.Resize += (_, _) =>
        {
            if (Fullscreen) return;

            if (IsIconic((HWND)Container.Handle))
            {
                Controller.IsVisible = false;
                return;
            }
            else
            {
                Controller.IsVisible = true;
            }

            Controller.Bounds = new System.Drawing.Rectangle(0, 0, Container.ClientRectangle.Width, Container.ClientRectangle.Height);
        };

        WebResourceManager.Initialize(webview);

        var version = typeof(Formedge).Assembly.GetName().Version?.ToString() ?? webview.Environment.BrowserVersionString;
        var script = Properties.Resources.Version;
        script = script.Replace("{{WINFORMEDGE_VERSION_INFO}}", $"%cChromium%c{webview.Environment.BrowserVersionString}%c %cFormedge%c{version}%c %cArchitect%c{(IntPtr.Size == 4 ? "x86" : "x64")}%c");

        await webview.AddScriptToExecuteOnDocumentCreatedAsync(script);

        WebViewCreated?.Invoke(Container, EventArgs.Empty);

        webview.Navigate(_defferedUrl ?? ABOUT_BLANK);

        controller.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);

        _defferedUrl = null;

        if (_isFullscrrenRequired)
        {
            HandleFullscreenChanged();
            _isFullscrrenRequired = false;
        }
    }

    /// <summary>
    /// Handles changes to the fullscreen state, creating or closing the fullscreen window as needed.
    /// </summary>
    private void HandleFullscreenChanged()
    {
        if (!Initialized)
        {
            _isFullscrrenRequired = Fullscreen;
            return;
        }

        var fullscreen = Fullscreen;

        if (fullscreen)
        {
            if (_fullscreenWindow is null)
            {
                _fullscreenWindow = new FullscreenWindow(this);
            }
            _fullscreenWindow.Show();
        }
        else
        {
            _fullscreenWindow?.Close();
            _fullscreenWindow = null;
        }
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
        /// Gets the <see cref="WebViewCore"/> instance associated with this fullscreen window.
        /// </summary>
        public WebViewCore WebView { get; }

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
}
