using Avalonia.Markup.Xaml;
using OlibUI.Windows;

namespace Regul.Views.Windows
{
    public class SaveCleaner : OlibWindow
    {
        public SaveCleaner()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
