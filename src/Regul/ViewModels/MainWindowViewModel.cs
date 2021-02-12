using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Regul.Core;
using Regul.Core.Interfaces;
using Regul.Structures;
using Regul.ViewModels.Controls.ContentTab;
using Regul.ViewModels.Controls.Tab;
using Regul.ViewModels.Windows;
using Regul.Views.Controls.ContentTab;
using Regul.Views.Controls.Tab;
using Regul.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Regul.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
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
            set => RaiseAndSetIfChanged(ref _tabs, value);
        }

        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set => RaiseAndSetIfChanged(ref _projects, value);
        }

        public string CreatorName
        {
            get => _creatorName;
            set => RaiseAndSetIfChanged(ref _creatorName, value);
        }

        public Project SelectedProject
        {
            get => _selectedProject;
            set => RaiseAndSetIfChanged(ref _selectedProject, value);
        }

        public bool IsNotNull
        {
            get => _isNotNull;
            set => RaiseAndSetIfChanged(ref _isNotNull, value);
        }

        public TabItem SelectedTabItem
        {
            get => _selecetedTabItem;
            set
            {
                RaiseAndSetIfChanged(ref _selecetedTabItem, value);
                IsNotNull = SelectedTabItem != null;

                if (SelectedTabItem == null) return;

                for (int i = 0; i < Tabs.Count; i++)
                {
                    TabItem item = Tabs[i];
                    if (item == SelectedTabItem)
                    {
                        ((TheSims3TypeContentViewModel)((TheSims3TypeContent)item.Content).DataContext).Active = ((TabHeaderViewModel)((TabHeader)item.Header).DataContext).PackageType switch
                        {
                            _ => true,
                        };
                        continue;
                    }

                    ((TheSims3TypeContentViewModel)((TheSims3TypeContent)item.Content).DataContext).Active = ((TabHeaderViewModel)((TabHeader)item.Header).DataContext).PackageType switch
                    {
                        _ => false,
                    };
                }
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            Application.Current.Styles[2] = !string.IsNullOrEmpty(Program.Settings.Theme)
                ? new StyleInclude(new Uri("resm:Styles?assembly=Regul"))
                {
                    Source = new Uri($"avares://Regul.OlibUI/Themes/{Program.Settings.Theme}.axaml")
                }
                : new StyleInclude(new Uri("resm:Styles?assembly=Regul"))
                {
                    Source = new Uri("avares://Regul.OlibUI/Themes/Dazzling.axaml")
                };

            CreatorName = Program.Settings.CreatorName;
            Projects = new ObservableCollection<Project>(Program.Settings.Projects);
        }

        private void SaveClearWindow()
        {
            App.SaveClear = new SaveClear { DataContext = new SaveClearViewModel() };
            App.SaveClear.ShowDialog(App.MainWindow);
        }
        private void SettingsWindow()
        {
            App.Settings = new Views.Windows.Settings { DataContext = new SettingsViewModel() };
            App.Settings.ShowDialog(App.MainWindow);
        }
        private void AboutWindow()
        {
            App.About = new About { DataContext = new AboutViewModel() };
            App.About.ShowDialog(App.MainWindow);
        }
        private void HEXNumberConverter()
        {
            App.HEXNumberConverter = new HEXNumberConverter();
            App.HEXNumberConverter.Show();
        }

        public void Exit()
        {
            Program.Settings.Projects = Projects.ToList();
            Program.Settings.CreatorName = CreatorName;

            FileSettings.SaveSettings();

            SaveAll();
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
                        DataContext = new TabHeaderViewModel
                        {
                            Icon = (Geometry)Application.Current.FindResource("TheSims3Icon"),
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
        private async void OpenPackage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter { Extensions = { "package" }, Name = "Package files" });
            dialog.Filters.Add(new FileDialogFilter { Extensions = { "nhd" }, Name = "Save files" });
            List<string> files = (await dialog.ShowAsync(App.MainWindow)).ToList();

            if (files.Count == 0) return;

            App.SelectType = new SelectType { DataContext = new SelectTypeViewModel() };
            if (await App.SelectType.ShowDialog<bool>(App.MainWindow))
            {
                Tabs.Add(new TabItem
                {
                    Header = new TabHeader
                    {
                        DataContext = new TabHeaderViewModel
                        {
                            Icon = (Geometry)Application.Current.FindResource("TheSims3Icon"),
                            NameTab = System.IO.Path.GetFileNameWithoutExtension(files[0]),
                            CloseTabAction = CloseTab,
                            ID = Guid.NewGuid().ToString("N"),
                            PackageType = ((SelectTypeViewModel)App.SelectType.DataContext).Type,
                            IsSave = true
                        }
                    },
                    Content = ((SelectTypeViewModel)App.SelectType.DataContext).Type switch
                    {
                        _ => new TheSims3TypeContent(files[0])
                    }
                });
            }
        }

        private void SaveAll()
        {
            for (int i = 0; i < Tabs.Count; i++)
                ((IPackageContent)Tabs[i].Content)?.PackageType.SavePackage();
        }

        private void CloseTab(string id)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                TabHeaderViewModel item = (TabHeaderViewModel)((TabHeader)Tabs[i].Header)?.DataContext;
                if (item?.ID == id)
                {
                    ((IPackageContent)Tabs[i].Content)?.PackageType.SavePackage();
                    Tabs.RemoveAt(i);
                    break;
                }
            }
        }

        private void CloseProgram()
        {
            App.MainWindow.Close();
        }

        private void ClearGC() => GC.Collect();
        private void DeleteProject() => Projects.Remove(SelectedProject);
    }
}