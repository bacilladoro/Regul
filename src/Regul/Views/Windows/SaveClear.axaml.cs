﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.OlibUI;

namespace Regul.Views.Windows
{
    public class SaveClear : OlibModalWindow
    {
        public SaveClear()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
