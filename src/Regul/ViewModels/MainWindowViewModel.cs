using ReactiveUI;
using Regul.ViewModels.Windows;
using Regul.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Regul.Core;
using Regul.Structures;
using Regul.ViewModels.Controls.Tab;
using Regul.Views.Controls.ContentTab;
using Regul.Views.Controls.Tab;

namespace Regul.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private ObservableCollection<TabItem> _tabs = new();
        private ObservableCollection<Project> _projects = new();

        private string _creatorName;
        private bool _isNotNull;
        private TabItem _selecetedTabItem;

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

        public bool IsNotNull
        {
            get => _isNotNull;
            set => this.RaiseAndSetIfChanged(ref _isNotNull, value);
        }

        public TabItem SelectedTabItem
        {
            get => _selecetedTabItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selecetedTabItem, value);
                IsNotNull = !Tabs.Any();
            }
        }

        #endregion
        
        #region ReactiveCommands

        private ReactiveCommand<Unit, Unit> ExitCommand { get; }
        private ReactiveCommand<Unit, Unit> SaveClearWindowCommand { get; }
        private ReactiveCommand<Unit, Unit> NewPackageCommand { get; }
        private ReactiveCommand<Unit, Unit> ClearGCCommand { get; }
        private ReactiveCommand<Unit, Unit> DeleteProjectCommand { get; }
        private ReactiveCommand<Unit, Unit> SettingsWindowCommand { get; }
        private ReactiveCommand<Unit, Unit> AboutWindowCommand { get; }

        #endregion

        public MainWindowViewModel()
        {
            ExitCommand = ReactiveCommand.Create(() => App.MainWindow.Close());
            SaveClearWindowCommand = ReactiveCommand.Create(SaveClearWindow);
            NewPackageCommand = ReactiveCommand.Create(NewPackage);
            ClearGCCommand = ReactiveCommand.Create(ClearGC);
            DeleteProjectCommand = ReactiveCommand.Create(DeleteProject);
            SettingsWindowCommand = ReactiveCommand.Create(SettingsWindow);
            AboutWindowCommand = ReactiveCommand.Create(AboutWindow);

            Initialize();
        }

        private void Initialize()
        {
            Application.Current.Styles[2] = !string.IsNullOrEmpty(Program.Settings.Theme)
                ? new StyleInclude(new Uri("resm:Styles?assembly=Regul"))
                {
                    Source = new Uri($"avares://Regul/Assets/Themes/{Program.Settings.Theme}.axaml")
                }
                : new StyleInclude(new Uri("resm:Styles?assembly=Regul"))
                {
                    Source = new Uri("avares://Regul/Assets/Themes/Dazzling.axaml")
                };
            
            CreatorName = Program.Settings.CreatorName;
            Projects = new ObservableCollection<Project>(Program.Settings.Projects);
        }

        private void SaveClearWindow()
        {
            App.SaveClear = new SaveClear{ DataContext = new SaveClearViewModel() };
            App.SaveClear.ShowDialog(App.MainWindow);
        }
        private void SettingsWindow()
        {
            App.Settings = new Views.Windows.Settings { DataContext = new SettingsViewModel() };
            App.Settings.ShowDialog(App.MainWindow);
        }
        private void AboutWindow()
        {
            App.About = new About {DataContext = new AboutViewModel()};
            App.About.ShowDialog(App.MainWindow);
        }

        public void Exit()
        {
            Program.Settings.Projects = Projects.ToList();
            Program.Settings.CreatorName = CreatorName;
            
            FileSettings.SaveSettings();
        }

        private async void NewPackage()
        {
            App.SelectType = new SelectType { DataContext = new SelectTypeViewModel() };
            if (await App.SelectType.ShowDialog<bool>(App.MainWindow))
            {
                Tabs.Add(new TabItem
                {
                    Header = new TabHeader
                    {
                        ViewModel =
                        {
                            NameTab = (string)Application.Current.FindResource("NoName"), 
                            CloseTabAction = CloseTab, 
                            ID = Guid.NewGuid().ToString("N"),
                            PackageType = ((SelectTypeViewModel)App.SelectType.DataContext).Type
                        }
                    },
                    Content = ((SelectTypeViewModel)App.SelectType.DataContext).Type switch
                    {
                        _ => new TheSims3TypeContent()
                    }
                });
            }
        }

        private void CloseTab(string id)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                TabHeaderViewModel item = ((TabHeader)Tabs[i].Header)?.ViewModel;
                if (item?.ID == id)
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