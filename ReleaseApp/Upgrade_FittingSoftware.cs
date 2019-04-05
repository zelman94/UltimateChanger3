using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{
    public struct InfoUpdate
    {
        public string Release,Branch,Option; // option - master/rc
        public DateTime Time_Update;
    } 

    public class Upgrade_FittingSoftware
    {
        public InfoUpdate info = new InfoUpdate();

        public Upgrade_FittingSoftware(string Release, string Branch, string Option, DateTime updateTime)
        {
            info.Release = Release;
            info.Branch = Branch;
            info.Option = Option;
            info.Time_Update = updateTime;
        }


    }
}
