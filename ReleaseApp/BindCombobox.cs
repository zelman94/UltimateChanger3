﻿using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UltimateChanger
{
    class BindCombobox
    {
        private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SortedDictionary<string, string> market = new SortedDictionary<string, string>
        {
            { "Australia (AU)", "AU"},
            { "Denmark (DK)", "DK"},
            { "Germany (DE)", "DE"},
            { "United Kingdom (UK)", "UK"},
            { "United States (US)", "US"},
            { "Canada (CA)", "CA"},
            { "Spain (ES)", "ES"},
            { "New Zeland (NZ)", "NZ"},
            { "Switzerland (CH)", "CH"},
            { "Finland (FI)", "FI"},
            { "France (FR)", "FR"},
            { "Italy (IT)", "IT"},
            { "Japan (JP)", "JP"},
            { "Korea (KR)", "KR"},
            { "Norway (NO)", "NO"},
            { "Nederland (NL)", "NL"},
            { "Brazil (BR)", "BR"},
            { "Poland (PL)", "PL"},
            { "Portugal (PT)", "PT"},
            { "Sweden (SE)", "SE"},
            { "Singapore (SG)", "SG"},
            { "PRC China (CN)", "CN"},
            { "South Africa (ZA)", "ZA"},
            { "Default", "Default"}
        };

        static public List<string> marketIndex = new List<string>()
        {

            {"AU"},
            {"BR"},
            {"CA"},
            {"Default"},
            {"DK"},
            {"FI"},
            {"FR"},
            {"DE"},
            {"IT"},
            {"JP"},
            {"KR"},
            {"NL"},
            {"NZ"},
            {"NO"},
            {"PL"},
            {"PT"},
            {"CN"},
            {"SG"},
            {"ZA"},
            {"ES"},
            {"SE"},
            {"CH"},
            {"UK"},
            {"US"},
            {"he"}
        };
        SortedDictionary<string, string> mode = new SortedDictionary<string, string>
            {
                { "ALL", "ALL"},
                { "DEBUG", "DEBUG"},
                { "ERROR", "ERROR"}
            };
        SortedDictionary<string, string> Settings = new SortedDictionary<string, string>
            {
                { "Default", "Default"},
                { "New file with Start FS", "DeleteWithWtart"},
                { "One file 100 MB", "OneF100"}
            };


        public static SortedDictionary<string, string> BrandtoFS = new SortedDictionary<string, string>
            {
                { "Oticon", "Genie"},
                { "Bernafon", "Oasis"},
                { "Sonic", "ExpressFit"},
                { "Medical", "Genie Medical"},
                { "Cumulus", "Cumulus"}
            };

        static public List<string> listPP = new List<string>()
        {
            {"OPN 1"},
            {"OPN 2"},
            {"OPN 3"},
            {"Siya 1"},
            {"Siya 2"},
        };

        static public List<string> listOfSerchingOption = new List<string>() // rc albo master dla kompozycji
        {
            {"RC"},
            {"MASTER"},
            {"IP"}
        };


        public void bindlogmode()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogMode.ItemsSource = mode;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogMode.DisplayMemberPath = "Key";
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogMode.SelectedValuePath = "Value";


            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogSettings.ItemsSource = Settings;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogSettings.DisplayMemberPath = "Key";
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogSettings.SelectedValuePath = "Value";

            /////////////kompozycje
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogMode_Compo.ItemsSource = mode;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogMode_Compo.DisplayMemberPath = "Key";
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogMode_Compo.SelectedValuePath = "Value";


            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogSettings_Compo.ItemsSource = Settings;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogSettings_Compo.DisplayMemberPath = "Key";
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogSettings_Compo.SelectedValuePath = "Value";

        }

        public void setMarketCmb()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbMarket.ItemsSource = market;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbMarket.DisplayMemberPath = "Key";
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbMarket.SelectedValuePath = "Value";

            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbMarket_Compo.ItemsSource = market;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbMarket_Compo.DisplayMemberPath = "Key";
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbMarket_Compo.SelectedValuePath = "Value";
        }

        public void setFScomboBox() // full medium
        {
            List<string> bind = new List<string> { "Genie", "Oasis", "ExpressFit", "GenieMedical", "HearSuite"};
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbBrandstoinstall.ItemsSource = bind;
        }
        public void setFScomboBox_compositions()
        {
            List<string> bind = new List<string> { "Genie", "Oasis", "ExpressFit", "GenieMedical", "HearSuite" };
            try
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbBrandstoinstall_Compo.ItemsSource = bind;
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }

        }

        public void setReleaseComboBox()
        {
            List<string> bind;//= new List<string> { "16.1", "16.2", "17.1", "17.2", "18.2", "19.1", "19.2", "20.1" }; //zamienic na odczytanie z XML 
            bind = myXMLReader.getReleases();
            List<string> tmp = new List<string>();
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease.ItemsSource = bind;
            try
            {
                List<string> bindRelease = ((MainWindow)System.Windows.Application.Current.MainWindow).
                dataBaseManager.executeSelect("select DISTINCT release from builds where type = 'FULL' order by release");
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease_Nightly.ItemsSource = bindRelease;
            }
            catch (Exception x)
            {
                Log.Error("setReleaseComboBox: ERROR :\n" + x.ToString());
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease_Nightly.ItemsSource = bind;
            }
            
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease_Compo.ItemsSource = bind;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbBuild2_Compo.ItemsSource = listOfSerchingOption;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbBuild_mode.ItemsSource = listOfSerchingOption;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbBranch.ItemsSource = new List<string>() { { "IP" }, { "RC" }, { "Master" } };
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOption.ItemsSource = new List<string>() { { "Full" } };
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOption.SelectedIndex = 0;
           
            try
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbBuild_mode.SelectedIndex = 0;
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
            }   

        }

        public void setOEMComboBox(string FS)
        {
            if (FS.Contains("GenieMedical"))
            {
                List<string> bind = new List<string> { "GenieMedical" };
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.SelectedIndex = 0;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM_Compo.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM_Compo.SelectedIndex = 0;
            }
            
            else if (FS.Contains("Oasis") || FS.Contains("Bernafon"))
            {
                List<string> bind = new List<string> { "PrivateLable", "AccuQuest", "Audilab", "Bernafon", "Costco", "GPL", "HansAnders", "Horex", "Maico", "Meditrend", "ProAkustik" };
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.SelectedIndex = 3;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM_Compo.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM_Compo.SelectedIndex = 3;
            }
            else if(FS.Contains("ExpressFit") || FS.Contains("Sonic"))
            {
                List<string> bind = new List<string> { "Sonic" };
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.SelectedIndex = 0;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM_Compo.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM_Compo.SelectedIndex = 0;
            }
            else if (FS.Contains("HearSuite") || FS.Contains("Philips"))
            {
                List<string> bind = new List<string> { "Philips" };
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.SelectedIndex = 0;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM_Compo.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM_Compo.SelectedIndex = 0;
            }
            else if(FS.Contains("Genie") || FS.Contains("Oticon"))
            {
                List<string> bind = new List<string> { "Audigy", "Audika", "Audionova", "Avada", "Kind", "Oticon", "VA" };
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.SelectedIndex = 5;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM_Compo.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM_Compo.SelectedIndex = 5;
            }


        }

        public List<string> GetFS()
        {
            List<string> lista = new List<string>();


            return lista;
        }

        public List<string> GetBuilds(string path)
        {
            List<string> lista = new List<string>();
            try
            {

                foreach (var d in System.IO.Directory.GetDirectories(path))
                {
                    var dirName = new DirectoryInfo(d).Name;
                    lista.Add(dirName);
                }


            }
            catch (Exception)
            {
                return null;
            }



            return lista;
        }

        public void bindListBox()
        {
           List<MyHardware> listaHardware = myXMLReader.getHardware(); // dostaje liste instancji Hardware           

            ((MainWindow)System.Windows.Application.Current.MainWindow).ListBoxHardware.ItemsSource = MyHardware.ToNameAndID(listaHardware);

            List<string> listPerson = myXMLReader.getTeamPerson(); // dostaje liste instancji Hardware  
            listPerson.Sort();
            ((MainWindow)System.Windows.Application.Current.MainWindow).ListTeamPerson.ItemsSource = listPerson;

            ((MainWindow)System.Windows.Application.Current.MainWindow).ListBoxOfAvailableFeautures.ItemsSource = myXMLReader.getFiczurs();


            myXMLReader.GetStylesInRelease("19.1");
        }

        public List<string> getAllPathsOem(string OEM, int FSnr, List<pathAndDir> allPaths) 
        {
            List<string> PathsOem = new List<string>();
            if (FSnr!=-1)
            {
                try
                {
                    foreach (var item in allPaths[FSnr].path)
                    {
                        if (item.Contains(OEM))
                        {
                            PathsOem.Add(item);
                        }
                    }
                }
                catch (Exception) // pewnie kompozycja
                {
                    try
                    {
                        foreach (var item in allPaths[0].path)
                        {
                            if (item.Contains(OEM))
                            {
                                PathsOem.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                   
                }
                

            }


            return PathsOem;
        }

    }



}
