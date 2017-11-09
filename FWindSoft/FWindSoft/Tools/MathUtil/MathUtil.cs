using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FWindSoft.Tools
{
    public class MathUtil
    {
        /// <summary>
        /// 求最大公约数
        /// </summary>
        /// <param name="ints"></param>
        /// <returns></returns>
        public static int MaxCommonDivisor(List<int> ints)
        {
            if (ints.Count == 0)
                return 0;
            int divisor = ints[0];
            for (int i = 1; i < ints.Count; i++)
            {
                divisor = MaxCommonDivisor(divisor, ints[i]);
            }
            return divisor;
        }
        /// <summary>
        /// 求最大公约数
        /// </summary>
        /// <param name="int1"></param>
        /// <param name="int2"></param>
        /// <returns></returns>
        public static int MaxCommonDivisor(int int1, int int2)
        {
            int divesior = 1;
            if (int1 < int2)
            {
                int temp = int1;
                int1 = int2;
                int2 = temp;
            }

            do
            {
                int tempMod = int1 % int2;
                if (tempMod == 0)
                {
                    divesior = int2;
                    break;
                }
                int1 = int2;
                int2 = tempMod;
            } while (true);
            return divesior;
        }
        /// <summary>
        /// 求最小公倍数
        /// </summary>
        /// <param name="ints"></param>
        /// <returns></returns>
        public static int MinCommonMultiple(List<int> ints)
        {
            if (ints.Count == 0)
                return 0;
            int multiple = ints[0];
            for (int i = 1; i < ints.Count; i++)
            {
                multiple = MinCommonMultiple(multiple, ints[i]);
            }
            return multiple;
        }
        /// <summary>
        /// 求最小公倍数
        /// </summary>
        /// <param name="int1"></param>
        /// <param name="int2"></param>
        /// <returns></returns>
        public static int MinCommonMultiple(int int1, int int2)
        {
            int multiple = int1 * int2;
            if (multiple == 0)
                return multiple;
            int divesior = MaxCommonDivisor(int1, int2);
            return multiple / divesior;
        }
    }
}
