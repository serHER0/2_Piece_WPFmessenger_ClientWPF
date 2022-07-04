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

namespace ClientWPF
{
    /// <summary>
    /// Логика взаимодействия для chatWindow.xaml
    /// </summary>
    public partial class chatWindow : Window
    {
        public chatWindow()
        {
            InitializeComponent();
        }

        static string mainNick;
        static string nickf;
        public void delegateinfo(string nickfriend, string mainnick)
        {
            mainNick = mainnick;
            nickf = nickfriend;
        }

        private void sendButt_Click(object sender, RoutedEventArgs e)
        {
            if (chatBox.Text != "")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.sendMessage(chatBox.Text, nickf);
                chatList.Items.Clear();
                BdClass reg = new BdClass();
                reg.viewdialog(mainNick, nickf, out List<string> messageslist);
                foreach (var item in messageslist)
                    chatList.Items.Add(item);
                chatBox.Text = "";
            }
        }

        private void updateButt_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainw = new MainWindow();
            mainw.update();
            chatW_load(sender,e);
        }

        public void updatemet()
        {
            chatList.Items.Clear();
            BdClass reg = new BdClass();
            reg.viewdialog(mainNick, nickf, out List<string> messageslist);
            foreach (var item in messageslist)
                chatList.Items.Add(item);
        }
        private void chatW_load(object sender, RoutedEventArgs e)
        {

            accLabel.Content = $"Accaunt:{mainNick}";
            friendLabel.Content = $"Friend:{nickf}";
            BdClass bd = new BdClass();
            bd.viewdialog(mainNick, nickf, out List<string> messageslist);
            foreach (var item in messageslist)
                chatList.Items.Add(item);

        }
    }
}
