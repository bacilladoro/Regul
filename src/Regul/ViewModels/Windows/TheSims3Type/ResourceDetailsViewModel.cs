using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using Regul.S3PI.Extensions;
using Regul.S3PI.Interfaces;
using System.Security.Cryptography;
using Avalonia;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using ResourceType = Regul.Core.TheSims3Type.ResourceType;

namespace Regul.ViewModels.Windows.TheSims3Type
{
    public class ResourceDetailsViewModel : ReactiveObject, IResourceKey
    {
        private ObservableCollection<ResourceType> _resourceTypes = new();
        private ResourceType _selectedResourceType;

        private string _resourceName;
        private ulong _instance;
        private uint _group;
        private string _filename;

        private bool _internalCHG;
        private bool _importedFile;

        private bool _compress;
        private bool _useResourceName;

        public string ResourceName
        {
            get => _resourceName;
            set
            {
                this.RaiseAndSetIfChanged(ref _resourceName, value);
                UpdateTGIN();
            }
        }
        public ulong Instance
        {
            get => _instance;
            set
            {
                this.RaiseAndSetIfChanged(ref _instance, value);
                UpdateTGIN();
            }
        }
        public uint Group
        {
            get => _group;
            set
            {
                this.RaiseAndSetIfChanged(ref _group, value);
                UpdateTGIN();
            }
        }
        public string Filename
        {
            get => _filename;
            set
            {
                this.RaiseAndSetIfChanged(ref _filename, value);
                ImportedFile = !string.IsNullOrEmpty(value);
                FillPanel();
            }
        }

        public bool Compress
        {
            get => _compress;
            set => this.RaiseAndSetIfChanged(ref _compress, value);
        }
        public bool UseResourceName
        {
            get => _useResourceName;
            set => this.RaiseAndSetIfChanged(ref _useResourceName, value);
        }

        public bool Replace { get; set; }

        private bool ImportedFile
        {
            get => _importedFile;
            set => this.RaiseAndSetIfChanged(ref _importedFile, value);
        }

        public ResourceType SelectedResourceType
        {
            get => _selectedResourceType;
            set => this.RaiseAndSetIfChanged(ref _selectedResourceType, value);
        }
        public ObservableCollection<ResourceType> ResourceTypes
        {
            get => _resourceTypes;
            set => this.RaiseAndSetIfChanged(ref _resourceTypes, value);
        }
        public uint ResourceType
        {
            get => SelectedResourceType.Type;
            set => throw new System.NotImplementedException();
        }
        public uint ResourceGroup
        {
            get => Group;
            set => throw new System.NotImplementedException();
        }

        public static implicit operator TGIN(ResourceDetailsViewModel viewModel) { return viewModel.details; }

        public ResourceDetailsViewModel() : this(true) { }
        public ResourceDetailsViewModel(bool useName)
        {
            UseResourceName = useName;
            Compress = true;

            ResourceType type = new ResourceType { Type = 0x00000000, Tag = "UNKN" };

            ResourceTypes.Add(type);
            SelectedResourceType = type;

            foreach (KeyValuePair<string, List<string>> item in ExtList.Ext)
            {
                try
                {
                    ResourceTypes.Add(new ResourceType
                    {
                        Type = System.Convert.ToUInt32(item.Key, item.Key.StartsWith("0x") ? 16 : 10),
                        Tag = item.Value[0]
                    });

                }
                catch { }
            }
        }
        public ResourceDetailsViewModel(bool useName, IResourceKey rk) : this(useName)
        {
            _internalCHG = true;
            try
            {
                SelectedResourceType = new ResourceType { Type = rk.ResourceType, Tag = ExtList.Ext[rk.ResourceType.ToString()][0] };
                Group = rk.ResourceGroup;
                Instance = rk.Instance;
            }
            finally { _internalCHG = false; UpdateTGIN(); }
        }

        TGIN details;
        private void FillPanel()
        {
            _internalCHG = true;
            try
            {
                details = Filename;
                SelectedResourceType = new ResourceType { Type = details.ResType, Tag = ExtList.Ext[details.ResType.ToString()][0] };
                Group = details.ResGroup;
                Instance = details.ResInstance;
                ResourceName = details.ResName;
            }
            finally { _internalCHG = false; }
        }
        private void UpdateTGIN()
        {
            if (_internalCHG) return;
            details = new TGIN();
            details.ResType = SelectedResourceType.Type;
            details.ResGroup = Group;
            details.ResInstance = Instance;
            details.ResName = ResourceName;
        }

        private void FNV64Convert()
        {
            Instance = FNV64.GetHash(ResourceName);
        }
        private void CLIPIIDConvert()
        {
            Instance = FNV64CLIP.GetHash(ResourceName);
        }
        private void FNV32Convert()
        {
            Instance = FNV32.GetHash(ResourceName);
        }

        private void CopyResourceKeyDetails()
        {
            Application.Current.Clipboard.SetTextAsync((AResourceKey)details + "");
        }

        private async void PasteResourceKeyDetails()
        {
            TGIBlock item = new TGIBlock(null);
            if (!TGIBlock.TryParse(await Application.Current.Clipboard.GetTextAsync(), item)) return;

            Group = item.ResourceGroup;
            Instance = item.Instance;
            SelectedResourceType = new ResourceType { Type = item.ResourceType, Tag = ExtList.Ext[item.ResourceType.ToString()][0] };
        }

        private async void ImportFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter { Extensions = { "*" }, Name = "All files" });
            List<string> files = (await dialog.ShowAsync(App.MainWindow)).ToList();

            if (files.Count == 0) return;

            Filename = files[0];
        }

        private void OK() => App.ResourceDetails.Close(true);
        private void Cancel() => App.ResourceDetails.Close();

        public bool Equals(IResourceKey x, IResourceKey y) => x.Equals(y);

        public int GetHashCode(IResourceKey obj) => obj.GetHashCode();

        public override int GetHashCode() => ResourceType.GetHashCode() ^ ResourceGroup.GetHashCode() ^ Instance.GetHashCode();

        public bool Equals(IResourceKey other) => CompareTo(other) == 0;

        public int CompareTo(IResourceKey other)
        {
            int res = ResourceType.CompareTo(other.ResourceType); if (res != 0) return res;
            res = ResourceGroup.CompareTo(other.ResourceGroup); if (res != 0) return res;
            return Instance.CompareTo(other.Instance);
        }
    }
}
