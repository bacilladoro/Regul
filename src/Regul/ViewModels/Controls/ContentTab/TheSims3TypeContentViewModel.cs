using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Regul.Core.TheSims3Type;
using Regul.S3PI;
using Regul.S3PI.Extensions;
using Regul.S3PI.Interfaces;
using Regul.S3PI.Package;

namespace Regul.ViewModels.Controls.ContentTab
{
    public class TheSims3TypeContentViewModel : ReactiveObject
    {
        private ObservableCollection<Resource> _resources = new();
        private ObservableCollection<Resource> _globalResources = new();

        private uint _resourceTypeView;
        private bool _openedMenu;
        private Bitmap _imageResource;
        private Resource _selectedResource;
        private int _selectedIndex;

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
                this.RaiseAndSetIfChanged(ref _checkName, value);
                SearchResources();
            }
        }
        private bool CheckTag
        {
            get => _checkTag;
            set
            {
                this.RaiseAndSetIfChanged(ref _checkTag, value);
                SearchResources();
            }
        }
        private bool CheckResourceType
        {
            get => _checkResourceType;
            set
            {
                this.RaiseAndSetIfChanged(ref _checkResourceType, value);
                SearchResources();
            }
        }
        private bool CheckResourceGroup
        {
            get => _checkResourceGroup;
            set
            {
                this.RaiseAndSetIfChanged(ref _checkResourceGroup, value);
                SearchResources();
            }
        }
        private bool CheckInstance
        {
            get => _checkInstance;
            set
            {
                this.RaiseAndSetIfChanged(ref _checkInstance, value);
                SearchResources();
            }
        }
        private bool CheckCompressed
        {
            get => _checkCompressed;
            set
            {
                this.RaiseAndSetIfChanged(ref _checkCompressed, value);
                SearchResources();
            }
        }

        #endregion
        #region MyRegion

        private string Name
        {
            get => _name;
            set
            {
                this.RaiseAndSetIfChanged(ref _name, value);
                SearchResources();
            }
        }
        private string Tag
        {
            get => _tag;
            set
            {
                this.RaiseAndSetIfChanged(ref _tag, value);
                SearchResources();
            }
        }
        private string ResourceType
        {
            get => _resourceType;
            set
            {
                this.RaiseAndSetIfChanged(ref _resourceType, value);
                SearchResources();
            }
        }
        private string ResourceGroup
        {
            get => _resourceGroup;
            set
            {
                this.RaiseAndSetIfChanged(ref _resourceGroup, value);
                SearchResources();
            }
        }
        private string Instance
        {
            get => _instance;
            set
            {
                this.RaiseAndSetIfChanged(ref _instance, value);
                SearchResources();
            }
        }
        private string Compressed
        {
            get => _compressed;
            set
            {
                this.RaiseAndSetIfChanged(ref _compressed, value);
                SearchResources();
            }
        }

        #endregion

        #region Viewers
        public bool VisibleImageViewer
        {
            get => _visibleImageViewer;
            set => this.RaiseAndSetIfChanged(ref _visibleImageViewer, value);
        }
        public bool VisibleTextViewer
        {
            get => _visibleTextViewer;
            set => this.RaiseAndSetIfChanged(ref _visibleTextViewer, value);
        }

        public string TextPreview
        {
            get => _textPreview;
            set => this.RaiseAndSetIfChanged(ref _textPreview, value);
        }
        #endregion

        public bool OpenedMenu
        {
            get => _openedMenu;
            set => this.RaiseAndSetIfChanged(ref _openedMenu, value);
        }

        public uint ResourceTypeView
        {
            get => _resourceTypeView;
            set => this.RaiseAndSetIfChanged(ref _resourceTypeView, value);
        }

        public Bitmap ImageResource
        {
            get => _imageResource;
            set => this.RaiseAndSetIfChanged(ref _imageResource, value);
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
        }

        public Resource SelectedResource
        {
            get => _selectedResource;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedResource, value);

                if (value == null) return;

                switch (value.Tag)
                {
                    case "SNAP":
                    case "_IMG":
                        VisibleImageViewer = true;
                        VisibleTextViewer = false;

                        ImageResource = new Bitmap(WrapperDealer.GetResource(0, CurrentPackage, value.ResourceIndexEntry).Stream);
                        break;
                    default:
                        VisibleImageViewer = false;
                        VisibleTextViewer = true;

                        IResource res = WrapperDealer.GetResource(0, CurrentPackage, value.ResourceIndexEntry);
                        string s = "";

                        foreach (string prop in res.ContentFields)
                        {
                            s += prop;
                        }

                        TextPreview = s;
                        break;
                }
            }
        }

        public ObservableCollection<Resource> Resources
        {
            get => _resources;
            set => this.RaiseAndSetIfChanged(ref _resources, value);
        }
        public ObservableCollection<Resource> GlobalResources
        {
            get => _globalResources;
            set
            {
                this.RaiseAndSetIfChanged(ref _globalResources, value);
                SearchResources();
            }
        }

        private IPackage CurrentPackage { get; set; }

        public TheSims3TypeContentViewModel()
        {
            CurrentPackage = Package.NewPackage(1);
        }

        public TheSims3TypeContentViewModel(string path)
        {
            CurrentPackage = Package.OpenPackage(1, path, true);
            foreach (IResourceIndexEntry item in CurrentPackage.GetResourceList)
            {
                Resource res = new Resource
                {
                    ResourceIndexEntry = item,
                    Tag = ResourceTag(item),
                    ResourceName = ResourceName(item)
                };
                GlobalResources.Add(res);
            }
            Resources = GlobalResources;
        }

        private async void AddResource()
        {
            App.ResourceDetails = new Views.Windows.TheSims3Type.ResourceDetails();

            bool result = await App.ResourceDetails.ShowDialog<bool>(App.MainWindow);

            if (result)
            {
                IResourceIndexEntry rie = NewResource(App.ResourceDetails.ViewModel, null,
                    App.ResourceDetails.ViewModel.Replace ? DuplicateHandling.Replace : DuplicateHandling.Reject,
                    App.ResourceDetails.ViewModel.Compress);

                if (rie == null) return;

                GlobalResources.Add(new Resource
                {
                    Tag = ResourceTag(rie),
                    ResourceName = ResourceName(rie),
                    ResourceIndexEntry = rie
                });
            }
        }

        private void CopyResource()
        {
            //if (SelectedResource == null) return;

            //try
            //{
            //    DataFormat d = new DataFormat();
            //    d.TGIN = SelectedResource.ResourceIndexEntry as AResourceIndexEntry;
            //    d.TGIN.ResName = SelectedResource.ResourceName;
            //    d.Data = WrapperDealer.GetResource(0, CurrentPackage,
            //        SelectedResource.ResourceIndexEntry, true).AsBytes;

            //    IFormatter formatter = new BinaryFormatter();
            //    MemoryStream ms = new MemoryStream();
            //    formatter.Serialize(ms, d);
            //    IDataObject dataObject = new DataObject();

            //    Application.Current.Clipboard.SetDataObjectAsync()
            //}
        }

        private void DublicateResource()
        {
            //if (SelectedResource == null) return;
            //byte[] buffer = SelectedResource.ResourceIndexEntry.

            //IResourceIndexEntry rie = CurrentPackage.AddResource(SelectedResource.ResourceIndexEntry, )
        }

        private void ResourceCompressed()
        {
            ushort target = 0xFFFF;

            if (CompressedCheck()) target = 0;
            SelectedResource.ResourceIndexEntry.Compressed = target;
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

            IResource rres = WrapperDealer.CreateNewResource(0, "*");
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

            try
            {
                if (SelectedResource == null || SelectedResource.ResourceIndexEntry as AResourceIndexEntry == null) return;
                TGIN tgin = SelectedResource.ResourceIndexEntry as AResourceIndexEntry;
                tgin.ResName = SelectedResource.ResourceName;
                string file = Path.Combine(path, tgin);
                if (File.Exists(file))
                {
                    if (!overwriteAll)
                    {
                        overwriteAll = true;
                    }
                }
                ExportFile(SelectedResource.ResourceIndexEntry, file);
            }
            finally { }
        }

        private void ExportFile(IResourceIndexEntry rie, string fileName)
        {
            IResource res = WrapperDealer.GetResource(0, CurrentPackage, rie, true);
            Stream s = res.Stream;
            s.Position = 0;

            BinaryWriter w = new BinaryWriter(new FileStream(fileName, FileMode.Create));
            w.Write((new BinaryReader(s)).ReadBytes((int)s.Length));
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
                return SelectedResource.ResourceIndexEntry.Compressed != 0 ? true : false;
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

            rie.Compressed = (ushort)(compress ? 0xFFFF : 0);

            return rie;
        }

        private void SearchResources()
        {
            List<Resource> resName = new(), resTag = new(), resType = new(), resGroup = new(), resInstance = new(), resCompressed = new();

            if (!CheckName && !CheckTag && !CheckResourceType && !CheckResourceGroup && !CheckInstance && !CheckCompressed)
            {
                Resources = GlobalResources;
                return;
            }

            List<Resource> complete = new();

            if (CheckName)
                resName = GlobalResources.ToList().FindAll(x => x.ResourceName.ToLower().Contains(Name.ToLower()));
            if (CheckTag)
                resTag = GlobalResources.ToList().FindAll(x => x.Tag == Tag);
            if (CheckResourceType)
                resType = GlobalResources.ToList().FindAll(x => x.ResourceIndexEntry.ResourceType == uint.Parse(ResourceType));
            if (CheckResourceGroup)
                resGroup = GlobalResources.ToList().FindAll(x => x.ResourceIndexEntry.ResourceGroup == uint.Parse(ResourceGroup));
            if (CheckInstance)
                resInstance = GlobalResources.ToList().FindAll(x => x.ResourceIndexEntry.Instance == ulong.Parse(Instance));
            if (CheckCompressed)
                resCompressed = GlobalResources.ToList().FindAll(x => x.ResourceIndexEntry.Compressed == ushort.Parse(Compressed));


            complete = complete.Concat(resName).ToList();
            complete = complete.Concat(resTag).ToList();
            complete = complete.Concat(resType).ToList();
            complete = complete.Concat(resGroup).ToList();
            complete = complete.Concat(resInstance).ToList();
            complete = complete.Concat(resCompressed).ToList();

            Resources = new ObservableCollection<Resource>(complete);
        }
    }
}
