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
using System.Threading;
using Rekurencjon; // logi

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
        public Task TaskConnectToDB = null;
        Log logging = new Log("DataBaseManager");
        public bool getConnectionstatus() // true - skonczyl sie watek / false - watek trwa
        {
            if (TaskConnectToDB.IsCompleted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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

            TaskConnectToDB = Task.Run(() => {
                SQLConnection = ConnectToDB(switch_);
                try
                {
                    if (SQLConnection == null)
                    {
                        SQLConnection.Open();
                    }
                   
                    // SQLConnection.Close();
                    DB_connection = true;

                    setLogs_Begin(); // logowanie wlaczenia UC3
                  

                }
                catch (Exception x)
                {
                    // DB_connection = false;
                    //MessageBox.Show("no acess to DB");

                }
            });
            //t.Wait();


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
                logging.AddLog(e.ToString() + " switch: " + switch_);
                //System.Windows.MessageBox.Show(e.ToString() + " switch: " + switch_);
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

                    Kolumna.Add(myReader.GetString(5)); // copyRecu
                    Kolumna.Add(myReader.GetString(6)); // pcopySettings
                    Kolumna.Add(myReader.GetString(7)); // copyUpdater
                    Kolumna.Add(myReader.GetString(8)); // copyResources
                    Kolumna.Add(myReader.GetString(9)); // copyImages

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

                    if ((APPversion == "3.1.0.0" || APPversion == "3.1.1.0") && System.IO.File.Exists(@"C:\Program Files\UltimateChanger\tmp.txt")) // w pozniejszej wersji do wywalenia
                    {                       
                            System.IO.File.Delete(@"C:\Program Files\UltimateChanger\tmp.txt");                    
                       
                        try
                        {
                            Process.Start(@"C:\Program Files\UltimateChanger\UpdaterCopy.exe", @"\\10.128.3.1\DFS_data_SSC_FS_Images-SSC\PAZE\change_market\Multi_Changer\v_3.1.1\portable" + " false");
                        }
                        catch (Exception)
                        {
                            try
                            {
                                Process.Start(@"C:\Program Files\UltimateChanger\UpdaterCopy.exe", @"\\demant.com\data\KBN\RnD\FS_Programs\Support_Tools\Ultimate_changer\v_3.1.1\portable" + " false");
                            }
                            catch (Exception)
                            {

                                
                            }
                        }
                      
                    }

                    if (APPversion == "3.0.2.0"  && FileOperator.getCountUCRun() == "0") // dodać "i pierwszy start aplikacji"
                    {


                        try // usuwan stary
                        {
                            System.IO.File.Delete(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Ultimate Changer.lnk"));
                        }
                        catch (Exception)
                        {

                        }

                        string shortcutLocation = System.IO.Path.Combine(@"C:\Program Files\UltimateChanger\", "shortcut Ultimate Changer" + ".lnk");
                        IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                        IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutLocation);

                        shortcut.Description = "Ultimate Changer";   // The description of the shortcut
                        shortcut.IconLocation = @"C:\Program Files\UltimateChanger\icon.ico";           // The icon of the shortcut
                        shortcut.TargetPath = @"C:\Program Files\UltimateChanger\Ultimate Changer.exe";   // The path of the file that will launch when the shortcut is run
                        shortcut.WorkingDirectory = @"C:\Program Files\UltimateChanger";
                        shortcut.Save();                                    // Save the shortcut

                        System.IO.File.Move(shortcutLocation, System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Ultimate Changer.lnk"));


                    }


                    bool message = false;
                    for (int i = 0; i < 3; i++)
                    {
                        if (ver_apki[i] < ver[i] && message == false)
                        {
                            //System.Windows.Forms.MessageBox.Show($"Update available: {Kolumna[1]}");

                            Window Update = new UpdateWindow(Kolumna[1], Kolumna[3], Kolumna[5], Kolumna[6], Kolumna[7], Kolumna[8], Kolumna[9]);
                            Update.ShowDialog();


                            pathsToUpdate = Kolumna[1];

                            return true;

                           /// message = true; /*HATORI NARAZIE PODZIEKUJEMY*/
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

            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
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

        public string logIn(string name, string pass) // logowanie jako admin 
        {
            string power = "USER";
            try
            {
                MySqlCommand myCommand = new MySqlCommand($"SELECT power FROM UltimateUsers WHERE name = {name} AND pass ={pass} ", SQLConnection);
                MySqlDataReader myReader;
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    power = myReader.GetString(0);
                }
                myReader.Close();
            }
   

            catch (Exception x)
            {
                logging.AddLog(x.ToString());
                //System.Windows.MessageBox.Show(x.ToString());
                return "";
            }


            return power;
        }

        public void CreateNew(string name, string pass)
        {
            MySqlDataReader myReader = null;
            try
            {
                MySqlCommand myCommand = new MySqlCommand($"INSERT INTO `UltimateUsers` (`name`, `pass`, `power`) VALUES ('{name}', '{pass}', '0')", SQLConnection);
                
                myReader = myCommand.ExecuteReader();   
                
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }
            myReader.Close();
        }

        public List<string> GetAllAvailableVerifit()
        {
            List<string> listVerifit = new List<string>();
            MySqlDataReader myReader = null;
            try
            {
                MySqlCommand myCommand = new MySqlCommand($"SELECT `name_verifit` FROM `VeriFit` WHERE `status` = '0'", SQLConnection);
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    listVerifit.Add(myReader.GetString(0));
                }
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }
            myReader.Close();
            return listVerifit;
        }

        public List<string> GetMyVerifit(string user)
        {
            List<string> listVerifit = new List<string>();
            MySqlDataReader myReader = null;
            try
            {
                MySqlCommand myCommand = new MySqlCommand($"SELECT `name_verifit` FROM `VeriFit` WHERE `user` = '{user}'", SQLConnection);
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    listVerifit.Add(myReader.GetString(0));
                }
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show("Cannot connect to Database user: " + user);
            }
            myReader.Close();
            return listVerifit;
        }

        public List<string> FindVerifits()
        {
            List<string> listVerifit = new List<string>();
            MySqlDataReader myReader = null;
            try
            {
                MySqlCommand myCommand = new MySqlCommand($"SELECT `name_verifit`,`user` FROM `VeriFit` WHERE `status` = '1'", SQLConnection);
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    listVerifit.Add(myReader.GetString(0)+ " " + myReader.GetString(1));
                }
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }
            myReader.Close();
            return listVerifit;
        }

        public bool setUserForDevice(string device, string user)
        {
            List<string> listVerifit = GetAllAvailableVerifit();
            MySqlDataReader myReader = null;
            foreach (var item in listVerifit)
            {
                if (item == device)
                {

                    try
                    {
                        MySqlCommand myCommand = new MySqlCommand($"UPDATE `VeriFit` SET `user`='{user}',`status`='1' WHERE `name_verifit` = '{device}'", SQLConnection);
                        myReader = myCommand.ExecuteReader();
                        myReader.Close();
                        return true;
                    }
                    catch (Exception)
                    {
                        myReader.Close();
                        return false;
                    }
                }
            }
            myReader.Close();
            return false;
        }

        public bool returnVerifit(string device)
        {
            MySqlDataReader myReader = null;
            try
            {
                MySqlCommand myCommand = new MySqlCommand($"UPDATE `VeriFit` SET `user`='',`status`='0' WHERE `name_verifit` = '{device}'", SQLConnection);
                myReader = myCommand.ExecuteReader();
                myReader.Close();
                return true;
            }
            catch (Exception)
            {
                myReader.Close();
                return false;
            }
        }


        public void setLogs_Begin()
        {
            try
            {
                string data = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "/" + DateTime.Now.ToString("h:mm:ss tt");
                MySqlCommand myCommand = new MySqlCommand($"INSERT INTO Logs VALUES ('{Environment.UserName}','{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()}','{data}')", SQLConnection);
                myCommand.ExecuteNonQuery();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }
        }


        public void setLogs(ClickCounter CounterOfclicks,string skin_name)
        { // dodac implementacje logowania do bazy SQL
            try
            {
                //user	ver	time	StartFittingSoftware	Start_Hattori	InstallFittingSoftware	Update_Market	UpdateMode	DeleteLogs	UninstallFittingSoftware	Kill_FS	Downgrade	RandomHI	CopyMyHardware
                //StartFittingSoftware,
                //StartHAttori,
                //InstallFittingSoftware,
                //UpdateMarket,
                //UpdateMode,
                //DeleteLogs,
                //UninstallFittingSoftware,
                //Kill,
                //Downgrade,
                //RandomHI,
                //CopyMyHardware


                string data = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "/" + DateTime.Now.ToString("h:mm:ss tt");

                MySqlCommand myCommand = new MySqlCommand($"INSERT INTO Advance_logs VALUES ('{Environment.UserName}','{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()}','{data}','{skin_name}',{CounterOfclicks.Clicks[(int)Buttons.StartFittingSoftware]},{CounterOfclicks.Clicks[(int)Buttons.StartHAttori]},{CounterOfclicks.Clicks[(int)Buttons.InstallFittingSoftware]},{CounterOfclicks.Clicks[(int)Buttons.UpdateMarket]},{CounterOfclicks.Clicks[(int)Buttons.UpdateMode]},{CounterOfclicks.Clicks[(int)Buttons.DeleteLogs]},{CounterOfclicks.Clicks[(int)Buttons.UninstallFittingSoftware]},{CounterOfclicks.Clicks[(int)Buttons.Kill]},{CounterOfclicks.Clicks[(int)Buttons.Downgrade]},{CounterOfclicks.Clicks[(int)Buttons.RandomHI]},{CounterOfclicks.Clicks[(int)Buttons.CopyMyHardware]})", SQLConnection);
                myCommand.ExecuteNonQuery();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }
        }

    }
}
