// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace WinFormedge.HostForms;

#region FormIconDisablerPlaceHolder
public partial class _WinFormClassDisabler
{

}
#endregion

/// <summary>
/// Provides a base class for custom forms with advanced window style, borderless, shadow, and system backdrop support.
/// </summary>
abstract class FormBase : Form
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormBase"/> class.
    /// </summary>
    public FormBase()
    {
        InitializeComponent();

        _windowBorderResizer = new WebView2BorderlessResizer() { Visible = false };

    }

    /// <summary>
    /// Gets or sets a value indicating whether the form is shown as a popup window.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Popup
    {
        get => _isPopup;
        set
        {
            if (value == _isPopup) return;
            _isPopup = value;
            if (IsHandleCreated)
            {
                HandleWindowStyleChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the content extends into the title bar (borderless).
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ExtendsContentIntoTitleBar
    {
        get => _extendsContentIntoTitleBar;
        set
        {
            if (value == _extendsContentIntoTitleBar) return;
            _extendsContentIntoTitleBar = value;
            if (IsHandleCreated)
            {
                HandleWindowStyleChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the form is resizable.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Resizable
    {
        get => _resizable;
        set
        {
            if (value == _resizable) return;
            _resizable = value;
            if (IsHandleCreated)
            {
                HandleWindowStyleChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the system menu is enabled.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SystemMenu
    {
        get => _systemMenu;
        set
        {
            if (value == _systemMenu) return;
            _systemMenu = value;
            if (IsHandleCreated)
            {
                HandleWindowStyleChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the window shadow is decorated.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShadowDecorated
    {
        get => _shadowDecorated;
        set
        {
            if (value == _shadowDecorated) return;
            _shadowDecorated = value;
            if (IsHandleCreated)
            {
                HandleWindowStyleChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the form is in fullscreen mode.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Fullscreen
    {
        get => _fullscreen;
        set
        {

            if (value == _fullscreen) return;

            _fullscreen = value;

            HandleFullScreen(value);

        }
    }

    /// <summary>
    /// Gets or sets the background color of the form. Alpha is forced to 255 except for Color.Transparent.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Color BackColor
    {
        get => _backColor ?? base.BackColor;
        set
        {
            if (value == Color.Transparent)
            {
                _backColor = null;
                base.BackColor = Color.White;
            }
            else if (value.A != 255)
            {
                _backColor = value;
                base.BackColor = Color.FromArgb(255, value.R, value.G, value.B);
            }
            else
            {
                _backColor = value;
                base.BackColor = Color.FromArgb(255, value.R, value.G, value.B);
            }

        }
    }

    /// <summary>
    /// Gets or sets the padding offsets for the window edge, used for borderless resizing.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Padding WindowEdgeOffsets
    {
        get => _windowBorderResizer.BorderOffset;
        set => _windowBorderResizer.BorderOffset = value;
    }

    /// <summary>
    /// Gets or sets the system backdrop type for the window (e.g., Mica, Acrylic, BlurBehind).
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SystemBackdropType SystemBackdropType
    {
        get => _systemBackdropType;
        set
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 0) && value >= SystemBackdropType.BlurBehind)
            {
                return;
            }
            else if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22621) && value > SystemBackdropType.Acrylic)
            {
                return;
            }

            if (value == _systemBackdropType) return;

            if (IsHandleCreated)
            {
                HandleSystemBackdropTypeChanged(value);
            }
            else
            {
                _systemBackdropType = value;
            }
        }
    }

    /// <summary>
    /// Shows the form with the specified owner window.
    /// </summary>
    /// <param name="owner">The owner window.</param>
    public new void Show(IWin32Window? owner)
    {
        AssignOwnerFromHandle(owner);

        base.Show(owner);
    }

    /// <summary>
    /// Shows the form as a modal dialog with the specified owner window.
    /// </summary>
    /// <param name="owner">The owner window.</param>
    /// <returns>A <see cref="DialogResult"/> indicating how the dialog was closed.</returns>
    public new DialogResult ShowDialog(IWin32Window? owner)
    {
        AssignOwnerFromHandle(owner);

        return base.ShowDialog(owner);
    }

    /// <summary>
    /// Gets the window handle as <see cref="HWND"/>.
    /// </summary>
    internal HWND hWnd => (HWND)Handle;

    /// <summary>
    /// Performs hit testing for non-client area (NCA) for custom borderless resizing.
    /// </summary>
    /// <param name="lParam">The lParam from the message, containing the cursor position.</param>
    /// <returns>The hit test result code.</returns>
    internal uint HitTestNCA(nint lParam)
    {
        var cursor = MARCOS.ToPoint(lParam);

        var border = new Point(GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXFRAME) + GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXPADDEDBORDER), GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYFRAME) + GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXPADDEDBORDER));

        if (!GetWindowRect((HWND)Handle, out var windowRect))
        {
            return HTNOWHERE;
        }

        var result =
            (byte)HitTestNCARegionMask.left * (cursor.X < (windowRect.left + border.X) ? 1 : 0) |
            (byte)HitTestNCARegionMask.right * (cursor.X >= (windowRect.right - border.X) ? 1 : 0) |
            (byte)HitTestNCARegionMask.top * (cursor.Y < (windowRect.top + border.Y) ? 1 : 0) |
            (byte)HitTestNCARegionMask.bottom * (cursor.Y >= (windowRect.bottom - border.Y) ? 1 : 0);

        return result switch
        {
            (byte)HitTestNCARegionMask.left => Resizable ? HTLEFT : HTCLIENT,
            (byte)HitTestNCARegionMask.right => Resizable ? HTRIGHT : HTCLIENT,
            (byte)HitTestNCARegionMask.top => Resizable ? HTTOP : HTCLIENT,
            (byte)HitTestNCARegionMask.bottom => Resizable ? HTBOTTOM : HTCLIENT,
            (byte)(HitTestNCARegionMask.top | HitTestNCARegionMask.left) => Resizable ? HTTOPLEFT : HTCLIENT,
            (byte)(HitTestNCARegionMask.top | HitTestNCARegionMask.right) => Resizable ? HTTOPRIGHT : HTCLIENT,
            (byte)(HitTestNCARegionMask.bottom | HitTestNCARegionMask.left) => Resizable ? HTBOTTOMLEFT : HTCLIENT,
            (byte)(HitTestNCARegionMask.bottom | HitTestNCARegionMask.right) => Resizable ? HTBOTTOMRIGHT : HTCLIENT,
            (byte)HitTestNCARegionMask.client => HTCLIENT,
            _ => HTNOWHERE,
        };
    }

    /// <summary>
    /// Gets the non-client area metrics (padding) for the current window style.
    /// </summary>
    /// <returns>The <see cref="Padding"/> representing the non-client area.</returns>
    internal protected Padding GetNonClientMetrics()
    {
        var rect = new RECT();

        var screenRect = ClientRectangle;

        screenRect.Offset(-Bounds.Left, -Bounds.Top);

        rect.top = screenRect.Top;
        rect.left = screenRect.Left;
        rect.bottom = screenRect.Bottom;
        rect.right = screenRect.Right;

        AdjustWindowRect(ref rect, (WINDOW_STYLE)CreateParams.Style, (WINDOW_EX_STYLE)CreateParams.ExStyle);

        return new Padding
        {
            Top = screenRect.Top - rect.top,
            Left = screenRect.Left - rect.left,
            Bottom = rect.bottom - screenRect.Bottom,
            Right = rect.right - screenRect.Right
        };
    }

    /// <summary>
    /// Gets the parameters required to create the window, including custom styles and extended styles.
    /// </summary>
    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;

            cp.Style = (int)GetWindowStyle();

            if (OperatingSystem.IsWindowsVersionAtLeast(8))
            {
                switch (SystemBackdropType)
                {
                    case SystemBackdropType.None:
                    case SystemBackdropType.BlurBehind:
                    case SystemBackdropType.Acrylic:
                    case SystemBackdropType.Mica:
                    case SystemBackdropType.Transient:
                    case SystemBackdropType.MicaAlt:
                        cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_NOREDIRECTIONBITMAP;
                        break;
                    default:
                        break;
                }
            }

            return cp;
        }
    }

    /// <summary>
    /// Creates the window handle and applies custom window styles and system backdrop.
    /// </summary>
    protected override void CreateHandle()
    {
        var size = Size;

        base.CreateHandle();

        HandleWindowStyleChanged();

        HandleSystemBackdropTypeChanged(SystemBackdropType);

        if (!RecreatingHandle)
        {
            CorrectWindowPos(size);
        }
        HandleFullScreen(Fullscreen);
    }

    /// <summary>
    /// Sets the bounds of the window, with special handling for borderless and DPI scenarios.
    /// </summary>
    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
        {
            base.SetBoundsCore(x, y, width, height, specified);
            return;
        }


        if (ExtendsContentIntoTitleBar && _shouldPatchBoundsSize && ((specified & BoundsSpecified.Size) != BoundsSpecified.None) && WindowState == FormWindowState.Normal)
        {
            if (ClientSize.Width != width || ClientSize.Height != height)
            {
                var padding = GetNonClientMetrics();
                width = width - padding.Horizontal;
                height = height - padding.Vertical;

            }
        }



        base.SetBoundsCore(x, y, width, height, specified);
    }

    /// <summary>
    /// Releases the window handle and resets internal state.
    /// </summary>
    protected override void DestroyHandle()
    {
        base.DestroyHandle();

        _shouldPatchBoundsSize = false;
    }

    /// <summary>
    /// Processes Windows messages for the form, handling custom non-client area and DPI logic.
    /// </summary>
    /// <param name="m">The Windows message.</param>
    protected override void WndProc(ref Message m)
    {
        var msg = (uint)m.Msg;
        var wParam = m.WParam;
        var lParam = m.LParam;
        switch (msg)
        {
            case WM_NCCREATE when OperatingSystem.IsWindowsVersionAtLeast(10, 0, 14393):
                {
                    EnableNonClientDpiScaling((HWND)m.HWnd);
                    if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
                    {
                        unsafe
                        {
                            BOOL useHostBackdropBrush = true;
                            DwmSetWindowAttribute((HWND)Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_HOSTBACKDROPBRUSH, &useHostBackdropBrush, (uint)sizeof(BOOL));


                        }

                    }
                }
                break;
            case WM_NCCALCSIZE when wParam == 1 && ExtendsContentIntoTitleBar && !Popup && !Fullscreen:
                {
                    var nccalc = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(lParam);

                    if (!AdjustMaximizedClientRect((HWND)m.HWnd, ref nccalc.rgrc._0))
                    {
                        //OnNcResize(nccalc.rgrc._0.Width, nccalc.rgrc._0.Height);
                    }

                    Marshal.StructureToPtr(nccalc, m.LParam, false);
                }
                return;
            case WM_NCCALCSIZE when wParam == 0 && !OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000):
                {
                    //var rect = Marshal.PtrToStructure<RECT>(lParam);
                    //var r = System.Drawing.Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);

                    //Console.WriteLine(r);

                    _shouldPatchBoundsSize = true;

                }
                break;
                //case WM_NCHITTEST:
                //    {
                //        if (Popup || ExtendsContentIntoTitleBar)
                //        {
                //            m.Result = (nint)HitTestNCA(lParam);
                //            return;
                //        }
                //    }
                //    break;

        }
        //var handled = OnWindowProc?.Invoke(ref m) ?? false;

        //if (handled) return;

        base.WndProc(ref m);
    }

    /// <summary>
    /// Processes default Windows messages for the form.
    /// </summary>
    /// <param name="m">The Windows message.</param>
    protected override void DefWndProc(ref Message m)
    {
        //var handled = OnDefWindowProc?.Invoke(ref m) ?? false;

        //if (handled) return;

        base.DefWndProc(ref m);
    }

    /// <summary>
    /// Handles the resize event and updates the resizer visibility.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        PerformResizerVisiblity();

    }

    /// <summary>
    /// The standard windowed style.
    /// </summary>
    const WINDOW_STYLE WINDOWED_STYLE = WINDOW_STYLE.WS_OVERLAPPEDWINDOW;

    /// <summary>
    /// The borderless style used when extending content into the title bar.
    /// </summary>
    const WINDOW_STYLE BORDERLESS_STYLE = WINDOW_STYLE.WS_OVERLAPPED | WINDOW_STYLE.WS_THICKFRAME | WINDOW_STYLE.WS_CAPTION | WINDOW_STYLE.WS_SYSMENU | WINDOW_STYLE.WS_MINIMIZEBOX | WINDOW_STYLE.WS_MAXIMIZEBOX;

    /// <summary>
    /// The style used for fullscreen windows.
    /// </summary>
    const WINDOW_STYLE FULL_SCREEN_STYLE = WINDOW_STYLE.WS_POPUP | WINDOW_STYLE.WS_SYSMENU | WINDOW_STYLE.WS_MINIMIZEBOX;

    /// <summary>
    /// The style used for popup windows.
    /// </summary>
    const WINDOW_STYLE POPUP_STYLE = WINDOW_STYLE.WS_POPUP | WINDOW_STYLE.WS_SYSMENU | WINDOW_STYLE.WS_MINIMIZEBOX | WINDOW_STYLE.WS_MAXIMIZEBOX;

    private bool _resizable = true;

    private bool _shadowDecorated = true;

    private bool _isPopup = false;

    private bool _fullscreen = false;

    private bool _extendsContentIntoTitleBar = false;

    private bool _systemMenu = true;

    private WebView2BorderlessResizer _windowBorderResizer;

    private Color? _backColor = null;

    private MARGINS[] SHADOW_DECORATORS = [
            new MARGINS(){ cxLeftWidth = 0, cxRightWidth = 0, cyTopHeight = 0, cyBottomHeight = 0 },
            new MARGINS(){ cxLeftWidth = 0, cxRightWidth = 0, cyTopHeight = 1, cyBottomHeight = 0 }
        ];

    private WINDOWPLACEMENT? _wpPrev;

    bool _shouldPatchBoundsSize = false;

    SystemBackdropType _systemBackdropType = SystemBackdropType.Auto;

    /// <summary>
    /// Defines bitmask values for hit test regions in the non-client area.
    /// </summary>
    private enum HitTestNCARegionMask : byte
    {
        client = 0b0000,
        left = 0b0001,
        right = 0b0010,
        top = 0b0100,
        bottom = 0b1000,
    }

    #region Resizer of borderless window
    /// <summary>
    /// Provides a transparent control for handling borderless window resizing.
    /// </summary>
    private class WebView2BorderlessResizer : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebView2BorderlessResizer"/> class.
        /// </summary>
        public WebView2BorderlessResizer()
        {
            Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Gets or sets the border offset for resizing.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Padding BorderOffset
        {
            get => _borders;
            set
            {
                if (_borders == value) return;

                _borders = value;

                OnResize(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Performs hit testing for the resizer control.
        /// </summary>
        /// <param name="lParam">The lParam from the message, containing the cursor position.</param>
        /// <returns>The hit test result code.</returns>
        internal uint HitTestNCA(nint lParam)
        {
            var cursor = MARCOS.ToPoint(lParam);

            var border = new Point(GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXFRAME)/* + GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXPADDEDBORDER)*/, GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYFRAME) /*+ GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXPADDEDBORDER)*/);


            if (!GetWindowRect((HWND)Parent!.Handle, out var windowRect))
            {
                return HTNOWHERE;
            }

            ClientToScreen((HWND)Handle, ref cursor);

            var result =
                (byte)HitTestNCARegionMask.left * (cursor.X >= (windowRect.left + DpiScaledBorderOffset.Left) && cursor.X < (windowRect.left + border.X + DpiScaledBorderOffset.Left) ? 1 : 0) |
                (byte)HitTestNCARegionMask.right * (cursor.X >= (windowRect.right - border.X - DpiScaledBorderOffset.Right) && cursor.X <= (windowRect.right - DpiScaledBorderOffset.Right) ? 1 : 0) |
                (byte)HitTestNCARegionMask.top * (cursor.Y >= (windowRect.top + DpiScaledBorderOffset.Top) && cursor.Y < (windowRect.top + border.Y + DpiScaledBorderOffset.Top) ? 1 : 0) |
                (byte)HitTestNCARegionMask.bottom * (cursor.Y >= (windowRect.bottom - border.Y - DpiScaledBorderOffset.Bottom) && cursor.Y <= (windowRect.bottom - DpiScaledBorderOffset.Bottom) ? 1 : 0);

            return result switch
            {
                (byte)HitTestNCARegionMask.left => HTLEFT,
                (byte)HitTestNCARegionMask.right => HTRIGHT,
                (byte)HitTestNCARegionMask.top => HTTOP,
                (byte)HitTestNCARegionMask.bottom => HTBOTTOM,
                (byte)(HitTestNCARegionMask.top | HitTestNCARegionMask.left) => HTTOPLEFT,
                (byte)(HitTestNCARegionMask.top | HitTestNCARegionMask.right) => HTTOPRIGHT,
                (byte)(HitTestNCARegionMask.bottom | HitTestNCARegionMask.left) => HTBOTTOMLEFT,
                (byte)(HitTestNCARegionMask.bottom | HitTestNCARegionMask.right) => HTBOTTOMRIGHT,
                (byte)HitTestNCARegionMask.client => HTCLIENT,
                _ => HTNOWHERE,
            };
        }

        /// <summary>
        /// Gets the parameters required to create the resizer control, including extended styles.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;

                cp.ExStyle |= (int)(WINDOW_EX_STYLE.WS_EX_TRANSPARENT | WINDOW_EX_STYLE.WS_EX_TOPMOST | WINDOW_EX_STYLE.WS_EX_NOACTIVATE);

                SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                SetStyle(ControlStyles.Opaque, true);

                return cp;
            }
        }

        /// <summary>
        /// Handles the resize event and updates the drag region.
        /// </summary>
        /// <param name="eventargs">The event arguments.</param>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);


            var parentHandle = Parent?.Handle ?? 0;

            if (parentHandle == 0) return;

            SetWindowPos((HWND)Handle, (HWND)parentHandle, 0, 0, 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOSIZE);

            CreateDragRegion();

        }

        /// <summary>
        /// Processes Windows messages for the resizer control, handling mouse hit testing and resizing.
        /// </summary>
        /// <param name="m">The Windows message.</param>
        protected override void WndProc(ref Message m)
        {
            var msg = (uint)m.Msg;

            switch (msg)
            {
                case WM_MOUSEMOVE:
                    {
                        var region = HitTestNCA(m.LParam);

                        if (region == HTNOWHERE || region == HTCLIENT)
                        {
                            Cursor = Cursors.Default;
                            break;
                        }


                        switch (region)
                        {
                            case HTTOP:
                            case HTBOTTOM:
                                Cursor = Cursors.SizeNS;
                                break;
                            case HTLEFT:
                            case HTRIGHT:
                                Cursor = Cursors.SizeWE;
                                break;
                            case HTTOPLEFT:
                            case HTBOTTOMRIGHT:
                                Cursor = Cursors.SizeNWSE;
                                break;
                            case HTTOPRIGHT:
                            case HTBOTTOMLEFT:
                                Cursor = Cursors.SizeNESW;
                                break;
                        }
                    }
                    return;
                case WM_LBUTTONDOWN:
                    {
                        var hittest = HitTestNCA(m.LParam);

                        if (hittest == HTNOWHERE || hittest == HTCLIENT) break;

                        PostMessage((HWND)Parent!.Handle, (uint)WM_NCLBUTTONDOWN, hittest, m.LParam);
                    }
                    return;
            }

            base.WndProc(ref m);
        }

        Padding _borders = Padding.Empty;

        /// <summary>
        /// Gets the DPI-scaled border offset.
        /// </summary>
        private Padding DpiScaledBorderOffset
        {
            get
            {
                var scaleFactor = DeviceDpi / 96f;
                return new Padding((int)(BorderOffset.Left * scaleFactor), (int)(BorderOffset.Top * scaleFactor), (int)(BorderOffset.Right * scaleFactor), (int)(BorderOffset.Bottom * scaleFactor));
            }
        }

        /// <summary>
        /// Creates the drag region for the resizer control.
        /// </summary>
        private void CreateDragRegion()
        {
            var border = new Point(GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXFRAME) + GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXPADDEDBORDER), GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYFRAME) + GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXPADDEDBORDER));

            var windowRect = new Region(ClientRectangle);


            if (BorderOffset.All != 0)
            {
                var borderRect = new Rectangle(ClientRectangle.Left + DpiScaledBorderOffset.Left, ClientRectangle.Top + DpiScaledBorderOffset.Top, ClientRectangle.Width - DpiScaledBorderOffset.Horizontal, ClientRectangle.Height - DpiScaledBorderOffset.Vertical);

                windowRect.Intersect(borderRect);
            }

            var excludedRect = new Rectangle(border.X + DpiScaledBorderOffset.Left, border.Y + DpiScaledBorderOffset.Top, ClientRectangle.Width - border.X * 2 - DpiScaledBorderOffset.Horizontal, ClientRectangle.Height - border.Y * 2 - DpiScaledBorderOffset.Vertical);

            windowRect.Exclude(excludedRect);

            Region = windowRect;

        }
    }

    #endregion

    #region WindowComposition
    /// <summary>
    /// Provides window accent composition (blur/acrylic) for legacy Windows versions.
    /// </summary>
    private class WindowAccentCompositor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowAccentCompositor"/> class.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        /// <param name="isAcrylic">Whether to use acrylic blur.</param>
        public WindowAccentCompositor(nint windowHandle, bool isAcrylic = false)
        {
            _handle = windowHandle;
            _isAcrylic = isAcrylic;
        }

        /// <summary>
        /// Applies the accent composition with the specified color.
        /// </summary>
        /// <param name="color">The color to use for the accent.</param>
        public void Composite(Color color)
        {
            var gradientColor = color.R | (color.G << 8) | (color.B << 16) | (color.A << 24);
            Composite(_handle, gradientColor);
        }

        private readonly bool _isAcrylic;
        nint _handle;

        /// <summary>
        /// Accent state for the window composition.
        /// </summary>
        private enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT,
            ACCENT_ENABLE_TRANSPARENTGRADIENT,
            ACCENT_ENABLE_BLURBEHIND,
            ACCENT_ENABLE_ACRYLICBLURBEHIND,
            ACCENT_INVALID_STATE
        }

        /// <summary>
        /// Window composition attribute for accent policy.
        /// </summary>
        private enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        /// <summary>
        /// Applies the accent composition to the window.
        /// </summary>
        /// <param name="handle">The window handle.</param>
        /// <param name="color">The gradient color value.</param>
        private void Composite(IntPtr handle, int color)
        {
            var accent = new AccentPolicy { AccentState = _isAcrylic ? AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND : AccentState.ACCENT_ENABLE_BLURBEHIND, GradientColor = color };
            var accentPolicySize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentPolicySize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            try
            {
                var data = new WindowCompositionAttributeData
                {
                    Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                    SizeOfData = accentPolicySize,
                    Data = accentPtr
                };
                SetWindowCompositionAttribute(handle, ref data);
            }
            finally
            {
                Marshal.FreeHGlobal(accentPtr);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }
    }
    #endregion

    /// <summary>
    /// Initializes the form's components and default properties.
    /// </summary>
    private void InitializeComponent()
    {
        this.SuspendLayout();
        // 
        // BrowserHostForm
        // 
        this.ClientSize = new System.Drawing.Size(960, 640);
        this.Name = "WinFormedgeForm";
        this.Text = "WinFormedge";
        this.AutoScaleMode = AutoScaleMode.Dpi;
        this.BackColor = Color.Transparent;
        this.ResumeLayout(false);
    }

    /// <summary>
    /// Assigns the owner form from the specified <see cref="IWin32Window"/> handle.
    /// </summary>
    /// <param name="owner">The owner window.</param>
    private void AssignOwnerFromHandle(IWin32Window? owner)
    {
        if (owner is not null)
        {
            var forms = Application.OpenForms.Cast<Form>();

            var ownerForm = forms.SingleOrDefault(x => x.Handle == owner.Handle);

            Owner = ownerForm;
        }
    }

    /// <summary>
    /// Corrects the window position and size for DPI and screen bounds.
    /// </summary>
    /// <param name="rawSize">The original size before correction.</param>
    private void CorrectWindowPos(Size rawSize)
    {
        var screen = Screen.FromPoint(MousePosition);
        var width = rawSize.Width;
        var height = rawSize.Height;

        var x = Location.X;
        var y = Location.Y;

        if (DeviceDpi != 96)
        {
            var scaleFactor = DeviceDpi / 96f;


            width = (int)(width * scaleFactor);
            height = (int)(height * scaleFactor);



            if (width > screen.WorkingArea.Width)
            {
                width = screen.WorkingArea.Width - (GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXFRAME) + GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXPADDEDBORDER)) * 2;
            }

            if (height > screen.WorkingArea.Height)
            {
                height = screen.WorkingArea.Height - (GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYFRAME) + GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXPADDEDBORDER)) * 2;
            }

            x = (int)(Location.X / scaleFactor);
            y = (int)(Location.Y / scaleFactor);

            if (!screen.WorkingArea.Contains(new Rectangle(x, y, width, height)))
            {
                x = screen.Bounds.X + (screen.WorkingArea.Width - width) / 2;
                y = screen.Bounds.Y + (screen.WorkingArea.Height - height) / 2;
            }



        }

        Location = new Point(x, y);

        Size = new Size(width, height);



        if (IsMaximized((HWND)Handle) || WindowState == FormWindowState.Maximized) return;


        if (StartPosition == FormStartPosition.CenterScreen || (StartPosition == FormStartPosition.CenterParent && Owner is null))
        {
            var screenWidth = screen.WorkingArea.Width;
            var screenHeight = screen.WorkingArea.Height;
            Location = new Point(screen.Bounds.X + (screenWidth - Size.Width) / 2, screen.Bounds.Y + (screenHeight - Size.Height) / 2);
        }
        else if (StartPosition == FormStartPosition.CenterParent && Owner is not null)
        {

            Location = new Point(Owner.Left + (Owner.Width - Size.Width) / 2, Owner.Top + (Owner.Height - Size.Height) / 2);

        }
    }

    /// <summary>
    /// Handles changes to the window style and updates the window accordingly.
    /// </summary>
    private void HandleWindowStyleChanged()
    {
        if (!IsHandleCreated) return;

        var oldStyle = (WINDOW_STYLE)GetWindowLong((HWND)Handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        var newStyle = RecreatingHandle ? oldStyle : GetWindowStyle();


        //if (newStyle == oldStyle) return;

        if (_wpPrev.HasValue)
        {
            var wpPrev = _wpPrev.Value;

            if (wpPrev.showCmd == SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED)
            {
                newStyle |= WINDOW_STYLE.WS_MAXIMIZE;
            }
            else if (wpPrev.showCmd == SHOW_WINDOW_CMD.SW_SHOWMINIMIZED)
            {
                newStyle &= ~WINDOW_STYLE.WS_MINIMIZE;
            }
        }


        SetWindowLong((HWND)Handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)newStyle);

        if (!Popup)
        {
            DwmExtendFrameIntoClientArea((HWND)Handle, SHADOW_DECORATORS[ShadowDecorated ? 1 : 0]);
        }


        SetWindowPos((HWND)Handle, HWND.Null, 0, 0, 0, 0, SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE);



        ShowWindow(hWnd, _wpPrev?.showCmd ?? SHOW_WINDOW_CMD.SW_SHOW);

        PerformResizerVisiblity();
    }

    /// <summary>
    /// Handles entering or exiting fullscreen mode.
    /// </summary>
    /// <param name="fullScreen">True to enter fullscreen, false to exit.</param>
    private void HandleFullScreen(bool fullScreen)
    {
        if (!IsHandleCreated) return;

        if (WindowState == FormWindowState.Minimized)
        {

            if (_wpPrev?.showCmd == SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }
        }

        if (fullScreen)
        {
            var newStyle = GetWindowStyle();

            var screen = Screen.FromHandle(Handle);
            var rect = screen.Bounds;
            WINDOWPLACEMENT wp = default;

            GetWindowPlacement((HWND)Handle, ref wp);
            SetWindowLong((HWND)Handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)newStyle);
            SetWindowPos((HWND)Handle, HWND.HWND_TOP, rect.Left, rect.Top, rect.Width, rect.Height, SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER | SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED);

            _wpPrev = wp;
        }
        else
        {
            HandleWindowStyleChanged();
            if (_wpPrev.HasValue)
            {
                SetWindowPlacement((HWND)Handle, _wpPrev.Value);
            }

            SetWindowPos((HWND)Handle, HWND.Null, 0, 0, 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER | SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED);
        }

        PerformResizerVisiblity();
    }

    /// <summary>
    /// Updates the visibility of the borderless resizer control based on window state.
    /// </summary>
    private void PerformResizerVisiblity()
    {
        if (_windowBorderResizer is null || !IsHandleCreated || RecreatingHandle) return;


        if (Resizable && !Fullscreen && (ExtendsContentIntoTitleBar || Popup) && WindowState == FormWindowState.Normal)
        {
            Controls.Add(_windowBorderResizer);
            _windowBorderResizer.Visible = true;
            _windowBorderResizer.BringToFront();
        }
        else
        {
            _windowBorderResizer.Visible = false;
            Controls.Remove(_windowBorderResizer);
        }
    }

    /// <summary>
    /// Determines whether the specified window is maximized.
    /// </summary>
    /// <param name="hwnd">The window handle.</param>
    /// <returns>True if maximized; otherwise, false.</returns>
    private bool IsMaximized(HWND hwnd)
    {
        WINDOWPLACEMENT placement = new();
        if (!GetWindowPlacement(hwnd, ref placement))
        {
            return false;
        }

        if (placement.showCmd == SHOW_WINDOW_CMD.SW_SHOWMINIMIZED)
        {
            _screenBeforeMinimized = Screen.FromHandle(hwnd);
        }


        return placement.showCmd == SHOW_WINDOW_CMD.SW_MAXIMIZE;
    }

    /// <summary>
    /// Stores the screen before the window is minimized, used to restore the maximized client rectangle.
    /// </summary>
    Screen? _screenBeforeMinimized;

    /// <summary>
    /// Adjusts the maximized client rectangle for custom borderless windows.
    /// </summary>
    /// <param name="hwnd">The window handle.</param>
    /// <param name="rect">The rectangle to adjust.</param>
    /// <returns>True if adjusted; otherwise, false.</returns>
    private bool AdjustMaximizedClientRect(HWND hwnd, ref RECT rect)
    {
        if (!IsMaximized(hwnd)) return false;

        Screen screen;

        if (_screenBeforeMinimized is not null)
        {
            screen = _screenBeforeMinimized;
            _screenBeforeMinimized = null;
        }
        else
        {
            screen = Screen.FromHandle(Handle);
        }

        if (screen is null) return false;

        rect = screen.WorkingArea;

        return true;
    }

    /// <summary>
    /// Gets the current window style based on the form's properties.
    /// </summary>
    /// <returns>The <see cref="WINDOW_STYLE"/> value.</returns>
    private WINDOW_STYLE GetWindowStyle()
    {
        var style = Fullscreen ? FULL_SCREEN_STYLE : Popup ? POPUP_STYLE : ExtendsContentIntoTitleBar ? BORDERLESS_STYLE : WINDOWED_STYLE;

        if (!MaximizeBox)
        {
            style &= ~WINDOW_STYLE.WS_MAXIMIZEBOX;
        }

        if (!MinimizeBox)
        {
            style &= ~WINDOW_STYLE.WS_MINIMIZEBOX;
        }

        if (!Resizable)
        {
            style &= ~WINDOW_STYLE.WS_THICKFRAME;
        }

        if (!SystemMenu)
        {
            style &= ~WINDOW_STYLE.WS_SYSMENU;
        }

        return style;
    }

    /// <summary>
    /// Adjusts the window rectangle for the specified style and DPI.
    /// </summary>
    /// <param name="rect">The rectangle to adjust.</param>
    /// <param name="style">The window style.</param>
    /// <param name="exStyle">The extended window style.</param>
    private void AdjustWindowRect(ref RECT rect, WINDOW_STYLE style, WINDOW_EX_STYLE exStyle)
    {
        if (DeviceDpi != 96 && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 14393))
        {
            AdjustWindowRectExForDpi(ref rect, style, false, exStyle, (uint)DeviceDpi);
        }
        else
        {
            AdjustWindowRectEx(ref rect, style, false, exStyle);
        }

    }
    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    //internal WindowProc? OnWindowProc { get; set; }

    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    //internal WindowProc? OnDefWindowProc { get; set; }

    //public new void CenterToParent() => base.CenterToParent();
    //public new void CenterToScreen() => base.CenterToScreen();

    /// <summary>
    /// Handles changes to the system backdrop type and applies the appropriate effect.
    /// </summary>
    /// <param name="value">The new <see cref="SystemBackdropType"/> value.</param>
    private void HandleSystemBackdropTypeChanged(SystemBackdropType value)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        if (value != _systemBackdropType)
        {
            _systemBackdropType = value;
            RecreateHandle();
        }

        if (value == SystemBackdropType.BlurBehind)
        {
            WindowAccentCompositor compositor = new(Handle);
            compositor.Composite(Color.FromArgb(0, 255, 255, 255));
            _systemBackdropType = value;
            return;
        }

        if (value == SystemBackdropType.Acrylic)
        {
            WindowAccentCompositor compositor = new(Handle, true);
            var mode = WinFormedgeApp.Current.GetSystemColorMode();

            if (_backColor != null)
            {
                compositor.Composite(_backColor.Value);
            }
            else
            {
                if (mode == SystemColorMode.Light)
                {
                    compositor.Composite(Color.FromArgb(0, 255, 255, 255));
                }
                else
                {
                    compositor.Composite(Color.FromArgb(60, 0, 0, 0));
                }
            }

            _systemBackdropType = value;
            return;
        }


        var systemBackdropType = _systemBackdropType switch
        {
            SystemBackdropType.None => DWM_SYSTEMBACKDROP_TYPE.DWMSBT_NONE,
            SystemBackdropType.Mica => DWM_SYSTEMBACKDROP_TYPE.DWMSBT_MAINWINDOW,
            SystemBackdropType.Transient => DWM_SYSTEMBACKDROP_TYPE.DWMSBT_TRANSIENTWINDOW,
            SystemBackdropType.MicaAlt => DWM_SYSTEMBACKDROP_TYPE.DWMSBT_TABBEDWINDOW,
            _ => DWM_SYSTEMBACKDROP_TYPE.DWMSBT_AUTO
        };

        unsafe
        {
            DwmSetWindowAttribute((HWND)Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, &systemBackdropType, sizeof(DWM_SYSTEMBACKDROP_TYPE));
        }

        _systemBackdropType = value;

    }
}

