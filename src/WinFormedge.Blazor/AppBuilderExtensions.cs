// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge.Blazor;
public static class AppBuilderExtensions
{
    static internal IServiceCollection Services { get; } = new ServiceCollection();

    static internal IServiceProvider? ServiceProvider { get; private set; }

    /// <summary>
    /// Add Blazor Hybrid support to the AppBuilder.
    /// </summary>
    /// <param name="builder">
    /// The AppBuilder instance.
    /// </param>
    /// <param name="configureServicesAction">
    /// An optional action to configure additional services.
    /// </param>
    /// <returns>
    /// The updated AppBuilder instance.
    /// </returns>
    public static AppBuilder AddBlazorHybridSupport(this AppBuilder builder, Action<IServiceCollection>? configureServicesAction = null)
    {
        Services.AddBlazorWebView();

        configureServicesAction?.Invoke(Services);

        ServiceProvider = Services.BuildServiceProvider();

        return builder;
    }
}
