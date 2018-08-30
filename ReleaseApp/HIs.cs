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
        public HIs(bool t_coil,bool led,bool twobuttons,bool wireless,bool custom,bool s, bool magneto, string release)
        {
            T_Coil = t_coil;
            Led = led;
            twoButtons = twobuttons;
            Wireless = wireless;
            Custom = custom;
            S = s;
            Magneto = magneto;
            Release = release;
        }


        public static string randomHI(List<string> listHIs)
        {
            Random rnd = new Random();
            string rand = "";
            rand = listHIs[rnd.Next(listHIs.Count)];
            return rand;
        }

        public static string randomCOMDEV(List<string> ListComDev)
        {
            Random rnd = new Random();
            string rand = "";
            rand = ListComDev[rnd.Next(ListComDev.Count)];
            return rand;
        }


    }
}
