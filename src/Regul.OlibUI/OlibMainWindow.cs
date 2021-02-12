using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System;

namespace Regul.OlibUI
{
    public class OlibMainWindow : Window, IStyleable
    {
        public static readonly StyledProperty<Menu> TitleBarMenuProperty =
            AvaloniaProperty.Register<OlibMainWindow, Menu>(nameof(TitleBarMenu));
        public static readonly StyledProperty<bool> InLoadModeProperty =
            AvaloniaProperty.Register<OlibMainWindow, bool>(nameof(InLoadMode));

        public Menu TitleBarMenu
        {
            get => GetValue(TitleBarMenuProperty);
            set => SetValue(TitleBarMenuProperty, value);
        }

        public bool InLoadMode
        {
            get => GetValue(InLoadModeProperty);
            set => SetValue(InLoadModeProperty, value);
        }

        private void SetupSide(string name, StandardCursorType cursor, WindowEdge edge, ref TemplateAppliedEventArgs e)
        {
            Control control = e.NameScope.Get<Control>(name);
            control.Cursor = new Cursor(cursor);
            control.PointerPressed += (_, ep) =>
            {
                if (ep.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
                    ((Window)this.GetVisualRoot()).PlatformImpl?.BeginResizeDrag(edge, ep);
            };
        }

        Type IStyleable.StyleKey => typeof(OlibMainWindow);

        T GetControl<T>(TemplateAppliedEventArgs e, string name) where T : class => e.NameScope.Get<T>(name);

        private MenuItem ExpandMenuItem;
        private MenuItem ReestablishMenuItem;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            OlibMainWindow window = this;

            try
            {
                Control titleBar = GetControl<Control>(e, "TitleBar");

                ReestablishMenuItem = GetControl<MenuItem>(e, "ReestablishMenuItem");
                ExpandMenuItem = GetControl<MenuItem>(e, "ExpandMenuItem");

                ReestablishMenuItem.IsEnabled = false;

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    titleBar.DoubleTapped += (_, e1) =>
                    {
                        if (((Window)this.GetVisualRoot()).WindowState == WindowState.Maximized)
                        {
                            window.WindowState = WindowState.Normal;
                            ReestablishMenuItem.IsEnabled = false;
                            ExpandMenuItem.IsEnabled = true;
                        }
                        else
                        {
                            window.WindowState = WindowState.Maximized;
                            ReestablishMenuItem.IsEnabled = true;
                            ExpandMenuItem.IsEnabled = false;
                        }
                    };
                }

                titleBar.PointerPressed += (s, ep) =>
                {
                    GetControl<ContextMenu>(e, "GlobalContextMenu").Close();
                    window.PlatformImpl?.BeginMoveDrag(ep);
                };

                window.PointerReleased += (s, ep) =>
                {
                    GetControl<ContextMenu>(e, "GlobalContextMenu").Close();
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
                    if (window.WindowState == WindowState.Maximized)
                    {

                        window.WindowState = WindowState.Normal;
                        ReestablishMenuItem.IsEnabled = false;
                        ExpandMenuItem.IsEnabled = true;
                    }
                    else
                    {
                        window.WindowState = WindowState.Maximized;
                        ReestablishMenuItem.IsEnabled = true;
                        ExpandMenuItem.IsEnabled = false;
                    }
                };
                GetControl<Button>(e, "CloseButton").Click += (s, ep) =>
                {
                    window.Close();
                };

                ReestablishMenuItem.Click += (s, ep) =>
                {
                    window.WindowState = WindowState.Normal;
                    ExpandMenuItem.IsEnabled = true;
                    ReestablishMenuItem.IsEnabled = false;
                };
                ExpandMenuItem.Click += (s, ep) =>
                {
                    window.WindowState = WindowState.Maximized;
                    ExpandMenuItem.IsEnabled = false;
                    ReestablishMenuItem.IsEnabled = true;
                };

                GetControl<MenuItem>(e, "CloseMenuItem").Click += (s, ep) =>
                {
                    window.Close();
                };
                GetControl<MenuItem>(e, "CollapseMenuItem").Click += (s, ep) =>
                {
                    window.WindowState = WindowState.Minimized;
                };
            }
            catch { }
        }
    }
}
