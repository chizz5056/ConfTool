using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickFix;

namespace LSEHub.ConfTool
{
    public sealed class ConfigurationSingleton
    {
        private static readonly Lazy<ConfigurationSingleton> lazy = new Lazy<ConfigurationSingleton>(() => new ConfigurationSingleton());

        public static ConfigurationSingleton Instance { get { return lazy.Value; } }

        private ConfigurationSingleton()
        {
            qfnSettings = new SessionSettings();        
        }

        public SessionSettings QFNSettings
        {
            get { return qfnSettings; }
        }
        
        private SessionSettings qfnSettings;

        public void AddQFNSettings(QuickFix.Dictionary dic)
        {
            qfnSettings.Set(dic);
        }
            

        public void AddQFNSettings(SessionID sid, QuickFix.Dictionary dic)
        {
            qfnSettings.Set(sid, dic);
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


    }
}
