using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSEHub.ConfTool
{
    public class TagGenerator
    {
        private int _ID;
        public TagGenerator()
        {
            _ID = 1;
        }

        public string GetClOrdID()
        {
            string s = "LSE" + _ID.ToString().PadLeft(5, '0');
            _ID++;
            return s; 
        }

        public string GetPrevClOrdID()
        {
            string s = "LSE" + (_ID-2).ToString().PadLeft(5, '0');
            return s;
        }

        public string GetUtcDate()
        {
            return DateTime.UtcNow.ToString("yyyyMMdd");
        }

        public string GetUtcTimestamp()
        {
            return DateTime.UtcNow.ToString("yyyyMMdd-hh:mm:ss");
        }
    }
}
