using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Data.SqlClient;
using MySql;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Forms.Integration;
using System.Text.RegularExpressions;
//  poprawic pobieranie info o buildzie !!!!!!!!!!!!!!!!!!!!!!!!!! brac z pliku a nie z info o pliku...
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.Net;
using System.Data;

[assembly: System.Reflection.AssemblyVersion("2.1.1.0")]
namespace UltimateChanger
{//
    public partial class MainWindow : Window
    {
        int Licznik_All_button = 0;

        //TrashCleaner Cleaner;
        Dictionary<string, string> FStoPath;
        FileOperator fileOperator;
        DataBaseManager dataBaseManager;
        ClockManager clockManager;
        // DataBaseManager dataBaseManager;
        DispatcherTimer RefUiTIMER, Rekurencja;
        DispatcherTimer dispatcherTimer, progressBarTimer;
        DispatcherTimer uninstallTimer;
        BindCombobox BindCombo;
        private List<pathAndDir> paths_Dirs = new List<pathAndDir>();
        //string OEMname = "";
        List<Image> ListImages;
        List<Label> listlabelsinfoFS;
        List<Label> ListLabelsonUI = new List<Label>();
        List<ListBox> ListListBoxsonUI = new List<ListBox>();
        List<Button> ListButtonsonUI = new List<Button>();
        List<CheckBox> checkBoxList = new List<CheckBox>();
        List<ComboBox> comboBoxList = new List<ComboBox>();
        List<string> listOfTeammembers = new List<string>();
        List<string> listOfFiczursSelected = new List<string>();
        List<string> listOfRandomHardawre_perPerson = new List<string>();
        List<RadioButton> RadioButtonsList = new List<RadioButton>();
        public SortedDictionary<string, string> StringToUI = new SortedDictionary<string, string>(); // slownik do zamiany stringow z xml do wartości UI 
        List<Rectangle> ListRactanglesNames;
        //BackgroundWorker worker;
        HIs Random_HI = new HIs();
        myXMLReader XMLReader = new myXMLReader();
        public List<List<string>> AllbuildsPerFS = new List<List<string>>();
        internal List<pathAndDir> Paths_Dirs { get => paths_Dirs; set => paths_Dirs = value; }
        string User_Power;
        public List<string> RandomHardware;

        public MainWindow()
        {
            try
            {
                var exists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
                if (exists) // jezeli wiecej niz 1 instancja to nie uruchomi sie
                {
                    System.Environment.Exit(1);
                }
                fileOperator = new FileOperator();
                clockManager = new ClockManager();
                InitializeComponent();
                // Localization.SetAttributes(this,"TOP"); 

                przegladarka.Navigate("http://confluence.kitenet.com/display/SWSQA/Ultimate+Changer");
                initializeElements();
                initiationForprograms();
                BindCombo = new BindCombobox();
                BindCombo.setFScomboBox();
                BindCombo.setReleaseComboBox();
                BindCombo.setMarketCmb();
                BindCombo.bindlogmode();
                bindMarketDictionary();
                BindCombo.bindListBox();


                fileOperator.getDataToBuildCombobox();
                initializeTimers();
                // zamiast watku napisac maly programik osobny ktory bedzie uruchamiany na timerze co 3 s i bedzie sprawdzac czy sie zakonczyl ! :D
                if (!statusOfProcess("Rekurencjon"))
                {
                    //Process.Start(Environment.CurrentDirectory + @"\reku" + @"\Rekurencjon.exe", cmbRelease.Text); // wlaczyc gdy bedzie nowy exe gotowy
                }

                try
                {
                    foreach (Label tb in FindLogicalChildren<Label>(this)) // dziala
                    {
                        ListLabelsonUI.Add(tb);
                    }
                    foreach (ListBox item in FindLogicalChildren<ListBox>(this))
                    {
                        ListListBoxsonUI.Add(item);
                    }
                    foreach (Button item in FindLogicalChildren<Button>(this))
                    {
                        ListButtonsonUI.Add(item);
                    }
                }
                catch (Exception xc)
                {
                    MessageBox.Show($"error MainWindow \n {xc.ToString()}");
                }


                cmbRelease.IsEnabled = false;
                Rekurencja = new DispatcherTimer();
                Rekurencja.Tick += checkRekurencja;
                Rekurencja.Interval = new TimeSpan(0, 0, 1);
                Rekurencja.Start();






                // napisac funkcje w fileoperation na pobieranie zapisanych danych z pliku i wpisanie do PathDir lista czy cos 
                /*refreshUI(); */// funkcja  caly ui
                //fileOperator.GetInfoAboutFs(@"C:\ProgramData\Bernafon\Common\ManufacturerInfo.xml"); // dziala 
            }
            catch (Exception x)
            {
                MessageBox.Show("inicjalizacja \n" + x.ToString());
            }
            sliderRelease.Maximum = cmbRelease.Items.Count - 1; // max dla slidera -1 bo count nie uwzglednia zerowego indexu
            sliderWeightWireless.Maximum = 1;
            sliderRelease.Value = cmbRelease.SelectedIndex; // ustalenie defaulta jako obecny release
            sliderWeightWireless.Value = 0.5; // to oznacza ze nic nie zmieniam i wszystko jes po rowno w szansach 
            lblWeightWireless.Content = sliderWeightWireless.Value.ToString();

            ListBoxOfAvailableFeautures.SelectionMode = SelectionMode.Multiple;

            ListBoxOfAvailableStyles.SelectionMode = SelectionMode.Multiple;
            ListBoxOfAvailableTypes.SelectionMode = SelectionMode.Multiple;

            refreshUI(new object(), new EventArgs());
            dataBaseManager = new DataBaseManager(XMLReader.getDefaultSettings("DataBase").ElementAt(0).Value);
            if (dataBaseManager != null)
            {
                dataBaseManager.getInformation_DB();
            }

            setUIdefaults(XMLReader.getDefaultSettings("RadioButtons"), "RadioButtons");
            setUIdefaults(XMLReader.getDefaultSettings("CheckBoxes"), "CheckBoxes");
            setUIdefaults(XMLReader.getDefaultSettings("ComboBox"), "ComboBox");

           
        }
        //________________________________________________________________________________________________________________________________________________

        public void setUIdefaults(SortedDictionary<string, string> settings, string mode) // mode to tryb ustawienia co zmieniasz radiobutton checkbox
        {

            switch (mode)
            {
                case ("RadioButtons"):
                    foreach (var item in RadioButtonsList)
                    {
                        try
                        {
                            //string tmpNameOfRadioButton = StringToUI[item.Name];
                            // w item mam nazwe radiobuttona i radiobutton
                            foreach (var item2 in StringToUI.Keys)
                            {
                                if (item2 == item.Name)
                                {
                                    item.IsChecked = Convert.ToBoolean(settings[StringToUI[item2]]);
                                }
                            }
                        }
                        catch (Exception)
                        {}
                    }
                    break;

                case ("CheckBoxes"):
                    foreach (var item in checkBoxList)
                    {
                        try
                        {
                            //string tmpNameOfRadioButton = StringToUI[item.Name];
                            // w item mam nazwe radiobuttona i radiobutton
                            foreach (var item2 in StringToUI.Keys)
                            {
                                if (item2 == item.Name)
                                {
                                    item.IsChecked = Convert.ToBoolean(settings[StringToUI[item2]]);
                                }
                            }
                        }
                        catch (Exception)
                        { }


                    }

                    break;

                case ("ComboBox"):
                    foreach (var item in comboBoxList)
                    {
                        try
                        {
                            //string tmpNameOfRadioButton = StringToUI[item.Name];
                            // w item mam nazwe radiobuttona i radiobutton
                            foreach (var item2 in StringToUI.Keys)
                            {
                                if (item2 == item.Name)
                                {
                                    //item.Text = (settings[StringToUI[item2]]);
                                    cmbRelease.Text = settings[StringToUI[item2]];
                                    cmbRelease.Items.Refresh();
                                    sliderRelease.Value = cmbRelease.SelectedIndex;
                                }
                            }
                        }
                        catch (Exception)
                        { }
                    }


                    break;
                default:
                    break;
            }


        }

        public void initiationForprograms()
        {
            lblVersion.Content = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            User_Power = "USER";


            try
            {
                CredentialCache.DefaultNetworkCredentials.Domain = "EMEA";

                CredentialCache.DefaultNetworkCredentials.UserName = "gl_ssc_swtest";

                CredentialCache.DefaultNetworkCredentials.Password = "Start123";
                string[] fileonServer = Directory.GetFiles(@"\\demant.com\data\KBN\RnD\FS_Programs\Support_Tools\REMedy\_currentVersion"); // pobieram nazwy plikow

                if (fileOperator.checkInstanceFakeVerifit())
                {
                    btnFakeV.IsEnabled = true;
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(@"C:\Program Files (x86)\REMedy\REMedy.Launcher.exe");
                    FileVersionInfo veronserver = FileVersionInfo.GetVersionInfo(fileonServer[0]);//pobieram info o pliku 

                    if (myFileVersionInfo.FileVersion != veronserver.FileVersion)
                    {
                        try
                        {
                            string[] dd = Directory.GetFiles(@"C:\Program Files\DGS - PAZE & MIBW\Resources");
                            FileInfo nazwa = new FileInfo(dd[0]);
                            Process.Start(dd.Last(), "/uninstall /quiet ");
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Dir Error :) kiedyś naprawie :)");
                        }
                    }
                    btnFakeV.ToolTip = myFileVersionInfo.FileVersion;
                }
                else
                {
                    btnFakeV.IsEnabled = false;
                    if (!Directory.Exists(@"C:\Program Files\DGS - PAZE & MIBW\Resources"))
                    {
                        Directory.CreateDirectory(@"C:\Program Files\DGS - PAZE & MIBW\Resources");
                    }
                    FileInfo nazwa = new FileInfo(fileonServer[0]);
                    try
                    {
                        File.Copy(fileonServer[0], @"C:\Program Files\DGS - PAZE & MIBW\Resources\" + nazwa.Name);
                    }
                    catch (Exception)
                    {
                    }
                    Process.Start(fileonServer[0], "/silent /install ");
                    // uruchomic silent installera 
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }

            if (fileOperator.checkInstanceNewPreconditioner())
            {
                btnNewPrecon.IsEnabled = true;
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(@"C:\Program Files (x86)\NewPreconditioner\NewPreconditioner.exe");
                btnNewPrecon.ToolTip = myFileVersionInfo.FileVersion;
            }
            else
            {
                btnNewPrecon.IsEnabled = false;
            }


            StringToUI.Add("rbnStartwithWindows", "StartWithWindows");
            StringToUI.Add("rbnholdlogs", "HoldLogs");
            StringToUI.Add("rbnNotStartwithWindows", "NotStartWithWindows");
            StringToUI.Add("rbnDeletelogs", "NotHoldLogs");
            StringToUI.Add("rbnDefaultNormal", "InstallModeNormal");
            StringToUI.Add("rbnDefaultSilent", "InstallModeSilent");
            StringToUI.Add("rbnHI_1", "HI_1");
            StringToUI.Add("rbnHI_2", "HI_2");
            StringToUI.Add("cmbRelease", "Release");
            StringToUI.Add("rbnLight_skin", "Light_skin");
            StringToUI.Add("rbnDark_skin", "Dark_skin");
            StringToUI.Add("rbn_Genie", "Genie_skin");
            StringToUI.Add("rbn_Oasis", "Oasis_skin");
            StringToUI.Add("rbn_ExpressFit", "ExpressFit_skin");
            StringToUI.Add("rbnLogsAll_YES", "SetAll");
            StringToUI.Add("rbnLogsAll_NO", "NotSetAll");
        }

        public void checkbox(object sender, RoutedEventArgs e)
        {
            btnUpdate.IsEnabled = true;
            btnDeletelogs.IsEnabled = true;
            btnSavelogs.IsEnabled = true;
            btnFS.IsEnabled = true;
            btnHattori.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnLogMode.IsEnabled = true;
            btnAdvancelogs.IsEnabled = true;
            cmbLogMode.IsEnabled = true;
            cmbLogSettings.IsEnabled = true;
            cmbMarket.IsEnabled = true;
            btnuninstal.IsEnabled = true;

            List<string> logmod = fileOperator.getLogMode();
            List<string> ListofMarkets = new List<string>();
            for (int i = 0; i < logmod.Count; i++)
            {
                if (checkBoxList[i].IsChecked.Value)
                {
                    cmbLogMode.Text = logmod[i];
                    cmbMarket.SelectedIndex = BindCombobox.marketIndex.FindIndex(x => x == listlabelsinfoFS[i].Content.ToString());
                    ListofMarkets.Add(listlabelsinfoFS[i].Content.ToString());
                }
            }
            byte licznik = 0;
            for (int i = 1; i < ListofMarkets.Count; i++)
            {
                if (ListofMarkets[i] == ListofMarkets[i - 1])
                {
                    licznik++;
                }
            }
            if (licznik != ListofMarkets.Count - 1)
            {
                cmbMarket.SelectedIndex = -1;
            }
        }
        public void uncheckbox(object sender, RoutedEventArgs e)
        {

            byte licznikk = 0;
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    //item.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                   
                    licznikk++;
                }
                else
                {
                    //item. = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    //item.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    
                }
            }
            if (licznikk == 0)
            {
                btnUpdate.IsEnabled = false;
                btnDeletelogs.IsEnabled = false;
                btnSavelogs.IsEnabled = false;
                btnFS.IsEnabled = false;
                btnHattori.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnLogMode.IsEnabled = false;
                btnAdvancelogs.IsEnabled = false;
                cmbLogMode.IsEnabled = false;
                cmbLogSettings.IsEnabled = false;
                cmbMarket.IsEnabled = false;
                btnuninstal.IsEnabled = false;

                cmbMarket.SelectedIndex = -1;
                cmbLogMode.SelectedIndex = -1;
            }
            else
            {
                List<string> logmod = fileOperator.getLogMode();
                List<string> ListofMarkets = new List<string>();
                for (int i = 0; i < logmod.Count; i++)
                {
                    if (checkBoxList[i].IsChecked.Value)
                    {
                        cmbLogMode.Text = logmod[i];
                        cmbMarket.SelectedIndex = BindCombobox.marketIndex.FindIndex(x => x == listlabelsinfoFS[i].Content.ToString());
                        ListofMarkets.Add(listlabelsinfoFS[i].Content.ToString());
                    }
                }
                byte licznik = 0;
                for (int i = 1; i < ListofMarkets.Count; i++)
                {
                    if (ListofMarkets[i] == ListofMarkets[i - 1])
                    {
                        licznik++;
                    }
                }
                if (licznik != ListofMarkets.Count - 1)
                {
                    cmbMarket.SelectedIndex = -1;
                }
            }
        }


        public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                if (obj is T)
                    yield return obj as T;

                foreach (DependencyObject child in LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>())
                    foreach (T c in FindLogicalChildren<T>(child))
                        yield return c;
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject // funkcja wyszukujaca okreslony typ UI Label itp...
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        public void refreshUI(object sender, EventArgs e)
        {

           


            verifyInstalledBrands();
            List<string> logmodesFS = fileOperator.getLogMode();
            try
            {
                List<BuildInfo> ListBuildsInfo = new List<BuildInfo>();
                int licznik = 0;
                foreach (var item in BuildInfo.ListPathsToManInfo)
                {
                    try
                    {
                        ListBuildsInfo.Add(fileOperator.GetInfoAboutFs(item, BuildInfo.ListPathsToAboutInfo[licznik]));
                    }
                    catch (Exception)
                    {
                        ListBuildsInfo.Add(new BuildInfo("", "", "", "", ""));
                    }
                    licznik++;
                }
                for (int i = 0; i < ListBuildsInfo.Count; i++)
                {
                    ListRactanglesNames[i].ToolTip = ListBuildsInfo[i].Brand + "\n" + ListBuildsInfo[i].Version + "\n" + logmodesFS[i];
                    foreach (var item in ListRactanglesNames)
                    {
                        if (item.Name.Contains(ListBuildsInfo[i].Brand.ToLower()))
                        {

                            foreach (var obrazki in ListImages)
                            {
                                string tmppp = obrazki.Name.ToLower().Substring(3, obrazki.Name.Length - 3);

                                if (item.Name.ToLower().Contains(tmppp))
                                {
                                    try
                                    {
                                        obrazki.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + $"\\Images\\" + tmppp + ".png"));
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("Problem with images");
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }
            //for (int i = 0; i < checkBoxList.Count; i++)
            int licz = 0;
            foreach (var item in checkBoxList)
            {
                try
                {
                    //listlabelsinfoFS[licz].Foreground = new SolidColorBrush(Colors.Black);
                    listlabelsinfoFS[licz].Content = fileOperator.getMarket(licz);
                }
                catch (Exception)
                {
                    listlabelsinfoFS[licz].Foreground = new SolidColorBrush(Colors.Gray);
                    listlabelsinfoFS[licz].Content = "FS not installed";
                }
                licz++;
            }
        }

        void initializeTimers()
        {
            try
            {
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 15);
                dispatcherTimer.Start();
            }
            catch (Exception)
            {

            }

            //progressBarTimer = new DispatcherTimer();
            progress.ToolTip = "Binding builds ...";
            //progressBarTimer.Tick += makeProgress;
            //progressBarTimer.Interval = new TimeSpan(0, 0, 1);
            //progressBarTimer.Start();


            RefUiTIMER = new DispatcherTimer();
            RefUiTIMER.Tick += refreshUI;
            RefUiTIMER.Interval = new TimeSpan(0, 0, 20);
            RefUiTIMER.Start();

            try
            {
                uninstallTimer.Tick += checkUninstallation_Tick;
                uninstallTimer.Interval = new TimeSpan(0, 0, 5);
            }
            catch (Exception)
            {
            }
        }
        void bindMarketDictionary()// czy to potrzebne ?
        {

            FStoPath = new Dictionary<string, string>()
            {
                {"Oticon",@"D:\moje apki\test"},
                {"Bernafon",@"D:\moje apki\test"},
                {"Sonic",@"D:\moje apki\test"}
            };


        }

        void initializeElements()
        {
            checkBoxList = new List<CheckBox>()
            {
                Oticon,
                Bernafon,
                Sonic,
                Medical,
                Cumulus
            };

            RadioButtonsList = new List<RadioButton>()
            {
                rbnStartwithWindows,
                rbnNotStartwithWindows,
                rbnholdlogs,
                rbnDeletelogs,
                rbnDefaultNormal,
                rbnDefaultSilent,
                rbnHI_1,
                rbnHI_2,
                rbnLight_skin,
                rbnDark_skin,
                rbn_Genie,
                rbn_Oasis,
                rbn_ExpressFit,
                rbnLogsAll_YES,
                rbnLogsAll_NO,
            };
            comboBoxList = new List<ComboBox>()
            {
                cmbRelease
            };

            ListRactanglesNames = new List<Rectangle>()
            {
                oticonRectangle,
                bernafonRectangle,
                sonicRectangle,
                oticonmedicalnRectangle,
                startoRectangle
            };

            ListImages = new List<Image>()
            {
                imgOticon,
                imgBernafon,
                imgSonic,
                imgOticonMedical,
                imgStarto
            };

            listlabelsinfoFS = new List<Label>()
            {
                lblG,
                lblO,
                lblE,
                lblM,
                lblC
            };
        }

        //________________________________________________________________________________________________________________________________________________



        private void Window_Closing_1(object sender, CancelEventArgs e) // closing window by X button
        {
            //Process[] proc = Process.GetProcessesByName("Rekurencjon");
            //try
            //{
            //    proc[0].Kill(); // zamykam process rekurencja gdy zamyrefream UCH do przedyskutowania czy zostawiac ten process
            //}
            //catch (Exception)
            //{

            //}
        }



        void changeMarket(string source)
        {
            string[] oldFile;
            int counter = 0;

            try
            {
                oldFile = File.ReadAllLines(source);
                using (StreamWriter sw = new StreamWriter(source))
                {
                    foreach (var line in oldFile)
                    {
                        if (counter == 3)
                        {
                            sw.WriteLine($"  <MarketName>{cmbMarket.SelectedValue}</MarketName>");
                        }
                        else
                        {
                            sw.WriteLine(line);
                        }
                        counter++;
                    }
                }
            }
            catch (FileNotFoundException)
            { }
            catch (DirectoryNotFoundException)
            { }
            catch (NullReferenceException)
            { }
        }
        void UpdateLogModeOnUI()
        {
            List<string> mode = new List<string>() { "ALL", "DEBUG", "ERROR" };
            int numberOfChecks = 0;
            string[] selectedModes = new string[3];
            bool AreEqual = true;

            if (Oticon.IsChecked == true)
            {
                selectedModes[numberOfChecks] = GetLogMode(@"C:\Program Files (x86)\Oticon\Genie\Genie2\Configure.log4net");
                numberOfChecks++;
            }
            if (Bernafon.IsChecked == true)
            {
                selectedModes[numberOfChecks] = GetLogMode(@"C:\Program Files (x86)\Bernafon\Oasis\Oasis2\Configure.log4net");
                numberOfChecks++;
            }
            if (Sonic.IsChecked == true)
            {
                selectedModes[numberOfChecks] = GetLogMode(@"C:\Program Files (x86)\Sonic\ExpressFit\ExpressFit2\Configure.log4net");
                numberOfChecks++;
            }

            for (int i = 0; i < numberOfChecks - 1; ++i)
            {
                if (selectedModes[i] != selectedModes[i + 1])
                {
                    AreEqual = false;
                }
            }

            if (AreEqual)
            {
                cmbLogMode.SelectedIndex = mode.IndexOf(selectedModes[0]);
            }
            else
            {
                cmbLogMode.SelectedIndex = -1;
            }
        }

        string GetLogMode(string source)
        {
            string line = "";
            if (File.Exists(source))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(source))
                    {
                        for (int i = 0; i < 23; ++i)
                        {
                            sr.ReadLine();
                        }
                        line = sr.ReadLine();
                        string[] subLine = line.Split('"');
                        return subLine[1];
                    }
                }
                catch (Exception)
                {
                    return "";
                }
               
            }
            else
            {
                return "";
            }

        }



        void verifyInstalledBrands()
        {
            // if (!File.Exists(@"C:/Program Files (x86)/Oticon/Genie/Genie2/Genie.exe"))
            if (!Directory.Exists(@"C:\ProgramData\Oticon"))
            {
                Oticon.IsEnabled = false;
                lblG.Foreground = new SolidColorBrush(Colors.Red);
                lblG.Content = "FS not installed";
                Oticon.IsChecked = false;
                //oticonRectangle.Opacity = 0.3;
            }
            else
            {
                Oticon.IsEnabled = true;
                //oticonRectangle.Opacity = 1.0;
            }
            // if (!File.Exists(@"C:/Program Files (x86)/Bernafon/Oasis/Oasis2/Oasis.exe"))
            if (!Directory.Exists(@"C:\ProgramData\Bernafon"))
            {
                Bernafon.IsEnabled = false;
                lblO.Foreground = new SolidColorBrush(Colors.Red);
                lblO.Content = "FS not installed";
                Bernafon.IsChecked = false;
                //bernafonRectangle.Opacity = 0.3;
            }
            else
            {
                Bernafon.IsEnabled = true;
                //bernafonRectangle.Opacity = 1.0;
            }
            //if (!File.Exists(@"C:/Program Files (x86)/Sonic/ExpressFit/ExpressFit2/ExpressFit.exe"))
            if (!Directory.Exists(@"C:\ProgramData\Sonic"))
            {
                Sonic.IsEnabled = false;
                lblE.Foreground = new SolidColorBrush(Colors.Red);
                lblE.Content = "FS not installed";
                Sonic.IsChecked = false;
                //sonicRectangle.Opacity = 0.3;
            }
            else
            {
                Sonic.IsEnabled = true;
                //sonicRectangle.Opacity = 1.0;
            }

            if (!Directory.Exists(@"C:\ProgramData\OticonMedical")) // medical
            {
                Medical.IsEnabled = false;
                lblM.Foreground = new SolidColorBrush(Colors.Red);
                lblM.Content = "FS not installed";
                Medical.IsChecked = false;
                //oticonmedicalnRectangle.Opacity = 0.3;
            }
            else
            {
                Medical.IsEnabled = true;
                //oticonmedicalnRectangle.Opacity = 1.0;
            }

            if (!Directory.Exists(@"C:\ProgramData\Strato")) // cumulus
            {
                Cumulus.IsEnabled = false;
                lblC.Foreground = new SolidColorBrush(Colors.Red);
                lblC.Content = "FS not installed";
                Cumulus.IsChecked = false;
                //startoRectangle.Opacity = 0.3;
            }
            else
            {
                Cumulus.IsEnabled = true;
                //startoRectangle.Opacity = 1.0;
            }
        }

        bool checkRunningProcess(string name)
        {
            Process[] proc = Process.GetProcessesByName(name);
            Process[] localAll = Process.GetProcesses();

            foreach (Process item in localAll)
            {
                string tmop = item.ProcessName;
                if (tmop == name)
                {
                    return false;
                }
            }
            return true;
        }

        //void startAnimation()
        //{
        //    blinkAnimation = new DoubleAnimation
        //    {
        //        From = 1.0,
        //        To = 0.3,
        //        Duration = TimeSpan.FromSeconds(1),
        //        AutoReverse = true,
        //        RepeatBehavior = RepeatBehavior.Forever
        //    };
        //    if (Oticon.IsChecked == true)   oticonRectangle.BeginAnimation(Rectangle.OpacityProperty, blinkAnimation);
        //    if (Bernafon.IsChecked == true) bernafonRectangle.BeginAnimation(Rectangle.OpacityProperty, blinkAnimation);
        //    if (Sonic.IsChecked == true)    sonicnRectangle.BeginAnimation(Rectangle.OpacityProperty, blinkAnimation);
        //}

        //void stopAnimation()
        //{
        //    blinkAnimation = new DoubleAnimation();
        //    if (Oticon.IsChecked == false)   oticonRectangle.BeginAnimation(Rectangle.OpacityProperty, blinkAnimation);
        //    if (Bernafon.IsChecked == false) bernafonRectangle.BeginAnimation(Rectangle.OpacityProperty, blinkAnimation);
        //    if (Sonic.IsChecked == false)    sonicnRectangle.BeginAnimation(Rectangle.OpacityProperty, blinkAnimation);
        //}




        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            TrashCleaner smieciarka = new TrashCleaner();
            byte i = 0;
            byte j = 1;
            byte licznik = 0;
            bool flag = false;
            string message = "Deleted: \n";
            string message2 = "Close FS or uninstall: \n";
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    if (checkRunningProcess(item.Name) && !fileOperator.checkInstanceOfFS(licznik))
                    {
                        smieciarka.DeleteTrash(FileOperator.pathToTrash[i]);
                        smieciarka.DeleteTrash(FileOperator.pathToTrash[j]);
                        refreshUI(new object(), new EventArgs());
                        message = message + item.Name + "\n";
                        flag = true;
                    }
                    else
                    {
                        message2 = message2 + item.Name;
                    }
                }
                i += 2;
                j += 2;
                licznik++;
            }
            if (flag)
            {
                MessageBox.Show(message + message2);
            }
        }
        private void btnFS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(@"C:\Program Files (x86)\NewPreconditioner\NewPreconditioner.exe");
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }
        }
        private void btnHattori_Click(object sender, RoutedEventArgs e)
        {
            byte licznik = 0;
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    try
                    {
                        Process.Start(BuildInfo.ListPathsToHattori[licznik] + "FirmwareUpdater.exe");
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.ToString());
                    }
                }
                licznik++;
            }
            refreshUI(new object(), new EventArgs());
        }
        private void btnuninstal_Click(object sender, RoutedEventArgs e)
        {
            byte count = 0;
            string checkboxname = "";
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    count++;
                    checkboxname = item.Name;
                }
            }
            if (count > 1)
            {
                MessageBox.Show("Only one FS at a time can be uninstalled");
                return;
            }
            FSInstaller instal = new FSInstaller();

            try
            {
                instal.UninstallBrand(fileOperator.ReadPathToFsInstallator(BindCombobox.BrandtoFS[checkboxname]), RBnormal.IsChecked.Value);
            }
            catch (Exception)
            {
                MessageBox.Show("Can not be uninstalled by Ultimate Changer");
                return;
            }
            fileOperator.deleteinfoaboutinstallerpath(BindCombobox.BrandtoFS[checkboxname]); // dopisać funkcje

            /*
             1 FS na raz timer sprawdzający czy uninstall się skończył 
             gdy uninstallacja trwa uninstall i install button zablokowany

            pobranie z pliku jaka wersja FS jest zainstalowana - path
            uruchomienie procesu z path usunięcie informacji o path z pliku             
             
             */
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox checkbox in checkBoxList)
            {
                int tmp = Licznik_All_button % 2;
                if (tmp == 0)
                {
                    if (checkbox.IsEnabled == true)
                    {
                        checkbox.IsChecked = true;
                    }
                }
                else
                {
                    if (checkbox.IsEnabled == true)
                    {
                        checkbox.IsChecked = false;
                    }
                }
            }
            Licznik_All_button++;
        }
        private void cmbMarket_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }
        private void btnChange_mode_log(object sender, RoutedEventArgs e)
        {
            if (cmbLogSettings.Visibility == Visibility.Hidden)
            {
                if (txtsettlog1.Text != "" || txtsettlog2.Text != "" || txtsettlog3.Text != "")
                {
                    byte licznik = 0;
                    bool flag = false;
                    string message = "updated: \n";
                    foreach (var item in checkBoxList)
                    {
                        if (item.IsChecked.Value)
                        {
                            fileOperator.setLogMode(cmbLogMode.Text, cmbLogSettings.SelectedIndex, licznik, true, txtsettlog1.Text, txtsettlog2.Text, txtsettlog3.Text);
                            message = message + item.Name + "\n";                            
                        }
                        licznik++;
                    }
                    if (flag)
                    {
                        MessageBox.Show(message);
                    }
                }
                else
                {
                    MessageBox.Show("please add some Advance settings next time :) \n No file update");
                    return;
                }
            }
            else
            {
                if (cmbLogMode.SelectedIndex != -1 && cmbLogSettings.SelectedIndex != -1)
                {
                    byte licznik = 0;
                    bool flag = false;
                    string message = "updated: \n";
                    foreach (var item in checkBoxList)
                    {
                        if (item.IsChecked.Value)
                        {
                            fileOperator.setLogMode(cmbLogMode.Text, cmbLogSettings.SelectedIndex, licznik, false);
                            message = message + item.Name + "\n";
                        }
                        licznik++;
                    }
                    if (flag)
                    {
                        MessageBox.Show(message);
                    }
                }
                else
                {
                    MessageBox.Show("Select Log Mode / Log Settings");
                    return;
                }
            }

            try
            {
                txtsettlog1.Text = "";
                txtsettlog2.Text = "";
                txtsettlog3.Text = "";
                cmbLogSettings.Visibility = Visibility.Visible;
                txtsettlog1.Visibility = Visibility.Hidden;
                txtsettlog2.Visibility = Visibility.Hidden;
                txtsettlog3.Visibility = Visibility.Hidden;
                lblSetlog1.Visibility = Visibility.Hidden;
                lblSetlog2.Visibility = Visibility.Hidden;
                lblSetlog3.Visibility = Visibility.Hidden;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }
            refreshUI(new object(), new EventArgs());
        }
        private void btnDelete_logs(object sender, RoutedEventArgs e)
        {
            byte licznik = 0;
            TrashCleaner smieciarka = new TrashCleaner();
            bool flag = false;
            string message = "Deleted";
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    if (fileOperator.checkRunningProcess(item.Name))
                    {
                        try
                        {
                            smieciarka.DeleteTrash(fileOperator.pathToLogs[licznik]);
                            message = message + item.Name + "\n";
                            flag = true;
                        }
                        catch (Exception x)
                        {
                            MessageBox.Show(x.ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Close FS to Delete Logs");
                    }

                }
                licznik++;
            }
            if (flag)
            {
                MessageBox.Show(message);
            }

        }
        private void cmbLogMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbLogMode.SelectedIndex < 0)
            {
                btnLogMode.IsEnabled = false;
                cmbLogSettings.SelectedIndex = 0;
            }
            else
            {
                btnLogMode.IsEnabled = true;
            }
        }
        private void btninstal_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBuild.SelectedIndex > -1)
            {
                FSInstaller installer = new FSInstaller();
                //cmbOEM.Items.Refresh();
                foreach (var item in Paths_Dirs[cmbBrandstoinstall.SelectedIndex].path)
                {
                    if (item.Contains(cmbBuild.Text) && item.Contains(cmbOEM.Text))
                    {
                        installer.InstallBrand(item, RBnormal.IsChecked.Value);
                        fileOperator.SavePathToFsInstallator(item);
                        //zapisanie patha do instalatora do  pozniejszej uninstalki bez sciagania do pliku
                    }
                }
            }
            else
            {
                MessageBox.Show("select build to install");
            }
        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }
            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void cmbbrandstoinstall_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cmbBuild.ItemsSource = Paths_Dirs[cmbBrandstoinstall.SelectedIndex].dir;
                cmbBrandstoinstall.Items.Refresh();
                BindCombo.setOEMComboBox(cmbBrandstoinstall.Text);
                cmbBuild.ItemsSource = Paths_Dirs[cmbBrandstoinstall.SelectedIndex].dir;
                cmbBuild.Items.Refresh();
                cmbBrandstoinstall.Items.Refresh();
                cmbBrandstoinstall.ToolTip = FileOperator.listPathTobuilds[cmbBrandstoinstall.SelectedIndex];
                //cmbBrandstoinstall.ToolTip = FileOperator.listPathTobuilds[cmbBrandstoinstall.SelectedIndex];
            }
            catch (Exception x)
            {
                MessageBox.Show("Error FS Combo \n" + x.ToString());
            }
        }
        private void cmbbuild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBuild.SelectedIndex != -1)
            {
                btninstal.IsEnabled = true;
            }
            else
            {
                btninstal.IsEnabled = false;
            }
        }
        private void LoggingMouseEnter(object sender, MouseEventArgs e)
        {
            int border;
            Border elementLeft, elementRight, buf;
            buf = new Border();
            for (int y = 0; y < 7; ++y)
            {
                border = 11;
                for (int x = 0; x < 6; ++x)
                {
                    elementLeft = Mario.Children.Cast<Border>().FirstOrDefault(b => Grid.GetColumn(b) == x && Grid.GetRow(b) == y);
                    elementRight = Mario.Children.Cast<Border>().FirstOrDefault(b => Grid.GetColumn(b) == border && Grid.GetRow(b) == y);
                    buf.Background = elementRight.Background;
                    elementRight.Background = elementLeft.Background;
                    elementLeft.Background = buf.Background;
                    border--;
                }
            }
        }
        private void btnChangeDate_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateTime;
            if (calendar.SelectedDate.HasValue)
            {
                dateTime = calendar.SelectedDate.Value;
                clockManager.SetTime((short)dateTime.Year, (short)dateTime.Month, (short)dateTime.Day);
            }
            else
            {
                dateTime = DateTime.Now;
                clockManager.SetTime((short)dateTime.Year, (short)dateTime.Month, (short)dateTime.Day);
            }
            clockManager.DateWasSet();
        }
        private void btnHoursDown_Click(object sender, RoutedEventArgs e)
        {
            clockManager.HourDown();
            lblTime.Content = clockManager.GetTime();
            clockManager.DateWasChanged();
        }
        private void btnHoursUp_Click(object sender, RoutedEventArgs e)
        {
            clockManager.HourUp();
            lblTime.Content = clockManager.GetTime();
            clockManager.DateWasChanged();
        }
        private void btnMinutesDown_Click(object sender, RoutedEventArgs e)
        {
            clockManager.MinuteDown();
            lblTime.Content = clockManager.GetTime();
            clockManager.DateWasChanged();
        }
        private void btnMinutesUp_Click(object sender, RoutedEventArgs e)
        {
            clockManager.MinuteUp();
            lblTime.Content = clockManager.GetTime();
            clockManager.DateWasChanged();
        }
        private void btnResetDate_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("CMD.exe", "/C NET TIME /domain:EMEA /SET /Y");
            lblTime.Content = clockManager.GetTime();
        }
        private void btnLogToDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User_Power = dataBaseManager.logIn(txtNameUser.Text, passwordBox.Password.ToString());
                MessageBox.Show("done");
            }
            catch (Exception x)
            {

            }

        }
        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {
            if (User_Power == "SUPERUSER")
            {
                dataBaseManager.CreateNew(txtNameUser.Text, passwordBox.Password.ToString());
            }
            else
            {
                MessageBox.Show("Only SUPERUSER can create new Accounts");
            }
        }
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string firstHalf = cmbBuild.Text.ToString().Split(new char[] { ' ' }, 2)[0];
            //cmbBuild.ToolTip = Directory_toIntall + firstHalf;
        }
        private void textBox_TextChanged(object sender, RoutedEventArgs e)
        {
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //fileOperator.UpdateLabels();
            ////this.updateLabels();
            //this.verifyInstalledBrands();
            //this.startAnimation();
            //this.checkInstallationStatus();
            //this.TemporaryToolTipMethod();
            //set_mode_run_app();
            //CheckFileinfo2();
            //clockManager.UpdateTime();
            //lblTime.Content = clockManager.GetTime();
        }
        private void checkUninstallation_Tick(object sender, EventArgs e)
        {
        }
        public bool statusOfProcess(string name)
        {
            Process[] pname = Process.GetProcessesByName(name);
            if (pname.Length > 0) // jezeli process istnieje
            {
                return true;
            }
            else
            {
                return false; // process nie istnieje
            }
        }
        private void checkRekurencja(object sender, EventArgs e)
        {
            Process[] pname = Process.GetProcessesByName("Rekurencjon");
            progress.Value += 10;
            if (progress.Value == 100)
            {
                progress.Value = 0;
            }
            if (pname.Length == 0)
            {
                fileOperator.GetfilesSaveData();
                Rekurencja.Stop();
                cmbRelease.IsEnabled = true;
                progress.Visibility = Visibility.Hidden;
            }
        }


        private void txtCompositionPart2_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Releases_prereleases          
        }

        private void txtOEM_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetText(cmbBuild.ToolTip.ToString());
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }
        }

        private void btnFakeV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(@"C:\Program Files (x86)\REMedy\REMedy.Launcher.exe");
            }
            catch (Exception)
            {
                btnFakeV.IsEnabled = false;
            }

        }

        private void cmbOEM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbOEM.Items.Refresh();
        }

        private void btnFSRun(object sender, RoutedEventArgs e)
        {
            int licznik = 0;
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    try
                    {
                        Process.Start(BuildInfo.ListPathsToSetup[licznik]);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Probably FS is not installed! \n Please delete Trashs");
                    }
                }
                licznik++;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            fileOperator.KillFS();
        }

        private void btnSavelogs_Click(object sender, RoutedEventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.zip)|*.zip|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if ((bool)saveFileDialog1.ShowDialog())
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    //MessageBox.Show(saveFileDialog1.FileName); // to daje path do pliku nowego
                    byte licznik = 0;
                    foreach (var item in checkBoxList)
                    {
                        if (item.IsChecked.Value)
                        {
                            try
                            {
                                System.IO.Compression.ZipFile.CreateFromDirectory(fileOperator.pathToLogs[licznik], saveFileDialog1.FileName + "_" + item.Name + ".zip"); // dziala 
                            }
                            catch (IOException xx)
                            {
                                File.Delete(saveFileDialog1.FileName + "_" + item.Name + ".zip");
                                System.IO.Compression.ZipFile.CreateFromDirectory(fileOperator.pathToLogs[licznik], saveFileDialog1.FileName + "_" + item.Name + ".zip"); // dziala 
                            }
                            catch (Exception x)
                            {
                                MessageBox.Show(x.ToString());
                            }

                        }
                        licznik++;
                    }
                    myStream.Close();
                }
            }
        }

        private void Downgrade(object sender, RoutedEventArgs e)
        {
            Window downgrade = new DowngradeWindow();
            downgrade.ShowDialog();
        }
        private void txtOdp_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (txtOdp.Text != "")
            //{
            //    txtOdp.Text = "";
            //}
            //txtOdp.Text = "";
            // btnAddKnowlage.IsEnabled = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Random_HI.Magneto = !Random_HI.Magneto;
        }

        private void btnRANDHI_Click(object sender, RoutedEventArgs e)
        {
                Random rnd = new Random();
            bool flag = false;
                foreach (var item in listOfTeammembers) // przechodze po osobach z  listy i losuje im wszystko co trzeba
                {          
                    try
                    { // można poprawić żeby było mniej kodu ale to kiedys 
                        if (rbnHI_1.IsChecked.Value) // jezeli 1 HI wybrany to wyswietlic co zostalo wybrane do losowej strony i wpisac tam txtLeftHI lub txtRightHI co wybrane + zapisac w xml jaki wybór został dokonany 
                        {
                            // random HI to :

                            if (rnd.Next(2) == 0) // jezeli 1 to lewa jezeli nie to prawa 
                            {//lewa
                            RandomHIandHardware tmp = new RandomHIandHardware();

                            //string random_HI= HIs.randomHI(ListOfAvailableHIs,lblRelease.Content.ToString());

                            tmp.Name_Team_member = item;
                            List<string> tmp_hi_Types_Name = new List<string>();
                            foreach (var HI in ListBoxOfAvailableTypes.SelectedItems)
                            {
                                tmp_hi_Types_Name.Add(HI.ToString());
                            }
                            List<string> listofstyles = new List<string>();
                            foreach (var itemm in ListBoxOfAvailableStyles.SelectedItems)
                            {
                                listofstyles.Add(item.ToString());
                            }
                            HIs tmpHIL = HIs.randomHI(lblRelease.Content.ToString(), listofstyles, tmp_hi_Types_Name);
                            tmp.HIL_ = tmpHIL.Name;

                           
                            tmp.HIR_ ="N/A";

                            string wireless = "FALSE";

                            if (tmpHIL.Wireless) //jezeli ma wireless
                            {
                                wireless = "TRUE";
                            }
                            List<string> tmpListFiczurs = myXMLReader.getFiczurs();

                            if (listOfFiczursSelected.Count == 0)
                            {
                                tmp.Ficzur_ = tmpListFiczurs[MyRandomizer.Instance.Next(0, tmpListFiczurs.Count)]; //listOfFiczursSelected - wybrane ficzury
                            }
                            else
                            {

                                tmp.Ficzur_ = listOfFiczursSelected[MyRandomizer.Instance.Next(0, listOfFiczursSelected.Count)]; //listOfFiczursSelected - wybrane ficzury
                            }

                            try
                            {
                                string changed = lblWeightWireless.Content.ToString().Replace(',', '.');
                                // double tmpp = Convert.ToDouble(changed);

                                tmp.ComDev_ = myXMLReader.GetComDEV(wireless, Math.Round(sliderWeightWireless.Value));
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show($"Unable to convert to a Double : " + lblWeightWireless.Content.ToString());
                                tmp.ComDev_ = "ERROR";
                            }
                            catch (Exception)
                            {
                                tmp.ComDev_ = "ERROR";
                            }

                            listOfRandomHardawre_perPerson.Add(tmp.Name_Team_member + "," + tmp.HIL_ + "," + tmp.HIR_ + "," + tmp.Ficzur_ + "," + tmp.ComDev_);
                            GridDataRandomHardware.Items.Add(tmp);
                        }
                    
                        else
                            {//prawa
                            RandomHIandHardware tmp = new RandomHIandHardware();

                            //string random_HI= HIs.randomHI(ListOfAvailableHIs,lblRelease.Content.ToString());

                            tmp.Name_Team_member = item;
                            List<string> tmp_hi_Types_Name = new List<string>();
                            foreach (var HI in ListBoxOfAvailableTypes.SelectedItems)
                            {
                                tmp_hi_Types_Name.Add(HI.ToString());
                            }
                           
                            tmp.HIL_ = "N/A";

                            List<string> listofstyles = new List<string>();
                            foreach (var itemm in ListBoxOfAvailableStyles.SelectedItems)
                            {
                                listofstyles.Add(item.ToString());
                            }

                            HIs tmpHIR = HIs.randomHI(lblRelease.Content.ToString(), listofstyles, tmp_hi_Types_Name);
                            tmp.HIR_ = tmpHIR.Name;

                            string wireless = "FALSE";

                            if ( tmpHIR.Wireless) //jezeli ma wireless
                            {
                                wireless = "TRUE";
                            }

                            List<string> tmpListFiczurs = myXMLReader.getFiczurs();

                            if (listOfFiczursSelected.Count == 0)
                            {
                                tmp.Ficzur_ = tmpListFiczurs[MyRandomizer.Instance.Next(0, tmpListFiczurs.Count)]; //listOfFiczursSelected - wybrane ficzury
                            }
                            else
                            {

                                tmp.Ficzur_ = listOfFiczursSelected[MyRandomizer.Instance.Next(0, listOfFiczursSelected.Count)]; //listOfFiczursSelected - wybrane ficzury
                            }

                            try
                            {
                                string changed = lblWeightWireless.Content.ToString().Replace(',', '.');
                                // double tmpp = Convert.ToDouble(changed);

                                tmp.ComDev_ = myXMLReader.GetComDEV(wireless, Math.Round(sliderWeightWireless.Value));
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show($"Unable to convert to a Double : " + lblWeightWireless.Content.ToString());
                                tmp.ComDev_ = "ERROR";
                            }
                            catch (Exception)
                            {
                                tmp.ComDev_ = "ERROR";
                            }

                            listOfRandomHardawre_perPerson.Add(tmp.Name_Team_member + "," + tmp.HIL_ + "," + tmp.HIR_ + "," + tmp.Ficzur_ + "," + tmp.ComDev_);
                            GridDataRandomHardware.Items.Add(tmp);
                        }
                        }
                        else // na dwie strony
                        {
                            RandomHIandHardware tmp = new RandomHIandHardware();

                       //string random_HI= HIs.randomHI(ListOfAvailableHIs,lblRelease.Content.ToString());

                            tmp.Name_Team_member = item;
                       
                        List<string> tmp_hi_Types_Name = new List<string>();

                        
                        foreach (var HI in ListBoxOfAvailableTypes.SelectedItems)
                        {
                            tmp_hi_Types_Name.Add(HI.ToString());
                        }
                        List<string> listofstyles = new List<string>();
                        foreach (var itemm in ListBoxOfAvailableStyles.SelectedItems)
                        {
                            listofstyles.Add(itemm.ToString());
                        }

                        HIs tmpHIL = HIs.randomHI(lblRelease.Content.ToString(), listofstyles, tmp_hi_Types_Name);
                        tmp.HIL_ = tmpHIL.Name;

                        tmp.Family_Name = tmpHIL.Name_fammily;

                        List<string> listOfNames = new List<string>() { tmp.Family_Name };
                        HIs tmpHIR = HIs.randomHI(lblRelease.Content.ToString(), listOfNames, tmp_hi_Types_Name);
                        tmp.HIR_ = tmpHIR.Name;

                        string wireless = "FALSE";

                        if (tmpHIL.Wireless && tmpHIR.Wireless) //jezeli oba maja wireless
                        {
                            wireless = "TRUE";
                        }

                        List<string> tmpListFiczurs = myXMLReader.getFiczurs();

                        if (listOfFiczursSelected.Count == 0 )
                        {
                            tmp.Ficzur_ = tmpListFiczurs[MyRandomizer.Instance.Next(0, tmpListFiczurs.Count)]; //listOfFiczursSelected - wybrane ficzury
                        }
                        else
                        {

                            tmp.Ficzur_ = listOfFiczursSelected[MyRandomizer.Instance.Next(0, listOfFiczursSelected.Count)]; //listOfFiczursSelected - wybrane ficzury
                        }

                        try
                        {
                            string changed = lblWeightWireless.Content.ToString().Replace(',', '.');
                           // double tmpp = Convert.ToDouble(changed);

                            tmp.ComDev_ = myXMLReader.GetComDEV(wireless, Math.Round(sliderWeightWireless.Value)); 



                        }
                        catch (FormatException)
                        {
                           MessageBox.Show($"Unable to convert to a Double : " + lblWeightWireless.Content.ToString());
                            tmp.ComDev_ = "ERROR";
                        }
                        catch (Exception)
                        {
                            tmp.ComDev_ = "ERROR";
                        }
                        
                        listOfRandomHardawre_perPerson.Add(tmp.Name_Team_member+","+tmp.Family_Name+"," + tmp.HIL_+","+ tmp.HIR_+ "," +tmp.Ficzur_+"," + tmp.ComDev_);
                            GridDataRandomHardware.Items.Add(tmp);
                        }

                    }
                    catch (ArgumentOutOfRangeException)
                    {
                    flag = true;
                    }
                }
            if (flag)
            {
                MessageBox.Show("lack of adequate HI");
            }
        }

        private void sliderRelease_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblRelease.Content = cmbRelease.Items[Convert.ToInt32(sliderRelease.Value)];
            Random_HI.Release = lblRelease.Content.ToString();
            ListBoxOfAvailableStyles.ItemsSource = myXMLReader.GetStylesInRelease(lblRelease.Content.ToString());
        }

        private void chBt_coil_Checked(object sender, RoutedEventArgs e)
        {
            Random_HI.T_Coil = !Random_HI.T_Coil;
        }

        private void chBlED_Checked(object sender, RoutedEventArgs e)
        {
            Random_HI.Led = !Random_HI.Led;
        }

        private void chBbUTTONS_Checked(object sender, RoutedEventArgs e)
        {
            Random_HI.twoButtons = !Random_HI.twoButtons;
        }


        private void chBWireless_Checked(object sender, RoutedEventArgs e)

        {
            Random_HI.Wireless = !Random_HI.Wireless;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            Random_HI.Custom = !Random_HI.Custom;
        }

        private void CheckBox_Checked_2(object sender, RoutedEventArgs e)
        {
            Random_HI.S = !Random_HI.S;
        }

        private void ChangeSkin(Brush c1, Brush c2)
        {
            //tabControl.Background = c1;


            //Grid1.BorderBrush = c2;
            //Grid2.BorderBrush = c2;
            //Grid3.BorderBrush = c2;
            //Grid4.BorderBrush = c2;
            //Grid5.BorderBrush = c2;
        }

        private void Dark_skin_Checked(object sender, RoutedEventArgs e)
        {
            Brush c1 = new SolidColorBrush(Color.FromRgb(70, 70, 70));

            ChangeSkin(c1, c1);
            //Zmiany na ciemny motyw (można zmienić kolor ramki itd.)
            XMLReader.setSetting("Dark_skin", "RadioButtons", Convert.ToString(rbnDark_skin.IsChecked.Value).ToUpper());
            bool tmp = !rbnDark_skin.IsChecked.Value;
            XMLReader.setSetting("Light_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Genie_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Oasis_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("ExpressFit_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());

            imgBrandSkin.Visibility = Visibility.Hidden;

            foreach (var item in ListLabelsonUI)
            {
                item.Foreground = Brushes.White;
            }
            foreach (var item in ListListBoxsonUI)
            {
                item.Foreground = Brushes.White;
            }
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#FF616161");
           
            foreach (var item in ListButtonsonUI)
            {
                item.Background = brush;               
            }


        }

        private void Light_skin_Checked_1(object sender, RoutedEventArgs e)
        {
            Brush c1 = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            Brush c2 = new SolidColorBrush(Colors.LightBlue);

            ChangeSkin(c1, c2);
            //Zmiany na jasny motyw
            XMLReader.setSetting("Light_skin", "RadioButtons",Convert.ToString(rbnLight_skin.IsChecked.Value).ToUpper());
            bool tmp = !rbnLight_skin.IsChecked.Value;
            XMLReader.setSetting("Dark_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Genie_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Oasis_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("ExpressFit_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());

            imgBrandSkin.Visibility = Visibility.Hidden;

            foreach (var item in ListLabelsonUI)
            {
                item.Foreground = Brushes.Turquoise;
            }
            foreach (var item in ListListBoxsonUI)
            {
                item.Foreground = Brushes.Black;
            }
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#8A959B");

            foreach (var item in ListButtonsonUI)
            {
                item.Background = brush;
            }

        }

        private void Radio_Genie_Checked(object sender, RoutedEventArgs e)
        {
            Brush c1 = new SolidColorBrush(Color.FromRgb(183, 18, 180));
            Brush c2 = new SolidColorBrush(Colors.Black);

            ChangeSkin(c1, c2);


            XMLReader.setSetting("Genie_skin", "RadioButtons", Convert.ToString(rbn_Genie.IsChecked.Value).ToUpper());
            bool tmp = !rbn_Genie.IsChecked.Value;
            XMLReader.setSetting("Dark_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Light_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Oasis_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("ExpressFit_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());


            imgBrandSkin.Visibility = Visibility.Visible;

            imgBrandSkin.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + $"\\Images\\oticon.png"));

        }


        private void Radio_Oasis_Checked(object sender, RoutedEventArgs e)
        {
            Brush c1 = new SolidColorBrush(Color.FromRgb(183, 18, 18));
            Brush c2 = new SolidColorBrush(Colors.Black);

            ChangeSkin(c1, c2);

            XMLReader.setSetting("Oasis_skin", "RadioButtons", Convert.ToString(rbn_Oasis.IsChecked.Value).ToUpper());
            bool tmp = !rbn_Oasis.IsChecked.Value;
            XMLReader.setSetting("Dark_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Light_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Genie_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("ExpressFit_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());

            imgBrandSkin.Visibility = Visibility.Visible;

            imgBrandSkin.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + $"\\Images\\bernafon.png"));
        }

        private void Radio_ExpressFit_Checked(object sender, RoutedEventArgs e)
        {
            Brush c1 = new SolidColorBrush(Color.FromRgb(72, 196, 249));
            Brush c2 = new SolidColorBrush(Colors.Black);


            XMLReader.setSetting("ExpressFit_skin", "RadioButtons", Convert.ToString(rbn_ExpressFit.IsChecked.Value).ToUpper());
            bool tmp = !rbn_ExpressFit.IsChecked.Value;
            XMLReader.setSetting("Dark_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Light_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Genie_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Oasis_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());

            ChangeSkin(c1, c2);

            imgBrandSkin.Visibility = Visibility.Visible;

            imgBrandSkin.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + $"\\Images\\sonic.png"));
        }



        private void RBnormal_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RBsilet_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnStartwithWindows_Checked(object sender, RoutedEventArgs e)
        {
            XMLReader.setSetting("StartWithWindows", "RadioButtons", Convert.ToString(rbnStartwithWindows.IsChecked.Value));
            bool tmp = rbnStartwithWindows.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("NotStartWithWindows", "RadioButtons", Convert.ToString(tmp));
            fileOperator.setAutostart(true);


        }

        private void rbnNotStartwithWindows_Checked(object sender, RoutedEventArgs e)
        {
            XMLReader.setSetting("NotStartWithWindows", "RadioButtons", Convert.ToString(rbnNotStartwithWindows.IsChecked.Value));
            bool tmp = rbnNotStartwithWindows.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("StartWithWindows", "RadioButtons", Convert.ToString(tmp));
            fileOperator.setAutostart(false);
        }

        private void rbnholdlogs_Checked(object sender, RoutedEventArgs e)
        {
            XMLReader.setSetting("HoldLogs", "RadioButtons", Convert.ToString(rbnholdlogs.IsChecked.Value));
            bool tmp = rbnholdlogs.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("NotHoldLogs", "RadioButtons", Convert.ToString(tmp));
        }

        private void rbnDeletelogs_Checked(object sender, RoutedEventArgs e)

        {
            XMLReader.setSetting("NotHoldLogs", "RadioButtons", Convert.ToString(rbnDeletelogs.IsChecked.Value));
            bool tmp = rbnDeletelogs.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("HoldLogs", "RadioButtons", Convert.ToString(tmp));
        }

        private void rbnDefaultNormal_Checked(object sender, RoutedEventArgs e)
        {
            XMLReader.setSetting("InstallModeNormal", "RadioButtons", Convert.ToString(rbnDefaultNormal.IsChecked.Value));
            bool tmp = rbnDefaultNormal.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("InstallModeSilent", "RadioButtons", Convert.ToString(tmp));
            RBnormal.IsChecked = true;
            RBsilet.IsChecked = false;
        }

        private void rbnDefaultSilent_Checked(object sender, RoutedEventArgs e)
        {
            XMLReader.setSetting("InstallModeSilent", "RadioButtons", Convert.ToString(RBsilet.IsChecked.Value));
            bool tmp = RBsilet.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("InstallModeNormal", "RadioButtons", Convert.ToString(tmp));
            RBnormal.IsChecked = false;
            RBsilet.IsChecked = true;
        }

        private void rbnHI_1_Checked(object sender, RoutedEventArgs e)
        {
            XMLReader.setSetting("HI_1", "RadioButtons", Convert.ToString(rbnHI_1.IsChecked.Value));
            bool tmp = rbnHI_1.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("HI_2", "RadioButtons", Convert.ToString(tmp));
        }

        private void rbnHI_2_Checked(object sender, RoutedEventArgs e)
        {
            XMLReader.setSetting("HI_2", "RadioButtons", Convert.ToString(rbnHI_2.IsChecked.Value));
            bool tmp = rbnHI_2.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("HI_1", "RadioButtons", Convert.ToString(tmp));
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxHardware.SelectedIndex != -1)
            {
                txtMyItemsList.Text =  txtMyItemsList.Text +"\n"+ (MyHardware.convertToString(MyHardware.findHardwareByID(ListBoxHardware.SelectedIndex)));
                BindCombo.bindListBox();
            }
            else
            {
                MessageBox.Show("select item");
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtMyItemsList.Text = "";
            ListBoxHardware.SelectedIndex = -1;
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtMyItemsList.Text);
        }

        private void btnAddNewHardware_Click(object sender, RoutedEventArgs e)
        {
            myXMLReader.SetNewHardware(txtName.Text, txtManuf.Text, txtType.Text, txtId.Text, txtLocal.Text);
            BindCombo.bindListBox();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxHardware.SelectedIndex != -1)
            {
                myXMLReader.DeleteItem(ListBoxHardware.SelectedIndex);
                BindCombo.bindListBox();
            }
            else
            {
                MessageBox.Show("select item");
            }
        }

        private void ListBoxHardware_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxHardware.SelectedIndex!= -1)
            {
                MyHardware tmp = myXMLReader.getHardware()[ListBoxHardware.SelectedIndex];
                txtName.Text = tmp.Name;
                txtManuf.Text = tmp.Manufacturer;
                txtType.Text = tmp.Type;
                txtId.Text = tmp.ID;
                txtLocal.Text = tmp.Localization;
            }
            else
            {
                txtName.Text = "";
                txtManuf.Text = "";
                txtType.Text = "";
                txtId.Text = "";
                txtLocal.Text = "";
            }

        }

        private void btnClearFields_Click(object sender, RoutedEventArgs e)
        {
            txtName.Text = "";
            txtManuf.Text = "";
            txtType.Text = "";
            txtId.Text = "";
            txtLocal.Text = "";
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxHardware.SelectedIndex != -1)
            {
                myXMLReader.SetEditItem(ListBoxHardware.SelectedIndex, txtName.Text, txtManuf.Text, txtType.Text, txtId.Text, txtLocal.Text);
                BindCombo.bindListBox();
            }
            else
            {
                MessageBox.Show("select item");
            }
        }

        private void btnAdvancelogs_Click(object sender, RoutedEventArgs e)
        {
            if (txtsettlog1.Visibility == Visibility.Visible)
            {
                txtsettlog1.Visibility = Visibility.Hidden;
                txtsettlog2.Visibility = Visibility.Hidden;
                txtsettlog3.Visibility = Visibility.Hidden;
                lblSetlog1.Visibility = Visibility.Hidden;
                lblSetlog2.Visibility = Visibility.Hidden;
                lblSetlog3.Visibility = Visibility.Hidden;
                cmbLogSettings.SelectedIndex = -1;
                cmbLogSettings.Visibility = Visibility.Visible;
            }
            else
            {
                txtsettlog1.Visibility = Visibility.Visible;
                txtsettlog2.Visibility = Visibility.Visible;
                txtsettlog3.Visibility = Visibility.Visible;
                lblSetlog1.Visibility = Visibility.Visible;
                lblSetlog2.Visibility = Visibility.Visible;
                lblSetlog3.Visibility = Visibility.Visible;
                cmbLogSettings.SelectedIndex = -1;
                cmbLogSettings.Visibility = Visibility.Hidden;
            }

        }

        private void btnAddPersonToList_Click(object sender, RoutedEventArgs e)
        {
            if (ListTeamPerson.SelectedIndex == -1)
            {
                MessageBox.Show("Select Person");
            }
            else
            {
                List<string> tmp = new List<string>();
                string tooltip = btnClearListTeamPerson.ToolTip.ToString();
                foreach (var item in ListTeamPerson.SelectedItems)
                {
                    listOfTeammembers.Add(item.ToString());
                    tooltip = tooltip + item.ToString();
                    tooltip = tooltip + "\n";
                }

                btnClearListTeamPerson.ToolTip = tooltip;

                foreach (var persononTeam in ListTeamPerson.Items)                    
                {
                    bool flag = false;
                    foreach (var selectedPerson in listOfTeammembers)
                    {
                        if (selectedPerson.ToString() == persononTeam.ToString())
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        tmp.Add(persononTeam.ToString());
                    }
                }

                ListTeamPerson.ItemsSource = tmp;
            }
        }

        private void btnClearListTeamPerson_Click(object sender, RoutedEventArgs e)
        {
            BindCombo.bindListBox();
            listOfTeammembers.Clear();
            btnClearListTeamPerson.ToolTip = "";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (ListTeamPerson.SelectedIndex!=-1)
            {
                myXMLReader.deletePerdon(ListTeamPerson.SelectedValue.ToString());
                BindCombo.bindListBox();
            }
            else
            {
                MessageBox.Show("Select Person");
            }

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            // xmlreader nowa funkcja do zapisu nowej osoby w xml
            // refresh listteammember
            if (txtnewTeamMember.Text.Length != 0)
            {
                myXMLReader.addPerdon(txtnewTeamMember.Text.ToUpper());
                BindCombo.bindListBox();
                txtnewTeamMember.Text = "";
            }
            else
            {
                MessageBox.Show("need: NAME");
            }
        }

        private void btnClearTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GridDataRandomHardware.Items.Clear();
                listOfRandomHardawre_perPerson.Clear();
            }
            catch (Exception)
            {

     
            }

        }

        private void rbnLogsAll_YES_Checked(object sender, RoutedEventArgs e)
        {
            XMLReader.setSetting("SetAll", "RadioButtons", Convert.ToString(rbnLogsAll_YES.IsChecked.Value));
            bool tmp = rbnLogsAll_YES.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("NotSetAll", "RadioButtons", Convert.ToString(tmp));

            byte licznik = 0;
            foreach (var item in checkBoxList)
            {
                if (item.IsEnabled)
                {
                    fileOperator.setLogMode("ALL", 0, licznik, false, txtsettlog1.Text, txtsettlog2.Text, txtsettlog3.Text);
                }
                
                licznik++;
            }
        }

        private void rbnLogsAll_NO_Checked(object sender, RoutedEventArgs e)
        {
            XMLReader.setSetting("NotSetAll", "RadioButtons", Convert.ToString(rbnLogsAll_NO.IsChecked.Value));
            bool tmp = rbnLogsAll_NO.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("SetAll", "RadioButtons", Convert.ToString(tmp));
        }

        private void btnExportData_Click(object sender, RoutedEventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            bool flag = false;
            if ((bool)saveFileDialog1.ShowDialog())
            {
                //if ((myStream = saveFileDialog1.OpenFile()) != null)
                //{
                    try
                    {
                        if (saveFileDialog1.FileName.Contains(".")) //jezeli zawiera "." to zakładam że już ma wpisane csv  jezeli nie ma to wiadomo
                        {

                            if (File.Exists(saveFileDialog1.FileName))
                            {
                                List<string> allfile = File.ReadAllLines(saveFileDialog1.FileName).ToList<string>();
                                File.Delete(saveFileDialog1.FileName);

                                using (TextWriter tw = new StreamWriter(saveFileDialog1.FileName, true))
                                {

                                    foreach (var item in allfile)
                                    {
                                        tw.WriteLine(item);
                                    }

                                    tw.WriteLine(DateTime.Now.ToString());

                                    foreach (var item in listOfRandomHardawre_perPerson)
                                    {
                                        tw.WriteLine(item.ToString());
                                    }
                                    tw.Close();
                                }

                            }
                            else
                            {
                                using (TextWriter tw = new StreamWriter(saveFileDialog1.FileName, true))
                                {
                                    tw.WriteLine(DateTime.Now.ToString());

                                    foreach (var item in listOfRandomHardawre_perPerson)
                                    {
                                        tw.WriteLine(item.ToString());
                                    }
                                    tw.Close();
                                }
                            }

                        }
                        else
                        {
                           
                            string file = saveFileDialog1.FileName + ".csv";

                            using (TextWriter tw = new StreamWriter(file, true))
                            {
                                tw.WriteLine(DateTime.Now.ToString());

                                foreach (var item in listOfRandomHardawre_perPerson)
                                {
                                    tw.WriteLine(item.ToString());
                                }
                                tw.Close();
                                flag = true;
                            }


                        }                   
                        
                    }
                    catch (Exception ex)  //Writing to log has failed, send message to trace in case anyone is listening.
                    {
                        System.Diagnostics.Trace.Write(ex.ToString());
                    }
                    
                //}
            }

            try
            {
                if (flag) // jezeli robimy nowy plik czyli bez .csv
                {
                    File.Delete(saveFileDialog1.FileName);
                }
                
            }
            catch (Exception)
            {

            }

        }
        private void ListBoxOfAvailableStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                List<string> listAllSelectedTypes = new List<string>();
                foreach (var item in ListBoxOfAvailableStyles.SelectedItems)
                {
                    listAllSelectedTypes.AddRange(myXMLReader.GetTypesInStyleString(lblRelease.Content.ToString(), item.ToString()));
                }

                ListBoxOfAvailableTypes.ItemsSource = listAllSelectedTypes;
            }
            catch (Exception) // zapobieganie crashowi gdy zmieniassz release a masz wybrany Styl i Typ HI
            {
                ListBoxOfAvailableTypes.ItemsSource = null;
            }
           
        }


        private void sliderWeightWireless_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lblWeightWireless.Content =  Math.Round(sliderWeightWireless.Value,1);
            }
            catch (Exception)
            {

            }

        }


        private void ListBoxOfAvailableFeautures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listOfFiczursSelected.Clear();
            foreach (var item in ListBoxOfAvailableFeautures.SelectedItems)
            {
                listOfFiczursSelected.Add(item.ToString());
            }
            
        }

        private void ListBoxOfAvailableTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbRelease_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbRelease.Items.Refresh();
            XMLReader.setSetting("Release", "ComboBox", cmbRelease.Text);
            try
            {
                if (!Rekurencja.IsEnabled)
                {
                    progress.Visibility = Visibility.Visible;


                    try
                    {
                        if (!statusOfProcess("Rekurencjon"))
                        {
                            //Process.Start(Environment.CurrentDirectory + @"\reku" + @"\Rekurencjon.exe", cmbRelease.Text); // właczyc gdy bedzie gotowa nowa wersja 
                        }
                        Rekurencja.Start();
                    }
                    catch (Exception)
                    {

                    }

                }
            }
            catch (Exception)
            {

            }

        }




        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int licz = 0;
            string message = "updated: \n";
            bool flag = false;
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    if (!fileOperator.checkRunningProcess(BindCombobox.BrandtoFS[item.Name]))
                    {
                        if (cmbMarket.SelectedIndex != -1)
                        {
                            fileOperator.setMarket(licz, BindCombobox.marketIndex[cmbMarket.SelectedIndex]);
                            message = message + item.Name + "\n";
                            flag = true;
                        }
                        else
                        {
                            MessageBox.Show("Select Market to update");
                        }

                    }
                    else
                    {
                        MessageBox.Show($"close FS [{item.Name}]");
                    }

                }
                licz++;
            }
            if (flag)
            {
                MessageBox.Show(message);
            }
            

            refreshUI(new object(), new EventArgs());

        }
    }
    class RandomHIandHardware
    {
        public string Name_Team_member { get; set; }
        public string Family_Name { get; set; }
        public string HIL_ { get; set; }
        public string HIR_ { get; set; }
        public string Ficzur_ { get; set; }
        public string ComDev_ { get; set; }
    }

}