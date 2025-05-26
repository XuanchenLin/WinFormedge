// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge.WebResource;

/// <summary>
/// Provides options for configuring access to embedded file resources within an assembly.
/// </summary>
public sealed class EmbeddedFileResourceOptions : WebResourceOptions
{
    /// <summary>
    /// Gets the default folder name used to resolve embedded resources.
    /// </summary>
    public string? DefaultFolderName { get; init; }

    /// <summary>
    /// Gets the default namespace used to locate embedded resources within the assembly.
    /// </summary>
    public string? DefaultNamespace { get; init; }

    /// <summary>
    /// Gets the web resource context that determines which types of web resources are handled.
    /// </summary>
    public CoreWebView2WebResourceContext WebResourceContext { get; init; } = CoreWebView2WebResourceContext.All;

    /// <summary>
    /// Gets the assembly containing the embedded resources to be served.
    /// </summary>
    public required Assembly ResourceAssembly { get; init; }
}
