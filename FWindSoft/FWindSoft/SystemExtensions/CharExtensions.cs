using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWindSoft.SystemExtensions
{
    public static class CharExtensions
    {
        public static bool IsNumeric(this char c)
        {
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                case '.':
                case 'E':
                case 'e':
                    return true;
            }
            return false;
        }
    }
}
