using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Regul.Views.Windows
{
    public class SaveClear : Window
    {
        public SaveClear()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
