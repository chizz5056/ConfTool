using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.IO;

using QuickFix;

namespace LSEHub.ConfTool
{
    public class ConformanceManager
    {
        private XmlNode xScenario;
        private List<string> messages;
        private Queue<QuickFix.Message> inQ;
        private Queue<RawMessage> outQ;
        QFNApp qfnapp;
        public event Action EndOfScenarioEvent;
        private List<MessageResult> results;

        public ConformanceManager()
        {
            qfnapp = new QFNApp(ConfigurationSingleton.Instance.QFNSettings);
            qfnapp.MessageEvent += new Action<QuickFix.Message, bool>(ProcessMessage);
            qfnapp.Start();
        }

        public void ReInitialise(XmlNode xNode)
        {
            xScenario = xNode;
            messages = new List<string>();
            inQ = new Queue<Message>();
            outQ = new Queue<RawMessage>();
            results = new List<MessageResult>();

            const string format = "{0,-30} {1,0}";
            Console.WriteLine(string.Format(format,"Initialising conformance test:", xScenario.Attributes["Name"].Value));
            Console.WriteLine(string.Format(format, "Side:", xScenario.Attributes["Side"].Value));
            Console.WriteLine(string.Format(format, "Type:", xScenario.Attributes["Type"].Value));
            
            //Console.WriteLine(string.Format(format,"Sender CompID:", xScenario.Attributes["SenderCompID"].Value));
            //Console.WriteLine(string.Format(format, "Counterparty CompID:", xScenario.Attributes["DeliverToCompID"].Value));
            //Console.WriteLine(string.Format(format, "Ignore session messages:", xScenario.Attributes["IgnoreSessionMessages"].Value));
            //Console.WriteLine(string.Format(format, "Ignore unexpected messages:", xScenario.Attributes["IgnoreUnexpectedMessages"].Value));

            Console.WriteLine(string.Format(format, "SenderCompID:", ConfigurationSingleton.Instance.GetSetting("SenderCompID")));
            Console.WriteLine(string.Format(format, "TargetCompID:", ConfigurationSingleton.Instance.GetSetting("TargetCompID")));
            Console.WriteLine(string.Format(format, "DeliverToCompID:", ConfigurationSingleton.Instance.GetSetting("DeliverToCompID")));
            //Console.WriteLine(string.Format(format, "Ignore session messages:", xScenario.Attributes["IgnoreSessionMessages"].Value));
            //Console.WriteLine(string.Format(format, "Ignore unexpected messages:", xScenario.Attributes["IgnoreUnexpectedMessages"].Value));

            Console.WriteLine();
            
            

            
            switch ((ScenarioType)Enum.Parse(typeof(ScenarioType), xScenario.Attributes["Type"].Value))
            {
                case ScenarioType.Explicit:
                    break;
                case ScenarioType.Raw:
                    foreach (XmlElement message in xScenario.ChildNodes)
                    {
                        messages.Add(message.InnerText);
                    }
                    RawSideConverter rsc = new RawSideConverter((Side)Enum.Parse(typeof(Side), xScenario.Attributes["Side"].Value.ToUpper()), messages);
                    rsc.Convert();                                  
                    

                    foreach (RawMessage raw in rsc.GetConvertedMessages())
                    {
                        outQ.Enqueue(raw);
                    }

                    while (!qfnapp.IsConnected)
                    {

                    }

                    if (qfnapp.IsConnected && outQ.Peek().Direction == MessageDirection.OUTBOUND)
                    {
                        // Start test by sending first message
                        RawMessage rm = outQ.Dequeue();
                        qfnapp.Send(rm);
                    }
                    break;
            }


            
        }

        public void ProcessMessage(QuickFix.Message msg, bool isIncoming)
        {
            Debug.WriteLine("Q:{0} Incoming:{1} - {2}", outQ.Count.ToString(), isIncoming.ToString(), msg.ToString());

            //Console.WriteLine("Message received: {0}", msg.ToString());
            if (isIncoming)
            {
                if (outQ.Count > 0 && outQ.Peek().Direction == MessageDirection.INBOUND)
                {
                    RawMessage rm = outQ.Dequeue();
                    Debug.WriteLine("Q:{0} Incoming:{1} - {2}", outQ.Count.ToString(), isIncoming.ToString(), msg.ToString());
                    //Console.WriteLine();
                    //Console.WriteLine("Received: {0}", msg.ToString());
                    //Console.WriteLine("Expected: {0}", rm.Message);

                    MessageResult mr = new MessageResult(rm.Message, msg.ToString());
                    ProcessResult(mr);
                    results.Add(mr);

                }

                if (outQ.Count == 0)
                {
                    ProcessResults();
                }
            }

            if (outQ.Count > 0 && outQ.Peek().Direction == MessageDirection.OUTBOUND)
            {
                System.Threading.Thread.Sleep(100);
                RawMessage rm = outQ.Dequeue();
                if (qfnapp.IsConnected)
                    qfnapp.Send(rm);

                if (outQ.Count == 0)
                {
                    ProcessResults();
                }
            }


        }

        public void ProcessResult(MessageResult mr)
        {
            //Console.WriteLine("Message Result {0}", i.ToString());
            
            
            Console.WriteLine("Expected: {0}", mr.Expected);
            
            
            Console.WriteLine("Actual: {0}", mr.Actual);
            
            string matched = "";
            foreach (TagValue tv in mr.GetMatched())
            {
                matched += (tv.GetTagVal() + ",");
            }
            matched = matched.Remove(matched.Length - 1);
            Console.WriteLine("\tMatched: {0}", matched);
            
            string missing = "";
            foreach (TagValue tv in mr.GetMissing())
            {
                missing += (tv.GetTagVal() + ",");
            }
            missing = missing.Remove(missing.Length - 1);
            Console.WriteLine("\tMissing: {0}", missing);
            

            string extras = "";
            foreach (TagValue tv in mr.GetExtra())
            {
                extras += (tv.GetTagVal() + ",");
            }
            extras = extras.Remove(extras.Length - 1);
            Console.WriteLine("\tExtras: {0}", extras);
            Console.WriteLine();
        }

        public void ProcessResults()
        {
            Console.WriteLine();
            Console.WriteLine("End of Test");
            Console.WriteLine("Press any key to process results");
            Console.ReadKey();

            using (StreamWriter sw = new StreamWriter(xScenario.Attributes["Name"].Value + "_RESULTS.txt"))
            {

                int i = 1;
                foreach (MessageResult mr in results)
                {
                    //Console.WriteLine("Message Result {0}", i.ToString());
                    sw.WriteLine("Message Result {0}", i.ToString());
                    sw.WriteLine();
                    //Console.WriteLine("Expected: {0}", mr.Expected);
                    sw.WriteLine("Expected: {0}", mr.Expected);
                    sw.WriteLine();
                    //Console.WriteLine("Actual: {0}", mr.Actual);
                    sw.WriteLine("Actual: {0}", mr.Actual.Replace("\x01","|"));
                    sw.WriteLine();

                    string matched = "";
                    foreach (TagValue tv in mr.GetMatched())
                    {
                        matched += (tv.GetTagVal() + ",");
                    }
                    matched = matched.Remove(matched.Length - 1);
                    //Console.WriteLine("\tMatched: {0}", matched);
                    sw.WriteLine("\tMatched: {0}", matched);
                    sw.WriteLine();

                    string missing = "";
                    foreach (TagValue tv in mr.GetMissing())
                    {
                        missing += (tv.GetTagVal() + ",");
                    }
                    missing = missing.Remove(missing.Length - 1);
                    //Console.WriteLine("\tMissing: {0}", missing);
                    sw.WriteLine("\tMissing: {0}", missing);
                    sw.WriteLine();

                    string extras = "";
                    foreach (TagValue tv in mr.GetExtra())
                    {
                        extras += (tv.GetTagVal() + ",");
                    }
                    extras = extras.Remove(extras.Length - 1);
                    //Console.WriteLine("\tExtras: {0}", extras);
                    sw.WriteLine("\tExtras: {0}", extras);
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine();
                    //Console.WriteLine();
                    i++;
                }

            }

            Console.ReadKey();

            if (EndOfScenarioEvent != null)
                EndOfScenarioEvent();
        }
    }
}

