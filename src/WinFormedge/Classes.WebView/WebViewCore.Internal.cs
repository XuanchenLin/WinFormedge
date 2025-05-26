// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;
partial class WebViewCore
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebViewCore"/> class with the specified host control.
    /// Subscribes to the <c>HandleCreated</c> and <c>HandleDestroyed</c> events of the container.
    /// </summary>
    /// <param name="hostControl">The control that will host the WebView.</param>
    public WebViewCore(Control hostControl)
    {
        _hostControl = hostControl;
        Container.HandleCreated += HostHandleCreated;
        Container.HandleDestroyed += HostHandleDestroyed;
    }

    /// <summary>
    /// Closes the WebView and releases associated resources.
    /// If the WebView is initialized, the controller is closed and set to <c>null</c>.
    /// </summary>
    public void Close()
    {
        if (Initialized)
        {
            Controller.Close();
            _controller = null;
        }
    }
}
