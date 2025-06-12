using System.Reflection;
using System.Text.RegularExpressions;

using WinFormedge.WebResource;

namespace WinFormedge.Blazor;

class FormedgeWebViewManager : WebViewManager
{


    public FormedgeWebViewManager(IServiceProvider serviceProvider, Formedge formedge, Uri appBaseUri, /*string contentRoot,*/ Assembly assembly, string relativePath, Type rootComponent, string selector = "#app", IDictionary<string, object?>? parameterView = null) : base(serviceProvider, Dispatcher.CreateDefault(), appBaseUri, new EmbeddedFileProvider(assembly), new JSComponentConfigurationStore(), relativePath)
    {
        Formedge = formedge;
        AppBaseUri = appBaseUri;
        //ContentRoot = contentRoot;
        RelativePath = relativePath;
        Dispatcher.InvokeAsync(async () =>
        {
            await AddRootComponentAsync(rootComponent, selector, parameterView is null ? ParameterView.Empty : ParameterView.FromDictionary(parameterView));
        });

        Formedge.Load += Formedge_Load;


        Navigate("/");

    }

    private void Formedge_Load(object? sender, EventArgs e)
    {
        if (Formedge.CoreWebView2 is not null)
        {
            var webview = Formedge.CoreWebView2;

            webview.AddScriptToExecuteOnDocumentCreatedAsync(""""
window.__receiveMessageCallbacks = [];

window.__dispatchMessageCallback = function(message) {
	window.__receiveMessageCallbacks.forEach(function(callback) { callback(message); });
};

window.external = {
	sendMessage: function(message) {
		window.chrome.webview.postMessage(message);
	},
	receiveMessage: function(callback) {
		window.__receiveMessageCallbacks.push(callback);
	}
};
"""").ConfigureAwait(continueOnCapturedContext: true);

            webview.WebMessageReceived += (s, args) =>
            {
                var message = args.TryGetWebMessageAsString();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageReceived(new Uri(args.Source), message);
                }
            };
        }
    }

    public Formedge Formedge { get; }
    public Uri AppBaseUri { get; }
    //public string ContentRoot { get; }
    public string RelativePath { get; }

    public bool TryGetResponseContent(string uri, out int statusCode, out string statusMessage, out Stream content, out IDictionary<string, string> headers)
    {
        return TryGetResponseContent(uri, false, out statusCode, out statusMessage, out content, out headers);
    }

    protected override void NavigateCore(Uri absoluteUri)
    {
        Formedge.CoreWebView2?.Navigate(absoluteUri.ToString());
    }

    protected override void SendMessage(string message)
    {
        Formedge.InvokeIfRequired(() =>
        {
            if (Formedge.CoreWebView2 is not null)
            {
                var script = $"__dispatchMessageCallback(\"{HttpUtility.JavaScriptStringEncode(message)}\")";
                Formedge.CoreWebView2.ExecuteScriptAsync(script);
            }
        });



    }
}

class BlazorResourceHandler : WebResourceHandler
{
    public override string Scheme { get; }
    public override string HostName { get; }
    public override CoreWebView2WebResourceContext WebResourceContext { get; }
    public ServiceProvider Services { get; }
    public FormedgeWebViewManager FormedgeWebViewManager { get; }
    public BlazorWebViewOptions Options { get; }

    public BlazorResourceHandler(BlazorWebViewOptions options, Formedge formedge)
    {
        Options = options;

        Scheme = options.Scheme;
        HostName = options.HostName;
        WebResourceContext = CoreWebView2WebResourceContext.All;

        Services = new ServiceCollection()
            .AddBlazorWebView()
            .BuildServiceProvider();

        var assmbly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Entry assembly is not available.");

        FormedgeWebViewManager = new FormedgeWebViewManager(Services, formedge, new Uri($"{Scheme}://{HostName}"), assmbly, options.RelativeHostPath, options.RootComponent, options.Selector, options.Parameters);
    }

    protected override WebResourceResponse GetResourceResponse(WebResourceRequest request)
    {
        var url = request.RequestUrl;

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

                ContentBody = content,
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

public sealed class BlazorWebViewOptions : WebResourceOptions
{
    public required Type RootComponent { get; init; }
    public string HostPath { get; init; } = Path.Combine("wwwroot", "index.html");
    public string ContentRoot => Path.GetDirectoryName(Path.GetFullPath(HostPath))!;
    public string RelativeHostPath => Path.GetRelativePath(ContentRoot, HostPath);

    public string Selector { get; init; } = "#app";

    public Dictionary<string, object?>? Parameters { get; init; } = null;
}

public static class BlazorWebViewExtensions
{
    public static void SetVirtualHostNameToBlazorHybrid(this Formedge formedge, BlazorWebViewOptions options)
    {
        formedge.RegisterWebResourceHander(new BlazorResourceHandler(options, formedge));
    }

    public static void ClearVirtualHostNameToBlazorHybrid(this Formedge formedge, BlazorWebViewOptions options)
    {
        formedge.UnregisterWebResourceHandler(options.Scheme, options.HostName);
    }
}