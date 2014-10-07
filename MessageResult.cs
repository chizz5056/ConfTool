using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSEHub.ConfTool
{
    public class MessageResult
    {
        string expected;
        string actual;

        List<TagValue> expectedList;
        List<TagValue> actualList;

        List<TagValue> matchedList;
        List<TagValue> missingList;
        List<TagValue> extraList;

        public MessageResult(string Expected, string Actual)
        {
            expected = Expected;
            actual = Actual;
            expectedList = MessageFunctions.GetTagValueList(expected);
            actualList = MessageFunctions.GetTagValueList(actual);
            CompareMessages();
        }

        private void CompareMessages()
        {
            matchedList = new List<TagValue>();
            missingList = new List<TagValue>();
            extraList = new List<TagValue>();

            foreach (TagValue tv in expectedList)
            {
                if (actualList.Contains(tv))
                {
                    matchedList.Add(tv);
                }
                else if(!actualList.Contains(tv))
                {
                    missingList.Add(tv);
                }
            }

            foreach (TagValue tv in actualList)
            { 
                if(!expectedList.Contains(tv))
                {
                    extraList.Add(tv);
                }
            }


        }


    }
}
