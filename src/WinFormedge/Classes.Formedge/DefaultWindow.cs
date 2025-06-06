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

    /// <inheritdoc/>
    protected override void WndProc(ref Message m)
    {
        var wndProcs = _settings.WndProc?.GetInvocationList() ?? [];

        var result = false;

        foreach (WindowProc wndProc in wndProcs)
        {
            result |= wndProc.Invoke(ref m);
        }

        if (result) return;

        base.WndProc(ref m);
    }

    /// <inheritdoc/>
    protected override void DefWndProc(ref Message m)
    {
        var wndProcs = _settings.DefWndProc?.GetInvocationList() ?? [];

        var result = false;
        foreach (WindowProc wndProc in wndProcs)
        {
            result |= wndProc.Invoke(ref m);
        }

        if (result) return;

        base.DefWndProc(ref m);
    }

}
