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
using ImapX.Collections;
using System.IO;
namespace email_encrpt
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class email : Window
    {
        private static ImapClient client { get; set; }

        public email()
        {
            InitializeComponent();
        }

        public void init(ImapClient cl)
        {
            //add items to ListView
            client = cl;
            foldersList.ItemsSource = EmailHelper.GetFolders(client);
        }
        private void foldersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Reloadfolder();
        }

        private void Reloadfolder()
        {
            var item = foldersList.SelectedItem as Folder;
            if (item != null)
            {
                // Load the folder for its messages.
                messagesList.ItemsSource = null;
                messagesList.ItemsSource = EmailHelper.loadFolder(item.Name, client);
                messagesList.Items.Refresh();
            }
        }

        private void createmail_Click(object sender, RoutedEventArgs e)
        {
            Write_Email write_email = new Write_Email();
            write_email.Show();
        }

        private void messagesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var message = (sender as ListView).SelectedItem as Message;
            if (message != null && (sender as ListView).SelectedItems.Count == 1)
            {
                Message_Window message_window = new Message_Window(message);
                message_window.Show();
            }
        }

        private void delete_emails_Click(object sender, RoutedEventArgs e)
        {
            Message[] messages = new Message[this.messagesList.SelectedItems.Count];
            this.messagesList.SelectedItems.CopyTo(messages, 0);
            foreach(var message in messages)
            message.Remove();
            MessageBox.Show("Selected Emails has been deleted");
            Reloadfolder();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (MyInfo.isconnected)
                client.Logout();
            App.Current.Shutdown();   
        }

        private void ShowPubKey_Click(object sender, RoutedEventArgs e)
        {
            FileStream pubKey = new FileStream("C:/EasySecurity/userPubKey.txt", FileMode.Open, FileAccess.Read, FileShare.None);
            string pubkeyxml = "";
            using (StreamReader sr = new StreamReader(pubKey))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                    pubkeyxml += line;
            }
            MessageBox.Show(pubkeyxml, "Your Public Key", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}
