using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UltimateChanger
{
    public class FittingSoftwareComposition : FittingSoftware
    {
        public FittingSoftwareComposition(FittingSoftware tmpFS) : base(tmpFS)
        {
        }

        public FittingSoftwareComposition(string name, string brand, int index)
        {
            Name_FS = name;
            Brand = brand;
            indexFS = index;
            Path_Local_Installer = findUnInstaller();
            Version = getFS_Version();
            // Market = getMarket();
            customPath = false;
            this.composition = true;
            CreateComposition();
        }

        private void CreateComposition()
        {
            var localCompo = fileOperator.GetAllLocalCompositions();
            PathTrash = new List<string>() { localCompo[indexFS] };
            fileOperator.getPathToLogMode_Compo();
            PathToLogMode = fileOperator.getPathToConfigure(indexFS);
            pathToLogs = ""; // do sprawdzenia gdzie sie zapisuja
            pathToManu = fileOperator.FindSettingFileForComposition(indexFS);
            indexFS += 5;

            ListpathsToManInfo = fileOperator.getPathToManufacturerInfo_Compo_List();
            try
            {
                pathToExe = fileOperator.GetExeCompo(indexFS - 5)[0];
            }
            catch (Exception)
            {
                pathToExe = "";
            }

            pathToManu = fileOperator.FindSettingFileForComposition(indexFS - 5);
            Market = getMarket();
            OEM = "";
            SelectedLanguage = "";
            Version = getFS_Version();
            LogMode = getLogMode();
        }

        public override string getFS_Version()
        {
            try
            {
                FileVersionInfo tmp = FileVersionInfo.GetVersionInfo(pathToExe);

                return tmp.FileVersion;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public override string getMarket()
        {
            return fileOperator.getMarket(indexFS - 5, !composition);
        }
    }
}