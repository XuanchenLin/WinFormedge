// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

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
