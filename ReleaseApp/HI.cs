using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{
    class HI
    {
        public HI()
        {
            this.genericProductModelConfiguration = new GenericProductModelConfiguration();
            this.hardwarePlatformConfiguration = new HardwarePlatformConfiguration();
            this.internalInstrumentModelConfiguration = new InternalInstrumentModelConfiguration();
            this.brandedProductModelConfiguration = new BrandedProductModelConfiguration();
            this.internalInstrumentModelConfigurationVersion = new InternalInstrumentModelConfigurationVersion();
            this.serialNumber = new SerialNumber();
            this.chipsetName = new ChipsetName();
        }

        public HI(JObject jsonn, string side)
        {
            this.genericProductModelConfiguration = new GenericProductModelConfiguration();
            this.hardwarePlatformConfiguration = new HardwarePlatformConfiguration();
            this.internalInstrumentModelConfiguration = new InternalInstrumentModelConfiguration();
            this.brandedProductModelConfiguration = new BrandedProductModelConfiguration();
            this.internalInstrumentModelConfigurationVersion = new InternalInstrumentModelConfigurationVersion();
            this.serialNumber = new SerialNumber();
            this.chipsetName = new ChipsetName();

            try
            {
                this.chipsetName.Value = jsonn["data"][side]["ChipsetName"]["Value"].ToString();
            }
            catch (Exception)
            {
                this.chipsetName.Value = "error";
            }
            try
            {
                this.internalInstrumentModelConfiguration.Value = jsonn["data"][side]["InternalInstrumentModelConfiguration"]["Value"].ToString();
            }
            catch (Exception)
            {
                this.internalInstrumentModelConfiguration.Value = "error";
            }
            try
            {
                this.brandedProductModelConfiguration.Value = jsonn["data"][side]["BrandedProductModelConfiguration"]["Value"].ToString();
            }
            catch (Exception)
            {
                this.brandedProductModelConfiguration.Value = "error";
            }
            try
            {
                this.serialNumber.Value = jsonn["data"][side]["SerialNumber"]["Value"].ToString();
            }
            catch (Exception)
            {
                this.serialNumber.Value = "error";
            }

            try
            {
                this.hardwarePlatformConfiguration.Value = jsonn["data"][side]["HardwarePlatformConfiguration"]["Value"].ToString();
            }
            catch (Exception)
            {
                this.hardwarePlatformConfiguration.Value = "error";
            }


        }

        public HI(GenericProductModelConfiguration genericProductModelConfiguration, HardwarePlatformConfiguration hardwarePlatformConfiguration, InternalInstrumentModelConfiguration internalInstrumentModelConfiguration, BrandedProductModelConfiguration brandedProductModelConfiguration, InternalInstrumentModelConfigurationVersion internalInstrumentModelConfigurationVersion, SerialNumber serialNumber, ChipsetName chipsetName)
        {
            this.genericProductModelConfiguration = genericProductModelConfiguration;
            this.hardwarePlatformConfiguration = hardwarePlatformConfiguration;
            this.internalInstrumentModelConfiguration = internalInstrumentModelConfiguration;
            this.brandedProductModelConfiguration = brandedProductModelConfiguration;
            this.internalInstrumentModelConfigurationVersion = internalInstrumentModelConfigurationVersion;
            this.serialNumber = serialNumber;
            this.chipsetName = chipsetName;
        }

        public GenericProductModelConfiguration genericProductModelConfiguration { get; set; }
        public HardwarePlatformConfiguration hardwarePlatformConfiguration { get; set; }
        public InternalInstrumentModelConfiguration internalInstrumentModelConfiguration { get; set; }
        public BrandedProductModelConfiguration brandedProductModelConfiguration { get; set; }
        public InternalInstrumentModelConfigurationVersion internalInstrumentModelConfigurationVersion { get; set; }
        public SerialNumber serialNumber { get; set; }
        public ChipsetName chipsetName { get; set; }


    }

    class GenericProductModelConfiguration
    {
        public string FullName { get; set; }
        public string Value { get; set; }
        public string Uid { get; set; }
    }

    class HardwarePlatformConfiguration
    {
        public string FullName { get; set; }
        public string Value { get; set; }
        public string Uid { get; set; }
    }
    class InternalInstrumentModelConfiguration
    {
        public string FullName { get; set; }
        public string Value { get; set; }
        public string Uid { get; set; }
    }
    class BrandedProductModelConfiguration
    {
        public string FullName { get; set; }
        public string Value { get; set; }
        public string Uid { get; set; }

    }
    class InternalInstrumentModelConfigurationVersion
    {
        public string FullName { get; set; }
        public string Value { get; set; }

    }
    class SerialNumber
    {
        public string FullName { get; set; }
        public string Value { get; set; }

    }
    class ChipsetName
    {
       public string FullName { get; set; }
        public string Value { get; set; }

    }

}
