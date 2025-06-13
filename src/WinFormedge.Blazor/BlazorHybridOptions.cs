using System.Reflection;

using WinFormedge.WebResource;

namespace WinFormedge.Blazor;

public delegate void ConfigureServices(ServiceCollection services);

public sealed class BlazorHybridOptions : WebResourceOptions
{
    public required Type RootComponent { get; init; }
    public string HostPath { get; init; } = Path.Combine("wwwroot", "index.html");
    public string ContentRoot => Path.GetDirectoryName(Path.GetFullPath(HostPath))!;
    public string RelativeHostPath => Path.GetRelativePath(ContentRoot, HostPath);

    public string Selector { get; init; } = "#app";

    public Dictionary<string, object?>? Parameters { get; init; } = null;


    public ConfigureServices? ConfigureServices { get; init; } = null;

    public Assembly? ResourceAssembly { get; init; } = null;

}
