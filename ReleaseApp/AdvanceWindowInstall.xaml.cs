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
        List<Label> lableListForUi = new List<Label>();
        List<TextBox> listTextBoxForUi = new List<TextBox>();
        List<Button> buttonListForUi = new List<Button>();
        List<ComboBox> comboBoxListForUi = new List<ComboBox>();
        List<CheckBox> checkBoxListForUi = new List<CheckBox>();
        DispatcherTimer FindingPaths;
        DataBaseManager databaseManager;

        public Task TaskFindBuilds { get; private set; }

        public AdvanceWindowInstalla(DataBaseManager databaseManager_ = null)
        {
            InitializeComponent();
            databaseManager = databaseManager_;

            List <string> bindRelease = databaseManager.executeSelect("select DISTINCT release from builds where type = 'FULL' order by release");
            cmbRelease.ItemsSource = bindRelease;
            try
            {
                cmbRelease.SelectedIndex = bindRelease.FindIndex(x => x.StartsWith(((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease.Text));

                // ((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease
            }
            catch (Exception)
            {
                cmbRelease.SelectedIndex = 0;
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

            setDefaultSkin();
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
                ((MainWindow)System.Windows.Application.Current.MainWindow).ProgressInstallation.ToolTip = "Installation in progress";
            }
            this.Close();
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
            try
            {
                cmbAbout.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }

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

        public void setDefaultSkin()
        {
            foreach (Label tb in FindLogicalChildren<Label>(this)) // dziala
            {
                lableListForUi.Add(tb);
            }

            foreach (TextBox item in FindLogicalChildren<TextBox>(this))
            {
                listTextBoxForUi.Add(item);
            }
            foreach (Button item in FindLogicalChildren<Button>(this))
            {
                buttonListForUi.Add(item);
            }
            foreach (ComboBox item in FindLogicalChildren<ComboBox>(this))
            {
                comboBoxListForUi.Add(item);
            }
            foreach (CheckBox item in FindLogicalChildren<CheckBox>(this))
            {
                checkBoxListForUi.Add(item);
            }

            //USTAWIENIA LABELI
            foreach (var item in lableListForUi)
            {
                item.Foreground = ((MainWindow)System.Windows.Application.Current.MainWindow).lblSavedTime.Foreground;
            }

            //USTAWIENIA BOXÓW
            foreach (var item in listTextBoxForUi)
            {
                item.Foreground = ((MainWindow)System.Windows.Application.Current.MainWindow).txtnewTeamMember.Foreground;
                item.BorderBrush = ((MainWindow)System.Windows.Application.Current.MainWindow).txtnewTeamMember.BorderBrush;
            }
            var converter = new System.Windows.Media.BrushConverter();

            //USTAWIENIA BUTTONÓW
            foreach (var item in buttonListForUi)
            {
                item.Background = ((MainWindow)System.Windows.Application.Current.MainWindow).btnNewPrecon.Background;
                item.Foreground = ((MainWindow)System.Windows.Application.Current.MainWindow).btnNewPrecon.Foreground;
                item.BorderBrush = ((MainWindow)System.Windows.Application.Current.MainWindow).btnNewPrecon.BorderBrush;
                item.Opacity = ((MainWindow)System.Windows.Application.Current.MainWindow).btnNewPrecon.Opacity;
                item.MaxWidth = ((MainWindow)System.Windows.Application.Current.MainWindow).btnNewPrecon.MaxWidth;
            }
            //USTAWIENIA COMBOBOXÓW
            foreach (var item in comboBoxListForUi)
            {
                item.Foreground = ((MainWindow)System.Windows.Application.Current.MainWindow).cmbBrandstoinstall.Foreground;
                item.BorderBrush = ((MainWindow)System.Windows.Application.Current.MainWindow).cmbBrandstoinstall.BorderBrush;

            }

            this.Background = ((MainWindow)System.Windows.Application.Current.MainWindow).Background;
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

    }
}
