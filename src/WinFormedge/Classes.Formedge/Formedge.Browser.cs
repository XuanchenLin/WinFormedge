// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinFormedge;
public partial class Formedge
{
    public bool AllowExternalDrop
    {
        get => WebView.Controller.AllowExternalDrop;
        set => WebView.Controller.AllowExternalDrop = value;
    }
    public bool CanGoBack => WebView.Browser?.CanGoBack ?? false;
    public bool CanGoForward => WebView.Browser?.CanGoForward ?? false;

    public string DocumentTitle => WebView.Browser?.DocumentTitle ?? string.Empty;

    internal protected CoreWebView2? CoreWebView2 => WebView.Browser;

    public string Url
    {
        get => WebView.Url;
        set => WebView.Url = value;
    }

    public bool AllowDeveloperTools { get; set; } = true;


    private Color _defaultBackgroundColor = WinFormedgeApp.Current.IsDarkMode ? Color.DimGray : Color.White;
    internal protected Color BackColor
    {
        get => _defaultBackgroundColor;
        set {
            if (WebView.Initialized)
            {
                _defaultBackgroundColor = WebView.Controller.DefaultBackgroundColor = value;
            }
            else
            {
                _defaultBackgroundColor = value;
            }

            var colorWithoutAlpha = Color.FromArgb(255, _defaultBackgroundColor.R, _defaultBackgroundColor.G, _defaultBackgroundColor.B);
            HostWindow.BackColor = colorWithoutAlpha;
        }
    }

    internal Color SolidBackColor => Color.FromArgb(255, BackColor.R, BackColor.G, BackColor.B);

    /// <inheritdoc />
    internal protected double ZoomFactor
    {
        get => WebView.Controller.ZoomFactor;
        set => WebView.Controller.ZoomFactor = value;
    }

    /// <inheritdoc />
    public Task<string> ExecuteScriptAsync(string script)
    {
        return CoreWebView2?.ExecuteScriptAsync(script) ?? Task.FromResult<string>(string.Empty);
    }
    /// <inheritdoc />
    public void GoBack()
    {
        WebView.Browser?.GoBack();
    }
    /// <inheritdoc />
    public void GoForward() 
    {
        WebView?.Browser?.GoForward();
    }
    /// <inheritdoc />
    public void NavigateToString(string htmlContent)
    {
        WebView.Browser?.NavigateToString(htmlContent);
    }
    /// <inheritdoc />
    public void Reload()
    {
        WebView.Browser?.Reload();
    }
    /// <inheritdoc />
    public void Stop()
    {
        WebView.Browser?.Stop();
    }


    protected virtual void ConfigureWebView2Settings(CoreWebView2Settings settings)
    {

    }

    protected virtual void OnContentLoading(object? sender, CoreWebView2ContentLoadingEventArgs e)
    {
        ContentLoading?.Invoke(this, e);
    }

    protected virtual void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        NavigationStarting?.Invoke(this, e);
    }
    protected virtual void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        NavigationCompleted?.Invoke(this, e);
    }

    protected virtual void OnDOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
    {
        DOMContentLoaded?.Invoke(this, e);
    }

    protected virtual void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        WebMessageReceived?.Invoke(this, e);
    }
    protected virtual void OnGotFocus(object? sender, object e)
    {
        GotFocus?.Invoke(this, EventArgs.Empty);
    }
    protected virtual void OnLostFocus(object? sender, object e)
    {
        LostFocus?.Invoke(this, EventArgs.Empty);
    }


    public event EventHandler<CoreWebView2NavigationStartingEventArgs>? NavigationStarting;
    public event EventHandler<CoreWebView2NavigationCompletedEventArgs>? NavigationCompleted;

    public event EventHandler<CoreWebView2ContentLoadingEventArgs>? ContentLoading;
    public event EventHandler<CoreWebView2DOMContentLoadedEventArgs>? DOMContentLoaded;

    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;


    
    
}
