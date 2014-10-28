using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixOrderBookRebuilder.Chain
{
    public struct OrderChainMessage
    {
        public string MsgType { get; set; }
        public string OrdType { get; set; }
        public string ExecType { get; set; }
        public string OrdStatus { get; set; }

        public string AllFields
        {
            get
            {
                string u = string.Empty;

                if (!String.IsNullOrEmpty(MsgType))
                    u += MsgType + "|";
                if (!String.IsNullOrEmpty(OrdType))
                    u += OrdType + "|";
                if (!String.IsNullOrEmpty(OrdStatus))
                    u += OrdStatus + "|";
                if (!String.IsNullOrEmpty(ExecType))
                    u += ExecType + "|";

                return u;
            }
                
        }
    }
}
