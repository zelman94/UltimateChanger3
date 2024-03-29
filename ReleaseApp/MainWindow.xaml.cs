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
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.Net;
using System.Data;
using Rekurencjon; // logi
using log4net;

[assembly: System.Reflection.AssemblyVersion("4.0.0.0")]
namespace UltimateChanger
{//
    public partial class MainWindow : Window 
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        int Licznik_All_button = 0;
        FileOperator fileOperator;
        public DataBaseManager dataBaseManager;
        ClockManager clockManager;
        // DataBaseManager dataBaseManager;
        DispatcherTimer RefUiTIMER, CopyTimer;
        DispatcherTimer ConnectionToDBTimer;
        public DispatcherTimer uninstallTimer, checkUpdate, InstallTimer, InstallTimer_Normal_Installation,
             checkTime_Timer, // czy juz czas na upgrade FS
             silentUninstal_Install_Timer, //silentUninstal_Install_Timer - timer do sprawdzenia czy uninstallacja sie skonczyla 
             progressHI_Timer;
        BindCombobox BindCombo;
        private List<pathAndDir> paths_Dirs = new List<pathAndDir>();
        //string OEMname = "";
        List<Label> listlabelsinfoFS, listlabelsinfoFS_Version;  
        List<CheckBox> checkBoxList = new List<CheckBox>();
        List<ComboBox> comboBoxList = new List<ComboBox>();
        List<TextBox> textBoxList = new List<TextBox>();
        int savedTime ; // to bind => lblSavedTime
        ClickCounter CounterOfclicks = new ClickCounter(10);


        List<Button> buttonListForUi = new List<Button>();
        List<Button> ListFSButtons = new List<Button>();

        List<Label> lableListForUi = new List<Label>();
        List<Label> labelListsforRefreshUI = new List<Label>();
        List<Label> labelListsforUninstall = new List<Label>() ; // lista zaznaczonych do usuniecia FS
        List<ListBox> listBoxForUi = new List<ListBox>();
        public List<string> listOfPathsToInstall = new List<string>(); // lista do zapisania pathów do instalatorow sillent install !
        List<Image> imagesListWarning = new List<Image>();

        List<CheckBox> checkBoxListForUi = new List<CheckBox>();
        List<ComboBox> comboBoxListForUi = new List<ComboBox>();
        List<RadioButton> radioButtonListForUi = new List<RadioButton>();
        List<TextBox> texBoxListForUi = new List<TextBox>();
        List<Border> borderListForUi = new List<Border>();
        List<Slider> sliderListForUi = new List<Slider>();
        List<CheckBox> CheckBoxNightList = new List<CheckBox>();

        List<string> listOfTeammembers = new List<string>();
        public List<FittingSoftware> listGlobalPathsToUninstall = new List<FittingSoftware>();
        List<string> listOfFiczursSelected = new List<string>();
        List<string> listOfRandomHardawre_perPerson = new List<string>();
        List<RadioButton> RadioButtonsList = new List<RadioButton>();
        public SortedDictionary<string, string> StringToUI = new SortedDictionary<string, string>(); // slownik do zamiany stringow z xml do wartości UI 
        //BackgroundWorker worker;
        HIs Random_HI = new HIs();
        myXMLReader XMLReader = new myXMLReader();
        public List<List<string>> AllbuildsPerFS = new List<List<string>>();
        internal List<pathAndDir> Paths_Dirs { get => paths_Dirs; set => paths_Dirs = value; }
        public DateTime Time_now { get; private set; }

        List<string> AllOemPaths = new List<string>();

        public List<string> RandomHardware;

        public string Advance_1 = "", Advance_2 = "", Advance_3 = "";

       public List<FittingSoftware> FittingSoftware_List = new List<FittingSoftware>();
        public string pathToCopyOfComposition = "";
        Task task_checkValidation;
        DispatcherTimer timer_checkValidation;
        List<int> list_checkValidation = new List<int>(); // 0 - uptodate / 1 - old / 2 - IP

        public NightlyUpgradeManager nightlyUpgradeManager = null;

        public MainWindow()
        {            
            InitializeComponent();
            fileOperator = new FileOperator();
            
             dataBaseManager = new DataBaseManager(); // tam jest wątek
            try
            {
                
                var exists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
                if (exists) // jezeli wiecej niz 1 instancja to nie uruchomi sie
                {
                    System.Environment.Exit(1);
                }

                if (FileOperator.getCountUCRun() == "0")
                {
                    //wersja apki
                    string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    Window ChangeLogWindow = new ChangeLog(dataBaseManager, version);
                    ChangeLogWindow.ShowDialog();

                }

                clockManager = new ClockManager();
                BindCombo = new BindCombobox();
               

                try
                {
                    przegladarka.Navigate("http://confluence.kitenet.com/display/SWSQA/Ultimate+Changer");
                }
                catch (Exception)
                {

                }
             
                initializeElements();                
                initiationForprograms();               
                initializeTimers();

                try
                {
                    foreach (Label tb in FindLogicalChildren<Label>(this)) // dziala
                    {
                        lableListForUi.Add(tb);
                    }
                    foreach (ListBox item in FindLogicalChildren<ListBox>(this))
                    {
                        listBoxForUi.Add(item);
                    }

                    foreach (Image item in FindLogicalChildren<Image>(this))
                    {
                        if (item.Name.Contains("Warning"))
                        {
                            imagesListWarning.Add(item);
                        }
                    }

                    foreach (Button item in FindLogicalChildren<Button>(this))
                    {
                        if (!item.Name.Contains("Image")) // jezeli nie jest bo button od FS 
                        {
                            buttonListForUi.Add(item);
                        }
                        else
                        {
                            ListFSButtons.Add(item);
                        }
                        
                    }
                    
                    foreach (CheckBox item in FindLogicalChildren<CheckBox>(this))
                    {
                        checkBoxListForUi.Add(item);

                        if (item.Name.Contains("_Night"))
                        {
                            CheckBoxNightList.Add(item);
                        }

                    }
                    foreach (ComboBox item in FindLogicalChildren<ComboBox>(this))
                    {
                        comboBoxListForUi.Add(item);
                    }
                    foreach (RadioButton item in FindLogicalChildren<RadioButton>(this))
                    {
                        radioButtonListForUi.Add(item);
                    }
                    foreach (TextBox item in FindLogicalChildren<TextBox>(this))
                    {
                        texBoxListForUi.Add(item);
                    }
                    foreach (Slider item in FindLogicalChildren<Slider>(this))
                    {
                        sliderListForUi.Add(item);
                    }
                    foreach (Border item in FindLogicalChildren<Border>(this))
                    {
                        borderListForUi.Add(item);
                    }
                }
                catch (Exception xc)
                {
                    MessageBox.Show($"error MainWindow \n {xc.ToString()}");
                }

                labelListsforRefreshUI.Add(lblG);
                labelListsforRefreshUI.Add(lblM);
                labelListsforRefreshUI.Add(lblO);
                labelListsforRefreshUI.Add(lblE);
                labelListsforRefreshUI.Add(lblC);

                // napisac funkcje w fileoperation na pobieranie zapisanych danych z pliku i wpisanie do PathDir lista czy cos 
                /*refreshUI(); */// funkcja  caly ui
                //fileOperator.GetInfoAboutFs(@"C:\ProgramData\Bernafon\Common\ManufacturerInfo.xml"); // dziala 
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
                MessageBox.Show("inicjalizacja \n" + x.ToString());
            }

            btnIdentify.Visibility = Visibility.Hidden;

            List<MenuItem> menuitems = new List<MenuItem>();

            FittingSoftware_List.Add(new FittingSoftware("Genie"));
            FittingSoftware_List.Add(new FittingSoftware("GenieMedical"));
            FittingSoftware_List.Add(new FittingSoftware("ExpressFit"));
            FittingSoftware_List.Add(new FittingSoftware("HearSuite"));
            FittingSoftware_List.Add(new FittingSoftware("Oasis"));
            FittingSoftware_List.Add(new FittingSoftware("Genie",true));
            FittingSoftware_List.Add(new FittingSoftware("GenieMedical", true));
            FittingSoftware_List.Add(new FittingSoftware("ExpressFit", true));
            FittingSoftware_List.Add(new FittingSoftware("HearSuite",true));
            FittingSoftware_List.Add(new FittingSoftware("Oasis",true));

            // ------- uruchomienie sprawdzania w tasku 
            // do l isty bool 5 elementowej dodawac wyniki z checkValidation, 
            // uruchomic timer sprawdzajacy czy task się zakończył, jezeli tak to przejsc po 
            // liscie i uruchomic obrazki

            task_checkValidation = Task.Run(() => { // watek 

                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        list_checkValidation.Add(FittingSoftware_List[i].checkValidationFS());
                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.ToString());
                }
                           

            });
            timer_checkValidation.Start(); // rozpoczynam sprawdanie czy task sie skonczyl

            //----------------------------------




            savedTime = Convert.ToInt32(fileOperator.getSavedTime());
            setNewSavedTime(0);
            tabControl.IsEnabled = true;

            try
            {
                BindCombo.setFScomboBox();
                BindCombo.setReleaseComboBox();
                BindCombo.setMarketCmb();
                BindCombo.bindlogmode();
                BindCombo.bindListBox();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }


            try
            {
                sliderRelease.Maximum = cmbRelease.Items.Count - 1; // max dla slidera -1 bo count nie uwzglednia zerowego indexu
                sliderWeightWireless.Maximum = 1;
                sliderRelease.Value = cmbRelease.SelectedIndex; // ustalenie defaulta jako obecny release
                

                sliderWeightWireless.Value = 0.5; // to oznacza ze nic nie zmieniam i wszystko jes po rowno w szansach 
                lblWeightWireless.Content = sliderWeightWireless.Value.ToString();
                cmbLogSettings_Compo.SelectedIndex = 0;

                ListBoxOfAvailableFeautures.SelectionMode = SelectionMode.Multiple;

                ListBoxOfAvailableStyles.SelectionMode = SelectionMode.Multiple;
                ListBoxOfAvailableTypes.SelectionMode = SelectionMode.Multiple;

            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
                MessageBox.Show("inicjalizacja part 2 \n" + x.ToString());
            }

            try
            {

                setUIdefaults(XMLReader.getDefaultSettings("RadioButtons"), "RadioButtons");
                setUIdefaults(XMLReader.getDefaultSettings("CheckBoxes"), "CheckBoxes");
                setUIdefaults(XMLReader.getDefaultSettings("ComboBox"), "ComboBox");
                setUIdefaults(XMLReader.getDefaultSettings("TextBox"), "TextBox");
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
                MessageBox.Show(x.ToString());
            }

            //-- DSZY LOSU LOSU--//
            listboxTeam.SelectionMode = SelectionMode.Multiple;
            Refresh();
            //--------------------//

            refreshUI(new object(), new EventArgs());
            R_Day.Visibility = Visibility.Hidden;
            Log.Info("Main created");
            fileOperator.checkVersion();

        }
        //________________________________________________________________________________________________________________________________________________
        FSInstaller instal = new FSInstaller();

        private void View_OnClick_Context_Uninstall(object sender, RoutedEventArgs e)
        {
            var clickedMenuItem = sender as MenuItem;
            var menuText = clickedMenuItem.Uid;


            if (TabFull.IsSelected)
            {
                if (FittingSoftware_List[Convert.ToInt32(menuText)].Path_Local_Installer == "")
                {
                    FittingSoftware_List[Convert.ToInt32(menuText)].findUnInstaller();
                }               

                instal.UninstallBrand( FittingSoftware_List[Convert.ToInt32(menuText)], true);
                InstallTimer_Normal_Installation.Start();
            }
            else
            {
                // usuwanie kompozycji
            }
            setNewSavedTime(15);

        }


        private void View_OnClick_Context_Change_LogLevel(object sender, RoutedEventArgs e)
        {
            var clickedMenuItem = sender as MenuItem;
            var ID = clickedMenuItem.Uid; // info jaki poziom logowania (na razie tylko 0 - ALL)
            var parent = clickedMenuItem.Parent as MenuItem;
            var parent_ID = parent.Uid; // informacja ktory brand


            if (TabFull.IsSelected)
            {
                FittingSoftware_List[Convert.ToInt32(parent_ID)].setLogMode("ALL", 0, TabFull.IsSelected);
            }
            else
            {
                FittingSoftware_List[Convert.ToInt32(parent_ID)+5].setLogMode("ALL", 0, TabFull.IsSelected);
            }
            setNewSavedTime(15);
        }

        
        private void View_OnClick_Context_Emulator(object sender, RoutedEventArgs e)
        {
            var clickedMenuItem = sender as MenuItem;
            var menuText = clickedMenuItem.Uid;
            if (TabFull.IsSelected)
            {
                // FittingSoftware_List[Convert.ToInt32(menuText)].StartEmulator();
                FittingSoftware_List[Convert.ToInt32(menuText) ].StartEmulator();
            }
            else
            {
                FittingSoftware_List[Convert.ToInt32(menuText) + 5].StartEmulator();
            }
            setNewSavedTime(15);
        }

        private void View_OnClick_Context_Edit(object sender, RoutedEventArgs e)
            {
            Window EditFittingSoftware = null;
            var clickedMenuItem = sender as MenuItem;
            var menuText = clickedMenuItem.Uid;

            if (TabFull.IsSelected)
            {
                EditFittingSoftware = new EditFittingSoftware(FittingSoftware_List[Convert.ToInt32(menuText)]);
            }
            else
            {
                EditFittingSoftware = new EditFittingSoftware(FittingSoftware_List[Convert.ToInt32(menuText)+5]);
            }
            EditFittingSoftware.Owner = this;
            EditFittingSoftware.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            EditFittingSoftware.ShowDialog();
            setNewSavedTime(15);
        }


        private void View_OnClick_Context_Delete_Trash(object sender, RoutedEventArgs e)
        {
            var clickedMenuItem = sender as MenuItem;
            var menuText = clickedMenuItem.Uid;
            if (TabFull.IsSelected)
            {
                FittingSoftware_List[Convert.ToInt32(menuText)].deleteTrash();
            }
            else
            {
                FittingSoftware_List[Convert.ToInt32(menuText)+5].deleteTrash();
            }
                       
            refreshUI(new object(), new EventArgs());
            setNewSavedTime(20);
        }

        private void View_OnClick_Context_DeleteLogs(object sender, RoutedEventArgs e)
        {
            var clickedMenuItem = sender as MenuItem;
            var menuText = clickedMenuItem.Uid;
            if (TabFull.IsSelected)
            {
                FittingSoftware_List[Convert.ToInt32(menuText)].deleteLogs();
            }
            else
            {
                FittingSoftware_List[Convert.ToInt32(menuText) + 5].deleteLogs();
            }
            setNewSavedTime(20);
        }
        

        private void View_OnClick_Context_Change_Market_US(object sender, RoutedEventArgs e)
        {
            var clickedMenuItem = sender as MenuItem;
            var menuText = clickedMenuItem.Uid;

            if (TabFull.IsSelected)
            {
                FittingSoftware_List[Convert.ToInt32(menuText)].setMarket("US");
            }
            else
            {
                FittingSoftware_List[Convert.ToInt32(menuText)+5].setMarket("US");
            }
            
            refreshUI(new object(), new EventArgs());
        }
        private void View_OnClick_Context_Change_Market(object sender, RoutedEventArgs e)
        {
            Window EditMarket = null;
            var clickedMenuItem = sender as MenuItem;
            var menuText = clickedMenuItem.Uid;
            if (TabFull.IsSelected)
            {
                EditMarket = new Edit_Market(FittingSoftware_List[Convert.ToInt32(menuText)]);
            }
            else
            {
                EditMarket = new Edit_Market(FittingSoftware_List[Convert.ToInt32(menuText)+5]);
            }    
            EditMarket.Owner = this;
            EditMarket.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            EditMarket.ShowDialog();
        }
        private void View_OnClick_Context_Release(object sender, RoutedEventArgs e)
        {
          
            var clickedMenuItem = sender as MenuItem;
            var menuText = clickedMenuItem.Uid;
            myXMLReader myXML = new myXMLReader();
            if (TabFull.IsSelected)
            {
                //menuText - 0 - set default
                //menuText - 1 - add release
                if (Convert.ToInt32(menuText) == 0)
                {
                    myXML.setSetting("Release", "ComboBox", cmbRelease.Text);
                }
                else
                {
                    Window add = new AddRelease();
                    add.Owner = this;
                    add.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    add.ShowDialog();
                    BindCombo.setReleaseComboBox();
                }

            }
            else
            {

            }

        }

        


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
                        catch (Exception x)
                        {
                            Log.Debug(x.ToString());
                        }
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
                        catch (Exception x)
                        {
                            Log.Debug(x.ToString());
                        }


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
                                    try
                                    {
                                        cmbRelease_Nightly.Text = settings[StringToUI[item2]];
                                        cmbRelease_Nightly.SelectedIndex = 1;
                                    }
                                    catch (Exception)
                                    {
                                        //cmbRelease_Nightly.SelectedIndex = 0;
                                    }
                                }
                            }
                        }
                        catch (Exception x)
                        {
                            Log.Debug(x.ToString());
                        }
                    }


                    break;
                case ("TextBox"):
                    foreach (var item in textBoxList)
                    {
                        try
                        {
                            //string tmpNameOfRadioButton = StringToUI[item.Name];
                            // w item mam nazwe radiobuttona i radiobutton
                            foreach (var item2 in StringToUI.Keys)
                            {
                                if (item2 == item.Name)
                                {
                                    item.Text = settings[StringToUI[item2]];                                   
                                }
                            }
                        }
                        catch (Exception x)
                        {
                            Log.Debug(x.ToString());
                        }
                    }
                    break;
                default:
                    break;
            }


        }

        public void initiationForprograms()
        {
            lblVersion.Content = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();


            var t = Task.Run(() => { // watek 
                try
                {
                    if (!Directory.Exists("C:\\Program Files\\UltimateChanger\\compositions\\"))
                    {
                        Directory.CreateDirectory("C:\\Program Files\\UltimateChanger\\compositions\\");
                        Log.Debug("Dir: C:\\Program Files\\UltimateChanger\\compositions\\ - Created");
                        return;
                    }
                    Log.Debug("Dir: C:\\Program Files\\UltimateChanger\\compositions\\ - Exist");
                }
                catch (Exception x)
                {
                    Log.Debug(x.ToString());
                    MessageBox.Show(x.ToString());
                }
            });

            var t2 = Task.Run(() => {
                try
                {
                    CredentialCache.DefaultNetworkCredentials.Domain = "EMEA";

                    CredentialCache.DefaultNetworkCredentials.UserName = "gl_ssc_swtest";

                    CredentialCache.DefaultNetworkCredentials.Password = "Start123";


                    string[] fileonServer = Directory.GetFiles(@"\\demant.com\data\KBN\RnD\FS_Programs\Support_Tools\REMedy\_currentVersion"); // pobieram nazwy plikow

                    if (fileOperator.checkInstanceFakeVerifit())
                    {
                        //btnFakeV.IsEnabled = true;
                        FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(@"C:\Program Files (x86)\REMedy\REMedy.Launcher.exe");
                        FileVersionInfo veronserver = FileVersionInfo.GetVersionInfo(fileonServer[0]);//pobieram info o pliku 

                        if (myFileVersionInfo.FileVersion != veronserver.FileVersion)
                        {
                            try
                            {
                                string[] dd = Directory.GetFiles(@"C:\Program Files\UltimateChanger\Resources");
                                FileInfo nazwa = new FileInfo(dd[0]);
                                Process.Start(dd.Last(), "/uninstall /quiet ");
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Dir Error :) kiedyś naprawie :)");
                            }
                        }
                       // btnFakeV.ToolTip = myFileVersionInfo.FileVersion;
                    }
                    else
                    {
                        //btnFakeV.IsEnabled = false;
                        if (!Directory.Exists(@"C:\Program Files\UltimateChanger\Resources"))
                        {
                            Directory.CreateDirectory(@"C:\Program Files\UltimateChanger\Resources");
                        }
                        FileInfo nazwa = new FileInfo(fileonServer[0]);
                        try
                        {
                            File.Copy(fileonServer[0], @"C:\Program Files\UltimateChanger\Resources\" + nazwa.Name);
                        }
                        catch (Exception x)
                        {
                            Log.Debug(x.ToString());
                            Process.Start("Resources\\REMedy.Installer.Mini.1.0.3.0.exe", "/silent /install ");
                        }
                        Process.Start(fileonServer[0], "/silent /install ");
                        // uruchomic silent installera 
                    }
                }
                catch (IOException y)
                {
                    Log.Debug(@"can not find \n \\demant.com\data\KBN\RnD\FS_Programs\Support_Tools\REMedy\_currentVersion");
                    Log.Debug(y.ToString());
                }
                catch (Exception x)
                {
                    Log.Debug(x.ToString());
                }
            });

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
            StringToUI.Add("rbnTurnOnVerifit", "TurnOnVerifit");
            StringToUI.Add("rbnTurnOffVerifit", "TurnOffVerifit");
            StringToUI.Add("txtLocalCompoPath", "LocalComposition");
            //get savedTime
            fileOperator.getSavedTime();

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
            // kompozycje:
            cmbMarket_Compo.IsEnabled = true;
            cmbLogMode_Compo.IsEnabled = true;
            cmbLogSettings_Compo.IsEnabled = true;
            btnAdvancelogs_Compo.IsEnabled = true;
            btnLogMode_Compo.IsEnabled = true;
            btnSavelogs_Compo.IsEnabled = true;
            btnFS_Compo.IsEnabled = true;
            btnHattori_Compo.IsEnabled = true;
            btnDeletelogs_Compo.IsEnabled = true;
            btnUpdate_Compo.IsEnabled = true;
            btnuninstal_Compo.IsEnabled = true;

            cmbMarket_Compo.SelectedIndex = 1;
            cmbLogMode_Compo.SelectedIndex = 0;

            List<string> logmod = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                logmod.Add(FittingSoftware_List[i].LogMode);
            }
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


                // kompozycje:
                cmbMarket_Compo.IsEnabled = false;
                cmbLogMode_Compo.IsEnabled = false;
                cmbLogSettings_Compo.IsEnabled = false;
                btnAdvancelogs_Compo.IsEnabled = false;
                btnLogMode_Compo.IsEnabled = false;
                btnSavelogs_Compo.IsEnabled = false;
                btnFS_Compo.IsEnabled = false;
                btnHattori_Compo.IsEnabled = false;
                btnDeletelogs_Compo.IsEnabled = false;
                btnUpdate_Compo.IsEnabled = false;
                btnuninstal_Compo.IsEnabled = false;

                cmbMarket.SelectedIndex = -1;
                cmbLogMode.SelectedIndex = -1;

                cmbMarket_Compo.SelectedIndex = -1;
                cmbLogMode_Compo.SelectedIndex = -1;
            }
            else
            {
                List<string> logmod = new List<string>();
                for (int i = 0; i < 5; i++)
                {
                    logmod.Add(FittingSoftware_List[i].LogMode);
                }
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
            try
            {
                int counter = 0;
                List<FittingSoftware> partListFS = new List<FittingSoftware>();                
                if (TabFull.IsSelected)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        FittingSoftware_List[i].getInfoBuild(i);
                        partListFS.Add(FittingSoftware_List[i]);
                    }
                }
                else
                {
                    for (int i = 5; i < FittingSoftware_List.Count; i++)
                    {
                        partListFS.Add(FittingSoftware_List[i]);
                    }
                }


                foreach (var item in partListFS)
                {
                    ListFSButtons[counter].ToolTip = item.Brand + ", " + item.OEM + "\n" + item.LogMode;
                    if (item.Market == null || item.Version == "")
                    {
                        ListFSButtons[counter].ToolTip = null;
                    }
                    if (!uninstallTimer.IsEnabled && !InstallTimer.IsEnabled)
                    {
                        listlabelsinfoFS_Version[counter].Content = item.Version;
                    }

                    try
                    {
                        listlabelsinfoFS[counter].Content = item.Market;
                    }
                    catch (Exception x)
                    {
                        Log.Debug(x.ToString());
                        listlabelsinfoFS[counter].Content = "FS not installed";
                    }

                    counter++;
                }
                try
                {
                    if (!nightlyUpgradeManager.waitForUpgrade) // jezeli nie czekamy na update to mozna update ui dla time
                    {
                        Time_now = new DateTime();
                        Time_now = Time_now.AddHours(DateTime.Now.Hour);
                        Time_now = Time_now.AddMinutes(DateTime.Now.Minute);
                        updateClockUI();
                    }
                }
                catch (Exception)
                {
                    Time_now = new DateTime();
                    Time_now = Time_now.AddHours(DateTime.Now.Hour);
                    Time_now = Time_now.AddMinutes(DateTime.Now.Minute);
                    updateClockUI();
                }
               

                

            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
            }           
        }

        public void updateConnectionStatusUI(object sender, EventArgs e)
        {
            if (dataBaseManager.getConnectionstatus())
            {
                if (dataBaseManager.DB_connection)
                {
                    lblConnectionToDB.Content = "Connected to DB";
                }
                ConnectionToDBTimer.Stop();
            }

        }

        public void checkUpdateOnServer(object sender, EventArgs e)
        {
            fileOperator.checkVersion();
        }

        void initializeTimers()
        {
            try
            {
                ConnectionToDBTimer = new DispatcherTimer();
                ConnectionToDBTimer.Tick += updateConnectionStatusUI;
                ConnectionToDBTimer.Interval = new TimeSpan(0, 0, 2);
                ConnectionToDBTimer.Start();
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
            }

            RefUiTIMER = new DispatcherTimer();
            RefUiTIMER.Tick += refreshUI;
            RefUiTIMER.Interval = new TimeSpan(0, 0, 30);
            RefUiTIMER.Start();

            checkUpdate = new DispatcherTimer();
            checkUpdate.Tick += checkUpdateOnServer;
            checkUpdate.Interval = new TimeSpan(1, 0, 0);
            checkUpdate.Start();


            InstallTimer = new DispatcherTimer();
            InstallTimer.Tick += checkInstallTimer_Tick;
            InstallTimer.Interval = new TimeSpan(0, 0, 20);

            try
            {
                uninstallTimer = new DispatcherTimer();
                uninstallTimer.Tick += checkUninstallation_Tick;
                uninstallTimer.Interval = new TimeSpan(0, 0, 5);
               
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
            }


            timer_checkValidation = new DispatcherTimer();
            timer_checkValidation.Tick += checkValidation_Tick;
            timer_checkValidation.Interval = new TimeSpan(0, 0, 2);

            InstallTimer_Normal_Installation = new DispatcherTimer();
            InstallTimer_Normal_Installation.Tick += checkNormal_Installation;
            InstallTimer_Normal_Installation.Interval = new TimeSpan(0, 0, 30);

            silentUninstal_Install_Timer = new DispatcherTimer();
            silentUninstal_Install_Timer.Tick += checkUninstall; 
            silentUninstal_Install_Timer.Interval = new TimeSpan(0, 1, 0);

            checkTime_Timer = new DispatcherTimer();
            checkTime_Timer.Tick += checkTime_forUpgradeFS;
            checkTime_Timer.Interval = new TimeSpan(0, 1, 0);

            CopyTimer = new DispatcherTimer();
            CopyTimer.Tick += checkTime_CopyStatus;
            CopyTimer.Interval = new TimeSpan(0,0,1);
        }

        void initializeElements()
        {
            checkBoxList = new List<CheckBox>()
            {
                Oticon,
                Medical,
                Sonic,
                Cumulus,
                Bernafon   
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
                rbnLogsAll_YES,
                rbnLogsAll_NO,
            };
            comboBoxList = new List<ComboBox>()
            {
                cmbRelease
            };

            textBoxList = new List<TextBox>()
            {
                txtLocalCompoPath
            };


            listlabelsinfoFS = new List<Label>()
            {
                lblG,
                lblM,
                lblE,
                lblC,
                lblO
            };
            listlabelsinfoFS_Version = new List<Label>()
            {
                lblGV,
                lblMV,
                lblEV,
                lblCV,
                lblOV
            };
        }

        //________________________________________________________________________________________________________________________________________________



        private void Window_Closing_1(object sender, CancelEventArgs e) // closing window by X button
        {
            FileOperator.setNextCountUCRun();
            fileOperator.saveSavedTime(savedTime.ToString());
            dataBaseManager.pushLogs();
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
            if (TabFull.IsSelected)
            {
                byte counter = 0;
                if (FittingSoftware_List.Count<0)
                {
                    return;
                }
                foreach (var item in checkBoxList)
                {                  
                    if (!File.Exists(FittingSoftware_List[counter].pathToExe) && !item.Name.Contains("Night")) // jezeli nie ma path to nie ma kompozycji 
                    {
                        if (!FittingSoftware_List[counter].checkTrashInstance()) // jezeli nie ma smieci
                        {
                            item.IsEnabled = false;
                            item.IsChecked = false;
                        }

                        labelListsforRefreshUI[counter].Content = "FS not installed";
                        FittingSoftware_List[counter] = new FittingSoftware(FittingSoftware_List[counter].Name_FS);

                        try
                        {
                            list_checkValidation[counter] = 3;
                            if (!timer_checkValidation.IsEnabled)
                            {
                                setImagesForWarningFS();
                            }
                            
                        }
                        catch (Exception)
                        {
                            imagesListWarning[counter].Visibility = Visibility.Hidden;
                        }


                    }
                    else
                    {
                        item.IsEnabled = true;
                    }
                    counter++;
                }
            }
            else // sprawdzanie kompozycji
            {
                // if (!File.Exists(@"C:/Program Files (x86)/Oticon/Genie/Genie2/Genie.exe"))
                byte counter = 0;

                for (int i = 5; i < FittingSoftware_List.Count; i++)
                {
                    FittingSoftware_List[i] = new FittingSoftware(FittingSoftware_List[i].Name_FS, true);
                }

                foreach (var item in checkBoxList)
                {
                    if (FittingSoftware_List[counter+5].pathToExe == "" && !item.Name.Contains("Night")) // jezeli nie ma path to nie ma kompozycji 
                    {
                        item.IsEnabled = false;
                        item.IsChecked = false;
                        labelListsforRefreshUI[counter].Content = "FS not installed";
                    }
                    else
                    {
                        item.IsEnabled = true;
                    }
                    counter++;
                }              
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            byte licznik = 0;
            
            string message = "Deleted: \n";
            string message2 = "Close FS or uninstall: \n";
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    if (checkRunningProcess(item.Name) && !fileOperator.checkInstanceOfFS(licznik))
                    {
                        FittingSoftware_List[licznik].deleteTrash();
                        setNewSavedTime(5);
                        refreshUI(new object(), new EventArgs());
                        message = message + item.Name + "\n";

                    }
                    else
                    {
                        message2 = message2 + item.Name;
                        MessageBox.Show(message2);
                    }
                }
                licznik++;
            }
            Log.Debug(message2);
            Log.Debug("btnDelete_Click");
                
                
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
            if (!TabFull.IsSelected)
            {
                MessageBox.Show("in progress ... \n next update");
                return;
            }
            byte licznik = 0;
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    try
                    {
                        Process.Start(BuildInfo.ListPathsToHattori[licznik] + "FirmwareUpdater.exe");
                        CounterOfclicks.AddClick((int)Buttons.StartHAttori);
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.ToString());
                    }
                }
                licznik++;
            }
            //refreshUI(new object(), new EventArgs());
        }
        private void btnuninstal_Click(object sender, RoutedEventArgs e)
        {
            bool mode_uninstall = RBnormal.IsChecked.Value;
            byte count = 0,countFS =0;
            bool flag = true;
            int chechboxNr = 0;
            List<int> listIndexOfCheckedFS = new List<int>();
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    count++;
                    flag = false;
                    labelListsforUninstall.Add(listlabelsinfoFS_Version[countFS]); 
                    listIndexOfCheckedFS.Add(countFS);
                }
                if (flag)
                {
                    chechboxNr++;
                }
                countFS++;
            }
            if (count > 1 && RBnormal.IsChecked.Value)
            {
                MessageBox.Show("Only one FS at a time can be uninstalled \nturn on silent mode to uninstall more than one FS");
                return;
            }

            if (count == 1 && mode_uninstall) // 1 FS with UI
            {
                Log.Debug("Normal Uninstall Started for: \n" + FittingSoftware_List[chechboxNr].string_For_Log());
                FittingSoftware_List[chechboxNr].uninstallFS(true); 
                return;
            }
            
            for (int i = 0; i < listIndexOfCheckedFS.Count; i++)
            {
                Log.Debug("Silent Uninstallation for: \n\n" + FittingSoftware_List[listIndexOfCheckedFS[i]].string_For_Log());
                listGlobalPathsToUninstall.Add(FittingSoftware_List[listIndexOfCheckedFS[i]]);// dodaje FS do listy FS do usuniecia
            }

            uninstallTimer.Start();

            /*
             1 FS na raz timer sprawdzający czy uninstall się skończył 
             gdy uninstallacja trwa uninstall i install button zablokowany     
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
            CounterOfclicks.AddClick((int)Buttons.UpdateMode);
            if (cmbLogSettings.Visibility == Visibility.Hidden)
            {
                if (Advance_1 != "" || Advance_2 != "" || Advance_3 != "")
                {
                    byte licznik = 0;
                    bool flag = false;
                    string message = "updated: \n";
                    foreach (var item in checkBoxList)
                    {
                        if (item.IsChecked.Value)
                        {
                            FittingSoftware_List[licznik].setLogMode(cmbLogMode.Text,cmbLogSettings.SelectedIndex, TabFull.IsSelected);
                            setNewSavedTime(20);
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
                    cmbLogSettings.Visibility = Visibility.Visible;
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
                            FittingSoftware_List[licznik].setLogMode(cmbLogMode.Text, cmbLogSettings.SelectedIndex, TabFull.IsSelected);
                            message = message + item.Name + "\n";
                            flag = true;
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


            refreshUI(new object(), new EventArgs());
        }
        private void btnDelete_logs(object sender, RoutedEventArgs e)
        {
            byte licznik = 0;
            CounterOfclicks.AddClick((int)Buttons.DeleteLogs);
            TrashCleaner smieciarka = new TrashCleaner();
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    if (!fileOperator.checkRunningProcess(item.Name) && item.Name != "Cumulus")
                    {
                        FittingSoftware_List[licznik].deleteLogs();
                        setNewSavedTime(10);
                    }
                    else if (item.Name == "Cumulus")
                    {
                          if (!fileOperator.checkRunningProcess("Philips HearSuite"))
                          {
                            FittingSoftware_List[licznik].deleteLogs();
                            setNewSavedTime(10);
                        }
                    } else
                    {
                         MessageBox.Show("Close FS to Delete Logs");
                    }
                }
                licznik++;
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
            List<string> listFScomposition = new List<string>()
            {
                {"GenieComposition"},
                {"OasisComposition"},
                {"EXPRESSfitComposition"},
                {"MedicalComposition"},
                {"HearSuiteComposition"},
            };
            CounterOfclicks.AddClick((int)Buttons.InstallFittingSoftware);

            if (cmbBuild.SelectedIndex > -1 || cmbBuild_Compo.SelectedIndex>-1)
            {
                if (TabCompo.IsSelected) // kompozycje
                {
                    
                    string from = cmbBuild_Compo.Text;
                    if (!File.Exists(from))
                    {
                        return;
                    }
                    FileInfo infoFile = new FileInfo(from);
                    string to = "C:\\Program Files\\UltimateChanger\\compositions\\" + infoFile.Name;

                    cmbRelease_Compo.IsEnabled = false;
                    cmbBrandstoinstall_Compo.IsEnabled = false;

                    cmbBuild2_Compo.IsEnabled = false;
                    cmbOEM_Compo.IsEnabled = false;
                    CopyTimer.Start();
                    fileOperator.StartCopyProcess(from, to);
                    pathToCopyOfComposition = to; // zmienna globalna do uruchomienia extraktora kompozycji 
                    progress_Compo.Visibility = Visibility.Visible;
                    return;                      
                }
                else
                {
                    cmbBuild.Items.Refresh();
                    FSInstaller installer = new FSInstaller();
                    installer.InstallBrand(cmbBuild.Text, RBnormal.IsChecked.Value);
                    InstallTimer_Normal_Installation.Start();
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

        public void ChangedBrandOfFittingSoftware()
        {

            if (TabCompo.IsSelected)
            {
                try
                {                    
                    cmbBrandstoinstall_Compo.Items.Refresh();
                    BindCombo.setOEMComboBox(cmbBrandstoinstall_Compo.Text);
                    cmbBuild_Compo.ItemsSource = dataBaseManager.getBuilds("Composition", cmbRelease_Compo.Text, cmbBuild2_Compo.Text, cmbBrandstoinstall_Compo.Text, cmbBrandstoinstall_Compo.Text);
                    // cmbBrandstoinstall.ToolTip = FileOperator.listPathTobuilds[cmbBrandstoinstall.SelectedIndex];
                    //cmbBrandstoinstall.ToolTip = FileOperator.listPathTobuilds[cmbBrandstoinstall.SelectedIndex];
                }
                catch (Exception x)
                {
                    MessageBox.Show("Error FS Combo \n" + x.ToString());
                }
            }
            else // bla full/medium
            {
                try
                {
                    BindCombo.setOEMComboBox(cmbBrandstoinstall.Text);
                    cmbBuild.ItemsSource = dataBaseManager.getBuilds("FULL", cmbRelease.Text, cmbBuild_mode.Text, cmbBrandstoinstall.Text, cmbOEM.Text);
                }
                catch (Exception x)
                {
                    // MessageBox.Show("Error FS Combo \n" + x.ToString());
                    Log.Info("Error FS Combo \n");
                    Log.Debug(x.ToString());
                }
            }
        }

        private void cmbbrandstoinstall_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbBrandstoinstall.Items.Refresh();
            ChangedBrandOfFittingSoftware();
        }
        private void cmbbuild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBuild.SelectedIndex != -1)
            {
                btninstal.IsEnabled = true;
                btnInfo.IsEnabled = true;

                cmbBuild.Items.Refresh();
                cmbBrandstoinstall.Items.Refresh();
                cmbBuild.ToolTip = cmbBuild.Text;
            }
            else
            {
                btninstal.IsEnabled = false;
                btnInfo.IsEnabled = false;
            }
        }

        private void cmbBuild_Compo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBuild_Compo.SelectedIndex != -1)
            {
                btninstal_Compo.IsEnabled = true;
                btnInfo_Compo.IsEnabled = true;

                cmbBuild_Compo.Items.Refresh();
                cmbBrandstoinstall_Compo.Items.Refresh();
                cmbBuild_Compo.ToolTip = cmbBuild_Compo.Text;
            }
            else
            {
                btninstal_Compo.IsEnabled = false;
                btnInfo_Compo.IsEnabled = false;
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
            if (txtNameUser.Text == "SWS")
            {
                R_Day.Visibility = Visibility.Visible;
            }

        }
        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {
           
        }
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            //cmbBuild.ToolTip = Directory_toIntall + firstHalf;
        }
        private void textBox_TextChanged(object sender, RoutedEventArgs e)
        {
        }


        private void checkInstallTimer_Tick(object sender, EventArgs e)
        {
            Process currentProcess = Process.GetCurrentProcess();
            //string pid = currentProcess.Id.ToString();

           List<string> childs = FileOperator.FindAllProcessesSpawnedBy(Convert.ToUInt32(currentProcess.Id));

            if (childs.Count == 0 ) 
            {
                if (listOfPathsToInstall.Count != 0)
                {
                    ProgressInstallation.Visibility = Visibility.Visible;
                    ProgressInstallation.ToolTip = "Installation in progress";
                    try
                    {
                        Process.Start(listOfPathsToInstall[0], " /quiet");
                        //labelListsforRefreshUI[listOfPathsToInstall[0]]
                        labelListsforUninstall[0].Content = "Installation in progress";
                        labelListsforUninstall.RemoveAt(0);
                        listOfPathsToInstall.RemoveAt(0);                        
                    }
                    catch (Exception x)
                    {
                        Log.Debug(x.ToString());
                        Log.Debug("path to setup: " + listOfPathsToInstall[0]);
                        InstallTimer.Stop();
                        MessageBox.Show("Error installation");
                        return;
                    }
                }
                else
                {
                    ProgressInstallation.Visibility = Visibility.Hidden;
                    InstallTimer.Stop();
                }
            }
            else
            {
                ProgressInstallation.Value += 10;

                if (ProgressInstallation.Value == 100)
                {
                    ProgressInstallation.Value = 0;
                }
            }
        }
        private void checkUninstallation_Tick(object sender, EventArgs e)
        {
            Process currentProcess = Process.GetCurrentProcess();
            //string pid = currentProcess.Id.ToString();

            List<string> childs = FileOperator.FindAllProcessesSpawnedBy(Convert.ToUInt32(currentProcess.Id));

            if (childs.Count == 0)
            {
                if (listGlobalPathsToUninstall.Count != 0)
                {
                   ProgressInstallation.ToolTip = "Uninstall in progress";
                    //uninstallTimer.Stop(); // chce skanowac zawsze czy inaczej ?
                    try
                    {
                        listGlobalPathsToUninstall[0].Kill();
                        Process.Start(listGlobalPathsToUninstall[0].Path_Local_Installer, " /uninstall /quiet");
                        FittingSoftware_List[listGlobalPathsToUninstall[0].indexFS].uninstalled = true;
                        Log.Debug("Silent Uninstallation Started For: "+ listGlobalPathsToUninstall[0].Path_Local_Installer);
                        //FittingSoftware_List[listGlobalPathsToUninstall[0].indexFS].uninstalled = true;
                        listGlobalPathsToUninstall.RemoveAt(0);
                        labelListsforUninstall[0].Content = "Uninstall in progress";
                        labelListsforUninstall.RemoveAt(0);
                    }
                    catch (Exception x)
                    {
                        Log.Debug(x.ToString());
                        uninstallTimer.Stop();
                        return;
                    }
                    btnuninstal.IsEnabled = false;
                    btninstal.IsEnabled = false;
                    btnDelete.IsEnabled = false;
                }
                else
                {
                    uninstallTimer.Stop();
                    ProgressInstallation.Visibility = Visibility.Hidden;
                    btnuninstal.IsEnabled = true;
                    btninstal.IsEnabled = true;
                    btnDelete.IsEnabled = true;
                    Log.Debug("Uninstallation DONE");
                    if (RByesRemove.IsChecked.Value) // jezeli chcemy usuwac smieci to usuwamy
                    {
                        btnDelete_Click(new object(), new RoutedEventArgs());
                    }
                   // MessageBox.Show("Uninstallation DONE");
                }
            }
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

        private void checkNormal_Installation(object sender, EventArgs e)
        {
            Process currentProcess = Process.GetCurrentProcess();
            List<string> childs = FileOperator.FindAllProcessesSpawnedBy(Convert.ToUInt32(currentProcess.Id));
            if (childs.Count > 0)
            {
                ProgressInstallation.Visibility = Visibility.Visible;
                ProgressInstallation.Value += 10;

                if (ProgressInstallation.Value == 100)
                {
                    ProgressInstallation.Value = 0;
                }
            }
            else
            {
                InstallTimer_Normal_Installation.Stop();
                ProgressInstallation.Visibility = Visibility.Hidden;
            }

        }

        private void checkTime_CopyStatus(object sender, EventArgs e)
        {
            if (!fileOperator.worker.IsBusy)
            {
                cmbBrandstoinstall_Compo.IsEnabled = true;
                cmbOEM_Compo.IsEnabled = true;
                cmbRelease_Compo.IsEnabled = true;
                cmbBuild2_Compo.IsEnabled = true;
                try
                {
                    Process.Start(pathToCopyOfComposition);
                    CopyTimer.Stop();
                }
                catch (Exception x)
                {
                    Log.Debug(x.ToString());
                }
                
            }
        }

        public void setImagesForWarningFS()
        {
            try
            {
                for (int i = 0; i < 5; i++)
                {

                    if (list_checkValidation[i] == 0) // uptodate
                    {
                        imagesListWarning[i].Source = new BitmapImage(new Uri("/Images/ok.png", UriKind.Relative));
                        imagesListWarning[i].Visibility = Visibility.Visible;
                    }
                    else if (FittingSoftware_List[i].Currentr_BuildInformation.Type == "IP") // IP 
                    {
                        imagesListWarning[i].Source = new BitmapImage(new Uri("/Images/IP_Ponint.png", UriKind.Relative));
                        imagesListWarning[i].Visibility = Visibility.Visible;
                        imagesListWarning[i].ToolTip = "IP";

                    }
                    else if(list_checkValidation[i] == 1) // old
                    {
                        imagesListWarning[i].Source = new BitmapImage(new Uri("/Images/warning.png", UriKind.Relative));
                        imagesListWarning[i].Visibility = Visibility.Visible;
                        imagesListWarning[i].ToolTip = FittingSoftware_List[i].buildInformation.Version.ToString();
                    }
                    else
                    {
                        imagesListWarning[i].Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
            }
           
        }

        private void checkValidation_Tick(object sender, EventArgs e) // funkcja sprawdzajaca czy zakonczyl sie task do sprawdzania validacji FS jezeli sie skonczyl to dziala na UI 
            //i uruchamia odpowiednie ikony
        {
            if (task_checkValidation.IsCompleted)
            {
                //setImagesForWarningFS();
                timer_checkValidation.Stop();
            }
        }

        private void checkUninstall(object sender, EventArgs e) // sprawdz czy uninstallacja trwa jezeli juz sie skonczyla wtedy wlacz timer do instalacji nocnej
        {
            Process currentProcess = Process.GetCurrentProcess();
            List<string> childs = FileOperator.FindAllProcessesSpawnedBy(Convert.ToUInt32(currentProcess.Id));
            // przerobic na sprawdzenie czy jest cos na liscie do uninstallacji i moze przejscie po FS czy trwa uninstlacja?
           
            if (childs.Count > 0)
            {
                ProgressInstallation.Visibility = Visibility.Visible;
                ProgressInstallation.Value += 10;

                if (ProgressInstallation.Value == 100)
                {
                    ProgressInstallation.Value = 0;
                }
            }
            else if(!uninstallTimer.IsEnabled && childs.Count == 0) // jezeli nie ma nic do usuniecia bo wylaczony timer i nie ma procesu odpalonego
            {
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (FittingSoftware_List[i].Upgrade_FS.info.TrashCleaner)
                        {
                            btnDelete_Click(new object(), new RoutedEventArgs());
                            break;
                        }
                    }                    
                }
                catch (Exception)
                {

                }
                InstallTimer_Normal_Installation.Stop();                
                InstallTimer.Start();
                ProgressInstallation.Visibility = Visibility.Hidden;
                silentUninstal_Install_Timer.Stop();
            }
        }

        private void InitFindingNewBuild()
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (FittingSoftware_List[i].Upgrade_FS.info.path_to_root == "")
                    {
                        FittingSoftware_List[i].getNewFSPath();
                    }
                    else
                    {
                        // tylko full 
                        FittingSoftware_List[i].PathToNewVerFS = fileOperator.getPathToSetup(FittingSoftware_List[i]); // dodac wyszukiwanie z rootpatha path to setup.exe dla main brandu                     
                    }
                }
                catch (Exception)
                {
                    Log.Debug($"FittingSoftware_List[{i}].Upgrade_FS was null");
                }

            }

            for (int i = 0; i < 5; i++)
            {
                if (FittingSoftware_List[i].Task_GetNewBuild != null) // jezeli jest rozny od null
                {
                    while (FittingSoftware_List[i].Task_GetNewBuild.Status == TaskStatus.Running) // czekam az sie nie skonczy szukanie patha
                    {
                        FittingSoftware_List[i].Task_GetNewBuild.Wait();

                    }
                   // MessageBox.Show(FittingSoftware_List[i].Task_GetNewBuild.Status.ToString());
                    Thread.Sleep(1000);
                }
                else
                {
                    Log.Debug("Task_GetNewBuild is null for: " + FittingSoftware_List[i].Name_FS);
                }

                Log.Debug(FittingSoftware_List[i].Name_FS + " path to new build setup: " + FittingSoftware_List[i].PathToNewVerFS);

                if (FittingSoftware_List[i].PathToNewVerFS != "") // jezlei jest nowsza warsja to dodaje do usuniecia checkbox
                {
                    checkBoxList[i].IsChecked = true;
                    listOfPathsToInstall.Add(FittingSoftware_List[i].PathToNewVerFS); // dodaje na liste paths do instalacji
                }
                else
                {
                    checkBoxList[i].IsChecked = false;
                }
            }
        }

        private void checkTime_forUpgradeFS(object sender, EventArgs e) // sprawdz czy uninstallacja trwa jezeli juz sie skonczyla wtedy wlacz timer do instalacji nocnej
        {
            int countFS_WithUpdate = -1;
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    if (FittingSoftware_List[i].Upgrade_FS != null)
                    {
                        countFS_WithUpdate = i;
                    }
                }
                if (countFS_WithUpdate == -1)
                {
                    return;
                }

            }
            catch (Exception)
            {

            }

            if (FittingSoftware_List[countFS_WithUpdate].Upgrade_FS.info.Time_Update.Hour == DateTime.Now.Hour) // jezeli godzina sie zgadza
            {
                if (DateTime.Now.Minute >= FittingSoftware_List[countFS_WithUpdate].Upgrade_FS.info.Time_Update.Minute) // jezeli minuta sie zgadza lub juz minela
                {
                    InitFindingNewBuild();
                
                    // zamykam wszystkie FS
                    Button_Click_2(new object(), new RoutedEventArgs());
                    // po zaznaczeniu checkboxow uruchamiam uninstalacje
                    RBsilet.IsChecked = true;
                    btnuninstal_Click(new object(), new RoutedEventArgs());
                    // dodac timer sprawdzajacy czy uninstallsilet sie skonczyl jezeli sie skonczyl to uruchomić silet instalacje 
                    silentUninstal_Install_Timer.Start(); // jezeli uninstall sie skonczy to uruchomi tam InstallTimer.Start() i zainstaluje wszystkie FS;
                    checkTime_Timer.Stop();
                }
                else
                {
                    lblTime_toUpgrade.Content = "Time to start: " + (FittingSoftware_List[countFS_WithUpdate].Upgrade_FS.info.Time_Update.Hour - DateTime.Now.Hour) + " H " + (FittingSoftware_List[countFS_WithUpdate].Upgrade_FS.info.Time_Update.Minute - DateTime.Now.Minute) + " M";
                }
            }
            else
            {
                lblTime_toUpgrade.Content = "Time to start: " + (FittingSoftware_List[countFS_WithUpdate].Upgrade_FS.info.Time_Update.Hour - DateTime.Now.Hour) + " H " + ( FittingSoftware_List[countFS_WithUpdate].Upgrade_FS.info.Time_Update.Minute - DateTime.Now.Minute) + " M";
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
            if (TabFull.IsSelected)
            {
                try
                {
                    System.Windows.Forms.Clipboard.SetText(cmbBuild.ToolTip.ToString());
                }
                catch (Exception x)
                {
                    Log.Debug(x.ToString());
                    Console.WriteLine(x.ToString());
                }
            }
            else
            {
                try
                {
                    System.Windows.Forms.Clipboard.SetText(cmbBuild_Compo.ToolTip.ToString());
                }
                catch (Exception x)
                {
                    Log.Debug(x.ToString());
                    Console.WriteLine(x.ToString());
                }
            }           
        }

        private void btnFakeV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(@"C:\Program Files (x86)\REMedy\REMedy.Launcher.exe");
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
                btnFakeV.IsEnabled = false;
            }
        }

        private void cmbOEM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            cmbOEM.Items.Refresh();
            AllOemPaths = BindCombo.getAllPathsOem(cmbOEM.Text, cmbBrandstoinstall.SelectedIndex, Paths_Dirs);
            cmbBuild.ItemsSource = AllOemPaths;
            if (TabFull.IsSelected)
            {
                cmbBuild.ItemsSource = dataBaseManager.getBuilds("FULL", cmbRelease.Text, cmbBuild_mode.Text, cmbBrandstoinstall.Text, cmbOEM.Text);
            }
            else
            {
                cmbBuild_Compo.ItemsSource = dataBaseManager.getBuilds("Composition", cmbRelease_Compo.Text, cmbBuild2_Compo.Text, cmbBrandstoinstall_Compo.Text, cmbBrandstoinstall_Compo.Text);
            }
                
        }
        private void cmbOEM_Compo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            cmbBuild_Compo.ItemsSource = Paths_Dirs[0].dir;
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
                        fileOperator.StartFS(licznik,TabFull.IsSelected);
                        CounterOfclicks.AddClick((int)Buttons.StartFittingSoftware);
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
            setNewSavedTime(10);
            CounterOfclicks.AddClick((int)Buttons.Kill);
        }

        private void btnSavelogs_Click(object sender, RoutedEventArgs e)
        {          
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.zip)|*.zip|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            List<string> listFiles = new List<string>();
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
                                System.IO.Compression.ZipFile.CreateFromDirectory(FittingSoftware_List[licznik].pathToLogs, saveFileDialog1.FileName + "_" + item.Name + ".zip"); // dziala 
                                listFiles.Add(saveFileDialog1.FileName);
                            }
                            catch (IOException y)
                            {
                                File.Delete(saveFileDialog1.FileName + "_" + item.Name + ".zip");
                                System.IO.Compression.ZipFile.CreateFromDirectory(FittingSoftware_List[licznik].pathToLogs, saveFileDialog1.FileName + "_" + item.Name + ".zip"); // dziala 
                            }
                            catch (Exception x)
                            {
                                MessageBox.Show(x.ToString());
                            }
                        }
                        licznik++;
                    }
                    myStream.Close();
                    foreach (var item in listFiles)
                    {
                        try
                        {
                            File.Delete(item);
                        }
                        catch (Exception x)
                        {
                            Log.Debug(x.ToString());
                        }                       
                    }
                    MessageBox.Show("Logs Saved");
                }
            }
        }

        private void Downgrade(object sender, RoutedEventArgs e)
        {            
            Window downgrade = new DowngradeWindow();
            //downgrade.ShowDialog();
            downgrade.Show();
            CounterOfclicks.AddClick((int)Buttons.Downgrade);
        }


        private void btnRANDHI_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxOfAvailableTypes.SelectedIndex == -1)
            {
                MessageBox.Show("Select Style to Rand");
                return;
            }
            if (listOfTeammembers.Count == 0)
            {
                MessageBox.Show("Remember to Select some Person(s) and press Add button");
                return;
            }
            CounterOfclicks.AddClick((int)Buttons.RandomHI);

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
                                listofstyles.Add(itemm.ToString());
                            }

                            HIs tmpHIL = HIs.randomHI(lblRelease.Content.ToString(), listofstyles, tmp_hi_Types_Name);
                            tmp.HIL_ = tmpHIL.Name;

                            tmp.Family_Name = tmpHIL.Name_fammily;

                            List<string> listOfNames = new List<string>() { tmp.Family_Name };
                            
                            tmp.HIR_ = "N/A";

                            string wireless = "FALSE";

                            if (tmpHIL.Wireless) //jezeli oba maja wireless
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

                            listOfRandomHardawre_perPerson.Add(tmp.Name_Team_member + "," + tmp.Family_Name + "," + tmp.HIL_ + "," + tmp.HIR_ + "," + tmp.Ficzur_ + "," + tmp.ComDev_);
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
                            List<string> listofstyles = new List<string>();
                            foreach (var itemm in ListBoxOfAvailableStyles.SelectedItems)
                            {
                                listofstyles.Add(itemm.ToString());
                            }

                           
                            tmp.HIL_ = "N/A";


                            HIs tmpHIR = HIs.randomHI(lblRelease.Content.ToString(), listofstyles, tmp_hi_Types_Name);
                            tmp.HIR_ = tmpHIR.Name;
                            tmp.Family_Name = tmpHIR.Name_fammily;

                            string wireless = "FALSE";

                            if ( tmpHIR.Wireless) //jezeli oba maja wireless
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

                            listOfRandomHardawre_perPerson.Add(tmp.Name_Team_member + "," + tmp.Family_Name + "," + tmp.HIL_ + "," + tmp.HIR_ + "," + tmp.Ficzur_ + "," + tmp.ComDev_);
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

        private void Dark_skin_Checked(object sender, RoutedEventArgs e)
        {
           //Zmiany na ciemny motyw (można zmienić kolor ramki itd.)
            XMLReader.setSetting("Dark_skin", "RadioButtons", Convert.ToString(rbnDark_skin.IsChecked.Value).ToUpper());
            bool tmp = !rbnDark_skin.IsChecked.Value;
            XMLReader.setSetting("Light_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Genie_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Oasis_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("ExpressFit_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());

            //USTAWIENIA LABELI
            foreach (var item in lableListForUi)
            {
                item.Foreground = Brushes.White;
            }
            //USTAWIENIA BOXÓW
            foreach (var item in listBoxForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = Brushes.White;

            }
            var converter = new System.Windows.Media.BrushConverter();

            //USTAWIENIA BUTTONÓW
            foreach (var item in buttonListForUi)
            {
                item.Background = (Brush)converter.ConvertFromString("#FF616161");
                item.Foreground = (Brush)converter.ConvertFromString("#E5FFFFFF");
                item.BorderBrush = (Brush)converter.ConvertFromString("#FF424242");
                item.Opacity = 1;
                item.MaxWidth = 250;
            }

            //USTAWIENIA CHECKBOXÓW
            foreach (var item in checkBoxListForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = (Brush)converter.ConvertFromString("#FF424242");
                item.Background = Brushes.White;
                UpdateLayout();
                item.Style = Resources["CheckboxDark"] as Style;
            }

            //USTAWIENIA COMBOBOXÓW
            foreach (var item in comboBoxListForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = Brushes.White;
            }

            //USTAWIENIA RADIOBUTTONÓW
            foreach (var item in radioButtonListForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = Brushes.White;
                item.Background = Brushes.White;
                UpdateLayout();
                item.Style = Resources["RadiobuttonDark"] as Style;
            }

            //USTAWIENIA TEXTBOXÓW
            foreach (var item in texBoxListForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = Brushes.White;
            }

            //USTAWIENIA SLIDERÓW
            foreach (var item in sliderListForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = Brushes.White;               
            }

            //USTAWIENIA RAMEK
            foreach (var item in borderListForUi)
            {
                item.BorderBrush = (Brush)converter.ConvertFromString("#FF616161"); 
            }

            //USTAWIENIA PASSWORDBOXÓW
            passwordBox.Foreground = Brushes.White;
            passwordBox.BorderBrush = Brushes.White;

            //USTAWIENIA TABELI
            UpdateLayout();
            GridDataRandomHardware.ColumnHeaderStyle = Resources["DataGridDark"] as Style;

            //USTAWIENIA ZAKŁADEK
            UpdateLayout();
            tabControl.ItemContainerStyle = Resources["TabItemDark"] as Style;
            tabControl.Background= (Brush)converter.ConvertFromString("#FF212121");
            tabControl.Background = (Brush)converter.ConvertFromString("#FF212121");
            tabControl.Foreground = Brushes.White;

            UpdateLayout();
            tabControl2.ItemContainerStyle = Resources["TabItemDark"] as Style;
            tabControl2.Background = (Brush)converter.ConvertFromString("#FF212121");
            tabControl2.Background = (Brush)converter.ConvertFromString("#FF212121");
            tabControl2.Foreground = Brushes.White;

            //USTAWIENIA TŁA
            this.Background = (Brush)converter.ConvertFromString("#E2212121");
          //  oticonmedicalnRectangle.Fill= (Brush)converter.ConvertFromString("#FFECB3");
            //oticonRectangle.Fill = (Brush)converter.ConvertFromString("#FAFAFA");

            //USTAWIENIA AKCENTÓW
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml", UriKind.RelativeOrAbsolute)
            });

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Grey.xaml", UriKind.RelativeOrAbsolute)
            });

        }

        private void Light_skin_Checked(object sender, RoutedEventArgs e)
        {

            //Zmiany na jasny motyw
            XMLReader.setSetting("Light_skin", "RadioButtons",Convert.ToString(rbnLight_skin.IsChecked.Value).ToUpper());
            bool tmp = !rbnLight_skin.IsChecked.Value;
            XMLReader.setSetting("Dark_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Genie_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Oasis_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("ExpressFit_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());


            //USTAWIENIA LABELI
            foreach (var item in lableListForUi)
            {
                item.Foreground = Brushes.Black;
            }

            //USTAWIENIA BOXÓW
            foreach (var item in listBoxForUi)
            {
                item.Foreground = Brushes.Black;
                item.BorderBrush = Brushes.Black;                
            }
            var converter = new System.Windows.Media.BrushConverter();

            //USTAWIENIA BUTTONÓW
            foreach (var item in buttonListForUi)
            {
                item.Background = Brushes.Black;
                item.Foreground = Brushes.White;
                item.BorderBrush = Brushes.Black;
                item.Opacity = 0.8;
                item.MaxWidth = 250;
            }

            //USTAWIENIA CHECBOXÓW
            foreach (var item in checkBoxListForUi)
            {
                item.Foreground = Brushes.Black;
                item.BorderBrush = Brushes.Black;
                item.Background = Brushes.Black;
                UpdateLayout();
                item.Style = Resources["CheckboxLight"] as Style;
            }

            //USTAWIENIA COMBOBOXÓW
            foreach (var item in comboBoxListForUi)
            {
                item.Foreground = Brushes.Black;
                item.BorderBrush = Brushes.Black;
            }

            //USTAWIENIA RADIOBATTONÓW
            foreach (var item in radioButtonListForUi)
            {
                item.Foreground = Brushes.Black;
                item.BorderBrush = Brushes.Black;
                item.Background = Brushes.Black;
                UpdateLayout();
                item.Style = Resources["RadiobuttonLight"] as Style;
            }

            //USTAWIENIA TEXTBOXÓW
            foreach (var item in texBoxListForUi)
            {
                item.Foreground = Brushes.Black;
                item.BorderBrush = Brushes.Black;
            }

            //USTAWIENIA SLIEDERÓW
            foreach (var item in sliderListForUi)
            {
                item.Foreground = Brushes.Black;
                item.BorderBrush = Brushes.Black;
            }

            //USTAWIENIA RAMEK
            foreach (var item in borderListForUi)
            {
                item.BorderBrush = Brushes.Black;
            }

            //USTAWIENIA PASSWORDBOXÓW
            passwordBox.Foreground = Brushes.Black;
            passwordBox.BorderBrush = Brushes.Black;

            //USTAWIENIA TABELI
            UpdateLayout();
            GridDataRandomHardware.ColumnHeaderStyle = Resources["DataGridLight"] as Style;

            //USTAWIENIA ZAKŁADEK
            UpdateLayout();
            tabControl.ItemContainerStyle= Resources["TabItemLight"] as Style;
            tabControl.Background = (Brush)converter.ConvertFromString("#F5F5F5");
            tabControl.Background = (Brush)converter.ConvertFromString("#F5F5F5");
            tabControl.Foreground = Brushes.White;

            UpdateLayout();
            tabControl2.ItemContainerStyle = Resources["TabItemLight"] as Style;
            tabControl2.Background = (Brush)converter.ConvertFromString("#F5F5F5");
            tabControl2.Background = (Brush)converter.ConvertFromString("#F5F5F5");
            tabControl2.Foreground = Brushes.White;

            //USTAWIENIA TŁA
            this.Background= (Brush)converter.ConvertFromString("#F5F5F5");
        //    oticonmedicalnRectangle.Fill = Brushes.Black;
           // oticonRectangle.Fill = Brushes.White;

       
            //USTAWIENIA AKCENTÓW
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml", UriKind.RelativeOrAbsolute)
            });

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Blue.xaml", UriKind.RelativeOrAbsolute)
            });
        }

  
        private void RBnormal_Checked(object sender, RoutedEventArgs e)
        {
            uninstallTimer.Stop();
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
            // wlaczanie Godzilla logging dla wszystkich FS
            myXMLReader.setGodzilla();

            XMLReader.setSetting("NotHoldLogs", "RadioButtons", Convert.ToString(rbnDeletelogs.IsChecked.Value));
            bool tmp = rbnDeletelogs.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("HoldLogs", "RadioButtons", Convert.ToString(tmp));
        }

        private void rbnDefaultNormal_Checked(object sender, RoutedEventArgs e)
        {
            bool tmp = true;
            XMLReader.setSetting("InstallModeNormal", "RadioButtons", tmp.ToString());
            //bool tmp = rbnDefaultNormal.IsChecked.Value;
            tmp = !tmp;
            XMLReader.setSetting("InstallModeSilent", "RadioButtons", Convert.ToString(tmp));
            RBnormal.IsChecked = true;
            RBsilet.IsChecked = false;
        }

        private void rbnDefaultSilent_Checked(object sender, RoutedEventArgs e)
        {
            bool tmp = true;
            XMLReader.setSetting("InstallModeSilent", "RadioButtons", tmp.ToString());
           //bool tmp = RBsilet.IsChecked.Value;
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
            if (ListBoxHardware.SelectedIndex != -1 && rbnSystemView.IsChecked.Value)
            {
                txtMyItemsList.Text =  txtMyItemsList.Text +"\n"+ (MyHardware.convertToString_System(MyHardware.findHardwareByID(ListBoxHardware.SelectedIndex)));
                BindCombo.bindListBox();
            }
            else if(ListBoxHardware.SelectedIndex != -1 && !rbnSystemView.IsChecked.Value)
            {
                txtMyItemsList.Text = txtMyItemsList.Text  + (MyHardware.convertToString_Platform(MyHardware.findHardwareByID(ListBoxHardware.SelectedIndex)));
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
            CounterOfclicks.AddClick((int)Buttons.CopyMyHardware);
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
            MessageBox.Show("może kiedyś :)");
            return;
            //cmbLogSettings.Visibility = Visibility.Hidden;
            //AdvanseSettingsWindow advance = new AdvanseSettingsWindow();
            //advance.Show();
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
            catch (Exception x)
            {
                Log.Debug(x.ToString());
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
                    FittingSoftware_List[licznik].setLogMode("ALL",0, TabFull.IsSelected);
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
                    Log.Debug(ex.ToString());
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
            catch (Exception x)
            {
                Log.Debug(x.ToString());
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
            catch (Exception x)
            {
                Log.Debug(x.ToString());
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

    
        private void Radio_Christmas_Checked(object sender, RoutedEventArgs e)
        {
            //Zmiany na ciemny motyw (można zmienić kolor ramki itd.)
            XMLReader.setSetting("Dark_skin", "RadioButtons", Convert.ToString(rbnDark_skin.IsChecked.Value).ToUpper());
            bool tmp = !rbnDark_skin.IsChecked.Value;
            XMLReader.setSetting("Light_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Genie_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("Oasis_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());
            XMLReader.setSetting("ExpressFit_skin", "RadioButtons", Convert.ToString(tmp).ToUpper());

            foreach (var item in lableListForUi)
            {
                item.Foreground = Brushes.White;
            }
            foreach (var item in listBoxForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = Brushes.White;

            }
            var converter = new System.Windows.Media.BrushConverter();
            //var brush = (Brush)converter.ConvertFromString("#8A959B");

            foreach (var item in buttonListForUi)
            {
                item.Background = (Brush)converter.ConvertFromString("#FF616161");
                item.Foreground = (Brush)converter.ConvertFromString("#E5FFFFFF");
                item.BorderBrush = (Brush)converter.ConvertFromString("#FF424242");
                item.Opacity = 1;
            }

            foreach (var item in checkBoxListForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = (Brush)converter.ConvertFromString("#FF424242");
                item.Background = Brushes.White;
                UpdateLayout();
                item.Style = Resources["CheckboxDark"] as Style;
            }

            foreach (var item in comboBoxListForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = Brushes.White;
            }

            foreach (var item in radioButtonListForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = Brushes.White;
                item.Background = Brushes.White;
                UpdateLayout();
                item.Style = Resources["RadiobuttonDark"] as Style;
            }

            foreach (var item in texBoxListForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = Brushes.White;
            }

            foreach (var item in sliderListForUi)
            {
                item.Foreground = Brushes.White;
                item.BorderBrush = Brushes.White;

            }

            foreach (var item in borderListForUi)
            {
                item.BorderBrush = (Brush)converter.ConvertFromString("#FF616161");
            }

            tabControl.Background = (Brush)converter.ConvertFromString("#FF212121");
            tabControl.Foreground = Brushes.White;
            passwordBox.Foreground = Brushes.White;
            passwordBox.BorderBrush = Brushes.White;
            //oticonRectangle.Fill = (Brush)converter.ConvertFromString("#FAFAFA");

            UpdateLayout();
            GridDataRandomHardware.ColumnHeaderStyle = Resources["DataGridDark"] as Style;
            UpdateLayout();
            tabControl.ItemContainerStyle = Resources["TabItemDark"] as Style;
            tabControl.Background = (Brush)converter.ConvertFromString("#FF212121");
            this.Background = (Brush)converter.ConvertFromString("#E2212121");
            //oticonmedicalnRectangle.Fill = (Brush)converter.ConvertFromString("#FFECB3");

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml", UriKind.RelativeOrAbsolute)
            });

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Grey.xaml", UriKind.RelativeOrAbsolute)
            });
        }

        private void RBcomposition_Checked(object sender, RoutedEventArgs e)
        {
           
        }

        private void RBfullMedium_Checked(object sender, RoutedEventArgs e)
        {

        }



        private void btn_Gearbox(object sender, RoutedEventArgs e)
        {
            try
            {
                fileOperator.StartGearbox();
                setNewSavedTime(5);
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
            }
        }
        private void Click_btnNoah(object sender, RoutedEventArgs e)
        {
            fileOperator.StartNoah();
        }

        private void cmbLogMode_Compo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbLogMode_Compo.SelectedIndex != -1)
            {
                cmbLogSettings_Compo.SelectedIndex = 0;
            }
            else
            {
                cmbLogSettings_Compo.SelectedIndex = -1;
            }
        }
        private void cmbRelease_SelectionChanged_Compo(object sender, SelectionChangedEventArgs e)
        {
            cmbRelease.Items.Refresh();
            XMLReader.setSetting("Release", "ComboBox", cmbRelease.Text);
            cmbBuild_Compo.ItemsSource = dataBaseManager.getBuilds("Composition", cmbRelease_Compo.Text, cmbBuild2_Compo.Text, cmbBrandstoinstall_Compo.Text, cmbBrandstoinstall_Compo.Text);
        }

        private void btnAdvanceInstall_Click(object sender, RoutedEventArgs e)
        {
            Window AdvanceInstall = new AdvanceWindowInstalla(dataBaseManager);
            AdvanceInstall.Owner = this;
            AdvanceInstall.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            AdvanceInstall.ShowDialog();           
        }

        private void btnGenieImage_Click(object sender, RoutedEventArgs e)
        {            
               fileOperator.StartFS(0, TabFull.IsSelected);
               CounterOfclicks.AddClick((int)Buttons.StartFittingSoftware);
        }

        private void btnGenieMedicalImage_Click(object sender, RoutedEventArgs e)
        {
                fileOperator.StartFS(1, TabFull.IsSelected);
                CounterOfclicks.AddClick((int)Buttons.StartFittingSoftware);  
        }

        private void btnExpressfitImage_Click(object sender, RoutedEventArgs e)
        {
                fileOperator.StartFS(2, TabFull.IsSelected);
                CounterOfclicks.AddClick((int)Buttons.StartFittingSoftware);  
        }

        private void btnHearSuiteImage_Click(object sender, RoutedEventArgs e)
        {
                fileOperator.StartFS(3, TabFull.IsSelected);
                CounterOfclicks.AddClick((int)Buttons.StartFittingSoftware); 
        }

        private void btnOasisImage_Click(object sender, RoutedEventArgs e)
        {
                fileOperator.StartFS(4, TabFull.IsSelected);
                CounterOfclicks.AddClick((int)Buttons.StartFittingSoftware);
        }

        private void ListTeamPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ContextMenu_Genie_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MessageBox.Show("tststst");
            }
        }

        private void ContextMenu_Genie_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MessageBox.Show("test33");
        }

        private void btnResetSavedTime_Click(object sender, RoutedEventArgs e)
        {
            savedTime = 0;
            setNewSavedTime(0);
            fileOperator.saveSavedTime("0");
        }

        private void txtLocalCompoPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // jezeli był enter to spr czy byla jakas zmiana jezeli tak to zapisz do defaultow i pokaż komunikat ze zmieniono jezeli nie to olać
            {
                var tmp = XMLReader.getDefaultSettings("TextBox");

                if (txtLocalCompoPath.Text != tmp["LocalComposition"])
                {
                    XMLReader.setSetting("LocalComposition", "TextBox", txtLocalCompoPath.Text);
                }
            }
        }

        private void btnImportSettings_Click(object sender, RoutedEventArgs e)
        {

            //dodac otwieranie wskazanego pliku + jego podmiane w programfiles
            Stream myStream;
            OpenFileDialog saveFileDialog1 = new OpenFileDialog();
            saveFileDialog1.Filter = "txt files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if ((bool)saveFileDialog1.ShowDialog())
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    myStream.Close();
                    try
                    {
                        if (saveFileDialog1.FileName.Contains(".xml"))
                        {
                            File.Copy(saveFileDialog1.FileName, @"C:\Program Files\UltimateChanger\Settings\Defaults.xml", true);
                            File.Delete(saveFileDialog1.FileName.Remove(saveFileDialog1.FileName.Length - 4, 4));
                        }
                        else
                        {
                            File.Copy(saveFileDialog1.FileName + ".xml", @"C:\Program Files\UltimateChanger\Settings\Defaults.xml", true);
                            File.Delete(saveFileDialog1.FileName);
                        }
                    }

                    catch (Exception x)
                    {
                        MessageBox.Show(x.ToString());
                    }
                }
            }


            setUIdefaults(XMLReader.getDefaultSettings("RadioButtons"), "RadioButtons");
            setUIdefaults(XMLReader.getDefaultSettings("CheckBoxes"), "CheckBoxes");
            setUIdefaults(XMLReader.getDefaultSettings("ComboBox"), "ComboBox");
            setUIdefaults(XMLReader.getDefaultSettings("TextBox"), "TextBox");
        }

        public void btnExportSettings_Click(object sender, RoutedEventArgs e)
        {
            // zapis pliku do wskazanej przez usera lokalizacji 
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if ((bool)saveFileDialog1.ShowDialog())
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    myStream.Close();
                    try
                        {
                            if (saveFileDialog1.FileName.Contains(".xml"))
                            {
                                File.Copy(@"C:\Program Files\UltimateChanger\Settings\Defaults.xml", saveFileDialog1.FileName, true);
                                File.Delete(saveFileDialog1.FileName.Remove(saveFileDialog1.FileName.Length - 4, 4));
                            }
                            else
                            {
                                File.Copy(@"C:\Program Files\UltimateChanger\Settings\Defaults.xml", saveFileDialog1.FileName + ".xml", true);
                                File.Delete(saveFileDialog1.FileName);
                            }
                        }
                       
                        catch (Exception x)
                        {
                            MessageBox.Show(x.ToString());
                        }                      
                }
            }
        }

        private void btnDeleteC_Compo_Click(object sender, RoutedEventArgs e)
        {
            TrashCleaner smieciarka = new TrashCleaner();
            int licznik = 0;
            MessageBox.Show("In Progress ...");
            foreach (var item in checkBoxList)
            {
                if (item.IsChecked.Value)
                {
                    try
                    {
                        smieciarka.DeleteCompo(licznik); 
                        //CounterOfclicks.AddClick((int)Buttons.StartFittingSoftware);
                    }
                    catch (Exception x)
                    {
                        Log.Debug(x.ToString());
                    }
                }
                licznik++;
            }
            refreshUI(new object(), new EventArgs());
        }

        private void cmbRelease_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbRelease.Items.Refresh();
            XMLReader.setSetting("Release", "ComboBox", cmbRelease.Text);
            cmbBuild.ItemsSource = dataBaseManager.getBuilds("FULL", cmbRelease.Text, cmbBuild_mode.Text, cmbBrandstoinstall.Text, cmbOEM.Text);
        }

        void updateMarket(bool Full)
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
                                if (cmbMarket.SelectedIndex != -1 || cmbMarket_Compo.SelectedIndex != -1)
                                {
                                    if (Full)
                                    {
                                        if (!FittingSoftware_List[licz].setMarket(BindCombobox.marketIndex[cmbMarket.SelectedIndex])) // jezeli sie nie udalo to zmieniam message
                                        {
                                            message = "error: ";
                                        }
                                        else
                                        {
                                            setNewSavedTime(10);
                                        }
                                    }
                                    else
                                    {
                                        if (!FittingSoftware_List[licz + 5].setMarket(BindCombobox.marketIndex[cmbMarket_Compo.SelectedIndex])) // jezeli sie nie udalo to zmieniam message
                                        {
                                            message = "error: ";
                                        }
                                        else
                                        {
                                            setNewSavedTime(10);
                                        }
                                    }
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
        }


        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            CounterOfclicks.AddClick((int)Buttons.UpdateMarket);
            updateMarket(TabFull.IsSelected); // funkcja ustawiajaca mozna ja przeniesc do fileoperatora
            refreshUI(new object(), new EventArgs());
        }

        private void tabControl2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabFull != null && TabFull.IsSelected)
            {
                if (TabFull.IsEnabled)
                {
                    refreshUI(new object(), new EventArgs());
                    try
                    {

                        fileOperator.GetfilesSaveData(false);
                        BindCombo.setFScomboBox(); // full medium
                        cmbBrandstoinstall.SelectedIndex = 0;
                        cmbOEM_SelectionChanged(new object(),e);
                        RBnormal.IsEnabled = true;
                        RBsilet.IsEnabled = true;


                        // ustawiam na -1 bo inaczej jest crush jak nie masz zaznaczonego marketu a chcesz zrobic update
                        cmbMarket_Compo.SelectedIndex = -1;
                        cmbLogMode_Compo.SelectedIndex = -1;
                        cmbLogSettings_Compo.SelectedIndex = -1;


                        BindCombo.setOEMComboBox(cmbBrandstoinstall.Text);
                        cmbOEM.Items.Refresh();
                        TabFull.IsEnabled = false;
                        TabCompo.IsEnabled = true;
                        cmbBuild.ItemsSource = dataBaseManager.getBuilds("FULL",cmbRelease.Text, cmbBuild_mode.Text, cmbBrandstoinstall.Text, cmbOEM.Text);
                    }
                    catch (Exception x)
                    {
                        Log.Debug(x.ToString());
                    }                   
                }
            }
            if (TabCompo != null && TabCompo.IsSelected)
            {
                if (TabCompo.IsEnabled)
                {
                    refreshUI(new object(), new EventArgs());


                    cmbBuild2_Compo.SelectedIndex = 1;
                    BindCombo.setFScomboBox_compositions(); // bindowanie do compozycjji  
                    cmbRelease_Compo.Text = cmbRelease.Text;
                    cmbBrandstoinstall_Compo.SelectedIndex = 0;
                    try
                    {
                        fileOperator.GetfilesSaveData(TabCompo.IsSelected);
                    }
                    catch (Exception)
                    {
                        cmbBuild.ItemsSource = null;
                    }
                    ChangedBrandOfFittingSoftware();
                    cmbBuild.Items.Refresh();

                    // ustawiam na -1 bo inaczej jest crush jak nie masz zaznaczonego marketu a chcesz zrobic update
                    cmbMarket.SelectedIndex = -1;
                    cmbLogMode.SelectedIndex = -1;
                    //cmbLogSettings.SelectedIndex = -1;

                    RBnormal.IsEnabled = false;
                    RBsilet.IsEnabled = false;
                    TabCompo.IsEnabled = false;
                    TabFull.IsEnabled = true;
                    cmbBrandstoinstall.SelectedIndex = 0;

                }               
            }
        }

        public void setNewSavedTime(int time_)
        {
            savedTime += time_;
            TimeSpan time = TimeSpan.FromSeconds(savedTime);

            //here backslash is must to tell that colon is
            //not the part of format, it just a character that we want in output
            string str = time.ToString(@"hh\:mm\:ss");
            lblSavedTime.Content = "Saved Time: " + str;
        }

        //--- DSZY "LOSU LOSU" ---- //

        int numberOfPeople = 0;

        private void btnSendFeedBack_Click(object sender, RoutedEventArgs e)
        {
            if (txtFeedBack.Text!="")
            {
                dataBaseManager.SendFeedBack(txtFeedBack.Text);
                txtFeedBack.Text = "";
            }
            
        }

        private void cmbBuild_mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbBuild_mode.Items.Refresh();
            cmbBuild.ItemsSource = dataBaseManager.getBuilds("FULL", cmbRelease.Text, cmbBuild_mode.Text, cmbBrandstoinstall.Text, cmbOEM.Text);
        }

        private void cmbBuild2_Compo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbBuild2_Compo.Items.Refresh();
            cmbBuild_Compo.ItemsSource = dataBaseManager.getBuilds("Composition", cmbRelease_Compo.Text, cmbBuild2_Compo.Text, cmbBrandstoinstall_Compo.Text, cmbBrandstoinstall_Compo.Text);
        }

        private void btnUpdateFS_Click(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            string text = "";
            for (int i = 0; i < 5; i++)
            {
                if (checkBoxList[i].IsChecked.Value) // tylko dla zaznaczonych buildow
                {
                    FittingSoftware_List[i].PathToNewVerFS = fileOperator.GetAvailableNewFS(FittingSoftware_List[i], true);
                    Log.Debug("Path to new Ver FS for: " + FittingSoftware_List[i].Name_FS + FittingSoftware_List[i].PathToNewVerFS);
                    //listOfPathsToInstall.Add(FittingSoftware_List[i].PathToNewVerFS); // dodaje na liste paths do instalacji
                    labelListsforUninstall.Add(listlabelsinfoFS_Version[i]);
                    text += FittingSoftware_List[i].PathToNewVerFS + "\n";
                }               
            }
            

            var result = MessageBox.Show("Do you want upgrade FS to: \n" + text, "Update FS", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                flag = true;
            }
            else
            {
                return;
            }

            if (flag)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (FittingSoftware_List[i].PathToNewVerFS != "")
                    {
                        //checkBoxList[i].IsChecked = true;
                        listOfPathsToInstall.Add(FittingSoftware_List[i].PathToNewVerFS);
                    }
                    else
                    {
                        checkBoxList[i].IsChecked = false;
                    }
                }
            }

            // zamykam wszystkie FS
            Button_Click_2(new object(), new RoutedEventArgs());
            // po zaznaczeniu checkboxow uruchamiam uninstalacje
            RBsilet.IsChecked = true;
            btnuninstal_Click(new object(), new RoutedEventArgs());
            // dodac timer sprawdzajacy czy uninstallsilet sie skonczyl jezeli sie skonczyl to uruchomić silet instalacje 
            silentUninstal_Install_Timer.Start(); // jezeli uninstall sie skonczy to uruchomi tam InstallTimer.Start() i zainstaluje wszystkie FS;

        }

        private void btnReadHI_Click(object sender, RoutedEventArgs e)
        {

            txtHIBrand.Text = "";
            txtHIBrand_R.Text = "";
            txtPP.Text = "";
            txtPP_R.Text = "";
            txtSN.Text = "";
            txtSN_R.Text = "";
            txtFW.Text = "";    
            txtFW_R.Text = "";
            
           
            progressHI.Value = 0;
            HI_Reader readHI = new HI_Reader();
            progressHI.Value += 10;
            readHI.startServer();
            progressHI.Value += 40;
            readHI.CreateSession();
            progressHI.Value += 10;
            List<string> HI;
            string device;
            if (rbExpress.IsChecked.Value)
            {
                device = "ExpressLink";
            }
            else
            {
                device = "HiPro";
            }

            string side;
            if (rbLeft.IsChecked.Value)
            {
                side = "Left";
            } else if (rbBoth.IsChecked.Value)
            {
                side = "Both";
                try
                {
                    readHI.Connect(device, "Left");
                    HI = readHI.ReadHI("Left");
                    txtHIBrand.Text = HI[0];
                    txtPP.Text = HI[1];
                    txtFW.Text = HI[2];
                    txtSN.Text = readHI.getSerialNumber("Left");
                    progressHI.Value += 15;
                }
                catch (Exception)
                {
                    progressHI.Value += 15;
                }
                try
                {
                    readHI.Connect(device, "Right");
                    HI = readHI.ReadHI("Right");
                    txtHIBrand_R.Text = HI[0];
                    txtPP_R.Text = HI[1];
                    txtFW_R.Text = HI[2];
                    txtSN_R.Text = readHI.getSerialNumber("Right");
                    progressHI.Value += 15;
                }
                catch (Exception)
                {
                    progressHI.Value += 15;
                }
               
                readHI.shutDown();
                progressHI.Value += 10;
                return;
            }
            else
            {
                side = "Right";
            }
            readHI.Connect(device, side);
            HI = readHI.ReadHI(side);
            if (HI == null)
            {
                readHI.shutDown();
                return;
            }
            if (side =="Right")
            {
                txtHIBrand_R.Text = HI[0];
                txtPP_R.Text= HI[1];
                txtFW_R.Text = HI[2];
                txtSN_R.Text = readHI.getSerialNumber("Right");
            }
            else
            {
                txtHIBrand.Text = HI[0];
                txtPP.Text = HI[1];                
                txtFW.Text = HI[2];
                txtSN.Text = readHI.getSerialNumber("Left");
            }
            progressHI.Value += 30;
            readHI.shutDown();
            progressHI.Value += 10;
            setNewSavedTime(30);


        }

        private void btnHoursUp_Nightly_Click(object sender, RoutedEventArgs e)
        {
            Time_now = Time_now.AddHours(1);
            updateClockUI();
        }

        private void btnHoursDown_Nightly_Click(object sender, RoutedEventArgs e)
        {
            Time_now = Time_now.AddHours(-1);
            updateClockUI();
        }

        private void btnMinutesUp_Nightly_Click(object sender, RoutedEventArgs e)
        {
            Time_now = Time_now.AddMinutes(1);
            updateClockUI();
        }

        private void btnMinutesDown_Nightly_Click(object sender, RoutedEventArgs e)
        {
            Time_now = Time_now.AddMinutes(-1);
            updateClockUI();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            checkTime_Timer.Stop();
            nightlyUpgradeManager = null;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBranch.SelectedIndex == -1 && txtPathRoot.Text == "")
            {
                MessageBox.Show("Select Branch");
                return;
            }
            List<bool> listCheckboxStatus = new List<bool>();
            foreach (var item in CheckBoxNightList)
            {
                listCheckboxStatus.Add(item.IsChecked.Value);
            }

            InfoUpdate infoUpdate = new InfoUpdate();
            infoUpdate.Release = cmbRelease_Nightly.Text;
            infoUpdate.Branch = cmbBranch.Text;
            infoUpdate.Option = cmbOption.Text;
            infoUpdate.TrashCleaner = chbox_DeleteTrash.IsChecked.Value;
            infoUpdate.path_to_root = txtPathRoot.Text;
            infoUpdate.Time_Update = Time_now;

            nightlyUpgradeManager = new NightlyUpgradeManager(infoUpdate, listCheckboxStatus);

            for (int i = 0; i < 5; i++)
            {
                if (listCheckboxStatus[i]) // jezeli chcemy zrobic upgrade 
                {
                    FittingSoftware_List[i].Upgrade_FS = new Upgrade_FittingSoftware(infoUpdate);
                    if (txtPathRoot.Text == "")
                    {
                        FittingSoftware_List[i].Upgrade_FS.info.path_to_root = "";
                    }
                    else
                    {
                        FittingSoftware_List[i].Upgrade_FS.info.path_to_root = txtPathRoot.Text;
                    }
                                        
                }
            }
            // wlaczyc timer w mainwindow dla sprawdzania czy godzina już jest ok
            checkTime_Timer.Start();
           // lblTime_toUpgrade.Content = "Start Time: " + (FittingSoftware_List[0].Upgrade_FS.info.Time_Update.Hour) + " H " + (FittingSoftware_List[0].Upgrade_FS.info.Time_Update.Minute) + " M";
        }
        private void chbox_DeleteTrash_Checked(object sender, RoutedEventArgs e)
        {
           
        }

        private void chbox_DeleteTrash_Unchecked(object sender, RoutedEventArgs e)
        {
          
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {

            if (cmbBranch.SelectedIndex == -1 && txtPathRoot.Text == "")
            {
                MessageBox.Show("Select Branch");
                return;
            }
            List<bool> listCheckboxStatus = new List<bool>();
            foreach (var item in CheckBoxNightList)
            {
                listCheckboxStatus.Add(item.IsChecked.Value);
            }

            InfoUpdate infoUpdate = new InfoUpdate();
            infoUpdate.Release = cmbRelease_Nightly.Text;
            infoUpdate.Branch = cmbBranch.Text;
            infoUpdate.Option = cmbOption.Text;
            infoUpdate.TrashCleaner = chbox_DeleteTrash.IsChecked.Value;
            infoUpdate.path_to_root = txtPathRoot.Text;
            infoUpdate.Time_Update = Time_now;

            nightlyUpgradeManager = new NightlyUpgradeManager(infoUpdate, listCheckboxStatus);

            for (int i = 0; i < 5; i++)
            {
                if (listCheckboxStatus[i]) // jezeli chcemy zrobic upgrade 
                {
                    FittingSoftware_List[i].Upgrade_FS = new Upgrade_FittingSoftware(infoUpdate);
                    if (txtPathRoot.Text == "")
                    {
                        FittingSoftware_List[i].Upgrade_FS.info.path_to_root = "";
                    }
                    else
                    {
                        FittingSoftware_List[i].Upgrade_FS.info.path_to_root = txtPathRoot.Text;
                    }

                }
            }


            for (int i = 0; i < 5; i++)
            {
                FittingSoftware_List[i].getNewFSPath();
            }
            string text = "";

            for (int i = 0; i < 5; i++)
            {
                FittingSoftware_List[i].Task_GetNewBuild.Wait();
                text += FittingSoftware_List[i].Name_FS + " :\n";
                text += FittingSoftware_List[i].PathToNewVerFS + "\n";
                FittingSoftware_List[i].PathToNewVerFS = "";
                FittingSoftware_List[i].Upgrade_FS = null;
            }

            MessageBox.Show(text);

        }

        private void btnAll_Night_Click(object sender, RoutedEventArgs e)
        {
            byte counter = 0;
            foreach (var item in CheckBoxNightList)
            {
                if (item.IsChecked.Value)
                {
                    counter++;
                }
            }
            bool value_ForCheckbox = true;

            if (counter == 5)
            {
                value_ForCheckbox = false;
            }


            foreach (var item in CheckBoxNightList)
            {
                item.IsChecked = value_ForCheckbox;
            }

        }

        public void updateClockUI()
        {
            string time = "";
            if (Time_now.Hour < 10)
            {
                time = $"0{Time_now.Hour}:";
            }
            else
            {
                time = $"{Time_now.Hour}:";
            }
            if (Time_now.Minute < 10)
            {
                time += $"0{Time_now.Minute}";
            }
            else
            {
                time += $"{Time_now.Minute}";
            }
            lblTime_Nightly.Content = time;
        }

        private void InstallByNight_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                FittingSoftware_List[i].Upgrade_FS = null;
            }
            lblTime_toUpgrade.Content = "";
        }
        

        List<string> listOfPeople = new List<string>();

        public void SetButtons(bool state)
        {
            btnRandomize.IsEnabled = state;
            btnRemove.IsEnabled = state;
            btnRemoveAll.IsEnabled = state;
        }
        public void Refresh()
        {
            SetButtons(false);

            if (File.Exists("Teammates.txt"))
            {
                numberOfPeople = File.ReadLines("Teammates.txt").Count();

                if (numberOfPeople < 1)
                    SetButtons(false);

                else
                    SetButtons(true);

                labelCounter.Content = numberOfPeople;
                listOfPeople.Clear();

                foreach (string line in File.ReadLines("Teammates.txt", Encoding.UTF8))
                    listOfPeople.Add(line);
            }

            else
            {
                listboxTeam.Items.Clear();
                numberOfPeople = 0;
                labelCounter.Content = numberOfPeople;
                listOfPeople.Clear();
            }

            if (numberOfPeople > 0 && File.Exists("Teammates.txt"))
            {
                listboxTeam.Items.Clear();

                foreach (string line in File.ReadLines("Teammates.txt", Encoding.UTF8))
                    listboxTeam.Items.Add(line);
            }
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            txtbxPerson.Text = txtbxPerson.Text.Replace(" ", "");

            if (txtbxPerson.Text == "")
                return;

            txtbxPerson.Text = txtbxPerson.Text.ToLower();

            if (!File.Exists("Teammates.txt"))
            {
                StreamWriter sw = File.CreateText("Teammates.txt");

                sw.WriteLine(txtbxPerson.Text);
                sw.Close();
            }
            else
            {
                foreach (string line in File.ReadLines("Teammates.txt", Encoding.UTF8))
                {
                    if (line.Equals(txtbxPerson.Text))
                    {
                        MessageBox.Show(txtbxPerson.Text + " is already on the list.", "Oh man");
                        txtbxPerson.Text = "";
                        return;
                    }
                }

                StreamWriter sw = File.AppendText("Teammates.txt");

                sw.WriteLine(txtbxPerson.Text);
                sw.Close();
            }

            txtbxPerson.Text = "";
            Refresh();
        }

        private void BtnRandomize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Int32.Parse(txtbxTeamSize.Text) <= 0)
                {
                    MessageBox.Show("Insert real team size", "Very Funny");
                    return;
                }

                int teamSize = Int32.Parse(txtbxTeamSize.Text);
                double temp = listOfPeople.Count() / teamSize;

                double teamsNumber = Math.Ceiling(temp);

                ResultWindow result1 = new ResultWindow(teamsNumber, teamSize, listOfPeople);
                result1.Show();
            }
            catch
            {
                MessageBox.Show("Insert real team size", "Very Funny");
            }
        }

        private void BtnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Teammates.txt"))
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Remove all people?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    File.Delete("Teammates.txt");
                    //MessageBox.Show("All poeple removed", "BOOM");
                    Refresh();
                    return;
                }
                return;
            }

            MessageBox.Show("There is no one to remove!", "Are you kidding me?");
        }

        private void btnRemove_Click_DSZY(object sender, RoutedEventArgs e)
        {
            if (this.listboxTeam.SelectedIndex >= 0)
            {
                var newlist = listboxTeam.SelectedItems.Cast<string>().ToList();

                foreach (string s in newlist)
                    listboxTeam.Items.Remove(s);

                File.Delete("Teammates.txt");
                StreamWriter sw = File.CreateText("Teammates.txt");

                foreach (var person in listboxTeam.Items)
                    sw.WriteLine(person);

                sw.Close();
                Refresh();

                return;
            }
            MessageBox.Show("Select person to remove.");
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