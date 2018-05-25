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
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message_Window : Window
    {
        public ImapX.Message message;
        public Message_Window(ImapX.Message message)
        {
            InitializeComponent();
            this.message = message;
            if (this.message.Body.HasHtml)
                wb1.NavigateToString(message.Body.Html);
            else if(this.message.Body.HasText)
                wb1.NavigateToString(HTMLEncoding.TextToHTML(message.Body.Text));
            
            from.Text = this.message.From.ToString();
            subject.Text = this.message.Subject;
            time.Text = this.message.Date.Value.ToString();
        }
        private void reply_Click(object sender, RoutedEventArgs e)
        {
            Write_Email reply = new Write_Email(message,"Reply");
            reply.Show();
            this.Close();
        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            Write_Email forward = new Write_Email(message, "Forward");
            forward.Show();
            this.Close();
        }

        private void decrypt_Click(object sender, RoutedEventArgs e)
        {
            Enter_Password pass = new Enter_Password(this);
            this.Hide();
            pass.Show();
        }
    }
}
