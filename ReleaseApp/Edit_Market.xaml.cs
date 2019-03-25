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
    /// Interaction logic for Edit_Market.xaml
    /// </summary>
    public partial class Edit_Market : Window
    {
        List<Label> lableListForUi = new List<Label>();
        List<TextBox> listTextBoxForUi = new List<TextBox>();
        List<Button> buttonListForUi = new List<Button>();
        FittingSoftware FS;
        public Edit_Market(FittingSoftware FS)
        {
            this.FS = FS;
            InitializeComponent();
            cmbMarket.ItemsSource = ((MainWindow)System.Windows.Application.Current.MainWindow).cmbMarket.ItemsSource;
            cmbMarket.DisplayMemberPath = "Key";
            cmbMarket.SelectedValuePath = "Value";
            cmbMarket.SelectedValue = FS.Market;
            txtNameFS.Text = FS.Brand;

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

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (FS.Market == cmbMarket.Text || FS.Version == "") // jezeli nic nie zmieniłem to wychodze
            {
                this.Close();
            }
            FS.setMarket(BindCombobox.marketIndex[cmbMarket.SelectedIndex]);
            ((MainWindow)System.Windows.Application.Current.MainWindow).FittingSoftware_List[FS.indexFS].Market = BindCombobox.marketIndex[cmbMarket.SelectedIndex];
            ((MainWindow)System.Windows.Application.Current.MainWindow).refreshUI(new object(), new EventArgs());
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
