// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

public sealed class AppCreationAction
{
    internal Form? Form { get; set; }

    internal Formedge? EdgeForm { get; set; }


    public AppCreationAction()
    {
    }

    internal void ConfigureAppContext(StartupApplicationContext appContext)
    {
        if (EdgeForm != null)
        {
            appContext.UseStartupForm(EdgeForm);
        }
        else if (Form != null)
        {
            appContext.UseStartupForm(Form);
        }
    }
}
