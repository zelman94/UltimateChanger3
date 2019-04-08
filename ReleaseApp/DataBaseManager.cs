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
using System.Threading;
using Rekurencjon; // logi
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace UltimateChanger
{
    /// <summary>
    /// Matko bosko... w tej klasie jest taki syf, że o Jezus.
    /// </summary>
    public class DataBaseManager
    {
        public SqlConnection SQLConnection;
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
                    DB_connection = true;
                    setLogs_Begin(); // logowanie wlaczenia UC3                 

                }
                catch (Exception )
                {
                    // DB_connection = false;
                    //MessageBox.Show("no acess to DB");
                }

            });
            TaskConnectToDB.Wait();
        }

        private SqlConnection ConnectToDB(string switch_)
        {
            try
            {
                //Create a connection calling the App.config
                string conn = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
                //The query to use
                SqlConnection connection = new SqlConnection(conn);
                //Create a Data Adapter
                //SqlDataAdapter dadapter = new SqlDataAdapter(query, connection);
                //Create the dataset
                DataSet ds = new DataSet();
                //Open the connection
                connection.Open();
                //Fill the DatSet with the adapter information                  dadapter.Fill(ds, "employees");
                connection.Close();

                return connection;
            }

            catch (Exception e)
            {
                Console.WriteLine("Wystąpił nieoczekiwany błąd!");
                logging.AddLog(e.ToString());
                //System.Windows.MessageBox.Show(e.ToString() + " switch: " + switch_);
                return null;
            }
        }

        public string getInfo_AboutBuild(string ver)
        {
            try
            {
                string info = "";
                SQLConnection.Open();
                SqlCommand command = new SqlCommand($"Select Changelog from Info where Version='{ver}'", SQLConnection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    info = reader.GetString(0);
                }
                SQLConnection.Close();
                return info;
            }
            catch (Exception x)
            {              
                System.Windows.MessageBox.Show(x.ToString());
                return "";
            }
        }
        public List<string> Advance_GetPath(string root) //AdvanceBuild podajesz root patha i sprawdzasz czy jest juz w bazie jezeli jest to pobierasz pathy jako lista stringow
        {
            List<string> listOfPaths = new List<string>();

            try
            {
                SQLConnection.Open();
                SqlCommand command = new SqlCommand($"Select Paths From AdvanceBuild Where Root_ LIKE '{root}'", SQLConnection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listOfPaths.Add(reader.GetString(0));
                    }
                   
                }
                SQLConnection.Close();
                return listOfPaths;
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
                return null;
            }
        }
        public void Advance_AddPath(string root, List<string> paths)
        {
            try
            {
                SQLConnection.Open();

                foreach (var item in paths)
                {
                    SqlCommand command = new SqlCommand($"Insert INTO AdvanceBuild values ('{root}','{item}')", SQLConnection);
                    command.ExecuteNonQuery();
                }

                SQLConnection.Close();
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }
        }

        public void setLogs_Begin()
        {
            //try
            //{
            //    string data = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "/" + DateTime.Now.ToString("h:mm:ss tt");
            //    MySqlCommand myCommand = new MySqlCommand($"INSERT INTO Logs VALUES ('{Environment.UserName}','{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()}','{data}')", SQLConnection);
            //    myCommand.ExecuteNonQuery();
            //}
            //catch (Exception x)
            //{
            //    Console.WriteLine(x.ToString());
            //}
        }
    }
}
