using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Regul.Core.TheSims3Type;
using Regul.ViewModels.Controls.ContentTab;

namespace Regul.Views.Controls.ContentTab
{
    public class TheSims3TypeContent : UserControl
    {
        private DataGrid ResourceList;

        public TheSims3TypeContentViewModel ViewModel { get; set; }
        
        public TheSims3TypeContent()
        {
            InitializeComponent();
            DataContext = ViewModel = new();
            ResourceList.SelectionChanged += ResourceList_SelectionChanged;
        }

        private void ResourceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResourceList.SelectedItem != null)
            {
                ViewModel.SelectedResource = (Resource)ResourceList.SelectedItem;
            }
        }

        public TheSims3TypeContent(string path)
        {
            InitializeComponent();
            DataContext = ViewModel = new(path);
            ResourceList.SelectionChanged += ResourceList_SelectionChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            ResourceList = this.FindControl<DataGrid>("ResourceList");
        }
    }
}
