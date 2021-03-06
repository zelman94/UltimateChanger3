﻿using System;
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
    /// Logika interakcji dla klasy AddRelease.xaml
    /// </summary>
    public partial class AddRelease : Window
    {
        List<Label> lableListForUi = new List<Label>();
        List<TextBox> listTextBoxForUi = new List<TextBox>();
        List<Button> buttonListForUi = new List<Button>();
        List<ComboBox> comboBoxListForUi = new List<ComboBox>();
        public AddRelease()
        {
            InitializeComponent();
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

            //USTAWIENIA COMBOBOXÓW
            foreach (var item in comboBoxListForUi)
            {
                item.Foreground = ((MainWindow)System.Windows.Application.Current.MainWindow).cmbBrandstoinstall.Foreground;
                item.BorderBrush = ((MainWindow)System.Windows.Application.Current.MainWindow).cmbBrandstoinstall.BorderBrush;
            }

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

        private void txtRelease_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtRelease.Text.Length == 2)
            {
                txtRelease.Text += '.';
                txtRelease.CaretIndex = txtRelease.Text.Length;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            myXMLReader.AddRelease(txtRelease.Text);
            this.Close();
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
