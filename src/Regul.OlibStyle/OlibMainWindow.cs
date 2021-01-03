using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using Icon = System.Drawing.Icon;

namespace Regul.OlibStyle
{
    public class OlibMainWindow : Window, IStyleable
    {
        public static readonly StyledProperty<Menu> TitleBarMenuProperty;

        static OlibMainWindow()
        {
            TitleBarMenuProperty = AvaloniaProperty.Register<OlibMainWindow, Menu>(nameof(TitleBarMenu));
        }

        public Menu TitleBarMenu
        {
            get => GetValue(TitleBarMenuProperty);
            set => SetValue(TitleBarMenuProperty, value);
        }

        private void SetupSide(string name, StandardCursorType cursor, WindowEdge edge, ref TemplateAppliedEventArgs e)
        {
            Control control = e.NameScope.Get<Control>(name);
            control.Cursor = new Cursor(cursor);
            control.PointerPressed += (object sender, PointerPressedEventArgs ep) =>
            {
                if (ep.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
                    ((Window)this.GetVisualRoot()).PlatformImpl?.BeginResizeDrag(edge, ep);
            };
        }

        Type IStyleable.StyleKey => typeof(OlibMainWindow);

        T GetControl<T>(TemplateAppliedEventArgs e, string name) where T : class => e.NameScope.Get<T>(name);

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            OlibMainWindow window = this;

            try
            {
                Control titleBar = GetControl<Control>(e, "TitleBar");

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    titleBar.DoubleTapped += (s, ep) =>
                    {
                        window.WindowState = ((Window)this.GetVisualRoot()).WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    };
                }

                titleBar.PointerPressed += (s, ep) =>
                {
                    window.PlatformImpl?.BeginMoveDrag(ep);
                };

                try
                {
                    SetupSide("Left_top", StandardCursorType.LeftSide, WindowEdge.West, ref e);
                    SetupSide("Left_mid", StandardCursorType.LeftSide, WindowEdge.West, ref e);
                    SetupSide("Left_bottom", StandardCursorType.LeftSide, WindowEdge.West, ref e);
                    SetupSide("Right_top", StandardCursorType.RightSide, WindowEdge.East, ref e);
                    SetupSide("Right_mid", StandardCursorType.RightSide, WindowEdge.East, ref e);
                    SetupSide("Right_bottom", StandardCursorType.RightSide, WindowEdge.East, ref e);
                    SetupSide("Top", StandardCursorType.TopSide, WindowEdge.North, ref e);
                    SetupSide("Bottom", StandardCursorType.BottomSide, WindowEdge.South, ref e);
                    SetupSide("TopLeft", StandardCursorType.TopLeftCorner, WindowEdge.NorthWest, ref e);
                    SetupSide("TopRight", StandardCursorType.TopRightCorner, WindowEdge.NorthEast, ref e);
                    SetupSide("BottomLeft", StandardCursorType.BottomLeftCorner, WindowEdge.SouthWest, ref e);
                    SetupSide("BottomRight", StandardCursorType.BottomRightCorner, WindowEdge.SouthEast, ref e);
                }
                catch { }

                GetControl<Button>(e, "MinimizeButton").Click += (s, ep) =>
                {
                    window.WindowState = WindowState.Minimized;
                };
                GetControl<Button>(e, "MaximizeButton").Click += (s, ep) =>
                {
                    window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                };
                GetControl<Button>(e, "CloseButton").Click += (s, ep) =>
                {
                    window.Close();
                };
            }
            catch { }
        }
    }

    public class WindowIconToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                WindowIcon wIcon = value as WindowIcon;
                MemoryStream stream = new();
                wIcon.Save(stream);
                stream.Position = 0;
                try
                {
                    return new Bitmap(stream);
                }
                catch
                {
                    try
                    {

                        Icon icon = new(stream);
                        System.Drawing.Bitmap bmp = icon.ToBitmap();
                        bmp.Save(stream, ImageFormat.Png);
                        return new Bitmap(stream);
                    }
                    catch
                    {
                        Icon icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
                        System.Drawing.Bitmap bmp = icon.ToBitmap();
                        Stream stream3 = new MemoryStream();
                        bmp.Save(stream3, ImageFormat.Png);
                        return new Bitmap(stream3);
                    }
                }
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
