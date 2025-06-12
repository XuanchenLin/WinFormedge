// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace WinFormedge;

/// <summary>
/// Represents the main window class for the WinFormedge application.
/// Provides properties and methods for window management and events.
/// </summary>
public partial class Formedge : IWin32Window
{
    private bool _enabled = true;
    private Icon? _icon = null;

    private Point? _location;
    private bool _maximizable = true;
    private Size? _maximumSize;
    private bool _minimizable = true;
    private Size? _minimumSize;
    private bool _showInTaskbar = true;
    private Size? _size;
    private FormStartPosition _startPosition = FormStartPosition.WindowsDefaultLocation;
    private bool _topMost = false;
    private bool _visible = true;
    private string _windowCaption = "WinFormedge";
    private FormWindowState _windowState = FormWindowState.Normal;
    private bool _isDisposed;

    /// <summary>
    /// Occurs when the window is activated.
    /// </summary>
    public event EventHandler? Activated;

    /// <summary>
    /// Occurs when the window is deactivated.
    /// </summary>
    public event EventHandler? Deactivate;

    /// <summary>
    /// Occurs when the window has closed.
    /// </summary>
    public event FormClosedEventHandler? FormClosed;

    /// <summary>
    /// Occurs when the window is closing.
    /// </summary>
    public event FormClosingEventHandler? FormClosing;

    /// <summary>
    /// Occurs when the window receives focus.
    /// </summary>
    public event EventHandler? GotFocus;

    /// <summary>
    /// Occurs when the window loses focus.
    /// </summary>
    public event EventHandler? LostFocus;

    /// <summary>
    /// Occurs when the window is moved.
    /// </summary>
    public event EventHandler? Move;

    /// <summary>
    /// Occurs when the window is resized.
    /// </summary>
    public event EventHandler? Resize;

    /// <summary>
    /// Occurs when the window resize operation begins.
    /// </summary>
    public event EventHandler? ResizeBegin;
    /// <summary>
    /// Occurs when the window resize operation ends.
    /// </summary>
    public event EventHandler? ResizeEnd;
    /// <summary>
    /// Occurs when the window is shown.
    /// </summary>
    public event EventHandler? Shown;

    /// <summary>
    /// Occurs when the window's visibility changes.
    /// </summary>
    public event EventHandler? VisibleChanged;
    /// <summary>
    /// Gets or sets a value indicating whether the window is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.Enabled;
            }
            return _enabled;
        }
        set
        {
            _enabled = value;

            if (IsWindowCreated)
            {
                HostWindow.Enabled = _enabled;
            }
        }
    }



    /// <summary>
    /// Gets or sets a value indicating whether the window is in fullscreen mode.
    /// </summary>
    public bool Fullscreen
    {
        get => WindowStyleSettings.Fullscreen;
        set
        {
            if (!AllowFullscreen) return;

            WindowStyleSettings.Fullscreen = value;
        }
    }

    /// <summary>
    /// Gets the window handle.
    /// </summary>
    public nint Handle { get; private set; }

    /// <summary>
    /// Gets or sets the height of the window.
    /// </summary>
    public int Height
    {
        get => _hostWindow?.Height ?? 0;
        set
        {
            if (IsWindowCreated)
            {
                HostWindow.Height = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the window icon.
    /// </summary>
    public Icon? Icon
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.Icon;
            }
            return _icon;
        }
        set
        {
            _icon = value;

            if (IsWindowCreated)
            {
                HostWindow.Icon = value;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether invoke is required for thread safety.
    /// </summary>
    public bool InvokeRequired => _hostWindow?.InvokeRequired ?? false;

    /// <summary>
    /// Invokes the specified action on the UI thread.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    public void Invoke(Action action) => HostWindow.Invoke(action);

    /// <summary>
    /// Invokes the specified delegate on the UI thread.
    /// </summary>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The result of the delegate.</returns>
    public object Invoke(Delegate action) => HostWindow.Invoke(action);

    /// <summary>
    /// Invokes the specified delegate with arguments on the UI thread.
    /// </summary>
    /// <param name="method">The delegate to invoke.</param>
    /// <param name="args">The arguments for the delegate.</param>
    /// <returns>The result of the delegate.</returns>
    public object Invoke(Delegate method, params object[]? args) => HostWindow.Invoke(method, args);

    /// <summary>
    /// Invokes the specified function on the UI thread and returns the result.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="func">The function to invoke.</param>
    /// <returns>The result of the function.</returns>
    public T? Invoke<T>(Func<T> func) => HostWindow.Invoke(func);

    /// <summary>
    /// Invokes the specified action on the UI thread if required.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    public void InvokeIfRequired(Action action)
    {
        if (IsDisposed) return;

        if (HostWindow.InvokeRequired)
        {
            try
            {
                HostWindow.Invoke(action);
            }
            catch (ObjectDisposedException) { }
            catch (ThreadAbortException) { }
        }
        else
        {
            action();
        }
    }

    /// <summary>
    /// Invokes the specified delegate with arguments on the UI thread if required.
    /// </summary>
    /// <param name="method">The delegate to invoke.</param>
    /// <param name="args">The arguments for the delegate.</param>
    /// <returns>The result of the delegate.</returns>
    public object? InvokeIfRequired(Delegate method, params object[] args)
    {
        if (HostWindow.IsDisposed) return default;


        if (HostWindow.InvokeRequired)
            try
            {
                return HostWindow.Invoke(method, args);
            }
            catch (ObjectDisposedException) { }
            catch (ThreadAbortException) { }
        else
            return method.DynamicInvoke(args);

        return default;
    }

    /// <summary>
    /// Invokes the specified delegate with arguments on the UI thread if required and returns the result.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="method">The delegate to invoke.</param>
    /// <param name="args">The arguments for the delegate.</param>
    /// <returns>The result of the delegate.</returns>
    public T? InvokeIfRequired<T>(Delegate method, params object[] args)
    {
        if (HostWindow.IsDisposed) return default;

        if (HostWindow.InvokeRequired)
            try
            {
                return (T?)HostWindow.Invoke(method, args);
            }
            catch (ObjectDisposedException) { }
            catch (ThreadAbortException) { }
        else
            return (T?)method.DynamicInvoke(args);
        return default;

    }

    /// <summary>
    /// Invokes the specified function on the UI thread if required and returns the result.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="func">The function to invoke.</param>
    /// <returns>The result of the function.</returns>
    public T? InvokeIfRequired<T>(Func<T> func)
    {
        if (HostWindow.IsDisposed) return default;

        if (HostWindow.InvokeRequired)
            try
            {
                return HostWindow.Invoke(func);
            }
            catch (ObjectDisposedException) { }
            catch (ThreadAbortException) { }
        else
            return func();
        return default;
    }


    /// <summary>
    /// Gets or sets the left position of the window.
    /// </summary>
    public int Left
    {
        get => _hostWindow?.Left ?? 0;
        set
        {
            if (IsWindowCreated)
            {
                HostWindow.Left = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the location of the window.
    /// </summary>
    public Point Location
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.Location;
            }
            return _location ?? default;
        }
        set
        {
            _location = value;

            if (IsWindowCreated)
            {
                HostWindow.Location = _location.Value;
            }
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the window can be maximized.
    /// </summary>
    public bool Maximizable
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.MaximizeBox;
            }
            return _maximizable;
        }
        set
        {
            _maximizable = value;

            if (IsWindowCreated)
            {
                HostWindow.MaximizeBox = _maximizable;
            }
        }
    }

    /// <summary>
    /// Gets or sets the maximum size of the window.
    /// </summary>
    public Size MaximumSize
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.MaximumSize;
            }
            return _maximumSize ?? default;
        }
        set
        {
            _maximumSize = value;

            if (IsWindowCreated)
            {
                HostWindow.MaximumSize = _maximumSize.Value;
            }
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the window can be minimized.
    /// </summary>
    public bool Minimizable
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.MinimizeBox;
            }
            return _minimizable;
        }
        set
        {
            _minimizable = value;

            if (IsWindowCreated)
            {
                HostWindow.MinimizeBox = _minimizable;
            }
        }
    }
    /// <summary>
    /// Gets or sets the minimum size of the window.
    /// </summary>
    public Size MinimumSize
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.MinimumSize;
            }
            return _minimumSize ?? default;
        }
        set
        {
            _minimumSize = value;

            if (IsWindowCreated)
            {
                HostWindow.MinimumSize = _minimumSize.Value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the document title in the window caption.
    /// </summary>
    public bool ShowDocumentTitle { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the window is shown in the taskbar.
    /// </summary>
    public bool ShowInTaskbar
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.ShowInTaskbar;
            }
            return _showInTaskbar;
        }
        set
        {
            _showInTaskbar = value;

            if (IsWindowCreated)
            {
                HostWindow.ShowInTaskbar = _showInTaskbar;
            }
        }
    }

    /// <summary>
    /// Gets or sets the size of the window.
    /// </summary>
    public Size Size
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.Size;
            }
            return _size ?? default;
        }
        set
        {
            _size = value;

            if (IsWindowCreated)
            {
                HostWindow.Size = _size.Value;
            }

        }
    }    /// <summary>
         /// Gets or sets the start position of the window.
         /// </summary>
    public FormStartPosition StartPosition
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.StartPosition;
            }
            return _startPosition;
        }
        set
        {
            _startPosition = value;

            if (IsWindowCreated)
            {
                HostWindow.StartPosition = _startPosition;
            }
        }
    }

    /// <summary>
    /// Gets or sets the top position of the window.
    /// </summary>
    public int Top
    {
        get => _hostWindow?.Top ?? 0;
        set
        {
            if (IsWindowCreated)
            {
                HostWindow.Top = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the window is topmost.
    /// </summary>
    public bool TopMost
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.TopMost;
            }
            return _topMost;
        }
        set
        {
            _topMost = value;

            if (IsWindowCreated)
            {
                HostWindow.TopMost = _topMost;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the window is visible.
    /// </summary>
    public bool Visible
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.Visible;
            }
            return _visible;
        }
        set
        {
            _visible = value;

            if (IsWindowCreated)
            {
                HostWindow.Visible = _visible;
            }
        }
    }
    /// <summary>
    /// Gets or sets the width of the window.
    /// </summary>
    public int Width
    {
        get => _hostWindow?.Width ?? 0;
        set
        {
            if (IsWindowCreated)
            {
                HostWindow.Width = value;
            }
        }
    }
    /// <summary>
    /// Gets or sets the window state (normal, minimized, maximized).
    /// </summary>
    public FormWindowState WindowState
    {
        get
        {
            if (IsWindowCreated)
            {
                return HostWindow.WindowState;
            }

            return _windowState;
        }
        set
        {
            if (Fullscreen) return;

            if (IsWindowCreated)
            {
                HostWindow.WindowState = value;
            }
            else
            {
                _windowState = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the window caption (title).
    /// </summary>
    public string WindowTitle
    {
        get => _windowCaption;
        set
        {
            _windowCaption = value;

            if (IsWindowCreated)
            {
                UpdateWindowCaption();
            }

        }
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
    public void Show(IWin32Window owner)
    {
        AssignOwnerFromHandle(HostWindow, owner);

        HostWindow.Show(owner);
    }

    /// <summary>
    /// Shows the window.
    /// </summary>
    public void Show() => HostWindow.Show();

    /// <summary>
    /// Shows the window as a modal dialog with the specified owner.
    /// </summary>
    /// <param name="owner">The owner window.</param>
    /// <returns>A DialogResult value.</returns>
    public DialogResult ShowDialog(IWin32Window owner)
    {
        AssignOwnerFromHandle(HostWindow, owner);

        return HostWindow.ShowDialog(owner);
    }

    /// <summary>
    /// Shows the window as a modal dialog.
    /// </summary>
    /// <returns>A DialogResult value.</returns>
    public DialogResult ShowDialog() => HostWindow.ShowDialog();

    /// <summary>
    /// Toggles the fullscreen state of the window.
    /// </summary>
    public void ToggleFullscreen()
    {
        Fullscreen = !Fullscreen;
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

    /// <summary>
    /// Updates the window caption based on the current settings.
    /// </summary>
    protected virtual void UpdateWindowCaption()
    {
        if (ShowDocumentTitle && !string.IsNullOrWhiteSpace(DocumentTitle))
        {
            HostWindow.Text = $"{DocumentTitle} - {WindowTitle}";
        }
        else
        {
            HostWindow.Text = WindowTitle;
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
    /// Assigns the owner of the child form based on the provided handle.
    /// </summary>
    /// <param name="child">
    /// The child form to assign the owner to.
    /// </param>
    /// <param name="owner">
    /// The owner window handle to assign as the owner of the child form.
    /// </param>
    private void AssignOwnerFromHandle(Form child, IWin32Window owner)
    {
        if (owner is not null)
        {
            var forms = Application.OpenForms.Cast<Form>();

            var ownerForm = forms.SingleOrDefault(x => x.Handle == owner.Handle);

            child.Owner = ownerForm;
        }
    }

    public bool IsDisposed => _hostWindow?.IsDisposed ?? false || _isDisposed;

}