using System;
using System.Collections.Generic;
using System.Configuration;
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

        public static List<string> GetTypesInStyleString(string release, string Style)
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

        public static List<HIs> GetTypesInStyle(string release, List<string> Styles)
        {
            if (release.Contains("."))
            {
                release = release.Replace('.', '_');
            }
            List<HIs> Styles_ = new List<HIs>();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("Settings\\HIs.xml");

                foreach (var Style in Styles) // iteracja po 
                {

                    XmlNodeList NodesNames = doc.DocumentElement.SelectNodes(string.Format($"/Random_HI/Release{release}/Available_Style/{Style}"));
                    XmlNodeList NodesNames2 = NodesNames[0].ChildNodes; // pobieram OPN itp później można odczytać z tego parametry
                    foreach (XmlNode item in NodesNames2)
                    {
                        bool T_Coil;
                        bool Led;
                        bool twoButtons;
                        bool Wireless;
                        bool Custom;
                        bool S;
                        bool Magneto;
                        string Release;
                        string PP;
                        string family;

                        XmlNodeList SettingsHI = doc.DocumentElement.SelectNodes(string.Format($"/Random_HI/Release{release}/Available_Style/{Style}/{item.Name}"));

                        XmlNodeList SettingsHI_Childs = SettingsHI[0].ChildNodes;

                        T_Coil = Convert.ToBoolean(SettingsHI_Childs[3].InnerText.ToString());
                        Led = Convert.ToBoolean(SettingsHI_Childs[0].InnerText.ToString());
                        twoButtons = Convert.ToBoolean(SettingsHI_Childs[1].InnerText.ToString());
                        Wireless = Convert.ToBoolean(SettingsHI_Childs[2].InnerText.ToString());
                        Custom = /*Convert.ToBoolean(SettingsHI_Childs[4].InnerText.ToString());*/false;
                        S = /*Convert.ToBoolean(SettingsHI_Childs[5].InnerText.ToString());*/ false;
                        Magneto = Convert.ToBoolean(SettingsHI_Childs[4].InnerText.ToString());
                        Release = release.Replace('_','.');
                        PP = /*SettingsHI_Childs[4].InnerText.ToString();*/ "";
                        family = SettingsHI[0].ParentNode.Name.ToString();

                        Styles_.Add(new HIs(T_Coil,Led,twoButtons,Wireless,Custom,S,Magneto,Release,PP, item.Name, family));
                    }
                }
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }



            return Styles_;
        }

        public static string GetComDEV(string wireless, double weight ) // wireless true com dev z wirelessem //weight - 0,5 równe szanse 0,6 -> 10% więcej instancji dla wireless itd
        {
            
            List<string> ComDEV = new List<string>();
            List<string> ComDEV_Wireless = new List<string>();
            List<string> ComDEV_Wire = new List<string>();
            //string weig = Convert.ToString(weight);

            double diff = weight - 0.5;
            double weight_forComDev;
            if (diff == 0) // rowne szanse nic nie robimy
            {
                weight_forComDev = 1;
            }
            else
            {
                if (diff == 0.5) // gdy waga == 1 
                {
                    weight_forComDev = diff * 60.0;
                }
                else if (diff > 0) // wieksza waga na wireless // warunek raczej zbedny 
                {
                    weight_forComDev = diff * 20.0;
                }
                else // mniejsza waga na wireless
                {
                    weight_forComDev = diff * 20.0;
                }
            }



            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\HIs.xml");
            try
            {
                XmlNodeList NodesNames = doc.DocumentElement.SelectNodes(string.Format($"/Random_HI/Hardware/Available_Style"));

                XmlNodeList ComDevs = NodesNames[0].ChildNodes;
                if (Convert.ToBoolean(wireless)) // jezeli HI ma wireless to wszystkie ComDev mogą być
                {
                    foreach (XmlNode item in ComDevs)
                    {
                        if (item.FirstChild.InnerText == "TRUE") // jezeli jest z wirelessem to dodaje do listy com dev wireless
                        {
                            ComDEV_Wireless.Add(item.Name); // lista com dev z wirelessem
                        }
                        else
                        {
                            ComDEV_Wire.Add(item.Name); // lista com dev kablowych 
                        }                           
                    }

                    if (weight_forComDev > 0) // przewaga dla wireless
                    {
                        //weight_forComDev - to ile % ma być powielonych rekordow 
                        // 
                        int duplication =Convert.ToInt16( Math.Ceiling(ComDEV_Wireless.Count * weight_forComDev) / 10); // ile rekordów więcej

                        for (int i = 0; i < duplication; i++)
                        {
                            string tmp = ComDEV_Wireless[MyRandomizer.Instance.Next(0, ComDEV_Wireless.Count)]; // jaki string bedzie dodany ponownie
                            ComDEV_Wireless.Add(tmp);
                        }                  

                    }
                    else if (weight_forComDev < 0) // przewaga dla kabli
                    {
                        int duplication = Convert.ToInt16(Math.Ceiling(ComDEV_Wire.Count * (weight_forComDev * (-1) )) / 10); // ile rekordów więcej  // *-1 bo wartosc ujemna 

                        for (int i = 0; i < duplication; i++)
                        {
                            string tmp = ComDEV_Wire[MyRandomizer.Instance.Next(0, ComDEV_Wire.Count)]; // jaki string bedzie dodany ponownie
                            ComDEV_Wire.Add(tmp);
                        }
                    }

                    ComDEV = ComDEV_Wireless.Concat(ComDEV_Wire).ToList(); // zadziala ? // lista wszystkich 
                }
                else
                {
                    foreach (XmlNode item in ComDevs)
                    {
                        if (item.FirstChild.InnerText == wireless) // tylko wireless
                        {
                            ComDEV.Add(item.Name);
                            ComDEV_Wireless.Add(item.FirstChild.InnerText);
                        }

                    }
                }
                

            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }


            return ComDEV[MyRandomizer.Instance.Next(0, ComDEV.Count)]; // losowy COM DEV
        }

        public static List<string> getFiczurs()
        {
            List<string> listFicz = new List<string>();

            int counter = 0;
            string line;

            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader("Settings\\Ficzurs.txt");
            while ((line = file.ReadLine()) != null)
            {
                listFicz.Add(line);
                counter++;
            }

            file.Close();

            return listFicz;
        }

        public static List<string> getPaths(string listName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\Paths.xml");
            List<string> listpaths = new List<string>();
            try
            {
                XmlNodeList NodesNames = doc.DocumentElement.SelectNodes(string.Format($"/Paths/{listName}"));
                XmlNodeList NodesNames2 = NodesNames[0].ChildNodes; // pobieram OPN itp później można odczytać z tego parametry
                foreach (XmlNode item in NodesNames2)
                {
                    listpaths.Add(item.InnerText);
                }
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }
            return listpaths;
        }

        public static void setGodzilla()
        {
            List<string> listOfConfigFSExe = new List<string>()
            {
                {@"C:\Program Files(x86)\Oticon\Genie\Genie2\Genie.exe.config"},
                {@"C:\Program Files(x86)\Bernafon\Oasis\Oasis2\Oasis.exe.config"},
                {@"C:\Program Files(x86)\Sonic\ExpressFit\ExpressFit2\EXPRESSfit.exe.config"},
                {@"C:\Program Files(x86)\Philips HearSuite\HearSuite\HearSuite2\HearSuite.exe.config"},
                {@""} // medical
            };
           

            foreach (var item in listOfConfigFSExe)
            {
                if (System.IO.File.Exists(item)) // jezeli plik istnieje
                {


                    var doc1 = XDocument.Load(item);

                    var list1 = from appNode in doc1.Descendants("appSettings").Elements()
                                where appNode.Attribute("key").Value == "Wdh.DataDriven.Analytics.Windows.Metadata.EndpointAuthorizationToken"
                                select appNode;
                    var list2 = from appNode in doc1.Descendants("appSettings").Elements()
                                where appNode.Attribute("key").Value == "Wdh.DataDriven.Analytics.Windows.Metadata.Environment"
                                select appNode;
                    var list3 = from appNode in doc1.Descendants("appSettings").Elements()
                                where appNode.Attribute("key").Value == "Wdh.DataDriven.Analytics.Windows.Metadata.Endpoint"
                                select appNode;
                    var list4 = from appNode in doc1.Descendants("appSettings").Elements()
                                where appNode.Attribute("key").Value == "Wdh.DataDriven.Analytics.Windows.InstrumentationKey"
                                select appNode;
                    var list5 = from appNode in doc1.Descendants("appSettings").Elements()
                                where appNode.Attribute("key").Value == "Wdh.DataDriven.Analytics.Windows.ApiToken"
                                select appNode;
                    var element1 = list1.FirstOrDefault();
                    var element2 = list2.FirstOrDefault();
                    var element3 = list3.FirstOrDefault();
                    var element4 = list4.FirstOrDefault();
                    var element5 = list5.FirstOrDefault();
                    element1.Attribute("value").SetValue("QiIPdzVOjzCTfhKllRx8hv03svaahNmbmhGOpdsWOaoORSZNIJtofg==");
                    element2.Attribute("value").SetValue("test");
                    element3.Attribute("value").SetValue("https://oas-test-euw-functionapp.azurewebsites.net/api/v1/applicationdatalogging/1.0");
                    element4.Attribute("value").SetValue("fb1e3d8e-8462-4a29-8df8-5ddb0f3745de");
                    element5.Attribute("value").SetValue("b31b99bf-5257-41d3-86df-ad84b30aea8e");

                    var tmp = from appNode in doc1.Descendants("appSettings").Elements()
                              where appNode.Attribute("key").Value == "Wdh.DataDriven.Analytics.Windows.TestIdentifier"
                              select appNode;

                    if (tmp.Count() == 0)
                    {
                        XElement parent = element5.Parent;
                        var element6 = new XElement(element5);
                        element6.Attribute("key").SetValue("Wdh.DataDriven.Analytics.Windows.TestIdentifier");
                        element6.Attribute("value").SetValue("tests");

                        parent.Add(new XElement(element6));
                    }


                    doc1.Save(item);
                }
            }
        }


    }
}
