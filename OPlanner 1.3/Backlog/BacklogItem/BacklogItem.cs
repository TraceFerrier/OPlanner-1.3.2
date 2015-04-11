using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlannerNameSpace
{
    public enum MoveToNextTrainStatus
    {
        NotAssignedToTrain,
        AssignedToFutureTrain,
        NoOutstandingWorkRemaining,
        Movable,
    }

    public partial class BacklogItem : ForecastableItem, IDependsOnPillarItem
    {
        public BacklogItem()
        {
            TrainCommitmentUpdateReceived = false;
            m_workItems = new StoreItemCollection<WorkItem>();
            m_currentWorkItems = new StoreItemCollection<WorkItem>();
            ProductGroupManager.Instance.DefaultSpecTeamNameChanged += ProductGroupManager_DefaultSpecTeamNameChanged;
        }

        static SolidColorBrush m_reviewPagesBackgroundColor;
        static SolidColorBrush ReviewPagesBackgroundBrush
        {
            get
            {
                if (m_reviewPagesBackgroundColor == null)
                {
                    m_reviewPagesBackgroundColor = new SolidColorBrush();
                    m_reviewPagesBackgroundColor.Color = ReviewPagesBackgroundColor;
                }

                return m_reviewPagesBackgroundColor;
            }
        }

        public static Color ReviewPagesBackgroundColor
        {
            get { return Color.FromRgb(255, 215, 230); }
        }

        static TrainItem m_currentBacklogTrain;
        public static TrainItem CurrentBacklogTrain
        {
            get
            {
                if (m_currentBacklogTrain == null)
                {
                    return TrainManager.Instance.CurrentTrain;
                }
                else
                {
                    return m_currentBacklogTrain;
                }
            }

            set
            {
                m_currentBacklogTrain = value;
            }
        }

        static AvailableViews m_currentBacklogView;

        public static AvailableViews CurrentBacklogView
        {
            get { return BacklogItem.m_currentBacklogView; }
            set { BacklogItem.m_currentBacklogView = value; }
        }

        public override ItemTypeID StoreItemType { get { return ItemTypeID.BacklogItem; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        private TrainItem m_parentTrainItem;
        private bool TrainCommitmentUpdateReceived { get; set; }

        public CommitmentSettingValues PreviousCommitmentStatus { get; set; }
        public DateTime PreviousLandingDate { get; set; }

        public bool IsCommitmentSelectionEnabled
        {
            get { return IsActive; }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an enum indicating the completion status of this item, in relation to the
        /// train in which it was committed to be completed for.
        /// </summary>
        //------------------------------------------------------------------------------------
        public TrainCommitmentStatusValue TrainCommitmentStatus
        {
            get
            {
                // Take care of items are are inactive, completed, or set to uncommitted
                if (!IsActive || CommitmentSetting == CommitmentSettingValues.Completed)
                {
                    return TrainCommitmentStatusValue.Completed;
                }

                if (CommitmentSetting == CommitmentSettingValues.Uncommitted)
                {
                    return TrainCommitmentStatusValue.NotCommitted;
                }

                if (!Globals.ItemManager.IsDiscoveryComplete && !TrainCommitmentUpdateReceived)
                {
                    //return TrainCommitmentStatusValue.NotCalculated;
                }

                CommitmentSettingValues commitmentSetting = CommitmentSetting;
                TrainItem commitmentTrain = CurrentBacklogTrain == null ? TrainManager.Instance.CurrentTrain : CurrentBacklogTrain;

                // We know the item is set to Committed, and is active
                bool isCommittedToTrain = IsCommittedToTrain(commitmentTrain);
                if (!isCommittedToTrain)
                {
                    return TrainCommitmentStatusValue.CommittedNotApproved;
                }

                // Item is committed to the train the user is now viewing - determine if
                // it's been moved from the train it was originally committed to
                if (ParentTrainItem.EndDate > commitmentTrain.EndDate)
                {
                    return TrainCommitmentStatusValue.MovedToLaterTrain;
                }
                else if (ParentTrainItem.EndDate < commitmentTrain.EndDate)
                {
                    return TrainCommitmentStatusValue.MovedToEarlierTrain;
                }

                // The item is currently assigned to the train that the user is currently
                // reviewing.  If this is the current train, determine whether the item is
                // projected to finish on time.
                if (BacklogItem.CurrentBacklogTrain.TimeFrame == TrainTimeFrame.Current)
                {
                    if (commitmentSetting == CommitmentSettingValues.Committed)
                    {
                        return TrainCommitmentStatusValue.CommittedAndApproved;
                    }

                    else if (LandingDateStatus == PlannerNameSpace.LandingDateStatus.OnTrack)
                    {
                        return TrainCommitmentStatusValue.OnTrack;
                    }
                    else
                    {
                        return TrainCommitmentStatusValue.ProjectedPastDue;
                    }
                }

                // If the train being reviewed is in the past, then the item is past due
                else if (BacklogItem.CurrentBacklogTrain.TimeFrame == TrainTimeFrame.Past)
                {
                    return TrainCommitmentStatusValue.PastDue;
                }

                // If the train being review is in the future, then so note
                else
                {
                    return TrainCommitmentStatusValue.AssignedToFutureTrain;
                }

            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a string representing this item's current TrainCommitmentStatus.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string TrainCommitmentStatusText
        {
            get
            {
                return CommitmentStatusManager.GetStatusText(TrainCommitmentStatus);
            }
        }

        public string TrainCommitmentRecapStatusText
        {
            get
            {
                switch (TrainCommitmentStatus)
                {
                    case TrainCommitmentStatusValue.Completed:
                        return PlannerContent.TrainCommitmentCompleted;

                    case TrainCommitmentStatusValue.MovedToLaterTrain:
                        return PlannerContent.RecapTrainCommitmentChangedToLaterTrain;

                    default:
                        return TrainCommitmentStatusText;
                }
            }
        }

        public Brush TrainCommitmentRecapStatusColor
        {
            get
            {
                switch (TrainCommitmentStatus)
                {
                    case TrainCommitmentStatusValue.Completed:
                        return Brushes.LightGreen;

                    case TrainCommitmentStatusValue.MovedToLaterTrain:
                        return Globals.LightRedBrush;

                    default:
                        return Brushes.LightYellow;
                }
            }
        }

        public string TrainCommitmentNextTrainStatusText
        {
            get
            {
                bool isCommittedToTrain = IsCommittedToTrain(CurrentBacklogTrain);
                if (isCommittedToTrain)
                {
                    if (IsCommittedToPreviousTrain(TrainManager.Instance.GetNextTrain(CurrentBacklogTrain)))
                    {
                        return PlannerContent.TrainCommitmentCarriedOverFromPreviousTrain;
                    }
                }

                return PlannerContent.TrainCommitmentNewCommitment;
            }
        }

        public Brush TrainCommitmentNextTrainStatusColor
        {
            get
            {
                if (TrainCommitmentNextTrainStatusText == PlannerContent.TrainCommitmentCarriedOverFromPreviousTrain)
                {
                    return Brushes.LightYellow;
                }
                else
                {
                    return Brushes.LightGreen;
                }
            }
        }

        public Brush TrainCommitmentStatusColor
        {
            get
            {
                if (CurrentBacklogView == AvailableViews.BacklogThisTrainCommitments || CurrentBacklogView == AvailableViews.BacklogThisTrainNonCommitments)
                {
                    return ReviewPagesBackgroundBrush;
                }
                else
                {
                    return CommitmentStatusManager.GetStatusColor(TrainCommitmentStatus);
                }
            }
        }

        public override Brush LandingDateStatusColor
        {
            get
            {
                if (CurrentBacklogView == AvailableViews.BacklogLastTrainResultRecap || CurrentBacklogView == AvailableViews.BacklogThisTrainNonCommitments)
                {
                    return ReviewPagesBackgroundBrush;
                }
                else
                {
                    return base.LandingDateStatusColor;
                }
            }
        }

        public bool IsCommittedOrInProgress
        {
            get
            {
                if (CommitmentSetting == CommitmentSettingValues.In_Progress ||
                    CommitmentSetting == CommitmentSettingValues.Committed ||
                    CommitmentSetting == CommitmentSettingValues.Completed)
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsInProgressOrCompleted
        {
            get
            {
                if (CommitmentSetting == CommitmentSettingValues.In_Progress ||
                    CommitmentSetting == CommitmentSettingValues.Completed)
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsCommittedAndApproved
        {
            get
            {
                if (ParentTrainItem != TrainItem.BacklogTrainItem)
                {
                    if (IsCommittedOrInProgress)
                    {
                        if (IsCommittedToTrain(ParentTrainItem))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public CommitmentSettingValues CommitmentSetting
        {
            get
            {
                string statusText = GetStringValue(Datastore.PropNameBacklogCommitmentSetting);
                if (string.IsNullOrWhiteSpace(statusText))
                {
                    return CommitmentSettingValues.Uncommitted;
                }
                else
                {
                    // Backward-compatibility checks
                    if (statusText == "In Flight")
                    {
                        statusText = Utils.EnumToString<CommitmentSettingValues>(CommitmentSettingValues.In_Progress);
                    }
                    else if (statusText == "Not Ready")
                    {
                        statusText = Utils.EnumToString<CommitmentSettingValues>(CommitmentSettingValues.Uncommitted);
                    }
                    return Utils.StringToEnum<CommitmentSettingValues>(statusText);
                }
            }

            set
            {
                PreviousCommitmentStatus = CommitmentSetting;
                PreviousLandingDate = LandingDate;
                SetStringValue(Datastore.PropNameBacklogCommitmentSetting, Utils.EnumToString<CommitmentSettingValues>(value));
                if (value == CommitmentSettingValues.Uncommitted || value == CommitmentSettingValues.Committed)
                {
                    LandingDate = default(DateTime);
                }
                else if (value == CommitmentSettingValues.Completed && IsActive)
                {
                    LandingDate = DateTime.Today;
                    FixItem();
                }

                NotifyPropertyChanged(() => CommitmentSettingText);
                NotifyPropertyChanged(() => LandingDateStatusToolTip);
            }
        }

        public string CommitmentSettingText
        {
            get
            {
                return Utils.EnumToString<CommitmentSettingValues>(CommitmentSetting);
            }

            set
            {
                CommitmentSetting = Utils.StringToEnum<CommitmentSettingValues>(value);
            }
        }

        public static AsyncObservableCollection<string> AvailableCommitmentStatusValues
        {
            get { return Utils.GetEnumValues<CommitmentSettingValues>(true); }
        }

        public override string LandingDateText
        {
            get
            {
                if (Globals.CommitmentStatusManager.CommitmentStatusComputationComplete && CommitmentSetting == CommitmentSettingValues.In_Progress)
                {
                    if (WorkItems.Count == 0)
                    {
                        return Globals.NotScheduled;
                    }
                }

                return base.LandingDateText;
            }
        }

        public override TrainCommitmentStatusValue GetCommitmentStatus()
        {
            return TrainCommitmentStatus;
        }
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Displays UI giving the user the chance to create a new backlog item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void RequestNewBacklogItem(PillarItem parentPillarItem, TrainItem parentTrainItem, ScrumTeamItem scrumTeam = null)
        {
            BacklogItem newItem = null;

            PillarItem pillarItem = parentPillarItem;
            TrainItem trainItem = parentTrainItem;

            newItem = BacklogItem.CreateBacklogItem(pillarItem, trainItem);
            newItem.BeginSaveImmediate();

            NewBacklogItemDialog dialog = new NewBacklogItemDialog(newItem, pillarItem, trainItem);
            dialog.ShowDialog();

            if (dialog.DialogConfirmed)
            {
                newItem.Title = dialog.BacklogItemTitle;
                newItem.ParentPillarItem = dialog.SelectedPillarItem;
                newItem.ParentTrainItem = dialog.SelectedTrainItem;
                newItem.ParentSpec = dialog.SelectedSpec;
                newItem.ScrumTeamItem = scrumTeam;

                newItem.SaveImmediate();
            }
            else
            {
                newItem.CancelSaveImmediate();
            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Creates a new BacklogItem, parented to the given pillar and train.  If basedOn
        /// isn't null, properties from that item will copied to the new item.
        /// </summary>
        //------------------------------------------------------------------------------------
        private static BacklogItem CreateBacklogItem(PillarItem parentPillarItem, TrainItem parentTrainItem, string title = "New backlog item")
        {
            BacklogItem newItem = HostItemStore.Instance.CreateStoreItem<BacklogItem>(ItemTypeID.BacklogItem);

            newItem.ID = 0;
            newItem.Status = StatusValues.Active;
            newItem.ScrumTeamItem = null;
            newItem.ParentPillarItem = parentPillarItem;
            newItem.ShipCycle = Globals.OfficeCurrentShipCycle;

            if (parentTrainItem != null)
            {
                newItem.ParentTrainItem = parentTrainItem;
            }
            else
            {
                newItem.FixBy = Globals.OfficeBacklogFixBy;
            }

            newItem.Title = title;
            return newItem;
        }

        void NotifyTrainCommitmentStatusPropertyChanged()
        {
            NotifyPropertyChanged(() => TrainCommitmentStatusText);
            NotifyPropertyChanged(() => TrainCommitmentRecapStatusText);
            NotifyPropertyChanged(() => TrainCommitmentRecapStatusColor);
            NotifyPropertyChanged(() => TrainCommitmentNextTrainStatusText);
            NotifyPropertyChanged(() => TrainCommitmentNextTrainStatusColor);
            NotifyPropertyChanged(() => TrainCommitmentStatusColor);
        }

        public string ParentExperienceTitle
        {
            get
            {
                ScenarioItem scenarioItem = ParentScenarioItem;
                if (scenarioItem != null)
                {
                    ExperienceItem experienceItem = scenarioItem.ParentExperienceItem;
                    if (experienceItem != null)
                    {
                        return experienceItem.Title;
                    }
                }

                return Globals.c_None;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a string with the title of this backlog item, prefaced with the item ID,
        /// and suffixed with the title of the parent pillar.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string FullyQualifiedTitle
        {
            get
            {
                if (ParentPillarItem == null)
                {
                    return ID.ToString() + ": " + Title;
                }
                else
                {
                    return ID.ToString() + ": " + Title + " (Pillar: " + ParentPillarItem.Title + ")";
                }
            }
        }

        public AsyncObservableCollection<GroupMemberItem> AssignedDevTeamMembers
        {
            get
            {
                return GetAssignedTeamMembers(DisciplineValues.Dev);
            }

        }

        public AsyncObservableCollection<GroupMemberItem> AssignedTestTeamMembers
        {
            get
            {
                return GetAssignedTeamMembers(DisciplineValues.Test);
            }

        }

        AsyncObservableCollection<GroupMemberItem> GetAssignedTeamMembers(string discipline)
        {
            AsyncObservableCollection<GroupMemberItem> members = new AsyncObservableCollection<GroupMemberItem>();
            WorkItem[] workItems = new WorkItem[WorkItems.Count];
            WorkItems.CopyTo(workItems, 0);
            foreach (WorkItem workItem in workItems)
            {
                GroupMemberItem groupMemberItem = Globals.GroupMemberManager.GetMemberByAlias(workItem.AssignedTo);
                if (groupMemberItem != null && !members.Contains(groupMemberItem))
                {
                    if (groupMemberItem.Discipline == discipline)
                    {
                        members.Add(groupMemberItem);
                    }
                }
            }

            return members;
        }

        public bool CanBeCommitted
        {
            get
            {
                if (!StoreItem.IsRealItem(ParentTrainItem))
                {
                    return false;
                }

                if (!IsCommittedAndApproved)
                {
                    StoreSpecStatusValue storeSpecStatus = StoreSpecStatus.GetStoreSpecStatus(StoreSpecStatusText);
                    if (storeSpecStatus == StoreSpecStatusValue.ReadyForCoding || storeSpecStatus == StoreSpecStatusValue.SpecFinalized)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public override void SetDesignStatus(DesignStatus status, bool updateParent)
        {
            base.SetDesignStatus(status, updateParent);
            BacklogDesignStatus = GetDesignStatusText(status);
        }

        private string BacklogDesignStatus
        {
            get { return GetStringValue(Datastore.PropNameBacklogDesignStatus); }
            set
            {
                // Handle the case where the user has placed an unknown value in the
                // field using a different tool.  We treat unknown values as
                // NoDesignRequired, and don't change the backing value.
                string rawDesignStatus = BacklogDesignStatus;
                DesignStatus status = GetDesignStatus(rawDesignStatus);
                if (status != PlannerNameSpace.DesignStatus.DesignStatusUnknown)
                {
                    SetStringValue(Datastore.PropNameBacklogDesignStatus, value);
                }
                else if (!string.IsNullOrWhiteSpace(rawDesignStatus))
                {
                    #if DEBUG
                    Globals.ApplicationManager.WriteToEventLog(m_id.ToString() + ":" + m_title + " - Unrecognized BacklogDesignStatus");
                    #endif
                }
            }
        }

        public override void UpdateDesignStatus(bool updateParent)
        {
            string rawDesignStatus = BacklogDesignStatus;
            SetDesignStatus(GetDesignStatus(rawDesignStatus), updateParent);
        }

        public override void UpdateCompletionStatus(bool updateParent)
        {
            ItemStatusItem status = ItemStatus;
            if (Utils.StringsMatch(status.ItemStatus, BacklogItemStates.Completed))
            {
                SetCompletionStatus(PlannerNameSpace.CompletionStatus.Completed, updateParent);
            }
            else if (Utils.StringsMatch(status.ItemStatus, BacklogItemStates.InProgress))
            {
                SetCompletionStatus(PlannerNameSpace.CompletionStatus.In_Progress, updateParent);
            }
            else
            {
                SetCompletionStatus(PlannerNameSpace.CompletionStatus.Not_Started, updateParent);
            }
        }

        public override void AddItem(AsyncObservableCollection<ForecastableItem> items)
        {
            items.Add(this);
        }

        public override void AddItemWithSpecIssues(AsyncObservableCollection<ForecastableItem> items)
        {
            if (HasSpecIssue)
            {
                items.Add(this);
            }
        }

        public override void AddItemWithDesignIssues(AsyncObservableCollection<ForecastableItem> items)
        {
            if (HasDesignIssue)
            {
                items.Add(this);
            }
        }

        public override void AddItemsWithSpecOrDesignIssues(AsyncObservableCollection<ForecastableItem> items)
        {
            if (HasSpecIssue || HasDesignIssue)
            {
                items.Add(this);
            }
        }

        public bool IsAssignedToNextTrain
        {
            get
            {
                TrainItem trainItem = ParentTrainItem;
                if (trainItem != null)
                {
                    return TrainManager.Instance.IsNextTrain(trainItem);
                }

                return false;
            }
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Handles deletion of backlog items, confirming whether all child work items should
        /// be deleted as well.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override bool RequestDeleteItem(Window owner = null)
        {
            if (WorkItems.Count > 0)
            {
                DeleteBacklogItemDialog dialog = new DeleteBacklogItemDialog(this);
                dialog.ShowDialog();

                if (dialog.ShouldDelete)
                {
                    DeleteItem();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return base.RequestDeleteItem(owner);
            }
        }

        public override void DeleteItem()
        {
            List<WorkItem> workItems = WorkItems.ToList();
            foreach (WorkItem workItem in workItems)
            {
                if (workItem.IsResolvedAnyResolution)
                {
                    workItem.CloseItem();
                }
                else if (workItem.IsActive)
                {
                    workItem.DeleteItem();
                }
            }

            base.DeleteItem();
        }

        public void ShowBacklogItemEditor(int selectedTabIndex = 0)
        {
            BacklogItemEditorDialog dialog = new BacklogItemEditorDialog(this, selectedTabIndex);
            dialog.ShowDialog();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns collection of all child work items that still have unfinished work 
        /// remaining.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<WorkItem> UnfinishedWorkItems
        {
            get
            {
                AsyncObservableCollection<WorkItem> unfinishedWorkItems = new AsyncObservableCollection<WorkItem>();
                foreach (WorkItem workItem in WorkItems)
                {
                    if (workItem.WorkRemaining > 0)
                    {
                        unfinishedWorkItems.Add(workItem);
                    }
                }

                return unfinishedWorkItems;
            }
        }

        static AsyncObservableCollection<string> s_statusValues;
        public AsyncObservableCollection<string> ItemStatusValues
        {
            get
            {
                if (s_statusValues == null)
                {
                    s_statusValues = new AsyncObservableCollection<string>();
                    s_statusValues.Add(WorkItemDisplayStates.NotStarted);
                    s_statusValues.Add(WorkItemDisplayStates.InProgress);
                    s_statusValues.Add(WorkItemDisplayStates.Completed);
                    s_statusValues.Add(WorkItemDisplayStates.CompletedAndResolved);
                    s_statusValues.Add(WorkItemDisplayStates.CompletedAndClosed);
                }

                return s_statusValues;
            }
        }

        public override string GetItemStatusText()
        {
            return CommitmentSettingText;
        }

        public override CommitmentSettingValues GetCommitmentSetting()
        {
            return CommitmentSetting;
        }

        public override int GetStoryPoints()
        {
            return StoryPoints;
        }

        public string ItemStatusText
        {
            get { return ItemStatus.ItemStatus; }
        }

        public ItemStatusItem ItemStatus
        {
            get
            {
                int id = this.ID;
                TrainTimeFrame timeFrame = ParentTrainItem != null ? ParentTrainItem.TimeFrame : TrainTimeFrame.Unassigned;

                if ((TotalWorkRemaining == 0 && TotalWorkCompleted > 0) || IsFixed)
                {
                    return new ItemStatusItem(BacklogItemStates.Completed);
                }
                else if (timeFrame == TrainTimeFrame.Past)
                {
                    if (TotalWorkRemaining > 0)
                    {
                        return new ItemStatusItem(BacklogItemStates.PastDue);
                    }
                }
                else if (timeFrame == TrainTimeFrame.Current)
                {
                    if (TotalWorkCompleted > 0 && TotalWorkRemaining > 0)
                    {
                        return new ItemStatusItem(BacklogItemStates.InProgress);
                    }
                    else if (TotalWorkScheduled > 0)
                    {
                        return new ItemStatusItem(BacklogItemStates.NotStarted);
                    }
                }
                else if (timeFrame == TrainTimeFrame.Future)
                {
                    return new ItemStatusItem(BacklogItemStates.FutureWork);
                }

                else if (timeFrame == TrainTimeFrame.Unassigned)
                {
                    return new ItemStatusItem(BacklogItemStates.OnTheBacklog);
                }

                return new ItemStatusItem(BacklogItemStates.NotScheduled);
            }
        }

        public Brush StatusColor
        {
            get
            {
                switch (ItemStatus.ItemStatus)
                {
                    case BacklogItemStates.Completed:
                        switch (Status)
                        {
                            case StatusValues.Active:
                                return Brushes.LightGreen;
                            case StatusValues.Resolved:
                                return Brushes.YellowGreen;
                            case StatusValues.Closed:
                                return Brushes.Green;
                            default:
                                return Brushes.LightGreen;
                        }

                    case BacklogItemStates.InProgress:
                        return Brushes.LightYellow;

                    case BacklogItemStates.NotStarted:
                        return Brushes.Yellow;

                    case BacklogItemStates.PastDue:
                        return Brushes.Red;

                    case BacklogItemStates.FutureWork:
                        return Brushes.Plum;

                    case BacklogItemStates.PostponedToFutureTrain:
                        return Brushes.Orange;

                    case BacklogItemStates.OnTheBacklog:
                        return Brushes.Cornsilk;

                    case BacklogItemStates.NotScheduled:
                        return Brushes.LightPink;

                    default:
                        return Brushes.LightGray;
                }
            }
        }

        public Brush StatusTextColor
        {
            get
            {
                switch (ItemStatus.ItemStatus)
                {
                    case BacklogItemStates.Completed:
                        return Brushes.Black;
                    case BacklogItemStates.InProgress:
                        return Brushes.Black;
                    case BacklogItemStates.NotStarted:
                        return Brushes.Black;
                    case BacklogItemStates.PastDue:
                        return Brushes.Black;
                    case BacklogItemStates.FutureWork:
                        return Brushes.Black;
                    case BacklogItemStates.OnTheBacklog:
                        return Brushes.Black;
                    case BacklogItemStates.NotScheduled:
                        return Brushes.Black;
                    default:
                        return Brushes.Black;
                }
            }
        }

        public override string Status
        {
            get
            {
                return base.Status;
            }

            set
            {
                base.Status = value;
                NotifyPropertyChanged(() => StatusTextColor);
                NotifyPropertyChanged(() => StatusColor);
                NotifyPropertyChanged(() => ItemStatus);
                NotifyPropertyChanged(() => ItemStatusText);
                NotifyPropertyChanged(() => ResolutionStatusColor);
            }
        }


        public override TrainItem GetParentTrainItem()
        {
            return ParentTrainItem;
        }

        public override ScenarioItem GetParentScenarioItem()
        {
            return ParentScenarioItem;
        }

        public override ScrumTeamItem GetParentFeatureTeamItem()
        {
            return ScrumTeamItem;
        }

        public AsyncObservableCollection<ScrumTeamItem> ValidScrumTeams
        {
            get
            {
                return Globals.ScrumTeamManager.GetScrumTeams(ParentPillarItem);
            }
        }

        public void ClearCommittedTrain(TrainItem train)
        {
            int idx = GetCommittedTrainIndex(train);
            if (idx >= 0)
            {
                CommittedToTrains = Utils.ClearSubstring(CommittedToTrains, idx, '^');
                CommittedToApprovers = Utils.ClearSubstring(CommittedToApprovers, idx, '^');
                CommittedToDates = Utils.ClearSubstring(CommittedToDates, idx, '^');
            }
        }

        private string CommittedToTrains
        {
            get { return GetStringValue(Datastore.PropNameCommittedToTrains); }
            set { SetStringValue(Datastore.PropNameCommittedToTrains, value); }
        }

        private string CommittedToApprovers
        {
            get { return GetStringValue(Datastore.PropNameCommittedToApprovers); }
            set { SetStringValue(Datastore.PropNameCommittedToApprovers, value); }
        }

        private string CommittedToDates
        {
            get { return GetStringValue(Datastore.PropNameCommittedToDates); }
            set { SetStringValue(Datastore.PropNameCommittedToDates, value); }
        }

        void ValidateAndFixUpCommitmentValues()
        {
            string committedTrains = CommittedToTrains;
            if (!string.IsNullOrWhiteSpace(committedTrains))
            {
                string firstTrainTitle = Utils.GetSubstring(committedTrains, 0, '^');
                TrainItem validTrain = TrainManager.Instance.GetTrainByTitle(firstTrainTitle);
                if (validTrain == null)
                {
                    CommittedToTrains = null;
                    CommittedToApprovers = null;
                    CommittedToDates = null;
                }
            }
        }

        public void SetCommittedToTrain(TrainItem train)
        {
            ValidateAndFixUpCommitmentValues();

            int idx = GetCommittedTrainIndex(train);
            CommittedToTrains = Utils.SetSubstring(CommittedToTrains, train.Title, idx, '^');
            CommittedToApprovers = Utils.SetSubstring(CommittedToApprovers, Globals.ApplicationManager.CurrentUserAlias, idx, '^');
            CommittedToDates = Utils.SetSubstring(CommittedToDates, DateTime.Now.ToShortDateString(), idx, '^');

            if (CommitmentSetting == CommitmentSettingValues.Uncommitted)
            {
                CommitmentSetting = CommitmentSettingValues.Committed;
            }

            NotifyTrainCommitmentStatusPropertyChanged();
        }

        public AsyncObservableCollection<TrainItem> GetCommittedToTrains()
        {
            AsyncObservableCollection<TrainItem> trains = new AsyncObservableCollection<TrainItem>();
            string committedTrains = CommittedToTrains;
            int idx = 0;
            string trainTitle = null;
            do
            {
                trainTitle = Utils.GetSubstring(committedTrains, idx, '^');
                if (trainTitle != null)
                {
                    TrainItem train = TrainManager.Instance.GetTrainByTitle(trainTitle);
                    if (train != null)
                    {
                        trains.Add(train);
                    }
                }
                idx++;

            } while (trainTitle != null);

            return trains;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if this item has been formally approved and committed to the given
        /// train.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsCommittedToTrain(TrainItem train)
        {
            return GetCommittedTrainIndex(train) >= 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if this item has been committed and approved for any train previous
        /// to the given train.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsCommittedToPreviousTrain(TrainItem train)
        {
            AsyncObservableCollection<TrainItem> committedTrains = GetCommittedToTrains();
            foreach (TrainItem committedTrain in committedTrains)
            {
                if (committedTrain.EndDate < train.EndDate)
                {
                    return true;
                }
            }

            return false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// If this item has been approved and committed to the given train, the date on which
        /// the approval was made is returned. If not, null is returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public Nullable<DateTime> GetCommittedToTrainDate(TrainItem train)
        {
            int idx = GetCommittedTrainIndex(train);
            if (idx < 0)
            {
                return null;
            }

            string committedDateText = Utils.GetSubstring(CommittedToDates, idx, '^');
            return Utils.GetDateTimeValue(committedDateText);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// If this item has been approved and committed to the given train, the alias of the
        /// approving group member is returned. If not, null is returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string GetCommittedToTrainApprover(TrainItem train)
        {
            int idx = GetCommittedTrainIndex(train);
            if (idx < 0)
            {
                return null;
            }

            return Utils.GetSubstring(CommittedToApprovers, idx, '^');
        }

        int GetCommittedTrainIndex(TrainItem train)
        {
            string requestTrainTitle = train.Title;
            return Utils.FindSubstring(CommittedToTrains, requestTrainTitle, '^');
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the TrainItem that this backlog item is assigned to.
        /// </summary>
        //------------------------------------------------------------------------------------
        public TrainItem ParentTrainItem
        {
            get
            {
                if (m_parentTrainItem == null)
                {
                    m_parentTrainItem = TrainManager.Instance.FindTrain(GetEffectiveShipCycle(ShipCycle), FixBy);
                    if (m_parentTrainItem == null)
                    {
                        m_parentTrainItem = GetDummyItem<TrainItem>(DummyItemType.NoneType);
                    }
                }

                return m_parentTrainItem;
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                TrainItem currentTrain = m_parentTrainItem;
                if (currentTrain == null || currentTrain.IsNoneItem)
                {
                    currentTrain = null;
                }

                TrainItem proposedTrainItem = value;
                if (currentTrain == null || proposedTrainItem.StoreKey != currentTrain.StoreKey)
                {
                    m_parentTrainItem = value;

                    ShipCycle = m_parentTrainItem.TrainShipCycle;
                    FixBy = m_parentTrainItem.TrainFixBy;
                    NotifyPropertyChangedByName();

                    foreach (WorkItem workItem in WorkItems)
                    {
                        workItem.NotifyParentTrainChanged();
                    }
                }
            }
        }

        public ScrumTeamItem ScrumTeamItem
        {
            get
            {
                ScrumTeamItem scrumTeamItem = Globals.ItemManager.GetItem<ScrumTeamItem>(ScrumTeamKey);
                if (scrumTeamItem == null)
                {
                    return ScrumTeamItem.GetDummyNoneTeam();
                }

                return scrumTeamItem;
            }

            set
            {
                if (value == null || value.IsDummyItem)
                {
                    ScrumTeamKey = null;
                }
                
                else
                {
                    ScrumTeamItem scrumTeamItem = value;

                    if (scrumTeamItem != null)
                    {
                        ScrumTeamKey = scrumTeamItem.StoreKey;
                    }
                    else
                    {
                        ScrumTeamKey = null;
                    }

                    m_parentTrainItem = null;
                }
            }
        }

        public string ScrumTeamKey
        {
            get
            {
                return GetStringValue(Datastore.PropNameBacklogScrumTeamKey);
            }

            set
            {
                SetStringValue(Datastore.PropNameBacklogScrumTeamKey, value);
                NotifyPropertyChanged(() => ScrumTeamItem);
                NotifyPropertyChanged(() => ScrumTeamMembers);
                NotifyPropertyChanged(() => ScrumTeamName);
            }
        }


        public AsyncObservableCollection<ScenarioItem> AvailableScenarios
        {
            get { return Globals.FilterManager.GetScenarioItemsSortedByPillar(DummyItemType.NoneType); }
        }

        public string ParentScenarioName
        {
            get
            {
                if (ParentScenarioItem == null)
                {
                    return null;
                }

                return ParentScenarioItem.Title;
            }
        }

        public ScenarioItem ParentScenarioItem
        {
            get
            {
                string key = ParentScenarioItemKey;
                if (string.IsNullOrWhiteSpace(key))
                {
                    return StoreItem.GetDummyItem<ScenarioItem>(DummyItemType.NoneType);
                }

                return Globals.ItemManager.GetItem<ScenarioItem>(ParentScenarioItemKey);
            }

            set
            {
                ScenarioItem oldParent = ParentScenarioItem;

                if (value == null || value.IsDummyItem)
                {
                    ParentScenarioItemKey = null;
                }
                else
                {
                    ParentScenarioItemKey = value.StoreKey;
                }

                if (oldParent != null)
                {
                    oldParent.NotifyPropertyChanged(() => oldParent.TotalWorkRemainingDisplay);
                }

                if (ParentScenarioItem != null)
                {
                    ParentScenarioItem.NotifyPropertyChanged(() => ParentScenarioItem.TotalWorkRemainingDisplay);
                }


            }
        }

        public override ForecastableItem ParentForecastableItem
        {
            get { return ParentScenarioItem; }
        }

        public string ParentScenarioItemKey
        {
            get
            {
                return GetStringValue(Datastore.PropNameParentScenarioItemKey);
            }

            set
            {
                SetStringValue(Datastore.PropNameParentScenarioItemKey, value);

                // Update all properties dependent on this key
                NotifyPropertyChanged(() => ParentScenarioItem);
                NotifyPropertyChanged(() => ParentExperienceTitle);
            }
        }

        public AsyncObservableCollection<GroupMemberItem> ScrumTeamMembers
        {
            get
            {
                return null;// Globals.GlobalItemCache.GetScrumTeamMembers(ScrumTeamItem);
            }
        }

        public string ScrumTeamName
        {
            get
            {
                if (ScrumTeamItem == null)
                {
                    return FeatureTeamNotAllowedSelection;
                }

                return ScrumTeamItem.Title;
            }
        }

        public string TrainName
        {
            get { return ParentTrainItem == null ? Globals.c_None : ParentTrainItem.Title; }
            set { ; }
        }

        public int StoryPoints
        {
            get { return GetIntValue(Datastore.PropNameStoryPoints); }
            set { SetIntValue(Datastore.PropNameStoryPoints, value); }
        }

        private int m_totalWorkScheduled = -1;
        public int TotalWorkScheduled
        {
            get
            {
                if (m_totalWorkScheduled < 0)
                {
                    CalculateTotalWorkStatistics();
                }

                return m_totalWorkScheduled;
            }
       }

        private int m_totalWorkCompleted = -1;
        public int TotalWorkCompleted
        {
            get
            {
                if (m_totalWorkCompleted < 0)
                {
                    CalculateTotalWorkStatistics();
                }

                return m_totalWorkCompleted;
            }
        }

        private int m_totalWorkRemaining = -1;
        public int TotalWorkRemaining
        {
            get
            {
                if (m_totalWorkRemaining < 0)
                {
                    CalculateTotalWorkStatistics();
                }

                return m_totalWorkRemaining;
            }
        }

        private void CalculateTotalWorkStatistics()
        {
            m_totalWorkScheduled = 0;
            m_totalWorkCompleted = 0;
            m_totalWorkRemaining = 0;
            foreach (WorkItem workItem in WorkItems)
            {
                m_totalWorkScheduled += workItem.Estimate;
                m_totalWorkCompleted += workItem.Completed;
                m_totalWorkRemaining += workItem.WorkRemaining;
            }

            NotifyPropertyChanged(() => ItemStatusText);
            NotifyPropertyChanged(() => StatusColor);
            NotifyPropertyChanged(() => StatusTextColor);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the total work remaining for all this item's work items, without regard
        /// to commitment status.
        /// </summary>
        //------------------------------------------------------------------------------------
        public int TotalWorkAvailable
        {
            get
            {
                int totalWorkRemaining = 0;
                foreach (WorkItem workItem in WorkItems)
                {
                    totalWorkRemaining += workItem.WorkRemaining;
                }

                return totalWorkRemaining;
            }
        }

        public Brush SummaryViewBackgroundColor { get; set; }

        public Brush WorkRemainingColor
        {
            get
            {
                if (ParentTrainItem == null)
                {
                    return Brushes.Red;
                }

                DateTime startDate = ParentTrainItem.StartDate;
                if (startDate < DateTime.Today)
                {
                    startDate = DateTime.Today;
                }

                int workingHoursAvailable = Utils.GetNetWorkingHours(startDate, ParentTrainItem.EndDate);
                if (TotalWorkRemaining > workingHoursAvailable)
                {
                    return Brushes.Red;
                }
                else
                {
                    return Brushes.Green;
                }
            }
        }

        public Brush WorkRemainingTextColor
        {
            get
            {
                return Brushes.Black;
            }
        }

        public string PMOwnerDisplayName
        {
            get
            {
                GroupMemberItem member = Globals.GroupMemberManager.GetMemberByAlias(PMOwner);
                if (member != null)
                {
                    return member.DisplayName;
                }

                return Globals.c_None;
            }
        }

        public string PMOwner
        {
            get { return GetStringValue(Datastore.PropNamePM_Owner); }
            set 
            {
                SetStringValue(Datastore.PropNamePM_Owner, value);
                NotifyPropertyChanged(() => PMOwnerDisplayName);
            }
        }

        public string TestOwner
        {
            get { return GetStringValue(Datastore.PropNameTest_Owner); }
            set { SetStringValue(Datastore.PropNameTest_Owner, value); }
        }

        public string DevOwner
        {
            get { return GetStringValue(Datastore.PropNameDev_Owner); }
            set { SetStringValue(Datastore.PropNameDev_Owner, value); }
        }

        public void RequestClose()
        {
            string status = this.Status;
            if (status == StatusValues.Closed)
            {
                UserMessage.Show("This backlog item is already closed.");
                return;
            }

            int completedHours = this.TotalWorkCompleted;
            int workRemaining = this.TotalWorkRemaining;

            if (completedHours == 0 || workRemaining > 0)
            {
                UserMessage.Show("Only Backlog Items that have work completed, and no work remaining, can be closed as 'completed'.");
                return;
            }

            this.FixItem();
        }

        public MoveToNextTrainStatus CanMoveToNextTrain
        {
            get
            {
                TrainItem backlogItemTrain = this.ParentTrainItem;
                if (!StoreItem.IsRealItem(backlogItemTrain))
                {
                    return MoveToNextTrainStatus.NotAssignedToTrain;
                }

                if (backlogItemTrain.TimeFrame == TrainTimeFrame.Future)
                {
                    return MoveToNextTrainStatus.AssignedToFutureTrain;
                }

                if (TotalWorkRemaining == 0)
                {
                    return MoveToNextTrainStatus.NoOutstandingWorkRemaining;
                }

                return MoveToNextTrainStatus.Movable;
            }
        }

        public Visibility IsStandardIssue
        {
            get
            {
                return IsPostMortemIssueVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public Visibility IsPostMortemIssueVisibility
        {
            get
            {
                if (!IsPostMortemIssue)
                {
                    return Visibility.Hidden;
                }

                return Visibility.Visible;
            }
        }

        public bool IsPostMortemIssue
        {
            get
            {
                if (PostMortemStatus == Globals.c_NotSet)
                {
                    return false;
                }

                return true;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a ContextMenu suitable for the options available for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void PopulateContextMenu(Window ownerWindow, ContextMenu menu)
        {
            AddContextMenuItem(menu, "Edit...", "Edit.png", Edit_Click);
            AddContextMenuItem(menu, "Add or Edit Attached Files...", "FileOpen.png", EditAttachedFiles_Click);
            AddContextMenuItem(menu, "Show Work Items...", "TreeView.png", ShowWorkItems_Click);
            AddContextMenuItem(menu, "New Work Item...", "NewWorkItem.png", NewWorkItem_Click);

            if (!IsClosed)
            {
                AddContextMenuItem(menu, "Close as Completed...", "Completed.png", Close_Click);
            }

            if (IsClosed || IsResolved)
            {
                AddContextMenuItem(menu, "Re-activate", "Reactivate.png", Reactivate_Click);
            }

            AddContextMenuItem(menu, "Delete...", "Delete.png", Delete_Click);

            if (CanBeCommitted)
            {
                AddContextMenuItem(menu, "Commit to the Assigned Train...", "CompletionStatus.png", CommitToAssignedTrain_Click);
            }

        }


        void CommitToAssignedTrain_Click(object sender, RoutedEventArgs e)
        {
            TrainItem currentTrain = ParentTrainItem;
            if (StoreItem.IsRealItem(currentTrain))
            {
                AddItemToCommitmentsDialog dialog = new AddItemToCommitmentsDialog(this);
                dialog.ShowDialog();
                if (dialog.Confirmed)
                {
                    SetCommittedToTrain(currentTrain);
                }
            }
        }

        void Edit_Click(object sender, RoutedEventArgs e)
        {
            ShowBacklogItemEditor();
        }

        void EditAttachedFiles_Click(object sender, RoutedEventArgs e)
        {
            const int AttachedFilesTabIndex = 3;
            ShowBacklogItemEditor(AttachedFilesTabIndex);
        }

        void ShowWorkItems_Click(object sender, RoutedEventArgs e)
        {
            ShowWorkItemsWindow();
        }

        void NewWorkItem_Click(object sender, RoutedEventArgs e)
        {
            WorkItem newWorkItem = WorkItem.CreateWorkItem(this);
            newWorkItem.SaveNewItem();
        }

        void Close_Click(object sender, RoutedEventArgs e)
        {
            RequestClose();
        }

        void Reactivate_Click(object sender, RoutedEventArgs e)
        {
            ActivateItem();
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            RequestDeleteItem();
        }

        BacklogWorkItemsWindow WorkItemsWindow;
        void ShowWorkItemsWindow()
        {
            if (WorkItemsWindow != null)
            {
                if (!WorkItemsWindow.IsVisible)
                {
                    WorkItemsWindow.Show();
                }
                WorkItemsWindow.BringIntoView();
            }
            else
            {
                WorkItemsWindow = new BacklogWorkItemsWindow(this);
                WorkItemsWindow.Closed += WorkItemsWindow_Closed;
                WorkItemsWindow.Show();
            }
        }

        void WorkItemsWindow_Closed(object sender, EventArgs e)
        {
            WorkItemsWindow = null;
        }

    }
}
