using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FWindSoft.Wpf
{
    public static class LengthConvert
    {
        public static DataGridLength ConvertDataGridLength(this GridLength gridLength)
        {
            var unitType=gridLength.GridUnitType;
            return new DataGridLength();
        }
    }
}
