using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSEHub.ConfTool
{
    /// <summary>
    /// This class converts raw fix messages to interbnal format, and in the process of doing also updates any tags relevent to if the messages were from the BUY or SELL sessions
    /// </summary>
    public class RawSideConverter
    {
        private Side _side;
        private Side _actualSide;
        private List<string> _messages;
        private List<RawMessage> _convertedMessages;
        //private const string SOH = "\x01";
        private const string SOH = "|";
        private string _nos;
        private TagGenerator _tagGen;

        public RawSideConverter(Side side, List<string> messages)
        {
            _convertedMessages = new List<RawMessage>();
            _side = side;
            _messages = messages;
            _tagGen = new TagGenerator();
        }

        public void Convert()
        {
            // Find the NewOrderSingle
            foreach (string m in _messages)
            {
                if (m.Contains(SOH + "35=D" + SOH))
                {
                    _nos = m;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(_nos))
            {
                bool obo = _nos.Contains(SOH + "115=");
                bool d2 = _nos.Contains(SOH + "128=");

                if (obo && d2)
                {
                    // On the 'outbound' side of the LSEHub - but to/from another hub
                    _actualSide = Side.SELL;
                }
                else if (obo && !d2)
                {
                    // On the 'outbound' side of the LSEHub - direct to/from SELL side
                    _actualSide = Side.SELL;
                }
                else if (!obo && d2)
                {
                    // On the 'inbound' side of the LSEHub
                    _actualSide = Side.BUY;
                }
                else if (!obo && !d2)
                {
                    // Should never hit this condition
                }

                if (_actualSide != _side)
                {
                    // Requires conversion from one side to other
                    switch (_actualSide)
                    {
                        case Side.BUY:
                            ConvertBUYtoSELL();
                            break;
                        case Side.SELL:
                            ConvertSELLtoBUY();
                            break;
                    }
                }

            }

            


        }

        private void ConvertSELLtoBUY()
        {
            foreach (string m in _messages)
            {
                string newM = m;
                //newM = MessageFunctions.ReplaceTag(115, 128, newM, true, TagLocation.SOURCE);
                //newM = MessageFunctions.ReplaceTag(128, 49, newM, false, TagLocation.SOURCE);

                string msgType = MessageFunctions.GetTagValue(35, newM);
                MessageDirection md = new MessageDirection();

                switch (msgType)
                {
                    case "D":
                    case "F":
                    case "G":
                        // Direction
                        md = MessageDirection.OUTBOUND;

                        // IDs
                        newM = MessageFunctions.SetTagValue(11, newM, _tagGen.GetClOrdID());
                        newM = MessageFunctions.SetTagValue(41, newM, _tagGen.GetPrevClOrdID());
                        

                        // CompIds
                        if (MessageFunctions.ContainsTag(128, newM))
                        {
                            // If message is to a virtual-participant
                            string d2 = MessageFunctions.GetTagValue(128, newM);
                            newM = MessageFunctions.SetTagValue(56, newM, d2);
                            newM = MessageFunctions.RemoveTagAndValue(128, newM);
                        }

                        newM = MessageFunctions.SetTagValue(49, newM, MessageFunctions.GetTagValue(115, newM));
                        newM = MessageFunctions.SetTagValue(115, newM, MessageFunctions.GetTagValue(56, newM));
                        newM = MessageFunctions.SetTagValue(56, newM, "LSEHub");
                        newM = MessageFunctions.SetTag(115, 128, newM);

                        // SubIds
                        newM = MessageFunctions.RemoveTagAndValue(50, newM);
                        newM = MessageFunctions.SetTag(116, 50, newM);
                        newM = MessageFunctions.RemoveTagAndValue(129, newM);
                        newM = MessageFunctions.SetTag(57, 129, newM);

                        // LocationIds
                        newM = MessageFunctions.RemoveTagAndValue(142, newM);
                        newM = MessageFunctions.SetTag(144, 142, newM);
                        newM = MessageFunctions.RemoveTagAndValue(145, newM);
                        newM = MessageFunctions.SetTag(143, 145, newM);

                        // Time/Date tags
                        newM = MessageFunctions.RemoveTagAndValue(370, newM);

                        break;

                    case "8":
                    case "9":
                        // Direction
                        md = MessageDirection.INBOUND;

                        // CompIds
                        if (MessageFunctions.ContainsTag(115, newM))
                        {
                            // If message is from a virtual-participant
                            string obo = MessageFunctions.GetTagValue(115, newM);
                            newM = MessageFunctions.SetTagValue(49, newM, obo);
                            newM = MessageFunctions.RemoveTagAndValue(115, newM);
                        }

                        newM = MessageFunctions.SetTagValue(56, newM, MessageFunctions.GetTagValue(128, newM));
                        newM = MessageFunctions.SetTagValue(128, newM, MessageFunctions.GetTagValue(49, newM));
                        newM = MessageFunctions.SetTagValue(49, newM, "LSEHub");
                        newM = MessageFunctions.SetTag(128, 115, newM);

                        // SubIds

                        if (MessageFunctions.ContainsTag(116, newM) && MessageFunctions.ContainsTag(50, newM))
                        {
                            // Ideally should not enter this scenario!  Prioritse by putting tag 50 val into tag 116 then removing 50
                            string obo = MessageFunctions.GetTagValue(116, newM);
                            string snd = MessageFunctions.GetTagValue(50, newM);

                            newM = MessageFunctions.SetTagValue(116, newM, snd);
                            newM = MessageFunctions.RemoveTagAndValue(50, newM);
                        }
                        else if (!MessageFunctions.ContainsTag(116, newM) && MessageFunctions.ContainsTag(50, newM))
                        {
                            newM = MessageFunctions.SetTag(50, 116, newM);
                        }
                        else if (MessageFunctions.ContainsTag(116, newM) && !MessageFunctions.ContainsTag(50, newM))
                        {
                            // Do Nothing!  Processing overhead but it's only setup!
                            // Tag 116 is where we want to be for message and direction.
                        }

                        newM = MessageFunctions.RemoveTagAndValue(57, newM);
                        newM = MessageFunctions.SetTag(129, 57, newM);
                        newM = MessageFunctions.RemoveTagAndValue(50, newM);
                        newM = MessageFunctions.SetTag(116, 50, newM);

                        // LocationIds
                        if (MessageFunctions.ContainsTag(144, newM))
                        {
                            // If message is from a virtual-participant
                            string obo = MessageFunctions.GetTagValue(144, newM);
                            newM = MessageFunctions.SetTagValue(1, newM, obo);
                            newM = MessageFunctions.RemoveTagAndValue(116, newM);
                        }
                        //newM = MessageFunctions.RemoveTagAndValue(142, newM);
                        //newM = MessageFunctions.SetTag(144, 142, newM);
                        //newM = MessageFunctions.RemoveTagAndValue(145, newM);
                        //newM = MessageFunctions.SetTag(143, 145, newM);

                        // Time/Date tags
                        //newM = MessageFunctions.RemoveTagAndValue(370, newM);

                        break;
                }

                //Console.WriteLine();
                //Console.WriteLine(newM);

                RawMessage rm = new RawMessage(newM, md);
                _convertedMessages.Add(rm);

            }


        }

        private void ConvertBUYtoSELL()
        {

        }

        public List<RawMessage> GetConvertedMessages()
        {
            return _convertedMessages;
        }
    }
}
