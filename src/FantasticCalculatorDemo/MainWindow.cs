using WinFormedge;

namespace FantasticCalculatorDemo;
internal class MainCalculatorWindow : Formedge
{
    public MainCalculatorWindow()
    {
        WindowTitle = "计算器";
        Icon = Properties.Resources.AppIcon;
        Size = new Size(320, 480);
        BackColor = Color.Transparent;
        Maximizable = false;
        

        SetVirtualHostNameToEmbeddedResourcesMapping(new WinFormedge.WebResource.EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "embeddedassets.local",
            ResourceAssembly = typeof(MainCalculatorWindow).Assembly,
            WebResourceContext = Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All,
            DefaultFolderName = "wwwroot"
        });

        Url = "https://embeddedassets.local";

    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var win = opts.UseDefaultWindow();
        win.ExtendsContentIntoTitleBar = true;
        win.Resizable = false;
        win.SystemBackdropType = SystemBackdropType.BlurBehind;
        
        return win;
    }
}
