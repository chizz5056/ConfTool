using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSEHub.ConfTool
{
    public enum ScenarioType
    {
        Explicit,
        Raw
    }

    public enum Side
    {
        BUY = 1,
        SELL = 2
        //BuyMinus = 3,
        //SellPlus = 4,
        //SellShort = 5,
        //SellShortExampt = 6,
        //Undisclosed = 7,
        //Cross = 8,
        //CrossShort = 9
    }

    public enum TagLocation
    {
        SOURCE,
        DESTINATION
    }

    public enum MessageDirection
    {
        INBOUND,
        OUTBOUND
    }
}
