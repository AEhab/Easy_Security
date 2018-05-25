using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace email_encrpt
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        LoginWindow loginwindow;
        email mail;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            mail = new email();
            loginwindow = new LoginWindow (mail);
            //loginwindow.Show();
        }
        public void Application_Exit(object sender, ExitEventArgs e)
        {
            loginwindow.Close();
            mail.Close();
        }
    }
}
