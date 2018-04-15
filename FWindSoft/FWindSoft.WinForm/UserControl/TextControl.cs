using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FWindSoft.SystemExtensions;


namespace FWindSoft.WinForm
{
    public class TextControl
    {
        private Control m_RefControl;
       
        public  TextControl(Control ctrl)
        {
            m_RefControl = ctrl;
            TextBox textBox = ctrl as TextBox;
            if (textBox != null)
            {
                IsValid=true;
                Text=textBox.Text;
                SelectionStart=textBox.SelectionStart;
                SelectionLength=textBox.SelectionLength;
                return;
            }
            ComboBox cmb = ctrl as ComboBox;
            if (cmb != null)
            {
                IsValid=true;
                Text=cmb.Text;
                SelectionStart=cmb.SelectionStart;
                SelectionLength=cmb.SelectionLength;
                return;
            }

        }
        public bool IsValid{get;private set;}
        public Control RefControl { get { return this.m_RefControl; } }
        public string Text
        {
            get;private set;
        }

        public int SelectionStart
        {
            get;private set;
        }

        public int SelectionLength
        {
            get;private set;
        }

        /// <summary>
        /// 计算新的text
        /// </summary>
        /// <param name="ch">插入的c</param>
        /// <returns></returns>
        public string ClcNewText(char ch)
        {
            string strCurText = this.Text;
            if (SelectionLength <= 0)
            {
                strCurText = this.Text;
                strCurText = strCurText.Insert(this.SelectionStart, ch.ToString());
            }
            else
            {
                string a = strCurText.Substring(0, this.SelectionStart);
                string b = ch.ToString();
                string c = string.Empty;
                int intSelectIndex = this.SelectionStart + this.SelectionLength;
                if (intSelectIndex < strCurText.Length)
                {
                    c = strCurText.Substring(intSelectIndex);
                }
                strCurText = a + b + c;
            }
            return strCurText;
        }

        /// <summary>
        /// 验证控件输入键盘事件之后是否为数字
        /// </summary>
        /// <param name="e">控件输入的键盘事件</param>
        /// <returns></returns>
        public bool ValidateNumeric(KeyPressEventArgs e)
        {
            do
            {
                if (e.KeyChar == (char)Keys.Back)
                {
                    e.Handled = false;
                    break;
                }
                if (!e.KeyChar.IsNumeric())
                {
                    e.Handled = true;
                    break;
                }
                string newString = this.ClcNewText(e.KeyChar);
                e.Handled = !RegexCheck.IsDecimalInputing(newString); 
            } while (false);
            return !e.Handled;
        }
    }
}
