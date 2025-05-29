// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;
/// <summary>
/// Represents the core logic for managing the WebView2 browser instance, including initialization,
/// fullscreen handling, and web resource management.
/// </summary>
partial class WebViewCore
{
    /// <summary>
    /// Occurs when the WebView2 instance has been created and initialized.
    /// </summary>
    public event EventHandler? WebViewCreated;

    /// <summary>
    /// Occurs before the WebView2 settings are finalized, allowing customization of settings.
    /// </summary>
    public event Action<CoreWebView2Settings>? ConfigureSettings;

    /// <summary>
    /// Gets a value indicating whether the WebView2 controller has been initialized.
    /// </summary>
    public bool Initialized => _controller != null;

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
    /// <param name="resourceHandler">The web resource handler to unregister.</param>
    public void UnregisterWebResourceHander(WebResourceHandler resourceHandler)
    {
        WebResourceManager.UnregisterWebResourceHander(resourceHandler);
    }

    /// <summary>
    /// Gets the current WebView2 environment instance.
    /// </summary>
    internal CoreWebView2Environment WebViewEnvironment => WinFormedgeApp.Current!.WebView2Environment;

    /// <summary>
    /// Gets the current WebView2 controller instance.
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown if the controller is not initialized.</exception>
    internal CoreWebView2Controller Controller => _controller ?? throw new NullReferenceException(nameof(Controller));

    /// <summary>
    /// Gets the current CoreWebView2 browser instance, or null if not initialized.
    /// </summary>
    internal CoreWebView2? Browser => _controller?.CoreWebView2;

    /// <summary>
    /// The constant string representing the about:blank URL.
    /// </summary>
    private const string ABOUT_BLANK = "about:blank";

    /// <summary>
    /// Stores a deferred URL to navigate to after initialization.
    /// </summary>
    private string? _defferedUrl;

    /// <summary>
    /// Holds the current WebView2 controller instance.
    /// </summary>
    private CoreWebView2Controller? _controller;

    /// <summary>
    /// Gets the web resource manager responsible for handling custom resource requests.
    /// </summary>
    private WebResourceManager WebResourceManager { get; } = new WebResourceManager();

    /// <summary>
    /// Indicates whether the browser is currently in fullscreen mode.
    /// </summary>
    private bool _fullscreen;

    /// <summary>
    /// Indicates whether fullscreen mode is required after initialization.
    /// </summary>
    private bool _isFullscrrenRequired;

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
    /// Holds the current fullscreen window instance, if any.
    /// </summary>
    FullscreenWindow? _fullscreenWindow = null;

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
    /// Asynchronously creates and initializes the WebView2 controller and browser instance.
    /// Configures settings, event handlers, and resource management.
    /// </summary>
    private async void CreateWebView2()
    {
        var opts = WebViewEnvironment.CreateCoreWebView2ControllerOptions();

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
            
            Controller.Bounds = new System.Drawing.Rectangle(0,0, Container.Width, Container.Height);
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
}
