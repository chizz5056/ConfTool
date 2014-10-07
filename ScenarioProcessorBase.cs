using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSEHub.ConfTool
{
    public class ScenarioProcessorBase
    {
        public List<MessageTaskBase> _processingTasks;

        public ScenarioProcessorBase()
        {
            _processingTasks = new List<MessageTaskBase>();
        }

        public void AddProcessingTask(MessageTaskBase mtb)
        {
            _processingTasks.Add(mtb);
        }
    }
}
