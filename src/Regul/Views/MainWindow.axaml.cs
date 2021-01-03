using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Regul.OlibStyle;
using Regul.ViewModels;

namespace Regul.Views
{
    public class MainWindow : OlibMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            Closed +=OnClosed;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            App.MainWindowViewModel.Exit();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
