using System.Reflection;
using System.Security.Policy;
using System.Text.RegularExpressions;

using WinFormedge.WebResource;

namespace WinFormedge.Blazor;

class FormedgeWebViewManager : WebViewManager
{
    private readonly Uri _appBaseUri;
    private readonly string _contentRootRelativePath;
    private readonly string _hostPagePathWithinFileProvider;
    private readonly Formedge _formedge;
    private readonly BlazorHybridOptions _options;

    public FormedgeWebViewManager(Formedge formedge, IServiceProvider services,  Uri appBaseUri, IFileProvider fileProvider, JSComponentConfigurationStore jSComponent, string contentRootRelativePath, string hostPagePathWithinFileProvider, BlazorHybridOptions options) : base(services, Dispatcher.CreateDefault(), appBaseUri, fileProvider, jSComponent, hostPagePathWithinFileProvider)
    {
        _formedge = formedge;
        _appBaseUri = appBaseUri;
        _contentRootRelativePath = contentRootRelativePath;
        _hostPagePathWithinFileProvider = hostPagePathWithinFileProvider;
        _options = options;

        //Dispatcher.InvokeAsync(async () =>
        //{
        //    await AddRootComponentAsync(options.RootComponent, options.Selector, options.Parameters is null ? ParameterView.Empty : ParameterView.FromDictionary(options.Parameters));
        //});

        if (_formedge.CoreWebView2 is null)
        {
            _formedge.Load += (_, _) =>
            {
                InitScript(_formedge.CoreWebView2!);
            };
        }
        else
        {
            InitScript(_formedge.CoreWebView2);
        }




        Navigate("/");

    }

    public bool TryGetResponseContent(WebResourceRequest reqeust, out int statusCode, out string statusMessage, out Stream content, out IDictionary<string, string> headers)
    {
        var url = reqeust.Uri.ToString();
        var allowFallbackOnHostPage =
                reqeust.WebResourceContext == CoreWebView2WebResourceContext.Document ||
                reqeust.WebResourceContext == CoreWebView2WebResourceContext.Other || !reqeust.HasFileName;


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
            url += _hostPagePathWithinFileProvider;
        }






        return TryGetResponseContent(url, allowFallbackOnHostPage, out statusCode, out statusMessage, out content, out headers);
    }

    protected override void NavigateCore(Uri absoluteUri)
    {
        _formedge.CoreWebView2?.Navigate(absoluteUri.ToString());
    }

    protected override void SendMessage(string message)
    {
        _formedge.InvokeIfRequired(() =>
        {
            if (_formedge.CoreWebView2 is not null)
            {
                _formedge.CoreWebView2.PostWebMessageAsString(message);
            }
        });



    }

    private void InitScript(CoreWebView2 webview)
    {
        webview.AddScriptToExecuteOnDocumentCreatedAsync(""""
window.external = {
	sendMessage: message => {
		window.chrome.webview.postMessage(message);
	},
	receiveMessage: callback => {
		window.chrome.webview.addEventListener('message', e => {
            const { data } = e;
            if (typeof data === 'string') {
                callback(data);
            }
        });
	}
};
"""").ConfigureAwait(continueOnCapturedContext: true);

        webview.WebMessageReceived += (s, args) =>
        {
            MessageReceived(new Uri(args.Source), args.TryGetWebMessageAsString());
        };


    }
}
