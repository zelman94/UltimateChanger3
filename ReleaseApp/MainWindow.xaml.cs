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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Data.SqlClient;
using MySql;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Forms.Integration;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.Net;
using System.Data;
using Rekurencjon; // logi


[assembly: System.Reflection.AssemblyVersion("3.3.4.0")]
namespace UltimateChanger
{//
    public partial class MainWindow : Window
    {
 

        public MainWindow()
        {
            
          
                var exists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
                if (exists) // jezeli wiecej niz 1 instancja to nie uruchomi sie
                {
                    System.Environment.Exit(1);
                }
               
                InitializeComponent();
                try
                {
                    przegladarka.Navigate("http://confluence.kitenet.com/display/SWSQA/Ultimate+Changer");
                }
                catch (Exception)
                {

                }
             
              
            
        }
        private void Window_Closing_1(object sender, CancelEventArgs e) // closing window by X button
        {


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void btnFS_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Click_btnNoah(object sender, RoutedEventArgs e)
        {

        }

        private void btnFakeV_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Gearbox(object sender, RoutedEventArgs e)
        {

        }

        private void Downgrade(object sender, RoutedEventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdvancelogs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnChange_mode_log(object sender, RoutedEventArgs e)
        {

        }

        private void btnSavelogs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_logs(object sender, RoutedEventArgs e)
        {

        }

        private void btnHattori_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFSRun(object sender, RoutedEventArgs e)
        {

        }

        private void btnuninstal_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdvanceInstall_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btninstal_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHoursUp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHoursDown_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMinutesUp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMinutesDown_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnChangeDate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnResetDate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddPersonToList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void btnClearListTeamPerson_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void btnRANDHI_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExportData_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClearTable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddNewHardware_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClearFields_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnIdentify_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLogToDB_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbnStartwithWindows_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnNotStartwithWindows_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnDeletelogs_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnholdlogs_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnLogsAll_YES_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnLogsAll_NO_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnDefaultNormal_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnDefaultSilent_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnNormalSize_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnBiggerSize_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnTurnOnDevMode_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnTurnOffDevMode_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Light_skin_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Dark_skin_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnGenieImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGenieMedicalImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExpressfitImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHearSuiteImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOasisImage_Click(object sender, RoutedEventArgs e)
        {

        }
        //________________________________________________________________________________________________________________________________________________


        public void checkbox(object sender, RoutedEventArgs e)
        {
            btnUpdate.IsEnabled = true;
            btnDeletelogs.IsEnabled = true;
            btnSavelogs.IsEnabled = true;
            btnFS.IsEnabled = true;
            btnHattori.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnLogMode.IsEnabled = true;
            btnAdvancelogs.IsEnabled = true;
            cmbLogMode.IsEnabled = true;
            cmbLogSettings.IsEnabled = true;
            cmbMarket.IsEnabled = true;
            btnuninstal.IsEnabled = true;
            // kompozycje:
            cmbMarket_Compo.IsEnabled = true;
            cmbLogMode_Compo.IsEnabled = true;
            cmbLogSettings_Compo.IsEnabled = true;
            btnAdvancelogs_Compo.IsEnabled = true;
            btnLogMode_Compo.IsEnabled = true;
            btnSavelogs_Compo.IsEnabled = true;
            btnFS_Compo.IsEnabled = true;
            btnHattori_Compo.IsEnabled = true;
            btnDeletelogs_Compo.IsEnabled = true;
            btnUpdate_Compo.IsEnabled = true;
            btnuninstal_Compo.IsEnabled = true;

            cmbMarket_Compo.SelectedIndex = 1;
            cmbLogMode_Compo.SelectedIndex = 0;

           
        }
        public void uncheckbox(object sender, RoutedEventArgs e)
        {

        }
        private void cmbLogMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbOEM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbbrandstoinstall_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbRelease_SelectionChanged_Compo(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbBuild2_Compo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbBuild_Compo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbMarket_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbLogMode_Compo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbRelease_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbbuild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void sliderRelease_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void sliderWeightWireless_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void rbnHI_1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbnHI_2_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ListBoxOfAvailableFeautures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListBoxHardware_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ListBoxOfAvailableStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListTeamPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnDeleteC_Compo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RBnormal_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RBsilet_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ListBoxOfAvailableTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tabControl2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

    }
    class RandomHIandHardware
    {
        public string Name_Team_member { get; set; }
        public string Family_Name { get; set; }
        public string HIL_ { get; set; }
        public string HIR_ { get; set; }
        public string Ficzur_ { get; set; }
        public string ComDev_ { get; set; }
    }

}