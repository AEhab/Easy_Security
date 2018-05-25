using ImapX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace email_encrpt
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
       
        email mail;
        private static ImapClient client { get; set; }
        public LoginWindow(email m)
        {
            InitializeComponent();
            if (!File.Exists("C:/EasySecurity/first.bmv"))
            {
                MessageBox.Show("This is the first run of EasySecurity, please set a 12 charactar password for your key pair", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                EmailHelper.First_Run(this);
            }
            else
                this.Show();
            this.mail = m;
            MyInfo.isconnected = false;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            client = EmailHelper.Login(email.Text, password.Password);
            if (MyInfo.isconnected)
            {
                mail.init(client);
                //login successful
                mail.Show();
                this.Hide();
            }               
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(MyInfo.isconnected)
            client.Logout();
            App.Current.Shutdown();
        }
    }
}
