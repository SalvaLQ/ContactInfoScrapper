using Avalonia;
using Serilog;
using System;

namespace AllContactInfoScrapper
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Error()
               .WriteTo.File(System.IO.Path.Combine(Infraestructure.Files.FilesUtils.CurretDirectory(),"log.txt"), rollingInterval: RollingInterval.Day)
               .CreateLogger();
            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}
