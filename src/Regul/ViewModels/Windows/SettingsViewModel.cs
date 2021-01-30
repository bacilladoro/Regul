using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using ReactiveUI;

namespace Regul.ViewModels.Windows
{
    public class SettingsViewModel : ReactiveObject
    {
        private int _theme;

        private bool _hardwareAcceleration;

        #region Properties

        private int Theme
        {
            get => _theme;
            set
            {
                this.RaiseAndSetIfChanged(ref _theme, value);

                Program.Settings.Theme = value switch
                {
                    1 => "Gloomy",
                    2 => "Mysterious",
                    3 => "Turquoise",
                    4 => "Emerald",
                    _ => "Dazzling"
                };

                Application.Current.Styles[2] = new StyleInclude(new Uri("resm:Style?assembly=Regul"))
                {
                    Source = new Uri($"avares://Regul.OlibUI/Themes/{Program.Settings.Theme}.axaml")
                };
            }
        }
        private bool HardwareAcceleration
        {
            get => _hardwareAcceleration;
            set
            {
                this.RaiseAndSetIfChanged(ref _hardwareAcceleration, value);
                Program.Settings.HardwareAcceleration = value;
            }
        }

        #endregion

        public SettingsViewModel()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            HardwareAcceleration = Program.Settings.HardwareAcceleration;

            Theme = Program.Settings.Theme switch
            {
                "Gloomy" => 1,
                "Mysterious" => 2,
                "Turquoise" => 3,
                "Emerald" => 4,
                _ => 0
            };
        }

        private void CloseWindow()
        {
            App.Settings.Close();
        }
    }
}
