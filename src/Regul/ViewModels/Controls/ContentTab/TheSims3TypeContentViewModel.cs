using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Regul.ViewModels.Controls.ContentTab
{
    public class TheSims3TypeContentViewModel : ReactiveObject
    {
        private bool _openedMenu;

        public bool OpenedMenu
        {
            get => _openedMenu;
            set => this.RaiseAndSetIfChanged(ref _openedMenu, value);
        }
    }
}
