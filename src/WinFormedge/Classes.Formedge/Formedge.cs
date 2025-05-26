// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;
/// <summary>
/// Represents the base class for Formedge components, providing load event support.
/// </summary>
public partial class Formedge
{
    /// <summary>
    /// Raises the <see cref="Load"/> event.
    /// </summary>
    protected virtual void OnLoad()
    {
        Load?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Occurs when the Formedge component is loaded.
    /// </summary>
    public event EventHandler? Load;
}

