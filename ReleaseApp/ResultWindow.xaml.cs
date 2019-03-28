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
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow(double teams, int size, List<string> listOfPeople)
        {
            InitializeComponent();
            this.Topmost = true;

            var rnd = new Random();
            var randomNumbers = Enumerable.Range(0, listOfPeople.Count()).OrderBy(x => rnd.Next()).Take(listOfPeople.Count()).ToList();
            int lastTeam = 1;

            for (int i = 1; i <= teams; i++)
            {
                txtblockResults.Text += "Team " + i + "\n";

                for (int j = 0; j < size; j++)
                {
                    txtblockResults.Text += "\t" + listOfPeople[randomNumbers[0]] + "\n";
                    randomNumbers.RemoveAt(0);
                }

                txtblockResults.Text += "\n";
                lastTeam += 1;
            }

            if (randomNumbers.Count > 0)
            {
                txtblockResults.Text += "Team " + lastTeam + " (smaller) \n";

                int iterations = randomNumbers.Count();

                for (int i = 0; i < iterations; i++)
                {
                    txtblockResults.Text += "\t" + listOfPeople[randomNumbers[0]] + "\n";
                    randomNumbers.RemoveAt(0);
                }

                txtblockResults.Text += "\n";
            }
        }

    }
}

