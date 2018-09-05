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
        public string Name_fammily;
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
            Name_fammily = "";
        }
        public HIs(bool t_coil,bool led,bool twobuttons,bool wireless,bool custom,bool s, bool magneto, string release, string pp = "",string name="", string namefamilly="")
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
            Name_fammily = namefamilly;
        }


        public static HIs randomHI( string release,List<string> Style, List<string> SelectedTypes) // release 19.1 w funkcji jest zmienina na postac dla XML
        {
            List<HIs> TypesToRand = new List<HIs>();

            List<HIs> listOfAvailableTypes = myXMLReader.GetTypesInStyle(release, Style); // lista instancji dostępnych HIs // zmienic tutaj na liste styli 
            foreach (var item in listOfAvailableTypes)
            {
                foreach (var item2 in SelectedTypes)
                {
                    if (item2 == item.Name)
                    {
                        TypesToRand.Add(item);
                    }
                }

            }
     
            return TypesToRand[MyRandomizer.Instance.Next(0, TypesToRand.Count)];
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
