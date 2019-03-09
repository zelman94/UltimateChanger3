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
    /// Interaction logic for EditFittingSoftware.xaml
    /// </summary>
    public partial class EditFittingSoftware : Window
    {
        public FittingSoftware FS;
        List<Label> lableListForUi = new List<Label>();
        List<TextBox> listTextBoxForUi = new List<TextBox>();
        List<Button> buttonListForUi = new List<Button>();
        public EditFittingSoftware( FittingSoftware FS)
        {
            this.FS = new FittingSoftware(FS);
            InitializeComponent();
            txtOldMarket.Text = FS.Market;
            txtOldLogLevel.Text = FS.LogMode;
            txtOldPath.Text = FS.pathToExe;
            txtOldPathManu.Text = FS.pathToManu;

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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (txtNewLogLevel.Text != "")
            {
                FS.LogMode = txtNewLogLevel.Text;
                FS.setLogMode(FS.LogMode,0,!FS.composition);
            }
            if (txtNewMarket.Text != "")
            {
                FS.Market = txtNewMarket.Text;
                FS.setMarket(FS.Market);
            }
            if (txtNewPath.Text != "")
            {
                FS.pathToExe = txtNewPath.Text;
            }
            if (txtNewPathManu.Text != "")
            {
                FS.pathToManu = txtNewPathManu.Text;
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).FittingSoftware_List[FS.indexFS] = FS;

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
    }
}
