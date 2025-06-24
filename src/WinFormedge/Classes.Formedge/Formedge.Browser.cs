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
    /// Occurs when an accelerator key is pressed while the WebView2 control has focus.
    /// </summary>
    /// <remarks>This event is triggered for key presses that are considered accelerator keys, such as
    /// function keys or key combinations involving modifiers (e.g., Ctrl, Alt). Use this event to handle custom key
    /// actions or override default behavior.</remarks>
    public EventHandler<CoreWebView2AcceleratorKeyPressedEventArgs>? AcceleratorKeyPressed;
    /// <summary>
    /// Occurs when the WebView2 control requests to move focus to another element.
    /// </summary>
    /// <remarks>This event is triggered when the WebView2 control determines that focus should be moved, 
    /// such as during keyboard navigation or accessibility interactions.  Use this event to handle focus movement
    /// within your application.</remarks>
    public EventHandler<CoreWebView2MoveFocusRequestedEventArgs>? MoveFocusRequested;

    /// <summary>
    /// Occurs when content is loading in the browser.
    /// </summary>
    public event EventHandler<CoreWebView2ContentLoadingEventArgs>? ContentLoading;

    /// <summary>
    /// Occurs when the DOM content is loaded in the browser.
    /// </summary>
    public event EventHandler<CoreWebView2DOMContentLoadedEventArgs>? DOMContentLoaded;

    /// <summary>
    /// Occurs when a navigation is completed in the browser.
    /// </summary>
    public event EventHandler<CoreWebView2NavigationCompletedEventArgs>? NavigationCompleted;

    /// <summary>
    /// Occurs when a navigation is starting in the browser.
    /// </summary>
    public event EventHandler<CoreWebView2NavigationStartingEventArgs>? NavigationStarting;

    /// <summary>
    /// Occurs when a web message is received from the browser.
    /// </summary>
    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;

    /// <summary>
    /// Gets or sets a value indicating whether developer tools are allowed.
    /// </summary>
    public bool AllowDeveloperTools { get; set; } = true;

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
    /// Gets or sets the current URL of the browser.
    /// </summary>
    public string Url
    {
        get => WebView.Url;
        set => WebView.Url = value;
    }

    /// <summary>
    /// Gets the background color with full opacity (alpha = 255).
    /// </summary>
    internal Color SolidBackColor => Color.FromArgb(255, BackColor.R, BackColor.G, BackColor.B);

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
    /// Gets the underlying CoreWebView2 instance, if available.
    /// </summary>
    internal protected CoreWebView2? CoreWebView2 => WebView.Browser;
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
    /// Raises the <see cref="DOMContentLoaded"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnDOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
    {
        DOMContentLoaded?.Invoke(this, e);
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
    /// Raises the <see cref="NavigationCompleted"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        NavigationCompleted?.Invoke(this, e);
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
    /// Raises the <see cref="WebMessageReceived"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        WebMessageReceived?.Invoke(this, e);
    }

    /// <summary>
    /// Moves the focus within the WebView2 control based on the specified reason.
    /// </summary>
    /// <remarks>Use this method to programmatically adjust focus within the WebView2 control. The <paramref
    /// name="reason"/> parameter specifies the focus movement behavior, such as moving focus to the next or previous
    /// element, or handling focus changes triggered by user interactions.</remarks>
    /// <param name="reason">The reason for moving focus, represented by a <see
    /// cref="Microsoft.Web.WebView2.Core.CoreWebView2MoveFocusReason"/> value. This determines the direction or context
    /// in which the focus should be moved.</param>
    public void MoveFocus(Microsoft.Web.WebView2.Core.CoreWebView2MoveFocusReason reason)
    {
        WebView.Controller.MoveFocus(reason);
    }

    /// <summary>
    /// Notifies the parent window that its position has changed.
    /// </summary>
    /// <remarks>This method should be called whenever the position of the parent window changes. It ensures
    /// that the WebView's controller is updated to reflect the new position.</remarks>
    public void NotifyParentWindowPositionChanged()
    {
        WebView.Controller.NotifyParentWindowPositionChanged();
    }
}
