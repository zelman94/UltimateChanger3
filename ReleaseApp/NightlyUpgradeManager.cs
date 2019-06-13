using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{
    
    public class NightlyUpgradeManager
    {
        public bool waitForUpgrade = false;
        InfoUpdate Info_upgrade;
        List<bool> listCheckboxStatus;
        public NightlyUpgradeManager(string release, string branch, string option, DateTime time, string path_root, bool trash, List<bool> checkboxStatus)
        {
            Info_upgrade = new InfoUpdate();
            Info_upgrade.Release = release;
            Info_upgrade.Branch = branch;
            Info_upgrade.Option = option;
            Info_upgrade.Time_Update = time;
            Info_upgrade.TrashCleaner = trash;
        }
        public NightlyUpgradeManager(InfoUpdate info, List<bool> checkboxStatus)
        {
            Info_upgrade = info;
            listCheckboxStatus = checkboxStatus;
        }
       


    }
}
