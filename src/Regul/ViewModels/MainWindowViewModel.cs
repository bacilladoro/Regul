using ReactiveUI;
using Regul.ViewModels.Windows;
using Regul.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using Regul.Core;
using Regul.Structures;
using Regul.ViewModels.Controls.Tab;
using Regul.Views.Controls.Tab;

namespace Regul.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private ObservableCollection<TabItem> _tabs = new();
        private ObservableCollection<Project> _projects = new();

        private string _creatorName;

        private Project _selectedProject;

        #region Properties

        public ObservableCollection<TabItem> Tabs
        {
            get => _tabs;
            set => this.RaiseAndSetIfChanged(ref _tabs, value);
        }
        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set => this.RaiseAndSetIfChanged(ref _projects, value);
        }

        public string CreatorName
        {
            get => _creatorName;
            set => this.RaiseAndSetIfChanged(ref _creatorName, value);
        }
        
        public Project SelectedProject
        {
            get => _selectedProject;
            set => this.RaiseAndSetIfChanged(ref _selectedProject, value);
        }

        #endregion
        
        #region ReactiveCommands

        private ReactiveCommand<Unit, Unit> ExitCommand { get; }
        private ReactiveCommand<Unit, Unit> SaveClearWindowCommand { get; }
        private ReactiveCommand<Unit, Unit> NewPackageCommand { get; }
        private ReactiveCommand<Unit, Unit> ClearGCCommand { get; }
        private ReactiveCommand<Unit, Unit> DeleteProjectCommand { get; }

        #endregion

        public MainWindowViewModel()
        {
            ExitCommand = ReactiveCommand.Create(() => App.MainWindow.Close());
            SaveClearWindowCommand = ReactiveCommand.Create(SaveClearWindow);
            NewPackageCommand = ReactiveCommand.Create(NewPackage);
            ClearGCCommand = ReactiveCommand.Create(ClearGC);
            DeleteProjectCommand = ReactiveCommand.Create(DeleteProject);

            Initialize();
        }

        private void Initialize()
        {
            CreatorName = Program.Settings.CreatorName;
            Projects = new ObservableCollection<Project>(Program.Settings.Projects);
        }

        private void SaveClearWindow()
        {
            App.SaveClear = new SaveClear{ DataContext = new SaveClearViewModel() };
            App.SaveClear.ShowDialog(App.MainWindow);
        }

        public void Exit()
        {
            Program.Settings.Projects = Projects.ToList();
            Program.Settings.CreatorName = CreatorName;
            
            FileSettings.SaveSettings();
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
        
        private void ClearGC() => GC.Collect();
        private void DeleteProject() => Projects.Remove(SelectedProject);
    }
}
