using System.Linq;
using System.Windows.Controls;
using OBR.Tools.RegexCheck;

namespace FWindSoft.Wpf.Controls
{
    //输入控制
    public interface ITextInputControl
    {
        bool InputControl(string fullText);
        ItemCollection Parmerters { get; }
    }
    //为使用调用ItemCollection封装基类，如果在子类定义临时属性也可以
    public abstract class BaseTextInputControl : ITextInputControl
    {
        private ItemCollection m_Paramters;
        protected BaseTextInputControl()
        {
            ComboBox com = new ComboBox();
            this.m_Paramters = com.Items;
            this.m_Paramters.Clear();
        }

        public abstract bool InputControl(string fullText);
        public ItemCollection Parmerters
        {
            get { return this.m_Paramters; }
        }
    }


    #region 控件输入验证族

    public class AccessInteger : BaseTextInputControl
    {

        public override bool InputControl(string fullText)
        {
            if (fullText == null)
                return false;
            if (fullText.Length == 1)
            {
                if ("-".Equals(fullText) || RegexCheck.IsInteger(fullText))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return RegexCheck.IsInteger(fullText);
        }
    }
    public class AccessPlusInteger : BaseTextInputControl
    {

        public override bool InputControl(string fullText)
        {
            return RegexCheck.IsPlusInteger(fullText);
        }
    }
    public class AccessDecimal : BaseTextInputControl
    {

        public override bool InputControl(string fullText)
        {
            if (fullText.Length == 1)
            {
                if ("-".Equals(fullText) || RegexCheck.IsDecimal(fullText))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return RegexCheck.IsDecimal(fullText) || (fullText.Count(c => c == '.') == 1 && fullText.IndexOf('.') == fullText.Length - 1 && fullText.IndexOf("-.") < 0);//有一个小数点且在末尾
        }
    }
    public class AccessPlusDecimal : BaseTextInputControl
    {

        public override bool InputControl(string fullText)
        {
            if (fullText.Length == 1)
            {
                if (".".Equals(fullText))
                    return false;
            }
            return RegexCheck.IsPlusDecimal(fullText) || (fullText.Count(c => c == '.') == 1 && fullText.IndexOf('.') == fullText.Length - 1);//有一个小数点且在末尾
        }
    }
    public class AccessRangeDecimal : BaseTextInputControl
    {
        public double MaxValue { set; get; }
        public double MinValue { set; get; }
        public override bool InputControl(string fullText)
        {
            double value;
            if (double.TryParse(fullText, out value))
            {
                if (value >= MinValue && value <= MaxValue)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
    }
    #endregion
}
