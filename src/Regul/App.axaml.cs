using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Regul.ViewModels;
using Regul.Views;
using Regul.Views.Windows;
using Regul.Views.Windows.TheSims3Type;

namespace Regul
{
    public class App : Application
    {
        public static MainWindow MainWindow { get; set; }
        public static Settings Settings { get; set; }
        public static SaveClear SaveClear { get; set; }
        public static About About { get; set; }
        public static SelectType SelectType { get; set; }
        public static HEXNumberConverter HEXNumberConverter { get; set; }
        public static ResourceDetails ResourceDetails { get; set; }

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
