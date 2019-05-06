using log4net;
using System;
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
        private static readonly ILog Log =
      LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string Path_Gearbox;
        WebClient client;
        StreamReader reader;
        Stream data;
        string urlString = @"http://localhost:1111///general.createAuroraSession?name=default&modelFile=C:\Program Files\UltimateChanger\ReadHI\ModelCompilerOutput\CompactModel_xuda.corona";
        SortedDictionary<string, string> slownik = new SortedDictionary<string, string>();

        public HI_Reader()
        {
            client = new WebClient();           
            //reader = new StreamReader(data);
            Path_Gearbox = FindGearbox();

            slownik.Add("2700", "Oticon");
            slownik.Add("4786", "Amplifon");
            slownik.Add("2701", "AudioNova");
            slownik.Add("2702", "KIND");
            slownik.Add("3931", "GPL");
            slownik.Add("3703", "HHM");
            slownik.Add("3580", "Bernafon");
            slownik.Add("3581", "Sonic");
            slownik.Add("5278", "Medical");
            slownik.Add("4964", "Philips");
            Log.Info("HI_Reader Created");
        }

        public void startServer()
        {
            Log.Info("startServer Started");
            string gearbox = FindGearbox();
            string arguments = "server -s http://localhost:1111";
            Log.Debug("arguments: " + arguments);
            if (gearbox != "")
            {
                try
                {
                    Process gear = new Process();
                    gear.StartInfo.CreateNoWindow = true;
                    gear.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    gear.StartInfo.Arguments = arguments;
                    gear.StartInfo.FileName = gearbox + @"\gearboxj";
                    gear.Start();
                    //Process.Start(gearbox + @"\gearboxj", arguments);
                }
                catch (Exception x)
                {
                    Log.Debug(x.ToString());
                    Log.Info("startServer Error");
                    return;
                }               
            }
            Log.Info("startServer Done");
        }

        public void CreateSession()
        {
            Log.Info("CreateSession Started");
            try
            {
                data = client.OpenRead(urlString);
                reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                //Console.WriteLine(s);
                data.Close();
                reader.Close();
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
                Log.Debug(urlString);
            }
            Log.Info("CreateSession Done");
        }

        public void Connect(string device, string side)
        {
            Log.Info($"Connect Started for: {device} and {side}");
            urlString = @" http://localhost:1111//manager/sessions/default/general.createConnection?name=myConnection" + side + "&medium=" + $"{device}"+"&side="+$"{side}"+"&protocol=PIF2FW&properties=%22%22&connected=true";
            Log.Debug($"Connect urlString: {urlString}");
            try
            {
                data = client.OpenRead(urlString);
                reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                data.Close();
                reader.Close();
            }
            catch (Exception x)
            {
                Log.Info("Connect Error");
                Log.Debug(x.ToString());
                return;
            }

            Log.Info("Connect Done");
        }

        public List<string> ReadHI(string side)
        {
            urlString = @"http://localhost:1111//manager/sessions/default/connections/myConnection" + side + "/hiid.getEimData";
            Log.Info($"ReadHI Started for: {side}");
            try
            {
                data = client.OpenRead(urlString);
                reader = new StreamReader(data);
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
                return null;
            }
            
            List<string> s = findBrandModel(reader.ReadToEnd(),true);
            Console.WriteLine(s[0]);
            data.Close();
            reader.Close();
            if (s.Count<3)
            {
                s.Add("error");
            }
            Log.Info($"ReadHI Done");
            return s;
        }

        public string getSerialNumber(string side)
        {
            Log.Info($"getSerialNumber Started for {side}");
            urlString = @"http://localhost:1111//manager/sessions/default/connections/myConnection" + side + "/hiid.getProductionSerialNumber";
            data = client.OpenRead(urlString);
            reader = new StreamReader(data);
            try
            {
                List<string> s = findBrandModel(reader.ReadToEnd(), false);
                Console.WriteLine(s[1]);
                data.Close();
                reader.Close();
                Log.Debug($"SerialNumber for {side} side: {s[1]}");
                return s[1];
            }
            catch (Exception x)
            {
                Log.Debug($"getSerialNumber error: \n{x.ToString()}");
                return "error";
            }               
        }

        public void shutDown()
        {
            Log.Info("shutDown Started");
            try
            {
                urlString = @"http://localhost:1111///general.shutdownServer";
                data = client.OpenRead(urlString);
                reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                Console.WriteLine(s);
                data.Close();
                reader.Close();
            }
            catch (Exception x)
            {
                Log.Debug($"shutDown Error: \n{x.ToString()}");
            }
            Log.Info("shutDown Done");
        }

        public List<string> findBrandModel(string text, bool Translate)
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
                int maxIndex;
                if (Translate)
                {
                    maxIndex = indexes[i] + 4;
                }
                else
                {
                    maxIndex = indexes[i] + 8;
                }
                for (int j = indexes[i] + 1; j <= maxIndex; j++)
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
            if (Translate)
            {
                return TranslateModel(dataHI);
            }
            return dataHI;
        }
        public List<string> TranslateModel(List<string> model) //[0] - brand [1] - model [2] - Firmware
        {

            List<string> brand = new List<string>();            
            try
            {
                brand.Add(slownik[model[0]]); // brand
                Log.Info("Brand: " + model[0] + " Translated Brand: " + brand[0]);
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
                brand.Add("");
            }
                try
                {
                    brand.Add(((MainWindow)System.Windows.Application.Current.MainWindow).dataBaseManager.getModelHI(model[1]));
                    Log.Info("Model: " + model[1] + " Translated Model: " + brand[1]);               
                }
                catch (Exception x)
                {
                Log.Debug(x.ToString());
                brand.Add("error");
                }
            try
            {
                brand.Add("FW: " + model[2][0].ToString());
                Log.Info("FW: " + model[2][0].ToString());
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
                brand.Add("error");
            }

            return brand;
        }
        public string FindGearbox()
        {
            Log.Info("FindGearbox Started");
            List<string> tmppaths = new List<string>();
            try
            {
                DirectoryInfo tmp = new DirectoryInfo(@"C:\toolsuites\gearbox\gearboxj");
                var gearboxes = tmp.GetDirectories(); // nazwy wszystkich dostepnych gearboxow
                foreach (var item in gearboxes)
                {
                    Log.Debug($"Found Gearbox: {item.FullName}");
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
                Log.Debug($"used gearbox: {gearboxes[index_Hmajor].FullName}");
                return gearboxes[index_Hmajor].FullName;
            }
            catch (Exception x)
            {
                Log.Debug(x.ToString());
                return "";
            }
        }
    }
}
