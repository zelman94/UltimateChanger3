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
using System.Windows.Shapes;
using System.Diagnostics;
namespace UltimateChanger
{
    public partial class DowngradeWindow : Window
    {
        public DowngradeWindow()
        {
            InitializeComponent();
        }
        private void btnFitting_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("remember to connect Fitting Link 3.0");
            try
            {
                Process.Start("explorer.exe", @"C:\Program Files\UltimateChanger\\Downgrade\Production\Dongle 2.0.2 Downgrade\");   
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }     
        }

        private void btnNoah_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(@"C:\Program Files\UltimateChanger\\Downgrade\NOAHlink WLP downgrade\NLWUpgrader_11063s.exe");
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
        }
    }
}
