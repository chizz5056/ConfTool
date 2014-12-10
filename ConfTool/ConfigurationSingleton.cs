using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;

using QuickFix;

namespace LSEHub.ConfTool
{
    public sealed class ConfigurationSingleton
    {
        private static readonly Lazy<ConfigurationSingleton> lazy = new Lazy<ConfigurationSingleton>(() => new ConfigurationSingleton());

        public static ConfigurationSingleton Instance { get { return lazy.Value; } }

        private int _ID;

        private ConfigurationSingleton()
        {
            qfnSettings = new SessionSettings();
            xDoc = XDocument.Load("Scenarios.xml");
            
            string idFile = "ID.xml";

            if (File.Exists(idFile))
            {
                idDoc = XDocument.Load(idFile);
            }
            else
            {
                idDoc = new XDocument(new XElement("ID",100));
                idDoc.Save(idFile);
            }


            _ID = int.Parse(idDoc.Element("ID").Value);
            ResetIDInUse();
        }

        public SessionSettings QFNSettings
        {
            get { return qfnSettings; }
        }

        public string GetSetting(string val)
        {
            //return qfnSettings.Get(). .Get().GetString("val");
            //HashSet<SessionID> hs = qfnSettings.GetSessions();
            //List<SessionID> ls = hs.ToList();
            //SessionID sid = ls[0];
            return qfnSettings.Get(sessionID).GetString(val);
        }
        
        private SessionSettings qfnSettings;
        private SessionID sessionID;
        XDocument xDoc;
        XDocument idDoc;
        

        public void AddQFNSettings(QuickFix.Dictionary dic)
        {
            qfnSettings.Set(dic);
        }
            

        public void AddQFNSettings(SessionID sid, QuickFix.Dictionary dic)
        {
            qfnSettings.Set(sid, dic);
            sessionID = sid;
        }

        public void RemovaAllQFNSettings()
        {
            qfnSettings = new SessionSettings();
        }

        private List<int> tagsToIgnore;
        public List<int> TagsToIgnore()
        {
            return tagsToIgnore;
        }

        public void SetTagsToIgnore(string csvTags)
        {
            tagsToIgnore = csvTags.Split(',').Select(n => int.Parse(n)).ToList();
        }

        public string NextClOrdID
        {
            get
            {
                PrevClOrdID = "LSE" + (_ID-1).ToString().PadLeft(5, '0');
                CurrentClOrdID = "LSE" + _ID.ToString().PadLeft(5, '0');
                _ID++;
                SaveIDtoXml();
                return CurrentClOrdID;
            }
        }

        public string CurrentClOrdID { get; private set; }

        public string PrevClOrdID { get; private set; }

        private void SaveIDtoXml()
        {
            XDocument xdoc = XDocument.Load("ID.xml");
            var element = xdoc.Element("ID");
            element.Value = _ID.ToString();
            xdoc.Save("ID.xml");
        }




        public string GetUtcDate()
        {
            return DateTime.UtcNow.ToString("yyyyMMdd");
        }

        public string GetUtcTimestamp()
        {
            return DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss");
        }


        public void ResetIDInUse()
        {
            _Ids = new List<string>();
        }

        private List<string> _Ids;
        
        public List<string> GetIDInUseList()
        {
            return _Ids;
        }

        public void AddIDInUse(string id)
        {
            _Ids.Add(id);
        }


    }
}
