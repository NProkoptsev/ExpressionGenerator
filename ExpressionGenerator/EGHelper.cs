using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionGenerator
{
    public class EGHelper
    {
        public static string generate(int depth, int ivalue, int limit, int length)
        {
            Generator generator = new Generator(depth, ivalue, limit, length);
            StringBuilder ternar = new StringBuilder();
            ternar.Append('(');
            for (int i = 0; i < 3; i++)
            {
                if (i != 0)
                    ternar.Append('+');
                ternar.Append(generator.mRandom.Next(-5, 6));
                for (int j = 0; j < i; j++)
                    ternar.Append("*j");
            }
            ternar.Append(generator.mRandom.Next(2) == 0 ? " > " : " < ");
            ternar.Append(generator.mRandom.Next(10).ToString());
            ternar.Append(") ? ");
            generator.generate();
            ternar.Append(generator.ToString());
            ternar.Append(" : ");
            Generator generator1 = new Generator(depth, ivalue, limit, length);
            generator1.generate();
            ternar.Append(generator1.ToString());
            return ternar.ToString();
        }
    }
}