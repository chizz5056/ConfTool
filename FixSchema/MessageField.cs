using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixSchema
{
    public class MessageField : MessageObject
    {
        public MessageField(string name, bool required)
        {
            Name = name;
            Required = required;
            Field = Fields.Instance.GetField(name);
            Tag = Field.Tag;
        }


    }
}
