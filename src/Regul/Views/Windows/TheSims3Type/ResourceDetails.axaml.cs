using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using OlibUI.Windows;
using Regul.ViewModels.Windows.TheSims3Type;
using System.Linq;

namespace Regul.Views.Windows.TheSims3Type
{
    public class ResourceDetails : OlibWindow
    {
        public ResourceDetails()
        {
            InitializeComponent();
            DataContext = new ResourceDetailsViewModel();

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
                    ((ResourceDetailsViewModel)DataContext).Filename = e.Data.GetFileNames().First();
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
