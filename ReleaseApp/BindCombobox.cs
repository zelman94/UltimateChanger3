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
            { "", "NA"}
        };

        static public List<string> marketIndex = new List<string>()
        {
            {"NA"},
            {"AU"},
            {"BR"},
            {"CA"},
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

        public void bindlogmode()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogMode.ItemsSource = mode;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogMode.DisplayMemberPath = "Key";
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogMode.SelectedValuePath = "Value";


            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogSettings.ItemsSource = Settings;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogSettings.DisplayMemberPath = "Key";
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbLogSettings.SelectedValuePath = "Value";
        }

        public void setMarketCmb()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbMarket.ItemsSource = market;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbMarket.DisplayMemberPath = "Key";
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbMarket.SelectedValuePath = "Value";
        }

        public void setFScomboBox()
        {
            List<string> bind = new List<string> { "Oticon", "Bernafon", "Sonic", "GenieMedical", "Cumulus", "Oticon_PRE", "Bernafon_PRE", "Sonic_PRE", "GenieMedical_PRE", "Cumulus_PRE" };
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbBrandstoinstall.ItemsSource = bind;
        }
        public void setReleaseComboBox()
        {
            List<string> bind = new List<string> { "16.1", "16.2", "17.1", "17.2", "18.2", "19.1", "19.2", "20.1" };
            List<string> tmp = new List<string>();
            ((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease.ItemsSource = bind;

            int rok = System.DateTime.Today.Year;
            int release = rok - 2000;

            int miesiac = System.DateTime.Today.Month;
            int wydanie = miesiac - 6;

            foreach (var item in bind)
            {
                if (item.Contains(release.ToString()))
                {
                    tmp.Add(item);
                }
            }
            if (tmp.Count == 1)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease.SelectedItem = "19.1";// tmp[0];
            }
            else
            {
                if (wydanie < 0)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease.SelectedItem = release + ".1";
                }
                else
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease.SelectedItem = release + ".2";
                }
            }

        }



        public void setOEMComboBox(string FS)
        {
            if (FS.Contains("Oticon"))
            {
                List<string> bind = new List<string> { "Audigy", "Audika", "Audionova", "Avada", "Kind", "Oticon", "VA" };
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.SelectedIndex = 5;
            }
            else if (FS.Contains("Bernafon"))
            {
                List<string> bind = new List<string> { "PrivateLable", "AccuQuest", "Audilab", "Bernafon", "Costco", "GPL", "HansAnders", "Horex", "Maico", "Meditrend", "ProAkustik" };
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.SelectedIndex = 3;
            }
            else
            {
                List<string> bind = new List<string> { "Sonic" };
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.ItemsSource = bind;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cmbOEM.SelectedIndex = 0;
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




    }



}
