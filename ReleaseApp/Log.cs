using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rekurencjon
{
   public class Log
    {
        public Log(string name)
        {
            CreateNewLog(name);

        }

        public FileStream LogFile;
        public string pathToFile;
        public void CreateNewLog(string name)
        {
            if (!Directory.Exists("C:\\Program Files\\UltimateChanger\\Logs"))
            {
                Directory.CreateDirectory("C:\\Program Files\\UltimateChanger\\Logs");
            }
            deleteOldLogs();
            pathToFile = $"C:\\Program Files\\UltimateChanger\\Logs\\Log_{name}_{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")}.txt";
            LogFile = File.Create(pathToFile);
            LogFile.Close();


        }

        public void AddLog(string log)
        {
            File.AppendAllText(pathToFile, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\t" + log + Environment.NewLine);
        }


        public void deleteOldLogs()
        {
            if (Directory.GetFiles("C:\\Program Files\\UltimateChanger\\Logs").Length > 10)
            {
                foreach (var item in Directory.GetFiles("C:\\Program Files\\UltimateChanger\\Logs"))
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception)
                    {

                    }

                }
            }
        }


    }
}
