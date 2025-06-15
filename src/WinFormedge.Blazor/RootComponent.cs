namespace WinFormedge.Blazor;

/// <summary>
/// Describes a root component that can be added to a <see cref="Formedge"/>.
/// </summary>
public class RootComponent
{
    /// <summary>
    /// Constructs an instance of <see cref="RootComponent"/>.
    /// </summary>
    /// <param name="selector">The CSS selector string that specifies where in the document the component should be placed. This must be unique among the root components within the <see cref="Formedge"/>.</param>
    /// <param name="componentType">The type of the root component. This type must implement <see cref="IComponent"/>.</param>
    /// <param name="parameters">An optional dictionary of parameters to pass to the root component.</param>
    public RootComponent(string selector, Type componentType, IDictionary<string, object?>? parameters)
    {
        if (string.IsNullOrWhiteSpace(selector))
        {
            throw new ArgumentException($"'{nameof(selector)}' cannot be null or whitespace.", nameof(selector));
        }

        Selector = selector;
        ComponentType = componentType ?? throw new ArgumentNullException(nameof(componentType));
        Parameters = parameters;
    }

    /// <summary>
    /// Gets the CSS selector string that specifies where in the document the component should be placed.
    /// This must be unique among the root components within the <see cref="Formedge"/>.
    /// </summary>
    public string Selector { get; }

    /// <summary>
    /// Gets the type of the root component. This type must implement <see cref="IComponent"/>.
    /// </summary>
    public Type ComponentType { get; }

    /// <summary>
    /// Gets an optional dictionary of parameters to pass to the root component.
    /// </summary>
    public IDictionary<string, object?>? Parameters { get; }

    internal Task AddToWebViewManagerAsync(WebViewManager webViewManager)
    {
        var parameterView = Parameters == null ? ParameterView.Empty : ParameterView.FromDictionary(Parameters);
        return webViewManager.AddRootComponentAsync(ComponentType, Selector, parameterView);
    }

    internal Task RemoveFromWebViewManagerAsync(FormedgeWebViewManager webviewManager)
    {
        return webviewManager.RemoveRootComponentAsync(Selector);
    }
}