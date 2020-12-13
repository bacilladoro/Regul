using ReactiveUI;
using Regul.ViewModels.Windows;
using Regul.Views.Windows;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace Regul.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        #region ReactiveCommands

        private ReactiveCommand<Unit, Unit> ExitCommand { get; }
        private ReactiveCommand<Unit, Unit> SaveClearWindowCommand { get; }

        #endregion

        public MainWindowViewModel()
        {
            SaveClearWindowCommand = ReactiveCommand.Create(SaveClearWindow);
        }

        private void SaveClearWindow()
        {
            App.SaveClear = new SaveClear{ DataContext = new SaveClearViewModel() };
            App.SaveClear.ShowDialog(App.MainWindow);
        }
    }
}
