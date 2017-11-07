using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWindSoft.SystemExtensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmptyImpl(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
    }
}
