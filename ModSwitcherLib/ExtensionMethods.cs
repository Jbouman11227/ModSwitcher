using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModSwitcherLib
{
    public static class ExtensionMethods
    {
        public static string AddPeriod(this string str)
        {
            return str + (str.EndsWith(".") ? string.Empty : ".");
        }
    }
}
