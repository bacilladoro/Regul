using Avalonia;
using Avalonia.Controls;
using Regul.ViewModels;
using Regul.Views;
using System;
using System.IO;
using Regul.Core;
using Regul.Structures;
using System.Runtime.InteropServices;
using Avalonia.Dialogs;
using Regul.OlibUI;
using Avalonia.Rendering;
using System.Threading;
using System.Diagnostics;
using System.Text;
using Avalonia.OpenGL;
using System.Collections.Generic;

namespace Regul
{
    public class Program
    {
        private static readonly object Sync = new();

        public static Settings Settings;

        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Settings = File.Exists(AppDomain.CurrentDomain.BaseDirectory + "settings.json")
                ? FileSettings.LoadSettings()
                : new Settings();

            BuildAvaloniaApp().Start(AppMain, args);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "Log");
            if (!Directory.Exists(pathToLog)) Directory.CreateDirectory(pathToLog);

            if (e.ExceptionObject is Exception ex)
            {
                lock (Sync) File.AppendAllText(Path.Combine(pathToLog,
                            $"{AppDomain.CurrentDomain.FriendlyName}_{DateTime.Now:dd.MM.yyy}.log"),
                        $"[{DateTime.Now:dd.MM.yyy HH:mm:ss.fff}] | Fatal | [{ex.TargetSite.DeclaringType}.{ex.TargetSite.Name}()] {ex}\r\n",
                        Encoding.UTF8);
            }
        }

        private static AppBuilder BuildAvaloniaApp()
        {
            var result = AppBuilder.Configure<App>();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                result
                    .UseWin32()
                    .UseSkia()
                    .With(new AngleOptions { AllowedPlatformApis = new List<AngleOptions.PlatformApi> { AngleOptions.PlatformApi.DirectX11 } });

                if (DwmIsCompositionEnabled(out bool dwmEnabled) == 0 && dwmEnabled)
                {
                    var wp = result.WindowingSubsystemInitializer;
                    result.UseWindowingSubsystem(() =>
                    {
                        wp();
                        AvaloniaLocator.CurrentMutable.Bind<IRenderTimer>().ToConstant(new WindowsDWMRenderTimer());
                    });
                }
            }
            else
            {
                result.UsePlatformDetect()
                    .UseManagedSystemDialogs<AppBuilder, OlibMainWindow>();
            }
            return result
                .LogToTrace()
                .With(new Win32PlatformOptions { AllowEglInitialization = Settings.HardwareAcceleration, UseDeferredRendering = true, OverlayPopups = false, UseWgl = false })
                .With(new MacOSPlatformOptions { ShowInDock = true })
                .With(new AvaloniaNativePlatformOptions { UseGpu = Settings.HardwareAcceleration, UseDeferredRendering = true, OverlayPopups = false })
                .With(new X11PlatformOptions { UseGpu = Settings.HardwareAcceleration, UseEGL = true, OverlayPopups = false });
        }

        private static void AppMain(Application app, string[] args)
        {
            App.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };

            app.Run(App.MainWindow);
        }

        [DllImport("Dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled(out bool enabled);
    }

    public class WindowsDWMRenderTimer : IRenderTimer
    {
        public event Action<TimeSpan> Tick;
        private Thread _renderTick;

        public WindowsDWMRenderTimer()
        {
            _renderTick = new Thread(() =>
            {
                Stopwatch sw = new();
                sw.Start();
                while (true)
                {
                    DwmFlush();
                    Tick?.Invoke(sw.Elapsed);
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            _renderTick.Start();
        }

        [DllImport("Dwmapi.dll")]
        private static extern int DwmFlush();
    }
}
