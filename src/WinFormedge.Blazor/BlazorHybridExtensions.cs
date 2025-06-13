namespace WinFormedge.Blazor;

public static class BlazorHybridExtensions
{
    public static void SetVirtualHostNameToBlazorHybrid(this Formedge formedge, BlazorHybridOptions options)
    {
        formedge.RegisterWebResourceHander(new BlazorHybridResourceHandler(options, formedge));
    }

    public static void ClearVirtualHostNameToBlazorHybrid(this Formedge formedge, BlazorHybridOptions options)
    {
        formedge.UnregisterWebResourceHandler(options.Scheme, options.HostName);
    }
}