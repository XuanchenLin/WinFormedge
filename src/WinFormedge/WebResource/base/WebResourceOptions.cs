// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge.WebResource;

/// <summary>
/// Represents a delegate for providing a fallback resource URL when a web resource request fails.
/// </summary>
/// <param name="requestUrl">The original requested URL.</param>
/// <returns>The fallback URL as a string.</returns>
public delegate string WebResourceFallbackDelegate(string requestUrl);
/// <summary>
/// Provides base options for configuring web resource access, including scheme, host name, and fallback handling.
/// </summary>
public abstract class WebResourceOptions
{

    /// <summary>
    /// Gets the URI scheme (e.g., "http" or "https") used for web resource requests.
    /// </summary>
    public required string Scheme { get; init; } = "http";

    /// <summary>
    /// Gets the host name for the web resource.
    /// </summary>
    public required string HostName { get; init; }

    /// <summary>
    /// Gets or sets the delegate to handle fallback logic when a web resource cannot be resolved.
    /// </summary>
    public WebResourceFallbackDelegate? OnFallback { get; set; }
}
