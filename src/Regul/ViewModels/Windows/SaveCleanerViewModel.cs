using Avalonia;
using Avalonia.Controls;
using Regul.Core;
using Regul.S3PI.Interfaces;
using Regul.S3PI.Package;
using Regul.Views.Controls.ListBoxItems;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using OlibUI.Windows;
using System.Collections.Generic;
using OlibUI.Structures;
using System.Linq;
using RegulSaveCleaner.Structures;

namespace Regul.ViewModels.Windows
{
    internal sealed class SaveCleanerViewModel : ViewModelBase
    {
        private string _pathBackup;
        private bool _isLoading;
        private bool _isIndeterminate;

        private ObservableCollection<SaveFilePortrait> _saveFilePortraits = new ObservableCollection<SaveFilePortrait>();
        private ObservableCollection<SaveFilePortrait> _selectedSaves = new ObservableCollection<SaveFilePortrait>();

        private bool _deletingCharacterPortraits;
        private bool _removingLotThumbnails;
        private bool _removingTextures;
        private bool _removingPhotos;
        private bool _removingGeneratedImages;
        private bool _removingFamilyPortraits;
        private bool _createABackup;
        private bool _clearCache;

        [AccessedThroughProperty("bgwClean")] private BackgroundWorker _bgwClean;
        [AccessedThroughProperty("bgwClearMemory")] private BackgroundWorker _bgwClearMemory;

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

        private bool IsIndeterminate
        {
            get => _isIndeterminate;
            set => RaiseAndSetIfChanged(ref _isIndeterminate, value);
        }

        private ObservableCollection<SaveFilePortrait> SaveFilePortraits
        {
            get => _saveFilePortraits;
            set => RaiseAndSetIfChanged(ref _saveFilePortraits, value);
        }

        private ObservableCollection<SaveFilePortrait> SelectedSaves
        {
            get => _selectedSaves;
            set => RaiseAndSetIfChanged(ref _selectedSaves, value);
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
        private bool ClearCache
        {
            get => _clearCache;
            set => RaiseAndSetIfChanged(ref _clearCache, value);
        }

        #endregion

        public SaveCleanerViewModel()
        {
            bgwClean = new BackgroundWorker();
            bgwClearMemory = new BackgroundWorker();

            Initialize();
        }

        private void ChooseAll()
        {
            DeletingCharacterPortraits = true;
            RemovingLotThumbnails = true;
            RemovingPhotos = true;
            RemovingTextures = true;
            RemovingFamilyPortraits = true;
            RemovingGeneratedImages = true;
        }

        private void CancelAll()
        {
            DeletingCharacterPortraits = false;
            RemovingLotThumbnails = false;
            RemovingPhotos = false;
            RemovingTextures = false;
            RemovingFamilyPortraits = false;
            RemovingGeneratedImages = false;
        }

        public void Exit()
        {
            Program.Settings.DeletingCharacterPortraits = DeletingCharacterPortraits;
            Program.Settings.RemovingLotThumbnails = RemovingLotThumbnails;
            Program.Settings.RemovingPhotos = RemovingPhotos;
            Program.Settings.RemovingTextures = RemovingTextures;
            Program.Settings.RemovingFamilyPortraits = RemovingFamilyPortraits;
            Program.Settings.RemovingGeneratedImages = RemovingGeneratedImages;
            Program.Settings.CreateABackup = CreateABackup;
            Program.Settings.ClearCache = ClearCache;
        }

        private void Initialize()
        {
            DeletingCharacterPortraits = Program.Settings.DeletingCharacterPortraits;
            RemovingLotThumbnails = Program.Settings.RemovingLotThumbnails;
            RemovingPhotos = Program.Settings.RemovingPhotos;
            RemovingTextures = Program.Settings.RemovingTextures;
            RemovingFamilyPortraits = Program.Settings.RemovingFamilyPortraits;
            RemovingGeneratedImages = Program.Settings.RemovingGeneratedImages;
            CreateABackup = Program.Settings.CreateABackup;
            ClearCache = Program.Settings.ClearCache;

            LoadingSaves();
        }

        public async void LoadingSaves()
        {
            if (IsLoading) return;

            List<string> saves = new List<string>();

            for (int i = 0; i < SelectedSaves.Count; i++)
                saves.Add(SelectedSaves[i].SaveName.Text);

            SelectedSaves.Clear();
            SaveFilePortraits.Clear();

            if (!Directory.Exists(Program.Settings.PathToTheSims3Document))
            {
                await MessageBox.Show(App.MainWindow,
                    (string)Application.Current.FindResource("NotFindFolderTheSims3"),
                    (string)Application.Current.FindResource("Information"), null,
                    MessageBox.MessageBoxIcon.Information,
                    new List<MessageBoxButton>
                    {
                        new MessageBoxButton { IsKeyDown = true, Result = "OK", Text = (string) Application.Current.FindResource("OK") }
                    });

                OpenFolderDialog dialog = new OpenFolderDialog();
                dialog.Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string path = await dialog.ShowAsync(App.MainWindow);
                if (!string.IsNullOrEmpty(path))
                {
                    Program.Settings.PathToTheSims3Document = path;

                    Program.Settings.PathToSaves = Path.Combine(Program.Settings.PathToTheSims3Document, "Saves");

                    LoadingSaves();
                }

                return;
            }

            try
            {
                if (!Directory.Exists(Program.Settings.PathToSaves))
                {
                    await MessageBox.Show(App.MainWindow,
                        (string)Application.Current.FindResource("SaveFilesNotFound"),
                        (string)Application.Current.FindResource("Information"), null,
                        MessageBox.MessageBoxIcon.Information,
                        new List<MessageBoxButton>
                        {
                            new MessageBoxButton
                            {
                                IsKeyDown = true, Result = "OK", Text = (string) Application.Current.FindResource("OK")
                            }
                        });

                    OpenFolderDialog dialog = new OpenFolderDialog();
                    dialog.Directory = Program.Settings.PathToTheSims3Document;
                    string path = await dialog.ShowAsync(App.MainWindow);
                    if (!string.IsNullOrEmpty(path))
                    {
                        Program.Settings.PathToSaves = path;

                        LoadingSaves();
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                await MessageBox.Show(App.MainWindow, (string)Application.Current.FindResource("AnErrorHasOccurred"),
                    (string)Application.Current.FindResource("Error"), ex.ToString(),
                    MessageBox.MessageBoxIcon.Error,
                    new List<MessageBoxButton>
                    {
                        new MessageBoxButton { IsKeyDown = true, Result = "OK", Text = (string) Application.Current.FindResource("OK") }
                    });

                return;
            }

            foreach (string directory in Directory.EnumerateDirectories(Program.Settings.PathToSaves, "*.sims3", SearchOption.TopDirectoryOnly))
                foreach (string file in Directory.EnumerateFiles(directory, "*.nhd", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        Save save = new Save(file);

                        SaveFilePortrait saveFilePortrait = new SaveFilePortrait(directory, save);

                        if (file.IndexOf(saveFilePortrait.Location) != -1)
                        {
                            saveFilePortrait.IconFamily.Source = save.FamilyIcon;
                            saveFilePortrait.SaveFamily.Text = save.WorldName;
                            saveFilePortrait.ImgInstance = save.ImgInstance;
                            SaveFilePortraits.Add(saveFilePortrait);
                        }
                    }
                    catch (Exception ex)
                    {
                        await MessageBox.Show(App.MainWindow,
                            (string)Application.Current.FindResource("AnErrorHasOccurred"),
                            (string)Application.Current.FindResource("Error"), ex.ToString(),
                            MessageBox.MessageBoxIcon.Error,
                            new List<MessageBoxButton>
                            {
                            new MessageBoxButton
                            {
                                IsKeyDown = true, Result = "OK",
                                Text = (string) Application.Current.FindResource("OK")
                            }
                            });
                    }
                }

            if (SaveFilePortraits.Count == 0)
            {
                await MessageBox.Show(App.MainWindow, (string)Application.Current.FindResource("SaveFilesNotFound"),
                    (string)Application.Current.FindResource("Information"), null,
                    MessageBox.MessageBoxIcon.Information,
                    new List<MessageBoxButton>
                    {
                        new MessageBoxButton
                            {IsKeyDown = true, Result = "OK", Text = (string) Application.Current.FindResource("OK")}
                    });
            }

            foreach (string item in saves)
            {
                SelectedSaves.Add(SaveFilePortraits.FirstOrDefault(x => x.SaveName.Text == item));
            }
        }

        private async void SelectPath() => PathBackup = await new OpenFolderDialog().ShowAsync(App.MainWindow) + "\\";

        private async void Clear()
        {
            try
            {
                if (SelectedSaves.Count < 0)
                {
                    await MessageBox.Show(App.MainWindow, (string)Application.Current.FindResource("NoSelectedSaveFile"), (string)Application.Current.FindResource("Error"), null, MessageBox.MessageBoxIcon.Error,
                        new List<MessageBoxButton>
                        {
                            new MessageBoxButton
                            {
                                IsKeyDown = true, Result = "OK", Text = (string) Application.Current.FindResource("OK")
                            }
                        });
                    return;
                }
                IsLoading = true;
                bgwClean.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                await MessageBox.Show(App.MainWindow, (string)Application.Current.FindResource("AnErrorHasOccurred"), (string)Application.Current.FindResource("Error"), ex.ToString(), MessageBox.MessageBoxIcon.Error,
                    new List<MessageBoxButton>
                    {
                        new MessageBoxButton
                            {IsKeyDown = true, Result = "OK", Text = (string) Application.Current.FindResource("OK")}
                    });
            }
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
            App.SaveCleaner.ProgressText = (string)e.UserState;
            App.SaveCleaner.ProgressLoad = e.ProgressPercentage;
        }

        private void bgwClean_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsLoading = false;
            GC.Collect();

            List<CleanerResult> results = (List<CleanerResult>)e.Result;
            string s = "";

            for (var index = 0; index < results.Count; index++)
            {
                CleanerResult cleanerResult = results[index];
                s +=
                    $"{(string)Application.Current.FindResource("SaveNameC")} {cleanerResult.Save}" +
                    $"\n{(string)Application.Current.FindResource("TimePassedC")} {cleanerResult.TotalSecond} {(string)Application.Current.FindResource("Sec")}" +
                    $"\n\n{(string)Application.Current.FindResource("OldSizeC")} {cleanerResult.OldSize} MB" +
                    $"\n{(string)Application.Current.FindResource("NewSizeC")} {cleanerResult.NewSize} MB" +
                    $"\n{(string)Application.Current.FindResource("PercentC")} {(cleanerResult.OldSize - cleanerResult.NewSize) / cleanerResult.OldSize:P}";

                if (index + 1 < results.Count)
                    s += "\n-----------------------\n";
            }

            if (App.MainWindow.WindowState == WindowState.Minimized) App.MainWindow.WindowState = WindowState.Normal;

            MessageBox.Show(App.MainWindow, (string)Application.Current.FindResource("SaveFilesCleanedSuccessfully"),
                (string)Application.Current.FindResource("Successfully"), s, MessageBox.MessageBoxIcon.Information,
                new List<MessageBoxButton>
                {
                    new MessageBoxButton
                        {IsKeyDown = true, Result = "OK", Text = (string) Application.Current.FindResource("OK")}
                });
        }

        private void bgwClean_DoWork(object sender, DoWorkEventArgs e)
        {
            List<CleanerResult> cleanerResults = new List<CleanerResult>();
            for (int index1 = 0; index1 < SelectedSaves.Count; index1++)
            {
                SaveFilePortrait selectSave = SelectedSaves[index1];

                long num1 = 0;
                foreach (string file in
                    Directory.EnumerateFiles(selectSave.SaveDir, "*.*", SearchOption.AllDirectories))
                    num1 += new FileInfo(file).Length;

                Stopwatch w = new Stopwatch();
                w.Start();

                if (CreateABackup)
                {
                    bgwClean.ReportProgress(20, $"{selectSave.SaveName.Text}\n" + (string)Application.Current.FindResource("ProcessingCreateBackup"));
                    if (!string.IsNullOrEmpty(PathBackup))
                        DirectoryCopy(selectSave.SaveDir, PathBackup + selectSave.SaveName.Text + ".sims3", true);
                }

                bgwClean.ReportProgress(40, $"{selectSave.SaveName.Text}\n" + (string)Application.Current.FindResource("ProcessingCompressingSave"));
                foreach (string file in Directory.EnumerateFiles(selectSave.SaveDir, "*.package",
                    SearchOption.AllDirectories))
                {
                    if (file == Path.Combine(selectSave.SaveDir, "TravelDB.package")) continue;

                    IPackage pkg = Package.OpenPackage(file, true);

                    for (int index = 0; index < pkg.GetResourceList.Count; index++)
                    {
                        IResourceIndexEntry getResource = pkg.GetResourceList[index];
                        if (getResource.Filesize != getResource.Memsize && getResource.Compressed == 0)
                            getResource.Compressed = ushort.MaxValue;
                    }

                    pkg.SavePackage();
                    Package.ClosePackage(pkg);
                }

                bgwClean.ReportProgress(70, $"{selectSave.SaveName.Text}\n" + (string)Application.Current.FindResource("ProcessingClearingSave"));

                if (File.Exists(Path.Combine(selectSave.SaveDir, "TravelDB.package")) && RemovingGeneratedImages ||
                    RemovingPhotos)
                {
                    IPackage pkg1 = Package.OpenPackage(Path.Combine(selectSave.SaveDir, "TravelDB.package"), true);

                    for (int i = 0; i < pkg1.GetResourceList.Count; i++)
                    {
                        IResourceIndexEntry getResource = pkg1.GetResourceList[i];
                        if (RemovingGeneratedImages && getResource.ResourceType == 11720834U &&
                            (getResource.ResourceGroup == 38510538U) | (getResource.ResourceGroup == 41034393U) |
                            (getResource.ResourceGroup == 45967776U) ||
                            RemovingPhotos && getResource.ResourceType == 11720834U &&
                            getResource.ResourceGroup == 40488965U)
                        {
                            pkg1.DeleteResource(getResource);
                            continue;
                        }

                        if (getResource.Filesize != getResource.Memsize && getResource.Compressed == 0)
                            getResource.Compressed = ushort.MaxValue;
                    }

                    pkg1.SavePackage();
                    Package.ClosePackage(pkg1);
                }

                foreach (string file in Directory.EnumerateFiles(selectSave.SaveDir, "*.nhd",
                    SearchOption.AllDirectories))
                {
                    IPackage pkg2 = Package.OpenPackage(file, true);

                    for (int i1 = 0; i1 < pkg2.GetResourceList.Count; i1++)
                    {
                        IResourceIndexEntry getResource = pkg2.GetResourceList[i1];
                        if (RemovingFamilyPortraits && getResource.Instance != selectSave.ImgInstance &&
                            getResource.ResourceType == 1802339198U)
                        {
                            pkg2.DeleteResource(getResource);
                            continue;
                        }

                        if (RemovingGeneratedImages && getResource.ResourceType == 11720834U &&
                            (getResource.ResourceGroup == 38510538U) | (getResource.ResourceGroup == 41034393U) |
                            (getResource.ResourceGroup == 45967776U))
                        {
                            pkg2.DeleteResource(getResource);
                            continue;
                        }

                        if (RemovingPhotos && getResource.ResourceType == 11720834U &&
                            getResource.ResourceGroup == 40488965U)
                        {
                            pkg2.DeleteResource(getResource);
                            continue;
                        }

                        if (RemovingTextures && getResource.ResourceType == 11720834U &&
                            (getResource.ResourceGroup == 11584775U) | (getResource.ResourceGroup == 1U) |
                            (getResource.ResourceGroup == 12328524U) | (getResource.ResourceGroup == 1943529U) |
                            (getResource.ResourceGroup == 16441714U) | (getResource.ResourceGroup == 12328532U) |
                            (getResource.ResourceGroup == 8287573U))
                        {
                            pkg2.DeleteResource(getResource);
                            continue;
                        }

                        if (RemovingLotThumbnails && getResource.ResourceType == 3629023174U)
                        {
                            pkg2.DeleteResource(getResource);
                            continue;
                        }

                        if (DeletingCharacterPortraits && (getResource.ResourceType == 92316365U) |
                            (getResource.ResourceType == 92316366U) | (getResource.ResourceType == 92316367U))
                        {
                            pkg2.DeleteResource(getResource);
                            continue;
                        }

                        if (getResource.Filesize != getResource.Memsize && getResource.Compressed == 0)
                            getResource.Compressed = ushort.MaxValue;
                    }

                    pkg2.SavePackage();
                    Package.ClosePackage(pkg2);
                }

                w.Stop();

                long num2 = 0;
                foreach (string file in
                    Directory.EnumerateFiles(selectSave.SaveDir, "*.*", SearchOption.AllDirectories))
                    num2 += new FileInfo(file).Length;

                cleanerResults.Add(new CleanerResult(num1 * 0.0009765625 * 0.0009765625,
                    num2 * 0.0009765625 * 0.0009765625, w.Elapsed.TotalSeconds, selectSave.SaveName.Text));
            }


            if (ClearCache)
            {
                try
                {
                    bgwClean.ReportProgress(85, (string)Application.Current.FindResource("ProcessingClearingCache"));

                    if (File.Exists(Path.Combine(Program.Settings.PathToTheSims3Document, "CASPartCache.package")))
                        File.Delete(Path.Combine(Program.Settings.PathToTheSims3Document, "CASPartCache.package"));

                    if (File.Exists(Path.Combine(Program.Settings.PathToTheSims3Document, "compositorCache.package")))
                        File.Delete(Path.Combine(Program.Settings.PathToTheSims3Document, "compositorCache.package"));

                    if (File.Exists(Path.Combine(Program.Settings.PathToTheSims3Document, "scriptCache.package")))
                        File.Delete(Path.Combine(Program.Settings.PathToTheSims3Document, "scriptCache.package"));

                    if (File.Exists(Path.Combine(Program.Settings.PathToTheSims3Document, "simCompositorCache.package")))
                        File.Delete(Path.Combine(Program.Settings.PathToTheSims3Document, "simCompositorCache.package"));

                    if (File.Exists(Path.Combine(Program.Settings.PathToTheSims3Document, "socialCache.package")))
                        File.Delete(Path.Combine(Program.Settings.PathToTheSims3Document, "socialCache.package"));

                    if (Directory.Exists(Path.Combine(Program.Settings.PathToTheSims3Document, "WorldCaches")))
                        Directory.Delete(Path.Combine(Program.Settings.PathToTheSims3Document, "WorldCaches"), true);

                    if (Directory.Exists(Path.Combine(Program.Settings.PathToTheSims3Document, "IGACache")))
                        Directory.Delete(Path.Combine(Program.Settings.PathToTheSims3Document, "IGACache"), true);

                    Directory.CreateDirectory(Path.Combine(Program.Settings.PathToTheSims3Document, "IGACache"));

                    try
                    {
                        if (Directory.Exists(Path.Combine(Program.Settings.PathToTheSims3Document, "FeaturedItems")))
                            Directory.Delete(Path.Combine(Program.Settings.PathToTheSims3Document, "FeaturedItems"), true);
                    }
                    catch { }

                    foreach (string file in Directory.EnumerateFiles(Program.Settings.PathToTheSims3Document, "*.xml"))
                        File.Delete(file);

                    if (File.Exists(Path.Combine(Program.Settings.PathToTheSims3Document, "DCCache\\missingdeps.idx")))
                        File.Delete(Path.Combine(Program.Settings.PathToTheSims3Document, "DCCache\\missingdeps.idx"));

                    if (File.Exists(Path.Combine(Program.Settings.PathToTheSims3Document, "DCCache\\dcc.ent")))
                        File.Delete(Path.Combine(Program.Settings.PathToTheSims3Document, "DCCache\\dcc.ent"));

                    foreach (string file in Directory.EnumerateFiles(
                        Path.Combine(Program.Settings.PathToTheSims3Document, "Downloads"), "*.bin"))
                        File.Delete(file);

                    foreach (string file in Directory.EnumerateFiles(Path.Combine(Program.Settings.PathToTheSims3Document, "SigsCache"), "*.bin"))
                        File.Delete(file);

                    if (File.Exists(Path.Combine(Program.Settings.PathToTheSims3Document, "SavedSims\\Downloadedsims.index")))
                        File.Delete(Path.Combine(Program.Settings.PathToTheSims3Document, "SavedSims\\Downloadedsims.index"));
                }
                catch { }
            }

            e.Result = cleanerResults;
        }

        internal BackgroundWorker bgwClearMemory
        {
            get => _bgwClearMemory;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                ProgressChangedEventHandler changedEventHandler = bgwClearMemory_ProgressChanged;
                RunWorkerCompletedEventHandler completedEventHandler = bgwClearMemory_RunWorkerCompleted;
                DoWorkEventHandler workEventHandler = bgwClearMemory_DoWork;

                if (_bgwClearMemory != null)
                {
                    _bgwClearMemory.ProgressChanged -= changedEventHandler;
                    _bgwClearMemory.RunWorkerCompleted -= completedEventHandler;
                    _bgwClearMemory.DoWork -= workEventHandler;
                }

                _bgwClearMemory = value;

                if (_bgwClearMemory == null) return;

                _bgwClearMemory.ProgressChanged += changedEventHandler;
                _bgwClearMemory.RunWorkerCompleted += completedEventHandler;
                _bgwClearMemory.DoWork += workEventHandler;

                _bgwClearMemory.WorkerReportsProgress = true;
            }
        }

        private void bgwClearMemory_ProgressChanged(object sender, ProgressChangedEventArgs e) => App.MainWindow.ProgressText = (string)e.UserState;

        private void bgwClearMemory_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsLoading = false;
            IsIndeterminate = false;
            GC.Collect();

            List<CleanerResult> results = (List<CleanerResult>)e.Result;
            string s = "";

            for (var index = 0; index < results.Count; index++)
            {
                CleanerResult cleanerResult = results[index];
                s +=
                    $"{(string)Application.Current.FindResource("SaveNameC")} {cleanerResult.Save}" +
                    $"\n{(string)Application.Current.FindResource("TimePassedC")} {cleanerResult.TotalSecond} {(string)Application.Current.FindResource("Sec")}" +
                    $"\n\n{(string)Application.Current.FindResource("OldSizeC")} {cleanerResult.OldSize} MB" +
                    $"\n{(string)Application.Current.FindResource("NewSizeC")} {cleanerResult.NewSize} MB" +
                    $"\n{(string)Application.Current.FindResource("PercentC")} {(cleanerResult.OldSize - cleanerResult.NewSize) / cleanerResult.OldSize:P}";

                if (index + 1 < results.Count)
                    s += "\n-----------------------\n";
            }

            if (App.MainWindow.WindowState == WindowState.Minimized) App.MainWindow.WindowState = WindowState.Normal;

            MessageBox.Show(App.MainWindow, (string)Application.Current.FindResource("MemoriesDeleted"),
                (string)Application.Current.FindResource("Successfully"), s, MessageBox.MessageBoxIcon.Information,
                new List<MessageBoxButton>
                {
                    new MessageBoxButton
                        {IsKeyDown = true, Result = "OK", Text = (string) Application.Current.FindResource("OK")}
                });
        }

        private void bgwClearMemory_DoWork(object sender, DoWorkEventArgs e)
        {
            List<CleanerResult> cleanerResults = new List<CleanerResult>();
            foreach (SaveFilePortrait selectSave in SelectedSaves)
            {
                bgwClearMemory.ReportProgress(0, $"{selectSave.SaveName.Text}\n" + (string)Application.Current.FindResource("ProcessingClearingMemories"));

                long num1 = 0;
                foreach (string file in Directory.EnumerateFiles(selectSave.SaveDir, "*.*", SearchOption.AllDirectories))
                    num1 += new FileInfo(file).Length;

                Stopwatch w = new Stopwatch();
                w.Start();

                foreach (string file in Directory.EnumerateFiles(selectSave.SaveDir, "*.nhd", SearchOption.AllDirectories))
                {
                    IPackage pkg = Package.OpenPackage(file, true);

                    try
                    {
                        foreach (IResourceIndexEntry getResource in pkg.GetResourceList)
                        {
                            if (getResource.Memsize == 174904U)
                                pkg.DeleteResource(getResource);
                        }
                    }
                    catch { }

                    pkg.SavePackage();
                    Package.ClosePackage(pkg);
                }

                foreach (string file in Directory.EnumerateFiles(selectSave.SaveDir, "*.package", SearchOption.AllDirectories))
                {
                    IPackage pkg = Package.OpenPackage(file, true);

                    try
                    {
                        foreach (IResourceIndexEntry getResource in pkg.GetResourceList)
                        {
                            if (getResource.Memsize == 174904U)
                                pkg.DeleteResource(getResource);
                        }
                    }
                    catch { }

                    pkg.SavePackage();
                    Package.ClosePackage(pkg);
                }

                w.Stop();

                long num2 = 0;
                foreach (string file in Directory.EnumerateFiles(selectSave.SaveDir, "*.*", SearchOption.AllDirectories))
                    num2 += new FileInfo(file).Length;

                cleanerResults.Add(new CleanerResult(num1 * 0.0009765625 * 0.0009765625,
                    num2 * 0.0009765625 * 0.0009765625, w.Elapsed.TotalSeconds, selectSave.SaveName.Text));
            }

            e.Result = cleanerResults;
        }
    }
}