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
        public string Emulator_Path = "";
        public string DirFullBuildName = ""; // \\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\FullInstaller-19.2\rc-19.2_19.2_7.18.46.0-b278379\FullInstaller-19.2-Genie i tutaj (Oticon)
        DispatcherTimer Timer_InfoFS; // timer do sprawdzania info o buildach
        List<string> ListPathsToAboutInfo = new List<string>();
        List<string> ListpathsToManInfo = new List<string>();
        public string pathToExe, pathToManu;
        FileOperator fileOperator = new FileOperator();
        public bool composition,uninstallation=false;
        public string PathToNewVerFS=""; // path do nowej wersji FS tylko dla fulli bedzie
        public Task Task_GetNewBuild =null;
        public string path_ConfigData = "";
        public Upgrade_FittingSoftware Upgrade_FS = null; // jezeli null to nie ma zgody na nocny update


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
            var localCompo = fileOperator.GetAllLocalCompositions();
            switch (Name)
            {
                case ("Genie"):

                    Brand = "Oticon";
                    if (composition)
                    {
                        indexFS = 0 + 5;
                        PathTrash = new List<string>() { localCompo[0] };
                        fileOperator.getPathToLogMode_Compo();
                        PathToLogMode = fileOperator.getPathToConfigure(0);
                        pathToLogs = ""; // do sprawdzenia gdzie sie zapisuja
                        pathToManu = fileOperator.FindSettingFileForComposition(0);
                    }
                    else
                    {
                        indexFS = 0;
                        PathTrash = myXMLReader.getPaths("pathToTrash", Brand);
                        PathToLogMode = myXMLReader.getPaths("pathToLogMode", Brand)[0];
                        pathToLogs = myXMLReader.getPaths("pathToLogs", Brand)[0];
                        DirFullBuildName = "Oticon";
                        path_ConfigData = @"C:\ProgramData\Oticon\Genie\ConfigurationData";
                    }
                    
   
                    this.composition = composition;
                    break;

                case ("GenieMedical"):

                    Brand = "Medical";
                    if (composition)
                    {
                        indexFS = 1 + 5;
                        PathTrash = new List<string>() { localCompo[1] };
                        fileOperator.getPathToLogMode_Compo();
                        PathToLogMode = fileOperator.getPathToConfigure(1);
                        pathToLogs = ""; // do sprawdzenia gdzie sie zapisuja
                        pathToManu = fileOperator.FindSettingFileForComposition(1);
                    }
                    else
                    {
                        indexFS = 1;
                        PathTrash = myXMLReader.getPaths("pathToTrash", Brand);
                        PathToLogMode = myXMLReader.getPaths("pathToLogMode", Brand)[0];
                        pathToLogs = myXMLReader.getPaths("pathToLogs", Brand)[0];
                        DirFullBuildName = "OticonMedical";
                        path_ConfigData = @"C:\ProgramData\OticonMedical\GenieMedical\ConfigurationData";
                    }

                    this.composition = composition;
                    break;
                case ("ExpressFit"):

                    Brand = "Sonic";
                    if (composition)
                    {
                        indexFS = 2 + 5;
                        PathTrash = new List<string>() { localCompo[2] };
                        fileOperator.getPathToLogMode_Compo();
                        PathToLogMode = fileOperator.getPathToConfigure(2);
                        pathToLogs = ""; // do sprawdzenia gdzie sie zapisuja
                        pathToManu = fileOperator.FindSettingFileForComposition(2);
                    }
                    else
                    {
                        indexFS = 2;
                        PathTrash = myXMLReader.getPaths("pathToTrash", Brand);
                        PathToLogMode = myXMLReader.getPaths("pathToLogMode", Brand)[0];
                        pathToLogs = myXMLReader.getPaths("pathToLogs", Brand)[0];
                        DirFullBuildName = "Sonic";
                        path_ConfigData = @"C:\ProgramData\Sonic\Expressfit\ConfigurationData";
                    }


                    this.composition = composition;
                    break;
                case ("HearSuite"):

                    Brand = "Philips";
                    if (composition)
                    {
                        indexFS = 3 + 5;
                        PathTrash = new List<string>() { localCompo[3] };
                        fileOperator.getPathToLogMode_Compo();
                        PathToLogMode = fileOperator.getPathToConfigure(3);
                        pathToLogs = ""; // do sprawdzenia gdzie sie zapisuja
                        pathToManu = fileOperator.FindSettingFileForComposition(3);
                    }
                    else
                    {
                        indexFS = 3;
                        PathTrash = myXMLReader.getPaths("pathToTrash", Brand);
                        PathToLogMode = myXMLReader.getPaths("pathToLogMode", Brand)[0];
                        pathToLogs = myXMLReader.getPaths("pathToLogs", Brand)[0];
                        DirFullBuildName = "Philips";
                        path_ConfigData = @"C:\ProgramData\Philips" + "\"" + " " + "\"" + @"HearSuite\HearSuite\ConfigurationData";
                    }

                    this.composition = composition;
                    break;
                case ("Oasis"):

                    Brand = "Bernafon";
                    if (composition)
                    {
                        indexFS = 4 + 5;
                        PathTrash = new List<string>() { localCompo[4] };
                        fileOperator.getPathToLogMode_Compo();
                        PathToLogMode = fileOperator.getPathToConfigure(4);
                        pathToLogs = ""; // do sprawdzenia gdzie sie zapisuja
                        pathToManu = fileOperator.FindSettingFileForComposition(4);
                    }
                    else
                    {
                        indexFS = 4;
                        PathTrash = myXMLReader.getPaths("pathToTrash", Brand);
                        PathToLogMode = myXMLReader.getPaths("pathToLogMode", Brand)[0];
                        pathToLogs = myXMLReader.getPaths("pathToLogs", Brand)[0];
                        DirFullBuildName = "Bernafon";
                        path_ConfigData = @"C:\ProgramData\Bernafon\Oasis\ConfigurationData";
                    }


                    this.composition = composition;
                    break;
                default:
                    indexFS = -1;
                    break;
            }
            if (!composition) // dla full builds
            {
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
                // nowy timer dla sprawdzania czy nadszedl czas dla update FS ? 
                // jezeli null to wychodze ze sprawdzenia jezeli cos jest to wejsc do srodka obiektu i spr czy zgadza sie czas 
                // jezeli zgadza sie czas to przekazac liste do odpowiedniej listy w głównym oknie 
                // albo dac ten timer na mainwindow i tam to sprawdzacchyba lepsze :)
                //przejechac po liscie obiektow i spr czy jest nullem 
                // a jezeli zaczne już robić update to po przekazaniu listy od wszystkich dostepnych FS  z pathami do nowszej wersji 
                // mozna usunac obiekty i wylaczyc sprawdzanie timera czy obiekt jest nullem
            }
            else
            {
                ListpathsToManInfo = fileOperator.getPathToManufacturerInfo_Compo_List() ;
                try
                {
                    pathToExe = fileOperator.GetExeCompo(indexFS - 5)[0];
                }
                catch (Exception)
                {
                    pathToExe = "";
                }
               
                pathToManu = fileOperator.FindSettingFileForComposition(indexFS - 5);              
                Market = getMarket();
                OEM = "";
                SelectedLanguage = "";
                Version = getFS_Version();
                LogMode = getLogMode();
            }

            Emulator_Path = fileOperator.getPathToEmulator(indexFS, composition, pathToExe);
        }

        public void StartEmulator()
        {

            if (path_ConfigData != "")
            {
                try
                {
                    Process tmpProcess = new Process();
                    tmpProcess.StartInfo.WorkingDirectory = "Emulator\\";
                    tmpProcess.StartInfo.FileName = "Phoenix.HardwareAbstraction.Ninjago.Emulation.Program.exe";
                    tmpProcess.StartInfo.Arguments = $"ConfigurationDataPath={path_ConfigData}";
                    tmpProcess.Start();
                }
                catch (IOException y)
                {
                    // nie ma emulatora
                    MessageBox.Show("lack of Emulator.exe");
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.ToString());
                }                
            }
            else
            {
                MessageBox.Show("No available Emulator for " + Brand + "\nComposition: " + composition );
            }
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
                catch (Exception )
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
            if (composition)
            {
                return fileOperator.getMarket(indexFS - 5, !composition);
            }
            else
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
            this.LogMode = LogMode;
            fileOperator.setLogMode(PathToLogMode, LogMode, indexSetting,Convert.ToByte(indexFS),Full, false, "", "", "");
        }

        public void getNewFSPath()
        {

                Task_GetNewBuild = Task.Run(() => { // watek 
                try
                {
                    PathToNewVerFS = fileOperator.GetAvailableNewFS(this);
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.ToString());
                }
            });

            //PathToNewVerFS = fileOperator.GetAvailableNewFS(this);
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
            try
            {
                pathToLogsFiles = Directory.GetFiles(pathToLogs).ToList();
            }
            catch (Exception)
            {
                return;
            }

            string problems = "";
            foreach (var item in pathToLogsFiles)
            {
                try
                {
                    File.Delete(item);
                }
                catch (Exception)
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

        public string getFS_Version()
        {

            if (composition)
            {
                try
                {
                    FileVersionInfo tmp = FileVersionInfo.GetVersionInfo(pathToExe);

                    return tmp.FileVersion;
                }
                catch (Exception)
                {
                    return "";
                }
            }
            else
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
}
