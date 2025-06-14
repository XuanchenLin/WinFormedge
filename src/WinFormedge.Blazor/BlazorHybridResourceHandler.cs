using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using WinFormedge.WebResource;

namespace WinFormedge.Blazor;

class BlazorHybridResourceHandler : WebResourceHandler
{
    private IFileProvider _fileProvider;
    public FormedgeWebViewManager FormedgeWebViewManager { get; }
    public override string HostName { get; }
    public BlazorHybridOptions Options { get; }
    public string RelativePath { get; }
    public string RootFolderPath { get; }
    public override string Scheme { get; }
    public ServiceProvider Services { get; }
    public override CoreWebView2WebResourceContext WebResourceContext { get; }
    private bool _isEmbdeedeStaticResources =>
        Options.StaticResources is not null;

    private string? DefaultNamespace => Options.StaticResourcesNamespace ?? Options?.StaticResources?.EntryPoint?.DeclaringType?.Namespace ?? Options?.StaticResources?.GetName().Name!;
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

        string appRootDir;
#pragma warning disable IL3000
        var entryAssemblyLocation = Assembly.GetEntryAssembly()?.Location;
#pragma warning restore IL3000
        if (!string.IsNullOrEmpty(entryAssemblyLocation))
        {
            appRootDir = Path.GetDirectoryName(entryAssemblyLocation)!;
        }
        else
        {
            appRootDir = AppContext.BaseDirectory;
        }

        var startupUri = new Uri($"{Scheme}://{HostName}");

        var hostPageFullPath = Path.GetFullPath(Path.Combine(appRootDir, options.HostPage!)); // HostPage is nonnull because RequiredStartupPropertiesSet is checked above
        var contentRootDirFullPath = Path.GetDirectoryName(hostPageFullPath)!;
        var contentRootRelativePath = Path.GetRelativePath(appRootDir, contentRootDirFullPath);
        var hostPageRelativePath = Path.GetRelativePath(contentRootDirFullPath, hostPageFullPath);

        RelativePath = hostPageRelativePath;

        RootFolderPath = contentRootRelativePath;


        if (options.StaticResources is not null)
        {
            _fileProvider = new EmbeddedFileProvider(options.StaticResources, options.StaticResourcesNamespace);
        }
        else if (Directory.Exists(contentRootDirFullPath))
        {
            _fileProvider = new PhysicalFileProvider(contentRootDirFullPath);
        }
        else
        {
            throw new InvalidOperationException($"The content root directory '{contentRootDirFullPath}' does not exist.");
        }

        FormedgeWebViewManager = new FormedgeWebViewManager( formedge, Services, new Uri($"{Scheme}://{HostName}"), _fileProvider, contentRootRelativePath, hostPageRelativePath, options);
    }

    protected override WebResourceResponse GetResourceResponse(WebResourceRequest request)
    {
        var url = request.RequestUrl;


        if (_isEmbdeedeStaticResources)
        {

            var resourceName = GetResourceName(request.RelativePath, RootFolderPath);




            var fileInfo = _fileProvider.GetFileInfo(resourceName);



            if (!fileInfo.Exists && !request.HasFileName)
            {
                foreach (var defaultFileName in DefaultFileName)
                {

                    resourceName = string.Join(".", resourceName, defaultFileName);

                    fileInfo = _fileProvider.GetFileInfo(resourceName);

                    if (fileInfo.Exists)
                    {
                        break;
                    }
                }
            }

            if (!fileInfo.Exists && Options.OnFallback is not null)
            {
                var fallbackFile = Options.OnFallback.Invoke(url);

                resourceName = GetResourceName(fallbackFile, RootFolderPath);

                fileInfo = _fileProvider.GetFileInfo(resourceName);
            }

            if (fileInfo.Exists)
            {
                return new WebResourceResponse
                {
                    ContentBody = fileInfo.CreateReadStream(),
                    ContentType = GetMimeType(fileInfo.Name) ?? "application/octet-stream",
                    HttpStatus = StatusCodes.Status200OK,
                };
            }
        }





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




        //if (Options.ResourceAssembly is not null && !string.IsNullOrEmpty(Options.RootFolder))
        //{
        //    var baseUri = new Uri(url);

        //    var relativePath = baseUri.AbsolutePath;

        //    if (relativePath.StartsWith('/') && !relativePath.StartsWith($"/{Options.RootFolder}",StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        var path = Path.Combine(Options.RootFolder, relativePath.TrimStart('/','\\'));
        //        url = new Uri(baseUri, $"/{path}/").ToString();
        //    }
        //}

        var uri = new Uri(url);


        if (uri.PathAndQuery == "/")
        {
            url += RelativePath;
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

    private string GetResourceName(string relativePath, string? rootPath = null)
    {
        var filePath = relativePath;
        if (!string.IsNullOrEmpty(rootPath))
        {
            filePath = $"{rootPath?.Trim('/', '\\')}/{filePath.Trim('/', '\\')}";
        }

        filePath = filePath.Replace('\\', '/');

        var endTrimIndex = filePath.LastIndexOf('/');

        if (endTrimIndex > -1)
        {
            // https://stackoverflow.com/questions/5769705/retrieving-embedded-resources-with-special-characters

            var path = filePath.Substring(0, endTrimIndex);
            path = path.Replace("/", ".");
            if (Regex.IsMatch(path, "\\.(\\d+)"))
            {
                path = Regex.Replace(path, "\\.(\\d+)", "._$1");
            }

            const string replacePartterns = "`~!@$%^&(),-=";

            foreach (var parttern in replacePartterns)
            {
                path = path.Replace(parttern, '_');
            }

            filePath = $"{path}{filePath.Substring(endTrimIndex)}".Trim('/');
        }

        var resourceName = $"{DefaultNamespace}.{filePath.Replace('/', '.')}";

        return resourceName;

    }
}
