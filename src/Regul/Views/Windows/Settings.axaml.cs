using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.OlibStyle;
using Regul.ViewModels.Windows;

namespace Regul.Views.Windows
{
    public class Settings : OlibModalWindow
    {

        public Settings()
        {
            InitializeComponent();
            
            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            ((SettingsViewModel)DataContext)?.Exit();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
