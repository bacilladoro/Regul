using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace Regul.Instruments
{
    public class ImageViewer : TemplatedControl, IStyleable
    {
        public static readonly StyledProperty<Avalonia.Media.Imaging.Bitmap> ImageSourceProperty =
            AvaloniaProperty.Register<ImageViewer, Avalonia.Media.Imaging.Bitmap>(nameof(ImageSource));


        public static readonly StyledProperty<int> ChangeOfSizeProperty =
            AvaloniaProperty.Register<ImageViewer, int>(nameof(ChangeOfSize));

        public static readonly StyledProperty<int> CurrentSizeProperty =
            AvaloniaProperty.Register<ImageViewer, int>(nameof(CurrentSize));

        public static readonly StyledProperty<int> MaxSizeProperty =
            AvaloniaProperty.Register<ImageViewer, int>(nameof(MaxSize));

        public static readonly StyledProperty<int> MinSizeProperty =
            AvaloniaProperty.Register<ImageViewer, int>(nameof(MinSize));

        public static readonly StyledProperty<bool> InvertProperty =
            AvaloniaProperty.Register<ImageViewer, bool>(nameof(Invert));


        public static readonly StyledProperty<bool> RedChannelProperty =
            AvaloniaProperty.Register<ImageViewer, bool>(nameof(RedChannel));

        public static readonly StyledProperty<bool> GreenChannelProperty =
            AvaloniaProperty.Register<ImageViewer, bool>(nameof(GreenChannel));

        public static readonly StyledProperty<bool> BlueChannelProperty =
            AvaloniaProperty.Register<ImageViewer, bool>(nameof(BlueChannel));

        public static readonly StyledProperty<bool> AlphaChannelProperty = 
            AvaloniaProperty.Register<ImageViewer, bool>(nameof(AlphaChannel));

        private Avalonia.Controls.Image _imageView;
        private ScrollViewer _viewer;

        static ImageViewer()
        {

        }

        public Avalonia.Media.Imaging.Bitmap ImageSource
        {
            get => GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        /// Image resizing speed with percentage scrolling
        /// </summary>
        public int ChangeOfSize
        {
            get => GetValue(ChangeOfSizeProperty);
            set => SetValue(ChangeOfSizeProperty, value);
        }

        /// <summary>
        /// Current image size
        /// </summary>
        public int CurrentSize
        {
            get => GetValue(CurrentSizeProperty);
            set => SetValue(CurrentSizeProperty, value);
        }

        /// <summary>
        /// Maximum image size limit in percentage
        /// </summary>
        public int MaxSize
        {
            get => GetValue(MaxSizeProperty);
            set => SetValue(MaxSizeProperty, value);
        }

        /// <summary>
        /// Minimum image size limit in percentage
        /// </summary>
        public int MinSize
        {
            get => GetValue(MinSizeProperty);
            set => SetValue(MinSizeProperty, value);
        }

        /// <summary>
        /// Invert the image
        /// </summary>
        public bool Invert
        {
            get => GetValue(InvertProperty);
            set => SetValue(InvertProperty, value);
        }


        /// <summary>
        /// Activates the red channel of the image
        /// </summary>
        public bool RedChannel
        {
            get => GetValue(RedChannelProperty);
            set => SetValue(RedChannelProperty, value);
        }

        /// <summary>
        /// Activates the green channel of the image
        /// </summary>
        public bool GreenChannel
        {
            get => GetValue(GreenChannelProperty);
            set => SetValue(GreenChannelProperty, value);
        }
        
        /// <summary>
        /// Activates the blue channel of the image
        /// </summary>
        public bool BlueChannel
        {
            get => GetValue(BlueChannelProperty);
            set => SetValue(BlueChannelProperty, value);
        }

        /// <summary>
        /// Activates the alpha channel of the image
        /// </summary>
        public bool AlphaChannel
        {
            get => GetValue(AlphaChannelProperty);
            set => SetValue(AlphaChannelProperty, value);
        }

        Type IStyleable.StyleKey => typeof(ImageViewer);

        T GetControl<T>(TemplateAppliedEventArgs e, string name) where T : class => e.NameScope.Get<T>(name);

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            ImageViewer imageViewer = this;

            RedChannel = GreenChannel = BlueChannel = AlphaChannel = true;

            CurrentSize = CurrentSize == 0 ? 100 : CurrentSize;
            ChangeOfSize = ChangeOfSize == 0 ? 25 : ChangeOfSize;
            MaxSize = MaxSize == 0 ? 1000 : MaxSize;
            MinSize = MinSize == 0 ? 100 : MinSize;

            try
            {
                _imageView = GetControl<Avalonia.Controls.Image>(e, "ImageView");
                _viewer = GetControl<ScrollViewer>(e, "Viewer");

                ImageSourceProperty.Changed.Subscribe(ImageChanged);
            }
            catch { }
        }

        private void ImageChanged(AvaloniaPropertyChangedEventArgs e)
        {
            CurrentSize = 100;

            _imageView.Source = ImageSource;

            _imageView.Width = ImageSource.Size.Width;
            _imageView.Height = ImageSource.Size.Height;
        }
    }


    public class NumberToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value + "%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BitmapSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Bitmap bitmap = (Bitmap)value;

            if (bitmap != null)
                return bitmap.PixelSize.Width + "x" + bitmap.PixelSize.Height;
            else
                return "0x0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
