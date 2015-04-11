using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class StoreItemValueArgs
    {
        public StoreItem Item { get; set; }
        public string PropName { get; set; }
        public string PublicPropName { get; set; }
    }

    public class StoreItemValueManager
    {
        Queue<StoreItemValueArgs> ValueQueue;

        public StoreItemValueManager()
        {
            ValueQueue = new Queue<StoreItemValueArgs>();
        }

        public string GetBackgroundStringValue(StoreItem item, string propName, string publicPropName)
        {
            if (Globals.ApplicationManager.IsStartupComplete)
            {
                BackgroundTask storeItemValueWorker = new BackgroundTask(false);
                storeItemValueWorker.DoWork += storeItemValueWorker_DoWork;
                storeItemValueWorker.TaskCompleted += storeItemValueWorker_TaskCompleted;
                storeItemValueWorker.TaskArgs = new StoreItemValueArgs { PropName = propName, PublicPropName = publicPropName, Item = item };
                storeItemValueWorker.RunTaskAsync();
                return "";
            }
            else
            {
                return item.GetStringValue(propName, publicPropName);
            }
        }

        void storeItemValueWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundTask task = e.Argument as BackgroundTask;
            StoreItemValueArgs args = task.TaskArgs as StoreItemValueArgs;
            string value = Utils.GetStringValue(args.Item.Store.GetItemValue(args.Item, args.PropName, args.PublicPropName));
            args.Item.SetPublicProperty(args.PublicPropName, value);
        }

        void storeItemValueWorker_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
        }

    }
}
