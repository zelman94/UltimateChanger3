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
        private Dictionary<string, string> BrandtoSoft;
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

        public void DeleteLogs(string brand_name)
        {
            string DirectoryName = $"C:/ProgramData/{brand_name}/{BrandtoSoft[brand_name]}/Logfiles/";
            System.IO.DirectoryInfo di = new DirectoryInfo(DirectoryName);
            try
            {
                string tempName;
                if (Directory.Exists(DirectoryName))
                    foreach (FileInfo file in di.GetFiles())
                    {
                        tempName = $"{di.ToString()}/{file.Name.ToString()}";
                        if (File.Exists(tempName))
                        {
                            File.SetAttributes(tempName, FileAttributes.Normal);
                            File.Delete($"{di.ToString()}/{file.Name.ToString()}");
                        }
                    }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Cos sie zepsulo");
            }
            catch (DirectoryNotFoundException ee)
            {
                MessageBox.Show("Cos sie mocno zepsulo");
            }
        }
    }
}
