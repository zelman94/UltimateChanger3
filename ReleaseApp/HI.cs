using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{
    class HI
    {
        data data{ get; set; }
    }

    class data
    {
        Left left { get; set; }
    }

    class Left
    {
        GenericProductModelConfiguration genericProductModelConfiguration { get; set; }
        HardwarePlatformConfiguration hardwarePlatformConfiguration { get; set; }
        InternalInstrumentModelConfiguration internalInstrumentModelConfiguration { get; set; }
        BrandedProductModelConfiguration brandedProductModelConfiguration { get; set; }
        InternalInstrumentModelConfigurationVersion internalInstrumentModelConfigurationVersion { get; set; }
        SerialNumber serialNumber { get; set; }
        ChipsetName chipsetName { get; set; }
    }

    class GenericProductModelConfiguration
    {
        string FullName { get; set; }
        string Value { get; set; }
        string Uid { get; set; }
    }

    class HardwarePlatformConfiguration
    {
        string FullName { get; set; }
        string Value { get; set; }
        string Uid { get; set; }
    }
    class InternalInstrumentModelConfiguration
    {
        string FullName { get; set; }
        string Value { get; set; }
        string Uid { get; set; }
    }
    class BrandedProductModelConfiguration
    {
        string FullName { get; set; }
        string Value { get; set; }
        string Uid { get; set; }

    }
    class InternalInstrumentModelConfigurationVersion
    {
        string FullName { get; set; }
        string Value { get; set; }

    }
    class SerialNumber
    {
        string FullName { get; set; }
        string Value { get; set; }

    }
    class ChipsetName
    {
        string FullName { get; set; }
        string Value { get; set; }

    }

}
