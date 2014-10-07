using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LSEHub.ConfTool
{
    public static class MessageFunctions
    {
        private const string SOH = "|";
        private const string startPattern = "(?<=(^|\\|)";
        private const string endPattern = "=)(.+?)(?=\\|{1})";
        private const string valPattern = "(?<=(^|\\|)[tag]=)(.+?)(?=\\|{1})";
        private const string valPatternIncSOH = "(?<=(^|\\|))[tag]=.+?\\|{1}";
        private const string tagPattern = "(?<=(^|\\|))[tag](?==.+?\\|{1})";
        private const string tagValPattern = "(?<=(^|\\|))[tag]=.+?(?=\\|{1})";
        private const string tagValPatternIncSOH = "(?<=(^|\\|))[tag]=.+?\\|{1}";
        private const string tagHolder = "[tag]";

        /// <summary>
        /// Updates the specific tag value with the supplied value
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="newVal"></param>
        /// <returns></returns>
        public static string SetTagValue(int tag, string message, string newVal)
        {
            string pattern = valPattern.Replace(tagHolder, tag.ToString());
            Regex rgx = new Regex(pattern);
            return rgx.Replace(message, newVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetTagValue(int tag, string message)
        {
            string pattern = valPattern.Replace(tagHolder, tag.ToString());
            Regex rgx = new Regex(pattern);
            return rgx.Match(message).Value;
        }   
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldTag"></param>
        /// <param name="newTag"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string SetTag(int oldTag, int newTag, string message)
        {
            string pattern = tagPattern.Replace(tagHolder, oldTag.ToString());
            Regex rgx = new Regex(pattern);
            return rgx.Replace(message, newTag.ToString());
        }

        /// <summary>
        /// Removes the tag and value from the messages
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string RemoveTagAndValue(int tag, string message)
        {
            string pattern = tagValPatternIncSOH.Replace(tagHolder, tag.ToString());
            Regex rgx = new Regex(pattern);
            return rgx.Replace(message, "");
        }

        /// <summary>
        /// Replaces the src tag with dst tag, leaving src value the same but assigned to new dst tag
        /// </summary>
        /// <param name="srcTag"></param>
        /// <param name="dstTag"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        //public static string ReplaceTag(int srcTag, int dstTag, string message, bool removeDst = true, TagLocation loc = TagLocation.DESTINATION)
        //{
        //    if (ContainsTag(srcTag, message)) // && ContainsTag(dstTag, message))
        //    {
        //        string srcVal = GetTagValue(srcTag, message);
        //        string pattern = tagPattern.Replace(tagHolder, srcTag.ToString());
        //        Regex rgx = new Regex(pattern);

        //        switch (loc)
        //        {
        //            case TagLocation.SOURCE:
        //                if (removeDst)
        //                {
        //                    message = RemoveTagAndValue(dstTag, message);
        //                }
        //                return rgx.Replace(message, dstTag.ToString());

        //            case TagLocation.DESTINATION:
        //                message = RemoveTagAndValue(srcTag, message);
        //                message = SetTagValue(dstTag, message, srcVal);
        //                return message;
        //        }
        //    }
            
        //    return message;        
        //}

        /// <summary>
        /// Checks message to see if it contains specified tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ContainsTag(int tag, string message)
        {
            string pattern = tagPattern.Replace(tagHolder, tag.ToString());
            Regex rgx = new Regex(pattern);
            return rgx.Match(message).Success;
        }

        public static List<TagValue> GetTagValueList(string message)
        {
            try
            {
                string m = message.Replace("\x01","|");
                List<TagValue> l = new List<TagValue>();

                while (m.Length > 0)
                {
                    int delim = m.IndexOf("|");
                    string both = m.Substring(0, delim);
                    string[] tagVal = both.Split('=');
                    TagValue tv = new TagValue(int.Parse(tagVal[0]), tagVal[1]);
                    l.Add(tv);
                    m = m.Substring(delim + 1);
                }

                return l;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        } 


    }
}
