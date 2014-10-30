using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

using FixSchema;
using FixOrderBookRebuilder.Chain;

namespace FixOrderBookRebuilder
{
    class Program
    {
        const char SOH = '\x01';
        //const string PATTERN = "8=FIX.4.2" + SOH + "9=[0-9]+" + SOH + ".*" + SOH + "10=\\d{3}" + SOH;
        const string PATTERN = "8=FIX.4.2\u00019=[0-9]+\u0001.*\u000110=\\d{3}\u0001";
       

        static void Main(string[] args)
        {
            Schema schema = new Schema();
            Dictionary<string, string> fixmessages = ParseLogFiles();
            Dictionary<string, Order> orders = new Dictionary<string, Order>();

            // Get NewOrderSingles first removing 35=D messages from "fixmessages" List in the process
            //foreach (string fm in fixmessages)
            for(int i = fixmessages.Count - 1; i>0; i--)
            {
                if (fixmessages.ElementAt(i).Value.Contains(SOH+"35=D"+SOH))
                {
                    Order ord = new Order();
                    string clordid = GetTagValue(11, fixmessages.ElementAt(i).Value);
                    string date = GetTagValue(52, fixmessages.ElementAt(i).Value).Substring(0, 8);
                    ord.AddClOrdID("[" + date + "]" + clordid);
                    ord.AddMessages(fixmessages.ElementAt(i).Value);
                    try
                    {
                        orders.Add("[" + date + "]" + clordid, ord);
                        //orders.Add(clordid, ord);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString() + System.Environment.NewLine + fixmessages.ElementAt(i).Value);
                        //orders.Add(clordid + "[" + date + "]", ord);
                    }


                    //fixmessages.RemoveAt(i);
                    fixmessages.Remove(fixmessages.ElementAt(i).Key);
                }

                                
            }
            
            int y = 0;
            // Now remaining messages
            foreach (KeyValuePair<string, string> fm in fixmessages)
            {
                string msgtype = GetTagValue(35, fm.Value);
                string clordid = GetTagValue(11, fm.Value);
                string origclordid = GetTagValue(41, fm.Value);
                string orderid = GetTagValue(37, fm.Value);
                
                string date = GetTagValue(52, fm.Value).Substring(0, 8);
                string clordidkey = "[" + date + "]" + clordid;
                string origclordidkey = "[" + date + "]" + origclordid;

                // Debugging breakpoint catches
                //if (GetTagValue(17, fm.Value) == "053008877")
                //{
                //    Console.WriteLine("17=053008877");
                //}

                //if (GetTagValue(11, fm.Value) == "yAA0054")
                //{
                //    Console.WriteLine("11=yAA0054");
                //}

                

                if (origclordid == null)
                {
                    // Unchained
                    if (orders.ContainsKey(clordidkey))
                    {
                        ((Order)orders[clordidkey]).AddOrderID(orderid);
                        ((Order)orders[clordidkey]).AddMessages(fm.Value);
                    }
                    else
                    {
                        //Console.WriteLine("Deeper linking");
                        foreach (KeyValuePair<string, Order> kvp in orders)
                        {
                            if (((Order)kvp.Value).ContainsClOrdID(clordidkey))
                            {
                                ((Order)kvp.Value).AddClOrdID(clordidkey);
                                //((Order)kvp.Value).AddOrderID(orderid);
                                ((Order)kvp.Value).AddMessages(fm.Value);
                            }
                        }

                    }
                }
                else if (origclordid != null)
                {
                    // Chained

                    // First link
                    if (orders.ContainsKey(origclordidkey))
                    {
                        ((Order)orders[origclordidkey]).AddClOrdID(clordidkey);
                        //((Order)orders[origclordidkey]).AddOrderID(orderid);
                        ((Order)orders[origclordidkey]).AddMessages(fm.Value);
                    }
                    else
                    {
                        //Console.WriteLine("Deeper linking");
                        foreach (KeyValuePair<string,Order> kvp in orders)
                        {
                            if (((Order)kvp.Value).ContainsClOrdID(origclordidkey))
                            {
                                ((Order)kvp.Value).AddClOrdID(clordidkey);
                                //((Order)kvp.Value).AddOrderID(orderid);
                                ((Order)kvp.Value).AddMessages(fm.Value);
                            }
                        }
            
                    }

                }

                y++;

                #region switch
                /*
                switch (msgtype)
                {
                    case "8":
                        if (origclordid == null)
                        {
                            // Unchained
                            if (orders.ContainsKey(key))
                            {
                                ((Order)orders[key]).AddOrderID(orderid);
                                ((Order)orders[key]).AddMessages(fm);
                            }
                        }
                        else
                        {
                            // Chained

                            // First link
                            if (orders.ContainsKey(chainedkey))
                            {
                                ((Order)orders[chainedkey]).AddOrderID(orderid);
                                ((Order)orders[chainedkey]).AddMessages(fm);
                            }
                            else
                            {
                                Console.WriteLine("Deeper linking");
                            }

                        }
                        break;
                    case "G":
                        //if (origclordid == null)
                        //{
                        //    // Unchained
                        //    if (orders.ContainsKey(key))
                        //    {
                        //        ((Order)orders[key]).AddOrderID(orderid);
                        //        ((Order)orders[key]).AddMessages(fm);
                        //    }
                        //}
                        //else
                        //{
                        //    // Chained

                        //    // First link
                        //    if (orders.ContainsKey(chainedkey))
                        //    {
                        //        ((Order)orders[chainedkey]).AddOrderID(orderid);
                        //        ((Order)orders[chainedkey]).AddMessages(fm);
                        //    }
                        //    else
                        //    {
                        //        Console.WriteLine("Deeper linking");
                        //    }

                        //}
                        break;
                    case "F":
                        break;
                    case "D":
                        break;
                    case "9":
                        break;
                    default:
                        Console.WriteLine("Unknown message type: " + msgtype);
                        break;
                }
                */
                #endregion
            }
            
                
            var sortedorders = from pair in orders orderby pair.Key descending select pair;

            // RAW Output
            StreamWriter sw = new StreamWriter("output.txt");
            foreach (KeyValuePair<string, Order> kvp in sortedorders)
            {
                if (((Order)kvp.Value).MessageCount >= 0)
                {
                    sw.WriteLine("First ClOrdID in chain: " + kvp.Key);
                    sw.WriteLine(((Order)kvp.Value).GetAllMessages());
                }
            }
            sw.Close();

            // Converted output
            StreamWriter sw2 = new StreamWriter("output2.txt");
            foreach (KeyValuePair<string, Order> kvp in sortedorders)
            {
                if (((Order)kvp.Value).MessageCount >= 3)
                {
                    sw2.WriteLine("First ClOrdID in chain: " + kvp.Key);
                    foreach (string s in ((Order)kvp.Value).AllMessages())
                    {
                        string t = s;
                        string u = string.Empty;

                        while (t.Length > 0)
                        {
                            int a = t.IndexOf("=");
                            int b = t.IndexOf(SOH);

                            int tag = int.Parse(t.Substring(0, a));
                            string val = t.Substring(a + 1, (b - a - 1));

                            t = t.Substring(b + 1);

                            Field f = Fields.Instance.GetField(tag);

                            u += f.Name + "=";

                            if (f.ContainsFieldValue(val))
                            {
                                val = f.GetFieldValue(val);
                            }

                            u += (val + "|");
                                
                        }

                        sw2.WriteLine(u);
                    }
                    sw2.WriteLine();
                }
            }
            sw2.Close();

            // Pseudo converted output
            #region Pseudo converted output
            StreamWriter sw3 = new StreamWriter("pseudo.txt");
            OrderChains ocs = new OrderChains();
            foreach (KeyValuePair<string, Order> kvp in sortedorders)
            {
                OrderChain oc = new OrderChain();
                Order ord = (Order)kvp.Value;

                if (ord.MessageCount >= 0)
                {
                    oc.Initialise(ord.GetFirstClOrdID);
                    sw3.WriteLine("First ClOrdID in chain: " + kvp.Key);
                    
                    foreach (string s in ord.AllMessages())
                    {
                        string t = s;
                        string u = string.Empty;
                        string msgtype = string.Empty;
                        string ordtype = string.Empty;
                        string ordstatus = string.Empty;
                        string exectype = string.Empty;
                        OrderChainMessage ocm = new OrderChainMessage();
                        

                        while (t.Length > 0)
                        {
                            int a = t.IndexOf("=");
                            int b = t.IndexOf(SOH);

                            int tag = int.Parse(t.Substring(0, a));
                            string val = t.Substring(a + 1, (b - a - 1));

                            t = t.Substring(b + 1);



                            if (tag == 35 || tag == 40 || tag == 39 || tag == 150)
                            {
                                Field f = Fields.Instance.GetField(tag);

                                //u += f.Name + "=";

                                if (f.ContainsFieldValue(val))
                                {
                                    val = f.GetFieldValue(val);
                                }

                                switch (tag)
                                {
                                    case 35:
                                        msgtype = f.Name + "=" + val + "|";
                                        ocm.MsgType = val;
                                        break;
                                    case 40:
                                        ordtype = f.Name + "=" + val + "|";
                                        ocm.OrdType = val;
                                        break;
                                    case 39:
                                        ordstatus = f.Name + "=" + val + "|";
                                        ocm.OrdStatus = val;
                                        break;
                                    case 150:
                                        exectype = f.Name + "=" + val + "|";
                                        ocm.ExecType = val;
                                        break;
                                }


                            }

                        }

                        if (msgtype != string.Empty)
                            u += msgtype;
                        if (ordtype != string.Empty)
                            u += ordtype;
                        if (ordstatus != string.Empty)
                            u += ordstatus;
                        if (exectype != string.Empty)
                            u += exectype;

                        oc.AddOCM(ocm);
                        sw3.WriteLine(u);
                    }
                    sw3.WriteLine();
                    
                }
                ocs.AddOC(oc);
            }
            sw3.Close();
            #endregion

            // distinct pseudo converted output
            #region distinct pseudo converted output
            StreamWriter sw4 = new StreamWriter("distinct_pseudo.txt");
            foreach (KeyValuePair<string,string> kvp in ocs.ocDict)
            {
                sw4.WriteLine("First ClOrdID in chain: " + kvp.Key);
                string s = kvp.Value;
                while (s.Length > 0)
                {
                    int t = s.IndexOf(SOH);
                    sw4.WriteLine(s.Substring(0, t));
                    s = s.Substring(t+1);
                }
                sw4.WriteLine();
            }
            sw4.Close();
            #endregion

            //Console.ReadKey();
            
        }

        static string GetTagValue(int tag, string message)
        {
            if (message.Contains(SOH + tag.ToString() + "="))
            {
                // Get value
                int start = message.IndexOf(SOH + tag.ToString() + "=");
                start = start + 2 + tag.ToString().Length;  // Remove tag and "="
                int end = message.IndexOf(SOH, start);
                return message.Substring(start, end - start);

            }
            else
                return null;
        }

        static Dictionary<string,string> ParseLogFiles()
        {
            Dictionary<string, string> messages = new Dictionary<string, string>();

            string[] files = Directory.GetFiles(@"..\..\BLKUKNITE\","*.log");

            foreach (string s in files)
            {
                string line;
                StreamReader sr = new StreamReader(s);
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("CTRL"))
                    {
                        // Skip - Control
                    }
                    else if (line.Contains(SOH + "35=A" + SOH))
                    {
                        // Skip - Logon
                    }
                    else if (line.Contains(SOH + "35=0" + SOH))
                    {
                        // Skip - Heartbeat
                    }
                    else if (line.Contains(SOH + "35=BD" + SOH))
                    {
                        // Skip - NSR
                    }
                    else if (line.Contains(SOH + "35=5" + SOH))
                    {
                        // Skip - Logout
                    }
                    else if (line.Contains(SOH + "35=1" + SOH))
                    {
                        // Skip - Test Request
                    }
                    else if (line.Contains(SOH + "35=2" + SOH))
                    {
                        // Skip - Logout
                    }
                    else if (line.Contains(SOH + "35=4" + SOH))
                    {
                        // Skip - Test Request
                    }
                    else
                    {
                        // Just look at the NITE end - so mostly successful messages!
                        if (line.Contains(SOH + "56=NITE" + SOH) || line.Contains(SOH + "49=NITE" + SOH))
                        {
                            if (line.Contains(SOH + "115=BLKUKNITE" + SOH) || line.Contains(SOH + "128=BLKUKNITE" + SOH))
                            {
                                if (Regex.IsMatch(line, PATTERN))
                                {
                                    string key = GetTagValue(52, line).Substring(0, 8) + GetTagValue(52, line).Substring(9, 2) + GetTagValue(52, line).Substring(12, 2) + GetTagValue(52, line).Substring(15, 2) + GetTagValue(34,line);
                                    string message = Regex.Match(line, PATTERN).ToString();
                                    //Console.WriteLine(message);
                                    messages.Add(key, message);
                                }
                            }
                        }
                    }
                }
            }

            //var l = messages.OrderBy(key => key.Key);
            //var dic = l.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

            //return dic;

            return messages;
        }

    }
}
