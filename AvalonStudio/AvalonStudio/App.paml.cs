using Avalonia;
using Avalonia.Threading;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using AvalonStudio.Packages;
using AvalonStudio.Platforms;
using AvalonStudio.Shell;
using Serilog;
using System;
using AvalonStudio.Extensibility.Studio;
using AvalonStudio.Extensibility;
using System.IO;
using AvalonStudio.Extensibility.Platforms.Terminals.Unix;
using AvalonStudio.Utils;
using AvalonStudio.Packaging;
using AvalonStudio.Extensibility.Platforms.Terminals.Unix;

namespace AvalonStudio
{
    internal class App : Application
    {
#if !DEBUG
        static void Print(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                Print(ex.InnerException);
            }
        }
#endif

        [STAThread]
        private static void Main(string[] args)
        {
            if(args.Length>0 && args[0] == "--terminal-trampoline")
            {
                UnixPsuedoTerminal.Trampoline();
            }

            UnixPsuedoTerminal.Test();
        
#if !DEBUG
        try
            {
#endif
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            BuildAvaloniaApp().BeforeStarting(_ =>
            {
                var studio = IoC.Get<IStudio>();

                InitializeLogging();

                Platform.Initialise();

                Dispatcher.UIThread.Post(async () =>
                   {
                       await PackageManager.LoadAssetsAsync().ConfigureAwait(false);
                       
                   });
            })
            .StartShellApp<AppBuilder, MainWindow>("AvalonStudio", null, () => new MainWindowViewModel());
#if !DEBUG
    }
            catch (Exception e)
            {
                Print(e);
            }
            finally
#endif
            {
                Application.Current.Exit();
            }
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            var result = AppBuilder.Configure<App>();

            if(Platform.PlatformIdentifier == Platforms.PlatformID.Win32NT)
            {
                result
                    .UseWin32()
                    .UseDirect2D1();
            }
            else
            {
                result.UsePlatformDetect();
            }

            return result;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private static void InitializeLogging()
        {
#if DEBUG
            SerilogLogger.Initialize(new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Trace(outputTemplate: "{Area}: {Message}")
                .CreateLogger());
#endif
        }
    }
}