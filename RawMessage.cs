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

        public QuickFix.Message QFNMessage()
        {
            string raw=Message;
            if (raw.Contains("|"))
            {
                raw = raw.Replace("|", "\x01");
            }
            var qfnMessage = new QuickFix.Message(raw, false);
            qfnMessage.Header.SetField(new QuickFix.Fields.MsgType(MsgType));
            qfnMessage.Header.SetField(new QuickFix.Fields.BeginString(ConfigurationSingleton.Instance.GetSetting("BeginString")));
            qfnMessage.Header.SetField(new QuickFix.Fields.SenderCompID(ConfigurationSingleton.Instance.GetSetting("SenderCompID")));
            qfnMessage.Header.SetField(new QuickFix.Fields.TargetCompID(ConfigurationSingleton.Instance.GetSetting("TargetCompID")));
            qfnMessage.Header.SetField(new QuickFix.Fields.DeliverToCompID(ConfigurationSingleton.Instance.GetSetting("DeliverToCompID")));

            return qfnMessage;
        }
    }
}
