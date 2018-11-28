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


        public List<string> listCompoNames = new List<string> { // nazwy plikow do kompozycji 

            "GenieComposition",
            "GenieMedicalComposition",
            "EXPRESSfitComposition",
            "HearSuiteComposition",
            "OasisComposition"
        };
        public List<string> listExeNames = new List<string> { // nazwy plikow do kompozycji 

            "Genie.exe",
            "GenieMedical.exe",            
            "EXPRESSfit.exe",
            "HearSuite.exe",
            "Oasis.exe",
        };


        public List<string> listUninstallProcessNames = new List<string> { // nazwy plikow do kompozycji 


            "Install"
        };



        public List<string> pathToLogMode = myXMLReader.getPaths("pathToLogMode");
        public List<string> pathToLogMode_Compo = new List<string>();
        public List<string> pathToManufacturerInfo_Compo = new List<string>();

        public void getPathToLogMode_Compo() // zapisuje do listy pathToLogMode_Compo zebrane kompozycje pathy do ustawien logow
        {
            pathToLogMode_Compo = Directory.GetFiles(@"C:\Program Files\UltimateChanger\compositions\", "Configure.log4net",SearchOption.AllDirectories).ToList();
        }

        public void getPathToManufacturerInfo_Compo() // zapisuje do listy pathToLogMode_Compo zebrane kompozycje pathy do ustawien logow
        {
            pathToManufacturerInfo_Compo = Directory.GetFiles(@"C:\Program Files\UltimateChanger\compositions\", "ManufacturerInfo.xml", SearchOption.AllDirectories).ToList();
        }

        public string FindSettingFileForComposition(int numberOfCheckbox)
        {
            string path = "";

            foreach (var item in pathToManufacturerInfo_Compo)
            {
                if (item.Contains(listCompoNames[numberOfCheckbox]))
                {
                    return item;
                }
            }

            return path;
        }
        public List<string> GetExeCompo(int nrFS) // pewnie trzeba bedzie poprawic bo z OEM bedzie inna nazwa ...
        {
            List<string> listofExes = new List<string>();

            listofExes = Directory.GetFiles(@"C:\Program Files\UltimateChanger\compositions\", listExeNames[nrFS], SearchOption.AllDirectories).ToList();

            return listofExes;
        }

        public List<string> GetAllLocalCompositions() // lista nazw katalogow z kompozycjami
        {
            List<string> LocalCompos = new List<string>();
            try
            {
                LocalCompos = Directory.GetDirectories(@"C:\Program Files\UltimateChanger\compositions").ToList();
            }
            catch (Exception)
            {

            }
            return LocalCompos;
        }

        public bool CheckIfCompositionIsAvailable(List<string> LocalCompos,int Brand)
        {
            
            foreach (var item in LocalCompos)
            {
                if (item.Contains(listCompoNames[Brand]))
                {
                    return true;
                }
                else if(listCompoNames[Brand] == "CumulusC")
                {
                    if (item.Contains("HearSuite"))
                    {
                        return true;
                    }
                   
                }
            }
            return false;
        }


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

        public static void CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation)
        {

            try // usuwan stary
            {
                File.Delete(System.IO.Path.Combine(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\", "Ultimate Changer.lnk"));
            }
            catch (Exception)
            {

            }

            string shortcutLocation = System.IO.Path.Combine(shortcutPath, shortcutName + ".lnk");
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "Ultimate Changer";   // The description of the shortcut
            shortcut.IconLocation = @"C:\Program Files\UltimateChanger\icon.ico";           // The icon of the shortcut
            shortcut.TargetPath = targetFileLocation;                 // The path of the file that will launch when the shortcut is run
            shortcut.WorkingDirectory = @"C:\Program Files\UltimateChanger";
            shortcut.Save();                                    // Save the shortcut

            File.Move(shortcutLocation, System.IO.Path.Combine(shortcutPath, "Ultimate Changer.lnk"));

        }


        public void setAutostart(bool mode) // true - wlaczony autostar false - bez autostart
        {
            switch (mode.ToString().ToLower())
            {
                case ("true"):
                    try
                    {
                        string tmp = @"%programdata%\Microsoft\Windows\Start Menu\Programs\Startup\"+ System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe";
                        string dest = $"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup";
                            //@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\";

                       // File.Copy(Directory.GetCurrentDirectory() + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe", path2, true); // nowy skrot do exe 

                        CreateShortcut("Ultimate Changer", dest, @"C:\Program Files\UltimateChanger\Ultimate Changer.exe");

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error" + "\n setAutostart(true)");
                    }

                    break;
                case ("false"):
                    try
                    {
                        File.Delete(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\Ultimate Changer.lnk");
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

        public void setLogMode(string mode, int setting_number, byte number_checkbox,bool Full, bool advance, string sett1 = "", string sett2 = "", string sett3 = "") // advance true czyli zaawansowane ustawienia usera
        {
            List<string> plik = new List<string>();
            List<string> plik_edited = new List<string>();
            try
            {
                 // wczytywanie zaleznie od full lub compo - nowa lista z pathami do compo ? funkcja wyszukująca ?
                if (Full) // jezeli pelny build to zainstalowane czy li bez zmian
                {
                    plik = File.ReadAllLines(pathToLogMode[number_checkbox]).ToList<string>();
                }
                else
                {
                    getPathToLogMode_Compo(); // mam wszystkie path dla kompozycji do pliku konfiguracyjnego                     
                    plik = File.ReadAllLines(FindSettingFileForComposition(number_checkbox)).ToList<string>();
                }
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
                // brak pliku nic nie robie 
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
                if (Full)
                {
                    TextWriter tw = new StreamWriter(pathToLogMode[number_checkbox]);
                    foreach (String s in plik_edited)
                        tw.WriteLine(s);
                    tw.Close();
                }
                else
                {
                    TextWriter tw = new StreamWriter(FindSettingFileForComposition(number_checkbox));
                    foreach (String s in plik_edited)
                        tw.WriteLine(s);
                    tw.Close();
                }
               
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
                {"Philips"}
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

                List<string> allFiles = Directory.GetFiles(@"C:\Program Files\UltimateChanger\Data").ToList();
                List<string> allFullFiles = new List<string>(10);
                for (int i = 0; i < 10; i++)
                {
                    allFullFiles.Add("");
                }

                List<string> textDirFile = new List<string>();
                List<string> textPathFile = new List<string>();
                List<pathAndDir> tmpPathAdnDir = new List<pathAndDir>();

                foreach (var item in allFiles) // jade po plikach i biore te ktore mają Compositions w nazwie
                {
                    if (!item.Contains("Composition"))
                    {
                        if (item.Contains("Genie_"))
                        {
                            if (item.Contains("dir"))
                            {
                                allFullFiles[0] = item; // dodaje do listy nazw 
                            }
                            else
                            {
                                allFullFiles[1] = item; // dodaje do listy nazw 
                            }
                            
                        }
                        if (item.Contains("Oasis_"))
                        {
                            if (item.Contains("dir"))
                            {
                                allFullFiles[2] = item; // dodaje do listy nazw 
                            }
                            else
                            {
                                allFullFiles[3] = item; // dodaje do listy nazw 
                            }
                           
                        }
                        if (item.Contains("ExpressFit_"))
                        {
                            if (item.Contains("dir"))
                            {
                                allFullFiles[4] = item; // dodaje do listy nazw 
                            }
                            else
                            {
                                allFullFiles[5] = item; // dodaje do listy nazw 
                            }
                           
                        }
                        if (item.Contains("GenieMedical_"))
                        {
                            if (item.Contains("dir"))
                            {
                                allFullFiles[6] = item; // dodaje do listy nazw 
                            }
                            else
                            {
                                allFullFiles[7] = item; // dodaje do listy nazw 
                            }
                            
                        }
                        if (item.Contains("Philips_"))
                        {
                            if (item.Contains("dir"))
                            {
                                allFullFiles[8] = item; // dodaje do listy nazw 
                            }
                            else
                            {
                                allFullFiles[9] = item; // dodaje do listy nazw 
                            }
                          
                        }
                                                                      
                    }
                }

                pathAndDir tmp = new pathAndDir();

                textDirFile = allFullFiles.FindAll((s => s.Contains("_dir")));
                textPathFile = allFullFiles.FindAll((s => s.Contains("_path")));

                for (int i = 0; i < textDirFile.Count; i++)
                {
                    tmp = new pathAndDir();
                    tmp.dir = File.ReadAllLines(textDirFile[i]).ToList();
                    tmp.path = File.ReadAllLines(textPathFile[i]).ToList();
                    tmpPathAdnDir.Add(new pathAndDir(tmp));
                }


                 ((MainWindow)System.Windows.Application.Current.MainWindow).Paths_Dirs = tmpPathAdnDir;



            }


        }

        public bool setMarket(int licz, string market,bool FULL)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                if (FULL) // dla full nic nie zmieniam
                {
                    doc.Load(BuildInfo.ListPathsToManInfo[licz]);
                    doc.SelectSingleNode("/ManufacturerInfo/MarketName").InnerText = market;
                    doc.Save(BuildInfo.ListPathsToManInfo[licz]);
                }
                else // nowa funkcja na wyszukiwanie pliku dla kompozycji
                {
                    getPathToManufacturerInfo_Compo();
                    
                    doc.Load(FindSettingFileForComposition(licz));
                    doc.SelectSingleNode("/ManufacturerInfo/MarketName").InnerText = market;
                    doc.Save(FindSettingFileForComposition(licz));
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public string getMarket(int licz,bool FULL)
        {
            XmlDocument doc = new XmlDocument();
            if (FULL)
            {
                doc.Load(BuildInfo.ListPathsToManInfo[licz]);
            }
            else // dla kompozyji
            {
                getPathToManufacturerInfo_Compo();
                doc.Load(FindSettingFileForComposition(licz));
            }
           
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
            //Process[] proc = Process.GetProcessesByName(name);
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

        public bool checkRunningProcess(List <string> name)
        {
            Process[] localAll = Process.GetProcesses();
            foreach (var item_name in name)
            {
                //Process[] proc = Process.GetProcessesByName(item_name);               

                foreach (Process item in localAll)
                {
                    string tmop = item.ProcessName;
                    if (tmop.Contains(item_name))
                    {
                        if (!tmop.Contains("Updater"))
                        {
                            return true;
                        }
                        
                    }
                }
            }
           
            return false;
        }
        public bool checkRunningProcess(string name)
        {
            Process[] localAll = Process.GetProcesses();            
            //Process[] proc = Process.GetProcessesByName(item_name);               

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


        static public string getCountUCRun()
        {
            string count = "";
            try
            {
                if (Environment.CurrentDirectory.Contains("Updater")) // jezeli odpalam po update 
                {
                    if (!File.Exists(@"C:\Program Files\UltimateChanger\Settings\counter.txt"))
                    {
                        using (StreamWriter sr = new StreamWriter(@"C:\Program Files\UltimateChanger\Settings\counter.txt"))
                        {
                            sr.WriteLine("0");
                            sr.Close();
                        }
                        return "0";
                    }

                    using (StreamReader sr = new StreamReader(@"C:\Program Files\UltimateChanger\Settings\counter.txt"))
                    {
                        String line = sr.ReadLine();
                        count = line;
                        sr.Close();
                        return count;
                    }
                }// jezeli odpalam normalnie 



                if (!File.Exists(@"Settings\counter.txt"))
                {
                    using (StreamWriter sr = new StreamWriter(@"Settings\counter.txt"))
                    {
                        sr.WriteLine("0");
                        sr.Close();
                    }
                    return "0";
                }


                using (StreamReader sr = new StreamReader(@"Settings\counter.txt"))
                {
                    String line = sr.ReadLine();
                    count = line;
                    sr.Close();
                }
            }
            catch (Exception)
            {

            }


            return count;
        }

        static public void setNextCountUCRun()
        {
            string count = getCountUCRun();


            try
            {
                if (Environment.CurrentDirectory.Contains("Updater")) // jezeli odpalam po update 
                {
                    using (StreamWriter sr = new StreamWriter(@"C:\Program Files\UltimateChanger\Settings\counter.txt"))
                    {
                        int tmp = Convert.ToInt16(count);
                        sr.WriteLine((tmp + 1).ToString());
                        sr.Close();
                    }

                }
                else
                {
                    using (StreamWriter sr = new StreamWriter(@"Settings\counter.txt"))
                    {
                        int tmp = Convert.ToInt16(count);
                        sr.WriteLine((tmp + 1).ToString());
                        sr.Close();
                    }
                }
            }
            catch (Exception)
            {

            }
            
           
        }

        public void StartGearbox()
        {
            string PathToGearbox = "C:\\toolsuites\\gearbox\\eclipseg\\";            
            PathToGearbox += new DirectoryInfo("C:\\toolsuites\\gearbox\\eclipseg").GetDirectories()
                                   .OrderByDescending(d => d.LastWriteTimeUtc).First().ToString();

            try
            {
                Process.Start(PathToGearbox+ "\\eclipse.exe");
            }
            catch (Exception x) // nie ma gearboxa moze jest zainstalowany w innej lokalizacji ? albo dać możliwość do zainstalowania 
            { // dopisać funkcjonalość na zapis katalogu do xml i mozliwosc edycji 
                MessageBox.Show(x.ToString() + PathToGearbox + "\\eclipse.exe");
            }
        }

        public void StartNoah()
        {
            try
            {
                Process.Start("C:\\Program Files (x86)\\HIMSA\\Noah 4\\Noah4.exe");
            }
            catch (Exception x )
            {
                MessageBox.Show("Noah Problem");
            }
        }

        public void StartFS(int licznik, bool Full)
        {
            if (Full)
            {
                try
                {
                    Process.Start(BuildInfo.ListPathsToSetup[licznik]);
                }
                catch (Exception)
                {
                    
                }                
            }
            else
            {
                List<string> pathsToExeCompositions = GetExeCompo(licznik);
                Process.Start(pathsToExeCompositions[0]);

            }
        }


        public void checkVersion()
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //-----------------------------------
            int[] ver = new int[3]; // wersja z srvera
            FileVersionInfo versionInfo ;
            string path;
            try
            {
                versionInfo = FileVersionInfo.GetVersionInfo(@"\\10.128.3.1\DFS_data_SSC_FS_Images-SSC\PAZE\change_market\Multi_Changer\currentVersion\update\Ultimate Changer.exe"); // SSC
                path = @"\\10.128.3.1\DFS_data_SSC_FS_Images-SSC\PAZE\change_market\Multi_Changer\currentVersion\update\";
            }
            catch (Exception x)
            {
                try
                {
                    versionInfo = FileVersionInfo.GetVersionInfo(@"\\demant.com\data\KBN\RnD\FS_Programs\Support_Tools\Ultimate_changer\currentVersion\update\Ultimate Changer.exe"); //other
                    path = @"\\demant.com\data\KBN\RnD\FS_Programs\Support_Tools\Ultimate_changer\currentVersion\update\";
                }
                catch (Exception)
                {
                    return;  
                }
              
            }


            int.TryParse(versionInfo.FileVersion[0].ToString(), out ver[0]);
            int.TryParse(versionInfo.FileVersion[2].ToString(), out ver[1]);
            int.TryParse(versionInfo.FileVersion[4].ToString(), out ver[2]);
            //-----------------------------------------
            //wersja apki
            int[] ver_apki = new int[3];

            int.TryParse(version[0].ToString(), out ver_apki[0]);
            int.TryParse(version[2].ToString(), out ver_apki[1]);
            int.TryParse(version[4].ToString(), out ver_apki[2]);

            bool message = false;
            for (int i = 0; i < 3; i++)
            {
                if (ver_apki[i] < ver[i] && message == false)
                {
                    //System.Windows.Forms.MessageBox.Show($"Update available: {Kolumna[1]}");

                    Window Update = new UpdateWindow(path, "", "true", "true", "true", "true", "true");
                    Update.ShowDialog();


                    //pathsToUpdate = Kolumna[1];

                    //return true;

                     message = true; /*HATORI NARAZIE PODZIEKUJEMY*/
                }
            }



        }


    }
}
