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
        string PATHS = "",INFO="";
        public UpdateWindow(string paths, string info)
        {
            PATHS = paths;
            INFO = info;
            InitializeComponent();
            //txtInfoUpdate.Text = info;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //string IP = "";
            //try
            //{
            //    string strHostName = "";
            //    strHostName = System.Net.Dns.GetHostName();
            //    IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            //    IPAddress[] addr = ipEntry.AddressList;
            //    IP = addr[2].ToString();
            //}
            //catch (Exception)
            //{
            //    return;
            //}
            //Initializing a new xml document object to begin reading the xml file returned
            //XmlDocument doc = new XmlDocument();
            //try
            //{
            //    doc.Load("http://api.ipstack.com/check?access_key=b1f434893303697fffd4f9597a50e1f8&format=1"); // zrobione konto free na email paze 
            //    XmlNodeList nodeLstCity = doc.GetElementsByTagName("country_code");
            //    IP = nodeLstCity[0].InnerText; // IP to tutaj kod państwa w któym jestem
            //}
            //catch (Exception x )
            //{
            //    MessageBox.Show(x.ToString());
            //}
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
                    //System.Diagnostics.Process.Start(lines[lines.Length - 5]); // szczecin // do poprawy tu 
                    string tmp = Environment.CurrentDirectory + @"\Updater" + $"\\UltimateChangerUpdater.exe ";
                    System.Diagnostics.Process.Start(tmp, SSC);
                    }
                    catch (Exception x)
                    {
                        try
                        {
                        // System.Diagnostics.Process.Start(lines[lines.Length-1]);// dania
                        System.Diagnostics.Process.Start(Environment.CurrentDirectory + @"\Updater" + $"\\UltimateChangerUpdater.exe",$" {KBN}");
                    }
                        catch (Exception)
                        {
                            MessageBox.Show("ERROR :C");
                        }
                    }                 

                }
                catch (Exception x)
                {
                    MessageBox.Show("Error \n" + x.ToString());
                }
                this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnInfoUpdate_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(INFO);
        }

        private void txtInfoUpdate_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
