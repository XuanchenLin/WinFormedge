// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormedge;
public abstract class AppStartup
{
    internal protected virtual bool OnApplicationLaunched(string[] args)
    {
        return true;
    }

    internal abstract protected AppCreationAction? OnApplicationStartup(StartupSettings options);

    internal protected virtual void OnApplicationException(Exception? exception = null)
    {
    }


    internal protected virtual void OnApplicationTerminated()
    {
    }

    internal protected virtual void ConfigureSchemeRegistrations(List<CoreWebView2CustomSchemeRegistration> customSchemeRegistrations)
    {
    }

    internal protected virtual void ConfigureAdditionalBrowserArgs(NameValueCollection additionalBrowserArgs)
    {
    }

}
