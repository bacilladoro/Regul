using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using OlibUI.Structures;
using OlibUI.Windows;
using Regul.Core.Interfaces;
using Regul.Core.TheSims3Type;
using Regul.S3PI;
using Regul.S3PI.Extensions;
using Regul.S3PI.Interfaces;
using Regul.S3PI.Package;
using Regul.ViewModels.Windows.TheSims3Type;
using Regul.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Regul.ViewModels.Controls.ContentTab
{
    internal class TheSims3TypeContentViewModel : ViewModelBase, IPackageType
    {
        private ObservableCollection<Resource> _resources = new ObservableCollection<Resource>();
        private ObservableCollection<Resource> _globalResources = new ObservableCollection<Resource>();
        private List<Resource> _selectedResources = new List<Resource>();

        private uint _resourceTypeView;
        private bool _openedMenu;
        private Bitmap _imageResource;
        private Resource _selectedResource;
        private bool _active;

        private bool _visibleImageViewer;
        private bool _visibleTextViewer;

        private bool _checkName;
        private bool _checkCompressed;
        private bool _checkTag;
        private bool _checkResourceType;
        private bool _checkResourceGroup;
        private bool _checkInstance;

        private string _name;
        private string _tag;
        private string _resourceType;
        private string _resourceGroup;
        private string _instance;
        private string _compressed;

        private string _textPreview;

        [Serializable]
        public struct DataFormat
        {
            public TGIN TGIN;
            public byte[] Data;
        }

        #region CheckBoxs

        private bool CheckName
        {
            get => _checkName;
            set
            {
                RaiseAndSetIfChanged(ref _checkName, value);
                SearchResources();
            }
        }
        private bool CheckTag
        {
            get => _checkTag;
            set
            {
                RaiseAndSetIfChanged(ref _checkTag, value);
                SearchResources();
            }
        }
        private bool CheckResourceType
        {
            get => _checkResourceType;
            set
            {
                RaiseAndSetIfChanged(ref _checkResourceType, value);
                SearchResources();
            }
        }
        private bool CheckResourceGroup
        {
            get => _checkResourceGroup;
            set
            {
                RaiseAndSetIfChanged(ref _checkResourceGroup, value);
                SearchResources();
            }
        }
        private bool CheckInstance
        {
            get => _checkInstance;
            set
            {
                RaiseAndSetIfChanged(ref _checkInstance, value);
                SearchResources();
            }
        }
        private bool CheckCompressed
        {
            get => _checkCompressed;
            set
            {
                RaiseAndSetIfChanged(ref _checkCompressed, value);
                SearchResources();
            }
        }

        #endregion

        #region DataForFilter

        private string Name
        {
            get => _name;
            set
            {
                RaiseAndSetIfChanged(ref _name, value);
                SearchResources();
            }
        }
        private string Tag
        {
            get => _tag;
            set
            {
                RaiseAndSetIfChanged(ref _tag, value);
                SearchResources();
            }
        }
        private string ResourceType
        {
            get => _resourceType;
            set
            {
                RaiseAndSetIfChanged(ref _resourceType, value);
                SearchResources();
            }
        }
        private string ResourceGroup
        {
            get => _resourceGroup;
            set
            {
                RaiseAndSetIfChanged(ref _resourceGroup, value);
                SearchResources();
            }
        }
        private string Instance
        {
            get => _instance;
            set
            {
                RaiseAndSetIfChanged(ref _instance, value);
                SearchResources();
            }
        }
        private string Compressed
        {
            get => _compressed;
            set
            {
                RaiseAndSetIfChanged(ref _compressed, value);
                SearchResources();
            }
        }

        #endregion

        #region Viewers
        public bool VisibleImageViewer
        {
            get => _visibleImageViewer;
            set => RaiseAndSetIfChanged(ref _visibleImageViewer, value);
        }
        public bool VisibleTextViewer
        {
            get => _visibleTextViewer;
            set => RaiseAndSetIfChanged(ref _visibleTextViewer, value);
        }

        public string TextPreview
        {
            get => _textPreview;
            set => RaiseAndSetIfChanged(ref _textPreview, value);
        }
        #endregion

        public bool OpenedMenu
        {
            get => _openedMenu;
            set => RaiseAndSetIfChanged(ref _openedMenu, value);
        }

        public uint ResourceTypeView
        {
            get => _resourceTypeView;
            set => RaiseAndSetIfChanged(ref _resourceTypeView, value);
        }

        public Bitmap ImageResource
        {
            get => _imageResource;
            set => RaiseAndSetIfChanged(ref _imageResource, value);
        }

        public List<Resource> SelectedResources
        {
            get => _selectedResources;
            set => RaiseAndSetIfChanged(ref _selectedResources, value);
        }
        // Hack, without it hotkeys will only be processed for the first tab
        public bool Active
        {
            get => _active;
            set => RaiseAndSetIfChanged(ref _active, value);
        }

        public Resource SelectedResource
        {
            get => _selectedResource;
            set
            {
                RaiseAndSetIfChanged(ref _selectedResource, value);

                if (value == null || SelectedResources.Count > 1)
                {
                    VisibleImageViewer = false;
                    VisibleTextViewer = false;

                    return;
                }

                switch (value.Tag)
                {
                    case "SNAP":
                    case "THUM":
                    case "TWNI":
                    case "ICON":
                    case "IMAG":
                        VisibleImageViewer = true;
                        VisibleTextViewer = false;

                        ImageResource = new Bitmap(WrapperDealer.GetResource(CurrentPackage, value.ResourceIndexEntry).Stream);
                        break;

                    case "_XML":
                    case "CNFG":
                    case "_CSS":
                    case "LAYO":
                    case "VOCE":
                    case "MIXR":
                    case "ITUN":
                    case "DMTR":
                    case "_INI":
                    case "SKIL":
                    case "PTRN":
                    case "BUFF":
                    case "RMLS":
                    case "TRIG":
                        VisibleImageViewer = false;
                        VisibleTextViewer = true;

                        IResource res = WrapperDealer.GetResource(CurrentPackage, value.ResourceIndexEntry);
                        
                        if (res.Stream != null) TextPreview = new StreamReader(res.Stream).ReadToEnd();

                        break;
                    default:
                        VisibleImageViewer = false;
                        VisibleTextViewer = true;

                        TextPreview = "none";
                        
                        break;
                }
            }
        }

        public ObservableCollection<Resource> Resources
        {
            get => _resources;
            set => RaiseAndSetIfChanged(ref _resources, value);
        }
        public ObservableCollection<Resource> GlobalResources
        {
            get => _globalResources;
            set
            {
                RaiseAndSetIfChanged(ref _globalResources, value);
                SearchResources();
            }
        }

        public IPackage CurrentPackage { get; set; }

        public string PackageType { get; } = nameof(TheSims3TypeContentViewModel);

        public TheSims3TypeContentViewModel()
        {
            CurrentPackage = Package.NewPackage();
        }

        public TheSims3TypeContentViewModel(string path)
        {
            CurrentPackage = Package.OpenPackage(path, true);
            for (int i = 0; i < CurrentPackage.GetResourceList.Count; i++)
            {
                IResourceIndexEntry item = CurrentPackage.GetResourceList[i];
                GlobalResources.Add(new Resource
                {
                    ResourceIndexEntry = item,
                    Tag = ResourceTag(item),
                    ResourceName = ResourceName(item)
                });
            }
            Resources = GlobalResources;
        }

        private async void AddResource()
        {
            if (!Active) return;

            App.ResourceDetails = new Views.Windows.TheSims3Type.ResourceDetails();

            bool result = await App.ResourceDetails.ShowDialog<bool>(App.MainWindow);

            if (result)
            {
                IResourceIndexEntry rie = NewResource((ResourceDetailsViewModel)App.ResourceDetails.DataContext, null,
                    ((ResourceDetailsViewModel)App.ResourceDetails.DataContext).Replace ? DuplicateHandling.Replace : DuplicateHandling.Reject,
                    ((ResourceDetailsViewModel)App.ResourceDetails.DataContext).Compress);

                if (rie == null) return;

                GlobalResources.Add(new Resource
                {
                    Tag = ResourceTag(rie),
                    ResourceName = ResourceName(rie),
                    ResourceIndexEntry = rie
                });

                SearchResources();
            }
        }

        private void CopyResource()
        {
            if (!Active || SelectedResources.Count == 0) return;

            try
            {
                if (SelectedResources.Count == 1)
                {
                    DataFormat d = new DataFormat();
                    d.TGIN = SelectedResource.ResourceIndexEntry as AResourceIndexEntry;
                    d.TGIN.ResName = SelectedResource.ResourceName;
                    d.Data = WrapperDealer.GetResource(CurrentPackage, SelectedResource.ResourceIndexEntry, true).AsBytes;
                    IFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    formatter.Serialize(ms, d);
                    IDataObject obj = new DataObject();
                }
            }
            catch { }


            //try
            //{
            //    DataFormat d = new DataFormat();
            //    d.TGIN = SelectedResource.ResourceIndexEntry as AResourceIndexEntry;
            //    d.TGIN.ResName = SelectedResource.ResourceName;
            //    d.Data = WrapperDealer.GetResource(CurrentPackage,
            //        SelectedResource.ResourceIndexEntry, true).AsBytes;

            //    IFormatter formatter = new BinaryFormatter();
            //    MemoryStream ms = new MemoryStream();
            //    formatter.Serialize(ms, d);
            //    IDataObject dataObject = new DataObject();

            //    Application.Current.Clipboard.SetDataObjectAsync()
            //}
        }
        private void PasteResource()
        {
            if (!Active) return;

        }

        public async void ResourceDetails()
        {
            if (!Active || SelectedResource == null) return;

            App.ResourceDetails = new Views.Windows.TheSims3Type.ResourceDetails();

            bool result = await App.ResourceDetails.ShowDialog<bool>(App.MainWindow);

            if (result)
            {

            }
        }

        private void ImportFromFile()
        {
            if (!Active) return;

        }

        private void DublicateResource()
        {
            if (SelectedResource == null) return;

            //byte[] buffer = SelectedResource.ResourceIndexEntry.

            //IResourceIndexEntry rie = CurrentPackage.AddResource(SelectedResource.ResourceIndexEntry, )
        }

        private void DeleteResource()
        {
            CurrentPackage.DeleteResource(SelectedResource.ResourceIndexEntry);
            GlobalResources.Remove(SelectedResource);

            SearchResources();
        }

        private void ResourceCompressed()
        {
            ushort target = 0xFFFF;

            if (CompressedCheck()) target = 0;
            SelectedResource.ResourceIndexEntry.Compressed = target;
        }

        private void CopyRK()
        {
            if (SelectedResources.Count != 1) return;
            Application.Current.Clipboard.SetTextAsync(string.Join("\r\n", SelectedResources.Select(r => r.ResourceIndexEntry.ToString())));
        }

        private byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private IResource ReadResource(string fileName)
        {
            MemoryStream ms = new MemoryStream();
            using (BinaryReader br = new BinaryReader(new FileStream(fileName, FileMode.Open)))
            {
                ms.Write(br.ReadBytes((int)br.BaseStream.Length), 0, (int)br.BaseStream.Length);
                br.Close();
            }

            IResource rres = WrapperDealer.CreateNewResource("*");
            ConstructorInfo ci = rres.GetType().GetConstructor(new Type[] { typeof(int), typeof(Stream) });
            return (IResource)ci.Invoke(new object[] { 0, ms });
        }

        private void ExportResource()
        {

        }

        private async void ExportBatch()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            string path = await dialog.ShowAsync(App.MainWindow);

            if (string.IsNullOrEmpty(path)) return;

            bool overwriteAll = false;
            bool skipAll = false;
            try
            {
                foreach (IResourceIndexEntry rie in SelectedResources)
                {
                    TGIN tgin = rie as AResourceIndexEntry;
                    tgin.ResName = ResourceName(rie);
                    string file = Path.Combine(path, tgin);
                    if (File.Exists(file))
                    {
                        if (skipAll) continue;
                        if (!overwriteAll)
                        {
                            //string res = await MessageBox.Show(App.MainWindow, null, "Overwrite file?\n" + file, "Question", MessageBox.MessageBoxButtons.NoNoToAllYesYesToAllAbandon,
                            //    MessageBox.MessageBoxIcon.Question);

                            string res = await MessageBox.Show(App.MainWindow, "Overwrite file?\n" + file, "Question", null, MessageBox.MessageBoxIcon.Question, 
                                new List<MessageBoxButton> 
                                { 
                                    new MessageBoxButton { Result = "Yes", Text = "Yes" },
                                    new MessageBoxButton { Result = "YesToAll", Text = "Yes to all" },
                                    new MessageBoxButton { Result = "No", Text = "No" },
                                    new MessageBoxButton { Result = "NoToAll", Text = "NoToAll" },
                                    new MessageBoxButton { Result = "Abandon", Text = "Abandon" },
                                    
                                });

                            if (res == "No") continue;
                            if (res == "NoToAll") { skipAll = true; continue; }
                            if (res == "YesToAll") overwriteAll = true;
                            if (res == "Abandon") return;
                        }
                    }
                    ExportFile(rie, file);
                }
            }
            finally { }
        }

        private void ExportFile(IResourceIndexEntry rie, string fileName)
        {
            IResource res = WrapperDealer.GetResource(CurrentPackage, rie, true);
            Stream s = res.Stream;
            s.Position = 0;

            BinaryWriter w = new BinaryWriter(new FileStream(fileName, FileMode.Create));
            w.Write(new BinaryReader(s).ReadBytes((int)s.Length));
            w.Close();
        }

        private string ResourceName(IResourceIndexEntry rie)
        {
            //IResource res = S3PI.WrapperDealer.GetResource(0, CurrentPackage, rie);
            //if (res == null) return "";
            //List<IResource> ress = new List<IResource>();
            //ress.Add(res);

            //foreach (S3PI.DefaultResource.DefaultResource def in ress)
            //{
            //    return def.ToString();
            //}
            return "";
        }

        private enum DuplicateHandling
        {
            /// <summary>
            /// Refuse to create the request resource
            /// </summary>
            Reject,
            /// <summary>
            /// Delete any conflicting resource
            /// </summary>
            Replace,
            /// <summary>
            /// Ignore any conflicting resource
            /// </summary>
            Allow
        }

        private string ResourceTag(IResourceIndexEntry rie)
        {
            string key = rie["ResourceType"];
            if (ExtList.Ext.ContainsKey(key)) return ExtList.Ext[key][0];
            if (ExtList.Ext.ContainsKey("*")) return ExtList.Ext["*"][0];
            return "";
        }

        private bool CompressedCheck()
        {
            if (SelectedResource == null) return false;
            else
                return SelectedResource.ResourceIndexEntry.Compressed != 0;
        }

        private IResourceIndexEntry NewResource(IResourceKey rk, MemoryStream ms, DuplicateHandling dups, bool compress)
        {
            IResourceIndexEntry rie = CurrentPackage.Find(x => rk.Equals(x));

            if (rie != null)
            {
                if (dups == DuplicateHandling.Reject) return null;
                if (dups == DuplicateHandling.Replace) CurrentPackage.DeleteResource(rie);
            }

            rie = CurrentPackage.AddResource(rk, ms, false);
            if (rie == null) return null;

            if (compress)
                rie.Compressed = 0xFFFF;
            else rie.Compressed = 0;

            return rie;
        }

        private void SearchResources()
        {
            if (!CheckName && !CheckTag && !CheckResourceType && !CheckResourceGroup && !CheckInstance && !CheckCompressed)
            {
                Resources = GlobalResources;
                return;
            }

            ObservableCollection<Resource> res = new ObservableCollection<Resource>();

            //List<Resource> resName = new(), resTag = new(), resType = new(), resGroup = new(), resInstance = new(), resCompressed = new();


            //List<Resource> complete = new();

            //if (CheckName)
            //    resName = GlobalResources.ToList().FindAll(x => x.ResourceName.ToLower().Contains(Name.ToLower()));
            //if (CheckTag)
            //    resTag = GlobalResources.ToList().FindAll(x => x.Tag == Tag);
            //if (CheckResourceType)
            //    resType = GlobalResources.ToList().FindAll(x => x.ResourceIndexEntry.ResourceType == uint.Parse(ResourceType));
            //if (CheckResourceGroup)
            //    resGroup = GlobalResources.ToList().FindAll(x => x.ResourceIndexEntry.ResourceGroup == uint.Parse(ResourceGroup));
            //if (CheckInstance)
            //    resInstance = GlobalResources.ToList().FindAll(x => x.ResourceIndexEntry.Instance == ulong.Parse(Instance));
            //if (CheckCompressed)
            //    resCompressed = GlobalResources.ToList().FindAll(x => x.ResourceIndexEntry.Compressed == ushort.Parse(Compressed));


            //complete = complete.Concat(resName).ToList();
            //complete = complete.Concat(resTag).ToList();
            //complete = complete.Concat(resType).ToList();
            //complete = complete.Concat(resGroup).ToList();
            //complete = complete.Concat(resInstance).ToList();
            //complete = complete.Concat(resCompressed).ToList();

            //Resources = new ObservableCollection<Resource>(complete);
        }

        public void SavePackage()
        {
        }
    }
}
