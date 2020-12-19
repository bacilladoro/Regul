using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Regul.Views.Windows
{
    public class About : Window
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
