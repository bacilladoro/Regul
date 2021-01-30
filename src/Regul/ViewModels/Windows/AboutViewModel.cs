using System.Diagnostics;
using ReactiveUI;

namespace Regul.ViewModels.Windows
{
    public class AboutViewModel : ReactiveObject
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
