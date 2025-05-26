// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;
/// <summary>
/// Provides browser-related properties, methods, and events for the Formedge control.
/// </summary>
public partial class Formedge
{
    /// <summary>
    /// Gets or sets a value indicating whether external drag-and-drop is allowed in the browser.
    /// </summary>
    public bool AllowExternalDrop
    {
        get => WebView.Controller.AllowExternalDrop;
        set => WebView.Controller.AllowExternalDrop = value;
    }

    /// <summary>
    /// Gets a value indicating whether the browser can navigate back in its history.
    /// </summary>
    public bool CanGoBack => WebView.Browser?.CanGoBack ?? false;

    /// <summary>
    /// Gets a value indicating whether the browser can navigate forward in its history.
    /// </summary>
    public bool CanGoForward => WebView.Browser?.CanGoForward ?? false;

    /// <summary>
    /// Gets the title of the current document.
    /// </summary>
    public string DocumentTitle => WebView.Browser?.DocumentTitle ?? string.Empty;

    /// <summary>
    /// Gets the underlying CoreWebView2 instance, if available.
    /// </summary>
    internal protected CoreWebView2? CoreWebView2 => WebView.Browser;

    /// <summary>
    /// Gets or sets the current URL of the browser.
    /// </summary>
    public string Url
    {
        get => WebView.Url;
        set => WebView.Url = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether developer tools are allowed.
    /// </summary>
    public bool AllowDeveloperTools { get; set; } = true;

    private Color _defaultBackgroundColor = WinFormedgeApp.Current.IsDarkMode ? Color.DimGray : Color.White;

    /// <summary>
    /// Gets or sets the background color of the browser.
    /// </summary>
    internal protected Color BackColor
    {
        get => _defaultBackgroundColor;
        set
        {
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

    /// <summary>
    /// Gets the background color with full opacity (alpha = 255).
    /// </summary>
    internal Color SolidBackColor => Color.FromArgb(255, BackColor.R, BackColor.G, BackColor.B);

    /// <summary>
    /// Gets or sets the zoom factor of the browser.
    /// </summary>
    internal protected double ZoomFactor
    {
        get => WebView.Controller.ZoomFactor;
        set => WebView.Controller.ZoomFactor = value;
    }

    /// <summary>
    /// Executes the specified JavaScript code asynchronously in the context of the current page.
    /// </summary>
    /// <param name="script">The JavaScript code to execute.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the script result as a JSON-encoded string.</returns>
    public Task<string> ExecuteScriptAsync(string script)
    {
        return CoreWebView2?.ExecuteScriptAsync(script) ?? Task.FromResult<string>(string.Empty);
    }

    /// <summary>
    /// Navigates the browser back to the previous page in the history, if possible.
    /// </summary>
    public void GoBack()
    {
        WebView.Browser?.GoBack();
    }

    /// <summary>
    /// Navigates the browser forward to the next page in the history, if possible.
    /// </summary>
    public void GoForward()
    {
        WebView?.Browser?.GoForward();
    }

    /// <summary>
    /// Navigates the browser to the specified HTML content as a string.
    /// </summary>
    /// <param name="htmlContent">The HTML content to display.</param>
    public void NavigateToString(string htmlContent)
    {
        WebView.Browser?.NavigateToString(htmlContent);
    }

    /// <summary>
    /// Reloads the current page in the browser.
    /// </summary>
    public void Reload()
    {
        WebView.Browser?.Reload();
    }

    /// <summary>
    /// Stops the current navigation or page loading in the browser.
    /// </summary>
    public void Stop()
    {
        WebView.Browser?.Stop();
    }

    /// <summary>
    /// Configures the WebView2 settings. Can be overridden in derived classes.
    /// </summary>
    /// <param name="settings">The WebView2 settings to configure.</param>
    protected virtual void ConfigureWebView2Settings(CoreWebView2Settings settings)
    {

    }

    /// <summary>
    /// Raises the <see cref="ContentLoading"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnContentLoading(object? sender, CoreWebView2ContentLoadingEventArgs e)
    {
        ContentLoading?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="NavigationStarting"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        NavigationStarting?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="NavigationCompleted"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        NavigationCompleted?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="DOMContentLoaded"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnDOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
    {
        DOMContentLoaded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="WebMessageReceived"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        WebMessageReceived?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="GotFocus"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnGotFocus(object? sender, object e)
    {
        GotFocus?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Raises the <see cref="LostFocus"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnLostFocus(object? sender, object e)
    {
        LostFocus?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Occurs when a navigation is starting in the browser.
    /// </summary>
    public event EventHandler<CoreWebView2NavigationStartingEventArgs>? NavigationStarting;

    /// <summary>
    /// Occurs when a navigation is completed in the browser.
    /// </summary>
    public event EventHandler<CoreWebView2NavigationCompletedEventArgs>? NavigationCompleted;

    /// <summary>
    /// Occurs when content is loading in the browser.
    /// </summary>
    public event EventHandler<CoreWebView2ContentLoadingEventArgs>? ContentLoading;

    /// <summary>
    /// Occurs when the DOM content is loaded in the browser.
    /// </summary>
    public event EventHandler<CoreWebView2DOMContentLoadedEventArgs>? DOMContentLoaded;

    /// <summary>
    /// Occurs when a web message is received from the browser.
    /// </summary>
    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;



}
