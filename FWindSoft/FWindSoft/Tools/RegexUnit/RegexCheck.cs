
using System.Linq;
using System.Text.RegularExpressions;

namespace FWindSoft.Tools
{
    /// <summary>
    /// 正则检测
    /// </summary>
    public class RegexUnit
    {
        private readonly static Regex m_ReInteger = new Regex(@"^\-?(0|([1-9]\d*))$");
        private readonly static Regex m_RePlusInteger = new Regex(@"^(([1-9]\d*)|0)$");
        private readonly static Regex m_ReDecimal = new Regex(@"^-?(([1-9]\d*\.\d+)|(0\.\d+)|([1-9]\d*)|(0))$");
        private readonly static Regex m_RePlusDecimal = new Regex(@"^(([1-9]\d*\.\d+)|(0\.\d+)|([1-9]\d*)|(0))$");

        public static bool IsInteger(string match)
        {
           return m_ReInteger.IsMatch(match);
        }
        public static bool IsPlusInteger(string match)
        {
            return m_RePlusInteger.IsMatch(match);
        }
        public static bool IsDecimal(string match)
        {
            return m_ReDecimal.IsMatch(match);
        }
        public static bool IsPlusDecimal(string match)
        {
            return m_RePlusDecimal.IsMatch(match);
        }

        #region 输入中是否满足
        /// <summary>
        /// 是否满足整数的输入过程
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool IsIntegerInputing(string match)
        {
            if (match == null)
                return false;
            if (match.Length == 1)
            {
                if ("-".Equals(match) || m_ReInteger.IsMatch(match))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return m_ReInteger.IsMatch(match);
        }
        public static bool IsPlusIntegerInputing(string match)
        {
            if (match == null)
                return false;
            return m_RePlusInteger.IsMatch(match);
        }
        public static bool IsDecimalInputing(string match)
        {
            if (match == null)
                return false;
            if (match.Length == 1)
            {
                if ("-".Equals(match) || m_ReDecimal.IsMatch(match))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return m_ReDecimal.IsMatch(match) || (match.Count(c => c == '.') == 1 && match.IndexOf('.') == match.Length - 1 && match.IndexOf("-.") < 0);//有一个小数点且在末尾
        }
        public static bool IsPlusDecimalInputing(string match)
        {
            if (match == null)
                return false;
            if (match.Length == 1)
            {//第一个位置不能为“.”
                if (".".Equals(match))
                    return false;
            }
            return m_RePlusDecimal.IsMatch(match) || (match.Count(c => c == '.') == 1 && match.IndexOf('.') == match.Length - 1);//有一个小数点且在末尾
        } 
        #endregion
    }
}
