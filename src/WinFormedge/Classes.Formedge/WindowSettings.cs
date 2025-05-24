// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;

public delegate Form HostWindowCreationDelegate();

//public delegate bool RequireChangeFullscreenStateDelegate(bool fullscreen);

public abstract class WindowSettings
{

    internal protected abstract bool HasSystemTitlebar { get; }
    internal protected abstract WindowProc? WndProc { get; set; }
    internal protected abstract WindowProc? DefWndProc { get; set; }
    //public string WindowCaption { get; set; } = nameof(WinFormedge);
    //public Size Size { get => _size ?? Size.Empty; set => _size = value; }
    //public Point Location { get => _location ?? Point.Empty; set => _location = value; }
    //public FormStartPosition StartPosition { get; set; } = FormStartPosition.WindowsDefaultLocation;
    //public Size? MaximumSize { get; set; }
    //public Size? MinimumSize { get; set; }
    //public Icon? Icon { get; set; }
    //public bool Resizable { get; set; } = true;
    //public bool Maximizable { get; set; } = true;
    //public bool Minimizable { get; set; } = true;
    //public bool TopMost { get; set; } = false;
    //public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
    //public bool Enabled { get; set; } = true;
    //public bool AllowFullScreen { get; set; } = false;
    //public bool ShowInTaskbar { get; set; } = true;
    public abstract bool Fullscreen { get; set; }
    public bool Resizable { get; set; } = true;
    //public bool AllowSystemMenuOnNonClientRegion { get; set; } = true;
    //public Color BackColor { get; set; } = FormedgeApp.Current.IsDarkMode ? Color.DimGray : Color.White;


    //internal Color SolidBackColor =>
    //    Color.FromArgb(255, BackColor.R, BackColor.G, BackColor.B);

    internal protected abstract Form CreateHostWindow();

    internal protected virtual void ConfigureWinFormProps(Form form)
    {

    }

    internal protected virtual string? WindowSpecifiedJavaScript => null;
}
