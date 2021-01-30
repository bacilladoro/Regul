using System;
using Avalonia.Markup.Xaml;
using Regul.OlibUI;

namespace Regul.Views
{
    public class MainWindow : OlibMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs e) => App.MainWindowViewModel.Exit();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
