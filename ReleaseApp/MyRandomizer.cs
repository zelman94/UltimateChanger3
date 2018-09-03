using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UltimateChanger
{
    class MyRandomizer
    {


        public static int GetRandomInt(int MAX,int Times)
        {
            List<int> randoms = new List<int>();
            for (int i = 0; i < MAX; i++)
            {
                randoms.Add(0);
            }
            for (int i = 0; i < Times; i++)
            {

                Random rand = new Random();
                randoms[rand.Next(MAX)]++;
            }
            return randoms.IndexOf(randoms.Max());
        }

        private static int seed;

        private static ThreadLocal<Random> threadLocal = new ThreadLocal<Random>
            (() => new Random(Interlocked.Increment(ref seed)));

        static MyRandomizer()
        {
            seed = Environment.TickCount;
        }

        public static Random Instance { get { return threadLocal.Value; } }

    }
}
