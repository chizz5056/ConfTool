using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixOrderBookRebuilder.Chain
{
    public struct OrderChain
    {
        public List<OrderChainMessage> ocmList;
        public StringBuilder ocmString;
        const char SOH = '\x01';
        public string ClOrdID { get; set; }

        public void Initialise(string clOrdID)
        {
            ocmList = new List<OrderChainMessage>();
            ocmString = new StringBuilder();
            ClOrdID = clOrdID;
        }

        public void AddOCM(OrderChainMessage ocm)
        {            
            ocmList.Add(ocm);
            ocmString.Append(ocm.AllFields + SOH);
        }

        public string AllStrings()
        {
            return ocmString.ToString();
        }

    }
}
