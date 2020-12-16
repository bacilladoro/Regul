using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.ViewModels.Controls.Tab;

namespace Regul.Views.Controls.Tab
{
    public class TabHeader : UserControl
    {
        public TabHeaderViewModel ViewModel { get; set; } = new();
        
        public TabHeader()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
