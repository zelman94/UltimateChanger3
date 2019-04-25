using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace UltimateChanger
{

    class Install_
    {
        public void Start_InstallationAsync(string path)
        {
            Task.Run(() => Process.Start(path.ToString())); // watek asynchroniczny
        }

        public void Start_Silent_InstallationAsync(string path)
        {
            Task.Run(() => Process.Start(path.ToString(), " /uninstall /quiet")); // watek asynchroniczny
        }


    }
    public class FSInstaller
    {


        public bool InstallBrand(string path, bool mode_normal)
        {
            if (File.Exists(path) && mode_normal)
            {
                try
                { // dodać wątek 
                    Install_ THInstall = new Install_();
                    THInstall.Start_InstallationAsync(path);


                    //Process.Start(path);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            if (File.Exists(path) && !mode_normal) //silent installation
            {
                try
                {
                    // dodać wątek 
                    //string tmp2 = @"\\10.128.3.1\DFS_Data_SSC_FS_GenieBuilds\Phoenix\ExpressFit\ExpressFit_4.0.784.161\Full\Sonic\Setup.exe /quiet";
                    Install_ THInstall = new Install_();
                    THInstall.Start_Silent_InstallationAsync(path);


                    // Process.Start(path, " /uninstall /quiet");
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool UninstallBrand(FittingSoftware FS, bool mode)
        {
            List<string> paths = new List<string>();
           


                if (!File.Exists(FS.Path_Local_Installer) && FS.Path_Local_Installer != "")
                {                    
                    return false; // jakiś plik nie istnieje
                }
                else
                {
                    if (FS.Path_Local_Installer != "")
                    {
                        paths.Add(FS.Path_Local_Installer); // dodaje do listy path do istniejacych instllerow bez ""
                    }
                   
                }


            if (mode)
            {
                try
                {
                    Process.Start(paths[0], " /uninstall"); // normal mode tylko pierwszy element mozliwy
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            if (!mode) //silent installation
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).listGlobalPathsToUninstall = new List<FittingSoftware>() { FS }; // przekazanie listy do glownego okna dalsze operacje tam
                //try
                //{
                //    Process.Start(path, " /uninstall /quiet");
                //    return true;
                //}
                //catch (Exception)
                //{
                //    return false;
                //}
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
