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
using System.IO;
using System.Linq;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Newtonsoft.Json;
using OlibUI;
using OlibUI.Structures;

namespace Regul.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<TabItem> _tabs = new ObservableCollection<TabItem>();
        private ObservableCollection<Project> _projects = new ObservableCollection<Project>();

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
                        switch (((TabHeaderViewModel) ((TabHeader) item.Header).DataContext).PackageType)
                        {
                            default:
                                ((TheSims3TypeContentViewModel) ((TheSims3TypeContent) item.Content).DataContext)
                                    .Active = true;
                                break;
                        }

                        continue;
                    }

                    switch (((TabHeaderViewModel) ((TabHeader) item.Header).DataContext).PackageType)
                    {
                        default:
                            ((TheSims3TypeContentViewModel) ((TheSims3TypeContent) item.Content).DataContext).Active =
                                false;
                            break;
                    }
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
            switch (Program.Settings.Theme)
            {
                case "Gloomy":
                case "Mysterious":
                case "Turquoise":
                case "Emerald":
                case "Dazzling":
                    Application.Current.Styles[1] = new StyleInclude(new Uri("resm:Styles?assembly=Regul"))
                    {
                        Source = new Uri($"avares://OlibUI/Themes/{Program.Settings.Theme}.axaml")
                    };
                    break;
                default:
                    List<Theme> Themes = new List<Theme>();

                    if (Directory.Exists("Themes"))
                    {
                        foreach (string path in Directory.EnumerateFiles("Themes"))
                        {
                            string json = File.ReadAllText(path);
                            Themes.Add(JsonConvert.DeserializeObject<Theme>(json));
                        }
                    }

                    Theme? theme = Themes.FirstOrDefault(t => t.Name == Program.Settings.Theme);
                    if (theme != null)
                        Application.Current.Styles[1] = AvaloniaRuntimeXamlLoader.Parse<IStyle>(theme.ToAxaml());
                    else
                        Application.Current.Styles[1] = new StyleInclude(new Uri("resm:Styles?assembly=Regul"))
                        {
                            Source = new Uri("avares://OlibUI/Themes/Dazzling.axaml")
                        };
                    
                    break;
            }

            CreatorName = Program.Settings.CreatorName;
            Projects = new ObservableCollection<Project>(Program.Settings.Projects);
        }

        private void SaveCleanerWindow()
        {
            App.SaveCleaner = new SaveCleaner();
            App.SaveCleaner.ShowDialog(App.MainWindow);
        }

        private void SettingsWindow()
        {
            App.Settings = new Views.Windows.Settings();
            App.Settings.ShowDialog(App.MainWindow);
        }

        private void AboutWindow()
        {
            App.About = new About();
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
            App.SelectType = new SelectType {DataContext = new SelectTypeViewModel()};
            if (await App.SelectType.ShowDialog<bool>(App.MainWindow))
            {
                IPackageContent typeContent;

                switch (((SelectTypeViewModel) App.SelectType.DataContext).Type)
                {
                    default:
                        typeContent = new TheSims3TypeContent();
                        break;
                }

                Tabs.Add(new TabItem
                {
                    Header = new TabHeader
                    {
                        DataContext = new TabHeaderViewModel
                        {
                            Icon = (Geometry) Application.Current.FindResource("TheSims3Icon"),
                            NameTab = (string) Application.Current.FindResource("NoName"),
                            CloseTabAction = CloseTab,
                            ID = Guid.NewGuid().ToString("N"),
                            PackageType = ((SelectTypeViewModel) App.SelectType.DataContext).Type
                        }
                    },
                    Content = typeContent
                });
            }
        }

        private async void OpenPackage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter {Extensions = {"package"}, Name = "Package files"});
            dialog.Filters.Add(new FileDialogFilter {Extensions = {"nhd"}, Name = "Save files"});
            List<string> files = (await dialog.ShowAsync(App.MainWindow)).ToList();

            if (files.Count == 0) return;

            App.SelectType = new SelectType {DataContext = new SelectTypeViewModel()};
            if (await App.SelectType.ShowDialog<bool>(App.MainWindow))
            {
                IPackageContent typeContent;

                switch (((SelectTypeViewModel) App.SelectType.DataContext).Type)
                {
                    default:
                        typeContent = new TheSims3TypeContent(files[0]);
                        break;
                }

                Tabs.Add(new TabItem
                {
                    Header = new TabHeader
                    {
                        DataContext = new TabHeaderViewModel
                        {
                            Icon = (Geometry) Application.Current.FindResource("TheSims3Icon"),
                            NameTab = System.IO.Path.GetFileNameWithoutExtension(files[0]),
                            CloseTabAction = CloseTab,
                            ID = Guid.NewGuid().ToString("N"),
                            PackageType = ((SelectTypeViewModel) App.SelectType.DataContext).Type,
                            IsSave = true
                        }
                    },
                    Content = typeContent
                });
            }
        }

        private void SaveAll()
        {
            for (int i = 0; i < Tabs.Count; i++)
                ((IPackageContent) Tabs[i].Content)?.PackageType.SavePackage();
        }

        private void CloseTab(string id)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                TabHeaderViewModel item = (TabHeaderViewModel) ((TabHeader) Tabs[i].Header)?.DataContext;
                if (item?.ID == id)
                {
                    ((IPackageContent) Tabs[i].Content)?.PackageType.SavePackage();
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