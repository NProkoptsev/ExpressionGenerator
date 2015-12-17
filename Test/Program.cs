using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionGenerator;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(EGHelper.generate(2,77,100,5));
        }
    }
}
