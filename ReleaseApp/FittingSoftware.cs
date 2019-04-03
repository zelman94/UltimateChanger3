using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace UltimateChanger
{
    public class FittingSoftware
    {
        public string Name_FS;
        public string Path_Local_Installer;
        public string Version;
        public string Market;
        public string Hattori_Path;
        public bool customPath; // jezeli customowa lokalizacja FS
        public int indexFS;
        public string Brand;
        public List<string> PathTrash = new List<string>();
        public string PathToLogMode,pathToLogs;
        public string OEM;
        public string SelectedLanguage;
        public string LogMode;
        DispatcherTimer Timer_InfoFS; // timer do sprawdzania info o buildach
        List<string> ListPathsToAboutInfo = new List<string>();
        protected List<string> ListpathsToManInfo = new List<string>();
        public string pathToExe, pathToManu;
        protected FileOperator fileOperator = new FileOperator();
        public bool composition = false;
        public bool uninstallation = false;

        public FittingSoftware()
        {
        }

        public FittingSoftware(FittingSoftware tmpFS)
        {
            Name_FS = tmpFS.Name_FS;
            Path_Local_Installer = tmpFS.Path_Local_Installer;
            Version = tmpFS.Version;
            Market = tmpFS.Market;
            customPath = false;
            Brand = tmpFS.Brand;
            PathTrash = tmpFS.PathTrash;
            PathToLogMode = tmpFS.PathToLogMode;
            pathToLogs = tmpFS.pathToLogs;
            OEM = tmpFS.OEM;
            SelectedLanguage = tmpFS.SelectedLanguage;
            LogMode = tmpFS.LogMode;
            pathToExe = tmpFS.pathToExe;
            pathToManu = tmpFS.pathToManu;
        }

        private void CreateFullFsw()
        {
            PathTrash = myXMLReader.getPaths("pathToTrash", Brand);
            PathToLogMode = myXMLReader.getPaths("pathToLogMode", Brand)[0];
            pathToLogs = myXMLReader.getPaths("pathToLogs", Brand)[0];

            pathToExe = pathToExe = BuildInfo.ListPathsToSetup[indexFS];

            ListpathsToManInfo = BuildInfo.ListPathsToManInfo;
            ListPathsToAboutInfo = BuildInfo.ListPathsToAboutInfo;

            pathToManu = ListpathsToManInfo[indexFS];
            BuildInfo infoAboutFS = fileOperator.GetInfoAboutFs(ListpathsToManInfo[indexFS], ListPathsToAboutInfo[indexFS]);
            //Brand = infoAboutFS.Brand;
            Market = infoAboutFS.MarketName;
            OEM = infoAboutFS.OEM;
            SelectedLanguage = infoAboutFS.SelectedLanguage;
            Version = infoAboutFS.Version;
            LogMode = fileOperator.getLogMode(indexFS, PathToLogMode)[0];
            Timer_InfoFS = new DispatcherTimer();
            Timer_InfoFS.Tick += updateInfoFS;
            Timer_InfoFS.Interval = new TimeSpan(0, 0, 10);
            Timer_InfoFS.Start();
        }

        public FittingSoftware(string name, string brand, int index)
        {
            Name_FS = name;
            Brand = brand;
            indexFS = index;
            Path_Local_Installer = findUnInstaller();
            Version = getFS_Version();
           // Market = getMarket();
            customPath = false;
            CreateFullFsw();
        }

        public void updateInfoFS(object sender, EventArgs e)
        {
            if (uninstallation)
            {
                Market = "Uninstallation";
                return;
            }

            BuildInfo infoAboutFS = fileOperator.GetInfoAboutFs(ListpathsToManInfo[indexFS], ListPathsToAboutInfo[indexFS]);
            Market = infoAboutFS.MarketName;
            OEM = infoAboutFS.OEM;
            SelectedLanguage = infoAboutFS.SelectedLanguage;
            Version = infoAboutFS.Version;
            LogMode = fileOperator.getLogMode(indexFS, PathToLogMode)[0];
        }

            public string findUnInstaller()
            {
            var allFiles = Directory.GetFiles(@"C:\ProgramData\Package Cache", "*.exe", SearchOption.AllDirectories);
            foreach (var item in allFiles)
            {
                try
                {
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(item);

                    if ((myFileVersionInfo.FileName.Contains("OticonMedium") || fileOperator.checkIfGenie(myFileVersionInfo.FileDescription)) && Name_FS.Contains("2"))
                    {
                        return item;
                    }

                    if ((myFileVersionInfo.FileName.Contains("OticonMedicalMedium") || fileOperator.checkIfMedical(myFileVersionInfo.FileDescription)) && Name_FS.Contains("Medical"))
                    {
                        return item;
                    }

                    if ((myFileVersionInfo.FileName.Contains("BernafonMedium") || fileOperator.checkIfOasis(myFileVersionInfo.FileDescription)) && Name_FS.Contains("Oasis"))
                    {
                        return item;
                    }

                    if ((myFileVersionInfo.FileName.Contains("SonicMedium") || fileOperator.checkIfSonic(myFileVersionInfo.FileDescription)) && Name_FS.Contains("Express"))
                    {
                        return item;
                    }

                    if ((myFileVersionInfo.FileName.Contains("PhilipsMedium") || fileOperator.checkIfPhilips(myFileVersionInfo.FileDescription)) && Name_FS.Contains("HearSuite"))
                    {
                        return item;
                    }
                }
                catch (Exception x)
                {
                }
            }
            return "";
        }

        public void uninstallFS()
        {

        }

        public virtual string getMarket()
        {
            return fileOperator.getMarket(indexFS, !composition);
        }

        public bool setMarket(string Market)
        {
            if (fileOperator.setMarket(indexFS, Market, !composition))
            {
                this.Market = Market;
                return true;
            } else
                return false;
        }
        public string getLogMode()
        {
           return fileOperator.getLogMode(indexFS, PathToLogMode)[0];
        }

        public void setLogMode(string LogMode,int indexSetting,bool Full)
        {
            fileOperator.setLogMode(PathToLogMode, LogMode, indexSetting,Convert.ToByte(indexFS),Full, false, "", "", "");
        }

        public void deleteTrash()
        {
            TrashCleaner smieciarka = new TrashCleaner();
            foreach (var item in PathTrash)
            {
                if (Version=="")
                {
                    smieciarka.DeleteTrash(item);
                }
                else
                {
                    MessageBox.Show("Please Uninstall FS");
                    return;
                }
                
            }    
        }

        public void deleteLogs()
        {
            List<string> pathToLogsFiles = new List<string>();

            pathToLogsFiles = Directory.GetFiles(pathToLogs).ToList();
            string problems = "";
            foreach (var item in pathToLogsFiles)
            {
                try
                {
                    File.Delete(item);
                }
                catch (Exception x)
                {
                    problems += item + "\n";
                }
            }
            if (problems != "")
            {
                MessageBox.Show("Problems with files: \n" + problems);
            }

        }
        public void setUninstallation(bool status)
        {
            uninstallation = status;
        }

        public virtual string getFS_Version()
        {
            try
            {
                return fileOperator.GetInfoAboutFs(ListpathsToManInfo[indexFS], ListPathsToAboutInfo[indexFS]).Version;
            }
            catch (Exception)
            {
                return "";
            }
        }

    }
}
