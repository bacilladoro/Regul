using Avalonia.Markup.Xaml;
using Regul.OlibUI;

namespace Regul.Views.Windows
{
    public class SaveCleaner : OlibModalWindow
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
