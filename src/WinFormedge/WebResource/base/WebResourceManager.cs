// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge.WebResource;

/// <summary>
/// Manages web resource handlers and integrates with CoreWebView2 to handle web resource requests.
/// </summary>
internal sealed class WebResourceManager
{
    /// <summary>
    /// Gets the list of registered web resource handlers.
    /// </summary>
    public List<WebResourceHandler> Handlers { get; } = new();

    private CoreWebView2? _webView2 = null;

    private bool _initialized = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebResourceManager"/> class.
    /// </summary>
    internal WebResourceManager()
    {
    }

    /// <summary>
    /// Initializes the manager with the specified <see cref="CoreWebView2"/> instance and registers all handlers.
    /// </summary>
    /// <param name="coreWebView2">The CoreWebView2 instance to initialize with.</param>
    public void Initialize(CoreWebView2 coreWebView2)
    {
        _webView2 = coreWebView2;

        foreach (var handler in Handlers)
        {
            AddResourceRequestedFilter(coreWebView2, handler);
        }

        _initialized = true;

        coreWebView2.WebResourceRequested += CoreWebView2WebResourceRequested;
    }

    /// <summary>
    /// Handles the <see cref="CoreWebView2.WebResourceRequested"/> event and dispatches the request to the appropriate handler.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments containing the web resource request.</param>
    private void CoreWebView2WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        var uri = new Uri(e.Request.Uri);
        var webview = _webView2;

        //CoreWebView2WebResourceResponse GetNotFoundResponse() => webview.Environment.CreateWebResourceResponse(null, StatusCodes.Status404NotFound, "Not Found", "");

        if (webview == null)
        {
            return;
        }

        if (uri == null)
        {
            //e.Response = GetNotFoundResponse();
            return;
        }

        var matchedHandlers = Handlers.Where(x => x.WebResourceContext == CoreWebView2WebResourceContext.All || x.WebResourceContext == e.ResourceContext);

        matchedHandlers = matchedHandlers.Where(x => x.Uri.Scheme.Equals(uri.Scheme, StringComparison.InvariantCultureIgnoreCase));

        matchedHandlers = matchedHandlers.Where(x => x.Uri.Host.Equals(uri.Host, StringComparison.InvariantCultureIgnoreCase));

        matchedHandlers = matchedHandlers.Where(x => uri.AbsolutePath.StartsWith(x.Uri.AbsolutePath));

        var targetHandler = matchedHandlers.OrderBy(x => x.Uri.AbsolutePath.Length).FirstOrDefault();

        if (targetHandler == null)
        {
            //e.Response = GetNotFoundResponse();
            return;
        }

        targetHandler.HandleRequest(webview, e);
    }

    /// <summary>
    /// Adds a web resource requested filter to the specified <see cref="CoreWebView2"/> instance for the given handler.
    /// </summary>
    /// <param name="coreWebView2">The CoreWebView2 instance.</param>
    /// <param name="handler">The web resource handler.</param>
    private static void AddResourceRequestedFilter(CoreWebView2 coreWebView2, WebResourceHandler handler)
    {
        var scheme = handler.Scheme.ToLower();
        var hostName = handler.HostName.ToLower();

        var url = GetFilterUrl(scheme, hostName);

        coreWebView2.AddWebResourceRequestedFilter(url + "*", handler.WebResourceContext);
    }

    /// <summary>
    /// Constructs a filter URL from the specified scheme and host name.
    /// </summary>
    /// <param name="scheme">The URI scheme.</param>
    /// <param name="hostName">The host name.</param>
    /// <returns>The constructed filter URL.</returns>
    private static string GetFilterUrl(string scheme, string hostName)
    {
        var url = $"{scheme}://{hostName}";

        if (url.Last() != '/') url += '/';

        url = url.ToLower();
        return url;
    }

    /// <summary>
    /// Registers a new web resource handler and adds its filter if already initialized.
    /// </summary>
    /// <param name="handler">The web resource handler to register.</param>
    /// <exception cref="InvalidOperationException">Thrown if the handler is already registered.</exception>
    public void RegisterWebResourceHander(WebResourceHandler handler)
    {
        var scheme = handler.Scheme.ToLower();
        var hostName = handler.HostName.ToLower();

        var context = handler.WebResourceContext;

        if (Handlers.Contains(handler))
        {
            throw new InvalidOperationException("Handler is existed");
        }

        Handlers.Add(handler);

        if (_initialized)
        {
            AddResourceRequestedFilter(_webView2!, handler);
        }
    }

    /// <summary>
    /// Unregisters a web resource handler and removes its filter if already initialized.
    /// </summary>
    /// <param name="scheme">The URI scheme of the handler to unregister.</param>
    /// <param name="hostName">The host name of the handler to unregister.</param>
    public void UnregisterWebResourceHander(string scheme, string hostName)
    {
        var handler = Handlers.Find(x=>x.Scheme == scheme && x.HostName == hostName);

        if (handler != null)
        {
            Handlers.Remove(handler);
        }

        if (_initialized)
        {
            var url = GetFilterUrl(scheme, hostName);
            _webView2!.RemoveWebResourceRequestedFilter(url + "*", CoreWebView2WebResourceContext.All);
        }
    }
}