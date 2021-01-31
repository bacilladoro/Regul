using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System;

namespace Regul.OlibUI
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
        public static readonly StyledProperty<bool> InLoadModeProperty;

        static OlibModalWindow()
        {
            WindowButtonsProperty = AvaloniaProperty.Register<OlibModalWindow, WindowButtons>(nameof(WindowButtons));
            BottomPanelProperty = AvaloniaProperty.Register<OlibModalWindow, Grid>(nameof(BottomPanel));
            InLoadModeProperty = AvaloniaProperty.Register<OlibModalWindow, bool>(nameof(InLoadMode));
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

        public bool InLoadMode
        {
            get => GetValue(InLoadModeProperty);
            set => SetValue(InLoadModeProperty, value);
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

        private MenuItem ExpandMenuItem;
        private MenuItem ReestablishMenuItem;
        private MenuItem CollapseMenuItem;
        private Separator ContextMenuSeparator;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            OlibModalWindow window = this;

            ReestablishMenuItem = GetControl<MenuItem>(e, "ReestablishMenuItem");
            ExpandMenuItem = GetControl<MenuItem>(e, "ExpandMenuItem");
            CollapseMenuItem = GetControl<MenuItem>(e, "CollapseMenuItem");
            ContextMenuSeparator = GetControl<Separator>(e, "ContextMenuSeparator");

            ReestablishMenuItem.IsEnabled = false;

            try
            {
                Control titleBar = GetControl<Control>(e, "TitleBar");

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    titleBar.DoubleTapped += (s, ep) =>
                    {
                        if (WindowButtons == WindowButtons.CloseAndExpand || WindowButtons == WindowButtons.All)
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
                        }
                    };
                }

                window.KeyDown += (s, ep) =>
                {
                    if (ep.KeyModifiers == KeyModifiers.Control && ep.Key == Key.Q)
                    {
                        window.Close();
                    }
                };

                titleBar.PointerPressed += (s, ep) =>
                {
                    GetControl<ContextMenu>(e, "GlobalContextMenu").Close();
                    window.PlatformImpl?.BeginMoveDrag(ep);
                };

                window.PointerReleased += (s, ep) =>
                {
                    GetControl<ContextMenu>(e, "GlobalContextMenu").Close();
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
                CollapseMenuItem.Click += (s, ep) =>
                {
                    window.WindowState = WindowState.Minimized;
                };

                GetControl<MenuItem>(e, "CloseMenuItem").Click += (s, ep) =>
                {
                    window.Close();
                };

                if (WindowButtons == WindowButtons.CloseAndCollapse)
                {
                    ExpandMenuItem.IsVisible = false;
                    ReestablishMenuItem.IsVisible = false;
                }
                else if (WindowButtons == WindowButtons.CloseAndExpand)
                {
                    CollapseMenuItem.IsVisible = false;
                }
                else if (WindowButtons == WindowButtons.OnlyClose)
                {
                    ExpandMenuItem.IsVisible = false;
                    ReestablishMenuItem.IsVisible = false;
                    CollapseMenuItem.IsVisible = false;
                    ContextMenuSeparator.IsVisible = false;
                }
            }
            catch { }
        }
    }
}
