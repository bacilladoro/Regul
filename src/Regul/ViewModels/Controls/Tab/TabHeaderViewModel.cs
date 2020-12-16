using System;
using ReactiveUI;

namespace Regul.ViewModels.Controls.Tab
{
    public class TabHeaderViewModel : ReactiveObject
    {
        private string _nameTab;
        
        public string ID { get; set; }
        public int PackageType { get; set; }

        public Action<string> CloseTabAction;
        
        public string NameTab
        {
            get => _nameTab;
            set => this.RaiseAndSetIfChanged(ref _nameTab, value);
        }

        private void CloseTab() => CloseTabAction?.Invoke(ID);
    }
}
