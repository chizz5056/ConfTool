using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSEHub.ConfTool
{
    public sealed class TestManager
    {
        private static readonly Lazy<TestManager> lazy = new Lazy<TestManager>(() => new TestManager());

        public static TestManager Instance { get { return lazy.Value; } }

        private TestManager()
        {
            IsCancelled = false;
        }

        public bool IsCancelled { get; set; }
    }
}
