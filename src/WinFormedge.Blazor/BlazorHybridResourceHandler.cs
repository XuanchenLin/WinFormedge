using System.Reflection;

using WinFormedge.WebResource;

namespace WinFormedge.Blazor;

class BlazorHybridResourceHandler : WebResourceHandler
{
    public override string Scheme { get; }
    public override string HostName { get; }
    public override CoreWebView2WebResourceContext WebResourceContext { get; }
    public ServiceProvider Services { get; }
    public FormedgeWebViewManager FormedgeWebViewManager { get; }
    public BlazorHybridOptions Options { get; }

    public BlazorHybridResourceHandler(BlazorHybridOptions options, Formedge formedge)
    {
        Options = options;

        Scheme = options.Scheme;
        HostName = options.HostName;
        WebResourceContext = CoreWebView2WebResourceContext.All;

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddBlazorWebView();

        options.ConfigureServices?.Invoke(serviceCollection);

        Services = serviceCollection
            .BuildServiceProvider();

        var assmbly = options.ResourceAssembly ?? Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Entry assembly is not available.");

        FormedgeWebViewManager = new FormedgeWebViewManager(Services, formedge, new Uri($"{Scheme}://{HostName}"), assmbly, options.RelativeHostPath, options.RootComponent, options.Selector, options.Parameters);
    }

    protected override WebResourceResponse GetResourceResponse(WebResourceRequest request)
    {
        var url = request.RequestUrl;

        string RemovePossibleQueryString(string? url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            var indexOfQueryString = url.IndexOf('?', StringComparison.Ordinal);
            return (indexOfQueryString == -1)
                ? url
                : url.Substring(0, indexOfQueryString);
        }

        url = RemovePossibleQueryString(url);

        var uri = new Uri(url);

        if (uri.PathAndQuery == "/")
        {
            url += Options.RelativeHostPath;
        }

        if (FormedgeWebViewManager.TryGetResponseContent(url, out var statusCode, out var statusMessage, out var content, out var headers))
        {
            var response = new WebResourceResponse()
            {
                HttpStatus = statusCode,

                ContentBody = new AutoCloseStream(content),
                ContentType = headers.TryGetValue("Content-Type", out var contentType) ? contentType : "application/octet-stream"
            };


            foreach (var header in headers)
            {
                response.Headers[header.Key] = header.Value;
            }


            return response;
        }
        else
        {
            return new WebResourceResponse()
            {
                HttpStatus = statusCode,
            };
        }
    }
}
