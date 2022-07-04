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
    /// Логика взаимодействия для accWindow.xaml
    /// </summary>
    public partial class accWindow : Window
    {
        public accWindow()
        {
            InitializeComponent();
        }
        private static string accnick;
        private static string nickf;
        private void accWindow_load(object sender, RoutedEventArgs e)
        {
            BdClass bd = new BdClass();
            bd.check_sesKey(out accnick, out byte[] sesKey, out byte[] iv, out DateTime time, out int id1);
            acclabel.Content = accnick;
            MainWindow mainw = new MainWindow();
            mainw.Owner = this;

        }
        private void chatFriend_Click(object sender, RoutedEventArgs e)
        {
            if (friendList.SelectedItem != null)
            {
                chatWindow chatW = new chatWindow();
                nickf = friendList.SelectedItem.ToString();
                chatW.delegateinfo(nickf, accnick);
                chatW.Show();
            }

        }

        private void addFriendButt_Click(object sender, RoutedEventArgs e)
        {
            if (friendsButt.IsEnabled == true)
            {
                if (friendList.SelectedItem != null)
                {
                    MainWindow mainw = new MainWindow();
                    string nickf = friendList.SelectedItem.ToString();
                    mainw.addREQfriend(nickf);
                }
            }
        }

        private void reqButt_Click(object sender, RoutedEventArgs e)
        {
            friendsButt.IsEnabled = true;
            reqButt.IsEnabled = false;
            friendList.Items.Clear();
            BdClass registetre = new BdClass();
            registetre.check_sesKey(out string nick, out byte[] seskey, out byte[] IV, out DateTime time, out int id);
            if (time > DateTime.Now)
            {
                registetre.viewreqFriendlist(nick, out List<string> reqfriendlist);
                foreach (string reqfriend in reqfriendlist)
                    friendList.Items.Add(reqfriend);
            }
            else
            {
                friendList.Items.Add("Войдите в аккаунт");
            }
        }

        private void friendsButt_Click(object sender, RoutedEventArgs e)
        {
            friendsButt.IsEnabled = false;
            reqButt.IsEnabled = true;
            friendList.Items.Clear();
            BdClass registetre = new BdClass();
            registetre.check_sesKey(out string nick, out byte[] seskey, out byte[] IV, out DateTime time, out int id);
            if (time > DateTime.Now)
            {
                registetre.viewFriendlist(nick, out List<string> reqfriendlist);
                foreach (string reqfriend in reqfriendlist)
                    friendList.Items.Add(reqfriend);
            }
            else
            {
                friendList.Items.Add("Войдите в аккаунт");
            }
        }

        private void searchButt_Click(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text.Length >= 10 && searchBox.Text.Length <= 12)
            {
                string nickf = searchBox.Text;
                MainWindow mainw = new MainWindow();
                mainw.addfriend(nickf);
            }
        }

        private void exitButt_Click(object sender, RoutedEventArgs e)
        {
            BdClass registetre = new BdClass();
            registetre.check_sesKey(out string nick, out byte[] seskey, out byte[] IV, out DateTime time, out int id);
            registetre.dellseskey();
            friendsButt.IsEnabled = true;

            friendList.Items.Clear();
            Close();
            MainWindow mainw = new MainWindow();
            mainw.Show();
        }

        private void updateButt_Click(object sender, RoutedEventArgs e)
        {

            MainWindow mainw = new MainWindow();
            mainw.update();

        }
    }
}
