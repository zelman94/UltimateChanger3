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
