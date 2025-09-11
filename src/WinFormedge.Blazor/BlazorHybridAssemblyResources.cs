using System.Reflection;

namespace WinFormedge.Blazor;

/// <summary>
/// Represents an assembly containing static resources for a Blazor Hybrid application.
/// </summary>
public sealed class BlazorHybridAssemblyResources
{
    /// <summary>
    /// Gets the assembly containing the static resources.
    /// </summary>
    public Assembly ResourcesAssembly { get; }
    /// <summary>
    /// Gets or sets the base namespace within the assembly where the static resources are located.
    /// </summary>
    public string? BaseNamespace { get; set; }


    /// <summary>
    /// Constructs an instance of <see cref="BlazorHybridAssemblyResources"/>.
    /// </summary>
    /// <param name="resourcesAssembly">The assembly containing the static resources.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="resourcesAssembly"/> is null.
    /// </exception>
    public BlazorHybridAssemblyResources(Assembly resourcesAssembly)
    {
        ResourcesAssembly = resourcesAssembly ?? throw new ArgumentNullException(nameof(resourcesAssembly));

        BaseNamespace = resourcesAssembly.EntryPoint?.DeclaringType?.Namespace ?? resourcesAssembly.GetName().Name!;
    }
} 
