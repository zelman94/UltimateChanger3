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
        List<string> ListpathsToManInfo = new List<string>();
        public string pathToExe, pathToManu;
        FileOperator fileOperator = new FileOperator();
        public bool composition;


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
        public FittingSoftware(string Name,bool composition = false)
        {
            Name_FS = Name;
            Path_Local_Installer = findUnInstaller();
            Version = getFS_Version();
           // Market = getMarket();
            customPath = false;
            switch (Name)
            {
                case ("Genie 2"):
                    indexFS = 0;
                    Brand = "Oticon";
                    PathTrash = myXMLReader.getPaths("pathToTrash", Brand);
                    PathToLogMode = myXMLReader.getPaths("pathToLogMode", Brand)[0];
                    pathToLogs = myXMLReader.getPaths("pathToLogs", Brand)[0];
                    this.composition = composition;
                    break;
                case ("Medical"):
                    indexFS = 1;
                    Brand = "Medical";
                    PathTrash = myXMLReader.getPaths("pathToTrash", Brand);
                    PathToLogMode = myXMLReader.getPaths("pathToLogMode", Brand)[0];
                    pathToLogs = myXMLReader.getPaths("pathToLogs", Brand)[0];
                    this.composition = composition;
                    break;
                case ("Express"):
                    indexFS = 2;
                    Brand = "Sonic";
                    PathTrash = myXMLReader.getPaths("pathToTrash", Brand);
                    PathToLogMode = myXMLReader.getPaths("pathToLogMode", Brand)[0];
                    pathToLogs = myXMLReader.getPaths("pathToLogs", Brand)[0];
                    this.composition = composition;
                    break;
                case ("HearSuite"):
                    indexFS = 3;
                    Brand = "Philips";
                    PathTrash = myXMLReader.getPaths("pathToTrash", Brand);
                    PathToLogMode = myXMLReader.getPaths("pathToLogMode", Brand)[0];
                    pathToLogs = myXMLReader.getPaths("pathToLogs", Brand)[0];
                    this.composition = composition;
                    break;
                case ("Oasis"):
                    indexFS = 4;
                    Brand = "Bernafon";
                    PathTrash = myXMLReader.getPaths("pathToTrash", Brand);
                    PathToLogMode = myXMLReader.getPaths("pathToLogMode", Brand)[0];
                    pathToLogs = myXMLReader.getPaths("pathToLogs", Brand)[0];
                    this.composition = composition;
                    break;
                default:
                    indexFS = -1;
                    break;
            }
            pathToExe = BuildInfo.ListPathsToSetup[indexFS];

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

        public void updateInfoFS(object sender, EventArgs e)
        {
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

        public string getMarket()
        {
            return fileOperator.getMarket(indexFS, true);
        }

        public bool setMarket(string Market)
        {
            if (fileOperator.setMarket(indexFS, Market, true))
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

        public string getFS_Version()
        {
            List<string> ListPathsToAboutInfo = new List<string>();
            List<string> ListpathsToManInfo = new List<string>();
            
                ListpathsToManInfo = BuildInfo.ListPathsToManInfo;
                ListPathsToAboutInfo = BuildInfo.ListPathsToAboutInfo;

            //ListpathsToManInfo = BuildInfo.ListPathsToManInfo; // zakomentować gdy juz dopisze funkcje

            //fileOperator.getPathToManufacturerInfo_Compo(); // odkomentowac
            //ListpathsToManInfo = fileOperator.pathToManufacturerInfo_Compo;
            //ListPathsToAboutInfo // napisac funkcje do pobierania listy albo stringa z pathami do abouta

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
