using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for ChangeLog.xaml
    /// </summary>
    public partial class ChangeLog : Window
    {
        public ChangeLog(DataBaseManager dataBase_Manager, string ver)
        {
            InitializeComponent();
            if (dataBase_Manager.SQLConnection == null)
            {
                FileOperator fileOperator = new FileOperator();
                txtChangeLog.Content = fileOperator.getChangeLog();
            }
            else
            {
                txtChangeLog.Content = dataBase_Manager.getInfo_AboutBuild(ver);
            }
           
           
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
