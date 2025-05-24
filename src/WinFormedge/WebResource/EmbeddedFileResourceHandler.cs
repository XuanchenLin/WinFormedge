// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace WinFormedge.WebResource;
class EmbeddedFileResourceHandler : WebResourceHandler
{
    public override string Scheme { get; }
    public override string HostName { get; }
    public override CoreWebView2WebResourceContext WebResourceContext { get; }
    public Assembly ResourceAssembly { get; }
    public string? FolderName { get; }
    public string? DefaultNamespace => Options.DefaultNamespace ?? ResourceAssembly.EntryPoint?.DeclaringType?.Namespace ?? ResourceAssembly.GetName().Name!;
    public EmbeddedFileResourceOptions Options { get; }

    public EmbeddedFileResourceHandler(EmbeddedFileResourceOptions opts)
    {
        Scheme = opts.Scheme;
        HostName = opts.HostName;
        WebResourceContext = opts.WebResourceContext;
        ResourceAssembly = opts.ResourceAssembly;
        FolderName = opts.DefaultFolderName;
        Options = opts;
    }


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
