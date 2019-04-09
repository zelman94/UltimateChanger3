using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{
    public struct InfoUpdate
    {
        public string Release,Branch,Option; // option - full/medium
        public DateTime Time_Update;
        public string path_to_root;
        public bool TrashCleaner;
    } 

    public class Upgrade_FittingSoftware
    {
        public InfoUpdate info = new InfoUpdate();

        public Upgrade_FittingSoftware(string Release, string Branch, string Option, DateTime updateTime, bool TrashCleaner)
        {
            info.Release = Release;
            info.Branch = Branch;
            info.Option = Option;
            info.Time_Update = updateTime;
            info.TrashCleaner = TrashCleaner;
        }
        public Upgrade_FittingSoftware(string Option,string path_root, DateTime updateTime, bool TrashCleaner)
        {
            info.Release = "";
            info.Branch = "";
            info.Option = Option;
            info.Time_Update = updateTime;
            info.path_to_root = path_root;
            info.TrashCleaner = TrashCleaner;
        }


    }
}
