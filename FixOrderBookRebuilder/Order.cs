using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixOrderBookRebuilder
{
    public class Order
    {
        public Order()
        {
            this.Initiliase();
        }

        private void Initiliase()
        {
            _ClOrdIDs = new HashSet<string>();
            _OrderIDs = new HashSet<string>();
            _AllMessages = new List<string>();
        }

        public List<string> AllMessages()
        {
            return _AllMessages;
        }
        
        public string GetFirstClOrdID
        {
            get { return _ClOrdIDs.First(); }
        }

        private HashSet<string> _ClOrdIDs;
        private HashSet<string> _OrderIDs;
        private List<string> _AllMessages;

        public void AddClOrdID(string value)
        {
            if (!_ClOrdIDs.Contains(value))
            {
                _ClOrdIDs.Add(value);
            }
        }

        public void AddOrderID(string value)
        {
            if (!_OrderIDs.Contains(value))
            {
                _OrderIDs.Add(value);
            }
        }

        public void AddMessages(string value)
        {
            _AllMessages.Add(value); 
        }

        public bool ContainsClOrdID(string value)
        {
            if (_ClOrdIDs.Contains(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ContainsOrderID(string value)
        {
            if (_OrderIDs.Contains(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetAllMessages()
        {
            string output = "";
            foreach (string s in _AllMessages)
            {
                output += (s + System.Environment.NewLine);
            }
            return output;
        }

        public int MessageCount
        {
            get { return _AllMessages.Count; }
        }


    }
}
