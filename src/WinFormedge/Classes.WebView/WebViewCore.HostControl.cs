// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace WinFormedge;
partial class WebViewCore
{

    class FullscreenWindow : Form
    {
        private bool _isNonClientRegionSupportEnabled;
        public FullscreenWindow(WebViewCore webview)
        {
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            AutoScaleMode = AutoScaleMode.Dpi;
            ShowInTaskbar = false;
            WebView = webview;
            StartPosition = FormStartPosition.Manual;

            Location = webview.Container.Location;
            Size = webview.Container.Size;

            _isNonClientRegionSupportEnabled = webview.Browser?.Settings.IsNonClientRegionSupportEnabled ?? false;
        }

        public WebViewCore WebView { get; }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            var screen = Screen.FromHandle(WebView.Container.Handle);

            WebView.Controller.Bounds = ClientRectangle;
            WebView.Controller.ParentWindow = Handle;

            WebView.Browser!.Settings.IsNonClientRegionSupportEnabled = false;


        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            WebView.Controller.Bounds = ClientRectangle;
            WebView.Browser!.Settings.IsNonClientRegionSupportEnabled = _isNonClientRegionSupportEnabled;


        }

        protected override void OnShown(EventArgs e)
        {
            WebView.Container.Hide();

            base.OnShown(e);

            WindowState = FormWindowState.Maximized;

            Activate();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            WebView.Controller.ParentWindow = WebView.Container.Handle;

            WebView.Controller.Bounds = WebView.Container.ClientRectangle;

            WebView.Container.Show();

            base.OnClosing(e);
        }

        protected override void WndProc(ref Message m)
        {
            var msg = (uint)m.Msg;

            switch (msg)
            {
                case WM_SYSCOMMAND:
                    return;
            }

            base.WndProc(ref m);
        }

    }


    internal Control _hostControl;

    public Control Container
    {
        get
        {
            return _hostControl.TopLevelControl ?? _hostControl;
        }
    }

    private Control? _temporaryContainerControl;
    private void HostHandleCreated(object? sender, EventArgs e)
    {
        if (Container.RecreatingHandle)
        {
            if (_temporaryContainerControl == null) throw new NullReferenceException("Temporary container control is null.");

            Controller.ParentWindow = Container.Handle;

            _temporaryContainerControl.Dispose();
            _temporaryContainerControl = null;
        }
        else
        {
            CreateWebView2();
        }

        HandleSystemColorMode();
    }

    private void HandleSystemColorMode()
    {
        BOOL mode = WinFormedgeApp.Current.GetSystemColorMode() == SystemColorMode.Dark ? true : false;

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
        {
            unsafe
            {
                DwmSetWindowAttribute((HWND)Container.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &mode, (uint)sizeof(BOOL));
            }
        }
    }

    private void HostHandleDestroyed(object? sender, EventArgs e)
    {
        if (Container.RecreatingHandle)
        {
            _temporaryContainerControl = new Control();
            _temporaryContainerControl.CreateControl();
            Controller.ParentWindow = _temporaryContainerControl.Handle;
        }
    }

    internal bool HostWndProc(ref Message m)
    {
        var msg = (uint)m.Msg;
        switch (msg)
        {
            case WM_SETTINGCHANGE when m.LParam != 0:
                {
                    OnWmSettingChangeWithImmersiveColorSet(m.LParam);
                }
                break;
            case WM_ERASEBKGND:
                {
                    return Initialized;
                }
        }


        return false;
    }

    private void OnWmSettingChangeWithImmersiveColorSet(nint lParam)
    {
        const string IMMERSIVE_COLOR_SET = "ImmersiveColorSet";

        const int strlen = 255;

        var buffer = new byte[strlen];

        Marshal.Copy(lParam, buffer, 0, buffer.Length);


        var setting = Encoding.Unicode.GetString(buffer);

        setting = setting.Substring(0, setting.IndexOf('\0'));

        if (setting == IMMERSIVE_COLOR_SET)
        {
            HandleSystemColorMode();
        }

    }
}
