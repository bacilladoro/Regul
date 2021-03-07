using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using OlibUI.Structures;
using OlibUI.Windows;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace OlibUI.Instruments
{
    public class StyleSettings : TemplatedControl, IStyleable
    {
        public static readonly StyledProperty<Theme> ThemeProperty =
            AvaloniaProperty.Register<StyleSettings, Theme>(nameof(Theme));
        public static readonly StyledProperty<int> NumberStyleProperty =
            AvaloniaProperty.Register<StyleSettings, int>(nameof(NumberStyle));

        public Theme Theme
        {
            get => GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }
        
        public int NumberStyle
        {
            get => GetValue(NumberStyleProperty);
            set => SetValue(NumberStyleProperty, value);
        }

        static StyleSettings()
        {
        }

        Type IStyleable.StyleKey => typeof(StyleSettings);

        T GetControl<T>(TemplateAppliedEventArgs e, string name) where T : class => e.NameScope.Get<T>(name);

        #region Buttons

        Button AccentBrush;
        Button BackgroundBrush;
        Button HoverBackgroundBrush;
        Button ForegroundBrush;
        Button ForegroundOpacityBrush;
        Button PressedForegroundBrush;
        Button BorderBackgroundBrush;
        Button BorderBrush;
        Button WindowBorderBrush;
        Button HoverScrollBoxBrush;
        Button ScrollBoxBrush;
        Button ErrorBrush;

        #endregion

        StackPanel stackPanel;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            StyleSettings styleSettings = this;

            stackPanel = GetControl<StackPanel>(e, "Buttons");

            AccentBrush = GetControl<Button>(e, "AccentBrush");
            BackgroundBrush = GetControl<Button>(e, "BackgroundBrush");
            HoverBackgroundBrush = GetControl<Button>(e, "HoverBackgroundBrush");
            ForegroundBrush = GetControl<Button>(e, "ForegroundBrush");
            ForegroundOpacityBrush = GetControl<Button>(e, "ForegroundOpacityBrush");
            PressedForegroundBrush = GetControl<Button>(e, "PressedForegroundBrush");
            BorderBackgroundBrush = GetControl<Button>(e, "BorderBackgroundBrush");
            BorderBrush = GetControl<Button>(e, "BorderBrush");
            WindowBorderBrush = GetControl<Button>(e, "WindowBorderBrush");
            HoverScrollBoxBrush = GetControl<Button>(e, "HoverScrollBoxBrush");
            ScrollBoxBrush = GetControl<Button>(e, "ScrollBoxBrush");
            ErrorBrush = GetControl<Button>(e, "ErrorBrush");

            //new WindowColorPicker().Show();

            //Theme theme = AxamlToTheme("avares://OlibUI/Themes/Dazzling.axaml");
        }

        public void SelectColor(object sender, EventArgs e)
        {
        }
    }
}
