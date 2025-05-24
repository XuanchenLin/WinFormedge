// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using WinFormedge.WebResource;

namespace WinFormedge;
partial class WebViewCore
{
    public event EventHandler? WebViewCreated;

    public event Action<CoreWebView2Settings>? ConfigureSettings;

    public bool Initialized => _controller != null;
    
    public void RegisterWebResourceHander(WebResourceHandler resourceHandler)
    {
        WebResourceManager.RegisterWebResourceHander(resourceHandler);
    }

    public void UnregisterWebResourceHander(WebResourceHandler resourceHandler)
    {
        WebResourceManager.UnregisterWebResourceHander(resourceHandler);
    }

    internal CoreWebView2Environment WebViewEnvironment => WinFormedgeApp.Current!.WebView2Environment;
    
    internal CoreWebView2Controller Controller => _controller ?? throw new NullReferenceException(nameof(Controller));

    internal CoreWebView2? Browser => _controller?.CoreWebView2;
    
    private const string ABOUT_BLANK = "about:blank";

    private string? _defferedUrl;

    private CoreWebView2Controller? _controller;
    
    private WebResourceManager WebResourceManager { get; } = new WebResourceManager();

    private bool _fullscreen;
    private bool _isFullscrrenRequired;

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

    FullscreenWindow? _fullscreenWindow = null;

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
            if(_fullscreenWindow is null)
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
            Controller.Bounds = Container.ClientRectangle;
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
            _isFullscrrenRequired=false;
        }
    }
}