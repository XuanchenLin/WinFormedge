// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormedge;

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Forms;

using WinFormedge.HostForms;
using WinFormedge.WebResource;

public abstract partial class Formedge
{
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

    public void ClearVirtualHostNameToEmbeddedResourcesMapping(EmbeddedFileResourceOptions options)
    {
        UnregisterWebResourceHandler(new EmbeddedFileResourceHandler(options));
    }

    public void ClearVirtualHostNameToFolderMapping(string hostName)
    {
        if (CoreWebView2 != null)
        {
            CoreWebView2.ClearVirtualHostNameToFolderMapping(hostName);
        }
    }

    public void RegisterWebResourceHander(WebResourceHandler resourceHandler)
    {
        WebView.RegisterWebResourceHander(resourceHandler);
    }

    public void SetVirtualHostNameToEmbeddedResourcesMapping(EmbeddedFileResourceOptions options)
    {
        RegisterWebResourceHander(new EmbeddedFileResourceHandler(options));
    }

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

    public void UnregisterWebResourceHandler(WebResourceHandler resourceHandler)
    {
        WebView.UnregisterWebResourceHander(resourceHandler);
    }

    internal Form HostWindow { get; }
    internal WebViewCore WebView { get; }
    internal protected bool HasSystemTitlebar => _windowStyleSettings.HasSystemTitlebar;
    internal protected bool AllowFullscreen { get; set; }
    internal protected virtual WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        return opts.UseDefaultWindow();
    }

    protected virtual void OnContextMenuRequested(object? sender, CoreWebView2ContextMenuRequestedEventArgs e)
    {
    }


    protected virtual void OnActivated(object? sender, EventArgs e)
    {
        Activated?.Invoke(this, EventArgs.Empty);
    }


    protected virtual void OnDeactivate(object? sender, EventArgs e)
    {
        Deactivate?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnFormClosed(object? sender, FormClosedEventArgs e)
    {
        FormClosed?.Invoke(this, e);
    }

    protected virtual void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        FormClosing?.Invoke(this, e);
    }

    protected virtual void OnMove(object? sender, EventArgs e)
    {
        Move?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnResize(object? sender, EventArgs e)
    {
        Resize?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnResizeBegin(object? sender, EventArgs e)
    {
        ResizeBegin?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnResizeEnd(object? sender, EventArgs e)
    {
        ResizeEnd?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnShown(object? sender, EventArgs e)
    {
        Shown?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnVisibleChanged(object? sender, EventArgs e)
    {
        VisibleChanged?.Invoke(this, e);
    }

    private readonly string FORMEDGE_MESSAGE_PASSCODE = Guid.NewGuid().ToString("N");
    //internal bool AllowSystemMenuOnNonClientRegion => _windowStyleSettings.AllowSystemMenuOnNonClientRegion;

    private readonly WindowSettings _windowStyleSettings;
    private readonly HostWindowBuilder _hostWindowBuilder;
    private FormedgeHostObject? _formedgeHostObject = null;
    string? _currentWindowStateString = null;

    bool _currentWindowActivated = true;

    private Action? _setVirtualHostNameToFolderMapping;

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

    private void OnFormClosingCore(object? sender, FormClosingEventArgs e)
    {
        OnFormClosing(this, e);

        if (!e.Cancel)
        {
            WebView.Close();
        }
    }

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

        }

        OnWebMessageReceived(sender, e);
    }

    private void HandleJSWIndowResizeBy(JsonElement jsonElement)
    {
        if (!jsonElement.TryGetProperty("dx", out var elX) || !jsonElement.TryGetProperty("dy", out var elY)) return;

        var dx = elX.GetInt32();
        var dy = elY.GetInt32();

        Size = new Size(Width + dx, Height + dy);
    }

    private void HandleJSWIndowResizeTo(JsonElement jsonElement)
    {
        if (!jsonElement.TryGetProperty("width", out var elX) || !jsonElement.TryGetProperty("height", out var elY)) return;

        var dx = elX.GetInt32();
        var dy = elY.GetInt32();

        Size = new Size(dx, dy);
    }

    private void HandleJSWindowMoveBy(JsonElement jsonElement)
    {
        if (!jsonElement.TryGetProperty("dx", out var elX) || !jsonElement.TryGetProperty("dy", out var elY)) return;

        var dx = elX.GetInt32();
        var dy = elY.GetInt32();

        Location = new Point(Left + dx, Top + dy);
    }

    private void HandleJSWindowMoveTo(JsonElement jsonElement)
    {
        if (!jsonElement.TryGetProperty("x", out var elX) || !jsonElement.TryGetProperty("y", out var elY)) return;

        var x = elX.GetInt32();
        var y = elY.GetInt32();

        Location = new Point(x, y);
    }

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
    private bool WndProcCore(ref Message m)
    {
        return WndProc(ref m);
    }

    private bool DefWndProcCore(ref Message m)
    {
        return DefWndProc(ref m);


    }


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
                // 避免连续添加多个分隔符
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

    private void OnDocumentTitleChangedCore(object? sender, object e)
    {
        UpdateWindowCaption();
    }

    private void OnStatusBarTextChangedCore(object? sender, object e)
    {
    }


}