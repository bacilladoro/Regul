using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.OlibUI;

namespace Regul.Views.Windows
{
    public class SelectType : OlibModalWindow
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
