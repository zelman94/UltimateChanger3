using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{
    public class HIs
    {
        public bool T_Coil;
        public bool Led;
        public bool twoButtons;
        public bool Wireless;
        public bool Custom;
        public bool S;
        public bool Magneto;
        public string Release;

        public HIs()
        {
            T_Coil = false;
            Led = false;
            twoButtons = false;
            Wireless = false;
            Custom = false;
            S = false;
            Magneto = false;
            Release = "";
        }
    }
}
