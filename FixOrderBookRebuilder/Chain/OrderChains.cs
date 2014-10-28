using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixOrderBookRebuilder.Chain
{
    public class OrderChains
    {
        HashSet<OrderChain> ocHashset;
        public Dictionary<string,string> ocDict;

        public OrderChains()
        {
            ocHashset = new HashSet<OrderChain>();
            ocDict = new Dictionary<string, string>();
        }

        public void AddOC(OrderChain oc)
        {
            if (!ocHashset.Contains(oc))
            {
                ocHashset.Add(oc);
            }
            else
            {
                Console.WriteLine("Duplicate OrderChain");
            }

            if (!ocDict.ContainsValue(oc.AllStrings()))
            {
                ocDict.Add(oc.ClOrdID, oc.AllStrings());
            }
            else
            {
                Console.WriteLine("Duplicate OrderChain:" + System.Environment.NewLine + oc.AllStrings());
            }
        }
    }
}
