using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{
    class MyHardware
    {
        public string Name, Manufacturer, Type, ID, Localization;
        public MyHardware()
        {
            Name = "";
            Manufacturer = "";
            Type = "";
            ID = "";
            Localization = "";
        }
        public MyHardware(string name, string manufacturer, string type, string iD, string localization)
        {
            Name = name;
            Manufacturer = manufacturer;
            Type = type;
            ID = iD;
            Localization = localization;
        }
        public static List<string> ToNameAndID(List<MyHardware> lista)
        {
            List<string> listaStringow = new List<string>();

            foreach (var item in lista)
            {
                listaStringow.Add(item.Name + " " + item.ID);
            }

            return listaStringow;
        }

        public static string convertToString(MyHardware item)
        {
            string value = "";

            value = "Name: " + item.Name + "\n" + "id: " + item.ID + "\n" + "Type: " + item.Type + "\n" + "Manufacturer: " + item.Manufacturer + "\n" + "Localization: " + item.Localization + "\n";

            return value;
        }

        public static MyHardware findHardwareByID(int id_item)
        {
            List<MyHardware> lista = myXMLReader.getHardware();
            return lista[id_item];
        }
    }
}
