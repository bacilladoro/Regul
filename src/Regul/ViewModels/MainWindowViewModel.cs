using ReactiveUI;
using Regul.ViewModels.Windows;
using Regul.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Text;
using Avalonia.Controls;
using Regul.ViewModels.Controls.Tab;
using Regul.Views.Controls.Tab;

namespace Regul.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private ObservableCollection<TabItem> _tabs = new();

        #region Properties

        public ObservableCollection<TabItem> Tabs
        {
            get => _tabs;
            set => this.RaiseAndSetIfChanged(ref _tabs, value);
        }

        #endregion
        
        #region ReactiveCommands

        private ReactiveCommand<Unit, Unit> ExitCommand { get; }
        private ReactiveCommand<Unit, Unit> SaveClearWindowCommand { get; }
        private ReactiveCommand<Unit, Unit> NewPackageCommand { get; }

        #endregion

        public MainWindowViewModel()
        {
            ExitCommand = ReactiveCommand.Create(Exit);
            SaveClearWindowCommand = ReactiveCommand.Create(SaveClearWindow);
            NewPackageCommand = ReactiveCommand.Create(NewPackage);
        }

        private void SaveClearWindow()
        {
            App.SaveClear = new SaveClear{ DataContext = new SaveClearViewModel() };
            App.SaveClear.ShowDialog(App.MainWindow);
        }

        private void Exit()
        {
            App.MainWindow.Close();
        }

        private void NewPackage()
        {
            Tabs.Add(new TabItem { Header = new TabHeader { ViewModel = { NameTab = "dsafdasf", CloseTabAction = CloseTab, ID = Guid.NewGuid().ToString("N")}}});
        }

        private void CloseTab(string id)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                TabHeaderViewModel item = ((TabHeader)Tabs[i].Header).ViewModel;
                if (item.ID == id)
                {
                    Tabs.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
