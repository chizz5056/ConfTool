using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FixSchema;

namespace TagComparisonTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Schema schema = new Schema();
            Field f = Fields.Instance.GetField(31);
            string left = "0.0000";
            string right = "0.000000";

            bool test = FixDataTypes.Compare(f, left, right);

            Console.WriteLine(test.ToString());

        }
    }
}
