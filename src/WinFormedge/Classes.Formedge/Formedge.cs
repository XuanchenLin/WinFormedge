// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WinFormedge;
public partial class Formedge
{
    protected virtual void OnLoad()
    {
        Load?.Invoke(this, EventArgs.Empty);
    }



    public event EventHandler? Load;
}

