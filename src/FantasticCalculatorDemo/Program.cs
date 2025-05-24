using WinFormedge;

namespace FantasticCalculatorDemo;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var app = WinFormedgeApp.CreateAppBuilder()
            .UseModernStyleScrollbar()
            .UseWinFormedgeApp<CalculatorApp>()
#if DEBUG
            .UseDevTools()
#endif
            .Build();

        app.Run();
    }
}