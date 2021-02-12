using Avalonia.Media;
using System;

namespace Regul.ViewModels.Controls.Tab
{
    internal class TabHeaderViewModel : ViewModelBase
    {
        private string _nameTab;
        private bool _isSave;

        private Geometry _icon;

        public string ID { get; set; }
        public string PathPackage { get; set; }
        public int PackageType { get; set; }

        public Action<string> CloseTabAction;

        public string NameTab
        {
            get => _nameTab;
            set => RaiseAndSetIfChanged(ref _nameTab, value);
        }

        public bool IsSave
        {
            get => _isSave;
            set => RaiseAndSetIfChanged(ref _isSave, value);
        }

        public Geometry Icon
        {
            get => _icon;
            set => RaiseAndSetIfChanged(ref _icon, value);
        }

        private void CloseTab() => CloseTabAction?.Invoke(ID);
    }
}
