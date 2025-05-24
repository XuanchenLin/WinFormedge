// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormedge.HostForms;
#region FormIconDisablerPlaceHolder
public partial class _WinFormClassDisabler
{

}
#endregion

class DefaultWindow : FormBase
{
    private readonly DefaultWindowSettings _settings;

    public DefaultWindow(DefaultWindowSettings settings)
    {
        _settings = settings;

        ExtendsContentIntoTitleBar = settings.ExtendsContentIntoTitleBar;
        SystemBackdropType = settings.SystemBackdropType;
        SystemMenu = settings.SystemMenu;
        ShadowDecorated = settings.ShowWindowDecorators;
        WindowEdgeOffsets = settings.WindowEdgeOffsets;
        Resizable = settings.Resizable;

        //if (!settings.Size.IsEmpty)
        //{
        //    Size = settings.Size;
        //}

        //if (!settings.IsLocationSet)
        //{
        //    Location = settings.Location;
        //}

        //if (settings.MaximumSize is not null)
        //{
        //    MaximumSize = settings.MaximumSize.Value;
        //}

        //if (settings.MinimumSize is not null)
        //{
        //    MinimumSize = settings.MinimumSize.Value;
        //}

        //if (settings.Icon is not null)
        //{
        //    Icon = settings.Icon;
        //}
        //Text = settings.WindowCaption;
        //Resizable = settings.Resizable;
        //MaximizeBox = settings;
        //MinimizeBox = settings.Minimizable;
        //TopMost = settings.TopMost;
        //WindowState = settings.WindowState;
        //Enabled = settings.Enabled;
        //ShowInTaskbar = settings.ShowInTaskbar;
        //BackColor = settings.SolidBackColor;
        //StartPosition= settings.StartPosition;
    }

    protected override void WndProc(ref Message m)
    {
        if (_settings.WndProc?.Invoke(ref m) ?? false) return;

        base.WndProc(ref m);
    }
    protected override void DefWndProc(ref Message m)
    {
        if (_settings.DefWndProc?.Invoke(ref m) ?? false) return;

        base.DefWndProc(ref m);
    }

}
