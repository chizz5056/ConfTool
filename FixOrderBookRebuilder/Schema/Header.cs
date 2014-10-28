using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixOrderBookRebuilder.Schema
{
    public class Header
    {
        private static readonly Lazy<Header> lazy = new Lazy<Header>(() => new Header());
        public static Header Instance { get { return lazy.Value; } }

        HashSet<MessageField> _fields;

        private Header()
        {
            _fields = new HashSet<MessageField>();
        }

        public void AddMessageField(string name, bool required)
        {
            MessageField mf = new MessageField(name, required);
            _fields.Add(mf);
        }
    }
}
