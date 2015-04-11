using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public enum CommitType
    {
        UserCommit,
        ImmediateCommit,
        CommitAborted,
    }

    public class StoreCommitCompleteEventArgs : EventArgs
    {
        public StoreCommitCompleteEventArgs(CommitType commitType, BackgroundTaskResult result)
        {
            CommitType = commitType;
            Result = result;
        }

        public BackgroundTaskResult Result { get; set; }
        public CommitType CommitType { get; set; }
    }
}
