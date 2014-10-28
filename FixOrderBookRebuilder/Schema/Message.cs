using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixOrderBookRebuilder.Schema
{
    public class Message
    {
        public string Name { get; set; }
        public string MsgType { get; set; }
        public string MsgCat { get; set; }

        public HashSet<MessageObject> MessageObjects;

        public Message(string name, string msgtype, string msgcat)
        {
            Name = name;
            MsgType = msgtype;
            MsgCat = msgcat;
            MessageObjects = new HashSet<MessageObject>();
        }

        public void AddMessageObject(MessageObject mo)
        {
            MessageObjects.Add(mo);
        }
    }
}
