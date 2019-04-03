using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{
    abstract class FSFactory
    {
        public static FittingSoftware CreateFS(string name, bool isComposition = false)
        {
            if (!TryBrandFS(name, out var brand, out var index))
                return null;

            if (isComposition)
            {
                return new FittingSoftwareComposition(name, brand, index);
            }
            else
            {
                return new FittingSoftware(name, brand, index);
            }
        }

        private static bool TryBrandFS(string name, out string brand, out int index)
        {
            switch (name)
            {
                case ("Genie 2"):
                    brand = "Oticon";
                    index = 0;
                    return true;

                case ("Medical"):
                    brand = "Medical";
                    index = 1;
                    return true;

                case ("Express"):
                    brand = "Sonic";
                    index = 2;
                    return true;

                case ("HearSuite"):
                    brand = "Philips";
                    index = 3;
                    return true;

                case ("Oasis"):
                    brand = "Bernafon";
                    index = 4;
                    return true;

                default:
                    brand = "";
                    index = -1;
                    return false;
            }
        }
    }
}
