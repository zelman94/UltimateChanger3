using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Xml;

namespace UltimateChanger
{
    public partial class UpdateWindow : Window
    {
        string PATHS = "", INFO = "";
        public UpdateWindow(string paths, string info)
        {
            PATHS = paths;
            INFO = info;
            InitializeComponent();
            txtpath.Text = paths;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string SSC = "", KBN="";
            string[] lines = PATHS.Split(Environment.NewLine.ToCharArray());
                try
                {

                    foreach (var item in lines)
                    {
                        if (item.Contains("10.128.3.1"))
                        {
                            SSC = item;
                        }
                        if (item.Contains("demant.com"))
                        {
                            KBN = item;
                        }
                    }
                    try
                    {
                        //string tmp = Environment.CurrentDirectory + @"\Updater" + $"\\UltimateChangerUpdater.exe ";
                        System.Diagnostics.Process.Start(SSC);
                    }
                    catch (Exception )
                    {
                        try
                        {                        
                            System.Diagnostics.Process.Start(KBN);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("ERROR :C");
                            this.Close();
                        }
                    }
                
                }
                catch (Exception x)
                {
                    MessageBox.Show("Error \n" + x.ToString());
                }
            ((MainWindow)System.Windows.Application.Current.MainWindow).Close();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnInfoUpdate_Click(object sender, RoutedEventArgs e)
        {          
            MessageBox.Show("New version contain:\n" + INFO);
        }

        private void btnExportSettings_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).btnExportSettings_Click(new object(),new RoutedEventArgs());
        }

        private void txtInfoUpdate_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
