﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOlkin.HelpClasses
{
    public delegate bool Method();

    public class ChangeTimePeriod
    {
        public string Name { get; set; }

        public Method Method { get; set; }


        public ChangeTimePeriod(string name, Method method)
        {
            Name = name;
            Method = method;
        }

        public override string ToString() => Name;
    }
}
