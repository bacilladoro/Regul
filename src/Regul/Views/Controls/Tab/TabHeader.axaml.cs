using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Regul.Views.Controls.Tab
{
    public class TabHeader : UserControl
    {   
        public TabHeader()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
