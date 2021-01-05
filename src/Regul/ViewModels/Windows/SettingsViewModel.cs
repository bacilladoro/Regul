using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using ReactiveUI;

namespace Regul.ViewModels.Windows
{
    class SettingsViewModel : ReactiveObject
    {
        private int _theme;

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
                    Source = new Uri($"avares://Regul.OlibStyle/Themes/{Program.Settings.Theme}.axaml")
                };
            }
        }

        #endregion
        
        public SettingsViewModel()
        {
            Initialize();
        }
        
        private void Initialize()
        {
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

        public void Exit()
        {

        }
    }
}
