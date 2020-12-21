using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.ViewModels.Controls.ContentTab;

namespace Regul.Views.Controls.ContentTab
{
    public class TheSims3TypeContent : UserControl
    {
        public TheSims3TypeContentViewModel ViewModel { get; set; } = new();
        
        public TheSims3TypeContent()
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
