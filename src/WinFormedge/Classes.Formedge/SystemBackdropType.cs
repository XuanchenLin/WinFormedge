// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

/// <summary>
/// Specifies the type of system backdrop effect to apply to a window or control.
/// </summary>
public enum SystemBackdropType
{
    /// <summary>
    /// Automatically selects the most appropriate backdrop type based on system settings and capabilities.
    /// </summary>
    Auto,

    /// <summary>
    /// No system backdrop effect is applied.
    /// </summary>
    None,

    /// <summary>
    /// Applies a blur-behind effect to the background.
    /// </summary>
    BlurBehind,

    /// <summary>
    /// Applies an acrylic material effect to the background.
    /// </summary>
    Acrylic,

    /// <summary>
    /// Applies the standard Mica material effect to the background.
    /// </summary>
    Mica,

    /// <summary>
    /// Applies the alternative Mica material effect to the background.
    /// </summary>
    MicaAlt,

    /// <summary>
    /// Applies a transient backdrop effect, typically used for temporary surfaces.
    /// </summary>
    Transient,
}

