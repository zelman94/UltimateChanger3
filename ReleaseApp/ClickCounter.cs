using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{
    public enum Buttons
    {
        StartFittingSoftware,
        StartHAttori,
        InstallFittingSoftware,
        UpdateMarket,
        UpdateMode,
        DeleteLogs,
        UninstallFittingSoftware,
        Kill,
        Downgrade,
        RandomHI,
        CopyMyHardware
    };

    public class ClickCounter
    {
        private int[] clicks;
        public int[] Clicks
        {
            get
            {
                return clicks;
            }
        }
        private int sumOfClicks;
        public int SumOfClicks
        {
            get
            {
                return sumOfClicks;
            }
        }

        public ClickCounter(int numberOfButtons)
        {
            clicks = new int[numberOfButtons+1];
            sumOfClicks = 0;
        }

        public void AddClick(int button)
        {
            clicks[button]++;
            sumOfClicks++;
        }
    }
}
