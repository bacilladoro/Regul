using System.Diagnostics;

namespace Regul.ViewModels.Windows
{
    internal class AboutViewModel : ViewModelBase
    {
        private void CloseWindow()
        {
            App.About.Close();
        }

        private void GitHubSite()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Onebeld/Regul",
                UseShellExecute = true
            });
        }
    }
}
