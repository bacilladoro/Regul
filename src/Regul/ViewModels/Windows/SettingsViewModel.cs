using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using OlibUI.Structures;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Newtonsoft.Json;
using OlibUI;
using OlibUI.Instruments;
using OlibUI.Windows;
using Regul.S3PI;

namespace Regul.ViewModels.Windows
{
    internal class SettingsViewModel : ViewModelBase
    {
        private int _theme;

        private bool _hardwareAcceleration;

        private ObservableCollection<Theme> _customThemes;

        private Theme _customTheme;

        private string _pathToTheSims3Document;
        private string _pathToSaves;

        #region Properties

        private int Theme
        {
            get => _theme;
            set
            {
                RaiseAndSetIfChanged(ref _theme, value);

                if (value == -1)
                    return;

                CustomTheme = null;

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

                Application.Current.Styles[1] = new StyleInclude(new Uri("resm:Style?assembly=Regul"))
                {
                    Source = new Uri($"avares://OlibUI/Themes/{Program.Settings.Theme}.axaml")
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

        private string PathToTheSims3Document
        {
            get => _pathToTheSims3Document;
            set
            {
                RaiseAndSetIfChanged(ref _pathToTheSims3Document, value);
                Program.Settings.PathToTheSims3Document = value;
            }
        }
        private string PathToSaves
        {
            get => _pathToSaves;
            set
            {
                RaiseAndSetIfChanged(ref _pathToSaves, value);
                Program.Settings.PathToSaves = value;
            }
        }

        [DataMember]
        private ObservableCollection<Theme> CustomThemes
        {
            get => _customThemes;
            set => RaiseAndSetIfChanged(ref _customThemes, value);
        }

        private Theme CustomTheme
        {
            get => _customTheme;
            set
            {
                RaiseAndSetIfChanged(ref _customTheme, value);

                if (value == null)
                    return;

                Theme = -1;

                Program.Settings.Theme = value.Name;

                Application.Current.Styles[1] = AvaloniaRuntimeXamlLoader.Parse<IStyle>(CustomTheme.ToAxaml());
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
            PathToTheSims3Document = Program.Settings.PathToTheSims3Document;
            PathToSaves = Program.Settings.PathToSaves;

            CustomThemes = new ObservableCollection<Theme>();

            LoadThemes();

            switch (Program.Settings.Theme)
            {
                case "Dazzling":
                    Theme = 0;
                    break;
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
                    Theme? theme = CustomThemes.FirstOrDefault(t => t.Name == Program.Settings.Theme);
                    if (theme != null)
                        CustomTheme = theme;
                    else Theme = 0;
                    break;
            }
        }

        private void Define()
        {
            PathToTheSims3Document = TS3CC.Sims3MyDocFolder;
            PathToSaves = PathToTheSims3Document + "\\Saves";
        }

        private async void ChoosePath()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            dialog.Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string path = await dialog.ShowAsync(App.Settings);
            if (!string.IsNullOrEmpty(path))
            {
                PathToTheSims3Document = path;
                PathToSaves = PathToTheSims3Document + "\\Saves";
            }
        }

        public void CloseWindow() => App.Settings.Close();

        public void Closing()
        {
            if (!Directory.Exists("Themes"))
                Directory.CreateDirectory("Themes");

            foreach (string path in Directory.EnumerateFiles("Themes")) File.Delete(path);

            for (var index = 0; index < CustomThemes.Count; index++)
            {
                Theme customTheme = CustomThemes[index];
                switch (customTheme.Name)
                {
                    case "Dazzling":
                    case "Gloomy":
                    case "Mysterious":
                    case "Turquoise":
                    case "Emerald":
                        customTheme.Name += " New";
                        break;
                }
                if (!string.IsNullOrEmpty(customTheme.Name))
                    File.WriteAllText($"Themes/{customTheme.Name}.json", JsonConvert.SerializeObject(customTheme));
            }
        }

        private void LoadThemes()
        {
            if (Directory.Exists("Themes"))
            {
                foreach (string path in Directory.EnumerateFiles("Themes"))
                {
                    string json = File.ReadAllText(path);
                    CustomThemes.Add(JsonConvert.DeserializeObject<Theme>(json));
                }
            }
        }

        private void Create()
        {
            Theme theme = new Theme
            {
                Name = (string) Application.Current.FindResource("NoName"),

                AccentColor = (Color) Application.Current.FindResource("AccentColor"),
                BackgroundColor = (Color) Application.Current.FindResource("BackgroundColor"),
                BorderColor = (Color) Application.Current.FindResource("BorderColor"),
                ErrorColor = (Color) Application.Current.FindResource("ErrorColor"),
                ForegroundColor = (Color) Application.Current.FindResource("ForegroundColor"),
                BorderBackgroundColor = (Color) Application.Current.FindResource("BorderBackgroundColor"),
                ForegroundOpacityColor = (Color) Application.Current.FindResource("ForegroundOpacityColor"),
                HoverBackgroundColor = (Color) Application.Current.FindResource("HoverBackgroundColor"),
                PressedForegroundColor = (Color) Application.Current.FindResource("PressedForegroundColor"),
                ScrollBoxColor = (Color) Application.Current.FindResource("ScrollBoxColor"),
                WindowBorderColor = (Color) Application.Current.FindResource("WindowBorderColor"),
                HoverScrollBoxColor = (Color) Application.Current.FindResource("HoverScrollBoxColor"),
                WindowBorderBackgroundColor = (Color) Application.Current.FindResource("WindowBorderBackgroundColor"),
            };

            theme.Name = $"{CheckNames(theme)}";

            CustomThemes.Add(theme);
        }

        private string CheckNames(Theme newTheme)
        {
            int index = 0;

            while (true)
            {
                if (CustomThemes.Any(t => t.Name == newTheme.Name))
                {
                    index++;
                    newTheme.Name = $"{(string) Application.Current.FindResource("NoName")} {index}";
                }
                else
                    break;
            }

            return newTheme.Name;
        }

        private async void ChangeColor(Button buttom)
        {
            try
            {
                buttom.Background = ColorHelpers
                    .FromHexColor(await WindowColorPicker.SelectColor(App.Settings,
                        ((SolidColorBrush) buttom.Background).Color.ToString())).ToBursh();

                Application.Current.Styles[1] = AvaloniaRuntimeXamlLoader.Parse<IStyle>(CustomTheme.ToAxaml());
            }
            catch { }
        }

        private void Delete()
        {
            CustomThemes.Remove(CustomTheme);

            if (CustomThemes.Count == 0)
            {
                Theme = 0;
            }
        }

        private void CopyTheme()
        {
            Application.Current.Clipboard.SetTextAsync(JsonConvert.SerializeObject(CustomTheme));
        }

        private async void PasteTheme()
        {
            try
            {
                Theme theme = JsonConvert.DeserializeObject<Theme>(await Application.Current.Clipboard.GetTextAsync());
                
                CustomThemes[CustomThemes.IndexOf(CustomTheme)] = theme;
                CustomTheme = theme;
            }
            catch { }
        }

        private void CopyColor(Button button)
        {
            Application.Current.Clipboard.SetTextAsync(((SolidColorBrush)button.Background).Color.ToString());
        }

        private async void PasteColor(Button button)
        {
            string copingColor = await Application.Current.Clipboard.GetTextAsync();

            if (ColorHelpers.IsValidHexColor(copingColor))
            {
                button.Background = new SolidColorBrush(ColorHelpers.FromHexColor(copingColor));
                Application.Current.Styles[1] = AvaloniaRuntimeXamlLoader.Parse<IStyle>(CustomTheme.ToAxaml());
            }
        }
    }
}