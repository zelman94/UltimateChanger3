using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UltimateChanger
{
    /// <summary>
    /// Interaction logic for AdvanceWindowInstalla.xaml
    /// </summary>
    /// 
    public partial class AdvanceWindowInstalla : Window
    {
        public FileOperator fileOperator = new FileOperator();
        public List<string> PathTobuilds = new List<string>();
        public List<string> PathTobuildsUI = new List<string>();
        List<string> Paths = new List<string>();
        DispatcherTimer FindingPaths;
        DataBaseManager databaseManager;

        public Task TaskFindBuilds { get; private set; }

        public AdvanceWindowInstalla(DataBaseManager databaseManager_ = null)
        {
            InitializeComponent();
            databaseManager = databaseManager_;

            cmbRelease.ItemsSource = databaseManager.executeSelect("select DISTINCT release from builds where type = 'FULL' order by release");
            try
            {
                cmbRelease.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }
            cmbMode.ItemsSource = new List<string>() { { "IP" }, { "RC" }, { "Master" } };
            cmbMode.SelectedIndex = 0;

            

            try
            {
                cmbAbout.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }

            FindingPaths = new DispatcherTimer();
            FindingPaths = new DispatcherTimer();
            FindingPaths.Tick += updateListUI;
            FindingPaths.Interval = new TimeSpan(0, 0, 1);
            this.Background = ((MainWindow)System.Windows.Application.Current.MainWindow).Background;
           

            txtpathToBuilds.Foreground = ((MainWindow)System.Windows.Application.Current.MainWindow).rbnHI_1.Foreground;
            txtpathToBuilds.BorderBrush = ((MainWindow)System.Windows.Application.Current.MainWindow).rbnHI_1.Foreground;
            var converter = new System.Windows.Media.BrushConverter();

            btnCancelAdvance.Background = (Brush)converter.ConvertFromString("#FF616161");
            btnCancelAdvance.Foreground = (Brush)converter.ConvertFromString("#E5FFFFFF");

            btnInstallFSs.Background = (Brush)converter.ConvertFromString("#FF616161");
            btnInstallFSs.Foreground = (Brush)converter.ConvertFromString("#E5FFFFFF");

            btnFindPaths.Background = (Brush)converter.ConvertFromString("#FF616161");
            btnFindPaths.Foreground = (Brush)converter.ConvertFromString("#E5FFFFFF");
            progressAdvanceInstall.Visibility = Visibility.Hidden;
            //cmbLastselected.ItemsSource = getLastUsedPaths();
        }

        public void updateListUI(object sender, EventArgs e)
        {
            if (TaskFindBuilds.Status != TaskStatus.Running)
            {
                ListBoxBuilds.ItemsSource = PathTobuildsUI;
                databaseManager.Advance_AddPath(Paths[0], PathTobuilds); //Paths - root //PathTobuilds - cale pathy to setup.exe
                FindingPaths.Stop();
                progressAdvanceInstall.Visibility = Visibility.Hidden;
            }
            else
            {
                progressAdvanceInstall.Value += 10;
     
                if (progressAdvanceInstall.Value == 100 )
                {
                    progressAdvanceInstall.Value = 0;                    
                }
            }
        }

        public List<string> getLastUsedPaths()
        {
            if (!File.Exists(@"C:\Program Files\UltimateChanger\Settings\LastUsedRootPaths.txt"))
            {
                try
                {
                    File.Create(@"C:\Program Files\UltimateChanger\Settings\LastUsedRootPaths.txt");
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.ToString());
                    return null;
                }
                
                return null;
            }

            List<string> Roots = new List<string>();

            Roots = File.ReadAllLines(@"C:\Program Files\UltimateChanger\Settings\LastUsedRootPaths.txt").ToList();

            return Roots;
        }
        public void writeRootPath(string path)
        {
            List<string> Roots = new List<string>();

            Roots = File.ReadAllLines(@"C:\Program Files\UltimateChanger\Settings\LastUsedRootPaths.txt").ToList();
            if (Roots.Count == 10)
            {
                Roots.Clear();
            }
            Roots.Add(path);

            File.WriteAllLines(@"C:\Program Files\UltimateChanger\Settings\LastUsedRootPaths.txt",Roots);
        }

        public List<string> getPaths()
        {
            List<string> Paths = new List<string>();

            try
            {
                Paths = txtpathToBuilds.Text.Split(';').ToList();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }

            return Paths;
        }
        public List<string> findBuildsInPaths(List<string> Paths, string root)
        {

            List<string> builds = new List<string>();

            foreach (var item in Paths)
            {
                try
                {
                    List<string> tmp = Directory.GetFiles(item, "Setup.exe", SearchOption.AllDirectories).ToList();
                    foreach (var item2 in tmp)
                    {
                        builds.Add(item2.Remove(0, root.Length));
                    }
                    PathTobuilds.AddRange(tmp);
                }
                catch (Exception)
                {

                }
               
                // builds.AddRange(tmp); // dodaje liste wszystkich exe z pathow 
            }

            return builds;

        }

        private void BtnFindPaths(object sender, RoutedEventArgs e)
        {
            if (txtpathToBuilds.Text == "")
            {
                MessageBox.Show("Add Path(s)");
                return;
            }
            writeRootPath(txtpathToBuilds.Text);
            List<string> paths_Azure = databaseManager.Advance_GetPath(txtpathToBuilds.Text);
            if (paths_Azure.Count > 0) // jezeli jest cos na azure 
            {
                updateUIListPaths(paths_Azure);
                return;
            }
            else // jezeli nie ma to trzeba dodac!
            {
                Paths = getPaths();
                // dodanie start dl aprogress bar
                progressAdvanceInstall.Visibility = Visibility.Visible;
                string root = txtpathToBuilds.Text;
                FindingPaths.Start();
                TaskFindBuilds = Task.Run(() =>
                {
                    PathTobuildsUI = findBuildsInPaths(Paths, root);
                });
                cmbLastselected.ItemsSource = getLastUsedPaths();
               
            }            
        }

        private void ListBoxBuilds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnCancelAdvance_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnInstallFSs_Click(object sender, RoutedEventArgs e)
        {
            List<string> listOfSelectedPaths = new List<string>();
            foreach (var item in ListBoxBuilds.SelectedItems)
            {
                foreach (var item2 in PathTobuilds)
                {
                    if (item2.ToString().Contains(item.ToString()))
                    {
                        listOfSelectedPaths.Add(item2); // lista pelnych buildow ktore zostaly zaznaczone
                    }
                }
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).listOfPathsToInstall = listOfSelectedPaths; // przypisanie listy do glownego okna

            if ( ((MainWindow)System.Windows.Application.Current.MainWindow).uninstallTimer.IsEnabled)
            {
                MessageBox.Show("uninstallation in progress try later");
            }
            else // uruchamianie instalacji kolejkowej na oknie glownym
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).ProgressInstallation.Visibility = Visibility.Visible;
                ((MainWindow)System.Windows.Application.Current.MainWindow).InstallTimer.Start();
            }
            this.Close();
        }

        private void cmbLastselected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbLastselected.Items.Refresh();
            if (cmbLastselected.SelectedIndex !=-1)
            {
                txtpathToBuilds.Text = cmbLastselected.Text;
                if (databaseManager != null)
                {
                    updateUIListPaths(databaseManager.Advance_GetPath(txtpathToBuilds.Text));
                }                
            }
        }

        private void updateUIListPaths(List <string> listOfPaths)  // aktualizacja listy paths po pobraniu danych z SQL
        {
            PathTobuilds = listOfPaths;

            List<string> UIpaths = new List<string>();

            foreach (var item in listOfPaths)
            {
                UIpaths.Add(item.Remove(0, txtpathToBuilds.Text.Length));
            }

            ListBoxBuilds.ItemsSource = UIpaths;
        }

        private void cmbMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbMode.Items.Refresh();
            cmbAbout.Items.Refresh();
            cmbRelease.Items.Refresh();
            cmbAbout.ItemsSource = databaseManager.executeSelect($"select DISTINCT about from builds where type = 'FULL' AND RELEASE = '{cmbRelease.Text}' AND MODE like '%{cmbMode.Text}%' order by about DESC");
            cmbAbout_SelectionChanged(new object(), null);
        }

        private void cmbAbout_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbAbout.Items.Refresh();
            cmbMode.Items.Refresh();
            cmbRelease.Items.Refresh();
            txtpathToBuilds.Text = @"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\FullInstaller-"; // common part...
            updateUIListPaths(databaseManager.executeSelect($"select path from builds where type = 'FULL' AND RELEASE = '{cmbRelease.Text}' AND MODE like '%{cmbMode.Text}%' AND ABOUT = '{cmbAbout.Text}'"));
        }

        private void cmbRelease_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbRelease.Items.Refresh();
            cmbMode.Items.Refresh();
            cmbAbout.Items.Refresh();
            cmbAbout.ItemsSource = databaseManager.executeSelect($"select DISTINCT about from builds where type = 'FULL' AND RELEASE = '{cmbRelease.Text}' AND MODE like '%{cmbMode.Text}%' order by about DESC");
            try
            {
                cmbAbout.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }
            cmbAbout_SelectionChanged(new object(), null);
        }
    }
}
