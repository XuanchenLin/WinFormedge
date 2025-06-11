// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

using WinFormedge;

namespace MinimalExampleApp;
public class MainWindow : Formedge
{
//#if DEBUG
//    public const string DEMO_HOST_ADDR = "http://127.0.0.1:8080";
//#else
//    public const string DEMO_HOST_ADDR = "https://localresources.app";
//#endif

    public const string DEMO_HOST_ADDR = "https://localresources.app";

    static Icon ExampleIcon = new Icon(new MemoryStream(Properties.Resources.ExampleIcon));

    public MainWindow()
    {


        Url = $"{DEMO_HOST_ADDR}/home/";
        StartPosition = FormStartPosition.CenterScreen;
        Icon = ExampleIcon;
        AllowFullscreen = true;
        Load += MainWindow_Load;

        Load += (_, _) =>
        {
            // Set the JavaScript object for the main window
            if(CoreWebView2 is not null)
            {
                CoreWebView2.AddHostObjectToScript("mainWindow", new MainWindowJsObject(this));

                SetVirtualHostNameToEmbeddedResourcesMapping(new WinFormedge.WebResource.EmbeddedFileResourceOptions
                {
                    Scheme = "https",
                    HostName = "localresources.app",
                    ResourceAssembly = typeof(StartupWindow).Assembly,
                    DefaultFolderName = "wwwroot"
                });
            }
        };
    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var win = opts.UseDefaultWindow();
        MinimumSize = new Size(1024, 640);
        Size = new Size(1280, 800);
        win.ExtendsContentIntoTitleBar = true;

        return win;
    }

    private void MainWindow_Load(object? sender, EventArgs e)
    {
    }
}

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class MainWindowJsObject
{
    static Icon DVAIcon = new Icon(new MemoryStream(Properties.Resources.DVAIcon));
    static Icon BrowserIcon = new Icon(new MemoryStream(Properties.Resources.BrowserIcon));
    static Icon ColorsIcon = new Icon(new MemoryStream(Properties.Resources.ColorsIcon));
    static Icon MatrixIcon = new Icon(new MemoryStream(Properties.Resources.MatrixIcon));


    private readonly MainWindow _mainWindow;
    private List<Formedge> _demoWindows { get; } = new();

    private class KioskDemoWindow : Formedge
{
        public KioskDemoWindow()
        {
            Url = $"{MainWindow.DEMO_HOST_ADDR}/home/window-styles/kiosk/";
            BackColor = Color.Black;
            Icon = MatrixIcon;
            SetVirtualHostNameToEmbeddedResourcesMapping(new WinFormedge.WebResource.EmbeddedFileResourceOptions
            {
                Scheme = "https",
                HostName = "localresources.app",
                ResourceAssembly = typeof(StartupWindow).Assembly,
                DefaultFolderName = "wwwroot"
            });
        }
        protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
        {
            var settings = opts.UseKisokWindow();
            settings.Resizable = false;
            return settings;
        }
    }


    public MainWindowJsObject(MainWindow mainWindow) 
    {
        _mainWindow = mainWindow;
    }

    private KioskDemoWindow? _kioskDemoWindow;

    public void CreateKioskWindow()
    {
        MessageBox.Show(_mainWindow, "Kiosk 模式下窗口将无法被最小化、最大化或关闭，请使用 ALT + F4 关闭示例窗口。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        _kioskDemoWindow = new KioskDemoWindow
        {
            WindowTitle = "Kiosk Window Demo"
        };
        _kioskDemoWindow.Show();
    }

    public void CreateDemoWindow(string style, string backdrop, string page)
    {
        if (style == "NonDecorated" && !OperatingSystem.IsWindowsVersionAtLeast(8, 0))
        {
            MessageBox.Show(_mainWindow, "非装饰窗口样式仅在 Windows 8 及以上版本支持。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }


        if (!OperatingSystem.IsWindowsVersionAtLeast(8, 0) && backdrop != "Auto")
        {
            MessageBox.Show(_mainWindow, "Windows 7 无法使用背景类型选项。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        string[] win11Supports = ["Mica", "MicaAlt", "Transient"];

        if (!OperatingSystem.IsWindowsVersionAtLeast(10,0, 22000) && win11Supports.Contains(backdrop))
        {
            MessageBox.Show(_mainWindow, "选中的背景类型选项需要 Windows 11 版本。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }



        if (page == "ShapedTransparentPage" && _demoWindows.Any(x => x.WindowTitle.StartsWith("Shaped")))
        {
            MessageBox.Show(_mainWindow, "鉴于性能问题，每次仅能创建一个异性窗体。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var demoWin = new DemoWindow(_mainWindow, style, backdrop, page);

        demoWin.FormClosed += (_, _) =>
        {
            _demoWindows.Remove(demoWin);
        };

        _demoWindows.Add(demoWin);

        demoWin.Show(_mainWindow);
    }

    public async void CloseAllDemoWindows()
    {
        while (_demoWindows.Count > 0)
        {
            var window = _demoWindows[_demoWindows.Count - 1];
            _demoWindows.RemoveAt(_demoWindows.Count - 1);
            if (window is null || window.IsDisposed) continue;
            window.Close();
            window.Dispose();

            await Task.Delay(100);
        }

        GC.Collect();
    }

    private class DemoWindow : Formedge
    {
        private enum WindowStyle
        {
            ExtendsContentIntoTitlebar,
            NonDecorated,
            Default
        }

        private WindowStyle _style;
        private SystemBackdropType _backdropType;
        private bool _isResizable = true;
        private readonly MainWindow _mainWindow;

        public DemoWindow(MainWindow mainWindow, string? style, string? backdrop, string? page)
        {
            _mainWindow = mainWindow;




            if (Enum.TryParse<SystemBackdropType>(backdrop, true, out var backdropType))
            {
                _backdropType = backdropType;
            }
            else
            {
                _backdropType = SystemBackdropType.Auto;
            }

            WindowTitle = "Window Style Demo";


            switch (style)
            {
                case "ExtendsContentIntoTitlebar":
                    _style = WindowStyle.ExtendsContentIntoTitlebar;

                    break;

                case "NonDecorated":
                    _style = WindowStyle.NonDecorated;
                    break;

                default:
                    _style = WindowStyle.Default;
                    break;
            }

            var url = "https://www.bing.com/";

            switch (page)
            {
                case "BorderlessPage":
                    url = $"{MainWindow.DEMO_HOST_ADDR}/home/window-styles/default/borderless.html";

                    Size = new Size(1280, 960);
                    MinimumSize = new Size(1024, 640);
                    Icon = DVAIcon;

                    break;

                case "TransparentPage":
                    url = $"{MainWindow.DEMO_HOST_ADDR}/home/window-styles/default/transparent.html";

                    WindowTitle = "Transparent Window";

                    MinimumSize = Size = new Size(720, 640);

                    Icon = ColorsIcon;


                    break;

                case "ShapedTransparentPage":
                    url = $"{MainWindow.DEMO_HOST_ADDR}/home/window-styles/default/shaped.html";

                    WindowTitle = "Shaped Transparent Window";

                    Size = new Size(564, 564);

                    StartPosition = FormStartPosition.CenterParent;

                    _isResizable = false;
                    Icon = BrowserIcon;


                    break;

                default:

                    Icon = BrowserIcon;

                    ShowDocumentTitle = true;

                    Size = new Size(1280, 800);

                    break;
            }

            if (_backdropType != SystemBackdropType.Auto)
            {
                BackColor = Color.Transparent;
            }

            Url = url;

            Load += DemoWindow_Load;
        }

        private void DemoWindow_Load(object? sender, EventArgs e)
        {
            SetVirtualHostNameToEmbeddedResourcesMapping(new WinFormedge.WebResource.EmbeddedFileResourceOptions
            {
                Scheme = "https",
                HostName = "localresources.app",
                ResourceAssembly = typeof(StartupWindow).Assembly,
                DefaultFolderName = "wwwroot"
            });
        }

        protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
        {
            var settings = opts.UseDefaultWindow();
            settings.ExtendsContentIntoTitleBar = _style == WindowStyle.ExtendsContentIntoTitlebar || _style == WindowStyle.NonDecorated;
            settings.ShowWindowDecorators = _style != WindowStyle.NonDecorated;
            settings.SystemBackdropType = _backdropType;
            settings.Resizable = _isResizable;

            if (!settings.ShowWindowDecorators && _isResizable)
            {
                settings.WindowEdgeOffsets = new Padding { Top = 15, Right = 20, Bottom = 25, Left = 20 };
            }

            return settings;
        }
    }


}