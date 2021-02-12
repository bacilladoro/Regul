namespace Regul.ViewModels.Windows
{
    internal class SelectTypeViewModel : ViewModelBase
    {
        private int _type;

        public int Type
        {
            get => _type;
            set => RaiseAndSetIfChanged(ref _type, value);
        }

        private void Exit()
        {
            App.SelectType.Close(true);
        }
    }
}
