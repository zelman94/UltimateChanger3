﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChanger
{
    public class pathAndDir
    {
        public List<string> path;
        public List<string> dir;
        public pathAndDir()
        {
            path = new List<string>();
            dir = new List<string>();
        }

        public pathAndDir(pathAndDir tmp)
        {
            path = tmp.path;
            dir = tmp.dir;
        }


    }
}
