using Avalonia.Controls;
using Microsoft.VisualBasic;
using ReactiveUI;
using Regul.S3PI;
using Regul.S3PI.Interfaces;
using Regul.Structures;
using Regul.Views;
using Regul.Views.Controls.ListBoxItems;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Regul.ViewModels.Windows
{
    public class SaveClearViewModel : ReactiveObject
    {
        private string _pathFile;
        private int _progress;
        private int _selectVersion;
        private ObservableCollection<SaveFilePortrait> _saveFilePortraits = new();
        private SaveFilePortrait _selectSave;

        private bool _deletingCharacterPortraits;
        private bool _removingLotThumbnails;
        private bool _removingPhotosAndTextures;

        [AccessedThroughProperty("bgwClean")]
        private BackgroundWorker _bgwClean;

        #region Propertys

        private string PathFile
        {
            get => _pathFile;
            set => this.RaiseAndSetIfChanged(ref _pathFile, value);
        }

        private int Progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }
        private int SelectVersion
        {
            get=> _selectVersion;
            set => this.RaiseAndSetIfChanged(ref _selectVersion, value);
        }

        private ObservableCollection<SaveFilePortrait> SaveFilePortraits
        {
            get => _saveFilePortraits;
            set => this.RaiseAndSetIfChanged(ref _saveFilePortraits, value);
        }

        private SaveFilePortrait SelectSave
        {
            get => _selectSave;
            set => this.RaiseAndSetIfChanged(ref _selectSave, value);
        }

        private bool DeletingCharacterPortraits
        {
            get => _deletingCharacterPortraits;
            set => this.RaiseAndSetIfChanged(ref _deletingCharacterPortraits, value);
        }
        private bool RemovingLotThumbnails
        {
            get => _removingLotThumbnails;
            set => this.RaiseAndSetIfChanged(ref _removingLotThumbnails, value);
        }
        private bool RemovingPhotosAndTextures
        {
            get => _removingPhotosAndTextures;
            set => this.RaiseAndSetIfChanged(ref _removingPhotosAndTextures, value);
        }

        #endregion

        public SaveClearViewModel()
        {
            DeletingCharacterPortraits = true;
            bgwClean = new();

            try
            {
                if (!Directory.Exists(Path.Combine(TS3CC.Sims3MyDocFolder, "Saves")))
                {
                    MessageBox.Show(App.SaveClear, null, "No save file found", "Oops", MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Information);
                    App.SaveClear.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(App.SaveClear, ex.ToString(), "No save file found", "Oops", MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Information);
            }

            string path1 = "";

            if (TS3CC.Sims3MyDocFolder != null)
            {
                path1 = TS3CC.Sims3MyDocFolder;
            }
            else
            {
                
            }
            string[] directories = Directory.GetDirectories(Path.Combine(path1, "Saves"), "*.sims3", SearchOption.TopDirectoryOnly);
            int index1 = 0;
            
            while (index1 < directories.Length)
            {
                string path = directories[index1];
                string[] files = Directory.GetFiles(path, "*.nhd", SearchOption.TopDirectoryOnly);
                int index2 = 0;

                while (index2 < files.Length)
                {
                    string str = files[index2];
                    try
                    {
                        Save save = new(str);

                        SaveFilePortrait saveFilePortrait = new(path, save);

                        if (Strings.InStr(str, saveFilePortrait.Location) > 0)
                        {
                            saveFilePortrait.IconFamily.Source = save.FamilyIcon;
                            saveFilePortrait.SaveFamily.Text = save.WorldName;
                            SaveFilePortraits.Add(saveFilePortrait);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(App.SaveClear, ex.ToString(), "No save file found", "Oops", MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Information);
                    }
                    checked { ++index2; }
                }
                checked { ++index1; }
            }
        }

        private async void SelectPath()
        {
            OpenFileDialog dialog = new();
            dialog.Filters.Add(new FileDialogFilter { Name = "Save file The Sims 3", Extensions = { "nhd" } });
            List<string> files = (await dialog.ShowAsync(App.SaveClear)).ToList();

            if (files.Count != 0) PathFile = files.First();
        }

        private void Clear()
        {
            try
            {
                bgwClean.RunWorkerAsync(SelectSave);
            }
            catch (Exception ex)
            {
                MessageBox.Show(App.SaveClear, ex.ToString(), "Save Cleanup was successful!", "Successfully!", MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Information);
            }
            GC.Collect();
        }

        internal virtual BackgroundWorker bgwClean
        {
            get => _bgwClean;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                ProgressChangedEventHandler changedEventHandler = new(bgwClean_ProgressChanged);
                RunWorkerCompletedEventHandler completedEventHandler = new(bgwClean_RunWorkerCompleted);
                DoWorkEventHandler workEventHandler = new(bgwClean_DoWork);

                if (_bgwClean != null)
                {
                    _bgwClean.ProgressChanged -= changedEventHandler;
                    _bgwClean.RunWorkerCompleted -= completedEventHandler;
                    _bgwClean.DoWork -= workEventHandler;
                }

                _bgwClean = value;

                if (bgwClean == null) return;

                _bgwClean.ProgressChanged += changedEventHandler;
                _bgwClean.RunWorkerCompleted += completedEventHandler;
                _bgwClean.DoWork += workEventHandler;

                _bgwClean.WorkerReportsProgress = true;
            }
        }

        private void bgwClean_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        private void bgwClean_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(App.SaveClear, null, "Save Cleanup was successful!", "Successfully!", MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Information);
        }

        private void bgwClean_DoWork(object sender, DoWorkEventArgs e)
        {
            SaveFilePortrait saveFilePortrait = (SaveFilePortrait)e.Argument;
            long num1 = 0;
            string[] files1 = Directory.GetFiles(saveFilePortrait.SaveDir, "*.*", SearchOption.AllDirectories);
            int index1 = 0;

            while (index1 < files1.Length)
            {
                string file = files1[index1];
                checked { num1 += new FileInfo(file).Length; }
                checked { ++index1; }
            }

            bgwClean.ReportProgress(30, "Compressing Save...");
            string[] files2 = Directory.GetFiles(saveFilePortrait.SaveDir, "*.package", SearchOption.AllDirectories);
            int index2 = 0;

            while (index2 < files2.Length)
            {
                IPackage pkg = S3PI.Package.Package.OpenPackage(1, files2[index2], true);
                try
                {
                    foreach (IResourceIndexEntry getResource in pkg.GetResourceList)
                    {
                        if (getResource.Compressed == (ushort)0)
                            getResource.Compressed = ushort.MaxValue;
                    }
                }
                finally
                {
                }
                pkg.SavePackage();
                S3PI.Package.Package.ClosePackage(1, pkg);
                checked { ++index2; }
            }

            if (((IEnumerable<string>)Directory.GetFiles(saveFilePortrait.SaveDir, "*.nhd", SearchOption.AllDirectories)).Count<string>() > 1)
            {
                bgwClean.ReportProgress(60, (object)"Clearing Duplicated Images...");
                List<IResourceIndexEntry> resourceIndexEntries = new();

                IPackage pkg1 = S3PI.Package.Package.OpenPackage(1, Path.Combine(saveFilePortrait.SaveDir, "TravelDB.package"), true);
                try
                {
                    if (RemovingPhotosAndTextures)
                        foreach (IResourceIndexEntry getResource in pkg1.GetResourceList)
                        {
                            if (getResource.ResourceType == 11720834U)
                                resourceIndexEntries.Add(getResource);
                        }
                }
                finally { }
                string[] files3 = Directory.GetFiles(saveFilePortrait.SaveDir, "*.nhd", SearchOption.AllDirectories);
                int index3 = 0;
                while (index3 < files3.Length)
                {
                    string str = files3[index3];

                    if (Path.GetFileNameWithoutExtension(str).Contains(saveFilePortrait.Location))
                    {
                        IPackage pkg2 = S3PI.Package.Package.OpenPackage(1, str, true);
                        try
                        {
                            foreach (IResourceIndexEntry getResource in pkg2.GetResourceList)
                            {
                                if ((long)getResource.Instance != (long)saveFilePortrait.ImgInstance && getResource.ResourceType.Equals((object)1802339198))
                                    pkg2.DeleteResource(getResource);
                                if (getResource.Compressed == 0)
                                    getResource.Compressed = ushort.MaxValue;
                                if (DeletingCharacterPortraits && getResource.ResourceType == 92316365U | getResource.ResourceType == 92316366U | getResource.ResourceType == 92316367U)
                                    pkg2.DeleteResource(getResource);
                                if (RemovingLotThumbnails && getResource.ResourceType == 3629023174U)
                                    pkg2.DeleteResource(getResource);
                                if (RemovingPhotosAndTextures && getResource.ResourceType == 11720834U)
                                    pkg2.DeleteResource(getResource);
                            }
                        }
                        finally { }
                        bgwClean.ReportProgress(80, "Saving Save...");
                        pkg2.SavePackage();
                        S3PI.Package.Package.ClosePackage(0, pkg2);
                    }
                    else
                    {
                        IPackage pkg2 = S3PI.Package.Package.OpenPackage(1, str, true);
                        try
                        {
                            foreach (IResourceIndexEntry getResource in pkg2.GetResourceList)
                            {
                                if (getResource.Compressed == 0)
                                    getResource.Compressed = ushort.MaxValue;
                                if (DeletingCharacterPortraits && getResource.ResourceType == 92316365U | getResource.ResourceType == 92316366U | getResource.ResourceType == 92316367U)
                                    pkg2.DeleteResource(getResource);
                                if (RemovingLotThumbnails && getResource.ResourceType == 3629023174U)
                                    pkg2.DeleteResource(getResource);
                                if (RemovingPhotosAndTextures && getResource.ResourceType == 11720834U)
                                    pkg2.DeleteResource(getResource);
                            }
                        }
                        finally { }
                        bgwClean.ReportProgress(80, "Saving Save...");
                        pkg2.SavePackage();
                        S3PI.Package.Package.ClosePackage(0,pkg2);
                    }
                    checked { ++index3; }
                }
                pkg1.SavePackage();
                S3PI.Package.Package.ClosePackage(1, pkg1);
            }
            else
            {
                string[] files3 = Directory.GetFiles(saveFilePortrait.SaveDir, "*.nhd", SearchOption.AllDirectories);
                int index3 = 0;
                while (index3 < files3.Length)
                {
                    IPackage pkg = S3PI.Package.Package.OpenPackage(1, files3[index3], true);
                    bgwClean.ReportProgress(30, "Compressing Save...");
                    try
                    {
                        foreach (IResourceIndexEntry getResource in pkg.GetResourceList)
                        {
                            if (getResource.Compressed == 0)
                                    getResource.Compressed = ushort.MaxValue;
                                if (DeletingCharacterPortraits && getResource.ResourceType == 92316365U | getResource.ResourceType == 92316366U | getResource.ResourceType == 92316367U)
                                    pkg.DeleteResource(getResource);
                                if (RemovingLotThumbnails && getResource.ResourceType == 3629023174U)
                                    pkg.DeleteResource(getResource);
                                if (RemovingPhotosAndTextures && getResource.ResourceType == 11720834U)
                                    pkg.DeleteResource(getResource);
                        }
                    } finally{}
                    bgwClean.ReportProgress(60, "Saving Save...");
                    pkg.SavePackage();
                    S3PI.Package.Package.ClosePackage(1, pkg);
                    checked { ++index3; }
                }
            }
            long num2 = 0;
            string[] files4 = Directory.GetFiles(saveFilePortrait.SaveDir, "*.*", SearchOption.AllDirectories);
            int index4 = 0;

            while (index4 < files4.Length)
            {
                string file = files4[index4];
                checked { num2 += new FileInfo(file).Length; }
                checked { ++index4; }
            }
            bgwClean.ReportProgress(100, "Complete!");
            e.Result = true;
        }
    }
}
