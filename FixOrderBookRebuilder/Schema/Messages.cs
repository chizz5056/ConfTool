using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixOrderBookRebuilder.Schema
{
    public class Messages
    {
        private static readonly Lazy<Messages> lazy = new Lazy<Messages>(() => new Messages());
        public static Messages Instance { get { return lazy.Value; } }

        SortedDictionary<string, Message> MessageList;

        private Messages()
        {
            MessageList = new SortedDictionary<string, Message>();
        }

        public void AddMessage(Message m)
        {
            MessageList.Add(m.MsgType, m);
        }
    }
}
