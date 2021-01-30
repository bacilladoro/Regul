using System;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Regul.ViewModels.Controls.Tab
{
    public class TabHeaderViewModel : ReactiveObject
    {
        private string _nameTab;
        private bool _isSave;

        private DrawingImage _icon;
        
        public string ID { get; set; }
        public string PathPackage { get; set; }
        public int PackageType { get; set; } 

        public Action<string> CloseTabAction;
        
        public string NameTab
        {
            get => _nameTab;
            set => this.RaiseAndSetIfChanged(ref _nameTab, value);
        }

        public bool IsSave
        {
            get => _isSave;
            set => this.RaiseAndSetIfChanged(ref _isSave, value);
        }

        public DrawingImage Icon
        {
            get => _icon;
            set => this.RaiseAndSetIfChanged(ref _icon, value);
        }

        private void CloseTab() => CloseTabAction?.Invoke(ID);
    }
}
