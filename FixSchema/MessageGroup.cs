using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixSchema
{
    public class MessageGroup : MessageObject
    {
        private Dictionary<string, MessageObject> Group;

        public MessageGroup(string name, bool required)
        {
            Name = name;
            Required = required;
            Field = Fields.Instance.GetField(name);
            Tag = Field.Tag;
            Group = new Dictionary<string, MessageObject>();
        }

        public void AddMessageObject(MessageObject mo)
        {
            Group.Add(mo.Name, mo);
        }
    }
}
