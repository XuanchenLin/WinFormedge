// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

/// <summary>
/// Provides extension methods for registering embedded file resources with a <see cref="Formedge"/> instance.
/// </summary>
public static class RegisterEmbeddedFileResourceExtension
{
    /// <summary>
    /// Maps a virtual host name to embedded resources using the specified <see cref="EmbeddedFileResourceOptions"/>.
    /// Registers a web resource handler for serving embedded files in the WebView2 environment.
    /// </summary>
    /// <param name="formedge">The <see cref="Formedge"/> instance to configure.</param>
    /// <param name="options">The options specifying the embedded resource mapping.</param>
    public static void SetVirtualHostNameToEmbeddedResourcesMapping(this Formedge formedge, EmbeddedFileResourceOptions options)
    {
        formedge.RegisterWebResourceHander(new EmbeddedFileResourceHandler(options));
    }
}
