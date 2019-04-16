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

        public void pushLogs()
        {

            string data = DateTime.Now.Year.ToString();
            if (DateTime.Now.Month < 10)
            {
                data += $"0{DateTime.Now.Month.ToString()}";
            }
            else
            {
                data += $"{DateTime.Now.Month.ToString()}";
            }

            if (DateTime.Now.Day < 10)
            {
                data += $"0{DateTime.Now.Day.ToString()}";
            }
            else
            {
                data += $"{DateTime.Now.Day.ToString()}";
            }

            data +=" "+ DateTime.Now.ToString("h:mm:ss tt");

            try
            {
                SQLConnection.Open();
                SqlCommand myCommand = new SqlCommand($"INSERT INTO usage VALUES ('{Environment.UserName}','{data}','{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()}')", SQLConnection);
                myCommand.ExecuteNonQuery();
                SQLConnection.Close();
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }
            
        }

        public void SendFeedBack(string feedback)
        {
            try
            {
                SQLConnection.Open();
                SqlCommand command = new SqlCommand($"Insert INTO feedback values ('{Environment.UserName}','{feedback}')", SQLConnection); // dodac tabele do bazy danych
                command.ExecuteNonQuery();
                SQLConnection.Close();
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }
        }

        public string getModelHI(string model)
        {
            string model_name = "error";

            try
            {
                SQLConnection.Open();
                SqlCommand command = new SqlCommand($"Select value From model_HI Where key_value = '{model}'", SQLConnection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model_name = reader.GetString(0);
                        model_name = System.Text.RegularExpressions.Regex.Replace(model_name,"  ","");
                    }
                }
                SQLConnection.Close();
                return model_name;
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
                return model_name;
            }
        }

        public List<string> getBuilds(string TYPE, string RELEASE, string MODE, string BRAND, string OEM)
        {
            List<string> BuildsList = new List<string>();
            try
            {
                SQLConnection.Open();
                if (MODE == "IP")
                {

                }
                SqlCommand command = new SqlCommand($"select path from builds where type = '{TYPE}' AND release = '{RELEASE}' AND mode LIKE '%{MODE}%' AND brand = '{BRAND}' AND oem = '{OEM}' order by about desc", SQLConnection);
                logging.AddLog("getBuilds:  TYPE,  RELEASE,  MODE,  BRAND,  OEM \n" + TYPE +" "+ RELEASE + " " + MODE + " " + BRAND + " " + OEM);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BuildsList.Add(reader.GetString(0));
                    }
                }
                SQLConnection.Close();
                return BuildsList;
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
                return BuildsList;
            }
        }
    }
}
