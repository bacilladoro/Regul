using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using System;

namespace Regul.ViewModels.Windows
{
    internal class SettingsViewModel : ViewModelBase
    {
        private int _theme;

        private bool _hardwareAcceleration;

        #region Properties

        private int Theme
        {
            get => _theme;
            set
            {
                RaiseAndSetIfChanged(ref _theme, value);

                switch (value)
                {
                    case 1:
                        Program.Settings.Theme = "Gloomy";
                        break;
                    case 2:
                        Program.Settings.Theme = "Mysterious";
                        break;
                    case 3:
                        Program.Settings.Theme = "Turquoise";
                        break;
                    case 4:
                        Program.Settings.Theme = "Emerald";
                        break;
                    default:
                        Program.Settings.Theme = "Dazzling";
                        break;
                }

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
                RaiseAndSetIfChanged(ref _hardwareAcceleration, value);
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

            switch (Program.Settings.Theme)
            {
                case "Gloomy":
                    Theme = 1;
                    break;
                case "Mysterious":
                    Theme = 2;
                    break;
                case "Turquoise":
                    Theme = 3;
                    break;
                case "Emerald":
                    Theme = 4;
                    break;
                default:
                    Theme = 0;
                    break;
            }
        }

        private void CloseWindow()
        {
            App.Settings.Close();
        }
    }
}
