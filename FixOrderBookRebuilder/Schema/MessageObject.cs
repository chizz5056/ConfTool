using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixOrderBookRebuilder.Schema
{
    public abstract class MessageObject
    {
        public string Name { get; set; }

        public bool Required { get; set; }

        public int Tag { get; set; }

        public Field Field { get; set; }
    }

}
