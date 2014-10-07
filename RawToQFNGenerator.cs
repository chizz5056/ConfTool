using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickFix;

namespace LSEHub.ConfTool
{
    static class RawToQFNGenerator
    {

        public static QuickFix.Message QFNMessage(RawMessage raw)
        {
            if (raw.Message.Contains("|"))
            {
                raw.Message = raw.Message.Replace("|", "\x01");
            }
            var qfnMessage = new QuickFix.Message(raw.Message, false);
            qfnMessage.Header.SetField(new QuickFix.Fields.MsgType(raw.MsgType));
            qfnMessage.Header.SetField(new QuickFix.Fields.BeginString(ConfigurationSingleton.Instance.QFNSettings.Get().GetString("BeginString")));
            qfnMessage.Header.SetField(new QuickFix.Fields.SenderCompID(ConfigurationSingleton.Instance.QFNSettings.Get().GetString("SenderCompID")));
            qfnMessage.Header.SetField(new QuickFix.Fields.TargetCompID(ConfigurationSingleton.Instance.QFNSettings.Get().GetString("TargetCompID")));

            return qfnMessage;
        }
    }
}
