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

        if(Formedge.CoreWebView2 is null)
        {
            Formedge.Load += (_, _) =>
            {
                InitScript(Formedge.CoreWebView2!);
            };
        }
        else
        {
            InitScript(Formedge.CoreWebView2);
        }




        Navigate("/");

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
                //var script = $"__dispatchMessageCallback(\"{HttpUtility.JavaScriptStringEncode(message)}\")";
                //Formedge.CoreWebView2.ExecuteScriptAsync(script);

                Formedge.CoreWebView2.PostWebMessageAsString(message);
            }
        });



    }
}
