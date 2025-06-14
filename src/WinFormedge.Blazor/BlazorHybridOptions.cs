using System.Reflection;

using WinFormedge.WebResource;

namespace WinFormedge.Blazor;

/// <summary>
/// Delegate to configure services for Blazor Hybrid applications.
/// </summary>
/// <param name="services">
/// The service collection to configure. 
/// </param>
public delegate void ConfigureServices(ServiceCollection services);

/// <summary>
/// Options for configuring a Blazor Hybrid application.
/// </summary>
public sealed class BlazorHybridOptions : WebResourceOptions
{

    /// <summary>
    /// Gets the root component type for the Blazor application.
    /// </summary>
    public required Type RootComponent { get; init; }

    /// <summary>
    /// Gets the host path to the main HTML file (typically "wwwroot/index.html").
    /// </summary>
    public string HostPage { get; init; } = Path.Combine("wwwroot", "index.html");

    /// <summary>
    /// Gets the CSS selector used to mount the Blazor root component.
    /// </summary>
    public string Selector { get; init; } = "#app";

    /// <summary>
    /// Gets the parameters to pass to the root component.
    /// </summary>
    public Dictionary<string, object?>? Parameters { get; init; } = null;

    /// <summary>
    /// Gets the delegate to configure additional services for the Blazor application.
    /// </summary>
    public ConfigureServices? ConfigureServices { get; init; } = null;

    /// <summary>
    /// Gets or sets the assembly containing static resources for the Blazor application.
    /// </summary>
    public Assembly? StaticResources { get; init; }

    /// <summary>
    /// Gets or sets the namespace for static resources in the Blazor application.
    /// </summary>
    public string? StaticResourcesNamespace { get; set; }
}
