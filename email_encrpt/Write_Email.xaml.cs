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
using ImapX;

namespace email_encrpt
{

    /// <summary>
    /// Interaction logic for Write_Email.xaml
    /// </summary>
    public partial class Write_Email : Window
    {
        public string pass, pubKey;
        public Write_Email()
        {
            InitializeComponent();
            this.No_Encryption.IsChecked = true;
            this.Encrypt.IsChecked = false;
            this.Encrypt_Sign.IsChecked = false;
            this.EnterEncParams.IsEnabled = false;
        }
        public Write_Email(ImapX.Message message, string mode)
        {
            InitializeComponent();
            if (mode == "Reply")
            {
                to.Text = message.From.Address;
                subject.Text = message.Subject;
                body.Text = message.Body.Text;
            }
            else
            {
                subject.Text = message.Subject;
                body.Text = message.Body.Text;
            }
        }

        private void send_mail_Click(object sender, RoutedEventArgs e)
        {
            string encryptedMessage = null;
            bool isEncrypted = false;
            if(this.Encrypt_Sign.IsChecked == true)
            {
                isEncrypted = true;
                if(string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(pubKey))
                {
                    MessageBox.Show("Please enter encryption paramaters first","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    encryptedMessage = Crypto.Encryption.EncryptAndSignMessage(body.Text, pubKey, pass);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }
            else if(this.Encrypt.IsChecked == true)
            {
                isEncrypted = true;
                if (string.IsNullOrEmpty(pubKey))
                {
                    MessageBox.Show("Please enter encryption paramaters first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    encryptedMessage = Crypto.Encryption.EncryptMessage(body.Text, pubKey);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            mess Mess = new mess();
            Mess.From = MyInfo.userName;
            Mess.To = to.Text;
            Mess.Subject = subject.Text;
            Mess.Body = (isEncrypted) ? encryptedMessage : body.Text;
            EmailHelper.SendMail(Mess); // mesage sent
            MessageBox.Show("Email has been sent");
            this.Close();
        }

        private void Encrypt_Checked(object sender, RoutedEventArgs e)
        {
            this.EnterEncParams.IsEnabled = true;
        }

        private void Encrypt_Sign_Checked(object sender, RoutedEventArgs e)
        {
            this.EnterEncParams.IsEnabled = true;

        }

        private void No_Encryption_Checked(object sender, RoutedEventArgs e)
        {
            this.EnterEncParams.IsEnabled = false;

        }

        private void EnterEncParams_Click(object sender, RoutedEventArgs e)
        {
            PublicKeyEntry pubKeyWin = new PublicKeyEntry(this);
            this.Hide();
            pubKeyWin.Show();
        }
    }
}
