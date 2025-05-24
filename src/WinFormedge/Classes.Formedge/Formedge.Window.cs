// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WinFormedge;


public partial class Formedge : IWin32Window
{


    public event EventHandler? Activated;

    public event EventHandler? Deactivate;

    public event EventHandler? ResizeBegin;

    public event EventHandler? Resize;

    public event EventHandler? ResizeEnd;

    public event EventHandler? Move;

    public event EventHandler? Shown;

    public event EventHandler? VisibleChanged;

    public event FormClosingEventHandler? FormClosing;

    public event FormClosedEventHandler? FormClosed;

    public event EventHandler? GotFocus;

    public event EventHandler? LostFocus;

    public nint Handle => HostWindow.Handle;
    public Size Size { get => HostWindow.Size; set => HostWindow.Size = value; }
    public Point Location { get => HostWindow.Location; set => HostWindow.Location = value; }

    private string _windowCaption = "WinFormedge";

    public string WindowCaption
    {
        get => _windowCaption;
        set
        {
            _windowCaption = value;
            UpdateWindowCaption();
        }
    }
    public bool Visible { get => HostWindow.Visible; set => HostWindow.Visible = value; }
    public bool TopMost { get => HostWindow.TopMost; set => HostWindow.TopMost = value; }
    public bool ShowInTaskbar { get => HostWindow.ShowInTaskbar; set => HostWindow.ShowInTaskbar = value; }
    public bool Maximizable { get => HostWindow.MaximizeBox; set => HostWindow.MaximizeBox = value; }
    public bool Minimizable { get => HostWindow.MinimizeBox; set => HostWindow.MinimizeBox = value; }
    public Icon? Icon { get => HostWindow.Icon; set => HostWindow.Icon = value; }
    public int Left { get => HostWindow.Left; set => HostWindow.Left = value; }
    public int Top { get => HostWindow.Top; set => HostWindow.Top = value; }
    public int Width { get => HostWindow.Width; set => HostWindow.Width = value; }
    public int Height { get => HostWindow.Height; set => HostWindow.Height = value; }
    public Size MaximumSize { get => HostWindow.MaximumSize; set => HostWindow.MaximumSize = value; }
    public Size MinimumSize { get => HostWindow.MinimumSize; set => HostWindow.MinimumSize = value; }
    public bool Enabled { get => HostWindow.Enabled; set => HostWindow.Enabled = value; }
    public FormStartPosition StartPosition { get => HostWindow.StartPosition; set => HostWindow.StartPosition = value; }
    public FormWindowState WindowState
    {
        get => HostWindow.WindowState;
        set
        {
            if (Fullscreen) return;

            HostWindow.WindowState = value;
        }
    }
    public bool Fullscreen
    {
        get => _windowStyleSettings.Fullscreen;
        set
        {
            if (!AllowFullscreen) return;

            _windowStyleSettings.Fullscreen = value;
        }
    }


    public bool ShowDocumentTitle { get; set; } = false;

    public void ToggleFullscreen()
    {
        Fullscreen = !Fullscreen;
    }
    public void Activate()
    {
        HostWindow.Activate();

        if (WebView.Initialized)
        {
            WebView.Controller.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);
        }
    }

    public void Close() => HostWindow.Close();

    public void Show(IWin32Window? owner) => HostWindow.Show(owner);

    public void Show() => HostWindow.Show();

    public DialogResult ShowDialog(IWin32Window? owner) => HostWindow.ShowDialog(owner);

    public DialogResult ShowDialog() => HostWindow.ShowDialog();

    //public void CenterToParent() => HostWindow.CenterToParent();

    //public void CenterToScreen() => HostWindow.CenterToScreen();

    protected virtual void UpdateWindowCaption()
    {
        if (ShowDocumentTitle && !string.IsNullOrWhiteSpace(DocumentTitle))
        {
            HostWindow.Text = $"{DocumentTitle} - {WindowCaption}";
        }
        else
        {
            HostWindow.Text = WindowCaption;
        }
    }

    protected virtual bool WndProc(ref Message m)
    {
        return false;
    }

    protected virtual bool DefWndProc(ref Message m)
    {
        return false;
    }
}

