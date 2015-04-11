using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public enum ResultType
    {
        Completed,
        Failed,
        Cancelled,
        TimedOut,
    }

    public class BackgroundTaskResult
    {
        public BackgroundTask Task { get; set; }
        public ResultType ResultType { get; set; }
        public string ResultMessage { get; set; }
    }

}
