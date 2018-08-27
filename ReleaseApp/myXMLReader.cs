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
        public void setSetting(string tmp)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\Defaults.xml");
            XmlNode root = doc.DocumentElement;
            XmlNode myNode = root.SelectSingleNode($"{tmp}::Value");
            myNode.Value = "blabla";
            doc.Save("D:\\build.xml");
        }
        


    }
}
