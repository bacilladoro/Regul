using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Regul.ViewModels;
using Regul.Views;
using System;
using System.IO;
using Regul.Core;
using Regul.Structures;
using System.Runtime.InteropServices;
using Avalonia.Dialogs;
using Regul.OlibStyle;

namespace Regul
{
    class Program
    {
        public static Settings Settings;

        [STAThread]
        public static void Main(string[] args)
        {
            Settings = File.Exists(AppDomain.CurrentDomain.BaseDirectory + "settings.json")
                ? FileSettings.LoadSettings()
                : new Settings();

            BuildAvaloniaApp().Start(AppMain, args);
        }

        private static AppBuilder BuildAvaloniaApp()
        {
            var result = AppBuilder.Configure<App>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                result
                    .UseWin32()
                    .UseSkia();
            }
            else
            {
                result.UsePlatformDetect()
                    .UseManagedSystemDialogs<AppBuilder, OlibMainWindow>();
            }
            return result
                .LogToTrace()
                .UseReactiveUI()
                .With(new Win32PlatformOptions { AllowEglInitialization = false, UseDeferredRendering = true, OverlayPopups = false })
                .With(new MacOSPlatformOptions { ShowInDock = true })
                .With(new AvaloniaNativePlatformOptions { UseGpu = true, UseDeferredRendering = true, OverlayPopups = false })
                .With(new X11PlatformOptions { UseGpu = true, UseEGL = true });
        }

        private static void AppMain(Application app, string[] args)
        {
            App.MainWindowViewModel = new MainWindowViewModel();

            App.MainWindow = new MainWindow { DataContext = App.MainWindowViewModel };

            app.Run(App.MainWindow);
        }
    }
}
