using Avalonia;

namespace Regul.ViewModels.Windows
{
    internal class HEXNumberConverterViewModel : ViewModelBase
    {
        private string _hexText;
        private string _decimalText;

        public string HEXText
        {
            get => _hexText;
            set => RaiseAndSetIfChanged(ref _hexText, value);
        }
        public string DecimalText
        {
            get => _decimalText;
            set => RaiseAndSetIfChanged(ref _decimalText, value);
        }

        private void CloseWindow() => App.HEXNumberConverter.Close();

        private void Copy(string str) => Application.Current.Clipboard.SetTextAsync(str);
    }
}
