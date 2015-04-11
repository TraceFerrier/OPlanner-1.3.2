using ProductStudio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PlannerNameSpace
{
    public enum ShouldRefresh
    {
        Yes,
        No,
    }

    public enum RefreshType
    {
        QueryForChangedItems,
        QueryForAllItems,
    }

    public sealed class StoreQueryManager
    {
        private static readonly StoreQueryManager m_instance = new StoreQueryManager();

        public static StoreQueryManager Instance
        {
            get { return m_instance; }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Fired when the basic scheduling metadata items for the current product group 
        /// (OffTimeItems, GroupMemberItems, etc.) have been loaded and cached (before any 
        /// backlog or work items have been queried for).
        /// </summary>
        //------------------------------------------------------------------------------------
        public event EventHandler ScheduleMetadataReady;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Fired when a planner query requested by a call to BeginPlannerQuery has completed.
        /// </summary>
        //------------------------------------------------------------------------------------
        public event EventHandler<PlannerQueryCompletedEvent> PlannerQueryCompleted;

        string ProductGroupKey { get; set; }
        string ExternalProductGroupKey { get; set; }
        public bool IsQueryInProgress { get; set; }
        public bool IsRefreshInProgress { get; set; }

        public ShouldRefresh ShouldRefresh { get; set; }
        private RefreshType RefreshType;
        public DateTime LastRefreshTime { get; set; }
        BackgroundTask QueryTask { get; set; }

        public StoreQueryManager()
        {
            ShouldRefresh = ShouldRefresh.No;
            RefreshType = PlannerNameSpace.RefreshType.QueryForChangedItems;
            LastRefreshTime = DateTime.Now;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Kicks off a background query to retrieve the entire planner schedule for the
        ///  specified product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void BeginPlannerQuery(ShouldRefresh shouldRefresh, string productGroupKey, BackgroundTask continuingTask = null, RefreshType refreshType = RefreshType.QueryForChangedItems)
        {
            ProductGroupKey = productGroupKey;

            IsQueryInProgress = true;
            IsRefreshInProgress = shouldRefresh == PlannerNameSpace.ShouldRefresh.Yes;

            ShouldRefresh = shouldRefresh;
            RefreshType = refreshType;

            if (continuingTask != null)
            {
                QueryTask = new BackgroundTask(continuingTask);
            }
            else
            {
                QueryTask = new BackgroundTask(ShouldRefresh == ShouldRefresh.No);
            }

            QueryTask.DoWork += PlannerQueryTask_DoWork;
            QueryTask.TaskCompleted += PlannerQueryTask_Completed;
            QueryTask.IsProgressDialogIndeterminate = true;
            QueryTask.RunTaskAsync();
        }

        bool CheckResults(BackgroundTask taskWorker, DoWorkEventArgs e, BackgroundTaskResult result)
        {
            if (IsCancelled(taskWorker, e))
            {
                Globals.ApplicationManager.PlannerQueryCancelled(ShouldRefresh == ShouldRefresh.Yes);
                return false;
            }

            if (result.ResultType != ResultType.Completed)
            {
                e.Result = result;
                return false;
            }

            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Background worker that executes all planner store queries.
        /// </summary>
        //------------------------------------------------------------------------------------
        void PlannerQueryTask_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundTask taskWorker = e.Argument as BackgroundTask;

            const int totalTasks = 4;
            int currentTask = 1;

            {
                taskWorker.ReportProgress((currentTask * 100) / totalTasks, "", "Looking for your product group...");

                // First query for the items that are global to all product groups.
                List<ItemTypeID> typeList = new List<ItemTypeID>();

                /////
                typeList.Add(ItemTypeID.ProductGroup);
                typeList.Add(ItemTypeID.Train);
                typeList.Add(ItemTypeID.Quarter);
                typeList.Add(ItemTypeID.Persona);
                typeList.Add(ItemTypeID.HelpContent);
                ////

                ActiveItemTypeQuery query = new ActiveItemTypeQuery(ScheduleStore.Instance, null, typeList, ShouldRefresh);

                bool deferItemCreationforScheduleItems = ShouldRefresh == ShouldRefresh.Yes ? true : false;
                BackgroundTaskResult queryResult = ScheduleStore.Instance.ExecuteQuery(query, ShouldRefresh, taskWorker, deferItemCreationforScheduleItems);
                if (!CheckResults(taskWorker, e, queryResult))
                {
                    return;
                }

                // Query for bugs that users have filed against OPlanner
                OPlannerBugsQuery bugsQuery = new OPlannerBugsQuery(ScheduleStore.Instance, ShouldRefresh);
                queryResult = ScheduleStore.Instance.ExecuteQuery(bugsQuery, ShouldRefresh, taskWorker, deferItemCreationforScheduleItems);
                if (!CheckResults(taskWorker, e, queryResult))
                {
                    return;
                }

                ProductGroupItem productGroup = Globals.ItemManager.GetItem<ProductGroupItem>(ProductGroupKey);
                ProductGroupManager.Instance.CurrentProductGroup = productGroup;
                if (productGroup != null)
                {
                    productGroup.EnsureProductGroupMembers();
                }

                currentTask++;
                taskWorker.ReportProgress((currentTask * 100) / totalTasks, "", "Opening the plan for your product group...");

                // Query for all pillars first
                typeList.Clear();
                typeList.Add(ItemTypeID.Pillar);
                query = new ActiveItemTypeQuery(ScheduleStore.Instance, ProductGroupKey, typeList, ShouldRefresh);
                queryResult = ScheduleStore.Instance.ExecuteQuery(query, ShouldRefresh, taskWorker, deferItemCreationforScheduleItems);
                if (!CheckResults(taskWorker, e, queryResult))
                {
                    return;
                }

                // Next query for all the planner items associated with this product group
                typeList.Clear();
                typeList.Add(ItemTypeID.GroupMember);
                typeList.Add(ItemTypeID.ScrumTeam);
                typeList.Add(ItemTypeID.OffTime);
                typeList.Add(ItemTypeID.Scenario);
                typeList.Add(ItemTypeID.Experience);

                query = new ActiveItemTypeQuery(ScheduleStore.Instance, ProductGroupKey, typeList, ShouldRefresh);
                queryResult = ScheduleStore.Instance.ExecuteQuery(query, ShouldRefresh, taskWorker, deferItemCreationforScheduleItems);
                if (!CheckResults(taskWorker, e, queryResult))
                {
                    return;
                }

                currentTask++;
                taskWorker.ReportProgress((currentTask * 100) / totalTasks, "", "Loading all your team's backlog and work items...");

                OnScheduleMetadataReady();

                // Next, query for all backlogItems and workItems owned by the product group
                List<string> groupMembers = ProductGroupManager.Instance.GetMemberAliases();
                if (groupMembers.Count > 0)
                {
                    AsyncObservableCollection<TrainItem> currentTrains = TrainManager.Instance.GetQueryableTrains();
                    List<int> pillarPathIDs = ProductGroupManager.Instance.GetAllPillarPathIDs();

                    // Get all backlog items first, so they'll all be available once we query for work items and 
                    // attempt to associate them with their parent backlog items.
                    typeList.Clear();
                    typeList.Add(ItemTypeID.BacklogItem);

                    List<int> treeIds = new List<int>();
                    foreach (int pathID in pillarPathIDs)
                    {
                        treeIds.Add(pathID);
                    }

                    bool DeferItemCreationForHostItems = false;
                    HostItemQuery hostQuery = new HostItemQuery(HostItemStore.Instance, typeList, groupMembers, treeIds, currentTrains, ShouldRefresh, RefreshType);
                    queryResult = HostItemStore.Instance.ExecuteQuery(hostQuery, ShouldRefresh, taskWorker, DeferItemCreationForHostItems);
                    if (!CheckResults(taskWorker, e, queryResult))
                    {
                        return;
                    }

                    // Then get the work items.
                    typeList.Clear();
                    typeList.Add(ItemTypeID.WorkItem);

                    hostQuery = new HostItemQuery(HostItemStore.Instance, typeList, groupMembers, treeIds, currentTrains, ShouldRefresh, RefreshType);
                    queryResult = HostItemStore.Instance.ExecuteQuery(hostQuery, ShouldRefresh, taskWorker, DeferItemCreationForHostItems);
                    if (!CheckResults(taskWorker, e, queryResult))
                    {
                        return;
                    }

                }

                e.Result = new BackgroundTaskResult { ResultType = ResultType.Completed };

            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Called when the PlannerQuery task kicked off by BeginPlannerQuery completes.
        /// </summary>
        //------------------------------------------------------------------------------------
        void PlannerQueryTask_Completed(object TaskArgs, BackgroundTaskResult result)
        {
            IsQueryInProgress = false;
            IsRefreshInProgress = false;

            if (result.ResultType == ResultType.Completed)
            {
                if (ShouldRefresh == ShouldRefresh.Yes)
                {
                    LastRefreshTime = DateTime.Now;
                }
            }

            if (PlannerQueryCompleted != null)
            {
                PlannerQueryCompleted(this, new PlannerQueryCompletedEvent(result, ShouldRefresh));
            }
        }

        public void CancelQuery()
        {
            if (IsQueryInProgress && QueryTask != null)
            {
                QueryTask.CancelTask();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Kicks off a background query to retrieve the data needed to build burndown
        ///  statistics for the currently loaded product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void BeginProductGroupQuery(BackgroundTask continuingTask)
        {
            BackgroundTask queryTask = new BackgroundTask(continuingTask);
            queryTask.DoWork += ProductGroupQuery_DoWork;
            queryTask.TaskCompleted += ProductGroupQuery_TaskCompleted;
            queryTask.IsProgressDialogIndeterminate = true;
            queryTask.RunTaskAsync();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Background worker for the ProductGroup query.
        /// </summary>
        //------------------------------------------------------------------------------------
        void ProductGroupQuery_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Tracer.WriteTrace("ProductGroupQuery_DoWork");

            BackgroundTask taskWorker = e.Argument as BackgroundTask;
            taskWorker.ReportProgress(0, "", "Loading Product Groups...");

            try
            {
                List<ItemTypeID> typeList = new List<ItemTypeID>();
                typeList.Add(ItemTypeID.ProductGroup);
                typeList.Add(ItemTypeID.HelpContent);
                ActiveItemTypeQuery query = new ActiveItemTypeQuery(ScheduleStore.Instance, null, typeList, ShouldRefresh.No);

                Tracer.WriteTrace("ScheduleStore.Instance.ExecuteQuery");
                BackgroundTaskResult queryResult = ScheduleStore.Instance.ExecuteQuery(query, ShouldRefresh, taskWorker, false);
                if (!CheckResults(taskWorker, e, queryResult))
                {
                    return;
                }

                e.Result = new BackgroundTaskResult { ResultType = ResultType.Completed };
            }

            catch (Exception exception)
            {
                Globals.ApplicationManager.WriteToEventLog(exception.Message);
                Globals.ApplicationManager.WriteToEventLog(exception.StackTrace);
                e.Result = new BackgroundTaskResult { ResultType = ResultType.Failed, ResultMessage = exception.Message };
                return;
            }
        }

        void ProductGroupQuery_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            Globals.ApplicationManager.ProductGroupQueryComplete();
        }

        public void BeginRefreshQuery(string productGroupKey)
        {
            RefreshType refreshType = PlannerNameSpace.RefreshType.QueryForChangedItems;
            if (PillarManager.Instance.RefreshBasedOnPillarChangesPending)
            {
                PillarManager.Instance.RefreshBasedOnPillarChangesPending = false;
                refreshType = PlannerNameSpace.RefreshType.QueryForAllItems;
            }

            BeginPlannerQuery(ShouldRefresh.Yes, productGroupKey, null, refreshType);
        }

        bool IsCancelled(BackgroundTask taskWorker, System.ComponentModel.DoWorkEventArgs e)
        {
            if (taskWorker.CancellationPending)
            {
                e.Cancel = true;
                e.Result = new BackgroundTaskResult { ResultType = ResultType.Cancelled };
                return true;
            }

            return false;
        }

        void OnScheduleMetadataReady()
        {
            if (ScheduleMetadataReady != null)
            {
                ScheduleMetadataReady(this, EventArgs.Empty);
            }
        }

    }
}
