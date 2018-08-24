﻿using MySql.Data.MySqlClient;
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
        private ClickCounter clickCounter;
        public string pathsToUpdate="";
        public bool DB_connection ; //jezeli jest polaczenie z BD 
        private Stopwatch time;
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
                    return "";
                    SQLConnection.Close();
                }               

                SQLConnection.Close();
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
                    SQLConnection.Close();                    
                }
              

                SQLConnection.Close();
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
                        SQLConnection.Close();
                    }
                    catch (Exception ee2)
                    {
                       
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

        public DataBaseManager()
        {

            SQLConnection = ConnectToDB();
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

        private MySqlConnection ConnectToDB()
        {
            try
            {
                //string tmp = "server=zadanko-z-zutu.cba.pl;" +
                //                    "database=zelman;" +
                //                   "uid=zelman;" +
                //                   "password=Santiego94;";



                string tmp = "server=10.128.64.19;" +
                                    "database=zelman;" +
                                   "uid=changer;" +
                                   "password=changer;";

                MySqlConnection sqlConn = new MySqlConnection(tmp);
                sqlConn.Open();
                //sqlConn.Close();
                return sqlConn;
            }

            catch (Exception e)
            {
                Console.WriteLine("Wystąpił nieoczekiwany błąd!");
                Console.WriteLine(e.Message);
                return null;
            }
        }

       
        public void AddKnowlage(string deffinition, string param1, string param2="",string param3="")
        {
            try
            {
                using (MySqlCommand myCommand = new MySqlCommand($"INSERT INTO `glossary` VALUES('{param1}','{param2}','{param3}','{deffinition}')", SQLConnection))
                {
                    myCommand.ExecuteReader();

                }

            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }

        }

        public string FindKnowlage(string param1, string param2, string param3)
        {
            try
            {
                string zapytanie;

                if (param2 == "")
                {
                    zapytanie = $"SELECT `deffinition` FROM `glossary` WHERE `parameter1` = '{param1}'";
                }
                else if (param3 == "")
                {
                    zapytanie = $"SELECT `deffinition` FROM `glossary` WHERE `parameter1` = '{param1}' AND  `parameter2` = '{param2}' AND  `parameter3` = '{param3}'"; // wszyskie parametry 
                }
                else
                {
                    zapytanie = $"SELECT `deffinition` FROM `glossary` WHERE `parameter1` = '{param1}' AND  `parameter2` = '{param2}'"; //par 1 i 2
                }

                using (MySqlCommand myCommand = new MySqlCommand(zapytanie, SQLConnection))
                {
                    MySqlDataReader myReader = myCommand.ExecuteReader();
                    List<string> odpowiedzi = new List<string>();
                    while (myReader.Read())
                    {
                        odpowiedzi.Add(myReader[0].ToString());
                    }

                    myReader.Close();


                    return string.Join("\n", odpowiedzi.ToArray());
                }

            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }

            return "";
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

                            Window Update = new UpdateWindow(Kolumna[1], Kolumna[3]);
                            Update.ShowDialog();


                            pathsToUpdate = Kolumna[1];
                                message = true;
                            }
                        }
                        if (message)
                        {
                           
                            return true;
                        }
                        else
                        {
                           
                            return false;
                        }

                }
                else
                {
                    return false;
                }
                }

                catch (Exception ee)
                {

                   
                    SQLConnection.Close();
                    return false;
                }
                SQLConnection.Close();
 
        }

    }
}
