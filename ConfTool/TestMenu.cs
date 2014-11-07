using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using System.Threading;


using QuickFix;


namespace LSEHub.ConfTool
{
    public class TestMenu
    {
        ConformanceManager cm;
        XmlDocument xmlDoc;
        //QuickFix.Dictionary defaults;
        QuickFix.Dictionary session;
        List<string> sc;
        List<XmlNode> scNodes;

        public TestMenu()
        {
            LoadXml();
            cm = new ConformanceManager();
            cm.EndOfScenarioEvent += new Action(StartMenu);
            StartMenu();
        }

        private void StartMenu()
        {
            LoadXml();
            cm.ReInitialise(scNodes[GetMenuOption() - 1]);
        }

        private int GetMenuOption()
        {
            int choice = 0;
            while (choice < 1 || choice > sc.Count)
            {
                Console.Clear();
                Console.WriteLine("Please select scenario:");
                int i = 1;
                foreach (string s in sc)
                {
                    Console.WriteLine("\t[{0}] {1}", i.ToString(), s);
                    i++;
                }

                try
                {
                    choice = int.Parse(Console.ReadLine());
                }
                catch
                {

                }
            }
            return choice;
        }

        private void LoadXml()
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load("Scenarios.xml");

            XDocument xDoc = XDocument.Load("Scenarios.xml");

            // QFN Default Session Settings
            //defaults = new Dictionary();
            //IEnumerable<XElement> xDefault = xDoc.Root.Element("QFNSessionSettings").Element("Default").Descendants();
            //foreach (XElement xe in xDefault)
            //{
            //    defaults.SetString(xe.Name.ToString(), xe.Value);
            //}

            // QFN Specific Session Settings
            session = new Dictionary();
            IEnumerable<XElement> xSession = xDoc.Root.Element("QFNSessionSettings").Element("Session").Descendants();
            foreach (XElement xe in xSession)
            {
                session.SetString(xe.Name.ToString(), xe.Value);
            }

            SessionID sessionID = new SessionID(session.GetString("BeginString"), session.GetString("SenderCompID"), session.GetString("TargetCompID"));

            ConfigurationSingleton.Instance.RemovaAllQFNSettings();
            //ConfigurationSingleton.Instance.AddQFNSettings(defaults);
            ConfigurationSingleton.Instance.AddQFNSettings(sessionID, session);
            ConfigurationSingleton.Instance.SetTagsToIgnore(xDoc.Root.Element("TagsToIgnore").Value);

            sc = new List<string>();
            scNodes = new List<XmlNode>();
            foreach (XmlNode xNode in xmlDoc.GetElementsByTagName("Scenario"))
            {
                sc.Add(xNode.Attributes["Name"].Value);
                scNodes.Add(xNode);
            }

            
        }
    }
}
