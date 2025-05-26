// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge;


/// <summary>
/// Represents the main window class for the WinFormedge application.
/// Provides properties and methods for window management and events.
/// </summary>
public partial class Formedge : IWin32Window
{
    /// <summary>
    /// Occurs when the window is activated.
    /// </summary>
    public event EventHandler? Activated;

    /// <summary>
    /// Occurs when the window is deactivated.
    /// </summary>
    public event EventHandler? Deactivate;

    /// <summary>
    /// Occurs when the window resize operation begins.
    /// </summary>
    public event EventHandler? ResizeBegin;

    /// <summary>
    /// Occurs when the window is resized.
    /// </summary>
    public event EventHandler? Resize;

    /// <summary>
    /// Occurs when the window resize operation ends.
    /// </summary>
    public event EventHandler? ResizeEnd;

    /// <summary>
    /// Occurs when the window is moved.
    /// </summary>
    public event EventHandler? Move;

    /// <summary>
    /// Occurs when the window is shown.
    /// </summary>
    public event EventHandler? Shown;

    /// <summary>
    /// Occurs when the window's visibility changes.
    /// </summary>
    public event EventHandler? VisibleChanged;

    /// <summary>
    /// Occurs when the window is closing.
    /// </summary>
    public event FormClosingEventHandler? FormClosing;

    /// <summary>
    /// Occurs when the window has closed.
    /// </summary>
    public event FormClosedEventHandler? FormClosed;

    /// <summary>
    /// Occurs when the window receives focus.
    /// </summary>
    public event EventHandler? GotFocus;

    /// <summary>
    /// Occurs when the window loses focus.
    /// </summary>
    public event EventHandler? LostFocus;

    /// <summary>
    /// Gets the window handle.
    /// </summary>
    public nint Handle => HostWindow.Handle;

    /// <summary>
    /// Gets or sets the size of the window.
    /// </summary>
    public Size Size { get => HostWindow.Size; set => HostWindow.Size = value; }

    /// <summary>
    /// Gets or sets the location of the window.
    /// </summary>
    public Point Location { get => HostWindow.Location; set => HostWindow.Location = value; }

    private string _windowCaption = "WinFormedge";

    /// <summary>
    /// Gets or sets the window caption (title).
    /// </summary>
    public string WindowCaption
    {
        get => _windowCaption;
        set
        {
            _windowCaption = value;
            UpdateWindowCaption();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the window is visible.
    /// </summary>
    public bool Visible { get => HostWindow.Visible; set => HostWindow.Visible = value; }

    /// <summary>
    /// Gets or sets a value indicating whether the window is topmost.
    /// </summary>
    public bool TopMost { get => HostWindow.TopMost; set => HostWindow.TopMost = value; }

    /// <summary>
    /// Gets or sets a value indicating whether the window is shown in the taskbar.
    /// </summary>
    public bool ShowInTaskbar { get => HostWindow.ShowInTaskbar; set => HostWindow.ShowInTaskbar = value; }

    /// <summary>
    /// Gets or sets a value indicating whether the window can be maximized.
    /// </summary>
    public bool Maximizable { get => HostWindow.MaximizeBox; set => HostWindow.MaximizeBox = value; }

    /// <summary>
    /// Gets or sets a value indicating whether the window can be minimized.
    /// </summary>
    public bool Minimizable { get => HostWindow.MinimizeBox; set => HostWindow.MinimizeBox = value; }

    /// <summary>
    /// Gets or sets the window icon.
    /// </summary>
    public Icon? Icon { get => HostWindow.Icon; set => HostWindow.Icon = value; }

    /// <summary>
    /// Gets or sets the left position of the window.
    /// </summary>
    public int Left { get => HostWindow.Left; set => HostWindow.Left = value; }

    /// <summary>
    /// Gets or sets the top position of the window.
    /// </summary>
    public int Top { get => HostWindow.Top; set => HostWindow.Top = value; }

    /// <summary>
    /// Gets or sets the width of the window.
    /// </summary>
    public int Width { get => HostWindow.Width; set => HostWindow.Width = value; }

    /// <summary>
    /// Gets or sets the height of the window.
    /// </summary>
    public int Height { get => HostWindow.Height; set => HostWindow.Height = value; }

    /// <summary>
    /// Gets or sets the maximum size of the window.
    /// </summary>
    public Size MaximumSize { get => HostWindow.MaximumSize; set => HostWindow.MaximumSize = value; }

    /// <summary>
    /// Gets or sets the minimum size of the window.
    /// </summary>
    public Size MinimumSize { get => HostWindow.MinimumSize; set => HostWindow.MinimumSize = value; }

    /// <summary>
    /// Gets or sets a value indicating whether the window is enabled.
    /// </summary>
    public bool Enabled { get => HostWindow.Enabled; set => HostWindow.Enabled = value; }

    /// <summary>
    /// Gets or sets the start position of the window.
    /// </summary>
    public FormStartPosition StartPosition { get => HostWindow.StartPosition; set => HostWindow.StartPosition = value; }

    /// <summary>
    /// Gets or sets the window state (normal, minimized, maximized).
    /// </summary>
    public FormWindowState WindowState
    {
        get => HostWindow.WindowState;
        set
        {
            if (Fullscreen) return;

            HostWindow.WindowState = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the window is in fullscreen mode.
    /// </summary>
    public bool Fullscreen
    {
        get => _windowStyleSettings.Fullscreen;
        set
        {
            if (!AllowFullscreen) return;

            _windowStyleSettings.Fullscreen = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the document title in the window caption.
    /// </summary>
    public bool ShowDocumentTitle { get; set; } = false;

    /// <summary>
    /// Toggles the fullscreen state of the window.
    /// </summary>
    public void ToggleFullscreen()
    {
        Fullscreen = !Fullscreen;
    }

    /// <summary>
    /// Activates the window and sets focus to the WebView if initialized.
    /// </summary>
    public void Activate()
    {
        HostWindow.Activate();

        if (WebView.Initialized)
        {
            WebView.Controller.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);
        }
    }

    /// <summary>
    /// Closes the window.
    /// </summary>
    public void Close() => HostWindow.Close();

    /// <summary>
    /// Shows the window with the specified owner.
    /// </summary>
    /// <param name="owner">The owner window.</param>
    public void Show(IWin32Window? owner) => HostWindow.Show(owner);

    /// <summary>
    /// Shows the window.
    /// </summary>
    public void Show() => HostWindow.Show();

    /// <summary>
    /// Shows the window as a modal dialog with the specified owner.
    /// </summary>
    /// <param name="owner">The owner window.</param>
    /// <returns>A DialogResult value.</returns>
    public DialogResult ShowDialog(IWin32Window? owner) => HostWindow.ShowDialog(owner);

    /// <summary>
    /// Shows the window as a modal dialog.
    /// </summary>
    /// <returns>A DialogResult value.</returns>
    public DialogResult ShowDialog() => HostWindow.ShowDialog();

    //public void CenterToParent() => HostWindow.CenterToParent();

    //public void CenterToScreen() => HostWindow.CenterToScreen();

    /// <summary>
    /// Updates the window caption based on the current settings.
    /// </summary>
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

    /// <summary>
    /// Processes Windows messages before they are handled by the default window procedure.
    /// </summary>
    /// <param name="m">The Windows message.</param>
    /// <returns>True if the message was handled; otherwise, false.</returns>
    protected virtual bool WndProc(ref Message m)
    {
        return false;
    }

    /// <summary>
    /// Provides the default window procedure for processing Windows messages.
    /// </summary>
    /// <param name="m">The Windows message.</param>
    /// <returns>True if the message was handled; otherwise, false.</returns>
    protected virtual bool DefWndProc(ref Message m)
    {
        return false;
    }
}

