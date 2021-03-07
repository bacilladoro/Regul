using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OlibUI.Windows;

namespace Regul.Views.Windows
{
    public class SelectType : OlibWindow
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
