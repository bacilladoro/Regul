using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OlibUI.Windows;
using Regul.ViewModels.Windows;
using System;
using System.Reactive.Linq;

namespace Regul.Views.Windows
{
    public class HEXNumberConverter : OlibWindow
    {
        private TextBox HEXTextBox;
        private TextBox DecimalTextBox;

        public HEXNumberConverter()
        {
            InitializeComponent();

            HEXTextBox = this.FindControl<TextBox>("HEXTextBox");
            DecimalTextBox = this.FindControl<TextBox>("DecimalTextBox");

            HEXTextBox.GetObservable(TextBox.TextProperty).Subscribe(value =>
            {
                if (HEXTextBox.IsKeyboardFocusWithin)
                {
                    try
                    {
                        DecimalTextBox.Text = Convert.ToUInt32(value, value.StartsWith("0x") ? 16 : 10).ToString();
                    }
                    catch { }
                }
            });

            DecimalTextBox.GetObservable(TextBox.TextProperty).Subscribe(value =>
            {
                if (DecimalTextBox.IsKeyboardFocusWithin)
                {
                    try
                    {
                        HEXTextBox.Text = "0x" + long.Parse(value).ToString("X8");
                    }
                    catch { }
                }
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
