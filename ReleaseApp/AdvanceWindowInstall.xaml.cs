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

        public Task TaskFindBuilds { get; private set; }

        public AdvanceWindowInstalla()
        {
            InitializeComponent();
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
        }

        public void updateListUI(object sender, EventArgs e)
        {
            if (TaskFindBuilds.Status != TaskStatus.Running)
            {
                ListBoxBuilds.ItemsSource = PathTobuildsUI;
                FindingPaths.Stop();
            }
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
                List<string> tmp = Directory.GetFiles(item, "Setup.exe", SearchOption.AllDirectories).ToList();
                foreach (var item2 in tmp)
                {
                    builds.Add(item2.Remove(0, root.Length));
                }
                PathTobuilds.AddRange(tmp);
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
            Paths = getPaths();
            MessageBox.Show("please wait");
            string root = txtpathToBuilds.Text;
            FindingPaths.Start();
            TaskFindBuilds = Task.Run(() =>
            {
                PathTobuildsUI = findBuildsInPaths(Paths, root);
            });

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
                ((MainWindow)System.Windows.Application.Current.MainWindow).InstallTimer.Start();
            }
            this.Close();
        }
    }
}
