// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge.WebResource;
/// <summary>
/// Handles web resource requests by serving files embedded as resources in an assembly.
/// Supports localization via satellite assemblies and fallback logic.
/// </summary>
class EmbeddedFileResourceHandler : WebResourceHandler
{
    /// <summary>
    /// Gets the URI scheme handled by this resource handler.
    /// </summary>
    public override string Scheme { get; }

    /// <summary>
    /// Gets the host name handled by this resource handler.
    /// </summary>
    public override string HostName { get; }

    /// <summary>
    /// Gets the web resource context for this handler.
    /// </summary>
    public override CoreWebView2WebResourceContext WebResourceContext { get; }

    /// <summary>
    /// Gets the assembly containing the embedded resources to serve.
    /// </summary>
    public Assembly ResourceAssembly { get; }

    /// <summary>
    /// Gets the root folder name within the assembly for embedded resources, if specified.
    /// </summary>
    public string? FolderName { get; }

    /// <summary>
    /// Gets the default namespace used to resolve embedded resource names.
    /// </summary>
    public string? DefaultNamespace => Options.DefaultNamespace ?? ResourceAssembly.EntryPoint?.DeclaringType?.Namespace ?? ResourceAssembly.GetName().Name!;

    /// <summary>
    /// Gets the options used to configure this embedded file resource handler.
    /// </summary>
    public EmbeddedFileResourceOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedFileResourceHandler"/> class with the specified options.
    /// </summary>
    /// <param name="opts">The options for configuring the handler.</param>
    public EmbeddedFileResourceHandler(EmbeddedFileResourceOptions opts)
    {
        Scheme = opts.Scheme;
        HostName = opts.HostName;
        WebResourceContext = opts.WebResourceContext;
        ResourceAssembly = opts.ResourceAssembly;
        FolderName = opts.DefaultFolderName;
        Options = opts;
    }

    /// <summary>
    /// Returns a <see cref="WebResourceResponse"/> for the given web resource request.
    /// Attempts to resolve the request to an embedded resource, including support for satellite assemblies and fallback logic.
    /// </summary>
    /// <param name="webResourceRequest">The web resource request.</param>
    /// <returns>A <see cref="WebResourceResponse"/> containing the resource stream and content type, or a 404 response if not found.</returns>
    protected override WebResourceResponse GetResourceResponse(WebResourceRequest webResourceRequest)
    {
        var requestUrl = webResourceRequest.RequestUrl;
        Assembly mainAssembly = ResourceAssembly!;

        var request = webResourceRequest.Request;

        var response = new WebResourceResponse();

        if (!request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
        {
            response.HttpStatus = StatusCodes.Status404NotFound;

            return response;
        }

        var resourceName = GetResourceName(webResourceRequest.RelativePath, FolderName);

        Assembly? satelliteAssembly = null;

        try
        {
            var fileInfo = new FileInfo(new Uri(mainAssembly.Location).LocalPath);

            var satelliteFilePath = Path.Combine(fileInfo.DirectoryName ?? string.Empty, $"{Thread.CurrentThread.CurrentCulture}", $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}.resources.dll");

            if (File.Exists(satelliteFilePath))
            {
                satelliteAssembly = mainAssembly.GetSatelliteAssembly(Thread.CurrentThread.CurrentCulture);
            }
        }
        catch
        {

        }

        var embeddedResources = mainAssembly.GetManifestResourceNames().Select(x => new { Target = mainAssembly, Name = x, ResourceName = x, IsSatellite = false });

        if (satelliteAssembly != null)
        {
            static string ProcessCultureName(string filename) => $"{Path.GetFileNameWithoutExtension(Path.GetFileName(filename))}.{Thread.CurrentThread.CurrentCulture.Name}{Path.GetExtension(filename)}";

            embeddedResources = embeddedResources.Union(satelliteAssembly.GetManifestResourceNames().Select(x => new { Target = satelliteAssembly, Name = ProcessCultureName(x), ResourceName = ProcessCultureName(x), IsSatellite = true }));
        }

        var namespaces = mainAssembly.DefinedTypes.Select(x => x.Namespace).Distinct().ToArray();

        /// <summary>
        /// Adjusts the resource name to use the default namespace if necessary.
        /// </summary>
        /// <param name="rawName">The raw resource name.</param>
        /// <returns>The adjusted resource name.</returns>
        string ChangeResourceName(string rawName)
        {
            var targetName = namespaces.Where(x => x != null && !string.IsNullOrEmpty(x) && rawName.StartsWith(x!)).OrderByDescending(x => x!.Length).FirstOrDefault();

            if (targetName == null)
            {
                targetName = DefaultNamespace;
            }

            return $"{DefaultNamespace}{rawName.Substring($"{targetName}".Length)}";
        }

        embeddedResources = embeddedResources.Select(x =>
        new
        {
            x.Target,
            //Name = $"{DefaultNamespace}{x.Name.Substring($"{DefaultNamespace}".Length)}",
            Name = ChangeResourceName(x.Name),
            x.ResourceName,
            x.IsSatellite
        });

        var resource = embeddedResources.SingleOrDefault(x => x.Name.Equals(resourceName, StringComparison.CurrentCultureIgnoreCase));

        if (resource == null && !webResourceRequest.HasFileName)
        {
            foreach (var defaultFileName in DefaultFileName)
            {

                resourceName = string.Join(".", resourceName, defaultFileName);

                resource = embeddedResources.SingleOrDefault(x => x.Name.Equals(resourceName, StringComparison.CurrentCultureIgnoreCase));

                if (resource != null)
                {
                    break;
                }
            }
        }

        if (resource == null && Options.OnFallback != null)
        {
            var fallbackFile = Options.OnFallback.Invoke(requestUrl);

            resourceName = GetResourceName(fallbackFile, FolderName);

            resource = embeddedResources.SingleOrDefault(x => x.Name.Equals(resourceName, StringComparison.CurrentCultureIgnoreCase));
        }

        if (resource != null)
        {
            var manifestResourceName = resource.ResourceName;

            if (resource.IsSatellite)
            {
                manifestResourceName = $"{Path.GetFileNameWithoutExtension(Path.GetFileName(manifestResourceName))}{Path.GetExtension(manifestResourceName)}";
            }

            var contenStream = resource?.Target?.GetManifestResourceStream(manifestResourceName);

            if (contenStream != null)
            {

                response.ContentBody = contenStream;
                response.ContentType = GetMimeType(resourceName) ?? "text/plain";
                return response;
            }
        }

        response.HttpStatus = StatusCodes.Status404NotFound;

        return response;
    }

    /// <summary>
    /// Constructs the fully qualified resource name for an embedded resource based on the relative path and optional root path.
    /// Handles special characters and namespace adjustments.
    /// </summary>
    /// <param name="relativePath">The relative path of the requested resource.</param>
    /// <param name="rootPath">The root folder name, if any.</param>
    /// <returns>The fully qualified resource name.</returns>
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
