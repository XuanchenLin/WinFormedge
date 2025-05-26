// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;
internal partial class WebViewCore
{
    /// <summary>
    /// Gets or sets the current URL of the web browser.
    /// When getting, returns the current source URL or a blank page if the browser is not initialized.
    /// When setting, navigates the browser to the specified URL if the browser is available;
    /// otherwise, stores the URL to be navigated to when the browser is initialized.
    /// </summary>
    public string Url
    {
        get => Browser?.Source ?? ABOUT_BLANK;
        set
        {
            if (Browser != null)
            {
                Browser.Navigate(value);
            }
            else
            {
                _defferedUrl = value;
            }
        }
    }
}
