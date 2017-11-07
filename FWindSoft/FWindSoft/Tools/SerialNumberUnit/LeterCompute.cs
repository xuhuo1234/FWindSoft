using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FWindSoft.Tools
{
    /// <summary>
    /// 字母计算
    /// </summary>
    public class LeterCompute
    {
        private static List<char> m_Lowerletters = new List<char>();
        private static List<char> m_Upperletters = new List<char>();

        static LeterCompute()
        {
            for (int i = 0; i < 26; i++)
            {
                m_Upperletters.Add((char)('A' + i));
                m_Lowerletters.Add((char)('a' + i));
            }
        }

        /// <summary>
        /// 计算字母加上某数字
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="num">26以内</param>
        /// <param name="isCarry">标识是否进位</param>
        /// <returns></returns>
        private static char Plus(char letter, int num, out bool isCarry)
        {
            List<char> Currentletters = m_Upperletters;
            if (char.IsLower(letter))
            {
                Currentletters =m_Lowerletters;
            }
            //letter = char.ToUpper(letter);
            isCarry = false;
            int newNum = Currentletters.FindIndex(0, c => c == letter) + num;
            int quotient = newNum / 26;
            int mod = newNum % 26;
            if (quotient > 0) isCarry = true;
            return Currentletters[mod];
        }

        public static string PlusOne(string no)
        {
            Stack<char> chars = new Stack<char>();
            int index = no.ToList().FindLastIndex(c => !char.IsLetter(c));
            string newNo = no;
            if (index == -1)
            {
                newNo = no;
            }
            else if (index == (no.Length - 1))
            {
                return no;
            }
            else
            {
                newNo = no.Substring(index + 1);
            }
            newNo.ToList().ForEach(c => chars.Push(c));
            Stack<char> newChars = new Stack<char>();
            bool isCarry = false;
            int flag = 0;
            while (chars.Count > 0)
            {
                char tempChar = chars.Pop();
                char newChar = tempChar;
                if (flag++ == 0)
                {
                    newChar = Plus(tempChar, 1, out isCarry);
                }
                if (isCarry)
                {
                    newChar = Plus(tempChar, 1, out isCarry);
                }

                newChars.Push(newChar);
            }
            if (isCarry)
            {
                char lastChar = newChars.Peek();
                newChars.Push(char.IsLower(lastChar) ? 'a' : 'A');
            }
            StringBuilder builder = new StringBuilder();
            while (newChars.Count > 0)
            {
                builder.Append(newChars.Pop());
            }
            return builder.ToString();
        }

        //public string Plus(char letter, int num)
        //{
        //    int newNum = m_letters.FindIndex(0, c => c == letter) + num;
        //    return GetMod(newNum);
        //}
        //private string GetMod(int num)
        //{
        //    int quotient = num / 26;
        //    int mod = num % 26;
        //    string chars="";
        //    if (quotient > 0)
        //    {
        //        chars = GetMod(quotient);
        //    }
        //    chars += m_letters[mod].ToString();
        //    return chars;
        //}
    }

    /// <summary>
    /// 数字编号类
    /// </summary>
    public class DigitCompute
    {
        private static char Plus(char letter, int num, out bool isCarry)
        {
            isCarry = false;
            int newNum = Convert.ToInt32(letter.ToString()) + num;
            int quotient = newNum / 10;
            int mod = newNum % 10;
            if (quotient > 0) isCarry = true;
            return Convert.ToChar(mod.ToString());
        }
        public static string PlusOne(string no)
        {
            Stack<char> chars = new Stack<char>();
            int index = no.ToList().FindLastIndex(c => !char.IsDigit(c));
            string newNo = no;
            if (index == -1)
            {
                newNo = no;
            }
            else if (index == (no.Length - 1))
            {
                return no;
            }
            else
            {
                newNo = no.Substring(index + 1);
            }
            newNo.ToList().ForEach(c => chars.Push(c));
            Stack<char> newChars = new Stack<char>();
            bool isCarry = false;
            int flag = 0;
            while (chars.Count > 0)
            {
                char tempChar = chars.Pop();
                char newChar = tempChar;
                if (flag++ == 0)
                {
                    newChar = Plus(tempChar, 1, out isCarry);
                }
                if (flag != 0 && isCarry)
                {
                    newChar = Plus(tempChar, 1, out isCarry);
                }

                newChars.Push(newChar);
            }
            if (isCarry)
            {
                newChars.Push('1');
            }
            StringBuilder builder = new StringBuilder();
            while (newChars.Count > 0)
            {
                builder.Append(newChars.Pop());
            }
            return builder.ToString();
        }
    }

    /// <summary>
    /// 字符串编号类
    /// </summary>
    public class StrNoCoumpute
    {
        private static List<char> m_Lowerletters = new List<char>();
        private static List<char> m_Upperletters = new List<char>();

        static StrNoCoumpute()
        {
            for (int i = 0; i < 26; i++)
            {
                m_Upperletters.Add((char)('A' + i));
                m_Upperletters.Add((char)('a' + i));
            }
        }
        public static string PlusOne(string strNo)
        {
            string newNo = strNo;
            if (char.IsDigit(strNo.LastOrDefault()))
            {
                int index = strNo.ToList().FindLastIndex(c => !char.IsDigit(c));
                string strBefore = strNo.Substring(0, index + 1);
                string strAfter = strNo.Substring(index + 1);
                newNo = strBefore + DigitCompute.PlusOne(strAfter);
            }
            else if (char.IsLetter(strNo.LastOrDefault()))
            {
                int index = strNo.ToList().FindLastIndex(c => !char.IsLetter(c));
                string strBefore = strNo.Substring(0, index + 1);
                string strAfter = strNo.Substring(index + 1);
                newNo = strBefore + LeterCompute.PlusOne(strAfter);
            }
            return newNo;
        }

        /// <summary>
        /// 自动在最后一位编号加上指定数字
        /// </summary>
        /// <param name="loopNo"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private static string AutoPlus(string loopNo, int step = 1)
        {
            string newLoopNo = loopNo;
            if (string.IsNullOrWhiteSpace(newLoopNo))
                return newLoopNo;
            string startChar = "([{（【{";
            string endChar = ")]}）】}";
            Stack<int> stackIndex = new Stack<int>();
            bool isPush = true;
            for (int i = 0; i < newLoopNo.Length; i++)
            {
                char single = newLoopNo[i];
                do
                {
                    if (isPush && char.IsDigit(single))
                    {
                        if (stackIndex.Count > 0 && stackIndex.Peek() != (i - 1))
                        {
                            stackIndex.Clear();
                        }
                        stackIndex.Push(i);
                        break;
                    }
                    if (startChar.Contains(single))
                    {
                        isPush = false;
                        break;
                    }
                    if (endChar.Contains(single))
                    {
                        isPush = true;
                    }
                } while (false);
            }
            int count = stackIndex.Count;
            try
            {
                if (count > 0)
                {
                    int end = stackIndex.Peek();
                    int start = end - count + 1;

                    string digit = newLoopNo.Substring(start, stackIndex.Count);
                    int value = (Convert.ToInt32(digit) + step);
                    if (value > 0)
                    {
                        digit = (value).ToString();
                        newLoopNo = newLoopNo.Remove(start, count).Insert(start, digit);
                    }

                }
            }
            catch (Exception)
            {
            }
            return newLoopNo;
        }

        public static int Compare(string no1, string no2)
        {
            if (null == no1 && null == no2)
            {
                return 0;
            }
            if (null == no1)
            {
                return -1;
            }
            if (null == no2)
            {
                return 1;
            }
            int flag = 0;
            List<string> listNo1 = GroupNo(no1);
            List<string> listNo2 = GroupNo(no2);
            int groupNo = new List<int>() { listNo1.Count, listNo2.Count }.Min();
            #region 目前只处理最后一组信息(9-18 全部比较)

            int groupIndex = -1;
            while (groupNo>0)
            {
                groupNo--;
                groupIndex++;
                #region 单段处理
                do
                {
                    try
                    {
                        string tempNo1 = listNo1[groupIndex];
                        string tempNo2 = listNo2[groupIndex];
                        char lastChar1 = tempNo1.LastOrDefault();
                        char lastChar2 = tempNo2.LastOrDefault();
                        if (char.IsNumber(lastChar1) && char.IsLetter(lastChar2))
                        {
                            flag = -1;
                            break;
                        }
                        if (char.IsLetter(lastChar1) && char.IsNumber(lastChar2))
                        {
                            flag = 1;
                            break;
                        }
                        if (char.IsNumber(lastChar1) && char.IsNumber(lastChar2))
                        {
                            int intNo1, intNo2;
                            int.TryParse(tempNo1, out intNo1);
                            int.TryParse(tempNo2, out intNo2);
                            flag = intNo1.CompareTo(intNo2);
                            break;
                        }
                        if (char.IsLetter(lastChar1) && char.IsLetter(lastChar2))
                        {
                            int count = new List<int>() {tempNo1.Length, tempNo2.Length}.Max();
                            string cloneNo1 = tempNo1.Clone().ToString();
                            string cloneNo2 = tempNo2.Clone().ToString();
                            //加入A比较会出问题，所以加入一个集合中没有的字符
                            while (cloneNo1.Length != count)
                            {
                                cloneNo1=cloneNo1.Insert(0, "1");
                            }
                            while (cloneNo2.Length != count)
                            {
                                cloneNo2 = cloneNo2.Insert(0, "1");
                            }
                            for (int i = 0; i < count; i++)
                            {
                                //设定大小写 相同
                                flag = m_Upperletters.IndexOf(char.ToUpper(cloneNo1[i]))
                                    .CompareTo(m_Upperletters.IndexOf(char.ToUpper(cloneNo2[i])));
                                if (flag != 0)
                                    break;
                            }
                            break;
                        }
                        //flag = tempNo1.CompareTo(tempNo2);
                    }
                    catch (Exception)
                    {

                        flag = 0;
                    }
                } while (false);

                #endregion

                if (flag != 0)
                    break;
            }

            if (flag == 0)
                flag = listNo1.Count.CompareTo(listNo2.Count);
            #endregion
            return flag;
        }
        /// <summary>
        /// 将指定编号分段
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public static List<string> GroupNo(string no)
        {
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(no))
                return list;
            string strNo = no;
            while (strNo.Length > 0)
            {
                string strBefore = "";
                string strAfter = "";
                if (char.IsDigit(strNo.LastOrDefault()))
                {
                    int index = strNo.ToList().FindLastIndex(c => !char.IsDigit(c));
                    strBefore = strNo.Substring(0, index + 1);
                    strAfter = strNo.Substring(index + 1);
                }
                else if (char.IsLetter(strNo.LastOrDefault()))
                {
                    int index = strNo.ToList().FindLastIndex(c => !char.IsLetter(c));
                    strBefore = strNo.Substring(0, index + 1);
                    strAfter = strNo.Substring(index + 1);
                }
                strNo = strBefore;
                list.Add(strAfter);
            }

            return list;
        }
    }
}
