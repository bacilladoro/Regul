using Avalonia;
using Avalonia.Controls;
using Microsoft.VisualBasic;
using Regul.Core;
using Regul.S3PI;
using Regul.S3PI.Interfaces;
using Regul.S3PI.Package;
using Regul.Views;
using Regul.Views.Controls.ListBoxItems;
using Regul.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Regul.ViewModels.Windows
{
    internal sealed class SaveClearViewModel : ViewModelBase
    {
        private string _pathBackup;
        private bool _isLoading;
        private ObservableCollection<SaveFilePortrait> _saveFilePortraits = new();
        private SaveFilePortrait _selectSave;
        private Loading _loading;

        private bool _deletingCharacterPortraits;
        private bool _removingLotThumbnails;
        private bool _removingTextures;
        private bool _removingPhotos;
        private bool _removingGeneratedImages;
        private bool _removingFamilyPortraits;
        private bool _createABackup;

        [AccessedThroughProperty("bgwClean")]
        private BackgroundWorker _bgwClean;

        #region Propertys

        private string PathBackup
        {
            get => _pathBackup;
            set => RaiseAndSetIfChanged(ref _pathBackup, value);
        }

        private bool IsLoading
        {
            get => _isLoading;
            set => RaiseAndSetIfChanged(ref _isLoading, value);
        }

        private ObservableCollection<SaveFilePortrait> SaveFilePortraits
        {
            get => _saveFilePortraits;
            set => RaiseAndSetIfChanged(ref _saveFilePortraits, value);
        }

        private SaveFilePortrait SelectSave
        {
            get => _selectSave;
            set => RaiseAndSetIfChanged(ref _selectSave, value);
        }

        private bool DeletingCharacterPortraits
        {
            get => _deletingCharacterPortraits;
            set => RaiseAndSetIfChanged(ref _deletingCharacterPortraits, value);
        }
        private bool RemovingLotThumbnails
        {
            get => _removingLotThumbnails;
            set => RaiseAndSetIfChanged(ref _removingLotThumbnails, value);
        }
        private bool RemovingPhotos
        {
            get => _removingPhotos;
            set => RaiseAndSetIfChanged(ref _removingPhotos, value);
        }
        private bool RemovingTextures
        {
            get => _removingTextures;
            set => RaiseAndSetIfChanged(ref _removingTextures, value);
        }
        private bool RemovingGeneratedImages
        {
            get => _removingGeneratedImages;
            set => RaiseAndSetIfChanged(ref _removingGeneratedImages, value);
        }
        private bool RemovingFamilyPortraits
        {
            get => _removingFamilyPortraits;
            set => RaiseAndSetIfChanged(ref _removingFamilyPortraits, value);
        }
        private bool CreateABackup
        {
            get => _createABackup;
            set => RaiseAndSetIfChanged(ref _createABackup, value);
        }

        #endregion

        public SaveClearViewModel()
        {
            DeletingCharacterPortraits = true;
            bgwClean = new BackgroundWorker();

            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                if (!Directory.Exists(Path.Combine(TS3CC.Sims3MyDocFolder, "Saves")))
                {
                    await MessageBox.Show(App.SaveClear, null, (string)Application.Current.FindResource("SaveFilesNotFound"), (string)Application.Current.FindResource("Information"),
                        MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Information);
                    App.SaveClear.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                await MessageBox.Show(App.SaveClear, ex.ToString(), (string)Application.Current.FindResource("AnErrorHasOccurred"),
                    (string)Application.Current.FindResource("Error"), MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Error);
            }

            string path1;
            if (TS3CC.Sims3MyDocFolder != null) path1 = TS3CC.Sims3MyDocFolder;
            else
            {
                await MessageBox.Show(App.SaveClear, null, (string)Application.Current.FindResource("NotFindFolderTheSims3"),
                    (string)Application.Current.FindResource("Information"), MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Information);

                OpenFolderDialog dialog = new();
                string path = await dialog.ShowAsync(App.SaveClear);
                if (!string.IsNullOrEmpty(path)) path1 = path;
                else
                {
                    App.SaveClear.Close();
                    return;
                }
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
                            saveFilePortrait.ImgInstance = save.ImgInstance;
                            SaveFilePortraits.Add(saveFilePortrait);
                        }
                    }
                    catch (Exception ex)
                    {
                        await MessageBox.Show(App.SaveClear, ex.ToString(), (string)Application.Current.FindResource("AnErrorHasOccurred"),
                            (string)Application.Current.FindResource("Error"), MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Error);
                    }
                    checked { ++index2; }
                }
                checked { ++index1; }
            }
        }

        private async void SelectPath()
        {
            OpenFolderDialog dialog = new();
            PathBackup = await dialog.ShowAsync(App.SaveClear) + "\\";
        }

        private async void Clear()
        {
            try
            {
                if (SelectSave == null)
                {
                    await MessageBox.Show(App.SaveClear, null, (string)Application.Current.FindResource("NoSelectedSaveFile"),
                        (string)Application.Current.FindResource("Error"), MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Error);
                    return;
                }
                IsLoading = true;
                _loading = new Loading();
                bgwClean.RunWorkerAsync(SelectSave);
                await _loading.ShowDialog(App.SaveClear);
            }
            catch (Exception ex)
            {
                await MessageBox.Show(App.SaveClear, ex.ToString(), (string)Application.Current.FindResource("AnErrorHasOccurred"),
                    (string)Application.Current.FindResource("Error"), MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Information);
            }
            GC.Collect();
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            for (int index = 0; index < files.Length; index++)
            {
                FileInfo file = files[index];
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    DirectoryInfo subdir = dirs[i];
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, true);
                }
            }
        }

        internal BackgroundWorker bgwClean
        {
            get => _bgwClean;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                ProgressChangedEventHandler changedEventHandler = bgwClean_ProgressChanged;
                RunWorkerCompletedEventHandler completedEventHandler = bgwClean_RunWorkerCompleted;
                DoWorkEventHandler workEventHandler = bgwClean_DoWork;

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
            _loading.ProcessText.Text = (string)e.UserState;
            _loading.Progress.Value = e.ProgressPercentage;
        }

        private void bgwClean_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _loading.Close();
            IsLoading = false;
            MessageBox.Show(App.SaveClear, null, (string)Application.Current.FindResource("SaveFilesCleanedSuccessfully") + $"\nTotal: {e.Result}",
                (string)Application.Current.FindResource("Successfully"), MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Information);
        }

        private void bgwClean_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            SaveFilePortrait saveFilePortrait = (SaveFilePortrait)e.Argument;

            if (CreateABackup)
            {
                bgwClean.ReportProgress(20, (string)Application.Current.FindResource("ProcessingCreateBackup"));
                if (!string.IsNullOrEmpty(PathBackup))
                    DirectoryCopy(saveFilePortrait?.SaveDir, PathBackup + saveFilePortrait?.SaveName.Text + ".sims3",
                        true);
            }

            bgwClean.ReportProgress(40, (string)Application.Current.FindResource("ProcessingCompressingSave"));
            string[] files2 = Directory.GetFiles(saveFilePortrait?.SaveDir, "*.package", SearchOption.AllDirectories);
            int index2 = 0;

            while (index2 < files2.Length)
            {
                IPackage pkg = Package.OpenPackage(files2[index2], true);
                for (int index = 0; index < pkg.GetResourceList.Count; index++)
                {
                    IResourceIndexEntry getResource = pkg.GetResourceList[index];
                    if (getResource.Compressed == 0)
                        getResource.Compressed = ushort.MaxValue;
                }

                pkg.SavePackage();
                Package.ClosePackage(pkg);
                checked { ++index2; }
            }

            if (Directory.GetFiles(saveFilePortrait.SaveDir, "*.nhd", SearchOption.AllDirectories).Length > 1)
            {
                bgwClean.ReportProgress(70, (string)Application.Current.FindResource("ProcessingClearingSave"));

                if (RemovingGeneratedImages || RemovingPhotos)
                {
                    IPackage pkg1 = Package.OpenPackage(Path.Combine(saveFilePortrait.SaveDir, "TravelDB.package"), true);
                    for (int i = 0; i < pkg1.GetResourceList.Count; i++)
                    {
                        IResourceIndexEntry getResource = pkg1.GetResourceList[i];
                        if (RemovingGeneratedImages && getResource.ResourceType == 11720834U && getResource.ResourceGroup == 38510538U ||
                            RemovingPhotos && getResource.ResourceType == 11720834U && getResource.ResourceGroup == 40488965U)
                            pkg1.DeleteResource(getResource);
                    }

                    pkg1.SavePackage();
                    Package.ClosePackage(pkg1);
                }

                string[] files3 = Directory.GetFiles(saveFilePortrait.SaveDir, "*.nhd", SearchOption.AllDirectories);
                int index3 = 0;
                while (index3 < files3.Length)
                {
                    string str = files3[index3];

                    if (Path.GetFileNameWithoutExtension(str).Contains(saveFilePortrait.Location))
                    {
                        IPackage pkg2 = Package.OpenPackage(str, true);
                        for (int i = 0; i < pkg2.GetResourceList.Count; i++)
                        {
                            IResourceIndexEntry getResource = pkg2.GetResourceList[i];
                            if (RemovingFamilyPortraits && getResource.Instance != saveFilePortrait.ImgInstance &&
                                getResource.ResourceType == 1802339198U)
                            {
                                pkg2.DeleteResource(getResource);
                                continue;
                            }
                            if (RemovingGeneratedImages && getResource.ResourceType == 11720834U && getResource.ResourceGroup == 38510538U ||
                                RemovingPhotos && getResource.ResourceType == 11720834U && getResource.ResourceGroup == 40488965U ||
                                RemovingTextures && getResource.ResourceType == 11720834U &&
                                (getResource.ResourceGroup == 11584775U | getResource.ResourceGroup == 1U | getResource.ResourceGroup == 12328524U | getResource.ResourceGroup == 1943529U | getResource.ResourceGroup == 16441714U | getResource.ResourceGroup == 38510538U | getResource.ResourceGroup == 12328532U | getResource.ResourceGroup == 8287573U))
                            {
                                pkg2.DeleteResource(getResource);
                                continue;
                            }
                            if (RemovingLotThumbnails && getResource.ResourceType == 3629023174U)
                            {
                                pkg2.DeleteResource(getResource);
                                continue;
                            }
                            if (DeletingCharacterPortraits && getResource.ResourceType == 92316365U |
                                getResource.ResourceType == 92316366U | getResource.ResourceType == 92316367U)
                            {
                                pkg2.DeleteResource(getResource);
                                continue;
                            }
                            if (getResource.Compressed == 0) getResource.Compressed = ushort.MaxValue;
                        }

                        pkg2.SavePackage();
                        Package.ClosePackage(pkg2);
                    }
                    else
                    {
                        IPackage pkg2 = Package.OpenPackage(str, true);
                        for (int i = 0; i < pkg2.GetResourceList.Count; i++)
                        {
                            IResourceIndexEntry getResource = pkg2.GetResourceList[i];
                            if (RemovingGeneratedImages && getResource.ResourceType == 11720834U && getResource.ResourceGroup == 38510538U ||
                                RemovingPhotos && getResource.ResourceType == 11720834U && getResource.ResourceGroup == 40488965U ||
                                RemovingTextures && getResource.ResourceType == 11720834U &&
                                (getResource.ResourceGroup == 11584775U | getResource.ResourceGroup == 1U | getResource.ResourceGroup == 12328524U | getResource.ResourceGroup == 1943529U | getResource.ResourceGroup == 16441714U | getResource.ResourceGroup == 38510538U | getResource.ResourceGroup == 12328532U | getResource.ResourceGroup == 8287573U))
                            {
                                pkg2.DeleteResource(getResource);
                                continue;
                            }
                            if (RemovingLotThumbnails && getResource.ResourceType == 3629023174U)
                            {
                                pkg2.DeleteResource(getResource);
                                continue;
                            }
                            if (DeletingCharacterPortraits && getResource.ResourceType == 92316365U |
                                getResource.ResourceType == 92316366U | getResource.ResourceType == 92316367U)
                            {
                                pkg2.DeleteResource(getResource);
                                continue;
                            }
                            if (getResource.Compressed == 0)
                                getResource.Compressed = ushort.MaxValue;
                        }

                        pkg2.SavePackage();
                        Package.ClosePackage(pkg2);
                    }
                    checked { ++index3; }
                }
            }
            else
            {
                string[] files3 = Directory.GetFiles(saveFilePortrait.SaveDir, "*.nhd", SearchOption.AllDirectories);
                int index3 = 0;

                bgwClean.ReportProgress(40, (string)Application.Current.FindResource("ProcessingCompressingSave"));
                while (index3 < files3.Length)
                {
                    IPackage pkg = Package.OpenPackage(files3[index3], true);
                    for (int i = 0; i < pkg.GetResourceList.Count; i++)
                    {
                        IResourceIndexEntry getResource = pkg.GetResourceList[i];
                        if (RemovingGeneratedImages && getResource.ResourceType == 11720834U && getResource.ResourceGroup == 38510538U ||
                                RemovingPhotos && getResource.ResourceType == 11720834U && getResource.ResourceGroup == 40488965U ||
                                RemovingTextures && getResource.ResourceType == 11720834U &&
                                (getResource.ResourceGroup == 11584775U | getResource.ResourceGroup == 1U | getResource.ResourceGroup == 12328524U | getResource.ResourceGroup == 1943529U | getResource.ResourceGroup == 16441714U | getResource.ResourceGroup == 38510538U | getResource.ResourceGroup == 12328532U | getResource.ResourceGroup == 8287573U))
                        {
                            pkg.DeleteResource(getResource);
                            continue;
                        }
                        if (RemovingLotThumbnails && getResource.ResourceType == 3629023174U)
                        {
                            pkg.DeleteResource(getResource);
                            continue;
                        }
                        if (DeletingCharacterPortraits && getResource.ResourceType == 92316365U |
                            getResource.ResourceType == 92316366U | getResource.ResourceType == 92316367U)
                        {
                            pkg.DeleteResource(getResource);
                            continue;
                        }
                        if (getResource.Compressed == 0) getResource.Compressed = ushort.MaxValue;
                    }

                    pkg.SavePackage();
                    Package.ClosePackage(pkg);
                    checked { ++index3; }
                }
            }

            w.Stop();
            e.Result = w.Elapsed.TotalSeconds;
        }
    }
}