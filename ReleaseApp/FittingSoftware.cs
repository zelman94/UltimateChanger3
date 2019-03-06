using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{
    class FittingSoftware
    {
       public string Name_FS;
        public string Path_Local_Installer;
        public string Version;
        public string Market;
        public string Hattori_Path;
        public bool customPath; // jezeli customowa lokalizacja FS
        public int indexFS;
        public string Brand;

        public string OEM ;
        public string SelectedLanguage;
        public string LogMode;

        FileOperator fileOperator = new FileOperator();

        public FittingSoftware(string Name)
        {
            Name_FS = Name;
            Path_Local_Installer = findUnInstaller();
            Version = getFS_Version();
            Market = getMarket();
            customPath = false;
            switch (Name)
            {
                case("Genie 2"):
                    indexFS = 0;
                    break;
                case ("Medical"):
                    indexFS = 1;
                    break;
                case ("Express"):
                    indexFS = 2;
                    break;
                case ("HearSuite"):
                    indexFS = 3;
                    break;
                case ("Oasis"):
                    indexFS = 4;
                    break;
                default:
                    indexFS = -1;
                    break;
            }
            List<string> ListPathsToAboutInfo = new List<string>();
            List<string> ListpathsToManInfo = new List<string>();

            ListpathsToManInfo = BuildInfo.ListPathsToManInfo;
            ListPathsToAboutInfo = BuildInfo.ListPathsToAboutInfo;

            BuildInfo infoAboutFS = fileOperator.GetInfoAboutFs(ListpathsToManInfo[indexFS], ListPathsToAboutInfo[indexFS]);
            Brand = infoAboutFS.Brand;
            Market = infoAboutFS.MarketName;
            OEM = infoAboutFS.OEM;
            SelectedLanguage = infoAboutFS.SelectedLanguage;
            Version = infoAboutFS.Version;
            LogMode = fileOperator.getLogMode(indexFS)[0];
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

        public bool setMarket()
        {
            return true;
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
