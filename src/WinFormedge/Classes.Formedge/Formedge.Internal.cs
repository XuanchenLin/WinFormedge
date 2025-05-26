// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;
/// <summary>
/// Provides the core implementation for the Formedge window, including window creation, event handling, 
/// and integration with WebView2 for browser-based UI.
/// </summary>
public abstract partial class Formedge
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Formedge"/> class, sets up the host window, 
    /// configures window settings, and initializes the WebView2 browser.
    /// </summary>
    public Formedge()
    {
        _hostWindowBuilder = new HostWindowBuilder();
        _windowStyleSettings = ConfigureWindowSettings(_hostWindowBuilder);

        var hostWindow = _windowStyleSettings.CreateHostWindow();

        if (hostWindow is null) throw new InvalidOperationException("HostWindow can't be null.");

        _windowStyleSettings.ConfigureWinFormProps(hostWindow);

        HostWindow = hostWindow;

        WebView = new WebViewCore(HostWindow);

        WebView.WebViewCreated += (_, _) => WebViewCreatedCore();

        _windowStyleSettings.WndProc += WebView.HostWndProc;
        _windowStyleSettings.WndProc += WndProcCore;
        _windowStyleSettings.DefWndProc += DefWndProcCore;

        HostWindowCreatedCore();
    }

    /// <summary>
    /// Removes the mapping between a virtual host name and embedded resources.
    /// </summary>
    /// <param name="options">The embedded file resource options to clear.</param>
    public void ClearVirtualHostNameToEmbeddedResourcesMapping(EmbeddedFileResourceOptions options)
    {
        UnregisterWebResourceHandler(new EmbeddedFileResourceHandler(options));
    }

    /// <summary>
    /// Removes the mapping between a virtual host name and a local folder.
    /// </summary>
    /// <param name="hostName">The virtual host name to clear.</param>
    public void ClearVirtualHostNameToFolderMapping(string hostName)
    {
        if (CoreWebView2 != null)
        {
            CoreWebView2.ClearVirtualHostNameToFolderMapping(hostName);
        }
    }

    /// <summary>
    /// Registers a custom web resource handler for the WebView2 instance.
    /// </summary>
    /// <param name="resourceHandler">The web resource handler to register.</param>
    public void RegisterWebResourceHander(WebResourceHandler resourceHandler)
    {
        WebView.RegisterWebResourceHander(resourceHandler);
    }

    /// <summary>
    /// Maps a virtual host name to embedded resources using the specified options.
    /// </summary>
    /// <param name="options">The embedded file resource options to use for mapping.</param>
    public void SetVirtualHostNameToEmbeddedResourcesMapping(EmbeddedFileResourceOptions options)
    {
        RegisterWebResourceHander(new EmbeddedFileResourceHandler(options));
    }

    /// <summary>
    /// Maps a virtual host name to a local folder with the specified access kind.
    /// </summary>
    /// <param name="hostName">The virtual host name.</param>
    /// <param name="folderPath">The local folder path.</param>
    /// <param name="accessKind">The access kind for the resource.</param>
    public void SetVirtualHostNameToFolderMapping(string hostName, string folderPath, CoreWebView2HostResourceAccessKind accessKind)
    {
        if (CoreWebView2 != null)
        {
            CoreWebView2.SetVirtualHostNameToFolderMapping(hostName, folderPath, accessKind);
        }
        else
        {
            _setVirtualHostNameToFolderMapping += () => CoreWebView2!.SetVirtualHostNameToFolderMapping(hostName, folderPath, accessKind);
        }
    }

    /// <summary>
    /// Unregisters a previously registered web resource handler.
    /// </summary>
    /// <param name="resourceHandler">The web resource handler to unregister.</param>
    public void UnregisterWebResourceHandler(WebResourceHandler resourceHandler)
    {
        WebView.UnregisterWebResourceHander(resourceHandler);
    }

    /// <summary>
    /// Gets the host window (WinForms Form) for this Formedge instance.
    /// </summary>
    internal Form HostWindow { get; }

    /// <summary>
    /// Gets the WebViewCore instance used for browser integration.
    /// </summary>
    internal WebViewCore WebView { get; }

    /// <summary>
    /// Gets a value indicating whether the window has a system title bar.
    /// </summary>
    internal protected bool HasSystemTitlebar => _windowStyleSettings.HasSystemTitlebar;

    /// <summary>
    /// Gets or sets a value indicating whether fullscreen is allowed.
    /// </summary>
    internal protected bool AllowFullscreen { get; set; }

    /// <summary>
    /// Gets the activated state of the current window.
    /// </summary>
    internal protected bool IsActivated => _currentWindowActivated;

    /// <summary>
    /// Configures the window settings using the provided <see cref="HostWindowBuilder"/>.
    /// </summary>
    /// <param name="opts">The host window builder.</param>
    /// <returns>The configured <see cref="WindowSettings"/>.</returns>
    internal protected virtual WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        return opts.UseDefaultWindow();
    }


    /// <summary>
    /// Called when the WebView2 context menu is requested. Can be overridden to customize the context menu.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnContextMenuRequested(object? sender, CoreWebView2ContextMenuRequestedEventArgs e)
    {
    }

    /// <summary>
    /// Called when the window is activated.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnActivated(object? sender, EventArgs e)
    {
        Activated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the window is deactivated.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnDeactivate(object? sender, EventArgs e)
    {
        Deactivate?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the window is closed.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnFormClosed(object? sender, FormClosedEventArgs e)
    {
        FormClosed?.Invoke(this, e);
    }

    /// <summary>
    /// Called when the window is closing.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        FormClosing?.Invoke(this, e);
    }

    /// <summary>
    /// Called when the window is moved.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnMove(object? sender, EventArgs e)
    {
        Move?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the window is resized.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnResize(object? sender, EventArgs e)
    {
        Resize?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the window resize operation begins.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnResizeBegin(object? sender, EventArgs e)
    {
        ResizeBegin?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the window resize operation ends.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnResizeEnd(object? sender, EventArgs e)
    {
        ResizeEnd?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the window is shown.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnShown(object? sender, EventArgs e)
    {
        Shown?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the window's visibility changes.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnVisibleChanged(object? sender, EventArgs e)
    {
        VisibleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// The passcode used for secure message passing between JavaScript and the host.
    /// </summary>
    private readonly string FORMEDGE_MESSAGE_PASSCODE = Guid.NewGuid().ToString("N");

    private readonly WindowSettings _windowStyleSettings;
    private readonly HostWindowBuilder _hostWindowBuilder;
    private FormedgeHostObject? _formedgeHostObject = null;
    string? _currentWindowStateString = null;

    bool _currentWindowActivated = true;

    private Action? _setVirtualHostNameToFolderMapping;

    /// <summary>
    /// Attaches core event handlers to the host window after creation.
    /// </summary>
    private void HostWindowCreatedCore()
    {
        HostWindow.Activated += OnActivatedCore;
        HostWindow.Deactivate += OnDeactivateCore;
        HostWindow.ResizeBegin += OnResizeBegin;
        HostWindow.Resize += OnResizeCore;
        HostWindow.ResizeEnd += OnResizeEnd;
        HostWindow.VisibleChanged += OnVisibleChanged;
        HostWindow.Move += OnMoveCore;
        HostWindow.Shown += OnShown;
        HostWindow.FormClosing += OnFormClosingCore;
        HostWindow.FormClosed += OnFormClosed;
    }

    /// <summary>
    /// Handles the FormClosing event, invokes <see cref="OnFormClosing"/>, and closes the WebView if not cancelled.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnFormClosingCore(object? sender, FormClosingEventArgs e)
    {
        OnFormClosing(this, e);

        if (!e.Cancel)
        {
            WebView.Close();
        }
    }

    /// <summary>
    /// Called when the WebView2 browser is created. Sets up host objects, scripts, and event handlers.
    /// </summary>
    private void WebViewCreatedCore()
    {
        if (WebView.Browser == null) throw new InvalidOperationException();

        _formedgeHostObject = new FormedgeHostObject(this);

        var controller = WebView.Controller;
        var webview = WebView.Browser;

        controller.DefaultBackgroundColor = BackColor;

        WebView.ConfigureSettings += ConfigureWebView2Settings;

        controller.GotFocus += OnGotFocus;
        controller.LostFocus += OnLostFocus;

        webview.ContentLoading += OnContentLoading;
        webview.NavigationStarting += OnNavigationStarting;
        webview.NavigationCompleted += OnNavigationCompleted;
        webview.DOMContentLoaded += OnDOMContentLoaded;
        webview.WebMessageReceived += CoreWebView2WebMessageReceivedCore;

        webview.ContextMenuRequested += OnContextMenuRequestedCore;
        webview.DocumentTitleChanged += OnDocumentTitleChangedCore;
        webview.StatusBarTextChanged += OnStatusBarTextChangedCore;

        webview.Settings.IsNonClientRegionSupportEnabled = !HasSystemTitlebar && !Fullscreen;

        _setVirtualHostNameToFolderMapping?.Invoke();

        var script = Properties.Resources.Formedge;

        var version = typeof(Formedge).Assembly.GetName().Version?.ToString() ?? webview.Environment.BrowserVersionString;

        script = script.Replace("{{FORMEDGE_MESSAGE_PASSCODE}}", FORMEDGE_MESSAGE_PASSCODE);
        script = script.Replace("{{WINFORMEDGE_VERSION}}", version);
        script = script.Replace("{{HAS_TITLE_BAR}}", HasSystemTitlebar ? "true" : "false");

        webview.AddHostObjectToScript("hostWindow", _formedgeHostObject!);

        webview.AddScriptToExecuteOnDocumentCreatedAsync(script);

        if (_windowStyleSettings.WindowSpecifiedJavaScript is not null)
        {
            webview.AddScriptToExecuteOnDocumentCreatedAsync(_windowStyleSettings.WindowSpecifiedJavaScript);
        }

        OnLoad();
    }

    /// <summary>
    /// Handles messages received from JavaScript via WebView2, including window commands and movement/resize requests.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void CoreWebView2WebMessageReceivedCore(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            var jsdoc = JsonDocument.Parse(e.WebMessageAsJson);
            if (jsdoc == null || jsdoc.RootElement.ValueKind != JsonValueKind.Object)
            {
                OnWebMessageReceived(sender, e);
                return;
            }

            if (jsdoc.RootElement.TryGetProperty("passcode", out var elPasscode) && jsdoc.RootElement.TryGetProperty("message", out var elMessage))
            {
                var passcode = elPasscode.GetString();
                var name = elMessage.GetString();

                if (passcode != FORMEDGE_MESSAGE_PASSCODE) return;

                switch (name)
                {
                    case "FormedgeWindowCommand":
                        HandleJSWindowAppCommand(jsdoc.RootElement);
                        return;
                    case "FormedgeWindowMoveTo":
                        HandleJSWindowMoveTo(jsdoc.RootElement);
                        return;
                    case "FormedgeWindowMoveBy":
                        HandleJSWindowMoveBy(jsdoc.RootElement);
                        return;
                    case "FormedgeWindowResizeTo":
                        HandleJSWIndowResizeTo(jsdoc.RootElement);
                        return;
                    case "FormedgeWindowResizeBy":
                        HandleJSWIndowResizeBy(jsdoc.RootElement);
                        return;
                }
            }
        }
        catch
        {
            // Swallow exceptions to avoid breaking message handling.
        }

        OnWebMessageReceived(sender, e);
    }

    /// <summary>
    /// Handles a JavaScript request to resize the window by a delta.
    /// </summary>
    /// <param name="jsonElement">The JSON element containing dx and dy.</param>
    private void HandleJSWIndowResizeBy(JsonElement jsonElement)
    {
        if (!jsonElement.TryGetProperty("dx", out var elX) || !jsonElement.TryGetProperty("dy", out var elY)) return;

        var dx = elX.GetInt32();
        var dy = elY.GetInt32();

        Size = new Size(Width + dx, Height + dy);
    }

    /// <summary>
    /// Handles a JavaScript request to resize the window to a specific size.
    /// </summary>
    /// <param name="jsonElement">The JSON element containing width and height.</param>
    private void HandleJSWIndowResizeTo(JsonElement jsonElement)
    {
        if (!jsonElement.TryGetProperty("width", out var elX) || !jsonElement.TryGetProperty("height", out var elY)) return;

        var dx = elX.GetInt32();
        var dy = elY.GetInt32();

        Size = new Size(dx, dy);
    }

    /// <summary>
    /// Handles a JavaScript request to move the window by a delta.
    /// </summary>
    /// <param name="jsonElement">The JSON element containing dx and dy.</param>
    private void HandleJSWindowMoveBy(JsonElement jsonElement)
    {
        if (!jsonElement.TryGetProperty("dx", out var elX) || !jsonElement.TryGetProperty("dy", out var elY)) return;

        var dx = elX.GetInt32();
        var dy = elY.GetInt32();

        Location = new Point(Left + dx, Top + dy);
    }

    /// <summary>
    /// Handles a JavaScript request to move the window to a specific location.
    /// </summary>
    /// <param name="jsonElement">The JSON element containing x and y.</param>
    private void HandleJSWindowMoveTo(JsonElement jsonElement)
    {
        if (!jsonElement.TryGetProperty("x", out var elX) || !jsonElement.TryGetProperty("y", out var elY)) return;

        var x = elX.GetInt32();
        var y = elY.GetInt32();

        Location = new Point(x, y);
    }

    /// <summary>
    /// Handles a JavaScript window command such as minimize, maximize, fullscreen, or close.
    /// </summary>
    /// <param name="jsonElement">The JSON element containing the command.</param>
    private void HandleJSWindowAppCommand(JsonElement jsonElement)
    {
        if (!jsonElement.TryGetProperty("command", out var elCommand)) return;

        var command = elCommand.GetString();

        switch (command)
        {
            case "minimize":
                WindowState = FormWindowState.Minimized;
                break;
            case "maximize":
                if (WindowState == FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Normal;
                }
                else
                {
                    WindowState = FormWindowState.Maximized;
                }
                break;
            case "fullscreen":
                ToggleFullscreen();
                break;
            case "close":
                Close();
                break;
        }
    }

    //private bool _isSnapLayoutsRequired = false;

    //private void HandleWindowSnapLayoutsRequired(JsonElement jsonElement)
    //{
    //    if (!jsonElement.TryGetProperty("status", out var statusEl)) return;

    //    var status = statusEl.GetBoolean();

    //    _isSnapLayoutsRequired = status;

    //    if (status)
    //    {
    //        SendMessage((HWND)HostWindow.Handle, WM_NCMOUSEHOVER, (WPARAM)HTMAXBUTTON, MARCOS.FromPoint(Control.MousePosition));
    //    }
    //}

    /// <summary>
    /// Core window procedure handler for custom message processing.
    /// </summary>
    /// <param name="m">The Windows message.</param>
    /// <returns>True if the message was handled; otherwise, false.</returns>
    private bool WndProcCore(ref Message m)
    {
        return WndProc(ref m);
    }

    /// <summary>
    /// Default window procedure handler for message processing.
    /// </summary>
    /// <param name="m">The Windows message.</param>
    /// <returns>True if the message was handled; otherwise, false.</returns>
    private bool DefWndProcCore(ref Message m)
    {
        return DefWndProc(ref m);
    }

    /// <summary>
    /// Handles the Resize event, notifies JavaScript of window state and size changes.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnResizeCore(object? sender, EventArgs e)
    {
        if (Fullscreen) return;

        OnResize(this, e);

        if (WebView.Browser == null) return;

        var state = HostWindow.WindowState.ToString().ToLower();

        if (Fullscreen && _currentWindowStateString != $"{nameof(Fullscreen)}".ToLower())
        {
            state = $"{nameof(Fullscreen)}".ToLower();
        }

        if (state != _currentWindowStateString)
        {
            _currentWindowStateString = state;

            WebView.Browser.PostWebMessageAsJson(JsonSerializer.Serialize(new
            {
                passcode = FORMEDGE_MESSAGE_PASSCODE,
                message = "FormedgeNotifyWindowStateChange",
                state = _currentWindowStateString
            }));
        }

        WebView.Browser.PostWebMessageAsJson(JsonSerializer.Serialize(new
        {
            passcode = FORMEDGE_MESSAGE_PASSCODE,
            message = "FormedgeNotifyWindowResize",
            x = HostWindow.Left,
            y = HostWindow.Top,
            width = HostWindow.Width,
            height = HostWindow.Height
        }));
    }

    /// <summary>
    /// Handles the Move event, notifies JavaScript of window position changes.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnMoveCore(object? sender, EventArgs e)
    {
        if (Fullscreen) return;

        OnMove(this, e);

        if (WebView.Browser == null) return;

        var screen = Screen.FromHandle(Handle);

        var x = HostWindow.Left;
        var y = HostWindow.Top;
        var scrX = x - screen.Bounds.X;
        var scrY = y - screen.Bounds.Y;

        WebView.Browser.PostWebMessageAsJson(JsonSerializer.Serialize(new
        {
            passcode = FORMEDGE_MESSAGE_PASSCODE,
            message = "FormedgeNotifyWindowMove",
            x = HostWindow.Left,
            y = HostWindow.Top,
            screenX = scrX,
            screenY = scrY,
        }));
    }

    /// <summary>
    /// Handles the Activated event, notifies JavaScript, and sets focus to the WebView2 controller.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnActivatedCore(object? sender, EventArgs e)
    {
        _currentWindowActivated = true;

        if (WebView.Initialized)
        {
            WebView.Controller.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);
        }

        OnActivated(this, e);

        if (WebView.Browser == null) return;

        WebView.Browser.PostWebMessageAsJson(JsonSerializer.Serialize(new
        {
            passcode = FORMEDGE_MESSAGE_PASSCODE,
            message = "FormedgeNotifyWindowActivated",
            state = true
        }));
    }

    /// <summary>
    /// Handles the Deactivate event, notifies JavaScript.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnDeactivateCore(object? sender, EventArgs e)
    {
        _currentWindowActivated = false;
        OnDeactivate(this, e);

        if (WebView.Browser == null) return;

        WebView.Browser.PostWebMessageAsJson(JsonSerializer.Serialize(new
        {
            passcode = FORMEDGE_MESSAGE_PASSCODE,
            message = "FormedgeNotifyWindowActivated",
            state = false
        }));
    }

    /// <summary>
    /// Handles the WebView2 context menu request, filters menu items, and invokes <see cref="OnContextMenuRequested"/>.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnContextMenuRequestedCore(object? sender, CoreWebView2ContextMenuRequestedEventArgs e)
    {
        bool IsRequiredContextMenuItem(int commandId)
        {
            if (WinFormedgeApp.Current.EnableDevTools && commandId == 50162 && AllowDeveloperTools)
            {
                return true;
            }

            return commandId >= 50150 && commandId <= 50157 && commandId != 50154 && commandId != 50155;
        }

        var editingItems = new List<CoreWebView2ContextMenuItem>();

        for (int i = 0; i < e.MenuItems.Count; i++)
        {
            var item = e.MenuItems[i];

            if (IsRequiredContextMenuItem(item.CommandId) || item.Kind == CoreWebView2ContextMenuItemKind.Separator)
            {
                // Avoid adding consecutive separators
                if (item.Kind == CoreWebView2ContextMenuItemKind.Separator)
                {
                    if (editingItems.Count == 0 || editingItems[^1].Kind != CoreWebView2ContextMenuItemKind.Separator)
                    {
                        editingItems.Add(item);
                    }
                }
                else
                {
                    editingItems.Add(item);
                }
            }
        }

        if (editingItems.Count > 0 && editingItems[0].Kind == CoreWebView2ContextMenuItemKind.Separator)
        {
            editingItems.RemoveAt(0);
        }

        if (editingItems.Count > 0 && editingItems.LastOrDefault()?.Kind == CoreWebView2ContextMenuItemKind.Separator)
        {
            editingItems.RemoveAt(editingItems.Count - 1);
        }

        e.MenuItems.Clear();

        foreach (var item in editingItems)
        {
            e.MenuItems.Add(item);
        }

        OnContextMenuRequested(sender, e);
    }

    /// <summary>
    /// Handles the DocumentTitleChanged event and updates the window caption.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnDocumentTitleChangedCore(object? sender, object e)
    {
        UpdateWindowCaption();
    }

    /// <summary>
    /// Handles the StatusBarTextChanged event. (No implementation.)
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnStatusBarTextChangedCore(object? sender, object e)
    {
    }
}
