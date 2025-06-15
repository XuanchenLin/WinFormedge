using System.Reflection;
using System.Text.RegularExpressions;

using WinFormedge.WebResource;

namespace WinFormedge.Blazor;

class FormedgeWebViewManager : WebViewManager
{
    private readonly Uri _appBaseUri;
    private readonly string _contentRootRelativePath;
    private readonly Formedge _formedge;
    private readonly BlazorHybridOptions _options;

    public FormedgeWebViewManager(Formedge formedge, IServiceProvider services,  Uri appBaseUri, IFileProvider fileProvider, JSComponentConfigurationStore jSComponent, string contentRootRelativePath, string hostPagePathWithinFileProvider, BlazorHybridOptions options) : base(services, Dispatcher.CreateDefault(), appBaseUri, fileProvider, jSComponent, hostPagePathWithinFileProvider)
    {
        _formedge = formedge;
        _appBaseUri = appBaseUri;
        _contentRootRelativePath = contentRootRelativePath;
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




        //Navigate("/");

    }

    public bool TryGetResponseContent(string uri, out int statusCode, out string statusMessage, out Stream content, out IDictionary<string, string> headers)
    {
        return TryGetResponseContent(uri, false, out statusCode, out statusMessage, out content, out headers);
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
