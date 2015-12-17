using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionGenerator
{
    internal class Utilities
    {
        internal static List<int> findDividers(int d)
        {
            List<int> list = new List<int>();
            int value = Math.Abs(d);
            for (int i = 2; i <= value / 2; i++)
                if (value % i == 0)
                    list.Add(i);
            return list;
        }

        internal static List<int> findRemainders(int d, int mLimit)
         {
             List<int> list = new List<int>();
             int value = Math.Abs(d);
             for (int i = 2 * value; i <= mLimit; i += value)
                 list.Add(i);
             return list;
         }

        internal static string serializePolynom(List<int> pol, int length, char variable)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if (pol[i] != 0 || length == 1)
                {
                    if (i != 0)
                        s.Append("+");
                    s.Append(pol[i].ToString());
                    for (int j = 0; j < i; j++)
                        s.Append("*" + variable);
                }
            }
            return s.ToString();
        }


    }
}
