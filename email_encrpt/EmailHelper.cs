using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImapX;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace email_encrpt
{
    public struct mess
    {
        public string From, To, Body, Subject;
    }
    class EmailHelper
    {
        public static void First_Run(Window main)
        {
            Enter_Password pass = new Enter_Password(main);
            pass.Show();
        }
        public static ImapX.ImapClient Login(string str_email, string str_password)
        {
            ImapX.ImapClient client = new ImapClient("imap.gmail.com", true);
     
            string correct = "";
            string host = "";
            bool flag = false;
            for (int i = 0; i < str_email.Length; i++)
            {
                if (str_email[i] != ' ')
                    correct += str_email[i];
            }

            for (int i = 0; i < correct.Length; i++)
            {
                if (correct[i] == '@' || flag)
                {
                    if (flag)
                        host += correct[i];
                    flag = true;
                }
            }
            if (correct == "" || str_password == "")
                MessageBox.Show("Wrong username or password");
            else if (host != "")
            {
                str_email = correct;
                MyInfo.userName = str_email;
                MyInfo.password = str_password;
                MyInfo.host = host;
                host = "imap." + host;
                //client = new ImapClient(host, true);
                if (client.Connect())
                {
                    if (client.Login(str_email, str_password))
                    {
                        MyInfo.isconnected = true;
                    }
                }
                else
                {
                    MessageBox.Show("Error in connection");
                    // connection not successful
                }
            }
            else
                MessageBox.Show("Missing server name");

            return client;
        }

        public static List<Folder> GetFolders(ImapX.ImapClient client)
        {
            var folders = new List<Folder>();
            folders.Add(client.Folders.Inbox);

            for (int i = 0; i < client.Folders["[Gmail]"].SubFolders.Count(); i++)
                folders.Add(client.Folders["[Gmail]"].SubFolders[i]);

            // Before returning start the idling
            client.Folders.Inbox.StartIdling(); // And continue to listen for more.
            client.Folders.Inbox.OnNewMessagesArrived += Inbox_OnNewMessagesArrived;
            return folders;
        }

        private static void Inbox_OnNewMessagesArrived(object sender, IdleEventArgs e)
        {
            // Show a dialog
            MessageBox.Show($"A new message was downloaded in {e.Folder.Name} folder.");         
        }
        public static ImapX.Collections.MessageCollection loadFolder(string name, ImapX.ImapClient client)
        {
            if (name == "INBOX")
            {
                client.Folders[name].Messages.Download("ALL", ImapX.Enums.MessageFetchMode.Tiny, 10);
                return client.Folders[name].Messages;
            }
            else
            {
                client.Folders["[Gmail]"].SubFolders[name].Messages.Download("ALL", ImapX.Enums.MessageFetchMode.Tiny, 10);
                return client.Folders["[Gmail]"].SubFolders[name].Messages;
            }
        }

        public static MailMessage SendMail(mess Mess)
         {
             try
             {
                 System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                 mail.From = new System.Net.Mail.MailAddress(MyInfo.userName);
                 mail.To.Add(new System.Net.Mail.MailAddress(Mess.To));
                 mail.Subject = Mess.Subject;
                 mail.Body = Mess.Body;

                 SmtpClient client = new SmtpClient();

                 client.Host = "smtp.gmail.com";
                 client.Port = 587;
                 client.EnableSsl = true;
                 client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential(MyInfo.userName,MyInfo.password);
                 client.DeliveryMethod = SmtpDeliveryMethod.Network;
                 client.Send(mail);
                 return mail;
             }
             catch (Exception e)
             {
                 throw new Exception("Mail.Send: " + e.Message);
             }
         }
      
    }

}
