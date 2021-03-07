using System;
using Avalonia.Markup.Xaml;
using OlibUI.Windows;
using Regul.ViewModels;

namespace Regul.Views
{
    public class MainWindow : OlibWindow
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
