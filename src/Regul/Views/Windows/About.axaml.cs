using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OlibUI.Windows;

namespace Regul.Views.Windows
{
    public class About : OlibWindow
    {
        public About()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
