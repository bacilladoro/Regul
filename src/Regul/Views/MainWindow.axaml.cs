using System;
using Avalonia.Markup.Xaml;
using Regul.OlibUI;
using Regul.ViewModels;

namespace Regul.Views
{
    public class MainWindow : OlibMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs e) => ((MainWindowViewModel)DataContext).Exit();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
