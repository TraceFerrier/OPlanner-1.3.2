
namespace PlannerNameSpace
{
    //------------------------------------------------------------------------------------
    /// <summary>
    ///  WorkItem
    /// </summary>
    //------------------------------------------------------------------------------------
    public partial class WorkItem
    {
        public static string SubstatusPropName = Utils.GetPropertyName((WorkItem s) => s.SubStatus);
        public static string EstimatePropName = Utils.GetPropertyName((WorkItem s) => s.Estimate);
        public static string CompletedPropName = Utils.GetPropertyName((WorkItem s) => s.Completed);

        // Returns true if this item type is the leaf node of the parent-child hierarchy
        protected override bool IsLeafItem { get { return true; } }

        public override string ParentKeyProperty
        {
            get { return Utils.GetPropertyName((WorkItem s) => s.ParentBacklogItemKey); }
        }

        public override string OwnerAssignedProperty
        {
            get { return Utils.GetPropertyName((StoreItem s) => s.AssignedTo); }
        }

        protected override void ItemPropertyChanged(NotificationArgs changeArgs)
        {
            ItemProperty changedProperty = changeArgs.ChangedProperty;

            switch(changeArgs.ChangeSource)
            {
                case HierarchicalChangeSource.ParentItem:
                    if (IsPropNameTitle(changedProperty.PublicPropName))
                    {
                        NotifyPropertyChanged(() => ParentBacklogItemLongName);
                    }
                    break;
            }
        }
    }

    //------------------------------------------------------------------------------------
    /// <summary>
    ///  BacklogItem
    /// </summary>
    //------------------------------------------------------------------------------------
    public partial class BacklogItem
    {
        public override string ParentKeyProperty
        {
            get { return Utils.GetPropertyName((BacklogItem s) => s.ParentScenarioItemKey); }
        }

        private StoreItemCollection<WorkItem> m_workItems;
        private StoreItemCollection<WorkItem> m_currentWorkItems;

        protected override IStoreItemList GetChildItems()
        {
            return m_workItems;
        }

        public StoreItemCollection<WorkItem> WorkItems
        {
            get { return m_workItems; }
        }

        public AsyncObservableCollection<WorkItem> CurrentWorkItems
        {
            get { return m_currentWorkItems; }
        }

        public override string OwnerKeyProperty
        {
            get { return Utils.GetPropertyName((BacklogItem b) => b.ScrumTeamKey); }
        }

        protected override void ItemPropertyChanged(NotificationArgs changeArgs)
        {
            m_totalWorkScheduled = -1;
            m_totalWorkCompleted = -1;
            m_totalWorkRemaining = -1;
            NotifyPropertyChanged(() => TotalWorkScheduled);
            NotifyPropertyChanged(() => TotalWorkCompleted);
            NotifyPropertyChanged(() => TotalWorkRemaining);
        }
    }

    //------------------------------------------------------------------------------------
    /// <summary>
    ///  ScenarioItem
    /// </summary>
    //------------------------------------------------------------------------------------
    public partial class ScenarioItem
    {
        private StoreItemCollection<BacklogItem> m_backlogItems;

        public ScenarioItem()
        {
            m_backlogItems = new StoreItemCollection<BacklogItem>();
        }

        protected override IStoreItemList GetChildItems()
        {
            return m_backlogItems;
        }

        public override string ParentKeyProperty
        {
            get { return Utils.GetPropertyName((ScenarioItem b) => b.ParentExperienceItemKey); }
        }

        public StoreItemCollection<BacklogItem> BacklogItems
        {
            get
            {
                return m_backlogItems;
            }
        }
    }

    //------------------------------------------------------------------------------------
    /// <summary>
    ///  ExperienceItem
    /// </summary>
    //------------------------------------------------------------------------------------
    public partial class ExperienceItem
    {
        private StoreItemCollection<ScenarioItem> m_scenarioItems;

        public ExperienceItem()
        {
            m_scenarioItems = new StoreItemCollection<ScenarioItem>();
        }

        protected override IStoreItemList GetChildItems()
        {
            return m_scenarioItems;
        }

        public AsyncObservableCollection<ScenarioItem> ScenarioItems
        {
            get
            {
                return m_scenarioItems;
            }
        }


    }

    //------------------------------------------------------------------------------------
    /// <summary>
    ///  ScrumTeamItem
    /// </summary>
    //------------------------------------------------------------------------------------
    public partial class ScrumTeamItem
    {
        private StoreItemCollection<BacklogItem> m_ownedBacklogItems;
        private StoreItemCollection<GroupMemberItem> m_scrumTeamMembers;

        public ScrumTeamItem()
        {
            m_ownedBacklogItems = new StoreItemCollection<BacklogItem>();
        }

        protected override IStoreItemList GetOwnedItems()
        {
            return m_ownedBacklogItems;
        }

        public StoreItemCollection<BacklogItem> OwnedBacklogItems
        {
            get { return m_ownedBacklogItems; }
        }

        public StoreItemCollection<GroupMemberItem> Members
        {
            get
            {
                if (m_scrumTeamMembers == null)
                {
                    m_scrumTeamMembers = new StoreItemCollection<GroupMemberItem>();
                    foreach (BacklogItem backlogItem in m_ownedBacklogItems)
                    {
                        foreach (WorkItem workItem in backlogItem.WorkItems)
                        {
                            GroupMemberItem workItemMember = workItem.AssignedToGroupMember;
                            if (workItemMember != null && !m_scrumTeamMembers.Contains(workItemMember))
                            {
                                m_scrumTeamMembers.Add(workItemMember);
                            }
                        }
                    }
                }

                return m_scrumTeamMembers;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// This notification will bubble up if any owned item or a descendant is changed.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected override void ItemPropertyChanged(NotificationArgs changeArgs)
        {
            m_scrumTeamMembers = null;
            NotifyPropertyChanged(() => Members);
        }
    }

    public partial class GroupMemberItem
    {
        private StoreItemCollection<WorkItem> m_workItems;
        private StoreItemCollection<WorkItem> m_notStartedWorkItems;
        private StoreItemCollection<WorkItem> m_inProgressWorkItems;
        private StoreItemCollection<WorkItem> m_completedWorkItems;
        private StoreItemCollection<OffTimeItem> m_offTimeItems;
        private StoreItemCollection<BacklogItem> m_backlogItems;
        private StoreItemCollection<BacklogItem> m_activeBacklogItems;
        private StoreItemCollection<ScrumTeamItem> m_scrumTeams;

        public GroupMemberItem()
        {
            m_workItems = new StoreItemCollection<WorkItem>();
            m_offTimeItems = new StoreItemCollection<OffTimeItem>();
        }

        // Owns: WorkItems
        protected override IStoreItemList GetOwnedItems()
        {
            return m_workItems;
        }

        // Parent of: OffTimeItems
        protected override IStoreItemList GetChildItems()
        {
            return m_offTimeItems;
        }

        public StoreItemCollection<OffTimeItem> OffTimeItems
        {
            get { return m_offTimeItems; }
        }

        public StoreItemCollection<WorkItem> WorkItems
        {
            get { return m_workItems; }
        }

        public StoreItemCollection<BacklogItem> BacklogItems
        {
            get
            {
                if (m_backlogItems == null)
                {
                    BuildBacklogItems();
                }

                return m_backlogItems;
            }
        }

        public StoreItemCollection<BacklogItem> ActiveBacklogItems
        {
            get
            {
                if (m_activeBacklogItems == null)
                {
                    BuildBacklogItems();
                }

                return m_activeBacklogItems;
            }
        }

        private void BuildBacklogItems()
        {
            m_backlogItems = new StoreItemCollection<BacklogItem>();
            m_activeBacklogItems = new StoreItemCollection<BacklogItem>();
            foreach (WorkItem workItem in m_workItems)
            {
                BacklogItem backlogItem = workItem.ParentBacklogItem;
                if (backlogItem != null)
                {
                    if (!m_backlogItems.Contains(backlogItem))
                    {
                        m_backlogItems.Add(backlogItem);

                        if (backlogItem.IsActive)
                        {
                            m_activeBacklogItems.Add(backlogItem);
                        }
                    }
                }
            }
        }

        public StoreItemCollection<WorkItem> NotStartedWorkItems
        {
            get
            {
                if (m_notStartedWorkItems == null)
                {
                    BuildWorkItemCollections();
                }

                return m_notStartedWorkItems;
            }
        }

        public StoreItemCollection<WorkItem> InProgressWorkItems
        {
            get
            {
                if (m_inProgressWorkItems == null)
                {
                    BuildWorkItemCollections();
                }

                return m_inProgressWorkItems;
            }
        }

        public StoreItemCollection<WorkItem> CompletedWorkItems
        {
            get
            {
                if (m_completedWorkItems == null)
                {
                    BuildWorkItemCollections();
                }

                return m_completedWorkItems;
            }
        }

        private void BuildWorkItemCollections()
        {
            m_notStartedWorkItems = new StoreItemCollection<WorkItem>();
            m_inProgressWorkItems = new StoreItemCollection<WorkItem>();
            m_completedWorkItems = new StoreItemCollection<WorkItem>();

            foreach (WorkItem workItem in WorkItems)
            {
                if (workItem.ItemStatus == WorkItemStates.NotStarted)
                {
                    m_notStartedWorkItems.Add(workItem);
                }
                else if (workItem.ItemStatus == WorkItemStates.InProgress)
                {
                    m_inProgressWorkItems.Add(workItem);
                }
                else if (workItem.ItemStatus == WorkItemStates.Completed)
                {
                    m_completedWorkItems.Add(workItem);
                }
            }
        }

        protected override void ItemPropertyChanged(NotificationArgs changeArgs)
        {
            switch (changeArgs.ChangeSource)
            {
                // Notification received if any child OffTimeItem changes
                case HierarchicalChangeSource.ChildItem:
                    m_totalOffDays = null;
                    m_currentTrainHoursRemaining = -1;
                    NotifyPropertyChanged(() => TotalOffDays);
                    NotifyPropertyChanged(() => CurrentTrainHoursRemaining);
                    break;

                // Notification received if any owned WorkItem changes
                case HierarchicalChangeSource.OwnedItem:
                    //if (changeArgs.ChangedProperty.PublicPropName == WorkItem.SubstatusPropName ||
                    //    changeArgs.ChangedProperty.PublicPropName == WorkItem.EstimatePropName ||
                    //    changeArgs.ChangedProperty.PublicPropName == WorkItem.CompletedPropName)
                    //{
                    //}

                    m_notStartedWorkItems = null;
                    m_inProgressWorkItems = null;
                    m_completedWorkItems = null;
                    m_totalWorkRemaining = -1;
                    m_totalWorkCompleted = -1;
                    m_currentTrainWorkRemaining = -1;
                    NotifyPropertyChanged(() => NotStartedWorkItems);
                    NotifyPropertyChanged(() => InProgressWorkItems);
                    NotifyPropertyChanged(() => CompletedWorkItems);
                    NotifyPropertyChanged(() => TotalWorkRemaining);
                    NotifyPropertyChanged(() => TotalWorkCompleted);
                    NotifyPropertyChanged(() => CurrentTrainWorkRemaining);
                    NotifyPropertyChanged(() => DisplayHoursOfTotal);
                    NotifyPropertyChanged(() => WorkFillWidth);
                    NotifyPropertyChanged(() => WorkFillColor);
                    m_backlogItems = null;
                    m_scrumTeams = null;
                    NotifyPropertyChanged(() => BacklogItems);
                    NotifyPropertyChanged(() => MemberScrumTeams);
                    break;
            }
        }

        public AsyncObservableCollection<ScrumTeamItem> MemberScrumTeams
        {
            get
            {
                if (m_scrumTeams == null)
                {
                    m_scrumTeams = new StoreItemCollection<ScrumTeamItem>();
                    foreach (BacklogItem backlogItem in BacklogItems)
                    {
                        ScrumTeamItem scrumTeam = backlogItem.ScrumTeamItem;
                        if (scrumTeam != null && !m_scrumTeams.Contains(scrumTeam))
                        {
                            m_scrumTeams.Add(scrumTeam);
                        }
                    }
                }

                return m_scrumTeams;
            }
        }
    }

    public partial class OffTimeItem
    {
        public override string ParentKeyProperty
        {
            get { return Utils.GetPropertyName((OffTimeItem s) => s.ParentItemKey); }
        }

    }
}
