﻿using Avalonia;
using Avalonia.Controls;
using Microsoft.VisualBasic;
using ReactiveUI;
using Regul.S3PI;
using Regul.S3PI.Interfaces;
using Regul.Structures;
using Regul.Views;
using Regul.Views.Controls.ListBoxItems;
using Regul.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Regul.ViewModels.Windows
{
    public class SaveClearViewModel : ReactiveObject
    {
        private string _pathBackup;
        private bool _isLoading;
        private ObservableCollection<SaveFilePortrait> _saveFilePortraits = new();
        private SaveFilePortrait _selectSave;
        private Loading _loading;

        private bool _deletingCharacterPortraits;
        private bool _removingLotThumbnails;
        private bool _removingPhotosAndTextures;
        private bool _createABackup;

        [AccessedThroughProperty("bgwClean")]
        private BackgroundWorker _bgwClean;

        #region Propertys

        private string PathBackup
        {
            get => _pathBackup;
            set => this.RaiseAndSetIfChanged(ref _pathBackup, value);
        }

        private bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
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
        private bool CreateABackup
        {
            get => _createABackup;
            set => this.RaiseAndSetIfChanged(ref _createABackup, value);
        }

        #endregion

        public SaveClearViewModel()
        {
            DeletingCharacterPortraits = true;
            bgwClean = new();

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

            string path1 = "";

            if (TS3CC.Sims3MyDocFolder != null)
            {
                path1 = TS3CC.Sims3MyDocFolder;
            }
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
                _loading = new();
                _loading.ShowDialog(App.SaveClear);
                bgwClean.RunWorkerAsync(SelectSave);
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
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

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
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
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
            _loading.ProcessText.Text = (string)e.UserState;
            _loading.Progress.Value = e.ProgressPercentage;
        }

        private void bgwClean_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _loading.Close();
            IsLoading = false;
            MessageBox.Show(App.SaveClear, null, (string)Application.Current.FindResource("SaveFilesCleanedSuccessfully"), 
                (string)Application.Current.FindResource("Successfully"), MessageBox.MessageBoxButtons.Ok, MessageBox.MessageBoxIcon.Information);
        }

        private void bgwClean_DoWork(object sender, DoWorkEventArgs e)
        {
            SaveFilePortrait saveFilePortrait = (SaveFilePortrait)e.Argument;

            if (CreateABackup)
            {
                bgwClean.ReportProgress(10, "Create Backup...");
                if (!string.IsNullOrEmpty(PathBackup))
                {
                    DirectoryCopy(saveFilePortrait.SaveDir, PathBackup + saveFilePortrait.SaveName.Text + ".sims3", true);
                }
            }

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

            if (Directory.GetFiles(saveFilePortrait.SaveDir, "*.nhd", SearchOption.AllDirectories).Length > 1)
            {
                bgwClean.ReportProgress(60, "Clearing Duplicated Images...");

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
                        S3PI.Package.Package.ClosePackage(0, pkg2);
                    }
                    checked { ++index3; }
                }
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
                    }
                    finally { }
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
        }
    }
}
