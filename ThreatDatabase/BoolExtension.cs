﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreatDatabase
{
    static class BoolExtension
    {
        public static string MyToString(this bool b)
        {

            return b ? "Да" : "Нет";

        }
    }
}
