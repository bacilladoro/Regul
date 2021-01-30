using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.OlibUI;
using Regul.ViewModels.Windows;

namespace Regul.Views.Windows
{
    public class Settings : OlibModalWindow
    {

        public Settings()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
