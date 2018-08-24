using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace UltimateChanger
{
    public class FSInstaller
    {
        public bool InstallBrand(string path, bool mode_normal)
        {
            if (File.Exists(path) && mode_normal)
            {
                try
                {
                    Process.Start(path);
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
                    //string tmp2 = @"\\10.128.3.1\DFS_Data_SSC_FS_GenieBuilds\Phoenix\ExpressFit\ExpressFit_4.0.784.161\Full\Sonic\Setup.exe /quiet";
                    Process.Start(path, " /uninstall /quiet");
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

        public bool UninstallBrand(string path, bool mode_normal)
        {
            if (File.Exists(path) && mode_normal)
            {
                try
                {
                    Process.Start(path);
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
                    //string tmp2 = @"\\10.128.3.1\DFS_Data_SSC_FS_GenieBuilds\Phoenix\ExpressFit\ExpressFit_4.0.784.161\Full\Sonic\Setup.exe /quiet";
                    Process.Start(path, " /uninstall /quiet");
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
    }
}
