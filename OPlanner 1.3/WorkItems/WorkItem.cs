using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

public enum Discipline
{
    Dev,
    Test,
    PM
}

namespace PlannerNameSpace
{
    public partial class WorkItem : ForecastableItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.WorkItem; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        public static bool IsPropNameTitle(string publicPropName) { return publicPropName == Utils.GetPropertyName((WorkItem p) => p.Title); }
        public static bool IsPropNameEstimate(string publicPropName) { return publicPropName == Utils.GetPropertyName((WorkItem p) => p.Estimate); }
        public static bool IsPropNameCompleted(string publicPropName) { return publicPropName == Utils.GetPropertyName((WorkItem p) => p.Completed); }
        public static bool IsPropNameWorkRemaining(string publicPropName) { return publicPropName == Utils.GetPropertyName((WorkItem p) => p.WorkRemaining); }

        public WorkItemStates PreviousStatus { get; set; }

        public WorkItem()
        {
            PreviousStatus = WorkItemStates.NotSet;
        }

        public static WorkItem CreateWorkItem(BacklogItem parentBacklogItem, string title = "New WorkItem", string subtype = SubtypeValues.ProductCoding, 
            string assignedTo = null)
        {
            WorkItem newWorkItem = HostItemStore.Instance.CreateStoreItem<WorkItem>(ItemTypeID.WorkItem);
            if (assignedTo == null)
            {
                assignedTo = Globals.ApplicationManager.CurrentUserAlias;
            }

            newWorkItem.AssignedTo = assignedTo;
            newWorkItem.ParentBacklogItem = parentBacklogItem;
            newWorkItem.Title = title;
            newWorkItem.Subtype = subtype;
            return newWorkItem;
        }

        #region ParentItem implementation
        public BacklogItem ParentBacklogItem
        {
            get
            {
                return Globals.ItemManager.GetItem<BacklogItem>(ParentBacklogItemKey);
            }
            set
            {
                // If the given parent backlog item isn't committed to the back-end store yet,
                // then we'll set the actual ID after the commit happens.
                if (value.IsPersisted)
                {
                    ParentBacklogItemID = value.ID;
                }
                else
                {
                    UncommittedParentBacklogItemKey = value.StoreKey;
                }

                TreeID = value.TreeID;
                ShipCycle = value.ShipCycle;
                FixBy = value.FixBy;

                NotifyPropertyChanged(() => PillarName);
                NotifyPropertyChanged(() => TrainName);
                NotifyPropertyChanged(() => ScrumTeamName);
                NotifyPropertyChanged(() => ParentScrumTeamItem);
                NotifyPropertyChanged(() => ParentBacklogItemLongName);
            }
        }

        public int ParentBacklogItemID
        {
            get
            {
                return GetIntValue(Datastore.PropNameParentBacklogItemID);
            }

            set
            {
                SetIntValue(Datastore.PropNameParentBacklogItemID, value);
            }
        }

        public string ParentBacklogItemKey
        {
            get
            {
                if (!string.IsNullOrEmpty(UncommittedParentBacklogItemKey))
                {
                    return UncommittedParentBacklogItemKey;
                }

                return GetParentBacklogItemKey(ParentBacklogItemID);
            }
        }

        public static string GetParentBacklogItemKey(int itemID)
        {
            if (itemID != 0)
            {
                return GetHostItemKey(itemID);
            }

            return null;
        }

        #endregion

        public override ForecastableItem ParentForecastableItem
        {
            get { return ParentBacklogItem; }
        }

        public override string LandingDateText
        {
            get
            {
                if (WorkRemaining == 0 && Completed > 0)
                {
                    return Globals.AlreadyCompleted;
                }
                else if (WorkRemaining == 0 && Completed == 0)
                {
                    return Globals.NotScheduled;
                }

                return base.LandingDateText;
            }
        }

        public override DateTime LandingDate
        {
            get
            {
                return base.LandingDate;
            }
            set
            {
                base.LandingDate = value;

                GroupMemberItem assignedMember = AssignedToGroupMember;
                if (assignedMember != null && value != null)
                {
                    if (assignedMember.LatestLandingDate == null || value > assignedMember.LatestLandingDate.Value)
                    {
                        assignedMember.LatestLandingDate = value;
                    }
                }
            }
        }

        public string QualifiedTitle
        {
            get { return ID.ToString() + ": " + Title + " (" + WorkRemaining + " hours)"; }
        }

        public virtual int WorkRemaining
        {
            get
            {
                int workRemaining = Estimate - Completed;
                if (workRemaining < 0) workRemaining = 0;
                return workRemaining;
            }
        }

        public int OriginalEstimate
        {
            get { return GetIntValue(Datastore.PropNameOriginalEstimate); }
            set { SetIntValue(Datastore.PropNameOriginalEstimate, value); }
        }

        public int Estimate
        {
            get
            {
                return GetIntValue(Datastore.PropNameEstimate);
            }

            set
            {
                SetIntValue(Datastore.PropNameEstimate, value);
                NotifyStatusChanged();
            }
        }

        public int Completed
        {
            get { return GetIntValue(Datastore.PropNameCompleted); }
            set
            {
                int completed = value;
                int estimate = Estimate;
                if (completed > estimate)
                {
                    completed = estimate;
                }

                SetIntValue(Datastore.PropNameCompleted, completed);
                NotifyStatusChanged();
            }
        }

        void NotifyStatusChanged()
        {
            NotifyPropertyChanged(() => WorkRemaining);
            NotifyPropertyChanged(() => ItemStatus);
            NotifyPropertyChanged(() => ItemDisplayStatus);
            NotifyPropertyChanged(() => WorkItemStateColor);
            NotifyPropertyChanged(() => WorkItemStateTextColor);

            if (IsClosed && WorkRemaining > 0)
            {
                ActivateItem();
                Status = StatusValues.Active;
            }
        }

        public string AssignedToDisplayName
        {
            get
            {
                GroupMemberItem member = AssignedToGroupMember;
                if (member != null)
                {
                    return member.DisplayName;
                }

                return null;
            }
        }

        public static List<string> DevWorkItemSubtypes
        {
            get 
            {
                if (m_devWorkItemSubtypes == null)
                {
                    m_devWorkItemSubtypes = new List<string>();
                    m_devWorkItemSubtypes.Add("Product Coding");
                    m_devWorkItemSubtypes.Add("Service Improvements");
                    m_devWorkItemSubtypes.Add("Feature");
                    m_devWorkItemSubtypes.Add("Assessment");
                }

                return m_devWorkItemSubtypes;
            }
        }

        public static List<string> TestWorkItemSubtypes
        {
            get 
            {
                if (m_testWorkItemSubtypes == null)
                {
                    m_testWorkItemSubtypes = new List<string>();
                    m_testWorkItemSubtypes.Add("Automation");
                    m_testWorkItemSubtypes.Add("Automation Coding");
                    m_testWorkItemSubtypes.Add("Automated Testing");
                    m_testWorkItemSubtypes.Add("Manual Testing");
                }

                return m_testWorkItemSubtypes;
            }
        }

        private static List<string> m_devWorkItemSubtypes;
        private static List<string> m_testWorkItemSubtypes;

        public string UncommittedParentBacklogItemKey { get; set; }

        public ScrumTeamItem ParentScrumTeamItem
        {
            get
            {
                BacklogItem parentBacklogItem = ParentBacklogItem;
                if (parentBacklogItem != null)
                {
                    return parentBacklogItem.ScrumTeamItem;
                }

                return null;
            }
        }

        public string ParentBacklogItemLongName
        {
            get { return ParentBacklogItem == null ? Globals.c_None : ParentBacklogItem.ID.ToString() + ": " + ParentBacklogItem.Title; }
        }

        public override string Subtype
        {
            get
            {
                return base.Subtype;
            }
            set
            {
                base.Subtype = value;
                NotifyPropertyChanged(() => WorkItemDiscipline);
            }
        }

        public Discipline WorkItemDiscipline
        {
            get
            {
                foreach (string devSubType in DevWorkItemSubtypes)
                {
                    if (Utils.StringsMatch(devSubType, Subtype))
                    {
                        return Discipline.Dev;
                    }
                }

                foreach (string testSubType in TestWorkItemSubtypes)
                {
                    if (Utils.StringsMatch(testSubType, Subtype))
                    {
                        return Discipline.Test;
                    }
                }

                return Discipline.Dev;
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
                    s_statusValues.Add(WorkItemDisplayStates.Delete);
                }

                return s_statusValues;
            }
        }

        bool HasUserProposedReactivation { get; set; }
        public void ProposeStatusChange(WorkItemStates proposedStatus, string newOwnerAlias)
        {
            AssignedToAndItemStatusChanging = false;
            HasUserProposedReactivation = false;
            if (!Utils.StringsMatch(AssignedTo, newOwnerAlias) && ItemStatus != proposedStatus)
            {
                AssignedToAndItemStatusChanging = true;
            }

            if (Completed > 0 && proposedStatus == WorkItemStates.NotStarted)
            {
                WorkItems.WorkItemNotStartedDialog dialog = new WorkItems.WorkItemNotStartedDialog(this);
                dialog.ShowDialog();
                if (dialog.IsConfirmed)
                {
                    Completed = 0;
                    ItemStatus = proposedStatus;
                }
            }

            else if (ItemStatus == WorkItemStates.Completed && (proposedStatus == WorkItemStates.InProgress || proposedStatus == WorkItemStates.NotStarted))
            {
                WorkItems.WorkItemBackToActiveDialog dialog = new WorkItems.WorkItemBackToActiveDialog(this);
                dialog.ShowDialog();
                if (dialog.IsConfirmed)
                {
                    if (!IsActive)
                    {
                        ActivateItem();
                        Status = StatusValues.Active;
                    }

                    HasUserProposedReactivation = true;
                    AssignedTo = newOwnerAlias;
                    Completed = dialog.Completed;
                    Estimate = dialog.Estimate;
                    if (Completed == 0)
                    {
                        ItemStatus = WorkItemStates.NotStarted;
                    }
                    else
                    {
                        ItemStatus = WorkItemStates.InProgress;
                    }
                }
            }

            else
            {

                AssignedTo = newOwnerAlias;
                ItemStatus = proposedStatus;
            }
        }

        public bool AssignedToAndItemStatusChanging { get; set; }

        public void ShowWorkItemEditor()
        {
            WorkItems.WorkItemEditorDialog dialog = new WorkItems.WorkItemEditorDialog(this);
            dialog.ShowDialog();
        }

        // The actual purpose of the method is only to force the ItemStatus property to re-evaluate
        // (and send a PropertyUpdated notification if appropriate).
        void ForceItemStatusEvaluation(WorkItemStates itemStatus)
        {
            Globals.ApplicationManager.WriteToEventLog("WorkItem status updated: " + itemStatus.ToString());
        }

        void SyncItemResolutionStatus()
        {
            if (Globals.ItemManager.IsDiscoveryComplete)
            {
                int workRemaining = Estimate - Completed;
                if (workRemaining == 0 && Completed > 0)
                {
                    if (IsActive || IsResolved)
                    {
                        // For now, let's stop trying to do this automatically, to 
                        // avoid unexpected change list additions, and exceptions
                        //ResolvedBy = AssignedTo;
                        //BackgroundFixItem();
                        //NotifyStatusChanged();
                    }
                }
                else if (workRemaining > 0)
                {
                    if (IsClosed || IsResolved && Utils.StringsMatch(Resolution, ResolutionValues.Fixed))
                    {
                        if (HasUserProposedReactivation)
                        {
                            HasUserProposedReactivation = false;
                            ActivateItem();
                            Status = StatusValues.Active;
                            NotifyStatusChanged();
                        }
                    }
                }
            }
        }

        public WorkItemStates ItemStatus
        {
            get
            {
                WorkItemStates itemStatus;

                // Handle the case where the work item is currently closed, but then the user edited
                // the estimate or completed values to bring back work remaining.
                if (IsClosed && WorkRemaining > 0)
                {
                    if (Completed > 0)
                    {
                        itemStatus = WorkItemStates.InProgress;
                    }
                    else
                    {
                        itemStatus = WorkItemStates.NotStarted;
                    }
                }
                else if (IsClosed || IsResolved || WorkRemaining == 0 && Completed > 0)
                {
                    itemStatus = WorkItemStates.Completed;
                    SyncItemResolutionStatus();
                }
                else if (Completed > 0 || SubStatus == SubStatusValues.Investigating || SubStatus == SubStatusValues.WorkingOnFix)
                {
                    itemStatus = WorkItemStates.InProgress;
                }
                else
                {
                    itemStatus = WorkItemStates.NotStarted;
                }

                PreviousStatus = itemStatus;
                return itemStatus;
            }

            set
            {
                switch (value)
                {
                    case WorkItemStates.Completed:
                        Completed = Estimate;
                        SyncItemResolutionStatus();
                        break;

                    case WorkItemStates.InProgress:
                        SubStatus = SubStatusValues.WorkingOnFix;
                        break;

                    case WorkItemStates.Delete:
                        {
                            if (IsActive)
                            {
                                RequestDeleteItem();
                            }
                        }
                        break;

                    case WorkItemStates.NotStarted:
                        SubStatus = "";
                        break;
                }

                ForceItemStatusEvaluation(ItemStatus);
            }
        }

        public override TrainItem GetParentTrainItem()
        {
            if (ParentBacklogItem == null)
            {
                return null;
            }

            return ParentBacklogItem.ParentTrainItem;
        }

        public string ItemDisplayStatus
        {
            get
            {
                switch(ItemStatus)
                {
                    case WorkItemStates.Completed:
                        return WorkItemDisplayStates.Completed;
                    case WorkItemStates.InProgress:
                        return WorkItemDisplayStates.InProgress;
                    case WorkItemStates.NotStarted:
                        return WorkItemDisplayStates.NotStarted;
                    default:
                        return WorkItemDisplayStates.NotStarted;
                }
            }

            set
            {
                switch (value)
                {
                    case WorkItemDisplayStates.Completed:
                        ItemStatus = WorkItemStates.Completed;
                        break;
                    case WorkItemDisplayStates.InProgress:
                        ItemStatus = WorkItemStates.InProgress;
                        break;
                    case WorkItemDisplayStates.NotStarted:
                        ItemStatus = WorkItemStates.NotStarted;
                        break;
                    default:
                        ItemStatus = WorkItemStates.NotStarted;
                        break;
                }
            }
        }

        public Brush WorkItemStateColor
        {
            get
            {
                switch (ItemDisplayStatus)
                {
                    case WorkItemDisplayStates.CompletedAndClosed:
                        return Brushes.Green;
                    case WorkItemDisplayStates.CompletedAndResolved:
                        return Brushes.YellowGreen;
                    case WorkItemDisplayStates.Completed:
                        return Brushes.LightGreen;
                    case WorkItemDisplayStates.InProgress:
                        return Brushes.LightYellow;
                    case WorkItemDisplayStates.NotStarted:
                        SolidColorBrush fillBrush = new SolidColorBrush();
                        fillBrush.Color = Color.FromRgb(255, 215, 215);
                        return fillBrush;
                    default:
                        return Brushes.LightGray;
                }
            }
        }

        public Brush WorkItemStateTextColor
        {
            get
            {
                switch (ItemDisplayStatus)
                {
                    case WorkItemDisplayStates.Completed:
                    case WorkItemDisplayStates.CompletedAndClosed:
                        return Brushes.White;
                    case WorkItemDisplayStates.InProgress:
                        return Brushes.Black;
                    default:
                        return Brushes.Black;
                }
            }
        }

        public override PillarItem GetParentPillarItem()
        {
            BacklogItem parentBacklogItem = ParentBacklogItem;
            if (parentBacklogItem != null)
            {
                return parentBacklogItem.ParentPillarItem;
            }

            return null;
        }

        public string PillarName
        {
            get { return ParentBacklogItem == null ? Globals.c_None : ParentBacklogItem.PillarName; }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when the pillar of the parent backlog item changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void NotifyParentPillarChanged()
        {
            BacklogItem parentBacklogItem = ParentBacklogItem;
            if (parentBacklogItem != null)
            {
                TreeID = parentBacklogItem.TreeID;
                NotifyPropertyChanged(() => PillarName);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when the train of the parent backlog item changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void NotifyParentTrainChanged()
        {
            BacklogItem parentBacklogItem = ParentBacklogItem;
            if (parentBacklogItem != null)
            {
                ShipCycle = parentBacklogItem.ShipCycle;
                FixBy = parentBacklogItem.FixBy;
                NotifyPropertyChanged(() => TrainName);
            }
        }

        public string TrainName
        {
            get { return ParentBacklogItem == null ? Globals.c_None : ParentBacklogItem.TrainName; }
        }

        public string ScrumTeamName
        {
            get
            {
                if (ParentScrumTeamItem != null)
                {
                    return ParentScrumTeamItem.Title;
                }

                return Globals.c_None;
            }
        }

        Window ContextOwnerWindow;
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a ContextMenu suitable for the options available for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void PopulateContextMenu(Window ownerWindow, ContextMenu menu)
        {
            ContextOwnerWindow = ownerWindow;
            AddContextMenuItem(menu, "Edit...", "Edit.png", Edit_Click);
            AddContextMenuItem(menu, "Edit Parent Backlog Item...", "EditParent.png", EditParent_Click);

            if (!IsClosed)
            {
                AddContextMenuItem(menu, "Close as Completed...", "Completed.png", Close_Click);
            }
            
            AddContextMenuItem(menu, "Delete...", "DeleteWorkItem.png", Delete_Click);
            AddContextMenuItem(menu, "New WorkItem", "NewWorkItem.png", NewWorkItem_Click);
            AddContextMenuItem(menu, "Move to a different Backlog Item...", "MoveWorkItem.png", MoveToDifferentBacklogItem_Click);
            AddContextMenuItem(menu, "Copy ID", "Copy.png", CopyID_Click);
        }

        void Edit_Click(object sender, RoutedEventArgs e)
        {
            ShowWorkItemEditor();
        }

        void EditParent_Click(object sender, RoutedEventArgs e)
        {
            BacklogItem parent = ParentBacklogItem;
            if (parent != null)
            {
                parent.ShowBacklogItemEditor();
            }
        }

        void Close_Click(object sender, RoutedEventArgs e)
        {
            RequestClose();
        }

        public void RequestClose()
        {
            string status = this.Status;
            if (status == StatusValues.Closed)
            {
                UserMessage.Show("This item is already closed.");
                return;
            }

            if (WorkRemaining > 0)
            {
                if (!UserMessage.ShowYesNo(ContextOwnerWindow, "This WorkItem has work remaining - are you sure you want to close as Completed?", "Close WorkItem"))
                {
                    return;
                }
            }

            this.FixItem();
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            RequestDeleteItem();
        }

        void MoveToDifferentBacklogItem_Click(object sender, RoutedEventArgs e)
        {
            BacklogItem parentBacklogItem = ParentBacklogItem;
            AsyncObservableCollection<WorkItem> workItems = new AsyncObservableCollection<WorkItem>();
            workItems.Add(this);


            PillarItem pillarItem = parentBacklogItem != null ? parentBacklogItem.ParentPillarItem : null;
            TrainItem trainItem = parentBacklogItem != null ? parentBacklogItem.ParentTrainItem : null;
            MoveWorkItemsDialog dialog = new MoveWorkItemsDialog(workItems, pillarItem, trainItem, null);
            dialog.ShowDialog();
        }

        void NewWorkItem_Click(object sender, RoutedEventArgs e)
        {
            BacklogItem parentBacklogItem = ParentBacklogItem;
            if (parentBacklogItem != null)
            {
                string subtype = WorkItemDiscipline == Discipline.Dev ? SubtypeValues.ProductCoding : SubtypeValues.Automation;
                WorkItem newWorkItem = WorkItem.CreateWorkItem(parentBacklogItem, "New WorkItem", subtype, AssignedTo);
                newWorkItem.SaveNewItem();
            }
        }

        public override string AssignedTo
        {
            get
            {
                if (IsResolved || IsClosed)
                {
                    string workAssignedTo = WorkAssignedTo;
                    if (!string.IsNullOrWhiteSpace(workAssignedTo))
                    {
                        if (Globals.GroupMemberManager.GroupMemberExists(workAssignedTo))
                        {
                            return workAssignedTo;
                        }
                    }
                }
                
                return base.AssignedTo;
            }
            set
            {
                base.AssignedTo = value;

                if (IsActive)
                {
                    WorkAssignedTo = value;
                }
            }
        }

        public string WorkAssignedTo
        {
            get { return GetStringValue(Datastore.PropNameWorkItemWorkAssignedToKey); }
            set { SetStringValue(Datastore.PropNameWorkItemWorkAssignedToKey, value); }
        }

        public override void OnResolution()
        {
            base.OnResolution();
            WorkAssignedTo = AssignedTo;
        }
    }
}
