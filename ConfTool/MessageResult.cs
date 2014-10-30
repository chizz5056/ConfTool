using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FixSchema;

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

        public List<TagValue> GetMatched1()
        {
            return matchedList1;
        }

        public List<TagValue> GetMissingTags()
        {
            return missingTags;
        }

        public List<TagValue> GetExtraTags()
        {
            return extraTags;
        }

        public List<TagValue> GetIncorrectValueTags()
        {
            return incorrectValueTags;
        }

        List<TagValue> expectedList;
        List<TagValue> actualList;

        Dictionary<int, string> expectedDictionary;
        Dictionary<int, string> actualDictionary;

        List<TagValue> matchedList;
        List<TagValue> missingList;
        List<TagValue> extraList;

        List<TagValue> matchedList1;
        List<TagValue> missingTags;
        List<TagValue> extraTags;
        List<TagValue> incorrectValueTags;

        public MessageResult(string expected, string actual)
        {
            Expected = expected;
            Actual = actual;
            expectedList = MessageFunctions.GetTagValueList(expected);
            actualList = MessageFunctions.GetTagValueList(actual);
            expectedDictionary = MessageFunctions.GetTagValueDictionary(expected);
            actualDictionary = MessageFunctions.GetTagValueDictionary(actual);
            CompareMessages();
        }

        private void CompareMessages()
        {
            try
            {
                matchedList = new List<TagValue>();
                missingList = new List<TagValue>();
                extraList = new List<TagValue>();

                matchedList1 = new List<TagValue>();
                missingTags = new List<TagValue>();
                extraTags = new List<TagValue>();
                incorrectValueTags = new List<TagValue>();

                List<int> ignore = ConfigurationSingleton.Instance.TagsToIgnore();

                // Basic Matching - every value is a string!
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

                // Complex Matching - converted to relevant Val type first i.e. compare 0.0000 to 0.000000
                foreach (KeyValuePair<int, string> kvp in expectedDictionary)
                {
                    if (!ignore.Contains(kvp.Key))
                    {
                        if (actualDictionary.ContainsKey(kvp.Key))
                        {
                            Field f = Fields.Instance.GetField(kvp.Key);
                            if (FixDataTypes.Compare(f, kvp.Value, actualDictionary[kvp.Key].ToString()))
                            {
                                TagValue tv = new TagValue(kvp.Key, kvp.Value);
                                matchedList1.Add(tv);
                            }
                            else
                            {
                                TagValue tv = new TagValue(kvp.Key, actualDictionary[kvp.Key]);
                                incorrectValueTags.Add(tv);
                            }
                        }
                        else
                        {
                            TagValue tv = new TagValue(kvp.Key, kvp.Value);
                            missingTags.Add(tv);
                        }


                    }
                }

                foreach (KeyValuePair<int,string> kvp in actualDictionary)
                {
                    if (!ignore.Contains(kvp.Key))
                    {
                        if (!expectedDictionary.ContainsKey(kvp.Key))
                        {
                            extraTags.Add(new TagValue(kvp.Key, kvp.Value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
                    
        }
    }
}
