using System.ComponentModel;
using System.Threading;

namespace PlannerNameSpace
{
    public class AbortableBackgroundWorker : BackgroundWorker
    {
        private Thread WorkerThread;
        public bool IsAborted { get; set; }

        public AbortableBackgroundWorker()
        {
            IsAborted = false;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            WorkerThread = Thread.CurrentThread;

            try
            {
                base.OnDoWork(e);
            }

            catch (ThreadAbortException)
            {
                e.Cancel = true;
                IsAborted = true;

                //Prevents ThreadAbortException propagation
                Thread.ResetAbort(); 
            }
        }

        public void Abort()
        {
            if (WorkerThread != null)
            {
                WorkerThread.Abort();
                WorkerThread = null;
            }
        }
    }

}
