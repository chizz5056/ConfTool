using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSEHub.ConfTool
{
    public class MessageResult
    {
        public string Expected { get; set; }
        public string Actual { get; set; }
        
        public List<TagValue> GetMatched()
        {
            return matchedList;
        }

        public List<TagValue> GetMissing()
        {
            return missingList;
        }

        public List<TagValue> GetExtra()
        {
            return extraList;
        }

        List<TagValue> expectedList;
        List<TagValue> actualList;

        List<TagValue> matchedList;
        List<TagValue> missingList;
        List<TagValue> extraList;

        public MessageResult(string expected, string actual)
        {
            Expected = expected;
            Actual = actual;
            expectedList = MessageFunctions.GetTagValueList(expected);
            actualList = MessageFunctions.GetTagValueList(actual);
            CompareMessages();
        }

        private void CompareMessages()
        {
            matchedList = new List<TagValue>();
            missingList = new List<TagValue>();
            extraList = new List<TagValue>();

            List<int> ignore = ConfigurationSingleton.Instance.TagsToIgnore();

            foreach (TagValue tv in expectedList)
            {
                if (!ignore.Contains(tv.Tag))
                {
                    if (actualList.Contains(tv))
                    {
                        matchedList.Add(tv);
                    }
                    else if (!actualList.Contains(tv))
                    {
                        missingList.Add(tv);
                    }
                }
            }

            foreach (TagValue tv in actualList)
            {
                if (!ignore.Contains(tv.Tag))
                {
                    if (!expectedList.Contains(tv))
                    {
                        extraList.Add(tv);
                    }
                }
            }
        }
    }
}
