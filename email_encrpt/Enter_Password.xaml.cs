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
using System.Windows.Shapes;
using System.IO;
using Crypto;
namespace email_encrpt
{
    /// <summary>
    /// Interaction logic for Enter_Password.xaml
    /// </summary>
    public partial class Enter_Password : Window
    {
        Window win;
        bool check;
        public Enter_Password(Window m)
        {
            check = false;
            this.win = m;
            if(this.win.IsVisible)
            this.win.Hide();
            InitializeComponent();
        }
        private void done_Click(object sender, RoutedEventArgs e)
        {
            if (Pass.Password.Length < 12)
                MessageBox.Show("The password should be more than 12 character");
            else if(this.win.GetType() == (typeof (email_encrpt.LoginWindow)))
            {
                check = true;
                if (win.IsVisible == false)
                this.win.Show();
                System.IO.Directory.CreateDirectory(@"C:\EasySecurity");
                Crypto.KeyVault.SaveRSAParamaters(Pass.Password);
                File.Create("C:/EasySecurity/first.bmv");
                this.Close();
                win.Show();
            }
            else if(this.win.GetType() == (typeof(email_encrpt.Message_Window)))
            {
                try
                {
                   string message =  Crypto.Encryption.DecryptMessage((this.win as email_encrpt.Message_Window).message.Body.Text, Pass.Password);
                   (this.win as Message_Window).wb1.NavigateToString(HTMLEncoding.TextToHTML(message));
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                this.Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (this.win.GetType() == (typeof(email_encrpt.LoginWindow)) && !check)
            {
                this.win.Close();
                this.Close();
                Application.Current.Shutdown();
            }
  
            else if (this.win.GetType() == (typeof(email_encrpt.Message_Window)))
            {
                this.win.Show();
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
