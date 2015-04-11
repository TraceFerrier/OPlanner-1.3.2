using PlannerNameSpace.Windows;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace PlannerNameSpace
{
    public enum ProgressDialogOption
    {
        NoProgress,
        StandardProgress,
        StandardProgressNoClose,
        InheritedProgress,
    }

    public delegate void BackgroundTaskCompleteHandler(object TaskArgs, BackgroundTaskResult result);

    public class BackgroundTask
    {
        public event DoWorkEventHandler DoWork;
        public event BackgroundTaskCompleteHandler TaskCompleted;

        ProgressDialogOption ProgressDialogOption;
        BackgroundTaskProgressDialog ProgressDialog = null;
        AbortableBackgroundWorker TaskWorker;
        Stopwatch TimeOutClock;
        public int TimeOutInMilliseconds { get; set; }
        public object TaskArgs { get; set; }

        public BackgroundTask(bool showProgressDialog)
        {
            InitializeTask(showProgressDialog ? ProgressDialogOption.StandardProgress : ProgressDialogOption.NoProgress);
        }

        public BackgroundTask(ProgressDialogOption progressOption)
        {
            InitializeTask(progressOption);
        }

        void InitializeTask(ProgressDialogOption progressOption)
        {
            ProgressDialogOption = progressOption;
            InitializeTask();

            if (ProgressDialogOption == ProgressDialogOption.StandardProgress || ProgressDialogOption == ProgressDialogOption.StandardProgressNoClose)
            {
                InitializeProgressDialog(new BackgroundTaskProgressDialog());
            }
        }

        public BackgroundTask(BackgroundTask task)
        {
            if (task == null)
            {
                InitializeTask(ProgressDialogOption.StandardProgress);
            }
            else
            {
                ProgressDialogOption = PlannerNameSpace.ProgressDialogOption.InheritedProgress;
                InitializeTask();
                InitializeProgressDialog(task.ProgressDialog);
            }
        }

        void InitializeTask()
        {
            TimeOutInMilliseconds = 1000 * 30;
            TaskWorker = new AbortableBackgroundWorker();
            TaskWorker.WorkerReportsProgress = true;
            TaskWorker.WorkerSupportsCancellation = true;
            TaskWorker.RunWorkerCompleted += BackgroundTask_RunWorkerCompleted;
            TaskWorker.ProgressChanged += BackgroundTask_ProgressChanged;

            TimeOutClock = new Stopwatch();
        }

        void InitializeProgressDialog(BackgroundTaskProgressDialog dialog)
        {
            ProgressDialog = dialog;
            ProgressDialog.Title = ApplicationManager.AssemblyProduct;
            ProgressDialog.CancelRequested += ProgressDialog_CancelRequested;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Instructs this task to begin background execution.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void RunTaskAsync()
        {
            if (DoWork != null)
            {
                TaskWorker.DoWork += DoWork;
                TaskWorker.RunWorkerAsync(this);

                TimeOutClock.Start();

                if (ProgressDialog != null)
                {
                    if (ProgressDialogOption != PlannerNameSpace.ProgressDialogOption.InheritedProgress)
                    {
                        ProgressDialog.ShowDialog();
                    }
                }
            }
        }

        public bool CancellationPending { get { return TaskWorker.CancellationPending; } }

        public bool IsProgressDialogIndeterminate
        {
            get 
            {
                if (ProgressDialog == null)
                {
                    return false;
                }

                return ProgressDialog.IsIndeterminate; 
            }

            set
            {
                if (ProgressDialog != null)
                {
                    ProgressDialog.IsIndeterminate = value;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when the user clicks the cancel button on the progress dialog
        /// </summary>
        //------------------------------------------------------------------------------------
        void ProgressDialog_CancelRequested()
        {
            CancelTask();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Can be called to attempt to cancel the current commit session.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void CancelTask()
        {
            if (TaskWorker.IsBusy)
            {
                TaskWorker.CancelAsync();

                if (ProgressDialog != null)
                {
                    ProgressDialog.SetProgressDescription("");
                    ProgressDialog.SetProgressMessage("Cancelling...");
                    ProgressDialog.IsIndeterminate = true;
                    ProgressDialog.IsCancelButtonEnabled = false;
                }

                // If the time between now and the last time we got a progress tick exceeds
                // the timeout limit, we have to assume the commit thread is not responding
                // to our cancel request, so kill it.
                if (TimeOutClock.ElapsedMilliseconds > TimeOutInMilliseconds)
                {
                    AbortTask();
                }

                // Else, send a cancellation request, and set a timer to go off once the timeout
                // limit is reached.
                else if (!TaskWorker.CancellationPending)
                {

                    int timeUntilTimeout = TimeOutInMilliseconds - (int)TimeOutClock.ElapsedMilliseconds;
                    if (timeUntilTimeout < 2)
                    {
                        timeUntilTimeout = 2;
                    }

                    DispatcherTimer onCancelledTimer = new DispatcherTimer();
                    onCancelledTimer.Interval = new TimeSpan(0, 0, 0, 0, timeUntilTimeout);
                    onCancelledTimer.Tick += onCancelledTimer_Tick;
                    onCancelledTimer.Start();
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when the cancel timeout timer goes off, indicating that the
        /// timeout waiting period has expired.
        /// </summary>
        //------------------------------------------------------------------------------------
        void onCancelledTimer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer onCancelledTimer = sender as DispatcherTimer;
            onCancelledTimer.Stop();

            // If the commit thread still hasn't responded to our cancellation request,
            // try to kill it.
            if (TaskWorker.IsBusy)
            {
                AbortTask();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Attempts to abort and clean up the currently running commit thread.
        /// </summary>
        //------------------------------------------------------------------------------------
        void AbortTask()
        {
            if (TaskWorker.IsBusy)
            {
                TaskWorker.Abort();
                TaskWorker.Dispose();
            }

            if (ProgressDialog != null)
            {
                ProgressDialog.CloseDialog();
            }

            Globals.EventManager.OnStoreCommitComplete(this, new StoreCommitCompleteEventArgs(CommitType.CommitAborted, new BackgroundTaskResult { ResultType = ResultType.Cancelled }));
        }

        class TaskProgressArgs
        {
            public string Description { get; set; }
            public string Message { get; set; }
        }

        public void ReportProgress(int progressPercentage, string progressDescription, string progressMessage)
        {
            TaskWorker.ReportProgress(progressPercentage, new TaskProgressArgs { Description = progressDescription, Message = progressMessage });
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Handles progress notifications from the currently running commit thread.
        /// </summary>
        //------------------------------------------------------------------------------------
        void BackgroundTask_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TimeOutClock.Restart();

            if (ProgressDialog != null)
            {
                TaskProgressArgs args = e.UserState as TaskProgressArgs;
                ProgressDialog.SetProgressDescription(args.Description);
                ProgressDialog.SetProgressMessage(args.Message);
                ProgressDialog.SetProgressValue(e.ProgressPercentage);
            }
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when the commit thread completes (or successfully responds to a
        /// cancellation request).
        /// </summary>
        //------------------------------------------------------------------------------------
        void BackgroundTask_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AbortableBackgroundWorker worker = sender as AbortableBackgroundWorker;
            if (worker.IsAborted)
            {
                if (TaskCompleted != null)
                {
                    TaskCompleted(TaskArgs, new BackgroundTaskResult {Task = this, ResultType = ResultType.Cancelled });
                }
            }
            else
            {
                if (ProgressDialog != null)
                {
                    ProgressDialog.CancelRequested -= ProgressDialog_CancelRequested;

                    if (ProgressDialogOption != PlannerNameSpace.ProgressDialogOption.StandardProgressNoClose)
                    {
                        ProgressDialog.CloseDialog();
                    }
                }

                if (e.Cancelled)
                {
                    if (TaskCompleted != null)
                    {
                        TaskCompleted(TaskArgs, new BackgroundTaskResult { Task = this, ResultType = ResultType.Cancelled });
                    }
                }
                else
                {
                    BackgroundTaskResult result = e.Result as BackgroundTaskResult;
                    if (result != null)
                    {
                        result.Task = this;
                    }

                    if (result != null && result.ResultType == ResultType.Failed)
                    {
                        Globals.ApplicationManager.HandleFailedTask(result);
                    }

                    if (TaskCompleted != null)
                    {
                        TaskCompleted(TaskArgs, result);
                    }
                }
            }
        }

    }
}
