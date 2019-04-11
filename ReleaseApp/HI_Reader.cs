﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{

    class HI_Reader
    {
        public string Path_Gearbox;
        WebClient client;
        StreamReader reader;
        Stream data;
        string urlString = @"http://localhost:1111///general.createAuroraSession?name=default&modelFile=C:\Users\paze\Documents\GitHub\UltimateChanger3\ReleaseApp\ReadHI\ModelCompilerOutput\CompactModel_xuda.corona";

        public HI_Reader()
        {
            client = new WebClient();           
            //reader = new StreamReader(data);
            Path_Gearbox = FindGearbox();
        }

        public void startServer()
        {
            string gearbox = FindGearbox();
            string arguments = "server -s http://localhost:1111";
            if (gearbox != "")
            {
                Process.Start(gearbox + @"\gearboxj", arguments);
            }           
        }

        public void CreateSession()
        {
            data = client.OpenRead(urlString);
            reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            //Console.WriteLine(s);
            data.Close();
            reader.Close();
        }

        public void Connect(string device, string side)
        {
            urlString = @" http://localhost:1111//manager/sessions/default/general.createConnection?name=myConnection&medium="+$"{device}"+"&side="+$"{side}"+"&protocol=PIF2FW&properties=%22%22&connected=true";

            data = client.OpenRead(urlString);
            reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            Console.WriteLine(s);
            data.Close();
            reader.Close();
        }

        public List<string> ReadHI()
        {
            urlString = @"http://localhost:1111//manager/sessions/default/connections/myConnection/hiid.getEimData";

            data = client.OpenRead(urlString);
            reader = new StreamReader(data);
            List<string> s = findBrandModel(reader.ReadToEnd());
            Console.WriteLine(s[0]);
            data.Close();
            reader.Close();
            return s;
        }
        public void shutDown()
        {
            urlString = @"http://localhost:1111///general.shutdownServer";
            data = client.OpenRead(urlString);
            reader = new StreamReader(data);
            string s = reader.ReadToEnd();
           
            Console.WriteLine(s);
            data.Close();
            reader.Close();
        }

        public List<string> findBrandModel(string text)
        {
           int tmp = text.IndexOf('=');
            List<int> indexes = new List<int>();
            List<string> dataHI = new List<string>(); // lista stringow z brand, model, version
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '=')
                {
                    indexes.Add(i); // lista pozycji zawierajaca '='
                }
            }

            for (int i = 0; i < indexes.Count; i++)
            {
                string stringg = "";
                for (int j = indexes[i] + 1; j <= indexes[i] + 4; j++)
                {
                    try
                    {
                        stringg += text[j];
                    }
                    catch (Exception)
                    {

                    }
                    
                }
                dataHI.Add(stringg);
            }

            return TranslateModel(dataHI);

        }
        public List<string> TranslateModel(List<string> model) //[0] - brand [1] - model
        {
            List<string> brand = new List<string>();

            SortedDictionary<string, string> slownik = new SortedDictionary<string, string>();
            SortedDictionary<string, string> slownik_PP = new SortedDictionary<string, string>();

            slownik.Add("2700", "Oticon");
            slownik.Add("3580", "Bernafon");
            slownik.Add("3581", "Sonic");
            slownik.Add("5278", "Medical");
            slownik.Add("4964", "Philips");

            slownik_PP.Add("5082", "Xceed 1");


            try
            {
                brand.Add(slownik[model[0]]); // brand
            }
            catch (Exception)
            {
                brand.Add("");
            }
            try
            {
                brand.Add(slownik_PP[model[1]]); // model
            }
            catch (Exception)
            {
                brand.Add("");
            }

            return brand;
        }
        public string FindGearbox()
        {
            List<string> tmppaths = new List<string>();
            try
            {
                DirectoryInfo tmp = new DirectoryInfo(@"C:\toolsuites\gearbox\gearboxj");
                var gearboxes = tmp.GetDirectories(); // nazwy wszystkich dostepnych gearboxow
                foreach (var item in gearboxes)
                {
                    string cut_dir = item.Name.Remove(0, "gearboxj-rel_".Length);
                    tmppaths.Add(cut_dir.Remove(5, cut_dir.Length - 5));
                }

                // porownanie ktory ma najwyzszy major nr
                int index_Hmajor = 0;
                for (int i = 0; i < tmppaths.Count; i++)
                {
                    if (Convert.ToInt16(tmppaths[i][0]) > Convert.ToInt16(tmppaths[index_Hmajor][0]))
                    {
                        index_Hmajor = i;
                    }
                }

                return gearboxes[index_Hmajor].FullName;

            }
            catch (Exception)
            {
                return "";
            }
        }

    }
}
