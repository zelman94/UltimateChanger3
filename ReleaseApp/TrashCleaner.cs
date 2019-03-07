using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace UltimateChanger
{
    public class TrashCleaner
    {       
        public TrashCleaner(/*Dictionary<string, string> BrandtoSofte*/)
        {
        }
        public void DeleteTrash(string DirectoryName)
        {
            if (Directory.Exists(DirectoryName) && !DirectoryName.Contains("SoundStudio"))
            {
                try
                {
                    Directory.Delete(DirectoryName, true);
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.ToString());
                }
            }
        }

        public void DeleteCompo(int nrFS) // dziala
        {
            FileOperator fileoperator = new FileOperator();
           List <string> listAllCompoLocal = fileoperator.GetAllLocalCompositions();

            if (fileoperator.CheckIfCompositionIsAvailable(listAllCompoLocal, nrFS)) // sprawdzam czy wybrany fs Compo istnieje jezeli tak to usuwam kompozycje
            {
                Directory.Delete(listAllCompoLocal[nrFS],true); // usuwam wszystko 
                try
                {
                    File.Delete(listAllCompoLocal[nrFS]+".exe");
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
