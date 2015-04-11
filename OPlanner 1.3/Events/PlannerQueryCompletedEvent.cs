using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class PlannerQueryCompletedEvent : EventArgs
    {
        public BackgroundTaskResult Result { get; set; }
        public ShouldRefresh ShouldRefresh { get; set; }

        public PlannerQueryCompletedEvent(BackgroundTaskResult result, ShouldRefresh shouldRefresh)
        {
            Result = result;
            ShouldRefresh = shouldRefresh;
        }
    }
}
