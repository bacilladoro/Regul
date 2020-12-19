using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Regul.ViewModels.Windows
{
    class AboutViewModel : ReactiveObject
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
