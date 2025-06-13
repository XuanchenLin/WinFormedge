using System.Reflection;
using System.Text.RegularExpressions;

using WinFormedge.WebResource;

namespace WinFormedge.Blazor;

class FormedgeWebViewManager : WebViewManager
{
    private readonly PhysicalFileProvider? _physicalFileProvider;

    public FormedgeWebViewManager(IServiceProvider serviceProvider, Formedge formedge, Uri appBaseUri, /*string contentRoot,*/ Assembly assembly, string relativePath, Type rootComponent, string selector = "#app", IDictionary<string, object?>? parameterView = null) : base(serviceProvider, Dispatcher.CreateDefault(), appBaseUri, new EmbeddedFileProvider(assembly), new JSComponentConfigurationStore(), relativePath)
    {
        Formedge = formedge;
        AppBaseUri = appBaseUri;
        //ContentRoot = contentRoot;
        RelativePath = relativePath;

        var wwwrootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        if (Directory.Exists(wwwrootPath))
            _physicalFileProvider = new PhysicalFileProvider(wwwrootPath);

        Dispatcher.InvokeAsync(async () =>
        {
            await AddRootComponentAsync(rootComponent, selector, parameterView is null ? ParameterView.Empty : ParameterView.FromDictionary(parameterView));
        });

        if (Formedge.CoreWebView2 is null)
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
        if (_physicalFileProvider != null)
        {
            var filePath = uri.Replace(AppBaseUri.ToString(), "").TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fileInfo = _physicalFileProvider.GetFileInfo(filePath);
            if (fileInfo.Exists)
            {
                statusCode = 200;
                statusMessage = "OK";
                content = fileInfo.CreateReadStream();
                headers = new Dictionary<string, string>
                {
                    { "Content-Type", MimeTypeUtil.GetMimeType(filePath) }
                };
                return true;
            }
        }

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
