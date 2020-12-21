using ReactiveUI;

namespace Regul.ViewModels.Windows
{
    public class SelectTypeViewModel : ReactiveObject
    {
        private int _type;

        public int Type
        {
            get => _type;
            set => this.RaiseAndSetIfChanged(ref _type, value);
        }

        private void Exit()
        {
            App.SelectType.Close(true);
        }
    }
}
