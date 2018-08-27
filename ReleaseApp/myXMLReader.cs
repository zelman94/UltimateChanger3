using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UltimateChanger
{
    class myXMLReader
    {

        public SortedDictionary<string, string> getDefaultSettings()
        {
            //pobieram z XML ustawienia default
            //dodaje do listy / macierz ustawienie i wartość
            //zwracam
            //przez slownik wartosc xml => kontrolka nadaje odpowiednie ustawienia
            SortedDictionary<string, string> StringToUI = new SortedDictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Settings\\Defaults.xml");
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                StringToUI.Add(node.Name, node.InnerText);              

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
