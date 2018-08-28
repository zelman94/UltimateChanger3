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
        string PATHS = "";
        public UpdateWindow(string paths, string info)
        {
            PATHS = paths;            
            InitializeComponent();
            //txtInfoUpdate.Text = info;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string IP = "";
            try
            {
                string strHostName = "";
                strHostName = System.Net.Dns.GetHostName();
                IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
                IPAddress[] addr = ipEntry.AddressList;
                IP = addr[2].ToString();
            }
            catch (Exception)
            {
                return;
            }
            //Initializing a new xml document object to begin reading the xml file returned
            XmlDocument doc = new XmlDocument();
            doc.Load("http://www.freegeoip.net/xml");
            XmlNodeList nodeLstCity = doc.GetElementsByTagName("CountryCode");
            IP = nodeLstCity[0].InnerText; // IP to tutaj kod państwa w któym jestem
            string[] lines = PATHS.Split(Environment.NewLine.ToCharArray());
            try
            {
                if (IP == "PL")
                {
                    try
                    {
                        System.Diagnostics.Process.Start(lines[0]);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(lines[2]);
                        }
                        catch (Exception)
                        {
                        }
                    }                 
                }
                else if (IP == "DK")
                {
                    try
                    {
                        System.Diagnostics.Process.Start(lines[4]);
                    }
                    catch (Exception)
                    { }                   
                }
                else
                {
                    MessageBox.Show("ERROR :C");
                   
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

        private void txtInfoUpdate_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
