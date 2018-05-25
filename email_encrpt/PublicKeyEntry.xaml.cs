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

namespace email_encrpt
{
    /// <summary>
    /// Interaction logic for PublicKeyEntry.xaml
    /// </summary>
    public partial class PublicKeyEntry : Window
    {
        Window win;
        public PublicKeyEntry(Window win)
        {
            this.win = win;
            InitializeComponent();

            if ((win as Write_Email).Encrypt_Sign.IsChecked == true)
            {
                label1.Text += " and your keys password.";
                label.Visibility = Visibility.Visible;
                Pass.Visibility = Visibility.Visible;
            }
        }
        private void done_Click(object sender, RoutedEventArgs e)
        {
            (win as Write_Email).pubKey = PubKey.Text;
            (win as Write_Email).pass = Pass.Password;
            win.Show();
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            win.Show();
            this.Close();
        }
    }
}
