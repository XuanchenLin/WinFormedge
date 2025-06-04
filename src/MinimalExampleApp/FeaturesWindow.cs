using WinFormedge;
using WinFormedge.WebResource;

using Microsoft.Web.WebView2.Core;
using System.Reflection;

namespace MinimalExampleApp;

internal class FeaturesWindow : Formedge
{
    public FeaturesWindow()
    {
        MinimumSize = new Size(960, 480);
        Size = new Size(1280, 800);
        AllowFullscreen = true;
        BackColor = Color.Transparent;

        Load += MyWindow_Load;
        DOMContentLoaded += MyWindow_DOMContentLoaded;

        SetVirtualHostNameToEmbeddedResourcesMapping(new EmbeddedFileResourceOptions
        {
            Scheme = "https",
            HostName = "embedded.appresource.local",
            ResourceAssembly = Assembly.GetExecutingAssembly(),
            DefaultFolderName = "Resources\\wwwroot"
        });

        Url = "https://embedded.appresource.local";
        //Url = "https://cn.bing.com/";
        //Url = "https://www.douyin.com";
    }

    protected override WindowSettings ConfigureWindowSettings(HostWindowBuilder opts)
    {
        var win = opts.UseDefaultWindow();

        win.ExtendsContentIntoTitleBar = true;
        win.SystemBackdropType = SystemBackdropType.BlurBehind;

        return win;
    }

    private void MyWindow_Load(object? sender, EventArgs e)
    {
    }

    private void MyWindow_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
    {
        ExecuteScriptAsync(""""
(()=>{
const headerEl = document.querySelector("#hdr");
//headerEl.querySelectorAll(":scope>div").forEach(c=>c.style.appRegion="no-drag");
headerEl.style.appRegion="drag";
})();
"""");
    }

    protected override void ConfigureWebView2Settings(CoreWebView2Settings settings)
    {
        base.ConfigureWebView2Settings(settings);
    }
}