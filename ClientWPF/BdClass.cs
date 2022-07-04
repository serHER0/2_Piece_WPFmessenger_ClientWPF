using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF
{
    internal class BdClass
    {
        SqlConnection sqlConnection;
        string Connection_String = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Programmirpvan\finalDiplom\framework\ClientWPF\ClientWPF\bdUsers.mdf;Integrated Security=True";


        public void Registration_0(string login, byte[] Hash_User, byte[] ID_User, byte[] IV)
        {


            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();

            SqlCommand command = new SqlCommand("INSERT INTO [ID_Users] (log_User, Hash_User, ID_Key, IV)VALUES(@Val1, @val2, @val3, @val4)", sqlConnection);

            command.Parameters.AddWithValue("@val1", login);
            command.Parameters.AddWithValue("@val2", Hash_User);
            command.Parameters.AddWithValue("@val3", ID_User);
            command.Parameters.AddWithValue("@val4", IV);

            command.ExecuteNonQuery();
        }
        public void Check_Login(string log, byte[] hash, byte[] ID_Key, byte[] IV, out string reply)
        {
            string witness = null;
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT log_User from [ID_Users] where log_User = @login", sqlConnection);
            command.Parameters.AddWithValue("@login", log);
            try
            {
                sqlReader = command.ExecuteReader();
                while (sqlReader.Read())
                {
                    witness += Convert.ToString(sqlReader["log_User"]);
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

            if (witness == null)
            {
                Registration_0(log, hash, ID_Key, IV);
                reply = "Зарегистрирован";
            }
            else
            {
                reply = "Такой логин уже есть";
            }
        }
        public void Check_Login(string log, out string reply, out byte[] ID_Key1, out byte[] IV1)
        {
            string witness = null;
            byte[] ID_Key_1 = new byte[64];
            byte[] IV_1 = new byte[16];
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT log_User, ID_Key, IV from [ID_Users] where log_User = @login", sqlConnection);
            command.Parameters.AddWithValue("@login", log);
            try
            {
                sqlReader = command.ExecuteReader();
                if (sqlReader.Read())
                {
                    witness += Convert.ToString(sqlReader["log_User"]);
                    ID_Key_1 = (byte[])sqlReader["ID_Key"];
                    IV_1 = (byte[])sqlReader["IV"];
                }



            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

            if (witness == null)
            {

                reply = "Пользователь не подключен к устройству";
                ID_Key1 = null;
                IV1 = null;
            }
            else
            {


                ID_Key1 = ID_Key_1;
                IV1 = IV_1;
                reply = "good";

            }
        }
        public void Check_SessKey_log(string login, out byte[] IV, out byte[] sessKey, out DateTime time, out string nick, out int id)
        {
            byte[] sessKey1 = new byte[64];
            byte[] IV1 = new byte[16];
            string nick1 = null;
            int id1 = 0;
            DateTime time1 = DateTime.Now;
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT nick, sesKey, IV, dateDel, sesID from [session_keys] where nick =@nick1", sqlConnection);
            command.Parameters.AddWithValue("@nick1", login);
            try
            {
                sqlReader = command.ExecuteReader();
                if (sqlReader.Read())
                {
                    nick1 += Convert.ToString(sqlReader["nick"]);
                    sessKey1 = (byte[])sqlReader["sesKey"];
                    IV1 = (byte[])sqlReader["IV"];
                    time1 = (DateTime)sqlReader["dateDel"];
                    id1 = (int)sqlReader["sesID"];
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

            if (nick1 != null)
            {
                if ((time1 - DateTime.Now).Hours > 0)
                {
                    nick = nick1;
                    IV = IV1;
                    sessKey = sessKey1;
                    time = time1;
                    id = id1;
                }
                else
                {
                    SqlCommand command1 = new SqlCommand("DELETE FROM [session_keys]", sqlConnection);
                    command.ExecuteNonQuery();
                    nick = "Сеансовый ключ не создан";
                    IV = null;
                    sessKey = null;
                    time = DateTime.Now;
                    id = 0;
                }

            }
            else
            {
                nick = "Сеансовый ключ не создан";
                IV = null;
                sessKey = null;
                time = time1;
                id = 0;
            }


        }
        public void Check_and_add_SessKey_log(string login, byte[] IV, byte[] sessKey, DateTime time, int id)
        {
            string nick1 = null;
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT nick from [session_keys] where nick =@nick1", sqlConnection);
            command.Parameters.AddWithValue("@nick1", login);
            try
            {
                sqlReader = command.ExecuteReader();
                if (sqlReader.Read())
                {
                    nick1 += Convert.ToString(sqlReader["nick"]);
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

            if (nick1 != null)
            {
                sqlConnection = new SqlConnection(Connection_String);
                sqlConnection.Open();
                SqlCommand command1 = new SqlCommand("DELETE FROM [session_keys]", sqlConnection);
                command1.ExecuteNonQuery();
                ADDsession_key(sessKey, IV, login, time, id);

            }
            else
            {
                sqlConnection = new SqlConnection(Connection_String);
                sqlConnection.Open();
                SqlCommand command1 = new SqlCommand("DELETE FROM [session_keys]", sqlConnection);
                command1.ExecuteNonQuery();
                ADDsession_key(sessKey, IV, login, time, id);
            }


        }
        public void ADDsession_key(byte[] sesKey, byte[] IV, string nick, DateTime timeDev, int id)
        {
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();

            SqlCommand command = new SqlCommand("INSERT INTO [session_keys] (nick, sesKey, IV, dateDel, sesID)VALUES(@Val1, @val2, @val3, @val4, @val5)", sqlConnection);

            command.Parameters.AddWithValue("@val1", nick);
            command.Parameters.AddWithValue("@val2", sesKey);
            command.Parameters.AddWithValue("@val3", IV);
            command.Parameters.AddWithValue("@val4", timeDev);
            command.Parameters.AddWithValue("@val5", id);
            command.ExecuteNonQuery();
        }
        public void addFriendList(string SenNick, string RecNick, byte[] Key, int type)
        {
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand($"INSERT INTO [MainReqTable] (senNick, recNick, privKey, type)VALUES(@Val1, @val2, @val3, @val4)", sqlConnection);

            command.Parameters.AddWithValue("@val1", SenNick);
            command.Parameters.AddWithValue("@val2", RecNick);
            command.Parameters.AddWithValue("@val3", Key);
            command.Parameters.AddWithValue("@val4", type);
            command.ExecuteNonQuery();
        }
        public void check_friendReq(string nick, string typeN, int type, out byte[] key, out string reply)
        {
            string senNick = null;
            string recNick = null;
            byte[] pubKey = new byte[16];
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand($"SELECT senNick, recNick, privKey from [MainReqTable] where {typeN}=@Nick", sqlConnection);
            command.Parameters.AddWithValue("@Nick", nick);
            try
            {
                sqlReader = command.ExecuteReader();
                while (sqlReader.Read())
                {
                    senNick += Convert.ToString(sqlReader["Sennder"]);
                    recNick += Convert.ToString(sqlReader["Recipient"]);
                    pubKey = (byte[])sqlReader["privKey"];
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

            if (senNick != null)
            {
                key = pubKey;

                reply = recNick;
            }
            else
            {
                key = null;
                reply = null;
            }
        }
        public void check_friendReq1(string snick, string rnick, out byte[] key, out string reply)
        {
            string senNick = null;
            string recNick = null;
            byte[] pubKey = new byte[206];
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand($"SELECT senNick, recNick, privKey from [MainReqTable] where senNick=@snick AND recNick=@rnick", sqlConnection);
            command.Parameters.AddWithValue("@snick", snick);
            command.Parameters.AddWithValue("@rnick", rnick);
            try
            {
                sqlReader = command.ExecuteReader();
                while (sqlReader.Read())
                {
                    senNick += Convert.ToString(sqlReader["senNick"]);
                    recNick += Convert.ToString(sqlReader["recNick"]);
                    pubKey = (byte[])sqlReader["privKey"];
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

            if (senNick != null)
            {
                key = pubKey;

                reply = recNick;
            }
            else
            {
                key = null;
                reply = null;
            }
        }
        public void dellfriendReq(string snick, string rnick)
        {
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlCommand command1 = new SqlCommand("DELETE FROM [MainReqTable] where senNick=@snick AND recNick=@rnick", sqlConnection);
            command1.Parameters.AddWithValue("@snick", snick);
            command1.Parameters.AddWithValue("@rnick", rnick);
            command1.ExecuteNonQuery();
        }
        public void dellfriendReq1(string snick, string rnick)
        {
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlCommand command1 = new SqlCommand("DELETE FROM [req_in_friend] where senNick=@snick AND recNick=@rnick", sqlConnection);
            command1.Parameters.AddWithValue("@snick", snick);
            command1.Parameters.AddWithValue("@rnick", rnick);
            command1.ExecuteNonQuery();
        }
        public void check_sesKey(out string nick, out byte[] sesKey, out byte[] IV, out DateTime time, out int id)
        {
            byte[] sessKey1 = new byte[64];
            byte[] iv1 = new byte[16];
            string nick1 = null;
            int id1 = 0;
            DateTime time1 = DateTime.Now;
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT Id, nick, sesKey, IV, dateDel, sesID FROM [session_keys]", sqlConnection);
            try
            {
                sqlReader = command.ExecuteReader();
                if (sqlReader.Read())
                {
                    nick1 += Convert.ToString(sqlReader["nick"]);
                    sessKey1 = (byte[])sqlReader["sesKey"];
                    iv1 = (byte[])sqlReader["IV"];
                    time1 = (DateTime)sqlReader["dateDel"];
                    id1 = (int)sqlReader["sesID"];
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

            time = time1;
            nick = nick1;
            sesKey = sessKey1;
            IV = iv1;
            id = id1;
        }
        public void addfriend(string mainNick, string friendNick, byte[] friendID)
        {
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            string nicki = "";
            SqlCommand command = new SqlCommand($"SELECT mainNick from [friend] where mainNick=@main AND friendNick=@nick", sqlConnection);
            command.Parameters.AddWithValue("@main", mainNick);
            command.Parameters.AddWithValue("@nick", friendNick);
            try
            {
                sqlReader = command.ExecuteReader();
                while (sqlReader.Read())
                {
                    nicki += Convert.ToString(sqlReader["mainNick"]);
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }



            if (nicki == "")
            {
                SqlCommand command1 = new SqlCommand($"INSERT INTO [friend] (mainNick, friendNick, idfriendKey)VALUES(@Val1, @val2, @val3)", sqlConnection);

                command1.Parameters.AddWithValue("@val1", mainNick);
                command1.Parameters.AddWithValue("@val2", friendNick);
                command1.Parameters.AddWithValue("@val3", friendID);
                command1.ExecuteNonQuery();
            }
            else
            {
                SqlCommand command1 = new SqlCommand("DELETE FROM [friend] where mainNick=@main AND friendNick=@nick", sqlConnection);
                command1.Parameters.AddWithValue("@main", mainNick);
                command1.Parameters.AddWithValue("@nick", friendNick);
                command1.ExecuteNonQuery();


                SqlCommand cmd = new SqlCommand($"INSERT INTO [friend] (mainNick, friendNick, idfriendKey)VALUES(@Val1, @val2, @val3)", sqlConnection);

                cmd.Parameters.AddWithValue("@val1", mainNick);
                cmd.Parameters.AddWithValue("@val2", friendNick);
                cmd.Parameters.AddWithValue("@val3", friendID);
                cmd.ExecuteNonQuery();
            }

        }
        public void friendlist(string sennick, string recnick, byte[] pubKey)
        {
            //Поиск ника отправителя в базе данных аккаунта(отправлял ли аккаунт заявку этому нику)
            check_friendReq1(recnick, sennick, out byte[] mainprivkey, out string ret);
            //если да, то ник добавляется в список друзей и формируется приватный ключ FriendID
            if (mainprivkey != null)
            {
                keysClass seSSionKey = new keysClass();
                //создание ключа
                byte[] friendID = seSSionKey.Creating_SessionKey(pubKey, mainprivkey);
                //добавление в друзья
                addfriend(recnick, sennick, friendID);
                //удаление старой заявки
                dellfriendReq(recnick, sennick);
            }
            // иначе заявка добавляется как новый запрос в друзья
            else
            {
                sqlConnection = new SqlConnection(Connection_String);
                sqlConnection.Open();
                SqlCommand command = new SqlCommand($"INSERT INTO [req_in_friend]" +
                    $" (senNick, recNick, pubKey)VALUES(@Val1, @val2, @val3)",
                    sqlConnection);

                command.Parameters.AddWithValue("@val1", sennick);
                command.Parameters.AddWithValue("@val2", recnick);
                command.Parameters.AddWithValue("@val3", pubKey);
                command.ExecuteNonQuery();
            }

        }
        public void viewreqFriendlist(string nick, out List<string> nicklist)
        {

            List<string> nicklist1 = new List<string>();

            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand($"SELECT senNick FROM [req_in_friend] WHERE recNick =@login", sqlConnection);
            command.Parameters.AddWithValue("@login", nick);
            try
            {
                sqlReader = command.ExecuteReader();
                while (sqlReader.Read())
                {
                    nicklist1.Add(Convert.ToString(sqlReader["senNick"]));
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

            if (nicklist1.Count > 0)
            {
                nicklist = nicklist1;
            }
            else
            {
                nicklist1.Add("Нет запросов в друзья");
                nicklist = nicklist1;
            }


        }
        public void viewFriendlist(string nick, out List<string> nicklist)
        {

            List<string> nicklist1 = new List<string>();

            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand($"SELECT friendNick FROM [friend] WHERE mainNick =@login", sqlConnection);
            command.Parameters.AddWithValue("@login", nick);
            try
            {
                sqlReader = command.ExecuteReader();
                while (sqlReader.Read())
                {
                    nicklist1.Add(Convert.ToString(sqlReader["friendNick"]));
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

            if (nicklist1.Count > 0)
            {
                nicklist = nicklist1;
            }
            else
            {
                nicklist1.Add("У вас нет друзей");
                nicklist = nicklist1;
            }


        }
        public void req_in_friend(string mainNick, string nickf, out byte[] pubkeyf)
        {
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            byte[] pubkey = new byte[140];
            SqlCommand command = new SqlCommand($"SELECT pubKey FROM [req_in_friend] WHERE recNick =@login and senNick =@login1", sqlConnection);
            command.Parameters.AddWithValue("@login", mainNick);
            command.Parameters.AddWithValue("@login1", nickf);
            try
            {
                sqlReader = command.ExecuteReader();
                while (sqlReader.Read())
                {
                    pubkey = (byte[])(sqlReader["pubkey"]);
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

            pubkeyf = pubkey;
        }
        public void dellseskey()
        {
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand("DELETE FROM [session_keys]", sqlConnection);
            command.ExecuteNonQuery();
        }
        public void idfriend(string mainNick, string nickf, out byte[] friendid)
        {
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            byte[] friendid1 = new byte[64];
            SqlCommand command = new SqlCommand($"SELECT idfriendKey from [friend] where mainNick=@main AND friendNick=@nick", sqlConnection);
            command.Parameters.AddWithValue("@main", mainNick);
            command.Parameters.AddWithValue("@nick", nickf);
            try
            {
                sqlReader = command.ExecuteReader();
                while (sqlReader.Read())
                {
                    friendid1 = (byte[])(sqlReader["idfriendKey"]);
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }
            friendid = friendid1;
        }
        public void Mainmessage(string mainnick, string nickf, byte[] message, byte[] iv, int leghtt)
        {
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlCommand cmd = new SqlCommand($"INSERT INTO [Messages] (sennder, mainNick, friend, message, IV, leghtmess)VALUES(@Val1, @val2, @val3, @val4, @val5, @val6)", sqlConnection);

            cmd.Parameters.AddWithValue("@val1", mainnick);
            cmd.Parameters.AddWithValue("@val2", mainnick);
            cmd.Parameters.AddWithValue("@val3", nickf);
            cmd.Parameters.AddWithValue("@val4", message);
            cmd.Parameters.AddWithValue("@val5", iv);
            cmd.Parameters.AddWithValue("@val6", leghtt);
            cmd.ExecuteNonQuery();
        }
        public void Friendmessage(string mainnick, string nickf, byte[] message, byte[] iv, int leghtmess)
        {
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlCommand cmd = new SqlCommand($"INSERT INTO [Messages] (sennder, mainNick, friend, message,IV, leghtmess)VALUES(@Val1, @val2, @val3, @val4, @val5, @val6)", sqlConnection);

            cmd.Parameters.AddWithValue("@val1", nickf);
            cmd.Parameters.AddWithValue("@val2", mainnick);
            cmd.Parameters.AddWithValue("@val3", nickf);
            cmd.Parameters.AddWithValue("@val4", message);
            cmd.Parameters.AddWithValue("@val5", iv);
            cmd.Parameters.AddWithValue("@val6", leghtmess);
            cmd.ExecuteNonQuery();
        }
        public void viewdialog(string mainnick, string nickf, out List<string> messages)
        {
            idfriend(mainnick, nickf, out byte[] idf);
            keysClass seskey = new keysClass();
            sqlConnection = new SqlConnection(Connection_String);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            List<string> messages1 = new List<string>();
            SqlCommand command = new SqlCommand($"SELECT sennder, message, IV, leghtmess from [Messages] where mainNick=@main AND friend=@nick ORDER BY Id", sqlConnection);
            command.Parameters.AddWithValue("@main", mainnick);
            command.Parameters.AddWithValue("@nick", nickf);
            try
            {
                sqlReader = command.ExecuteReader();
                while (sqlReader.Read())
                {

                    string senn = Convert.ToString(sqlReader["sennder"]);
                    byte[] mess = (byte[])(sqlReader["message"]);
                    byte[] iv = (byte[])(sqlReader["IV"]);
                    unbyte_massiv(mess, (int)(sqlReader["leghtmess"]), out mess, out byte[] trash);
                    seskey.DecryptMsg(idf, mess, iv, out byte[] message);
                    string messstr = Encoding.UTF8.GetString(message);
                    messages1.Add($"{senn}: {messstr}");

                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }
            messages = messages1;
        }
        private void unbyte_massiv(byte[] mass, int lenght, out byte[] one, out byte[] two)
        {

            byte[] mass1 = new byte[mass.Length];
            one = new byte[lenght];
            two = new byte[mass.Length - lenght];
            System.Buffer.BlockCopy(mass, 0, one, 0, lenght);
            System.Buffer.BlockCopy(mass, lenght, two, 0, mass.Length - lenght);


        }
    }
}
