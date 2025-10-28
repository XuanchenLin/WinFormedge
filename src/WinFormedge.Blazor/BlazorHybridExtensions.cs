namespace WinFormedge.Blazor;

public static class BlazorHybridExtensions
{
    /// <summary>
    /// Sets the virtual host name for a Blazor Hybrid application on the Formedge instance.
    /// </summary>
    /// <param name="formedge">
    /// The Formedge instance to configure.
    /// </param>
    /// <param name="options">
    /// The options for the Blazor Hybrid application, including the root component type, host path, and other settings.
    /// </param>
    /// <returns>
    /// The configured IServiceProvider for the Blazor Hybrid application.
    /// </returns>

    public static IServiceProvider SetVirtualHostNameToBlazorHybrid(this Formedge formedge, BlazorHybridOptions options)
    {
        formedge.RegisterWebResourceHander(new BlazorHybridResourceHandler(options, formedge));

        return AppBuilderExtensions.ServiceProvider!;
    }

    /// <summary>
    /// Clears the virtual host name for a Blazor Hybrid application on the Formedge instance.
    /// </summary>
    /// <param name="formedge">
    /// The Formedge instance to configure. 
    /// </param>
    /// <param name="options">
    /// The options for the Blazor Hybrid application, which should match the options used when setting the virtual host name.
    /// </param>
    public static void ClearVirtualHostNameToBlazorHybrid(this Formedge formedge, BlazorHybridOptions options)
    {
        formedge.UnregisterWebResourceHandler(options.Scheme, options.HostName);
    }
}