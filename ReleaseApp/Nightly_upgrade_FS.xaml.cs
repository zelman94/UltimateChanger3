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
    /// Interaction logic for Nightly_upgrade_FS.xaml
    /// </summary>

    public partial class Nightly_upgrade_FS : Window
    {

        List<FittingSoftware> FittingSoftware_list = new List<FittingSoftware>();
        List<Label> lableListForUi = new List<Label>();
        List<TextBox> listTextBoxForUi = new List<TextBox>();
        List<Button> buttonListForUi = new List<Button>();
        List<ComboBox> comboBoxListForUi = new List<ComboBox>();
        DateTime Time_now;
        public Nightly_upgrade_FS(List<FittingSoftware> FittingSoftware_list)
        {
            this.FittingSoftware_list = FittingSoftware_list;
            InitializeComponent();
            InitializeUI();
        }


        public void InitializeUI()
        {
            setDefaultSkin();
            bindComboBox();
            Time_now = new DateTime();
            Time_now = Time_now.AddHours(DateTime.Now.Hour);
            Time_now = Time_now.AddMinutes(DateTime.Now.Minute);
            updateClockUI();

        }

        public void bindComboBox()
        {
            cmbRelease.ItemsSource = ((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease.ItemsSource;
            cmbRelease.SelectedIndex = ((MainWindow)System.Windows.Application.Current.MainWindow).cmbRelease.SelectedIndex;
            cmbBranch.ItemsSource = new List<string>() {"master","rc"};
            cmbBranch.SelectedIndex = 1;
            cmbOption.ItemsSource = new List<string>() { "Full", "Medium"};
            cmbOption.SelectedIndex = 0;
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
                item.Background = ((MainWindow)System.Windows.Application.Current.MainWindow).txtnewTeamMember.Background;
                item.Foreground = ((MainWindow)System.Windows.Application.Current.MainWindow).txtnewTeamMember.Foreground;
                item.BorderBrush = ((MainWindow)System.Windows.Application.Current.MainWindow).txtnewTeamMember.BorderBrush;
                item.Opacity = ((MainWindow)System.Windows.Application.Current.MainWindow).txtnewTeamMember.Opacity;
                item.MaxWidth = ((MainWindow)System.Windows.Application.Current.MainWindow).txtnewTeamMember.MaxWidth;
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

        private void btnHoursUp_Click(object sender, RoutedEventArgs e)
        {
            Time_now = Time_now.AddHours(1);
            updateClockUI();
        }

        private void btnHoursDown_Click(object sender, RoutedEventArgs e)
        {
            Time_now = Time_now.AddHours(-1);
            updateClockUI();
        }

        private void btnMinutesUp_Click(object sender, RoutedEventArgs e)
        {
            Time_now = Time_now.AddMinutes(1);
            updateClockUI();
        }

        private void btnMinutesDown_Click(object sender, RoutedEventArgs e)
        {
            Time_now = Time_now.AddMinutes(-1);
            updateClockUI();
        }

        public void updateClockUI()
        {
            string time = "";
            if (Time_now.Hour < 10)
            {
                time = $"0{Time_now.Hour}:";
            }
            else
            {
                time = $"{Time_now.Hour}:";
            }
            if (Time_now.Minute < 10)
            {
                time += $"0{Time_now.Minute}";
            }
            else
            {
                time += $"{Time_now.Minute}";
            }
            lblTime.Content = time;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {            
                this.FittingSoftware_list[i].Upgrade_FS = new Upgrade_FittingSoftware(cmbRelease.Text,cmbBranch.Text,cmbOption.Text ,Time_now);
                ((MainWindow)System.Windows.Application.Current.MainWindow).FittingSoftware_List[i] = this.FittingSoftware_list[i]; // przekazanie obiektów do odpowiednikow glownych
            }
            // wlaczyc timer w mainwindow dla sprawdzania czy godzina już jest ok
            ((MainWindow)System.Windows.Application.Current.MainWindow).checkTime_Timer.Start();
            this.Close();
        }
    }

}
