using System;
using System.Collections.Generic;

namespace PlannerNameSpace
{
    public class CommitChangesWorker
    {
        private List<StoreItem> ChangeList;
        private List<StoreItem> ItemsCommittedSuccessfully;
        private List<StoreItem> NewCommittedItems;
        private CommitType CommitType;
        public CommitChangesWorker()
        {
            ItemsCommittedSuccessfully = new List<StoreItem>();
            NewCommittedItems = new List<StoreItem>();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Commits the given item to the backing store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void CommitItem(StoreItem item)
        {
            CommitType = CommitType.ImmediateCommit;
            SetCommitItem(item);
            DoCommit(false);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Commits the given collection of items to the backing store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void CommitItems(List<StoreItem> items, bool shouldShowProgress)
        {
            CommitType = CommitType.UserCommit;
            SetChangeList(items);
            DoCommit(shouldShowProgress);
        }

        private void SetChangeList(List<StoreItem> changeList)
        {
            System.Diagnostics.Debug.Assert(ChangeList == null);
            ChangeList = changeList;
        }

        private void SetCommitItem(StoreItem item)
        {
            System.Diagnostics.Debug.Assert(ChangeList == null);
            ChangeList = new List<StoreItem>();
            ChangeList.Add(item);
        }

        private StoreItem GetCommitItem()
        {
            System.Diagnostics.Debug.Assert(ChangeList != null && ChangeList.Count == 1);
            return ChangeList[0];
        }

        private void DoCommit(bool shouldShowProgress)
        {
            BackgroundTask commitTask = new BackgroundTask(shouldShowProgress);
            commitTask.DoWork += CommitTask_DoWork;
            commitTask.TaskCompleted += CommitTask_TaskCompleted;
            commitTask.RunTaskAsync();
        }

        void CommitTask_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundTask task = e.Argument as BackgroundTask;
            if (ChangeList.Count > 0)
            {
                int currentItem = 1;

                int totalItems = ChangeList.Count;

                foreach (StoreItem item in ChangeList)
                {
                    if (task.CancellationPending)
                    {
                        e.Cancel = true;
                        e.Result = new BackgroundTaskResult { ResultType = ResultType.Cancelled };
                        return;
                    }

                    task.ReportProgress((currentItem * 100) / totalItems, item.ID.ToString() + ": " + item.Title, "Change " + currentItem.ToString() + " of " + totalItems.ToString());

                    BackgroundTaskResult result = CommitItemWorker(item, NewCommittedItems);
                    if (result.ResultType != ResultType.Completed)
                    {
                        e.Result = result;
                        return;
                    }

                    ItemsCommittedSuccessfully.Add(item);
                    currentItem++;
                }

                e.Result = new BackgroundTaskResult { ResultType = ResultType.Completed };
            }
        }

        void CommitTask_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            HandleCommitComplete(result);
        }

        BackgroundTaskResult CommitItemWorker(StoreItem item, List<StoreItem> newCommittedItems)
        {
            //try
            {
                Datastore store = item.Store;
                switch (item.ChangeAction)
                {
                    case StoreChangeAction.Default:
                        if (item.IsNew)
                        {
                            item.DSItem = store.CreateDSItem();

                            string itemType = store.GetItemTypeName(item.StoreItemType);
                            item.DSItem.Fields[store.PropNameType].Value = itemType;

                            store.SaveItemWorker(item);
                            store.FinalizeAfterSave(item);
                            item.PersistState = PersistStates.PersistedToStore;
                            newCommittedItems.Add(item);
                        }
                        else
                        {
                            store.SaveItemWorker(item);
                            store.FinalizeAfterSave(item);
                        }
                        break;

                    case StoreChangeAction.ResolveItem:
                        item.Status = StatusValues.Active;
                        store.ResolveStoreItemWorker(item);
                        store.FinalizeAfterSave(item);
                        break;

                    case StoreChangeAction.ResolveAndCloseItem:
                        store.ResolveStoreItemWorker(item);
                        store.CloseStoreItemWorker(item);
                        store.FinalizeAfterSave(item);
                        break;

                    case StoreChangeAction.CloseItem:
                        store.CloseStoreItemWorker(item);
                        store.FinalizeAfterSave(item);
                        break;

                    case StoreChangeAction.ActivateItem:
                        store.ActivateStoreItemWorker(item);
                        store.FinalizeAfterSave(item);
                        break;
                }

                return new BackgroundTaskResult { ResultType = ResultType.Completed };
            }

            //catch (Exception exception)
            //{
            //    Globals.ApplicationManager.HandleException(exception);
            //    return new BackgroundTaskResult { ResultType = ResultType.Failed, ResultMessage = exception.Message };
            //}

        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Encapsulates all rules for handling the end of a commit operation.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void HandleCommitComplete(BackgroundTaskResult result)
        {
            foreach (StoreItem item in NewCommittedItems)
            {
                Globals.EventManager.OnNewItemCommittedToStore(item);
            }

            Globals.EventManager.OnStoreCommitComplete(this, new StoreCommitCompleteEventArgs(CommitType, result));
        }
    }


}
