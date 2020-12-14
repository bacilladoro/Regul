using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Regul.ViewModels;
using Regul.Views;
using System;

namespace Regul
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().Start(AppMain, args);
        }

        private static AppBuilder BuildAvaloniaApp() => 
                AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI()
                .With(new Win32PlatformOptions { AllowEglInitialization = false, UseDeferredRendering = true, OverlayPopups = false })
                .With(new MacOSPlatformOptions { ShowInDock = true })
                .With(new AvaloniaNativePlatformOptions { UseGpu = true, UseDeferredRendering = true, OverlayPopups = false })
                .With(new X11PlatformOptions { UseGpu = true, UseEGL = true });

        private static void AppMain(Application app, string[] args)
        {
            App.MainWindowViewModel = new MainWindowViewModel();

            App.MainWindow = new MainWindow { DataContext = App.MainWindowViewModel };

            app.Run(App.MainWindow);
        }
    }
}
