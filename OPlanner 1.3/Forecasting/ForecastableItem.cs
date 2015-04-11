using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public enum OverallStatus
    {
        AllCriteriaOnTrack,
        SomeCriteriaOnTrack,
        NoCriteriaOnTrack,
    }

    public enum LandingDateStatus
    {
        OnTrack,
        NotOnTrack,
        AssignedTrainHasExpired,
        LandingDateNotAvailable,
    }

    public enum CompletionStatus
    {
        Completed,
        In_Progress,
        Not_Started,
        Calculating,
    }

    public enum PlanningIssue
    {
        All_Items,
        Items_With_Spec_or_Design_Issues,
        Items_With_Spec_Issues,
        Items_With_Design_Issues,
    }

    public enum SpecStatus
    {
        No_Specs_Required,
        Specs_Not_Current,
        All_Specs_Current,
        Spec_Status_Not_Set,
        Calculating,
    }

    public enum DesignStatus
    {
        NoDesignRequired,
        DesignNotComplete,
        DesignComplete,
        DesignStatusUnknown,
        Calculating,
    }

    public class DesignStatusValues
    {
        public const string NoDesignRequired = "No Design Work Required";
        public const string DesignNotComplete = "Design Work Required";
        public const string OldDesignNotComplete = "Design Work Incomplete";
        public const string DesignComplete = "All Design Work Complete";
        public const string DesignStatusUnknown = "Design Status Unknown";
    }

    public abstract class ForecastableItem : StoreItem
    {
        private SpecStatus m_specStatus = SpecStatus.Calculating;
        private DesignStatus m_designStatus = DesignStatus.Calculating;
        private CompletionStatus m_completionStatus = CompletionStatus.Calculating;
        private int m_childCount = 0;
        private int m_activeChildCount = 0;

        public ForecastableItem()
        {
            ForecastedFinishTrain = null;
        }

        public virtual DateTime LandingDate
        {
            get 
            {
                return GetLocalDateValue();
            }

            set
            {
                SetLocalDateValue(value);
                NotifyPropertyChanged(() => LandingDateText);
                NotifyPropertyChanged(() => LandingDateStatusToolTip);
                NotifyPropertyChanged(() => LandingDateStatusColor);
            }
        }

        public virtual string LandingDateText
        {
            get
            {
                DateTime landingDate = LandingDate;
                if (landingDate.IsDefault())
                {
                    if (IsActive)
                    {
                        if (GetCommitmentSetting() != CommitmentSettingValues.In_Progress)
                        {
                            return Globals.NotInProgress;
                        }
                        else if (AssignedTo == StatusValues.Active)
                        {
                            return Globals.NotAssigned;
                        }

                        if (Globals.CommitmentStatusManager.CommitmentStatusComputationComplete)
                        {
                            return Globals.NotScheduled;
                        }
                        else
                        {
                            return Globals.CalculatingStatus;
                        }
                    }
                    else
                    {
                        return Globals.AlreadyCompleted;
                    }
                }
                else
                {
                    return landingDate.ToShortDateString();
                }
            }
        }

        public virtual Brush LandingDateStatusColor
        {
            get
            {
                TrainItem targetTrain = GetParentTrainItem();
                DateTime landingDate = LandingDate;

                if (!IsActive || GetCommitmentSetting() == CommitmentSettingValues.Completed)
                {
                    return Brushes.DarkGreen;
                }

                if (GetCommitmentStatus() == TrainCommitmentStatusValue.NotCalculated)
                {
                    return Brushes.DarkGray;
                }

                if (landingDate == default(DateTime))
                {
                    if (GetCommitmentStatus() == TrainCommitmentStatusValue.CommittedNotApproved)
                    {
                        return Brushes.Goldenrod;
                    }

                    if (GetCommitmentStatus() == TrainCommitmentStatusValue.CommittedAndApproved)
                    {
                        return Brushes.YellowGreen;
                    }

                    if (!StoreItem.IsRealItem(targetTrain))
                    {
                        return Brushes.DarkGoldenrod;
                    }

                    return Brushes.DarkGoldenrod;
                }
                else
                {
                    int workHoursAvailable = Utils.GetNetWorkingHours(DateTime.Today, targetTrain.EndDate);
                    int workHoursRemaining = Utils.GetNetWorkingHours(DateTime.Today, landingDate);
                    return Utils.GetWorkCapacityFillColor(workHoursRemaining, workHoursAvailable);
                }
            }
        }

        public LandingDateStatus LandingDateStatus
        {
            get
            {
                TrainItem targetTrain = GetParentTrainItem();
                if (targetTrain.TimeFrame == TrainTimeFrame.Past)
                {
                    return LandingDateStatus.AssignedTrainHasExpired;
                }

                DateTime landingDate = LandingDate;
                if (landingDate == default(DateTime))
                {
                    return LandingDateStatus.LandingDateNotAvailable;
                }

                int workHoursAvailable = Utils.GetNetWorkingHours(DateTime.Today, targetTrain.EndDate);
                int workHoursRemaining = Utils.GetNetWorkingHours(DateTime.Today, landingDate);
                if (workHoursRemaining <= workHoursAvailable)
                {
                    return LandingDateStatus.OnTrack;
                }
                else
                {
                    return LandingDateStatus.NotOnTrack;
                }
            }
        }

        GroupMemberItem m_projectedGroupMember = null;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// If a landing date has been projected for this item using story points, this is
        /// the member that was used to perform the projection.
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual GroupMemberItem ProjectedGroupMember
        {
            get { return m_projectedGroupMember; }
            set
            {
                m_projectedGroupMember = value;
                NotifyPropertyChanged(() => LandingDateStatusToolTip);
            }
        }

        public virtual string LandingDateStatusToolTip
        {
            get
            {
                TrainItem targetTrain = GetParentTrainItem();
                DateTime landingDate = LandingDate;
                CommitmentSettingValues commitmentSetting = GetCommitmentSetting();
                if (!IsActive || commitmentSetting == CommitmentSettingValues.Completed)
                {
                    return PlannerContent.LandingDateTooltipCompleted;
                }
                else if (commitmentSetting != CommitmentSettingValues.In_Progress)
                {
                    int storyPoints = GetStoryPoints();
                    if (commitmentSetting == CommitmentSettingValues.Committed && storyPoints > 0)
                    {
                        string memberDisplayName = ProjectedGroupMember == null ? "None" : ProjectedGroupMember.DisplayName;
                        return "The landing date for this item has been projected using your Story Points estimate (Dev: " + memberDisplayName + ").  Once the Commitment Setting is changed to 'In Progress', the actual estimated landing date will be calculated using the work item estimates assigned to this backlog item.";
                    }
                    else
                    {
                        return "The landing date for this item will be calculated once the Commitment Status is changed to 'In Progress', or you set a 'Story Points' value.";
                    }
                }
                else if (StoreItem.IsRealItem(targetTrain) && landingDate != default(DateTime))
                {
                    if (IsActive && targetTrain.TimeFrame == TrainTimeFrame.Past)
                    {
                        return "This item is late (the train it is assigned to has already wrapped up).";
                    }
                    else
                    {
                        int workHoursAvailable = Utils.GetNetWorkingHours(DateTime.Today, targetTrain.EndDate);
                        int workHoursNeededToFinish = Utils.GetNetWorkingHours(DateTime.Today, landingDate);
                        if (workHoursNeededToFinish <= workHoursAvailable)
                        {
                            int hoursToSpare = workHoursAvailable - workHoursNeededToFinish;
                            if (hoursToSpare <= 24)
                            {
                                return "This item is forecast to finish on time, but it's starting to get close to the deadline.";
                            }
                            else
                            {
                                return "This item is forecast to finish on time.";
                            }
                        }
                        else
                        {
                            return "This item is forecast to finish later than expected (i.e. after the end of the assigned train).  You might want to scale back the scope of the work, assign additional resources, or move this item to the next train.";
                        }
                    }
                }
                else
                {
                    if (!StoreItem.IsRealItem(targetTrain))
                    {
                        return "This item is not assigned to a valid Train, thus completion status cannot be determined.";
                    }

                    return "";
                }
            }
        }

        public bool IsForecastable
        {
            get { return ForecastedFinishTrain != null; }
        }

        public virtual AsyncObservableCollection<ForecastableItem> ForecastableChildren
        {
            get { return new AsyncObservableCollection<ForecastableItem>(); }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a status value that takes into account spec status, design status, and
        /// forecasting status.
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual OverallStatus OverallStatus
        {
            get
            {
                if (CompletionStatus == PlannerNameSpace.CompletionStatus.Completed)
                {
                    if (ChildCount > 0)
                    {
                        return PlannerNameSpace.OverallStatus.AllCriteriaOnTrack;
                    }
                    else
                    {
                        return PlannerNameSpace.OverallStatus.SomeCriteriaOnTrack;
                    }
                }

                bool specsOnTrack = SpecStatus == PlannerNameSpace.SpecStatus.All_Specs_Current || SpecStatus == PlannerNameSpace.SpecStatus.No_Specs_Required;
                bool designOnTrack = DesignStatus == PlannerNameSpace.DesignStatus.DesignComplete || DesignStatus == PlannerNameSpace.DesignStatus.NoDesignRequired;

                if (!specsOnTrack && !designOnTrack)
                {
                    return PlannerNameSpace.OverallStatus.NoCriteriaOnTrack;
                }

                if (!specsOnTrack || !designOnTrack)
                {
                    return PlannerNameSpace.OverallStatus.SomeCriteriaOnTrack;
                }

                return PlannerNameSpace.OverallStatus.AllCriteriaOnTrack;
            }
        }

        public Brush OverallStatusColor
        {
            get
            {
                switch (OverallStatus)
                {
                    case PlannerNameSpace.OverallStatus.AllCriteriaOnTrack:
                        return Utils.GetBrush(75, 164, 51);
                    case PlannerNameSpace.OverallStatus.SomeCriteriaOnTrack:
                        return Utils.GetBrush(254, 203, 0);
                    case PlannerNameSpace.OverallStatus.NoCriteriaOnTrack:
                    default:
                        return Brushes.Red;
                }
            }
        }

        public int DirectDescendentCount
        {
            get
            {
                return ForecastableChildren.Count;
            }
        }

        public int ChildCount
        {
            get
            {
                return m_childCount;
            }
        }

        public int ActiveChildCount
        {
            get
            {
                return m_activeChildCount;
            }
        }

        public CantForecastReasons CantForecastReason { get; set; }

        ItemTreeViewWindow TreeWindow;
        public void ShowTreeView()
        {
            if (TreeWindow != null)
            {
                if (!TreeWindow.IsVisible)
                {
                    TreeWindow.Show();
                }

                TreeWindow.BringIntoView();
            }
            else
            {
                TreeWindow = new ItemTreeViewWindow(this);
                TreeWindow.Closed += TreeWindow_Closed;
                TreeWindow.Show();
            }
        }

        void TreeWindow_Closed(object sender, EventArgs e)
        {
            TreeWindow = null;
        }

        TrainItem m_ForecastedFinishTrain;

        public abstract ForecastableItem ParentForecastableItem { get; }

        public TrainItem ForecastedFinishTrain
        {
            get { return m_ForecastedFinishTrain; }
            
            set 
            {
                m_ForecastedFinishTrain = value; 
                NotifyPropertyChanged(() => ShipTrainTitle);
                NotifyPropertyChanged(() => ShipQuarterTitle);
                NotifyPropertyChanged(() => ForecastingColor);
                NotifyPropertyChanged(() => ForecastTrainToolTip);
                NotifyPropertyChanged(() => ForecastQuarterToolTip);
                NotifyPropertyChanged(() => OverallStatus);
            }
        }

        public string ShipTrainTitle
        {
            get
            {
                if (ForecastedFinishTrain == null)
                {
                    return Globals.CannotForecast;
                }
                else
                {
                    if (ForecastedFinishTrain.TimeFrame == TrainTimeFrame.Past)
                    {
                        return "Completed";
                    }
                    else
                    {
                        return ForecastedFinishTrain.Title;
                    }
                }
            }
        }

        public string ShipQuarterTitle
        {
            get
            {
                try
                {
                    if (ForecastedFinishTrain == null)
                    {
                        return Globals.CannotForecast;
                    }
                    else
                    {
                        return ForecastedFinishTrain.ParentQuarterItem.Title;
                    }
                }

                catch
                {
                    return Globals.CannotForecast;
                }
            }
        }

        public Brush ForecastingColor
        {
            get
            {
                try
                {
                    if (ForecastedFinishTrain == null)
                    {
                        return Brushes.Red;
                    }
                    else
                    {
                        if (ForecastedFinishTrain.TimeFrame == TrainTimeFrame.Past)
                        {
                            return Brushes.DarkGreen;
                        }
                        else
                        {
                            return Brushes.Green;
                        }
                    }
                }

                catch
                {
                    return Brushes.Red;
                }
            }
        }

        public string ForecastTrainToolTip
        {
            get
            {
                if (IsForecastable)
                {
                    return ForecastMode.ForecastToolTip;
                }
                else
                {
                    string cantforecastReason = ForecastManager.Instance.TrainReasons[CantForecastReason];
                    return cantforecastReason;
                }
            }
        }

        public string SpecStatusToolTip
        {
            get
            {
                return "This represents the current spec status for this item, or for all child items.";
            }
        }

        public string DesignStatusToolTip
        {
            get
            {
                return "This represents the current design status for this item, or for all child items.";
            }
        }

        public string ForecastQuarterToolTip
        {
            get
            {
                return ForecastTrainToolTip;
            }
        }

        public AvailableViews CurrentView
        {
            get { return ViewManager.CurrentGlobalView; }
            set 
            { 
                NotifyPropertyChangedByName();
                NotifyPropertyChanged(() => IsExperienceForecastView);
                NotifyPropertyChanged(() => IsExperienceSpecStatusView);
                NotifyPropertyChanged(() => IsExperienceSummaryView);
                NotifyPropertyChanged(() => IsNotExperienceSummaryView);
            }
        }

        public virtual void SetCurrentView(AvailableViews currentView)
        {
            CurrentView = currentView;

            foreach (ForecastableItem item in ForecastableChildren)
            {
                item.SetCurrentView(currentView);
            }
        }

        // Supported Views
        public Visibility IsExperienceForecastView { get { return CurrentView == AvailableViews.ExperienceForecastView ? Visibility.Visible : Visibility.Collapsed; } }
        
        public Visibility IsExperienceSpecStatusView { get { return CurrentView == AvailableViews.ExperienceSpecStatusView ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility IsExperienceSummaryView { get { return CurrentView == AvailableViews.ExperienceSummaryView ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility IsNotExperienceSummaryView { get { return CurrentView != AvailableViews.ExperienceSummaryView ? Visibility.Visible : Visibility.Collapsed; } }

        public virtual SpecStatus SpecStatus
        {
            get
            {
                return m_specStatus;
            }
        }

        public void SetSpecStatus(SpecStatus status, bool updateParent)
        {
            m_specStatus = status;

            if (updateParent && ParentForecastableItem != null)
            {
                ParentForecastableItem.UpdateSpecStatus(updateParent);
            }

            NotifyPropertyChanged(() => SpecStatusText);
            NotifyPropertyChanged(() => SpecStatusColor);
        }


        public virtual void NotifySpecStatusChanged(bool updateStatus = true)
        {
            if (updateStatus)
            {
                UpdateSpecStatus(true);
            }

            NotifyPropertyChanged(() => SpecStatus);
            NotifyPropertyChanged(() => SpecStatusText);
            NotifyPropertyChanged(() => SpecStatusTextCompact);
            NotifyPropertyChanged(() => SpecStatusColor);
            NotifyPropertyChanged(() => OverallStatus);

            if (ParentForecastableItem != null)
            {
                ParentForecastableItem.NotifySpecStatusChanged(updateStatus);
            }
        }

        public virtual void NotifyDesignStatusChanged(bool updateStatus = true)
        {
            if (updateStatus)
            {
                UpdateDesignStatus(true);
            }

            NotifyPropertyChanged(() => DesignStatus);
            NotifyPropertyChanged(() => DesignStatusText);
            NotifyPropertyChanged(() => DesignStatusTextCompact);
            NotifyPropertyChanged(() => DesignStatusColor);
            NotifyPropertyChanged(() => OverallStatus);

            if (ParentForecastableItem != null)
            {
                ParentForecastableItem.NotifyDesignStatusChanged(updateStatus);
            }
        }

        public virtual void NotifyCompletionStatusChanged(bool updateStatus = true)
        {
            if (updateStatus)
            {
                UpdateCompletionStatus(true);
            }

            NotifyPropertyChanged(() => CompletionStatus);

            if (ParentForecastableItem != null)
            {
                ParentForecastableItem.NotifyCompletionStatusChanged(updateStatus);
            }
        }

        public virtual DesignStatus DesignStatus
        {
            get
            {
                return m_designStatus;
            }
        }

        public virtual void SetDesignStatus(DesignStatus status, bool updateParent)
        {
            m_designStatus = status;
            if (updateParent && ParentForecastableItem != null)
            {
                ParentForecastableItem.UpdateDesignStatus(updateParent);
            }

            NotifyPropertyChanged(() => DesignStatusText);
            NotifyPropertyChanged(() => DesignStatusColor);
        }

        public CompletionStatus CompletionStatus
        {
            get { return m_completionStatus; }
        }

        public void SetCompletionStatus(CompletionStatus status, bool updateParent)
        {
            m_completionStatus = status;

            if (updateParent && ParentForecastableItem != null)
            {
                ParentForecastableItem.UpdateCompletionStatus(updateParent);
            }
        }


        public virtual string SpecStatusTextCompact
        {
            get
            {
                switch (SpecStatus)
                {
                    case PlannerNameSpace.SpecStatus.No_Specs_Required:
                        return Globals.GoStatus;
                    case PlannerNameSpace.SpecStatus.All_Specs_Current:
                        return Globals.GoStatus;
                    case PlannerNameSpace.SpecStatus.Specs_Not_Current:
                        return Globals.NoGoStatus;
                    case PlannerNameSpace.SpecStatus.Spec_Status_Not_Set:
                        return Globals.NoGoStatus;
                    case PlannerNameSpace.SpecStatus.Calculating:
                        return Globals.CalculatingStatus;
                    default:
                        return Globals.NoGoStatus;
                }
            }
        }

        public virtual string SpecStatusText
        {
            get
            {
                return Utils.EnumToString<SpecStatus>(SpecStatus);
            }
        }

        public virtual Brush SpecStatusColor
        {
            get
            {
                switch (SpecStatus)
                {
                    case PlannerNameSpace.SpecStatus.No_Specs_Required:
                        return Brushes.DarkGreen;
                    case PlannerNameSpace.SpecStatus.All_Specs_Current:
                        return Brushes.Green;
                    case PlannerNameSpace.SpecStatus.Specs_Not_Current:
                        return Brushes.Red;
                    case PlannerNameSpace.SpecStatus.Spec_Status_Not_Set:
                        return Brushes.Red;
                    case PlannerNameSpace.SpecStatus.Calculating:
                        return Brushes.Black;
                    default:
                        return Brushes.Yellow;
                }
            }
        }

        public AsyncObservableCollection<AllowedValue> DesignStatusAllowedValues
        {
            get
            {
                AsyncObservableCollection<AllowedValue> values = new AsyncObservableCollection<AllowedValue>();
                values.Add(new AllowedValue(DesignStatusValues.NoDesignRequired));
                values.Add(new AllowedValue(DesignStatusValues.DesignNotComplete));
                values.Add(new AllowedValue(DesignStatusValues.DesignComplete));
                return values;
            }
        }

        public virtual string DesignStatusTextCompact
        {
            get
            {
                if (DesignStatus == PlannerNameSpace.DesignStatus.DesignComplete || DesignStatus == PlannerNameSpace.DesignStatus.NoDesignRequired)
                {
                    return Globals.GoStatus;
                }
                else if (DesignStatus == PlannerNameSpace.DesignStatus.Calculating)
                {
                    return Globals.CalculatingStatus;
                }

                return Globals.NoGoStatus;
            }
        }

        public virtual string DesignStatusText
        {
            get
            {
                return GetDesignStatusText(DesignStatus);
            }

            set
            {
                SetDesignStatus(GetDesignStatus(value), true);
            }
        }

        protected DesignStatus GetDesignStatus(string rawDesignStatus)
        {
            if (string.IsNullOrWhiteSpace(rawDesignStatus))
            {
                return DesignStatus.DesignStatusUnknown;
            }
            else if (Utils.StringsMatch(rawDesignStatus, DesignStatusValues.NoDesignRequired))
            {
                return DesignStatus.NoDesignRequired;
            }
            else if (Utils.StringsMatch(rawDesignStatus, DesignStatusValues.DesignComplete))
            {
                return DesignStatus.DesignComplete;
            }
            else if (Utils.StringsMatch(rawDesignStatus, DesignStatusValues.DesignNotComplete))
            {
                return DesignStatus.DesignNotComplete;
            }
            else if (Utils.StringsMatch(rawDesignStatus, DesignStatusValues.OldDesignNotComplete))
            {
                return DesignStatus.DesignNotComplete;
            }
            else
            {
                return DesignStatus.DesignStatusUnknown;
            }
        }
        protected string GetDesignStatusText(DesignStatus designStatus)
        {
            switch (designStatus)
            {
                case PlannerNameSpace.DesignStatus.NoDesignRequired:
                case PlannerNameSpace.DesignStatus.DesignStatusUnknown:
                    return DesignStatusValues.NoDesignRequired;
                case PlannerNameSpace.DesignStatus.DesignComplete:
                    return DesignStatusValues.DesignComplete;
                case PlannerNameSpace.DesignStatus.DesignNotComplete:
                    return DesignStatusValues.DesignNotComplete;
                default:
                    return DesignStatusValues.NoDesignRequired;
            }
        }

        public virtual Brush DesignStatusColor
        {
            get
            {
                switch (DesignStatus)
                {
                    case PlannerNameSpace.DesignStatus.NoDesignRequired:
                        return Brushes.DarkGreen;
                    case PlannerNameSpace.DesignStatus.DesignComplete:
                        return Brushes.Green;
                    case PlannerNameSpace.DesignStatus.DesignNotComplete:
                        return Brushes.Red;
                    case PlannerNameSpace.DesignStatus.Calculating:
                        return Brushes.Black;
                    default:
                        return Brushes.Yellow;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Adds options to the given menu appropriate for this current state of this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void AddForecastableContextMenuItems(ContextMenu menu)
        {
            if (ChildCount > 0)
            {
                if (SpecStatus != PlannerNameSpace.SpecStatus.All_Specs_Current && SpecStatus != PlannerNameSpace.SpecStatus.No_Specs_Required)
                {
                    AddContextMenuItem(menu, "Show descendents with Spec Issues...", "BacklogSpecStatusView.png", ShowSpecIssues);
                }

                if (DesignStatus != PlannerNameSpace.DesignStatus.DesignComplete && DesignStatus != PlannerNameSpace.DesignStatus.NoDesignRequired)
                {
                    AddContextMenuItem(menu, "Show descendents that require design work...", "DesignIssues.png", ShowDesignIssues);
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Puts up a window showing all child items that have at least one spec issue.
        /// </summary>
        //------------------------------------------------------------------------------------
        void ShowSpecIssues(object sender, RoutedEventArgs e)
        {
            ShowIssuesWindow(PlanningIssue.Items_With_Spec_Issues);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Puts up a window showing all child items that have at least one design issue.
        /// </summary>
        //------------------------------------------------------------------------------------
        void ShowDesignIssues(object sender, RoutedEventArgs e)
        {
            ShowIssuesWindow(PlanningIssue.Items_With_Design_Issues);
        }

        ForecastingIssuesWindow IssuesWindow;
        void ShowIssuesWindow(PlanningIssue issue)
        {
            if (IssuesWindow != null)
            {
                if (!IssuesWindow.IsVisible)
                {
                    IssuesWindow.Show();
                }
                IssuesWindow.BringIntoView();
                IssuesWindow.ChangeIssue(issue);
            }
            else
            {
                IssuesWindow = new ForecastingIssuesWindow(this, issue);
                IssuesWindow.Closed += IssuesWindow_Closed;
                IssuesWindow.Show();
            }
        }

        void IssuesWindow_Closed(object sender, EventArgs e)
        {
            IssuesWindow = null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of all the child items of this item that have the given planning
        /// issue.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<ForecastableItem> GetItemsWithPlanningIssues(PlanningIssue issue)
        {
            AsyncObservableCollection<ForecastableItem> items = new AsyncObservableCollection<ForecastableItem>();
            foreach (ForecastableItem item in ForecastableChildren)
            {
                if (issue == PlanningIssue.All_Items)
                {
                    item.AddItem(items);
                }
                else if (issue == PlanningIssue.Items_With_Spec_Issues)
                {
                    item.AddItemWithSpecIssues(items);
                }
                else if (issue == PlanningIssue.Items_With_Design_Issues)
                {
                    item.AddItemWithDesignIssues(items);
                }
                else if (issue == PlanningIssue.Items_With_Spec_or_Design_Issues)
                {
                    item.AddItemsWithSpecOrDesignIssues(items);
                }
            }

            return items;
        }

        public virtual void AddItem(AsyncObservableCollection<ForecastableItem> items)
        {
            foreach (ForecastableItem item in ForecastableChildren)
            {
                item.AddItem(items);
            }
        }

        public virtual void AddItemWithSpecIssues(AsyncObservableCollection<ForecastableItem> items)
        {
            foreach (ForecastableItem item in ForecastableChildren)
            {
                item.AddItemWithSpecIssues(items);
            }
        }

        public virtual void AddItemWithDesignIssues(AsyncObservableCollection<ForecastableItem> items)
        {
            foreach (ForecastableItem item in ForecastableChildren)
            {
                item.AddItemWithDesignIssues(items);
            }
        }

        public virtual void AddItemsWithSpecOrDesignIssues(AsyncObservableCollection<ForecastableItem> items)
        {
            foreach (ForecastableItem item in ForecastableChildren)
            {
                item.AddItemsWithSpecOrDesignIssues(items);
            }
        }

        public virtual bool HasSpecIssue
        {
            get
            {
                if (SpecStatus != PlannerNameSpace.SpecStatus.All_Specs_Current && SpecStatus != PlannerNameSpace.SpecStatus.No_Specs_Required)
                {
                    return true;
                }

                return false;
            }
        }

        public virtual bool HasDesignIssue
        {
            get
            {
                if (DesignStatus != PlannerNameSpace.DesignStatus.DesignComplete && DesignStatus != PlannerNameSpace.DesignStatus.NoDesignRequired)
                {
                    return true;
                }

                return false;
            }
        }

        public virtual void UpdateSpecStatus(bool updateParent)
        {
            if (ForecastableChildren.Count == 0 && SpecStatus == PlannerNameSpace.SpecStatus.Calculating)
            {
                SetSpecStatus(PlannerNameSpace.SpecStatus.All_Specs_Current, updateParent);
                NotifySpecStatusChanged(false);
                return;
            }

            int noSpecRequiredCount = 0;
            foreach (ForecastableItem item in ForecastableChildren)
            {
                if (item.SpecStatus == PlannerNameSpace.SpecStatus.Specs_Not_Current || item.SpecStatus == PlannerNameSpace.SpecStatus.Spec_Status_Not_Set)
                {
                    SetSpecStatus(PlannerNameSpace.SpecStatus.Specs_Not_Current, updateParent);
                    return;
                }
                else if (item.SpecStatus == PlannerNameSpace.SpecStatus.No_Specs_Required)
                {
                    noSpecRequiredCount++;
                }
            }

            if (noSpecRequiredCount == ForecastableChildren.Count)
            {
                SetSpecStatus(PlannerNameSpace.SpecStatus.No_Specs_Required, updateParent);
                return;
            }

            SetSpecStatus(PlannerNameSpace.SpecStatus.All_Specs_Current, updateParent);

        }

        public virtual void UpdateDesignStatus(bool updateParent)
        {
            if (ForecastableChildren.Count == 0 && DesignStatus == PlannerNameSpace.DesignStatus.Calculating)
            {
                SetDesignStatus(PlannerNameSpace.DesignStatus.NoDesignRequired, updateParent);
                NotifyDesignStatusChanged(false);
                return;
            }

            int noDesignRequiredCount = 0;
            int designCompleteCount = 0;
            foreach (ForecastableItem item in ForecastableChildren)
            {
                if (item.DesignStatus == PlannerNameSpace.DesignStatus.NoDesignRequired)
                {
                    noDesignRequiredCount++;
                    designCompleteCount++;
                }
                else if (item.DesignStatus == PlannerNameSpace.DesignStatus.DesignComplete)
                {
                    designCompleteCount++;
                }
            }

            if (noDesignRequiredCount == ForecastableChildren.Count)
            {
                SetDesignStatus(PlannerNameSpace.DesignStatus.NoDesignRequired, updateParent);
            }
            else if (designCompleteCount == ForecastableChildren.Count)
            {
                SetDesignStatus(PlannerNameSpace.DesignStatus.DesignComplete, updateParent);
            }
            else
            {
                SetDesignStatus(PlannerNameSpace.DesignStatus.DesignNotComplete, updateParent);
            }
        }

        public virtual void UpdateCompletionStatus(bool updateParent)
        {
            if (ForecastableChildren.Count == 0 && CompletionStatus == PlannerNameSpace.CompletionStatus.Calculating)
            {
                SetCompletionStatus(PlannerNameSpace.CompletionStatus.Completed, updateParent);
                NotifyCompletionStatusChanged(false);
                return;
            }

            foreach (ForecastableItem item in ForecastableChildren)
            {
                if (item.CompletionStatus != CompletionStatus.Completed)
                {
                    SetCompletionStatus(PlannerNameSpace.CompletionStatus.In_Progress, updateParent);
                    return;
                }
            }

            SetCompletionStatus(PlannerNameSpace.CompletionStatus.Completed, updateParent);
        }

        public virtual void UpdateChildCounts()
        {
            m_childCount = 0;
            m_activeChildCount = 0;
            ChildCountWorker(ForecastableChildren, ref m_childCount, ref m_activeChildCount);

            NotifyPropertyChanged(() => ChildCount);
            NotifyPropertyChanged(() => ActiveChildCount);

            if (ParentForecastableItem != null)
            {
                ParentForecastableItem.UpdateChildCounts();
            }
        }

        protected void ChildCountWorker(AsyncObservableCollection<ForecastableItem> items, ref int allCount, ref int activeCount)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            foreach (ForecastableItem item in items)
            {
                allCount++;
                if (item.CompletionStatus == PlannerNameSpace.CompletionStatus.In_Progress)
                {
                    activeCount++;
                }

                ChildCountWorker(item.ForecastableChildren, ref allCount, ref activeCount);
            }
        }

    }
}
