// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormedge;

namespace BlazorHybridExampleApp;
internal class MyFormedgeApp : AppStartup
{
    protected override AppCreationAction? OnApplicationStartup(StartupSettings options)
    {
        return options.UseMainWindow(new MainWindow());
    }
}
