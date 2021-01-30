﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.OlibUI;

namespace Regul.Views.Windows
{
    public class About : OlibModalWindow
    {
        public About()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
