// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

public sealed class StartupSettings
{


    public StartupSettings()
    {


    }

    public AppCreationAction UseMainWindow(Formedge formium)
    {
        return new AppCreationAction()
        {
            EdgeForm = formium
        };
    }

    public AppCreationAction UseMainWindow(Form form)
    {
        return new AppCreationAction()
        {
            Form = form
        };
    }

    public AppCreationAction StartWitoutWindow()
    {
        return new AppCreationAction() { };
    }



}
