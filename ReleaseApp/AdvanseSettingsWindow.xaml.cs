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

namespace UltimateChanger
{
    /// <summary>
    /// Logika interakcji dla klasy AdvanseSettingsWindow.xaml
    /// </summary>
    public partial class AdvanseSettingsWindow : Window
    {
        public AdvanseSettingsWindow()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {

            ((MainWindow)System.Windows.Application.Current.MainWindow).Advance_1 = txtsettlog1.Text;
            ((MainWindow)System.Windows.Application.Current.MainWindow).Advance_2 = txtsettlog2.Text;
            ((MainWindow)System.Windows.Application.Current.MainWindow).Advance_3 = txtsettlog3.Text;
        }
    }
}
