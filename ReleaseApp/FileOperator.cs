using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.ComponentModel;
using System.Xml;
using System.Diagnostics;
using System.IO.Compression;

namespace UltimateChanger
{
    public class FileOperator
    {
        private Label lblGenie;
        private Label lblOasis;
        private Label lblExpressFit;
        private ComboBox cmbMarket;
        private List<CheckBox> checkBoxList;
        private List<string> marketIndex;
        private DataBaseManager dataBase;
        //BackgroundWorker worker;
        private Image imgOticon;
        private Image imgBernafon;
        private Image imgSonic;
        public List<pathAndDir> lista;
        public int licznik_przejsc;
        public bool komunikat_trash = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="genie">Label opisujacy market dla Genie</param>
        /// <param name="oasis">Label opisujacy market dla Oasis</param>
        /// <param name="expressFit">Label opisujacy market dla ExpressFit</param>
        /// <param name="cmbMarket">Combobox z lista marketow</param>
        /// 
        static public List<string> listPathTobuilds = new List<string> {

            @"\\10.128.3.1\DFS_Data_SSC_FS_GenieBuilds\Phoenix\Genie", // po kolei jak w cmb FS
            @"\\10.128.3.1\DFS_Data_SSC_FS_GenieBuilds\Phoenix\Oasis",
            @"\\10.128.3.1\DFS_Data_SSC_FS_GenieBuilds\Phoenix\ExpressFit",
            @"", // medical
            @"", //cumulus
            @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Fitting Applications\Genie\20",
            @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Fitting Applications\Oasis\20",
            @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Fitting Applications\ExpressFit\20",
            @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Fitting Applications\GenieMedical\20",
            @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Fitting Applications\Cumulus\20"

        };
        public List<string> listExeFiles = new List<string> {
            @"Setup.exe",
            @"EXPRESSfitMini.exe"// po kolei 
        };

        public List<string> listFilesName = new List<string> {

            @"0Oticon_dir.txt", // 0FS_dir.txt
            @"0Oticon_path.txt", // 0FS_path.txt
            @"1Bernafon_dir.txt", // 0FS_dir.txt
            @"1Bernafon_path.txt",
            @"2Sonic_dir.txt", // 0FS_dir.txt
            @"2Sonic_path.txt",
            @"3GenieMedical_dir.txt", // 0FS_dir.txt
            @"3GenieMedical_path.txt",
            @"4Cumulus_dir.txt", // 0FS_dir.txt
            @"4Cumulus_path.txt",
            @"0Oticon_PRE_dir.txt", // 0FS_dir.txt
            @"0Oticon_PRE_path.txt", // 0FS_path.txt
            @"1Bernafon_PRE_dir.txt", // 0FS_dir.txt
            @"1Bernafon_PRE_path.txt",
            @"2Sonic_PRE_dir.txt", // 0FS_dir.txt
            @"2Sonic_PRE_path.txt",
            @"3GenieMedical_PRE_dir.txt", // 0FS_dir.txt
            @"3GenieMedical_PRE_path.txt",
            @"4Cumulus_PRE_dir.txt", // 0FS_dir.txt
            @"4Cumulus_PRE_path.txt"
        };

        static public List<string> ListUSB_AvailableComDev_description = new List<string> // string from description
        {
            "SONIC innovations EXPRESSlink",
            "HI-PRO", // == HI-PRO2 // HI-PRO Classic?
            ""
        };
        static public List<string> ListUSB_AvailableComDev = new List<string> // string name of device 
        {
            "EXPRESSlink",
            "HI-PRO", // == HI-PRO2 // HI-PRO Classic?
            ""
        };

        public List<string> pathToLogMode = myXMLReader.getPaths("pathToLogMode");

        public List<string> pathToLogs = myXMLReader.getPaths("pathToLogs");

        public static List<string> pathToTrash = myXMLReader.getPaths("pathToTrash");


        static public void DeleteOldDirs()
        {
            try
            {
                Directory.Delete(@"C:\Program Files\DGS - PAZE & MIBW", true);
            }
            catch (Exception)
            {
            }
        }

        public void setAutostart(bool mode) // true - wlaczony autostar false - bez autostart
        {
            switch (mode.ToString().ToLower())
            {
                case ("true"):
                    try
                    {
                        string tmp = @"%programdata%\Microsoft\Windows\Start Menu\Programs\Startup\"+ System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe";
                        string dest = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\";


                        string path = Path.GetTempPath() + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe";
                        string path2 = dest + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe";
                        File.Copy(Directory.GetCurrentDirectory() + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe", path2, true);

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error" + "\n setAutostart(true)");
                    }

                    break;
                case ("false"):
                    try
                    {
                        File.Delete(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\"+ System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error" + "\n setAutostart(false)");
                    }
                    break;
                default:
                    break;
            }
        }

        public List<string> getLogMode()
        {
            List<string> listalogmode = new List<string>();
            foreach (var item in pathToLogMode)
            {
                try
                {
                    var plik = File.ReadAllLines(item);
                    int licznik = 0;
                    foreach (var item2 in plik)
                    {
                        if (licznik == 15) // mode log
                        {
                            if (item2.Contains("ERROR"))
                            {
                                listalogmode.Add("ERROR");
                            }
                            else if (item2.Contains("DEBUG"))
                            {
                                listalogmode.Add("DEBUG");
                            }
                            else
                            {
                                listalogmode.Add("ALL");
                            }
                        }
                        licznik++;
                    }
                }
                catch (Exception)
                {
                    listalogmode.Add("");
                }
            }
            return listalogmode;
        }
        public bool deleteinfoaboutinstallerpath(string FSname)
        {
            List<string> plik = new List<string>();
            try
            {
                plik = File.ReadAllLines(@"C:\Program Files\DGS - PAZE & MIBW\InstallerPaths.txt").ToList<string>();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return false;
            }
            foreach (var item in plik)
            {
                if (item.Contains(BindCombobox.BrandtoFS[FSname]))
                {
                    plik.Remove(item);
                }
            }
            using (StreamWriter outputFile = new StreamWriter(@"C:\Program Files\DGS - PAZE & MIBW\InstallerPaths.txt"))
            {
                foreach (string line in plik)
                    outputFile.WriteLine(line);

                outputFile.Close();
            }
            return true;
        }

        public void SavePathToFsInstallator(string path) // zapisz path do instalatora instalowanego FS
        {
            List<string> plik = new List<string>();
            try
            {
                plik = File.ReadAllLines(@"C:\Program Files\DGS - PAZE & MIBW\InstallerPaths.txt").ToList<string>();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return;
            }
            plik.Add(path);
            using (StreamWriter outputFile = new StreamWriter(@"C:\Program Files\DGS - PAZE & MIBW\InstallerPaths.txt"))
            {
                foreach (string line in plik)
                    outputFile.WriteLine(line);
                outputFile.Close();
            }
        }
        public string ReadPathToFsInstallator(string FSname) // odczytanie path do instalatora zainstalowanego FS
        {
            List<string> plik = new List<string>();
            try
            {
                plik = File.ReadAllLines(@"C:\Program Files\DGS - PAZE & MIBW\InstallerPaths.txt").ToList<string>();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return null;
            }

            foreach (var item in plik)
            {
                if (item.Contains(FSname))
                {
                    return item;
                }
            }
            return "";
        }

        public void setLogMode(string mode, int setting_number, byte number_checkbox, bool advance, string sett1 = "", string sett2 = "", string sett3 = "") // advance true czyli zaawansowane ustawienia usera
        {
            List<string> plik = new List<string>();
            List<string> plik_edited = new List<string>();
            try
            {
                plik = File.ReadAllLines(pathToLogMode[number_checkbox]).ToList<string>();
            }
            catch(DirectoryNotFoundException e)
            {
                // super :) 

                if (komunikat_trash)
                {
                    MessageBox.Show("remove leavings \n press Delete Trash");
                    komunikat_trash = true;
                    return;
                }
                
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return;
            }
            // mode linia : 15 "      <level value="ERROR"/>"
            // sett1 linia: 21 "      <maximumFileSize value="1MB"/>"
            // sett2 linia: 24 "      <maxSizeRollBackups value="10"/>"
            // set3 linia:: 23 "      <appendToFile value="true"/>"
            byte licznik = 0;
            foreach (var item in plik)
            {
                bool przepis = true;
                if (licznik == 15)
                {
                    plik_edited.Add($"      <level value=\"{mode}\"/>");
                    przepis = false; // zaminaim na false zeby nie przepisywać tego samego wiersza bo juz go zmienilem
                }
                if (advance)
                {
                    if (licznik == 21 && sett1 != "")
                    {
                        plik_edited.Add($"      <maximumFileSize value=\"{sett1} \"/>");
                        przepis = false; // zaminaim na false zeby nie przepisywać tego samego wiersza bo juz go zmienilem
                    }
                    if (licznik == 23 && sett3 != "")
                    {
                        plik_edited.Add($"      <appendToFile value=\"{sett3} \"/>");
                        przepis = false; // zaminaim na false zeby nie przepisywać tego samego wiersza bo juz go zmienilem
                    }
                    if (licznik == 24 && sett2 != "")
                    {
                        plik_edited.Add($"      <maxSizeRollBackups value=\"{sett2}\"/>");
                        przepis = false; // zaminaim na false zeby nie przepisywać tego samego wiersza bo juz go zmienilem
                    }
                }
                else // jezeli nie advance to jakies ustawienia zdefiniowane 
                {
                    if (setting_number == 0) //Default
                    {
                        if (licznik == 21)
                        {
                            plik_edited.Add($"      <maximumFileSize value=\"10MB\"/>");
                            przepis = false; // zaminaim na false zeby nie przepisywać tego samego wiersza bo juz go zmienilem
                        }

                        if (licznik == 24)
                        {
                            plik_edited.Add($"      <maxSizeRollBackups value=\"10\"/>");
                            przepis = false; // zaminaim na false zeby nie przepisywać tego samego wiersza bo juz go zmienilem
                        }
                    }
                    if (setting_number == 1) //New file with Start FS
                    {
                        if (licznik == 21)
                        {
                            plik_edited.Add($"      <maximumFileSize value=\"50MB \"/>");
                            przepis = false; // zaminaim na false zeby nie przepisywać tego samego wiersza bo juz go zmienilem
                        }
                        if (licznik == 23)
                        {
                            plik_edited.Add($"      <appendToFile value=\"true\"/>");
                            przepis = false; // zaminaim na false zeby nie przepisywać tego samego wiersza bo juz go zmienilem
                        }
                        if (licznik == 24)
                        {
                            plik_edited.Add($"      <maxSizeRollBackups value=\"10\"/>");
                            przepis = false; // zaminaim na false zeby nie przepisywać tego samego wiersza bo juz go zmienilem
                        }
                    }
                    if (setting_number == 2) //One file 100 MB
                    {
                        if (licznik == 21)
                        {
                            plik_edited.Add($"      <maximumFileSize value=\"100MB \"/>");
                            przepis = false; // zaminaim na false zeby nie przepisywać tego samego wiersza bo juz go zmienilem
                        }

                        if (licznik == 24)
                        {
                            plik_edited.Add($"      <maxSizeRollBackups value=\"1\"/>");
                            przepis = false; // zaminaim na false zeby nie przepisywać tego samego wiersza bo juz go zmienilem
                        }
                    }
                }
                if (przepis)
                {
                    plik_edited.Add(item);
                }
                licznik++;
            }
            if (plik_edited.Count != plik.Count)
            {
                MessageBox.Show("Warning ! \n something was wrong ! \n Numbers of lines before and after changes are not the same ! \n changes not saved");
                return;
            }

            try
            {
                TextWriter tw = new StreamWriter(pathToLogMode[number_checkbox]);
                foreach (String s in plik_edited)
                    tw.WriteLine(s);
                tw.Close();
            }
            catch(DirectoryNotFoundException e)
            {

            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }
        }

        public void GetfilesSaveData(bool composition, int FS) // pobieram pliki z dysku i zapisuje z nich dane do zmiennej potrzebnej do cmbbuilds 
        {
            // compozycja true/false
            List<string> ListaFS = new List<string>()
            {
                {"Oticon"},
                {"Bernafon"},
                {"Sonic"},
                {"GenieMedical"},
                {"Cumulus"}
            };


            if (composition)
            {
               List<string> allFiles = Directory.GetFiles(@"C:\Program Files\UltimateChanger\Data").ToList();
                List<string> allCompositionsFiles = new List<string>();
                List<string> textDirFile = new List<string>();
                List<string> textPathFile = new List<string>();
                List<pathAndDir> tmpPathAdnDir = new List<pathAndDir>();

                foreach (var item in allFiles) // jade po plikach i biore te ktore mają Compositions w nazwie
                {
                    if (item.Contains("Composition") )
                    {
                        allCompositionsFiles.Add(item); // dodaje do listy nazw 
                    }
                }

                foreach (var item in allCompositionsFiles)
                {
                    if (item.Contains("dir")) // diry
                    {
                        textDirFile = File.ReadAllLines( item).ToList();
                    }
                    else // patchy
                    {
                        textPathFile = File.ReadAllLines( item).ToList();
                    }
                }

                pathAndDir tmp = new pathAndDir();
                tmp.dir = new List<string>(textDirFile);
                tmp.path = new List<string>(textPathFile);
                tmpPathAdnDir.Add(new pathAndDir(tmp));
                ((MainWindow)System.Windows.Application.Current.MainWindow).Paths_Dirs = tmpPathAdnDir;


            }
            else
            {

            }


        }

        public void setMarket(int licz, string market)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(BuildInfo.ListPathsToManInfo[licz]);
                doc.SelectSingleNode("/ManufacturerInfo/MarketName").InnerText = market;
                doc.Save(BuildInfo.ListPathsToManInfo[licz]);
            }
            catch (Exception)
            {

            }

        }

        public string getMarket(int licz)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(BuildInfo.ListPathsToManInfo[licz]);
            return doc.SelectSingleNode("/ManufacturerInfo/MarketName").InnerText;
        }

        public bool checkInstanceOfFS(int number)
        {
            if (File.Exists(BuildInfo.ListPathsToSetup[number]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool checkInstanceFakeVerifit()
        {
            if (File.Exists(@"C:\Program Files (x86)\REMedy\REMedy.Launcher.exe"))
            {
                return true; // jezeli istnieje to prawde
            }
            else
                return false;
        }

        public bool checkInstanceNewPreconditioner()
        {
            if (File.Exists(@"C:\Program Files (x86)\NewPreconditioner\NewPreconditioner.exe"))
            {
                return true; // jezeli istnieje to prawde
            }
            else
                return false;
        }

        string[] marki = { "Genie", "Oasis", "EXPRESSfit", "Philips HearSuite", "Philips HearSuite (development mode)", "Genie Medical BAHS", "HearSuite" };
        bool killRunningProcess(string name)
        {
            Process[] proc = Process.GetProcessesByName(name);
            Process[] localAll = Process.GetProcesses();
            foreach (Process item in localAll)
            {
                string tmop = item.ProcessName;
                if (tmop == name)
                {
                    item.Kill();
                    return true;
                }
            }
            return false;
        }

        public bool checkRunningProcess(string name)
        {
            Process[] proc = Process.GetProcessesByName(name);
            Process[] localAll = Process.GetProcesses();

            foreach (Process item in localAll)
            {
                string tmop = item.ProcessName;
                if (tmop.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        public void KillFS()
        {
            foreach (var item in marki)
            {
                killRunningProcess(item);
            }
        }

        public List<pathAndDir> getAllDirPath(string release) // pobieram wszystkie sciezki i dir z path i podmieniam w glownym pliku 
        {
            List<pathAndDir> lista2 = new List<pathAndDir>();

            foreach (var item in listPathTobuilds)
            {
                pathAndDir tmp = new pathAndDir();
                //licznik_przejsc++;
                if (item.Contains("FS_Programs"))
                {
                    lista2.Add(GetBindDirNames(item + release + @"\Pre-releases", listExeFiles, tmp, 0));
                }
                else
                    lista2.Add(GetBindDirNames(item, listExeFiles, tmp, 0));
            }
            return lista2;
        }
        
        private pathAndDir GetBindDirNames(string path, List<string> exenames, pathAndDir tmp, int nr)
        {
            List<string> dir = null;
            bool flag = false;
            int dl;
            try
            {
                dir = System.IO.Directory.GetDirectories(path).ToList<string>();
                var firstItems = dir.OrderBy(q => q).Take(20);
                dir = firstItems.ToList<string>();
            }
            catch (Exception)
            {}

            if (nr == 0 && dir != null)
            {
                foreach (var item in dir)
                {
                    tmp.dir.Add(new DirectoryInfo(item).Name);
                }
            }

            if (dir != null)
            {
                foreach (var item in dir)
                {
                    GetBindDirNames($"{item}", exenames, tmp, ++nr);
                    List<string> pliczki = System.IO.Directory.GetFiles(item).ToList<string>();

                    foreach (var item2 in exenames)
                    {
                        foreach (var item3 in pliczki)
                        {
                            dl = item3.Length - 1;

                            string tmp_item3 = item3.Substring(dl - item2.Length);
                            if (tmp_item3 == ("\\" + item2))
                            {
                                tmp.path.Add(item3);
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
            }
            return tmp;
        }

        public FileOperator()
        {
            licznik_przejsc = 0;
            try
            {
                if (!Directory.Exists($"C:\\Program Files\\UltimateChanger\\Data"))
                {
                    if (!Directory.Exists($"C:\\Program Files\\UltimateChanger"))
                    {
                        Directory.CreateDirectory($"C:\\Program Files\\UltimateChanger");
                        Directory.CreateDirectory($"C:\\Program Files\\UltimateChanger\\Data");
                    }
                    else
                    {
                        Directory.CreateDirectory($"C:\\Program Files\\UltimateChanger\\Data");
                    }
                }
            }
            catch (Exception x)
            {

                MessageBox.Show("can not create new directory C:\\Program Files\\UltimateChanger\\Data");
            }

        }

        public FileOperator(DataBaseManager dataBase, Label genie, Label oasis, Label expressFit, ComboBox cmbMarket, List<CheckBox> checkBoxList, List<string> marketIndex, Image imgOticon, Image imgBernafon, Image imgSonic)
        {
            this.dataBase = dataBase;
            this.lblGenie = genie;
            this.lblOasis = oasis;
            this.lblExpressFit = expressFit;
            this.cmbMarket = cmbMarket;
            this.checkBoxList = checkBoxList;
            this.marketIndex = marketIndex;
            this.imgOticon = imgOticon;
            this.imgBernafon = imgBernafon;
            this.imgSonic = imgSonic;
        }

        public void Savebuildsinfo()
        {
            List<pathAndDir> tmpList = ((MainWindow)System.Windows.Application.Current.MainWindow).Paths_Dirs;
            int j = 1;
            int k = 0;
            for (int i = 0; i < tmpList.Count; i++)
            {
                try
                {
                    //File.Delete(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[k]);
                    //File.Delete(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[j]);

                    //File.Create(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[k]);
                    //File.Create(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[j]);                    

                    using (StreamWriter outputFile = new StreamWriter(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[k]))
                    {
                        foreach (string line in tmpList[i].dir)
                            outputFile.WriteLine(line);

                        outputFile.Close();
                    }

                    using (StreamWriter outputFile = new StreamWriter(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[j]))
                    {
                        foreach (string line in tmpList[i].path)
                            outputFile.WriteLine(line);

                        outputFile.Close();
                    }

                    //System.IO.File.WriteAllLines(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[k], tmpList[i].dir);
                    //System.IO.File.WriteAllLines(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[j], tmpList[i].path);
                                       
                    j += 2; ;
                    k += 2; ;
                }
                catch (Exception)
                {
                    MessageBox.Show("cannot write to file");
                }
            }
        }

        public bool getDataToBuildCombobox()
        {
            if (Directory.Exists(@"C:\Program Files\DGS - PAZE & MIBW\Data"))
            {
                string[] filess = Directory.GetFiles(@"C:\Program Files\DGS - PAZE & MIBW\Data").Select(file => Path.GetFileName(file)).ToArray(); // wszystkie pliki z katalogu
                List<string> files = new List<string>(); // wszystkie pliki z dir
                List<string> paths = new List<string>(); // wszystkie pliki z path
                foreach (var item in filess)
                {
                    if (item.Contains("dir"))
                    {
                        files.Add(item);
                    }
                    else
                    {
                        paths.Add(item);
                    }
                }
                pathAndDir tmp = new pathAndDir();
                List<List<string>> dirsList = new List<List<string>>();
                List<List<string>> pathsList = new List<List<string>>();

                foreach (var item in files)
                {
                    try
                    {
                        List<string> logList = new List<string>();
                        using (StreamReader sr = new StreamReader(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + item))
                        {
                            while (!sr.EndOfStream)
                                logList.Add(sr.ReadLine());

                            sr.Close();
                        }
                        dirsList.Add(logList);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("The file could not be read:");
                        return false;
                    }
                }

                foreach (var item2 in paths)
                {
                    try
                    {
                        List<string> logList = new List<string>();
                        using (StreamReader sr = new StreamReader(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + item2))
                        {
                            while (!sr.EndOfStream)
                                logList.Add(sr.ReadLine());
                            sr.Close();
                        }
                        pathsList.Add(logList);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("The file could not be read:");
                        return false;
                    }
                }

                for (int i = 0; i < dirsList.Count; i++)
                {
                    pathAndDir tmpp = new pathAndDir();
                    tmpp.dir = dirsList[i];
                    tmpp.path = pathsList[i];
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Paths_Dirs.Add(tmpp);
                }
                return true;
            }
            else
            {
                try
                {
                    if (Directory.Exists(@"C:\Program Files\DGS - PAZE & MIBW\"))
                    {
                        Directory.CreateDirectory(@"C:\Program Files\DGS - PAZE & MIBW\Data");
                    }
                    else
                    {
                        Directory.CreateDirectory(@"C:\Program Files\DGS - PAZE & MIBW\");
                        Directory.CreateDirectory(@"C:\Program Files\DGS - PAZE & MIBW\Data");
                    }
                    // zrobić bindowanie z rekurencji podpiąć i zapisać do pliku 

                    //return true;
                }
                catch (Exception)
                {
                    return false;
                }

                List<pathAndDir> tmpList = ((MainWindow)System.Windows.Application.Current.MainWindow).Paths_Dirs;
                int j = 1;
                int k = 0;
                for (int i = 0; i < tmpList.Count; i++)
                {
                    try
                    {
                        File.Create(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[k]);
                        File.Create(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[j]);

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Program Files\DGS - PAZE & MIBW\Data\" + listFilesName[k]))
                        {
                            foreach (string line in tmpList[i].dir)
                            {
                                file.WriteLine(line);
                            }
                            file.Close();
                        }
                        j += 2; ;
                        k += 2; ;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("cannot write to file");
                    }
                }
                return true;
            }
        }

        public bool Get_run_with_Application() // czytanie z pliku czy chcemy zeby byla apka uruchamiana 
        {
            if (File.Exists(@"C:\Program Files\DGS - PAZE & MIBW\Multi Changer\info.txt"))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(@"C:\Program Files\DGS - PAZE & MIBW\Multi Changer\info.txt"))
                    {
                        String line = sr.ReadToEnd();
                        if (line == "1")
                        {
                            sr.Close();
                            return true; // jezeli jest 1 to chceymy zeby sie uruchamiał z windowsem czyli kopiujemy exe do folderu startup
                        }
                        else
                        {
                            sr.Close();
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("The file could not be read:");
                    return false;
                }
            }
            else
            {
                try
                {
                    File.Create(@"C:\Program Files\DGS - PAZE & MIBW\Multi Changer\info.txt");
                }
                catch (Exception)
                {
                    if (!Directory.Exists(@"C:\Program Files\DGS - PAZE & MIBW"))
                        Directory.CreateDirectory(@"C:\Program Files\DGS - PAZE & MIBW");
                    if (!Directory.Exists(@"C:\Program Files\DGS - PAZE & MIBW\Multi Changer"))
                        Directory.CreateDirectory(@"C:\Program Files\DGS - PAZE & MIBW\Multi Changer");
                    File.Create(@"C:\Program Files\DGS - PAZE & MIBW\Multi Changer\info.txt");
                }
                return false;
            }
        }

        public bool Get_delete_logs_file() // czytanie z pliku czy chcemy zeby byly usuwane log mody
        {
            if (File.Exists(@"C:\Program Files\DGS - PAZE & MIBW\Multi Changer\info2.txt"))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(@"C:\Program Files\DGS - PAZE & MIBW\Multi Changer\info2.txt"))
                    {
                        String line = sr.ReadToEnd();
                        if (line == "1")
                        {
                            sr.Close();
                            return true; // jezeli jest 1 to chceymy zeby usuwaly pliki
                        }
                        else
                        {
                            sr.Close();
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("The file could not be read:");
                    File.Create(@"C:\Program Files\DGS - PAZE & MIBW\Multi Changer\info2.txt");
                    return false;
                }
            }
            else
            {
                File.Create(@"C:\Program Files\DGS - PAZE & MIBW\Multi Changer\info2.txt");
                return false;
            }
        }

        public static long GetDirectorySize(string parentDirectory) //zwraca rozmiar directory w bajtach
        {
            return new DirectoryInfo(parentDirectory).GetFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);
        }

        private String GetData(string name)
        {
            String line = String.Empty;
            int counter = 0;
            if (File.Exists(name))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(name))
                    {
                        while (counter != 4)
                        {
                            line = sr.ReadLine();
                            counter++;
                        }
                        if (line[15] == 'e')
                        {
                            return "Defukt";
                        }
                        return $"{line[14]}{line[15]}";
                    }
                }
                catch (FileNotFoundException)
                {
                    return "";
                }
                catch (DirectoryNotFoundException)
                {
                    return "";
                }
                catch (NullReferenceException)
                {
                    return "";
                }
            }
            else
            {
                return "File is missing";
            }
        }

        public BuildInfo GetInfoAboutFs(string path, string path2)
        {
            XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc.Load(path); // Load the XML document from the specified file
                               // Get elements
            XmlNodeList Brand = xmlDoc.GetElementsByTagName("Brand");
            XmlNodeList MarketName = xmlDoc.GetElementsByTagName("MarketName");
            XmlNodeList OEM = xmlDoc.GetElementsByTagName("OEM");
            XmlNodeList SelectedLanguage = xmlDoc.GetElementsByTagName("SelectedLanguage");
            xmlDoc2.Load(path2);
            XmlNodeList Major = xmlDoc2.GetElementsByTagName("Major");
            XmlNodeList Minor = xmlDoc2.GetElementsByTagName("Minor");
            XmlNodeList Build = xmlDoc2.GetElementsByTagName("Build");
            XmlNodeList Revision = xmlDoc2.GetElementsByTagName("Revision");
            string about = Major[0].InnerText + "." + Minor[0].InnerText + "." + Build[0].InnerText + "." + Revision[0].InnerText;

            return new BuildInfo(Brand[0].InnerText, MarketName[0].InnerText, OEM[0].InnerText, SelectedLanguage[0].InnerText, about);
        }

   
    }
}
