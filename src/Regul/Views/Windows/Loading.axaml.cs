using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Regul.Views.Windows
{
    public class Loading : Window
    {
        public ProgressBar Progress;
        public TextBlock ProcessText;

        public Loading()
        {
            this.InitializeComponent();

            Progress = this.FindControl<ProgressBar>("progress");
            ProcessText = this.FindControl<TextBlock>("processText");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
