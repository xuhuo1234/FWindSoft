using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FWindSoft.WinForm
{
    /// <summary>
    /// 窗口类扩展
    /// </summary>
    public static class WinFormExtensions
    {
        public static Control GetControl(this Control con, string controlName)
        {
            Control confind = null;
            do
            {
                if (null == controlName || "".Equals(controlName))
                {
                    break;
                }
                if (controlName.Equals(con.Name))
                {
                    confind = con;
                    break;
                }
                confind = con.Controls[controlName];
                if (null != confind)
                {
                    break;
                }
                foreach (Control conTemp in con.Controls)
                {
                    confind = conTemp.GetControl(controlName);
                    if (null != confind)
                        break;
                }

            } while (false);

            return confind;
        }

        public static bool IsValidate(this Control control)
        {
            bool flag = true;
            if (control.Controls.Count > 0)
            {
                //controls大于0的不去验证
                foreach (Control subControl in control.Controls)
                {
                    if (!subControl.IsValidate())
                    {
                        flag = false;
                        break;
                    }
                }

            }
            else
            {
                bool isType = false;
                do
                {
                    isType = true;
                    if (control is TextBox)
                    {
                        break;
                    }
                    if (control is ComboBox)
                    {
                        break;
                    }
                    isType = false;
                } while (false);
                if (isType && string.IsNullOrWhiteSpace(control.Text.Trim()))
                {
                    MessageBox.Show((control.Tag ?? "输入值异常").ToString(), "操作提示", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    control.Focus();
                    flag = false;
                }
            }
            return flag;
        }

        private static bool IsValidate(this Control control, Predicate<Control> validateTypes)
        {
            bool flag = true;
            if (control.Controls.Count > 0)
            {
                //controls大于0的不去验证
                foreach (Control subControl in control.Controls)
                {
                    if (!subControl.IsValidate())
                    {
                        flag = false;
                        break;
                    }
                }

            }
            else
            {
                bool isType = validateTypes(control);
                if (isType && string.IsNullOrWhiteSpace(control.Text.Trim()))
                {
                    MessageBox.Show((control.Tag ?? "输入值异常").ToString(), "操作提示", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    control.Focus();
                    flag = false;
                }
            }
            return flag;
        }
        /// <summary>
        /// 设置看
        /// </summary>
        /// <param name="control"></param>
        /// <param name="names"></param>
        /// <param name="visible"></param>
        public static void SetVisible(this Control control, List<string> names, bool visible)
        {
            foreach (string name in names)
            {
                Control cont = control.GetControl(name);
                if (null != cont)
                {
                    cont.Visible = visible;
                }
            }
        }
        /// <summary>
        /// 设置控件可用性
        /// </summary>
        /// <param name="control"></param>
        /// <param name="names"></param>
        /// <param name="enabled"></param>
        public static void SetEnabled(this Control control, List<string> names, bool enabled)
        {
            foreach (string name in names)
            {
                Control cont = control.GetControl(name);
                if (null != cont)
                {
                    cont.Enabled = enabled;
                }
            }
        }
    }
}
