using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Regul.Views.Windows
{
    public class SelectType : Window
    {
        public SelectType()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
