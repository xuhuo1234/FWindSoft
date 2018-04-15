using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWindSoft.WinForm
{
    /// <summary>
    /// Frm相关的静态方法
    /// </summary>
    public static class FrmMethod
    {
        public static bool IsDesignMode()
        {
            bool flag = false;
            #if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                flag = true;
            }
            else if (Process.GetCurrentProcess().ProcessName.Equals("devenv"))
            {
                flag = true;
            }
            #endif

            return flag;
        }
    }
}
