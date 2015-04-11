using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public class CommitmentStatusManager
    {
        public bool CommitmentStatusComputationComplete { get; set; }

        public AsyncObservableCollection<BacklogItem> BacklogItemsOnTrack;
        public AsyncObservableCollection<BacklogItem> BacklogItemsNotOnTrack;
        static Dictionary<TrainCommitmentStatusValue, TrainCommitmentInfo> InfoValues;

        public CommitmentStatusManager()
        {
            CommitmentStatusComputationComplete = false;
            Globals.EventManager.DiscoveryComplete += Handle_DiscoveryComplete;
            BacklogItemsOnTrack = new AsyncObservableCollection<BacklogItem>();
            BacklogItemsNotOnTrack = new AsyncObservableCollection<BacklogItem>();
            InfoValues = new Dictionary<TrainCommitmentStatusValue, TrainCommitmentInfo>();
            TrainCommitmentInfo.CreateInfoValues(InfoValues);
        }

        public static string GetStatusText(TrainCommitmentStatusValue statusValue)
        {
            return InfoValues[statusValue].StatusText;
        }

        public static Brush GetStatusColor(TrainCommitmentStatusValue statusValue)
        {
            return InfoValues[statusValue].StatusColor;
        }

        public void ShowCommitmentStatusCharts()
        {
            TrainStatusChart chart = new TrainStatusChart();
            chart.Show();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if a 'snapshot' of approved backlog items has been taken and stored
        /// for the given pillar and train.  If so, the date of the snapshot will be returned
        /// in 'approvalDate', and the alias of the user that approved the snapshot will be 
        /// returned in 'approver'.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsApprovedSnapshotAvailable(PillarItem pillar, TrainItem train, out Nullable<DateTime> approvalDate, out string approver)
        {
            approvalDate = null;
            approver = null;
            List<BacklogItem> backlogItems = BacklogItem.Items.ToList();
            foreach (BacklogItem backlogItem in backlogItems)
            {
                if (backlogItem.ParentPillarItem == pillar)
                {
                    if (backlogItem.IsCommittedToTrain(train))
                    {
                        approvalDate = backlogItem.GetCommittedToTrainDate(train);
                        approver = backlogItem.GetCommittedToTrainApprover(train);
                        return true;
                    }
                }
            }

            return false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Clears the 'snapshot' of approved backlog items for the given pillar and train, if
        /// any.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ClearSnapshot(PillarItem pillar, TrainItem train)
        {
            List<BacklogItem> backlogItems = BacklogItem.Items.ToList();
            foreach (BacklogItem item in backlogItems)
            {
                if (item.ParentPillarItem == pillar)
                {
                    item.ClearCommittedTrain(train);
                }
            }
        }

        void Handle_DiscoveryComplete(object sender, EventArgs e)
        {
            StartLandingItems();
            RegisterStoreItemEventHandlers();
        }

        void RegisterStoreItemEventHandlers()
        {
            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;
        }

        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            string changedPropName = e.Change.DSPropName;
            string publicPropName = e.Change.PublicPropName;
            StoreItem changedItem = e.Change.Item;

            switch (e.Change.ChangeType)
            {
                case ChangeType.Added:
                case ChangeType.Removed:
                    RestartLandingItemsOnChange(changedItem);
                    break;
                case ChangeType.Updated:
                    {
                        switch (changedItem.StoreItemType)
                        {
                            case ItemTypeID.WorkItem:
                                if (WorkItem.IsPropNameEstimate(publicPropName) || WorkItem.IsPropNameCompleted(publicPropName))
                                {
                                    RestartLandingItemsOnChange(changedItem);
                                }
                                break;

                            case ItemTypeID.BacklogItem:
                                if (changedPropName.IsStoreProperty(Datastore.PropNameBacklogCommitmentSetting) ||
                                    changedPropName.IsStoreProperty(Datastore.PropNameStoryPoints))
                                {
                                    RestartLandingItemsOnChange(changedItem);
                                }
                                break;

                            case ItemTypeID.OffTime:
                                RestartLandingItemsOnChange(changedItem);
                                break;
                        }
                    }
                    break;
            }
        }

        void RestartLandingItemsOnChange(StoreItem item)
        {
            if (item.StoreItemType == ItemTypeID.WorkItem || item.StoreItemType == ItemTypeID.OffTime || item.StoreItemType == ItemTypeID.BacklogItem)
            {
                StartLandingItems();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Call this anytime you want to re-calculate all the landing dates for eligible
        /// backlog and work items.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void StartLandingItems()
        {
            BacklogItemsOnTrack.Clear();
            BacklogItemsNotOnTrack.Clear();
            InitializeGroupMembers();

            BackgroundWorker landingWorker = new BackgroundWorker();
            landingWorker.DoWork += landingWorker_DoWork;
            landingWorker.RunWorkerCompleted += landingWorker_RunWorkerCompleted;
            landingWorker.RunWorkerAsync();
        }

        void InitializeGroupMembers()
        {
            AsyncObservableCollection<GroupMemberItem> pillarMembers = Globals.ItemManager.GroupMemberItems;
            foreach (GroupMemberItem member in pillarMembers)
            {
                member.LatestLandingDate = null;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Background routine that does the actual work to calculate the landing dates for
        /// all in-progress work items, and all committed or in-progress backlog items.
        /// </summary>
        //------------------------------------------------------------------------------------
        void landingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            AsyncObservableCollection<BacklogItem> backlogItemsInProgress = new AsyncObservableCollection<BacklogItem>();
            Dictionary<string, AsyncObservableCollection<WorkItem>> workItemsByGroupMember = BuildWorkItemsByGroupMember(backlogItemsInProgress);
            SetWorkItemLandingDates(workItemsByGroupMember);
            SetInProgressBacklogItemLandingDates(backlogItemsInProgress);
            SetRemainingCommittedBacklogItemLandingDates();
        }

        void landingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Send out an event only the first time a computation completes
            if (!CommitmentStatusComputationComplete)
            {
                CommitmentStatusComputationComplete = true;
                Globals.EventManager.OnCommitmentStatusComputationComplete();
            }

            // Make sure that all backlog items that have no work items (and thus never got their landing
            // date updated) get a chance to refresh their landing date text.
            List<BacklogItem> allBacklogItems = BacklogItem.Items.ToList();
            foreach (BacklogItem backlogItem in allBacklogItems)
            {
                backlogItem.NotifyPropertyChanged(() => backlogItem.LandingDateText);
                backlogItem.NotifyPropertyChanged(() => backlogItem.TrainCommitmentStatusText);
                backlogItem.NotifyPropertyChanged(() => backlogItem.TrainCommitmentStatusColor);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// For every group member that has active and in-progress work assigned to them, this
        /// routine builds a list of that member's committed work items.  For a work item to
        /// be in-progress, it's parent Backlog Item must be in the in-progress state.
        /// </summary>
        //------------------------------------------------------------------------------------
        Dictionary<string, AsyncObservableCollection<WorkItem>> BuildWorkItemsByGroupMember(AsyncObservableCollection<BacklogItem> backlogItemsInProgress)
        {
            Dictionary<string, AsyncObservableCollection<WorkItem>> workItemsByGroupMember = new System.Collections.Generic.Dictionary<string, AsyncObservableCollection<WorkItem>>();
            List<WorkItem> workItems = WorkItem.Items.ToList();

            foreach (WorkItem workItem in workItems)
            {
                if (workItem.IsActive && workItem.WorkRemaining > 0)
                {
                    GroupMemberItem groupMember = workItem.AssignedToGroupMember;
                    BacklogItem backlogItem = workItem.ParentBacklogItem;
                    if (groupMember != null && backlogItem != null && backlogItem.IsInProgressOrCompleted)
                    {
                        string key = groupMember.StoreKey;
                        if (!workItemsByGroupMember.ContainsKey(key))
                        {
                            workItemsByGroupMember.Add(key, new AsyncObservableCollection<WorkItem>());
                        }

                        AsyncObservableCollection<WorkItem> memberWorkItems = workItemsByGroupMember[key];
                        memberWorkItems.Add(workItem);

                        if (!backlogItemsInProgress.Contains(backlogItem))
                        {
                            backlogItemsInProgress.Add(backlogItem);
                        }
                    }
                }
            }

            return workItemsByGroupMember;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// For each group member in the given dictionary, this routine calculates the 
        /// 'landing date' for each work item in the member's work item list.  The landing
        /// date is the date the work item is expected to be completed, based on the work
        /// remaining for each work item, the business rank of the work items, the capacity
        /// per day for the member, and any off days associated with the member.  When the
        /// landing date is calculated, it is stored in the appropriate property for the work
        /// item.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SetWorkItemLandingDates(Dictionary<string, AsyncObservableCollection<WorkItem>> workItemsByGroupMember)
        {
            foreach (KeyValuePair<string, AsyncObservableCollection<WorkItem>> kvp in workItemsByGroupMember)
            {
                string key = kvp.Key;
                GroupMemberItem groupMember = Globals.ItemManager.GetItem<GroupMemberItem>(key);
                if (groupMember != null)
                {
                    AsyncObservableCollection<WorkItem> memberWorkItems = kvp.Value;
                    memberWorkItems.Sort((x, y) => x.BusinessRank.CompareTo(y.BusinessRank));
                    SetLandingDates(DateTime.Today, groupMember.CapacityPerDay, memberWorkItems, groupMember.OffTimeItems);
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Worker function for SetWorkItemLandingDates.
        /// </summary>
        //------------------------------------------------------------------------------------
        static void SetLandingDates(DateTime startDate, double capacity, AsyncObservableCollection<WorkItem> workItems, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            if (capacity < 1 || capacity > Globals.IdealHoursPerDay)
            {
                throw new ArgumentOutOfRangeException();
            }

            int workItemIndex = 0;
            int workItemHour = 0;
            int dateHour = 0;
            int workItemCount = workItems.Count;
            DateTime landingDate = Utils.GetNextWorkingDay(startDate, offTimeItems);

            while (workItemIndex < workItemCount)
            {
                WorkItem currentWorkItem = workItems.GetItem(workItemIndex);
                workItemHour++;
                dateHour++;
                if (workItemHour > currentWorkItem.WorkRemaining)
                {
                    currentWorkItem.LandingDate = landingDate;
                    currentWorkItem.NotifyPropertyChanged(() => currentWorkItem.LandingDateText);
                    workItemIndex++;
                    workItemHour = 0;
                }

                if (dateHour > capacity)
                {
                    landingDate = landingDate.AddDays(1);
                    landingDate = Utils.GetNextWorkingDay(landingDate, offTimeItems);
                    dateHour = 0;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the landing date for each of the backlog items in the given list.  This list
        /// represents all the 
        /// </summary>
        //------------------------------------------------------------------------------------
        void SetInProgressBacklogItemLandingDates(AsyncObservableCollection<BacklogItem> backlogItemsInProgress)
        {
            foreach (BacklogItem backlogItem in backlogItemsInProgress)
            {
                AsyncObservableCollection<WorkItem> backlogWorkItems = backlogItem.WorkItems;

                DateTime landingDate = new DateTime();
                foreach (WorkItem workItem in backlogWorkItems)
                {
                    if (workItem.LandingDate > landingDate)
                    {
                        landingDate = workItem.LandingDate;
                    }
                }

                backlogItem.LandingDate = landingDate;
                if (backlogItem.LandingDateStatus == LandingDateStatus.OnTrack)
                {
                    if (!BacklogItemsOnTrack.Contains(backlogItem))
                    {
                        BacklogItemsOnTrack.Add(backlogItem);
                    }
                }
                else
                {
                    if (!BacklogItemsNotOnTrack.Contains(backlogItem))
                    {
                        BacklogItemsNotOnTrack.Add(backlogItem);
                    }
                }
            }

        }

        DateTime GetLatestLandingDate(List<BacklogItem> backlogItems)
        {
            Nullable<DateTime> latestInProgressLandingDate = null;
            foreach (BacklogItem backlogItem in backlogItems)
            {
                if (backlogItem.CommitmentSetting == CommitmentSettingValues.In_Progress)
                {
                    if (backlogItem.LandingDate != null && (latestInProgressLandingDate == null || backlogItem.LandingDate > latestInProgressLandingDate))
                    {
                        latestInProgressLandingDate = backlogItem.LandingDate;
                    }
                }
            }

            if (latestInProgressLandingDate == null)
            {
                latestInProgressLandingDate = DateTime.Today;
            }

            return latestInProgressLandingDate.Value;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the next member in the list represented by enumerator.  If the end of the
        /// list is reached, the enumeration will wrap, and the first member will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        GroupMemberItem GetNextGroupMember(IEnumerator<GroupMemberItem> enumerator)
        {
            enumerator.MoveNext();
            GroupMemberItem nextMember = enumerator.Current;
            if (nextMember == null)
            {
                enumerator.Reset();
                enumerator.MoveNext();
                nextMember = enumerator.Current;
                if (nextMember == null)
                {
                    return null;
                }
            }

            return nextMember;
        }

        int SortMembersHighCapacityToLow(GroupMemberItem x, GroupMemberItem y)
        {
            if (x.CapacityPerDay == y.CapacityPerDay)
            {
                return 0;
            }

            if (y.CapacityPerDay > x.CapacityPerDay)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the landing date for each of the remaining backlog items that aren't 
        /// in-progress, but have been marked as committed, and have a valid Story Points
        /// value set.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SetRemainingCommittedBacklogItemLandingDates()
        {
            // Get all the backlog items for a pillar
            List<PillarItem> pillars = PillarManager.PillarItems.ToList();

            // Set the landing dates, pillar by pillar
            foreach (PillarItem pillar in pillars)
            {
                // For this pillar, find the in-progress backlog item with the latest landing date
                List<BacklogItem> backlogItems = BacklogItem.GetPillarBacklogItems(pillar);
                backlogItems.Sort((x, y) => x.BusinessRank.CompareTo(y.BusinessRank));
                DateTime latestInProgressLandingDate = GetLatestLandingDate(backlogItems);

                List<GroupMemberItem> pillarMembers = Globals.GroupMembers.GetMembersByPillar(pillar, DisciplineValues.Dev);
                pillarMembers.Sort((x, y) => SortMembersHighCapacityToLow(x, y));
                if (pillarMembers.Count > 0)
                {
                    IEnumerator<GroupMemberItem> pillarEnumerator = pillarMembers.GetEnumerator();

                    // Then, for each pillar backlog item that isn't in progress, but is committed, advance through the
                    // items by business rank, and incrementally set the landing dates based on story point estimate
                    // (with each story point representing a business day of predicted effort).
                    foreach (BacklogItem backlogItem in backlogItems)
                    {
                        if (backlogItem.CommitmentSetting == CommitmentSettingValues.Committed)
                        {
                            if (backlogItem.StoryPoints > 0)
                            {
                                GroupMemberItem currentMember = GetNextGroupMember(pillarEnumerator);

                                // Estimate the number of days required by the current member to finish this backlog item, based on story points
                                int daysToFinish = (int)Math.Ceiling(((double)backlogItem.StoryPoints * (Globals.AvgCapacityPerDay / currentMember.CapacityPerDay)));

                                // Find out when the last job calculated for this member was scheduled to end - that's
                                // the date that this job job will be scheduled to start.
                                DateTime? startingDate = currentMember.LatestLandingDate;
                                if (startingDate == null)
                                {
                                    startingDate = DateTime.Today;
                                }

                                // Estimate the landing date for this job, if performed by this member.
                                DateTime newLandingDate = SetLandingDate(backlogItem, startingDate.Value, daysToFinish, currentMember.OffTimeItems);

                                // After the job is over, add in the team's number of bug buffer days between jobs (giving the member
                                // time to get current on bugs before starting the next job.
                                newLandingDate = Utils.AddWorkingDays(newLandingDate, Globals.BugBufferDaysBetweenBacklogItems, currentMember.OffTimeItems);
                                currentMember.LatestLandingDate = newLandingDate;
                                backlogItem.ProjectedGroupMember = currentMember;
                            }
                            else
                            {
                                backlogItem.LandingDate = default(DateTime);
                            }
                        }
                    }
                }
            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Worker function that sets the landing date for the given backlog item, based on
        /// the given starting date, and the given estimate of the number of working days to
        /// complete the job.
        /// </summary>
        //------------------------------------------------------------------------------------
        DateTime SetLandingDate(BacklogItem backlogItem, DateTime startingDate, int estimateInDays, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            DateTime landingDate = Utils.AddWorkingDays(startingDate, estimateInDays, offTimeItems);
            backlogItem.LandingDate = landingDate;
            return landingDate;
        }
    }
}
