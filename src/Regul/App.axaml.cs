using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Regul.ViewModels;
using Regul.Views;
using Regul.Views.Windows;

namespace Regul
{
    public class App : Application
    {
        public static MainWindow MainWindow { get; set; }
        public static MainWindowViewModel MainWindowViewModel { get; set; }

        public static SaveClear SaveClear { get; set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
