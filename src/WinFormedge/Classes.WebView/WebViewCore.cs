// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;
internal partial class WebViewCore
{
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
