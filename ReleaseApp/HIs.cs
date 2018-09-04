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
        public string PP;
        public string Name;
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
            PP = "";
        }
        public HIs(bool t_coil,bool led,bool twobuttons,bool wireless,bool custom,bool s, bool magneto, string release, string pp = "",string name="")
        {
            T_Coil = t_coil;
            Led = led;
            twoButtons = twobuttons;
            Wireless = wireless;
            Custom = custom;
            S = s;
            Magneto = magneto;
            Release = release;
            PP = pp;
            Name = name;
        }


        public static HIs randomHI( string release,string Style) // release 19.1 w funkcji jest zmienina na postac dla XML
        {
            Random rnd = new Random();

            List<HIs> listOfAvailableTypes = myXMLReader.GetTypesInStyle(release, Style); // lista instancji dostępnych HIs

     
            return listOfAvailableTypes[MyRandomizer.Instance.Next(0, listOfAvailableTypes.Count)];
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
