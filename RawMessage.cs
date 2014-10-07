using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSEHub.ConfTool
{
    public class RawMessage
    {
        public string Message { get; set; }
        public string MsgType { get; set; }
        public MessageDirection Direction { get; set; }
        

        public RawMessage(string message, MessageDirection md)
        {
            Message = message;
            Direction = md;
            MsgType = MessageFunctions.GetTagValue(35, message);
        }
    }
}
