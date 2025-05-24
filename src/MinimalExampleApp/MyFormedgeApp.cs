using WinFormedge;

namespace MinimalExampleApp;

internal class MyFormedgeApp : AppStartup
{
    protected override AppCreationAction? OnApplicationStartup(StartupSettings options)
    {
        var t = new ExcludedEdgesWindow();
        t.ShowDialog();
        return options.UseMainWindow(new FeaturesWindow());
    }
}