using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System;

namespace Regul.OlibStyle
{
    [Flags]
    public enum WindowButtons
    {
        OnlyClose = 0,
        CloseAndCollapse = 1,
        CloseAndExpand = 2,
        All = 3
    }

    public class OlibModalWindow : Window, IStyleable
    {
        public static readonly StyledProperty<WindowButtons> WindowButtonsProperty;
        public static readonly StyledProperty<Grid> BottomPanelProperty;

        static OlibModalWindow()
        {
            WindowButtonsProperty = AvaloniaProperty.Register<OlibModalWindow, WindowButtons>(nameof(WindowButtons));
            BottomPanelProperty = AvaloniaProperty.Register<OlibModalWindow, Grid>(nameof(BottomPanel));
        }

        public WindowButtons WindowButtons
        {
            get => GetValue(WindowButtonsProperty);
            set => SetValue(WindowButtonsProperty, value);
        }

        public Grid BottomPanel
        {
            get => GetValue(BottomPanelProperty);
            set => SetValue(BottomPanelProperty, value);
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
                        if (WindowButtons == WindowButtons.CloseAndExpand || WindowButtons == WindowButtons.All)
                            window.WindowState = ((Window)this.GetVisualRoot()).WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    };
                }

                titleBar.PointerPressed += (s, ep) =>
                {
                    window.PlatformImpl?.BeginMoveDrag(ep);
                };

                if (BottomPanel == null)
                {
                    GetControl<Border>(e, "BottomPanel").IsVisible = false;
                    GetControl<ContentPresenter>(e, "Content").CornerRadius = CornerRadius.Parse("0 0 4.5 4.5");
                }

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

                Button maximizeButton = GetControl<Button>(e, "MaximizeButton");
                maximizeButton.Click += (s, ep) =>
                {
                    window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                };

                if (WindowButtons != WindowButtons.All)
                {
                    if (WindowButtons != WindowButtons.CloseAndCollapse) minimizeButton.IsVisible = false;
                    if (WindowButtons != WindowButtons.CloseAndExpand) maximizeButton.IsVisible = false;
                }

                GetControl<Button>(e, "CloseButton").Click += (s, ep) =>
                {
                    window.Close();
                };
            }
            catch { }
        }
    }
}
