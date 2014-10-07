using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSEHub.ConfTool
{
    public struct TagValue
    {
        public int Tag {get;set;}
        public string Value {get;set;}

        public TagValue(int tag, string val) : this()
        {
            Tag = tag;
            Value = val;
        }

        public string GetTagVal()
        {
            return Tag + "=" + Value;
        }

    }
}
