﻿using System;
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

[assembly: System.Reflection.AssemblyVersion("3.0.0.0")]
namespace UltimateChanger
{//
    public partial class MainWindow : Window
    {
        int Licznik_All_button = 0;

        TrashCleaner Cleaner;
        Dictionary<string,string> FStoPath;
        FileOperator fileOperator;
        DataBaseManager dataBaseManager;
        ClockManager clockManager;
       // DataBaseManager dataBaseManager;
        DispatcherTimer dispatcherTimer, progressBarTimer,RefUiTIMER,Rekurencja;
        DispatcherTimer uninstallTimer;
        BindCombobox BindCombo;
        private List<pathAndDir> paths_Dirs = new List<pathAndDir>();
        string OEMname = "";
        List<Image> ListImages;
        List<Label> listlabelsinfoFS;
        List<CheckBox> checkBoxList = new List<CheckBox>();
        List<Rectangle> ListRactanglesNames;
        BackgroundWorker worker;
        HIs Random_HI = new HIs();
        public List<List<string>> AllbuildsPerFS = new List<List<string>>();

        internal List<pathAndDir> Paths_Dirs { get => paths_Dirs; set => paths_Dirs = value; }


        // sprawdzam GITA :)
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


                fileOperator.getDataToBuildCombobox();

                initializeTimers();
                // zamiast watku napisac maly programik osobny ktory bedzie uruchamiany na timerze co 3 s i bedzie sprawdzac czy sie zakonczyl ! :D
                if (!statusOfProcess("Rekurencjon"))
                {
                    Process.Start(Environment.CurrentDirectory + @"\reku" + @"\Rekurencjon.exe", cmbRelease.Text);
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
                MessageBox.Show(x.ToString());
            }
            sliderRelease.Maximum = cmbRelease.Items.Count-1 ; // max dla slidera -1 bo count nie uwzglednia zerowego indexu
            sliderRelease.Value = cmbRelease.SelectedIndex; // ustalenie defaulta jako obecny release
            refreshUI(new object(), new EventArgs());
            dataBaseManager = new DataBaseManager();
            if (dataBaseManager != null)
            {
                dataBaseManager.getInformation_DB();
            }
        }

        //________________________________________________________________________________________________________________________________________________

            public void initiationForprograms()
            {

            try
            {
                
                imgfake.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + $"\\Images\\Fake.png"));
                imgprecon.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + $"\\Images\\Preconditioner.png"));
            }
                catch (Exception x)
                {
                    MessageBox.Show(x.ToString());
                }

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
            for (int i = 1; i < ListofMarkets.Count ; i++)
            {
                if (ListofMarkets[i] == ListofMarkets[i-1])
                {
                    licznik++; 
                }
            }
            if (licznik != ListofMarkets.Count -1 )
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
                    licznikk++;
                }
            }
            if (licznikk == 0 )
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
                for (int i = 1; i < ListofMarkets.Count ; i++)
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
                    catch (Exception x)
                    {
                        ListBuildsInfo.Add(new BuildInfo("","","","",""));
                    }
                    licznik++;
                }

                for (int i = 0; i < ListBuildsInfo.Count; i++)
                {
                    ListRactanglesNames[i].ToolTip = ListBuildsInfo[i].Brand + "\n" + ListBuildsInfo[i].Version +"\n"+ logmodesFS[i];
                    foreach (var item in ListRactanglesNames)
                    {
                        if (item.Name.Contains(ListBuildsInfo[i].Brand.ToLower()))
                        {
                          
                            foreach (var obrazki in ListImages)
                            {
                                string tmppp = obrazki.Name.ToLower().Substring(3, obrazki.Name.Length -3);

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
                    listlabelsinfoFS[licz].Foreground = new SolidColorBrush(Colors.Black);
                    listlabelsinfoFS[licz].Content = fileOperator.getMarket(licz);
                }
                catch (Exception)
                {
                    listlabelsinfoFS[licz].Foreground = new SolidColorBrush(Colors.Red);
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

        void TemporaryToolTipMethod()
        {
            List<string> brands = new List<String>()
            {
                "C:/ProgramData/Bernafon/Oasis2/ApplicationVersion.XML",
                "C:/ProgramData/Sonic/EXPRESSfit2/ApplicationVersion.XML",
                "C:/ProgramData/Oticon/Genie2/ApplicationVersion.XML"
            };
           // Image[] images = { imgBernafon, imgSonic, imgOticon_Copy };
            String[] brandely = { "Oasis", "EF", "Genie" };


        }

        void bindMarketDictionary()
        {

            FStoPath = new Dictionary<string, string>()
            {
                {"Oticon",@"D:\moje apki\test"},
                {"Bernafon",@"D:\moje apki\test"},
                {"Sonic",@"D:\moje apki\test"}
            };



            //BrandtoSoft = new Dictionary<string, string>()
            //{
            //    {"Oticon", "Genie"},
            //    {"Bernafon", "Oasis"},
            //    {"Sonic", "ExpressFit"},
            //    {"Genie", "Oticon"},
            //    {"Oasis", "Bernafon"},
            //    {"EXPRESSFit", "Sonic"},
            //    {"Genie_N", "Oticon"},
            //    {"Oasis_N", "Bernafon"},
            //    {"EXPRESSfit_N", "Sonic"}
            //};

            //cmbMarket.ItemsSource = market;
            //cmbMarket.DisplayMemberPath = "Key";
            //cmbMarket.SelectedValuePath = "Value";
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

            ListRactanglesNames = new List<Rectangle>()
            {
                oticonRectangle,
                bernafonRectangle,
                sonicRectangle,  // dodac medical i cumulus
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

     
   


        //void getNamesInstallationFolders(string DirectoryName)
        //{
        //    try
        //    {
        //        System.IO.DirectoryInfo di = new DirectoryInfo(DirectoryName);
        //        nameFolders = di.EnumerateDirectories().ToArray();
        //    }
        //    catch (Exception)
        //    {
        //        //MessageBox.Show("directody doesnt exist");
        //    }

        //}

        //public bool IsDirectoryEmpty(string path)
        //{
        //    return !Directory.EnumerateFileSystemEntries(path).Any();
        //}

  

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
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
            catch (FileNotFoundException ex)
            {
               
            }
            catch (DirectoryNotFoundException ee)
            {
                
            }
            catch (NullReferenceException e)
            {
               
            }
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

            for (int i=0; i<numberOfChecks-1; ++i)
            {
                if (selectedModes[i] != selectedModes[i+1])
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
                catch (Exception ee)
                {
                   
                    return "";
                }
            }
            else
            {
                return "";
            }

        }

        //void SetLogSettings(bool mode) // powinno dzialac // ustawienia od TOOJ co do logow FS cos z tym ze same sie usuwaja po ponownym wlaczeniu 
        //{
        //    string[] oldFile;
        //    int counter = 0;
        //    string source;
        //    int count3 = 0;
        //    foreach (CheckBox checkbox in checkBoxList)
        //    {
        //        if ((bool)checkbox.IsEnabled)
        //        {
        //            try
        //            {
        //                source = $"C:/Program Files (x86)/{checkbox.Name}/{marki[count3]}/{marki[count3]}{"2"}/Configure.log4net";
        //                count3++;
        //                oldFile = File.ReadAllLines(source);
        //                using (StreamWriter sw = new StreamWriter(source))
        //                {
        //                    foreach (var line in oldFile)
        //                    {
        //                        if (mode)
        //                        {
        //                            if (counter == 36)
        //                            {
        //                                sw.WriteLine($"      <appendToFile value=\"{false.ToString().ToLower()}\"/>");
        //                            }
        //                            if (counter == 38)
        //                            {
        //                                sw.WriteLine($"      <staticLogFileName value=\"{false.ToString().ToLower()}\"/>");
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (counter == 36)
        //                            {
        //                                sw.WriteLine($"      <appendToFile value=\"{true.ToString().ToLower()}\"/>");
        //                            }
        //                            if (counter == 38)
        //                            {
        //                                sw.WriteLine($"      <staticLogFileName value=\"{true.ToString().ToLower()}\"/>");
        //                            }
        //                        }



        //                        if (counter != 36 && counter != 38)
        //                        {
        //                            sw.WriteLine(line);
        //                        }
        //                        counter++;
        //                    }
        //                }

        //            }
        //            catch (Exception ee)
        //            {
        //                dataBaseManager.LogException(ee.ToString(), "SetLogSettings ");
        //            }
        //        }

        //    }


 

        //}
        
     
        

        //bool verifyInstanceOfExec(string name)
        //{
        //    foreach (CheckBox checkbox in checkBoxList)
        //    {
        //        if (checkbox.Name == name)
        //        {     
        //            if (File.Exists($"C:/Program Files (x86)/{name}/{BrandtoSoft[checkbox.Name]}/{BrandtoSoft[checkbox.Name]}2/{BrandtoSoft[checkbox.Name]}.exe"))
        //            {
        //                return true;
        //            }
        //            else return false;
        //        }
        //    }
        //    return false;
        //}
        
        //[Obsolete]
        void verifyInstalledBrands()
        {
           // if (!File.Exists(@"C:/Program Files (x86)/Oticon/Genie/Genie2/Genie.exe"))
           if (!Directory.Exists(@"C:\ProgramData\Oticon"))
            {
                Oticon.IsEnabled = false;
                lblG.Foreground = new SolidColorBrush(Colors.Red);
                lblG.Content = "FS not installed";
                Oticon.IsChecked = false;
                oticonRectangle.Opacity = 0.3;
            }
            else
            {
                Oticon.IsEnabled = true;
                oticonRectangle.Opacity = 1.0;
            }
            // if (!File.Exists(@"C:/Program Files (x86)/Bernafon/Oasis/Oasis2/Oasis.exe"))
            if (!Directory.Exists(@"C:\ProgramData\Bernafon"))
            {
                Bernafon.IsEnabled = false;
                lblO.Foreground = new SolidColorBrush(Colors.Red);
                lblO.Content = "FS not installed";
                Bernafon.IsChecked = false;
                bernafonRectangle.Opacity = 0.3;
            }
            else
            {
                Bernafon.IsEnabled = true;
                bernafonRectangle.Opacity = 1.0;
            }
            //if (!File.Exists(@"C:/Program Files (x86)/Sonic/ExpressFit/ExpressFit2/ExpressFit.exe"))
            if (!Directory.Exists(@"C:\ProgramData\Sonic"))
            {
                Sonic.IsEnabled = false;
                lblE.Foreground = new SolidColorBrush(Colors.Red);
                lblE.Content = "FS not installed";
                Sonic.IsChecked = false;
                sonicRectangle.Opacity = 0.3;
            }
            else
            {
                Sonic.IsEnabled = true;
                sonicRectangle.Opacity = 1.0;
            }

            if (!Directory.Exists(@"C:\ProgramData\OticonMedical")) // medical
            {
                Medical.IsEnabled = false;
                lblM.Foreground = new SolidColorBrush(Colors.Red);
                lblM.Content = "FS not installed";
                Medical.IsChecked = false;
                oticonmedicalnRectangle.Opacity = 0.3;
            }
            else
            {
                Medical.IsEnabled = true;
                oticonmedicalnRectangle.Opacity = 1.0;
            }


            if (!Directory.Exists(@"C:\ProgramData\Strato")) // cumulus
            {
                Cumulus.IsEnabled = false;
                lblC.Foreground = new SolidColorBrush(Colors.Red);
                lblC.Content = "FS not installed";
                Cumulus.IsChecked = false;
                startoRectangle.Opacity = 0.3;
            }
            else
            {
                Cumulus.IsEnabled = true;
                startoRectangle.Opacity = 1.0;
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

        //private void btnUpdate_Click(object sender, RoutedEventArgs e)
        //{
        //    bool message = false;
        //    int count3 = 0;
        //    foreach (CheckBox checkbox in checkBoxList)
        //    {
        //        if ((bool)checkbox.IsChecked)
        //        {
        //            if (checkRunningProcess(marki[count3]))
        //            {
        //                changeMarket($"C:/ProgramData/{checkbox.Name}/Common/ManufacturerInfo.XML");
        //            }
        //            else
        //            {
        //                message = true;
        //            }
        //        }
        //        count3++;
        //    }
        //    if (message)
        //    {
        //        MessageBox.Show("Close fitting software", "Brand", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //    fileOperator.UpdateLabels();
        //    verifyInstalledBrands();

        //    clickCounter.AddClick((int)Buttons.UpdateMarket);
        //}

        //void deleteLogs()
        //{
        //    foreach (CheckBox checkbox in checkBoxList)
        //    {
        //        int brandCounter = 0;
        //        if ((bool)checkbox.IsChecked) //analiza => jeden zaznaczony dwa nie 
        //        {
        //            if (checkRunningProcess(marki[brandCounter]))
        //            {
        //                Cleaner.DeleteLogs(checkbox.Name.ToString());
        //                MessageBox.Show($" Deleted logs for {checkbox.Name}");
        //            }
        //            else
        //            {
        //                MessageBox.Show("Close fitting software", "Brand", MessageBoxButton.OK, MessageBoxImage.Warning);
        //            }
        //        }
        //        brandCounter++;
        //    }
        //}

      
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            TrashCleaner smieciarka = new TrashCleaner();
            byte i = 0;
            byte j = 1;
            byte licznik = 0;
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    if (checkRunningProcess(item.Name) && !fileOperator.checkInstanceOfFS(licznik))
                    {
                        smieciarka.DeleteTrash(FileOperator.pathToTrash[i]);
                        smieciarka.DeleteTrash(FileOperator.pathToTrash[j]);
                        refreshUI(new object(),new EventArgs());
                        MessageBox.Show(item.Name  + " Deleted");

                    }
                    else
                    {
                        MessageBox.Show("Close FS or uninstall");
                    }
                   
                }
                i += 2;
                j += 2;
                licznik++;
            }

        }

        private void btnFS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(@"C:\Program Files (x86)\NewPreconditioner\NewPreconditioner.exe");
            }
            catch (Exception x )
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
            if (count>1)
            {
                MessageBox.Show("Only 1 FS could be uninstalled");
                return;
            }

            

            FSInstaller instal = new FSInstaller();

            try
            {
                instal.UninstallBrand(fileOperator.ReadPathToFsInstallator(BindCombobox.BrandtoFS[checkboxname]), RBnormal.IsChecked.Value);
            }
            catch (Exception)
            {
                MessageBox.Show("can not uninstall by Ultimate Changer");
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
                    if (tmp == 0) {
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
                    foreach (var item in checkBoxList)
                    {
                        if (item.IsChecked.Value)
                        {
                            fileOperator.setLogMode(cmbLogMode.Text, cmbLogSettings.SelectedIndex, licznik,true, txtsettlog1.Text, txtsettlog2.Text, txtsettlog3.Text);
                            MessageBox.Show($"Updated [{item.Name}]");
                        }

                        licznik++;
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
                    foreach (var item in checkBoxList)
                    {
                        if (item.IsChecked.Value)
                        {
                            fileOperator.setLogMode(cmbLogMode.Text, cmbLogSettings.SelectedIndex, licznik,false);
                            MessageBox.Show($"Updated [{item.Name}]");
                        }
                        
                        licznik++;
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
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value && fileOperator.checkRunningProcess(item.Name))
                {
                    try
                    {
                        smieciarka.DeleteTrash(fileOperator.pathToLogs[licznik]);
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.ToString());
                    } 
                }
                licznik++;
            }
            MessageBox.Show("Deleted");
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
            if ( cmbBuild.SelectedIndex > -1 )
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
            for (int y=0; y<7; ++y)
            {
                border = 11;
                for (int x=0; x<6; ++x)
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
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string firstHalf = cmbBuild.Text.ToString().Split(new char[] { ' ' }, 2)[0];
            //cmbBuild.ToolTip = Directory_toIntall + firstHalf;
        }

        private void textBox_TextChanged(object sender, RoutedEventArgs e)
        {
        }



        private void makeProgress(object sender, EventArgs e)
        {
            int count = cmbBrandstoinstall.Items.Count;
            List<pathAndDir> tmplistapathdir = new List<pathAndDir>();
            Console.WriteLine("watek sobie dziala :)");
            bool warunek = true;
            while (warunek)
            {
                this.Dispatcher.Invoke((Action)delegate ()  //nie mam pojecia o co tu chodzi
                {
                    this.progress.Value += 10;
                });

                //List<pathAndDir> tmp = new List<pathAndDir>();
                try
                {
                    
                    this.Dispatcher.Invoke((Action)delegate ()  //nie mam pojecia o co tu chodzi
                    {
                        tmplistapathdir = this.fileOperator.getAllDirPath(cmbRelease.Text);
                        if (worker.IsBusy)
                        {

                            this.progress.Value += 10;
                        }
                        else
                            this.progress.Value += 10;

                    });
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.ToString());
                }



                this.Dispatcher.Invoke((Action)delegate ()  //nie mam pojecia o co tu chodzi
                {
                    if (this.progress.Value >= 100)
                    {

                        fileOperator.Savebuildsinfo();

                        this.Dispatcher.Invoke((Action)delegate ()  //nie mam pojecia o co tu chodzi
                        {
                            this.progress.Value = 0;
                            this.progress.Visibility = Visibility.Hidden;
                        });

                        if (worker.IsBusy)
                        {
                            worker.WorkerSupportsCancellation = true;
                            this.Paths_Dirs = tmplistapathdir;
                            this.fileOperator.lista = tmplistapathdir;
                            worker.CancelAsync();

                            fileOperator.Savebuildsinfo();
                            Console.WriteLine("WATEK UMARL");
                            warunek = false;
                        }


                    }
                    else
                    {
                        this.progress.Value += 10;
                    }
                });
            }

            
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
                //Thread.Sleep(1000);
                fileOperator.GetfilesSaveData();
                Rekurencja.Stop();
                cmbRelease.IsEnabled = true;
                progress.Visibility = Visibility.Hidden;
                
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

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

            if ((bool)saveFileDialog1.ShowDialog() )
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
                                System.IO.Compression.ZipFile.CreateFromDirectory(fileOperator.pathToLogs[licznik], saveFileDialog1.FileName+"_"+item.Name+".zip"); // dziala 
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
            dataBaseManager.getHI(Random_HI.T_Coil, Random_HI.Led, Random_HI.twoButtons, Random_HI.Wireless, Random_HI.Custom, Random_HI.S, Random_HI.Magneto, Random_HI.Release);
        }

        private void sliderRelease_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        { 
                lblRelease.Content = cmbRelease.Items[Convert.ToInt32(sliderRelease.Value)];
                Random_HI.Release = lblRelease.Content.ToString();           

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

        private void Dark_skin_Checked(object sender, RoutedEventArgs e)
        {
            tabControl.Background = new SolidColorBrush(Color.FromRgb(70, 70, 70));
            //Zmiany na ciemny motyw (można zmienić kolor ramki itd.)
        }

        private void Light_skin_Checked_1(object sender, RoutedEventArgs e)
        {
            tabControl.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            //Zmiany na jasny motyw
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

        private void cmbRelease_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbRelease.Items.Refresh();
            try
            {
                if (!Rekurencja.IsEnabled)
                {
                    progress.Visibility = Visibility.Visible;
                    

                    try
                    {
                        if (!statusOfProcess("Rekurencjon"))
                        {
                            Process.Start(Environment.CurrentDirectory + @"\reku" + @"\Rekurencjon.exe", cmbRelease.Text);
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
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    if (!fileOperator.checkRunningProcess(BindCombobox.BrandtoFS[item.Name]))
                    {
                        if (cmbMarket.SelectedIndex != -1)
                        {
                            fileOperator.setMarket(licz, BindCombobox.marketIndex[cmbMarket.SelectedIndex]);
                            MessageBox.Show($"updated [{item.Name}]");
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
            
            refreshUI(new object(), new EventArgs());

        }
    }

}