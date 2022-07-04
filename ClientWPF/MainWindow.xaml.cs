using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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


namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        static byte[] bobPublickKey;
        static byte[] bobPrivateKey;
        static byte[] IV_ID;
        static byte[] ID_Key;
        static string reply = null;
        static byte[] friendid;
        static int type_message;
        static string nickf;
        static string ip1;
        SimpleTcpClient client;

        private void registButt_Click(object sender, RoutedEventArgs e)
        {
            if (loginBox.Text.Length >= 10 & loginBox.Text.Length <= 12 & passBox.Text.Length <= 12 & passBox.Text.Length >= 10)
            {
                client = new SimpleTcpClient(ipBox.Text);
                ip1 = ipBox.Text;
                client.Events.Connected += Events_Connected;
                client.Events.DataReceived += Event_DataReceived;
                client.Events.Disconnected += Events_Disconnected;

                type_message = 11;
                reply = "regist";
                while (client.IsConnected == false)
                {

                    try
                    {

                        client.Connect();
                        System.Threading.Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }


                }

                if (client.IsConnected)
                {
                    Message mess = new Message();
                    mess.setlog(loginBox.Text);
                    mess.setpass(passBox.Text);
                }
            }
            else
            {

            }
            //Client.chatwpf chat = new Client.chatwpf();
            //chat.Close();
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {

                disconnLabel.Content = "Статус:сервер отключился";
                chatWindow chatw = new chatWindow();
                BdClass bd = new BdClass();
                bd.check_sesKey(out string nick, out byte[] seskey, out byte[] iv, out DateTime time, out int id);
                if (time > DateTime.Now)
                {
                    if (nickf != null)
                    {
                        if (type_message == 51)
                        {
                            //chatw.updatemet();
                        }

                    }
                }

            }));
        }

        public void newthorrent(byte[] mess)
        {
            BdClass registetre = new BdClass();
            registetre.check_sesKey(out string nick1, out byte[] seskey, out byte[] iv11, out DateTime time1, out int id1);
            registetre.Check_Login(nick1, out string reply, out byte[] iduser, out byte[] iv1);
            keysClass keys = new keysClass();
            keys.DecryptMsg(seskey, mess, iv11, out byte[] message1);
            keys.DecryptMsg(iduser, message1, iv1, out message1);
            List<byte[]> messages = new List<byte[]>();
            unbyte_massiv(message1, 1, out byte[] leghtCount, out message1);
            unbyte_massiv(message1, Convert.ToInt16(Encoding.UTF8.GetString(leghtCount)), out byte[] countMess, out message1);
            int count = Convert.ToInt16(Encoding.UTF8.GetString(countMess));
            for (int i = 0; i < count; i++)
            {
                unbyte_massiv(message1, 2, out byte[] leg, out message1);
                unbyte_massiv(message1, Convert.ToInt16(Encoding.UTF8.GetString(leg)), out byte[] messa, out message1);
                messages.Add(messa);
            }
            //unbyte_massiv_IV(message1, out byte[] iv, out byte[] mess1);

            foreach (byte[] m in messages)
            {
                unbyte_massiv(m, 2, out byte[] leg, out byte[] messes);
                unbyte_massiv(messes, Convert.ToInt16(Encoding.UTF8.GetString(leg)), out byte[] nickf, out messes);
                unbyte_massiv_IV(messes, out byte[] iv, out messes);
                registetre.Friendmessage(nick1, Encoding.UTF8.GetString(nickf), messes, iv, messes.Length);
            }
        }
        private void unbyte_massiv_IV(byte[] ty, out byte[] IV1, out byte[] msg1)
        {
            byte[] msg = new byte[ty.Length - 16];
            byte[] IV = new byte[16];
            System.Buffer.BlockCopy(ty, 0, IV, 0, 16);
            System.Buffer.BlockCopy(ty, 16, msg, 0, ty.Length - 16);
            msg1 = msg;
            IV1 = IV;
        }
        public void addREQfriend(string nickfr)
        {
            type_message = 32;
            client = new SimpleTcpClient(ip1);
            nickf = nickfr;
            client.Events.Connected += Events_Connected;
            client.Events.DataReceived += Event_DataReceived;
            client.Events.Disconnected += Events_Disconnected;
            while (client.IsConnected == false)
            {

                try
                {

                    client.Connect();

                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }

            if (client.IsConnected)
            {
                //Message mess = new Message();
                //mess.setlog(loginBox.Text);
                // mess.setpass(Pass_USER.Text);
            }

        }
        public void addfriend(string nickfr)
        {
            keysClass sessionKey = new keysClass();
            BdClass registetre = new BdClass();
            nickf = nickfr;
            registetre.check_sesKey(out string nick, out byte[] sessKey, out byte[] IV, out DateTime time, out int id);
            if (time > DateTime.Now)
            {

                type_message = 31;
                client = new SimpleTcpClient(ip1);
                client.Events.Connected += Events_Connected;
                client.Events.DataReceived += Event_DataReceived;
                client.Events.Disconnected += Events_Disconnected;
                while (client.IsConnected == false)
                {

                    try
                    {

                        client.Connect();
                        System.Threading.Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                }

                if (client.IsConnected)
                {
                    // Message mess = new Message();
                    // mess.setlog(log_USER.Text);

                }
            }
            else
            {
                //info.Items.Add("Войдите в аккаунт");

            }
        }

        private void Event_DataReceived(object sender, DataReceivedEventArgs e)
        {
            _ = Dispatcher.BeginInvoke(new Action(delegate
            {
                keysClass sessionKey = new keysClass();
                BdClass registetre = new BdClass();
                Message mess = new Message();
                byte[] M_S = e.Data;
                unbyte_massiv(M_S, 2, out byte[] type_massage, out byte[] M_S1);
                int c = Convert.ToInt16(Encoding.UTF8.GetString(type_massage));
                int a = mess.SetTypeM(Convert.ToUInt16(Encoding.UTF8.GetString(type_massage)), M_S1);
                if (a == 1)
                {
                    byte[] rep = mess.GetTypeM();
                    client.Send(rep);
                }
                else
                {
                    byte[] rep = mess.GetTypeM();
                    if (Encoding.UTF8.GetString(rep) == "zero")
                    {
                        //client.Disconnect();
                    }
                    if (UTF8Encoding.UTF8.GetString(rep) == "good")
                    {
                        registetre.check_sesKey(out string nick, out byte[] sessKey, out byte[] IV, out DateTime time, out int id);
                        registetre.addfriend(nick, nickf, friendid);
                        registetre.dellfriendReq1(nickf, nick);
                        client.Disconnect();

                    }
                    else
                    {
                        registetre.check_sesKey(out string nick, out byte[] sessKey, out byte[] iv, out DateTime time, out int id);
                        //setnickacc(nick);
                        statusLabel.Content = $"Status:" + (Encoding.UTF8.GetString(rep));
                        client.Disconnect();

                    }

                }


            }));
        }

        public void sendMessage(string message, string nickf)
        {
            BdClass bd = new BdClass();
            //вывод информации об аккаунте из бд
            bd.check_sesKey(out string mainnick, out byte[] seskey, out byte[] iv, out DateTime time, out int id);
            //вывод friendID которому отправляем сообщение
            bd.idfriend(mainnick, nickf, out byte[] idfriend);
            keysClass keys = new keysClass();
            //конвертируем сообщение в массив байт
            byte[] messag = Encoding.UTF8.GetBytes(message);
            //шифруем с помощью friendID и генерируем публичный ключ
            keys.EncryptMsg(idfriend, Encoding.UTF8.GetBytes(message), out byte[] encryMess, out byte[] ivmess);
            //выводим ID_Key
            bd.Check_Login(mainnick, out string answer1, out byte[] ID_Key1, out byte[] ID_IV1);
            //длинна сообщения
            int leghtmess = encryMess.Length;
            //Сохраняем свое сообщение в бд клиента
            bd.Mainmessage(mainnick, nickf, encryMess, ivmess, leghtmess);
            // обеденяем зашифрованное сообщение с публичным ключем
            encryMess = byte_massiv(ivmess, encryMess);
            //добавляем ник получателя
            encryMess = byte_massiv(Encoding.UTF8.GetBytes(nickf), encryMess);
            byte[] bytenickf = Encoding.UTF8.GetBytes(nickf);
            //длинна ника
            int leght = bytenickf.Length;
            //добавляем длинну ника
            encryMess = byte_massiv(UTF8Encoding.UTF8.GetBytes(Convert.ToString(leght)), encryMess);
            //шифруем сообщение ID_Key и сеансовым ключем
            keys.EncryptMsg_IV(ID_Key1, encryMess, out encryMess, ID_IV1);
            keys.EncryptMsg_IV(seskey, encryMess, out encryMess, iv);

            byte[] idbyte = Encoding.UTF8.GetBytes(Convert.ToString(id));

            byte[] leghtid = UTF8Encoding.UTF8.GetBytes(Convert.ToString(idbyte.Length));
            //добавляем Id отправителя
            encryMess = byte_massiv(idbyte, encryMess);
            //добавляем длинну Id
            encryMess = byte_massiv(leghtid, encryMess);
            //добавляем код сообщения
            encryMess = byte_massiv(UTF8Encoding.UTF8.GetBytes(Convert.ToString(41)), encryMess);

            client = new SimpleTcpClient(ip1);
            client.Events.Connected += Events_Connected;
            client.Events.DataReceived += Event_DataReceived;
            client.Events.Disconnected += Events_Disconnected;

            type_message = 41;
            reply = "regist";
            while (client.IsConnected == false)
            {

                try
                {

                    client.Connect();
                    System.Threading.Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }


            }

            if (client.IsConnected)
            {
                client.Send(encryMess);
                //client.Disconnect();
            }
        }

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                statusLabel.Content = "status:Server connected";

                keysClass sessionKey = new keysClass();
                Message mess = new Message();
                if (type_message == 11)
                {
                    //1.1 создание и отправка публичного ключа для сессионного ключа

                    mess.generateKey(out bobPublickKey, out bobPrivateKey);

                    byte[] fof = byte_massiv(Encoding.UTF8.GetBytes(Convert.ToString(type_message)), bobPublickKey);
                    client.Send(fof);

                }
                if (type_message == 21)
                {
                    //2.1
                    //генерация сеансового  ключа
                    mess.generateKey(out bobPublickKey, out bobPrivateKey);
                    byte[] fof = byte_massiv(Encoding.UTF8.GetBytes(Convert.ToString(type_message)), bobPublickKey);
                    client.Send(fof);
                }
                if (type_message == 31)
                {
                    //3.1

                    keysClass seSSionKey = new keysClass();
                    BdClass registetre = new BdClass();
                    //Выгрузка сеансового ключа из бд
                    registetre.check_sesKey(out string nick, out byte[] sessKey, out byte[] IV, out DateTime time, out int id);
                    //Выгрузка ID_Key из бд
                    registetre.Check_Login(nick, out string answer1, out byte[] ID_Key, out byte[] ID_IV);
                    //Генерация публичного и приватного ключа
                    mess.generateKey(out bobPublickKey, out bobPrivateKey);
                    mess.setlog(loginBox.Text);
                    mess.setPrivet(bobPrivateKey);

                    mess.setNfriend(nickf);
                    //Конвертация ник искомого пользователя в массив байтов
                    byte[] nickbyte = Encoding.UTF8.GetBytes(nickf);
                    //Объединение ника и публичного ключа в один массив
                    byte[] fof02 = byte_massiv(nickbyte, bobPublickKey);
                    //сохранение длинны ника
                    byte[] valuenick = Encoding.UTF8.GetBytes(Convert.ToString(nickf.Length));
                    //добавление ее в массив
                    fof02 = byte_massiv(valuenick, fof02);
                    //добавление id сеансового ключа клиента в массив
                    byte[] byteID = Encoding.UTF8.GetBytes(Convert.ToString(id));
                    //сохранение длинны id
                    byte[] valueid = Encoding.UTF8.GetBytes(Convert.ToString(byteID.Length));
                    //шифрование массива ID_Key
                    seSSionKey.EncryptMsg_IV(ID_Key, fof02, out fof02, ID_IV);
                    //Шифрование массива сеансовым ключом
                    seSSionKey.EncryptMsg_IV(sessKey, fof02, out fof02, IV);
                    fof02 = byte_massiv(byteID, fof02);
                    fof02 = byte_massiv(valueid, fof02);
                    byte[] fof = byte_massiv(Encoding.UTF8.GetBytes(Convert.ToString(type_message)), fof02);
                    client.Send(fof);


                }
                if (type_message == 32)
                {

                    BdClass registetre = new BdClass();
                    keysClass seSSionKey = new keysClass();

                    registetre.check_sesKey(out string nick, out byte[] seskey, out byte[] IV, out DateTime time, out int id);
                    registetre.Check_Login(nick, out string answer1, out byte[] ID_Key, out byte[] ID_IV);


                    registetre.req_in_friend(nick, nickf, out byte[] pubkey);
                    mess.generateKey(out bobPublickKey, out bobPrivateKey);
                    mess.setalicepubkey(pubkey);
                    mess.allpuck(seskey, IV, ID_Key, ID_IV);

                    byte[] byteid = Encoding.UTF8.GetBytes(Convert.ToString(id));
                    byte[] byteleghtid = Encoding.UTF8.GetBytes(Convert.ToString(byteid.Length));
                    byteid = byte_massiv(byteleghtid, byteid);
                    byte[] encrykey = byte_massiv(bobPublickKey, Encoding.UTF8.GetBytes(nickf));
                    seSSionKey.EncryptMsg_IV(ID_Key, encrykey, out encrykey, ID_IV);
                    seSSionKey.EncryptMsg_IV(seskey, encrykey, out encrykey, IV);
                    encrykey = byte_massiv(byteid, encrykey);

                    client.Send(byte_massiv(Encoding.UTF8.GetBytes(Convert.ToString(type_message)), encrykey));

                }
                if (type_message == 41)
                {

                }
                if (type_message == 51)
                {
                    BdClass registetre = new BdClass();
                    // извлечение Id из бд
                    registetre.check_sesKey(out string nick, out byte[] seskey, out byte[] IV, out DateTime time, out int id);

                    byte[] newsreq = byte_massiv(Encoding.UTF8.GetBytes("51"), UTF8Encoding.UTF8.GetBytes(Convert.ToString(id)));
                    client.Send(newsreq);
                }


            }));
        }

        private void logButt_Click(object sender, RoutedEventArgs e)
        {
            if (loginBox.Text.Length >= 10 & loginBox.Text.Length <= 12 & passBox.Text.Length <= 12 & passBox.Text.Length >= 10)
            {
                client = new SimpleTcpClient(ipBox.Text);
                ip1 = ipBox.Text;
                client.Events.Connected += Events_Connected;
                client.Events.DataReceived += Event_DataReceived;
                client.Events.Disconnected += Events_Disconnected;
                //client.Events.DataReceived += Event_login;
                //logButt.IsEnabled = false;
                //exitbutt.IsEnabled = true;
                reply = "login";
                type_message = 21;
                while (client.IsConnected == false)
                {

                    try
                    {

                        client.Connect();
                        System.Threading.Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        // MessageBox.Show(ex.Message);
                    }
                }

                //friendList.Items.Clear();

                if (client.IsConnected)
                {

                    Message mess = new Message();
                    mess.setlog(loginBox.Text);
                    mess.setpass(passBox.Text);
                }
            }
            else
            {
                statusLabel.Content = ("Введите логин и пароль");
                //friendList.Items.Clear();
            }
            //Client.chatwpf chat = new Client.chatwpf();
            //chat.Close();
        }

        public void update()
        {
            BdClass bd = new BdClass();
            client = new SimpleTcpClient(ip1);
            client.Events.Connected += Events_Connected;
            client.Events.DataReceived += Event_DataReceived;
            client.Events.Disconnected += Events_Disconnected;
            while (client.IsConnected == false)
            {
                type_message = 51;
                try
                {

                    client.Connect();
                    System.Threading.Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }
        }


        public void showAccWindow()
        {
            accWindow accw = new accWindow();
            accw.Show();
            Hide();
        }
        private byte[] byte_massiv(byte[] one, byte[] two)
        {
            byte[] mass = new byte[one.Length + two.Length];
            System.Buffer.BlockCopy(one, 0, mass, 0, one.Length);
            System.Buffer.BlockCopy(two, 0, mass, one.Length, two.Length);
            return (mass);
        }
        private void unbyte_massiv(byte[] mass, out byte[] one, out byte[] two)
        {

            byte[] mass1 = new byte[mass.Length];
            one = new byte[16];
            two = new byte[mass.Length - 16];
            System.Buffer.BlockCopy(mass, 0, one, 0, 16);
            System.Buffer.BlockCopy(mass, 16, two, 0, mass.Length - 16);


        }
        private void unbyte_massiv(byte[] mass, int lenght, out byte[] one, out byte[] two)
        {

            byte[] mass1 = new byte[mass.Length];
            one = new byte[lenght];
            two = new byte[mass.Length - lenght];
            System.Buffer.BlockCopy(mass, 0, one, 0, lenght);
            System.Buffer.BlockCopy(mass, lenght, two, 0, mass.Length - lenght);


        }
        private void mix_byte(byte[] one, byte[] two, out byte[] mix)
        {
            int a = 0;
            int b = 1;
            mix = new byte[one.Length * 2];
            for (int i = 0; i < one.Length; i++)
            {
                a = a + i;
                b = b + i;
                mix[a] = one[i];
                mix[b] = two[i];
                a++;

                System.Buffer.BlockCopy(one, i, mix, i, 1);
                System.Buffer.BlockCopy(two, i, mix, a, 1);
            }
        }

        public class Message
        {
            private int X;
            private byte[] rep;
            static byte[] bobPublickKey;
            static byte[] bobPrivateKey;
            static byte[] bobPublickKey_ID;
            static byte[] bobPrivateKey_ID;
            static byte[] alicePublickKey;
            static byte[] sessionKey;
            static byte[] sessionKey_IV;
            static byte[] ID_Key;
            static byte[] ID_IV;
            static string login;
            static string pass;
            static byte[] hash;
            static byte[] privetK;
            static string friendnick;
            public void allpuck(byte[] seskey, byte[] iv, byte[] idkey, byte[] idiv)
            {
                sessionKey = seskey;
                sessionKey_IV = iv;
                ID_IV = idiv;
                ID_Key = idkey;
            }
            public void setalicepubkey(byte[] alipubkey)
            {
                alicePublickKey = alipubkey;
            }
            public void setseskey(byte[] seskey)
            {
                sessionKey = seskey;
            }
            public void setsesiv(byte[] iv)
            {
                sessionKey_IV = iv;
            }
            public void setPrivet(byte[] privetKey)
            {
                privetK = privetKey;
            }
            public void setNfriend(string rnick)
            {
                friendnick = rnick;
            }
            public void setlog(string log)
            {
                login = log;
            }
            public void setpass(string passw)
            {
                pass = passw;
            }
            private byte[] byte_massiv(byte[] one, byte[] two)
            {
                byte[] mass = new byte[one.Length + two.Length];

                System.Buffer.BlockCopy(one, 0, mass, 0, one.Length);
                System.Buffer.BlockCopy(two, 0, mass, one.Length, two.Length);
                return (mass);
            }
            public void generateKey(out byte[] bobpub, out byte[] bobpriv)
            {
                keysClass seSSionKey = new keysClass();
                seSSionKey.generate_PublicKey(out bobPublickKey, out bobPrivateKey);
                bobpub = bobPublickKey;
                bobpriv = bobPrivateKey;
            }
            static List<byte[]> listmess;
            public int SetTypeM(int x, byte[] message)
            {

                listmess = new List<byte[]>();
                int ost = 0;
                this.X = x;
                keysClass seSSionKey = new keysClass();
                if (X == 11)
                {
                    sessionKey = seSSionKey.Creating_SessionKey(message, bobPrivateKey);
                    seSSionKey.generate_PublicKey(out bobPublickKey_ID, out bobPrivateKey_ID);
                    this.rep = byte_massiv(Encoding.UTF8.GetBytes(Convert.ToString(12)), bobPublickKey_ID);
                    ost = 1;
                }
                if (X == 12)
                {

                    ID_Key = seSSionKey.Creating_SessionKey(message, bobPrivateKey_ID);
                    var md5 = MD5.Create();
                    //Создание хэша с помощью логина и пароля
                    hash = md5.ComputeHash((Encoding.UTF8.GetBytes(login + pass)));
                    byte[] log_byte = Encoding.UTF8.GetBytes(login);
                    // объединение логина и хэша в массив байт 
                    byte[] log_and_hash = byte_massiv(log_byte, hash);
                    //шифрование массива с помощью id_Key
                    byte[] encypt_log;
                    seSSionKey.EncryptMsg(ID_Key, log_and_hash, out encypt_log, out ID_IV);
                    int log_lenght = log_byte.Length;
                    byte[] dopInfo = Encoding.UTF8.GetBytes(Convert.ToString(log_lenght));
                    // объединение зашифрованного массива с длиной логина
                    byte[] info_crypt = byte_massiv(dopInfo, encypt_log);
                    // объединение массива с публючным ID_Key
                    byte[] logi = byte_massiv(ID_IV, info_crypt);
                    byte[] encrypt_logi;
                    // Шифрование массива с помощью сеансовго ключа
                    seSSionKey.EncryptMsg(sessionKey, logi, out encrypt_logi, out sessionKey_IV);
                    // Объединение зашифрованного массива с публичным сеансовым ключем
                    encrypt_logi = byte_massiv(sessionKey_IV, encrypt_logi);
                    // объединение с кодом сообщения
                    this.rep = byte_massiv(Encoding.UTF8.GetBytes(Convert.ToString(13)), encrypt_logi);
                    ost = 1;

                }
                if (X == 13)
                {
                    BdClass registetre = new BdClass();
                    //расшифровка
                    seSSionKey.DecryptMsg(sessionKey, message, sessionKey_IV, out message);
                    seSSionKey.DecryptMsg(ID_Key, message, ID_IV, out message);
                    //вывод в окон статуса результат проверки
                    string reply1 = Encoding.UTF8.GetString(message);
                    if (reply1 == "registed")
                    {

                        // byte[] finaly_ID = byte_massiv(IV_ID,ID_Key);
                        registetre.Registration_0(login, hash, ID_Key, ID_IV);
                        this.rep = Encoding.UTF8.GetBytes("Вы успешно зарегистрированы!");
                    }
                    else
                    {
                        this.rep = Encoding.UTF8.GetBytes("Такой логин уже существует!");
                    }
                    ost = 0;
                }
                if (X == 21)
                {
                    //создание сеансового ключа
                    sessionKey = seSSionKey.Creating_SessionKey(message, bobPrivateKey);
                    //Шифрование логина сеансовым ключем
                    seSSionKey.EncryptMsg(sessionKey, Encoding.UTF8.GetBytes(login), out message, out sessionKey_IV);
                    // объединение байт массивов публичного сеансового ключа и зашифрованного массива
                    message = byte_massiv(sessionKey_IV, message);
                    this.rep = byte_massiv(Encoding.UTF8.GetBytes(Convert.ToString(22)), message);
                    ost = 1;
                }
                if (X == 22)
                {
                    BdClass registetre = new BdClass();
                    seSSionKey.DecryptMsg(sessionKey, message, sessionKey_IV, out message);
                    if (Encoding.UTF8.GetString(message) == "good")
                    {
                        registetre.Check_Login(login, out string answer1, out ID_Key, out ID_IV);
                        if (answer1 == "good")
                        {
                            //системное время клиента
                            DateTime localDate = DateTime.Now;

                            string date_HH = localDate.ToString("HH");
                            string date_mm = localDate.ToString("mm");
                            string date_ss = localDate.ToString("ss");
                            //соль
                            int time = Convert.ToInt32(date_HH) + Convert.ToInt32(date_mm) + Convert.ToInt32(date_ss);

                            var rand = new Random(time);

                            var md5 = MD5.Create();
                            // хэширование введненых пользователем данных
                            hash = md5.ComputeHash((Encoding.UTF8.GetBytes(login + pass)));
                            byte[] sole = new byte[hash.Length];
                            //генерация массива байтов с помощью соли
                            rand.NextBytes(sole);
                            // смешивание массивов
                            byte[] mix = byte_massiv(sole, hash);
                            //хэширование полученного массива
                            byte[] Hash_Test = md5.ComputeHash(mix);
                            byte[] soleT = { (byte)time };
                            //формирование массива байтов из соли и хэша
                            byte[] messagePass = byte_massiv(soleT, Hash_Test);
                            //шифрование массива ID_Key
                            seSSionKey.EncryptMsg_IV(ID_Key, messagePass, out message, ID_IV);
                            //шифрование массива сеансовым ключем
                            seSSionKey.EncryptMsg_IV(sessionKey, message, out message, sessionKey_IV);
                            this.rep = byte_massiv(Encoding.UTF8.GetBytes(Convert.ToString(23)), message);
                            ost = 1;
                        }
                        else
                        {
                            this.rep = Encoding.UTF8.GetBytes(answer1);
                            ost = 0;
                        }
                    }
                    else
                    {
                        this.rep = Encoding.UTF8.GetBytes("Такой пользователь не зарегистрирован!");
                        ost = 0;
                    }
                }
                if (X == 23)
                {
                    BdClass registetre = new BdClass();
                    seSSionKey.DecryptMsg(sessionKey, message, sessionKey_IV, out byte[] reply_fin);
                    seSSionKey.DecryptMsg(ID_Key, reply_fin, ID_IV, out reply_fin);
                    string info = Encoding.UTF8.GetString(reply_fin);
                    if (info != "Неправильный пароль!")
                    {
                        DateTime localDate = DateTime.Now.AddHours(5);
                        int id = Convert.ToInt16(info);
                        registetre.Check_and_add_SessKey_log(login, sessionKey_IV, sessionKey, localDate, id);
                        MainWindow mainw = new MainWindow();
                        mainw.showAccWindow();

                        this.rep = Encoding.UTF8.GetBytes("Вы успешно вошли");

                        ost = 0;
                    }
                    else
                    {
                        this.rep = Encoding.UTF8.GetBytes("Ошибка пароля");
                        ost = 0;
                    }

                }
                if (X == 31)
                {
                    BdClass registetre = new BdClass();
                    registetre.check_sesKey(out string nick, out sessionKey, out sessionKey_IV, out DateTime time, out int id);
                    registetre.Check_Login(nick, out string reply12, out ID_Key, out ID_IV);
                    seSSionKey.DecryptMsg(sessionKey, message, sessionKey_IV, out message);
                    seSSionKey.DecryptMsg(ID_Key, message, ID_IV, out message);
                    if (Encoding.UTF8.GetString(message) != "Пользователя не существует")
                    {
                        registetre.check_friendReq1(nick, friendnick, out byte[] key, out string reply);
                        if (reply != null)
                        {
                            registetre.dellfriendReq(nick, friendnick);
                            registetre.addFriendList(nick, friendnick, privetK, 1);
                        }
                        else
                        {
                            registetre.addFriendList(nick, friendnick, privetK, 1);
                        }
                        this.rep = Encoding.UTF8.GetBytes("Запрос отправлен");
                        ost = 0;

                    }
                    else
                    {

                        this.rep = Encoding.UTF8.GetBytes("Пользователя не существует");
                        ost = 0;
                    }
                }
                if (X == 32)
                {
                    seSSionKey.DecryptMsg(sessionKey, message, sessionKey_IV, out message);
                    seSSionKey.DecryptMsg(ID_Key, message, ID_IV, out message);
                    if (Encoding.UTF8.GetString(message) == "Good")
                    {
                        byte[] idfriend = seSSionKey.Creating_SessionKey(alicePublickKey, bobPrivateKey);
                        MainWindow wind = new MainWindow();
                        wind.setfriendid(idfriend);
                        this.rep = Encoding.UTF8.GetBytes("good");
                        ost = 0;
                    }

                }
                if (X == 51)
                {
                    BdClass registetre = new BdClass();
                    // извлечение данных
                    registetre.check_sesKey(out string nick1, out byte[] seskey, out byte[] iv, out DateTime time, out int id);
                    registetre.Check_Login(nick1, out string reply, out byte[] iduser, out byte[] iv1);
                    keysClass keys = new keysClass();
                    //расшифровка сообщения
                    keys.DecryptMsg(seskey, message, iv, out message);
                    keys.DecryptMsg(iduser, message, iv1, out message);
                    // извлечения публичного ключа из массива данных
                    unbyte_massiv(message, 140, out byte[] pubfrikey, out byte[] byte_nickfri);
                    // конвертация массива байтов в ник отправителя
                    string nickfri = UTF8Encoding.UTF8.GetString(byte_nickfri);
                    // проверка запроса
                    registetre.friendlist(Encoding.UTF8.GetString(byte_nickfri), nick1, pubfrikey);
                    this.rep = Encoding.UTF8.GetBytes("zero");
                    ost = 0;
                }
                if (X == 52)
                {
                    listmess = new List<byte[]>();
                    MainWindow mainw = new MainWindow();

                    mainw.newthorrent(message);

                    this.rep = Encoding.UTF8.GetBytes("zero");
                    ost = 0;

                }
                return ost;
            }
            private void unbyte_massiv_IV(byte[] ty, out byte[] IV1, out byte[] msg1)
            {
                byte[] msg = new byte[ty.Length - 16];
                byte[] IV = new byte[16];
                System.Buffer.BlockCopy(ty, 0, IV, 0, 16);
                System.Buffer.BlockCopy(ty, 16, msg, 0, ty.Length - 16);
                msg1 = msg;
                IV1 = IV;
            }
            private void unbyte_massiv(byte[] mass, int lenght, out byte[] one, out byte[] two)
            {

                byte[] mass1 = new byte[mass.Length];
                one = new byte[lenght];
                two = new byte[mass.Length - lenght];
                System.Buffer.BlockCopy(mass, 0, one, 0, lenght);
                System.Buffer.BlockCopy(mass, lenght, two, 0, mass.Length - lenght);


            }
            public byte[] GetTypeM()
            {
                return rep;
            }
        }


        private void setfriendid(byte[] idfriend)
        {
            friendid = idfriend;
        }

        private void MainWindow_load(object sender, RoutedEventArgs e)
        {
            ip1 = ipBox.Text;
            BdClass bd = new BdClass();
            bd.check_sesKey(out string nick, out byte[] seskey, out byte[] iv, out DateTime time, out int id);
            if (time > DateTime.Now)
            {
                accWindow accw = new accWindow();
                accw.Show();
                Hide();
            }
        }
    }
}
