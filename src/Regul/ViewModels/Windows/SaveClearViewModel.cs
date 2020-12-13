using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using ReactiveUI;
using Regul.S3PI.Interfaces;
using Regul.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regul.ViewModels.Windows
{
    public class SaveClearViewModel : ReactiveObject
    {
        private string _path;
        private string Path
        {
            get => _path;
            set => this.RaiseAndSetIfChanged(ref _path, value);
        }

        private async void SelectPath()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter { Name = "Save file The Sims 3", Extensions = {"nhd"} });
            List<string> files = (await dialog.ShowAsync(App.SaveClear)).ToList();

            if (files.Count != 0) Path = files.First();
        }

        private void Clear()
        {
            if (string.IsNullOrEmpty(Path))
            {
                MessageBox.Show(App.SaveClear, null, "You haven't selected a file!", "Error", MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Error);
                return;
            }
            IPackage package = S3PI.Package.Package.OpenPackage(0, Path, true);
            uint[] numArray = new uint[3]
            {
                92316365U,
                92316366U,
                92316367U
            };
            IList<IResourceIndexEntry> all = package.FindAll(Entry => ((IEnumerable<uint>)numArray).Contains(Entry.ResourceType));

            try
            {
                foreach (IResourceIndexEntry rc in all)
                    package.DeleteResource(rc);
            }
            finally
            {
                IEnumerator<IResourceIndexEntry> enumerator = null;
                enumerator?.Dispose();
            }

            package.SavePackage();
            MessageBox.Show(App.SaveClear, null, "Save Cleanup was successful!", "Successfully!", MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Information);
        }
    }
}
