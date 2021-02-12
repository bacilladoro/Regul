using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.Core.Interfaces;
using Regul.Core.TheSims3Type;
using Regul.ViewModels.Controls.ContentTab;

namespace Regul.Views.Controls.ContentTab
{
    public class TheSims3TypeContent : UserControl, IPackageContent
    {
        private DataGrid ResourceList;

        IPackageType IPackageContent.PackageType { get; set; }

        public TheSims3TypeContent()
        {
            InitializeComponent();
            IPackageContent content = this;
            DataContext = content.PackageType = new TheSims3TypeContentViewModel();

            ResourceList.SelectionChanged += ResourceList_SelectionChanged;
            ResourceList.DoubleTapped += ResourceList_DoubleTapped;
        }

        private void ResourceList_DoubleTapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ((TheSims3TypeContentViewModel)DataContext)?.ResourceDetails();
        }

        private void ResourceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((TheSims3TypeContentViewModel)DataContext)?.SelectedResources.Clear();
            for (int i = 0; i < ResourceList.SelectedItems.Count; i++)
                ((TheSims3TypeContentViewModel)DataContext)?.SelectedResources.Add((Resource)ResourceList.SelectedItems[i]);
            ((TheSims3TypeContentViewModel)DataContext).SelectedResource = (Resource)ResourceList.SelectedItem;
        }

        public TheSims3TypeContent(string path)
        {
            InitializeComponent();
            IPackageContent content = this;
            DataContext = content.PackageType = new TheSims3TypeContentViewModel(path);
            ResourceList.SelectionChanged += ResourceList_SelectionChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            ResourceList = this.FindControl<DataGrid>("ResourceList");
        }
    }
}
