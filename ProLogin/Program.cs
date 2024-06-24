using ProLogin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProLogin
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ProCore.Load();

            ProLoginSettings.Load();

            #region Launching WPF
            App app = new App();
            app.Run();

            // old method
            /*
            Application a = new Application();
            a.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            a.Run();
            */
            #endregion
        }
    }
}
