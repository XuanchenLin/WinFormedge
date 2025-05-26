// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

/// <summary>
/// Provides a builder for creating and configuring host windows.
/// </summary>
public sealed class HostWindowBuilder
{
    /// <summary>
    /// Configures the builder to use the default window settings.
    /// </summary>
    /// <returns>
    /// A <see cref="DefaultWindowSettings"/> instance for further configuration.
    /// </returns>
    public DefaultWindowSettings UseDefaultWindow()
    {
        return new DefaultWindowSettings();
    }
}
