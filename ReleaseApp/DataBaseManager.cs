using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows;

namespace UltimateChanger
{
    /// <summary>
    /// Matko bosko... w tej klasie jest taki syf, że o Jezus.
    /// </summary>
    public class DataBaseManager
    {
        public MySqlConnection SQLConnection;
        //private ClickCounter clickCounter;
        public string pathsToUpdate = "";
        public bool DB_connection; //jezeli jest polaczenie z BD 
        //private Stopwatch time;
        public string APPversion;

        public string GetActualVersion(string brand)
        {
            string IPVersion = "";
            string PreBrand = "";
            if (DB_connection)
            {
                switch (brand)
                {
                    case "Oticon":
                        IPVersion = "GenieIP";
                        PreBrand = "Genie_";
                        break;
                    case "Bernafon":
                        IPVersion = "Oasis_IP";
                        PreBrand = "Oasis_";
                        break;
                    case "Sonic":
                        IPVersion = "EF_IP";
                        PreBrand = "ExpressFit_";
                        break;
                }

                SQLConnection.Open();
                MySqlDataReader myReader;
                try
                {
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT {IPVersion} FROM BD_FOR_MultiChanger_DGS WHERE actual = 1", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        if (myReader.Read())
                        {
                            PreBrand = $"{PreBrand}{myReader[IPVersion].ToString()}";
                        }
                        myReader.Close();
                    }
                }
                catch (Exception)
                {
                    //SQLConnection.Close();
                    return "";
                }

               // SQLConnection.Close();
            }
            return PreBrand;
        }


        public string GetIPVersion(string brand, string about)
        {
            string IPVersion = "";
            string PreBrand = "";
            if (DB_connection)
            {
                switch (brand)
                {
                    case "Genie":
                        IPVersion = "GenieIP";
                        PreBrand = "Genie_";
                        brand = "Oticon";
                        break;
                    case "Oasis":
                        IPVersion = "Oasis_IP";
                        PreBrand = "Oasis_";
                        brand = "Bernafon";
                        break;
                    case "ExpressFit":
                        IPVersion = "EF_IP";
                        PreBrand = "ExpressFit_";
                        brand = "Sonic";
                        break;
                }

                MySqlDataReader myReader;
                try
                {
                    SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT {IPVersion} FROM BD_FOR_MultiChanger_DGS WHERE About_{brand} ={about}", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        if (myReader.Read())
                        {
                            PreBrand = $"{PreBrand}{myReader[IPVersion].ToString()}";
                        }
                        myReader.Close();
                    }
                }
                catch (Exception)
                {
                   // SQLConnection.Close();
                }


               // SQLConnection.Close();
            }
            return PreBrand;
        }



        public string GetDirectoryName(string about, string brand)
        {
            string IPVersion = "";
            string directory = "Sorry, no info :<";
            string AboutVersion = "";
            if (DB_connection)
            {
                switch (brand)
                {
                    case "Genie":
                        IPVersion = "GenieIP";
                        AboutVersion = "About_Oticon";
                        break;
                    case "Oasis":
                        IPVersion = "Oasis_IP";
                        AboutVersion = "About_Bernafon";
                        break;
                    case "EF":
                        IPVersion = "EF_IP";
                        AboutVersion = "About_Sonic";
                        break;
                }
                try
                {
                    SQLConnection.Open();
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.Message);
                }
                MySqlDataReader myReader;
                using (MySqlCommand myCommand = new MySqlCommand($"SELECT {IPVersion} FROM BD_FOR_MultiChanger_DGS WHERE {AboutVersion} = {about}", SQLConnection))
                {
                    try
                    {
                        myReader = myCommand.ExecuteReader();
                        if (myReader.Read())
                        {
                            directory = myReader[IPVersion].ToString();
                        }
                        myReader.Close();
                        //SQLConnection.Close();
                    }
                    catch (Exception ee2)
                    {
                        Console.WriteLine(ee2.Message);
                    }

                }
            }
            return directory;
        }

        private string FormatElementOfDate(string dateElement)
        {
            if (int.Parse(dateElement) < 10)
            {
                return $"0{dateElement}";
            }
            else
            {
                return dateElement;
            }
        }

        private string GetFormattedTime()
        {
            DateTime now = DateTime.Now.ToLocalTime();
            return $"{FormatElementOfDate(now.Day.ToString())}.{FormatElementOfDate(now.Month.ToString())}.{now.Year} {FormatElementOfDate(now.Hour.ToString())}:{FormatElementOfDate(now.Minute.ToString())}:{FormatElementOfDate(now.Second.ToString())}";
        }

        public DataBaseManager(string switch_)
        {

            SQLConnection = ConnectToDB(switch_);
            try
            {
                SQLConnection.Open();
                // SQLConnection.Close();
                DB_connection = true;



            }
            catch (Exception)
            {
                // DB_connection = false;
                //MessageBox.Show("no acess to DB");

            }



        }

        private MySqlConnection ConnectToDB(string switch_)
        {
            try
            {
                string tmp = "";
                // odczyt z XML DataBase ConnectionString :0/1 0 to zewnętrzny server a 1 to malina
                switch (switch_)
                {
                    case ("0"):
                        tmp = "server=zadanko-z-zutu.cba.pl;" +
                                    "database=zelman;" +
                                   "uid=zelman;" +
                                   "password=Santiego94;SslMode=none;";
                        break;
                    case ("1"):
                        tmp = "server=10.128.64.19;" +
                                            "database=zelman;" +
                                           "uid=changer;" +
                                           "password=changer;";

                        break;
                    default:

                        break;
                }
                
                

                MySqlConnection sqlConn = new MySqlConnection(tmp);
                sqlConn.Open();
                //sqlConn.Close();
                return sqlConn;
            }

            catch (Exception e)
            {
                Console.WriteLine("Wystąpił nieoczekiwany błąd!");
                System.Windows.MessageBox.Show(e.Message + " switch: " + switch_);
                return null;
            }
        }




        public bool getInformation_DB()
        {

            List<string> Kolumna = new List<string>();
            try
            {
                if (SQLConnection != null)
                {
                    MySqlCommand myCommand = new MySqlCommand("SELECT * FROM information", SQLConnection);
                    MySqlDataReader myReader;
                    myReader = myCommand.ExecuteReader();
                    myReader.Read();
                    string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    Kolumna.Add(myReader.GetString(0)); //update 1 = true
                    Kolumna.Add(myReader.GetString(1)); // path update string
                    Kolumna.Add(myReader.GetString(2)); // info 1 = true
                    Kolumna.Add(myReader.GetString(3)); // information string 
                    Kolumna.Add(myReader.GetString(4)); // information version update
                    myReader.Close();
                    string tmp = Kolumna[4];
                    //-----------------------------------
                    int[] ver = new int[3]; // wersja z srvera
                    int.TryParse(tmp[0].ToString(), out ver[0]);
                    int.TryParse(tmp[2].ToString(), out ver[1]);
                    int.TryParse(tmp[4].ToString(), out ver[2]);
                    //-----------------------------------------
                    //wersja apki
                    int[] ver_apki = new int[3];

                    int.TryParse(version[0].ToString(), out ver_apki[0]);
                    int.TryParse(version[2].ToString(), out ver_apki[1]);
                    int.TryParse(version[4].ToString(), out ver_apki[2]);
                    APPversion = version.ToString();
                    bool message = false;
                    for (int i = 0; i < 3; i++)
                    {
                        if (ver_apki[i] < ver[i] && message == false)
                        {
                            //MessageBox.Show($"Update available: {Kolumna[1]}");

                            /*Window Update = new UpdateWindow(Kolumna[1], Kolumna[3]);
                            Update.ShowDialog();


                            pathsToUpdate = Kolumna[1];
                            message = true; HATORI NARAZIE PODZIEKUJEMY */
                        }
                    }
                    if (message)
                        return true;
                    else
                        return false;
                }
                else
                {
                    //SQLConnection.Close();
                    return false;
                }
            }

            catch (Exception)
            {
                //SQLConnection.Close();
                return false;
            }
        }


        public List<string> getHI(bool t_coil,bool led, bool twobuttons, bool wireless,bool custom, bool s,bool magnego, string release)
        {
            List<string> HIs = new List<string>();
            try
            {
                MySqlCommand myCommand = new MySqlCommand($"SELECT Name FROM HIS WHERE t_coil = {t_coil} AND led = {led} AND twobuttons = {twobuttons} AND wireless = {wireless} AND custom = {custom} AND s = {s} AND magneto = {magnego} AND releasee = {release} ", SQLConnection);
                MySqlDataReader myReader;
                myReader = myCommand.ExecuteReader();
            
                while (myReader.Read())
                {
                    HIs.Add(myReader.GetString(0));
                }
                myReader.Close();

               
            }
            catch (Exception)
            {
                //SQLConnection.Close();
                return null;
            }

           // SQLConnection.Close();


            return HIs;
        }

        public List<string> getComDevice(bool wireless)
        {
            List<string> listaComDev = new List<string>();
            try
            {
                MySqlCommand myCommand = new MySqlCommand($"SELECT name FROM ComDev WHERE wireless = {wireless} ", SQLConnection);
                MySqlDataReader myReader;
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    listaComDev.Add(myReader.GetString(0));
                    //HIs.Add(myReader.GetString(0));
                }
                myReader.Close();
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }

            return listaComDev;
        }



    }
}
