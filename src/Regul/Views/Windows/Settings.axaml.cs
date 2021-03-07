using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OlibUI.Windows;
using Regul.ViewModels.Windows;

namespace Regul.Views.Windows
{
    public class Settings : OlibWindow
    {
        public Settings()
        {
            AvaloniaXamlLoader.Load(this);

            Closing += (sender, args) => ((SettingsViewModel) DataContext)?.Closing();
        }
    }
}
