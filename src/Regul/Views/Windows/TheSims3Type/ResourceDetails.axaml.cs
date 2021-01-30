using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Regul.OlibUI;
using Regul.ViewModels.Windows.TheSims3Type;
using System.Linq;

namespace Regul.Views.Windows.TheSims3Type
{
    public class ResourceDetails : OlibModalWindow
    {
        public ResourceDetailsViewModel ViewModel { get; set; } = new();

        public ResourceDetails()
        {
            InitializeComponent();
            DataContext = ViewModel;

            SetupDnD();
        }

        private void SetupDnD()
        {
            void DragOver(object sender, DragEventArgs e)
            {
                e.DragEffects &= DragDropEffects.Copy | DragDropEffects.Link;

                if (!e.Data.Contains(DataFormats.FileNames)) e.DragEffects = DragDropEffects.None;
            }
            void Drop(object sender, DragEventArgs e)
            {
                if (e.Data.Contains(DataFormats.FileNames))
                {
                    ViewModel.Filename = e.Data.GetFileNames().First();
                }
            }

            AddHandler(DragDrop.DropEvent, Drop);
            AddHandler(DragDrop.DragOverEvent, DragOver);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
