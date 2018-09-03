using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace UltimateChanger
{
    class myXMLReader
    {

        public static List<string> getReleases()
        {
            List<string> lista = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\Releases.xml");


            XmlNodeList NodesValues = doc.SelectNodes(string.Format($"/Release/Value"));
            for (int i = 0; i < NodesValues.Count; i++)
            {
                lista.Add(NodesValues[i].InnerText);
            }

            return lista;
        }

        public SortedDictionary<string, string> getDefaultSettings(string type) // type to nazwa noda do ustawien czyli RadioButtons albo CheckBoxes
        {
            //pobieram z XML ustawienia default
            //dodaje do listy / macierz ustawienie i wartość
            //zwracam
            //przez slownik wartosc xml => kontrolka nadaje odpowiednie ustawienia

            SortedDictionary<string, string> StringToUI = new SortedDictionary<string, string>();
            XmlDocument doc2 = new XmlDocument();
            doc2.Load("Settings\\Defaults.xml");
            XmlNodeList NodesNames = doc2.SelectNodes(string.Format($"/Settings/{type}/Name"));
            XmlNodeList NodesValues = doc2.SelectNodes(string.Format($"/Settings/{type}/Value"));
            for (int i = 0; i < NodesNames.Count; i++)
            {
                StringToUI.Add(NodesNames[i].InnerText, NodesValues[i].InnerText);
            }          

            return StringToUI;
        }
        public void setSetting(string node_toEdit,string type,string value)// type to nazwa noda do ustawien czyli RadioButtons albo CheckBoxes albo ComboBox
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\Defaults.xml");
            XmlNode root = doc.DocumentElement;
            foreach (XmlNode item in root.SelectNodes($"/Settings/{type}/Name"))
            {
                if (item.InnerText == node_toEdit)
                {
                    item.NextSibling.InnerText = value;
                }
            }
            doc.Save("Settings\\Defaults.xml");
        }      

        public static List<MyHardware> getHardware()
        {
            List<MyHardware> lista = new List<MyHardware>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\Hardware.xml");

            XmlNodeList NodesNames = doc.SelectNodes(string.Format($"/Hardware/Element/Name"));
            XmlNodeList NodesId = doc.SelectNodes(string.Format($"/Hardware/Element/Id"));
            XmlNodeList NodesManufacturer = doc.SelectNodes(string.Format($"/Hardware/Element/Manufacturer"));
            XmlNodeList NodesType = doc.SelectNodes(string.Format($"/Hardware/Element/Type"));
            XmlNodeList NodesLocalization = doc.SelectNodes(string.Format($"/Hardware/Element/Localization"));
            for (int i = 0; i < NodesNames.Count; i++)
            {
                try
                {
                    lista.Add(new MyHardware(NodesNames[i].InnerText, NodesManufacturer[i].InnerText, NodesType[i].InnerText, NodesId[i].InnerText,   NodesLocalization[i].InnerText));
                }

                catch (Exception x)
                {
                }
            } 
            return lista;
        }

        public static void SetNewHardware(string name,string manu, string type, string id, string local)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\Hardware.xml");
            XmlNode root = doc.DocumentElement;

            //Create a new node.
            XmlElement elem = doc.CreateElement("Element");         

            //Add the node to the document.
            root.AppendChild(elem);

            XmlElement elem2 = doc.CreateElement("Name");
            elem2.InnerText = name;
            elem.AppendChild(elem2);

            elem2 = doc.CreateElement("Id");
            elem2.InnerText = id;
            elem.AppendChild(elem2);

            elem2 = doc.CreateElement("Manufacturer");
            elem2.InnerText = manu;
            elem.AppendChild(elem2);

            elem2 = doc.CreateElement("Type");
            elem2.InnerText = type;
            elem.AppendChild(elem2);
            
            elem2 = doc.CreateElement("Localization");
            elem2.InnerText = local;
            elem.AppendChild(elem2);

            Console.WriteLine("Display the modified XML...");
            doc.Save(Console.Out);
            doc.Save("Settings\\Hardware.xml");
        }
        public static void DeleteItem(int index)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\Hardware.xml");
            XmlNodeList NodesId = doc.SelectNodes(string.Format($"/Hardware/Element/Id"));
            XmlNode parent = NodesId[index].ParentNode;
            parent.ParentNode.RemoveChild(parent);
            doc.Save("Settings\\Hardware.xml");
        }

        public static void SetEditItem(int index,string txtName, string txtManuf, string txtType, string txtId, string txtLocal)
        {
            List<MyHardware> lista = new List<MyHardware>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\Hardware.xml");

            XmlNodeList NodesNames = doc.SelectNodes(string.Format($"/Hardware/Element/Name"));
            XmlNodeList NodesId = doc.SelectNodes(string.Format($"/Hardware/Element/Id"));
            XmlNodeList NodesManufacturer = doc.SelectNodes(string.Format($"/Hardware/Element/Manufacturer"));
            XmlNodeList NodesType = doc.SelectNodes(string.Format($"/Hardware/Element/Type"));
            XmlNodeList NodesLocalization = doc.SelectNodes(string.Format($"/Hardware/Element/Localization"));

                try
                {
                NodesNames[index].InnerText = txtName;
                NodesId[index].InnerText = txtId;
                NodesManufacturer[index].InnerText = txtManuf;
                NodesType[index].InnerText = txtType;
                NodesLocalization[index].InnerText = txtLocal;
                }

                catch (Exception x)
                {
                }
            doc.Save("Settings\\Hardware.xml");
        }
        public static List<string> getTeamPerson()
        {
            List<string> listPersons = new List<string>();
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load("Settings\\myTeam.xml");
            }
            catch (Exception x)
            {
                 
            }   

            XmlNodeList NodesValues = doc.SelectNodes(string.Format($"/Person/Value"));
            for (int i = 0; i < NodesValues.Count; i++)
            {
                listPersons.Add(NodesValues[i].InnerText);
            }

            return listPersons;
        }

        public static void deletePerdon(string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\myTeam.xml");
            XmlNodeList NodesNames = doc.SelectNodes(string.Format($"/Person/Value"));

            foreach (XmlNode item in NodesNames)
            {
                if (item.InnerText == name)
                {
                    item.ParentNode.RemoveChild(item);
                }
            }
            doc.Save("Settings\\myTeam.xml");
        }

        public static void addPerdon(string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\myTeam.xml");

            XmlNode root = doc.DocumentElement;
            //Create a new node.
            XmlElement elem = doc.CreateElement("Value");
            elem.InnerText = name;
            //Add the node to the document.
            root.AppendChild(elem);


            doc.Save("Settings\\myTeam.xml");
        }

        public static List<string> GetStylesInRelease(string release) // release "19_1" "18_2" =? "." = "_" // zwraca np OPN, OPN_S, Custom czyli childs of Available_Style
        {
            if (release.Contains("."))
            {
                release = release.Replace('.','_');
            }
            List<string> Styles = new List<string>();

            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\HIs.xml");
            try
            {
                XmlNodeList NodesNames = doc.DocumentElement.SelectNodes(string.Format($"/Random_HI/Release{release}/Available_Style"));
                XmlNodeList NodesNames2 = NodesNames[0].ChildNodes; // pobieram OPN itp później można odczytać z tego parametry
                foreach (XmlNode item in NodesNames2)
                {
                    Styles.Add(item.Name);
                }
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }


            return Styles;
        }

        public static List<string> GetTypesInStyle(string release, string Style)
        {
            if (release.Contains("."))
            {
                release = release.Replace('.', '_');
            }
            List<string> Styles = new List<string>();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("Settings\\HIs.xml");
                XmlNodeList NodesNames = doc.DocumentElement.SelectNodes(string.Format($"/Random_HI/Release{release}/Available_Style/{Style}"));
                XmlNodeList NodesNames2 = NodesNames[0].ChildNodes; // pobieram OPN itp później można odczytać z tego parametry
                foreach (XmlNode item in NodesNames2)
                {
                    Styles.Add(item.Name);
                }
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }



            return Styles;
        }



    }
}
