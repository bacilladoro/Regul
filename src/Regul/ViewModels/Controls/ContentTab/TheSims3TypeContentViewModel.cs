using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Regul.Core.TheSims3Type;
using Regul.S3PI.Interfaces;
using Regul.S3PI.Package;

namespace Regul.ViewModels.Controls.ContentTab
{
    public class TheSims3TypeContentViewModel : ReactiveObject
    {
        private uint _resourceTypeView;
        private ObservableCollection<Resource> _resources = new();
        private bool _openedMenu;
        private Bitmap _imageResource;
        private Resource _selectedResource;

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

        public Resource SelectedResource
        {
            get => _selectedResource;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedResource, value);

                if (value == null) return;

                switch(value.Tag)
                {
                    case "SNAP":
                    case "_IMG":
                        using (MemoryStream stream = new MemoryStream(value.Data))
                        {
                            ImageResource = new Bitmap(stream);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public ObservableCollection<Resource> Resources
        {
            get => _resources;
            set => this.RaiseAndSetIfChanged(ref _resources, value);
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
                    Instance = item.Instance,
                    ResourceGroup = item.ResourceGroup,
                    ResourceType = item.ResourceType,
                    Memsize = item.Memsize,
                    Filesize = item.Filesize,
                    Chunkoffset = item.Chunkoffset,
                    Compressed = item.Compressed,
                    Unknown2 = item.Unknown2,
                    Data = ReadFully(S3PI.WrapperDealer.GetResource(0, CurrentPackage, item).Stream),
                    Tag = item.ResourceType switch
                    {
                        2415624438 => "_ADS",
                        16807867 => "_AUD",
                        16807876 => "_AUD",
                        16807882 => "_AUD",
                        25276626 => "_AUD",
                        27600859 => "_AUD",
                        32437818 => "_AUD",
                        2192848260 => "_CRC",
                        39620774 => "_CSS",
                        11720834 => "_IMG",
                        529034925 => "_INI",
                        3034411371 => "_INV",
                        23462796 => "_KEY",
                        439484877 => "_MOD",
                        2393838558 => "_RIG",
                        11883242 => "_SPT",
                        3931183280 => "_SWB",
                        103718624 => "_TTF",
                        2982943478 => "_VID",
                        53690476 => "_XML",
                        1944665835 => "_XML",
                        3843051622 => "_XML",
                        3843051624 => "_XML",
                        100625316 => "2ARY",
                        1671642791 => "ANIM",
                        2154832445 => "AUDT",
                        103580164 => "BBLN",
                        108833297 => "BGEO",
                        55959718 => "BOND",
                        11431015 => "BONE",
                        3711050633 => "BUFF",
                        55242443 => "CASP",
                        85848797 => "CBLN",
                        171372666 => "CCFP",
                        64504770 => "CCHE",
                        68746794 => "CFEN",
                        83086337 => "CFIR",
                        829192434 => "CFND",
                        103817841 => "CINF",
                        201803423 => "CLDR",
                        1797309683 => "CLIP",
                        81276304 => "CMRU",
                        38407762 => "CNFG",
                        201803117 => "COLL",
                        72016144 => "COMP",
                        106821691 => "CPCA",
                        78405011 => "CPRX",
                        80052483 => "CRAL",
                        4058889606 => "CRMT",
                        2448264510 => "CRST",
                        77374669 => "CSTR",
                        82660274 => "CTPT",
                        297949376 => "CTRG",
                        78841449 => "CTTL",
                        1365025997 => "CWAL",
                        101398796 => "CWAT",
                        2438063804 => "CWST",
                        64515748 => "DETL",
                        100969434 => "DMTR",
                        56144010 => "FACE",
                        103306152 => "FAMD",
                        3039776853 => "FBLN",
                        3548561239 => "FTPT",
                        22681673 => "GEOM",
                        103965146 => "HINF",
                        3714200534 => "HTML",
                        779470692 => "ICON",
                        779470693 => "ICON",
                        779470694 => "ICON",
                        779470695 => "ICON",
                        3629023173 => "ICON",
                        3629023174 => "ICON",
                        3629023175 => "ICON",
                        796721154 => "IMAG",
                        796721156 => "IMAG",
                        796721158 => "IMAG",
                        62078431 => "ITUN",
                        47570707 => "JAZZ",
                        1065771754 => "JPEG",
                        177793776 => "JSON",
                        1735860060 => "JSON",
                        39622070 => "LAYO",
                        3496170587 => "LDES",
                        62178845 => "LITE",
                        204973493 => "LOOK",
                        3579804847 => "LRIG",
                        30467933 => "MATD",
                        3482995406 => "MDLR",
                        114182148 => "META",
                        979742505 => "MINF",
                        46788594 => "MIXR",
                        30478132 => "MLOD",
                        23466547 => "MODL",
                        33659250 => "MTST",
                        4087579504 => "NGMP",
                        103965203 => "OBCI",
                        832458525 => "OBJD",
                        47985727 => "OBJK",
                        1293571465 => "OBJN",
                        112820717 => "OBJS",
                        1318566616 => "OBSC",
                        3735196956 => "PETB",
                        45741666 => "PROP",
                        3571055589 => "PTRN",
                        99422758 => "REFS",
                        3843051623 => "RMLS",
                        3540272417 => "RSLT",
                        121612807 => "S3SA",
                        83169331 => "SBNO",
                        24503757 => "SCPT",
                        2495992138 => "SIGR",
                        83396964 => "SIME",
                        39769844 => "SIMO",
                        2832567269 => "SKIL",
                        92316365 => "SNAP",
                        92316366 => "SNAP",
                        92316367 => "SNAP",
                        1802339197 => "SNAP",
                        1802339198 => "SNAP",
                        1802339199 => "SNAP",
                        35487372 => "SPT2",
                        570775514 => "STBL",
                        1919433749 => "STPR",
                        92316340 => "THUM",
                        92316341 => "THUM",
                        92316342 => "THUM",
                        92920900 => "THUM",
                        92920901 => "THUM",
                        92920902 => "THUM",
                        92920903 => "THUM",
                        95516312 => "THUM",
                        95516313 => "THUM",
                        95516314 => "THUM",
                        95532324 => "THUM",
                        95532325 => "THUM",
                        95532326 => "THUM",
                        643032008 => "THUM",
                        643032009 => "THUM",
                        643032010 => "THUM",
                        759334128 => "THUM",
                        759334129 => "THUM",
                        759334130 => "THUM",
                        1575607200 => "THUM",
                        1575607201 => "THUM",
                        1575607202 => "THUM",
                        1651466444 => "THUM",
                        1651466445 => "THUM",
                        1651466446 => "THUM",
                        2906025877 => "THUM",
                        2906025878 => "THUM",
                        4243240539 => "THUM",
                        53633251 => "TkMk",
                        55867754 => "TONE",
                        55925672 => "TONE",
                        4043265432 => "TRIG",
                        107542069 => "TWNI",
                        107542073 => "TWNP",
                        54137909 => "TXTC",
                        54635721 => "TXTF",
                        796721160 => "UTIX",
                        93434287 => "UPST",
                        43922235 => "VOCE",
                        1936229617 => "VPXY",
                        107542056 => "WDDT",
                        77912401 => "WDET",
                        107542064 => "WDNL",
                        3653044489 => "WDNM",
                        899890729 => "WDSH",
                        4127849824 => "WPID",
                        36402540 => "WRDH",
                        2422433293 => "WTXT",

                        _ => "UNKN"
                     }
                };

                Resources.Add(res);
            }
        }

        private void AddResource()
        {
            Resources.Add(new Resource 
            {
               ResourceName = "Res",
               Tag = "IMG",
               Chunkoffset = 8340932,
               Compressed = 0
            });
        }

        private byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
