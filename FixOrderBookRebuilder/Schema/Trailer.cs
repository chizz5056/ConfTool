using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixOrderBookRebuilder.Schema
{
    public class Trailer
    {
        private static readonly Lazy<Trailer> lazy = new Lazy<Trailer>(() => new Trailer());
        public static Trailer Instance { get { return lazy.Value; } }

        HashSet<MessageField> _fields;

        private Trailer()
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
