using Avalonia;
using ReactiveUI;
using System;

namespace Regul.ViewModels.Windows
{
    public class HEXNumberConverterViewModel : ReactiveObject
    {
        private string _hexText;
        private string _decimalText;

        public string HEXText
        {
            get => _hexText;
            set => this.RaiseAndSetIfChanged(ref _hexText, value);
        }
        public string DecimalText
        {
            get => _decimalText;
            set => this.RaiseAndSetIfChanged(ref _decimalText, value);
        }

        private void CloseWindow() => App.HEXNumberConverter.Close();

        private void Copy(string str) => Application.Current.Clipboard.SetTextAsync(str);
    }
}
