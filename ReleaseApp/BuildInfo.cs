using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{

    public class BuildInfo
    {
        public string Brand, MarketName, OEM, SelectedLanguage, Version;
        static public List<string> ListPathsToManInfo = new List<string>()
        {
            @"C:\ProgramData\Oticon\Common\ManufacturerInfo.xml",
            @"C:\ProgramData\Bernafon\Common\ManufacturerInfo.xml",
            @"C:\ProgramData\Sonic\Common\ManufacturerInfo.xml",
            @"C:\ProgramData\OticonMedical\Common\ManufacturerInfo.xml",
             @"C:\ProgramData\Philips HearSuite\Common\ManufacturerInfo.xml"
        };
        static public List<string> ListPathsToSetup = new List<string>()
        {
            @"C:\Program Files (x86)\Oticon\Genie\Genie2\Genie.exe",
            @"C:\Program Files (x86)\Bernafon\\Oasis\Oasis2\Oasis.exe",
            @"C:\Program Files (x86)\Sonic\\ExpressFit\EXPRESSfit2\EXPRESSfit.exe",
            @"C:\Program Files (x86)\OticonMedical\GenieMedical\GenieMedical2\GenieMedical.exe",
            @"C:\Program Files (x86)\Philips HearSuite\HearSuite\HearSuite2\HearSuite.exe"
        };

        static public List<string> ListPathsToHattori = new List<string>()
        {
            @"C:\Program Files (x86)\Oticon\Genie\Genie2\",
            @"C:\Program Files (x86)\Bernafon\Oasis\Oasis2\",
            @"C:\Program Files (x86)\Sonic\ExpressFit\ExpressFit2\",
            @"C:\Program Files (x86)\OticonMedical\GenieMedical\GenieMedical2\",
            @"C:\Program Files (x86)\Philips HearSuite\HearSuite\HearSuite2\"
        };

        static public List<string> ListPathsToAboutInfo = new List<string>()
        {
            @"C:\ProgramData\Oticon\Genie2\ApplicationVersion.xml",
            @"C:\ProgramData\Bernafon\Oasis2\ApplicationVersion.xml",
            @"C:\ProgramData\Sonic\EXPRESSfit2\ApplicationVersion.xml",
            @"C:\ProgramData\OticonMedical\GenieMedical2\ApplicationVersion.xml",
             @"C:\ProgramData\Philips HearSuite\HearSuite2\ApplicationVersion.xml"
        };
        public BuildInfo(string brand, string market, string oem, string language, string ver)
        {
            this.Brand = brand;
            this.MarketName = market;
            this.OEM = oem;
            this.SelectedLanguage = language;
            this.Version = ver;
        }
    }
}
