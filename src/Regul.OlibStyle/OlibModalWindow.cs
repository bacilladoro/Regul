using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System;

namespace Regul.OlibStyle
{
    public class OlibModalWindow : Window, IStyleable
    {
        public enum WindowControlButtons
        {
            OnlyClose,
            CloseAndCollapse,
            CloseAndExpand,
            All
        }

        public static readonly StyledProperty<WindowControlButtons> WindowButtonsProperty;

        static OlibModalWindow()
        {
            WindowButtonsProperty = AvaloniaProperty.Register<OlibModalWindow, WindowControlButtons>(nameof(WindowButtons));
        }

        public WindowControlButtons WindowButtons
        {
            get => GetValue(WindowButtonsProperty);
            set => SetValue(WindowButtonsProperty, value);
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

        Type IStyleable.StyleKey => typeof(OlibModalWindow);

        T GetControl<T>(TemplateAppliedEventArgs e, string name) where T : class => e.NameScope.Get<T>(name);

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            OlibModalWindow window = this;

            try
            {
                Control titleBar = GetControl<Control>(e, "TitleBar");

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    titleBar.DoubleTapped += (s, ep) =>
                    {
                        if (WindowButtons == WindowControlButtons.CloseAndExpand || WindowButtons == WindowControlButtons.All)
                            window.WindowState = ((Window)this.GetVisualRoot()).WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    };
                }

                titleBar.PointerPressed += (s, ep) =>
                {
                    window.PlatformImpl?.BeginMoveDrag(ep);
                };

                try
                {
                    if (window.CanResize)
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
                    else GetControl<Grid>(e, "ResizeGrid").IsVisible = false;
                }
                catch { }

                Button minimizeButton = GetControl<Button>(e, "MinimizeButton");
                minimizeButton.Click += (s, ep) =>
                {
                    window.WindowState = WindowState.Minimized;
                };
                if (WindowButtons != WindowControlButtons.CloseAndCollapse || WindowButtons != WindowControlButtons.All) minimizeButton.IsVisible = false;

                Button maximizeButton = GetControl<Button>(e, "MaximizeButton");
                maximizeButton.Click += (s, ep) =>
                {
                    window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                };
                if (WindowButtons != WindowControlButtons.CloseAndCollapse || WindowButtons != WindowControlButtons.All) maximizeButton.IsVisible = false;

                GetControl<Button>(e, "CloseButton").Click += (s, ep) =>
                {
                    window.Close();
                };
            }
            catch { }
        }
    }
}
