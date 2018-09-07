using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
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

    class USBHardware
    {
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }

        public USBHardware(string deviceID,string pnpDeviceID)
        {
            this.DeviceID = deviceID;
            this.DeviceName = pnpDeviceID;
        }

       static public string getIDName(string DeviceName)
        {
            string name = "";

            foreach (string item in FileOperator.ListUSB_AvailableComDev_description)
            {
                if (item.Contains(DeviceName))
                {
                    name = FileOperator.ListUSB_AvailableComDev[FileOperator.ListUSB_AvailableComDev_description.IndexOf(DeviceName)];
                }
            }

            return name;
        }

        static public string getComDevID(string DeviceID)
        {
            string ID = "";

           

            return ID;
        }

        static public void ShowAllConnectedUSB()
        {


            try
            {
                var usbDevices = GetUSBDevices();

                foreach (var usbDevice in usbDevices)
                {
                    Console.WriteLine("Device ID: {0}, PNP Device ID: {1}, Description: {2}",
                        usbDevice.DeviceID, usbDevice.PnpDeviceID, usbDevice.Description);
                    getIDName(usbDevice.Description);
                    getComDevID(usbDevice.SerialNumber);
                }
            }
            catch (Exception)
            {
                
            }


        }

        static List<USBDeviceInfo> GetUSBDevices()
        {


            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    Console.WriteLine(string.Format("({0}) {1}", drive.Name.Replace("\\", ""), drive.VolumeLabel));
                }
            }

            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                Console.WriteLine("USBHub device Friendly name:{0}", device["Name"].ToString());
                devices.Add(new USBDeviceInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("PNPDeviceID"),
                (string)device.GetPropertyValue("Description"),
                ""
                ));
            }

            collection.Dispose();
            return devices;
        }

        class USBDeviceInfo
        {
            public USBDeviceInfo(string deviceID, string pnpDeviceID, string description,string serialNumber)
            {
                this.DeviceID = deviceID;
                this.PnpDeviceID = pnpDeviceID;
                this.Description = description;
                this.SerialNumber = serialNumber;
            }
            public string DeviceID { get; private set; }
            public string PnpDeviceID { get; private set; }
            public string Description { get; private set; }
            public string SerialNumber { get; private set; }
        }

    }
}
