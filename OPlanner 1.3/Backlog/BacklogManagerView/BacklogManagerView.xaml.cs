using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections;

namespace PlannerNameSpace.Views
{
    public enum ApprovalStatus
    {
        Approved,
        NotApproved,
        Indeterminate,
    }

    /// <summary>
    /// Interaction logic for BacklogManagerView.xaml
    /// </summary>
    public partial class BacklogManagerView : UserControl
    {
        ColumnViewManager ViewManager;
        BacklogItem PillarCanceledBacklogItem = null;
        BacklogItem TrainCanceledBacklogItem = null;
        RowDefinition WorkItemsRow;
        RowDefinition WorkItemsSplitterRow;
        bool CommitmentStatusSelectionChanged { get; set; }
        ComboBox StatusCombo;

        BacklogViewItemCollection BacklogCollection;
        BacklogViewState ViewState;

        UncommittedBacklogViewItemCollection UncommittedBacklogCollection;
        BacklogViewMembersItemCollection MembersViewItemCollection;

        GridLength CurrentWorkItemsPanelHeight;

        BacklogItem CurrentBacklogItem { get; set; }
        BacklogItem OnUpdateCurrentBacklogItem { get; set; }

        ApprovalStatus CurrentCommitmentApprovalStatus { get; set; }
        Nullable<DateTime> CurrentCommitmentApprovalDate;
        string CurrentCommitmentApprover;

        public BacklogManagerView()
        {
            InitializeComponent();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Respond to changes in StoreItems that affect our UI.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            switch (e.Change.ChangeType)
            {
                case ChangeType.Added:
                    switch (e.Change.Item.StoreItemType)
                    {
                        case ItemTypeID.BacklogItem:
                            {
                                if (e.Change.ChangeSource != ChangeSource.Undo)
                                {
                                    BacklogItem backlogItem = (BacklogItem)e.Change.Item;
                                    BacklogCollection.SelectItem(backlogItem);
                                }
                            }
                            break;

                        case ItemTypeID.WorkItem:
                            {
                                // If this isn't an undo operation, click on the backlog item that a
                                // work item was just created for (and make sure the second panel is
                                // showing), so that the user can see the new work item show up in the UI.
                                if (e.Change.ChangeSource != ChangeSource.Undo)
                                {
                                    WorkItem workItem = (WorkItem)e.Change.Item;
                                    BacklogItem backlogItem = workItem.ParentBacklogItem;
                                    if (backlogItem != null)
                                    {
                                        BacklogCollection.ClickSelectItem(backlogItem);
                                        EnsureWorkItemsPanelIsShowing();
                                    }

                                    WorkItemGrid.ScrollIntoView(workItem);
                                }
                            }
                            break;
                    }

                    break;
                case ChangeType.Removed:
                    switch (e.Change.Item.StoreItemType)
                    {
                        case ItemTypeID.BacklogItem:
                            {
                                BacklogItem backlogItem = (BacklogItem)e.Change.Item;
                                if (backlogItem == CurrentBacklogItem)
                                {
                                    CurrentBacklogItem = null;
                                    SetWorkItemGridHeading();
                                }
                            }
                            break;
                    }
                    break;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called immediately after the view is created, allowing population of the data
        /// presented by the view.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void InitializeData()
        {
            StatusCombo = Globals.MainWindow.BacklogStatusFilterCombo;

            ViewState = new BacklogViewState(Globals.MainWindow.BacklogPillarFilterCombo, ProductPreferences.BacklogViewLastSelectedPillar,
                Globals.MainWindow.BacklogTrainFilterCombo, ProductPreferences.BacklogViewLastSelectedTrain);

            BacklogItem.CurrentBacklogTrain = ViewState.TrainState.CurrentItem;

            ViewState.PillarState.CurrentItemChanged += BacklogViewState_PillarChanged;
            ViewState.TrainState.CurrentItemChanged += BacklogViewState_TrainChanged;

            SetUpEventManagerHandlers();

            RowDefinitionCollection rows = MainGrid.RowDefinitions;
            WorkItemsRow = rows[3];
            WorkItemsSplitterRow = rows[2];
            CurrentWorkItemsPanelHeight = WorkItemsRow.Height;
            OnUpdateCurrentBacklogItem = null;

            ViewState.ShowOnlyUnassignedMembers = false;
            PopulateFilters();
            SetUpFirstPanel();

            BacklogCollection = new BacklogViewItemCollection(ViewState);
            BacklogCollection.CollectionCountChanged += Backlog_CollectionCountChanged;
            BacklogCollection.SetItemsControl(BacklogGrid);
            DataGrid grid = BacklogGrid;

            UncommittedBacklogCollection = new UncommittedBacklogViewItemCollection(ViewState);
            UncommittedBacklogCollection.CollectionCountChanged += UncommittedBacklogCollection_CollectionCountChanged;
            UncommittedBacklogCollection.SetItemsControl(UncommittedBacklogGrid);

            MembersViewItemCollection = new BacklogViewMembersItemCollection(ViewState);
            MembersViewItemCollection.SetItemsControl(MemberAssignmentsGrid);

            SetUpRibbonHandlers();

            BacklogGrid.AddHandler(MouseDownEvent, new MouseButtonEventHandler(BacklogGrid_MouseDown), true);
            BacklogGrid.SelectedCellsChanged += BacklogGrid_SelectedCellsChanged;

            SetUpSecondPanel();

            //if (Globals.ItemManager.IsDiscoveryComplete && CurrentSortColumn == null)
            {
                SortByBusinessRank(BacklogGrid);
            }

            BacklogGrid.Sorting += BacklogGrid_Sorting;

            ShowHideViewElements(ViewManager.CurrentView.ViewName);
            ViewManager.ViewSelectionChanged += ViewManager_ViewSelectionChanged;
            CalculateCommitmentApprovalStatus();
            SetWorkItemGridHeading();
        }

        void Backlog_CollectionCountChanged(object sender, CollectionCountChangedEventArgs e)
        {
            int backlogItemCount = e.NewCount;
            BacklogItemCountBox.Text = "(" + backlogItemCount + ")";
            TrainReviewBacklogItemCountBox.Text = BacklogItemCountBox.Text;
        }

        void UncommittedBacklogCollection_CollectionCountChanged(object sender, CollectionCountChangedEventArgs e)
        {
            int backlogItemCount = e.NewCount;
            UncommittedBacklogItemCountBox.Text = "(" + backlogItemCount + ")";
        }

        void BacklogViewState_TrainChanged(object sender, EventArgs e)
        {
            BacklogItem.CurrentBacklogTrain = ViewState.TrainState.CurrentItem;
            CalculateCommitmentApprovalStatus();
            ShowHideViewElements(ViewManager.CurrentView.ViewName);
        }

        public void TabViewActivated()
        {
            if (ViewState != null)
            {
                BacklogItem.CurrentBacklogTrain = ViewState.TrainState.CurrentItem;
            }

            if (ViewManager != null)
            {
                BacklogItem.CurrentBacklogView = ViewManager.CurrentView.ViewName;
            }
        }

        void BacklogViewState_PillarChanged(object sender, EventArgs e)
        {
            CalculateCommitmentApprovalStatus();
        }

        string CurrentSortColumn = null;
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Custom Sort handler for those columns for which we want to do custom sorting.
        /// </summary>
        //------------------------------------------------------------------------------------
        void BacklogGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            DataGridColumn column = e.Column;
            CurrentSortColumn = (string)column.Header;

            if ((string)column.Header == PlannerContent.ColumnHeaderLandingDate)
            {
                Utils.SetCustomSortingForColumn(BacklogGrid, new LandingDateSort(), e);
            }
            else if ((string)column.Header == PlannerContent.ColumnHeaderBusinessRank)
            {
                Utils.SetCustomSortingForColumn(BacklogGrid, new BusinessRankSort(), e);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Hooks up all required EventManager event handlers.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SetUpEventManagerHandlers()
        {
            Globals.EventManager.PropertyChangedCanceled += Instance_PropertyChangedCanceled;
            Globals.EventManager.PlannerRefreshStarting += Handle_PlannerRefreshStarting;
            Globals.EventManager.CommitmentStatusComputationComplete += Instance_CommitmentStatusComputationComplete;
            Globals.EventManager.DiscoveryComplete += Handle_DiscoveryComplete;
            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;
        }

        void Instance_CommitmentStatusComputationComplete()
        {
            CalculateCommitmentApprovalStatus();
            ShowHideViewElements(ViewManager.CurrentView.ViewName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Hooks up all required EventManager event handlers.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SetUpRibbonHandlers()
        {
            Globals.MainWindow.BacklogCreateButton.Click += BacklogCreateButton_Click;
            Globals.MainWindow.BacklogDeleteButton.Click += BacklogDeleteButton_Click;
            Globals.MainWindow.BacklogEditButton.Click += BacklogEditButton_Click;
            Globals.MainWindow.ShowHideWorkItemsPanelButton.Click += ShowHideWorkItemsPanelButton_Click;
            Globals.MainWindow.BacklogMoveWorkItemsButton.Click += BacklogMoveWorkItemsButton_Click;
            Globals.MainWindow.BacklogCopyButton.Click += BacklogCopyButton_Click;
            Globals.MainWindow.BacklogAssignToScenarioButton.Click += BacklogAssignToScenarioButton_Click;
            Globals.MainWindow.ApproveBacklogCommitmentsButton.Click += ApproveBacklogCommitmentsButton_Click;
            Globals.MainWindow.ApprovedCommitmentsChartButton.Click += ApprovedCommitmentsChartButton_Click;
        }

        void ApprovedCommitmentsChartButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.CommitmentStatusManager.ShowCommitmentStatusCharts();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes the settings for the first panel views.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SetUpFirstPanel()
        {
            // Set up view manager
            ViewManager = new ColumnViewManager(BacklogGrid, ProductPreferences.LastSelectedBacklogViewValue, ProductPreferences.LastSelectedBacklogViewTypeValue, AvailableViews.BacklogStandardView, ViewType.DataGridView);
            ViewManager.AddViewControl(AvailableViews.BacklogStandardView, Globals.MainWindow.BacklogStandardViewRadioButton);
            ViewManager.AddViewControl(AvailableViews.BacklogPlanningColumnsView, Globals.MainWindow.BacklogSpecStatusViewRadioButton);
            ViewManager.AddViewControl(AvailableViews.BacklogTrainReviewView, Globals.MainWindow.BacklogReviewViewRadioButton);

            //ViewManager.AddDataGridVisibilityElement(BacklogGrid);
            //ViewManager.AddDataGridVisibilityElement(StandardViewSplitter);
            //ViewManager.AddDataGridVisibilityElement(StandardViewWorkItemsGrid);

            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, PlannerContent.ColumnHeaderBusinessRank);
            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Train");

            if (Globals.ApplicationManager.IsCurrentUserOPlannerDev())
            {
                //ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Pillar");
            }

            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Commitment Setting");
            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Commitment Status");
            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Story Points");
            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Work Scheduled");
            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Work Completed");
            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Work Remaining");
            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, PlannerContent.ColumnHeaderLandingDate);
            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Resolution Status");
            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Scrum Team");
            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Assigned Dev Team Members");
            ViewManager.AddViewColumn(AvailableViews.BacklogStandardView, "Assigned Test Team Members");

            ViewManager.AddViewColumn(AvailableViews.BacklogPlanningColumnsView, PlannerContent.ColumnHeaderBusinessRank);
            ViewManager.AddViewColumn(AvailableViews.BacklogPlanningColumnsView, "Train");
            ViewManager.AddViewColumn(AvailableViews.BacklogPlanningColumnsView, "Scenario");
            ViewManager.AddViewColumn(AvailableViews.BacklogPlanningColumnsView, "Commitment Setting");
            ViewManager.AddViewColumn(AvailableViews.BacklogPlanningColumnsView, "Spec");
            ViewManager.AddViewColumn(AvailableViews.BacklogPlanningColumnsView, "Spec Status");
            ViewManager.AddViewColumn(AvailableViews.BacklogPlanningColumnsView, "Spec Status Comments");
            ViewManager.AddViewColumn(AvailableViews.BacklogPlanningColumnsView, "PM Owner");
            ViewManager.AddViewColumn(AvailableViews.BacklogPlanningColumnsView, "Design Status");

            ViewManager.AddViewColumn(AvailableViews.BacklogTrainReviewView, PlannerContent.ColumnHeaderBusinessRank);
            ViewManager.AddViewColumn(AvailableViews.BacklogTrainReviewView, "Train");
            ViewManager.AddViewColumn(AvailableViews.BacklogTrainReviewView, "Commitment Setting");
            ViewManager.AddViewColumn(AvailableViews.BacklogTrainReviewView, "Commitment Status");
            ViewManager.AddViewColumn(AvailableViews.BacklogTrainReviewView, "Story Points");
            ViewManager.AddViewColumn(AvailableViews.BacklogTrainReviewView, "Work Scheduled");
            ViewManager.AddViewColumn(AvailableViews.BacklogTrainReviewView, "Work Completed");
            ViewManager.AddViewColumn(AvailableViews.BacklogTrainReviewView, "Work Remaining");
            ViewManager.AddViewColumn(AvailableViews.BacklogTrainReviewView, PlannerContent.ColumnHeaderLandingDate);
            ViewManager.AddViewColumn(AvailableViews.BacklogTrainReviewView, "Resolution Status");
            ViewManager.AddViewColumn(AvailableViews.BacklogTrainReviewView, "Commitment Status");

            ViewManager.InitializeView();
            ViewState.ViewManager = ViewManager;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes the settings for the second panel views.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SetUpSecondPanel()
        {
            Globals.MainWindow.ShowHideWorkItemsPanelButton.IsChecked = Utils.GetBoolValue(Globals.UserPreferences.GetProductPreference(ProductPreferences.LastSelectedShowHideBacklogSecondPanel));

            bool isSecondPanelViewSelected = false;
            string lastSelectedSecondPanelViewName = Globals.UserPreferences.GetProductPreference(ProductPreferences.LastSelectedBacklogSecondPanelView);
            foreach (RibbonRadioButton button in Globals.MainWindow.BacklogSecondPanelRibbonGroup.Items)
            {
                button.Click += SecondPanelViewButtonClicked;
                button.IsChecked = false;
                if (Utils.StringsMatch(button.Name, lastSelectedSecondPanelViewName))
                {
                    button.IsChecked = true;
                    isSecondPanelViewSelected = true;
                    SecondPanelViewButtonClicked(button);
                }

            }

            ShowHideSecondPanel(Globals.MainWindow.ShowHideWorkItemsPanelButton.IsChecked == true);

            if (!isSecondPanelViewSelected)
            {
                Globals.MainWindow.BacklogWorkItemsViewRadioButton.IsChecked = true;
                SecondPanelViewButtonClicked(Globals.MainWindow.BacklogWorkItemsViewRadioButton);
            }

            if (Globals.MainWindow.ShowHideWorkItemsPanelButton.IsChecked == true)
            {
                Globals.MainWindow.BacklogSecondPanelRibbonGroup.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Globals.MainWindow.BacklogSecondPanelRibbonGroup.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void EnsureWorkItemsPanelIsShowing()
        {
            if (Globals.MainWindow.ShowHideWorkItemsPanelButton.IsChecked == false)
            {
                ShowHideSecondPanel(true);
                if (Globals.MainWindow.BacklogWorkItemsViewRadioButton.IsChecked == false)
                {
                    Globals.MainWindow.BacklogWorkItemsViewRadioButton.IsChecked = true;
                    SecondPanelViewButtonClicked(Globals.MainWindow.BacklogWorkItemsViewRadioButton);
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has clicked on a button to change the second panel view.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SecondPanelViewButtonClicked(object sender, RoutedEventArgs e)
        {
            SecondPanelViewButtonClicked(sender as RibbonRadioButton);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has clicked on a button to change the second panel view.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SecondPanelViewButtonClicked(RibbonRadioButton button)
        {
            if (button != null)
            {
                Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedBacklogSecondPanelView, button.Name);

                StandardViewWorkItemsGrid.Visibility = System.Windows.Visibility.Hidden;
                MemberAssignmentsGridView.Visibility = System.Windows.Visibility.Hidden;
                UncommittedBacklogItemsPanel.Visibility = System.Windows.Visibility.Hidden;

                switch (button.Name)
                {
                    case "BacklogWorkItemsViewRadioButton":
                        StandardViewWorkItemsGrid.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case "UncommittedBacklogItemsViewRadioButton":
                        UncommittedBacklogItemsPanel.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case "BacklogSpecAllPeopleViewRadioButton":
                        MemberAssignmentsGridView.Visibility = System.Windows.Visibility.Visible;
                        ViewState.ShowOnlyUnassignedMembers = false;
                        break;
                    case "BacklogUnassignedPeopleViewRadioButton":
                        MemberAssignmentsGridView.Visibility = System.Windows.Visibility.Visible;
                        ViewState.ShowOnlyUnassignedMembers = true;
                        break;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Approve Commitments
        /// </summary>
        //------------------------------------------------------------------------------------
        void ApproveBacklogCommitmentsButton_Click(object sender, RoutedEventArgs e)
        {
            bool shouldClearSnapshot = false;
            Nullable<DateTime> approvalDate;
            string approver;
            if (Globals.CommitmentStatusManager.IsApprovedSnapshotAvailable(ViewState.PillarState.CurrentItem, ViewState.TrainState.CurrentItem, out approvalDate, out approver))
            {
                if (Globals.ApplicationManager.IsCurrentUserAdmin())
                {
                    if (!UserMessage.ShowOkCancel(Globals.MainWindow, "A snapshot of committed backlog items for the selected pillar and train has already been taken.  As an OPlanner Admin, are you sure you want to take a new snapshot (replacing the old)?", "Train Commitment Snapshot"))
                    {
                        return;
                    }

                    shouldClearSnapshot = true;
                }
                else
                {
                    UserMessage.Show("A snapshot of committed backlog items for the selected pillar and train has already been taken.  To view this snapshot, you can switch to the 'Train Review' Backlog view using the ribbon.");
                    return;
                }
            }

            if (!Globals.ApplicationManager.ConfirmIsAdmin("Approve Train Commitments"))
            {
                return;
            }

            if (ViewManager.CurrentView.ViewName == AvailableViews.BacklogTrainReviewView)
            {
                UserMessage.Show("Using the Ribbon, please switch away from the 'Train Review' Backlog view to one of the other views before attempting to approve commitments.");
                return;
            }

            if (!StoreItem.IsRealItem(ViewState.PillarState.CurrentItem))
            {
                UserMessage.Show("Please select the pillar for which you want to approve backlog items to be committed.");
                return;
            }

            if (!StoreItem.IsRealItem(ViewState.TrainState.CurrentItem))
            {
                UserMessage.Show("Please select the train for which you will commit to finishing the approved backlog items by.");
                return;
            }

            if (!Globals.ApplicationManager.IsCurrentUserOPlannerDev())
            {
                if (ViewState.TrainState.CurrentItem.TimeFrame != TrainTimeFrame.Current && ViewState.TrainState.CurrentItem.TimeFrame != TrainTimeFrame.Future)
                {
                    UserMessage.Show("Please select a current or future train for which you will commit to finishing the approved backlog items by.");
                    return;
                }
            }

            CommitmentSettingItem currentStatus = StatusCombo.SelectedItem as CommitmentSettingItem;
            if (currentStatus == null || (currentStatus.Value != CommitmentSettingValues.Committed && currentStatus.Value != CommitmentSettingValues.aaCommitted_or_In_Progresszz && currentStatus.Value != CommitmentSettingValues.aaCommitted_or_In_Progress_or_Completedzz))
            {
                UserMessage.ShowTwoLines("You need to set the Status filter in the ribbon to display all your Committed and/or 'In Progress' items, so that OPlanner knows the set of committed items that you want to approve.", "If you don't have any backlog items set to the committed state, You can open the 'Second Panel' using the ribbon to display all the currently uncommitted items for your pillar, and then set those items to the 'Committed' state that you want to commit to for the current train.");
                return;
            }

            if (BacklogCollection.Count == 0)
            {
                UserMessage.Show("There are no backlog items in the current view to be approved. You can open the 'Second Panel' using the ribbon to display all the currently uncommitted items for your pillar, and then set those items to the 'Committed' state that you want to commit to for the current train.");
                return;
            }

            ApproveTrainCommitmentsDialog dialog = new ApproveTrainCommitmentsDialog(ViewState.PillarState.CurrentItem, ViewState.TrainState.CurrentItem, BacklogCollection.Count);
            dialog.ShowDialog();
            if (dialog.IsDialogConfirmed)
            {
                if (shouldClearSnapshot)
                {
                    Globals.CommitmentStatusManager.ClearSnapshot(ViewState.PillarState.CurrentItem, ViewState.TrainState.CurrentItem);
                }

                foreach (BacklogItem item in BacklogCollection.Items)
                {
                    item.SetCommittedToTrain(ViewState.TrainState.CurrentItem);
                }

                CalculateCommitmentApprovalStatus();
                ShowHideViewElements(ViewManager.CurrentView.ViewName);
            }
        }

        void ViewManager_ViewSelectionChanged(AvailableViews oldViewName, AvailableViews newViewName)
        {
            BacklogItem.CurrentBacklogView = newViewName;
            if (newViewName == AvailableViews.BacklogStandardView || newViewName == AvailableViews.BacklogPlanningColumnsView)
            {
                ShowHideViewElements(newViewName);
                if (oldViewName == AvailableViews.BacklogTrainReviewView)
                {
                    RefreshBacklogview();
                }
            }
            else if (newViewName == AvailableViews.BacklogTrainReviewView)
            {
                ShowHideViewElements(newViewName);
                if (oldViewName == AvailableViews.BacklogStandardView || oldViewName == AvailableViews.BacklogPlanningColumnsView)
                {
                    RefreshBacklogview();
                }
            }
        }

        void RefreshBacklogview()
        {
            BacklogCollection.RefreshCollection();
        }

        void ShowHideViewElements(AvailableViews newViewName)
        {
            if (newViewName == AvailableViews.BacklogTrainReviewView)
            {
                Globals.MainWindow.BacklogStatusFilterPanel.Visibility = System.Windows.Visibility.Hidden;
                StandardTopPanelHeading.Visibility = System.Windows.Visibility.Hidden;
                if (!Globals.ItemManager.IsDiscoveryComplete)
                {
                    TrainReviewNotReadyTopPanelHeading.Visibility = System.Windows.Visibility.Visible;
                    TrainReviewTopPanelHeading.Visibility = System.Windows.Visibility.Hidden;
                    TrainReviewNotReadyBox.Text = Globals.CommitmentsApprovalNotDetermined;
                }
                else if (CurrentCommitmentApprovalStatus == ApprovalStatus.NotApproved)
                {
                    TrainReviewNotReadyTopPanelHeading.Visibility = System.Windows.Visibility.Visible;
                    TrainReviewTopPanelHeading.Visibility = System.Windows.Visibility.Hidden;
                    TrainReviewNotReadyBox.Text = Globals.CommitmentsNotApproved;
                }
                else if (CurrentCommitmentApprovalStatus == ApprovalStatus.Indeterminate)
                {
                    TrainReviewNotReadyTopPanelHeading.Visibility = System.Windows.Visibility.Visible;
                    TrainReviewTopPanelHeading.Visibility = System.Windows.Visibility.Hidden;
                    TrainReviewNotReadyBox.Text = Globals.CommitmentsNotApplicable;
                }
                else
                {
                    TrainReviewNotReadyTopPanelHeading.Visibility = System.Windows.Visibility.Hidden;
                    TrainReviewTopPanelHeading.Visibility = System.Windows.Visibility.Visible;
                    if (StoreItem.IsRealItem(ViewState.PillarState.CurrentItem))
                    {
                        TrainReviewPillarBox.Text = ViewState.PillarState.CurrentItem.Title;
                    }

                    if (StoreItem.IsRealItem(ViewState.TrainState.CurrentItem))
                    {
                        TrainReviewTrainBox.Text = ViewState.TrainState.CurrentItem.Title;
                    }

                    TrainReviewSnapshotDateBox.Text = CurrentCommitmentApprovalDate == null ? "" : CurrentCommitmentApprovalDate.Value.ToLongDateString();
                    TrainReviewApproverBox.Text = CurrentCommitmentApprover;
                }
            }
            else
            {
                Globals.MainWindow.BacklogStatusFilterPanel.Visibility = System.Windows.Visibility.Visible;
                StandardTopPanelHeading.Visibility = System.Windows.Visibility.Visible;
                TrainReviewNotReadyTopPanelHeading.Visibility = System.Windows.Visibility.Hidden;
                TrainReviewTopPanelHeading.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Populates the filter combo boxes with appropriate values for backlog filtering.
        /// </summary>
        //------------------------------------------------------------------------------------
        void PopulateFilters()
        {
            // Commitment Status filtering
            StatusCombo.ItemsSource = CommitmentSettingItem.CommitmentSettingValues;
            string selectedItemText = Globals.UserPreferences.GetProductPreference(ProductPreferences.BacklogViewLastSelectedStatus);
            ViewState.CurrentCommitmentSetting = null;
            if (!string.IsNullOrWhiteSpace(selectedItemText))
            {
                ViewState.CurrentCommitmentSetting = CommitmentSettingItem.GetCommitmentSettingItem(selectedItemText);
            }
            else
            {
                ViewState.CurrentCommitmentSetting = CommitmentSettingItem.GetCommitmentSettingItem(CommitmentSettingValues.aaExclude_Completed_Itemszz);
            }
            StatusCombo.SelectedItem = ViewState.CurrentCommitmentSetting;
            ViewState.CurrentCommitmentSetting = StatusCombo.SelectedItem as CommitmentSettingItem;
            StatusCombo.SelectionChanged += StatusCombo_SelectionChanged;

        }

        void StatusCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewState.CurrentCommitmentSetting = StatusCombo.SelectedItem as CommitmentSettingItem;
            CommitmentStatusSelectionChanged = true;
            CommitmentSettingItem commitmentStatusItem = StatusCombo.SelectedItem as CommitmentSettingItem;
            if (commitmentStatusItem != null)
            {
                Globals.UserPreferences.SetEnumSelectionPreference<CommitmentSettingValues>(ProductPreferences.BacklogViewLastSelectedStatus, commitmentStatusItem.Value);
            }
        }

        void CalculateCommitmentApprovalStatus()
        {
            BackgroundTask approvalStatusWorker = new BackgroundTask(false);
            approvalStatusWorker.DoWork += approvalStatusWorker_DoWork;
            approvalStatusWorker.TaskCompleted += approvalStatusWorker_TaskCompleted;
            approvalStatusWorker.RunTaskAsync();
        }

        void approvalStatusWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (StoreItem.IsRealItem(ViewState.PillarState.CurrentItem) && StoreItem.IsRealItem(ViewState.TrainState.CurrentItem))
            {
                bool isAvailable = Globals.CommitmentStatusManager.IsApprovedSnapshotAvailable(ViewState.PillarState.CurrentItem, ViewState.TrainState.CurrentItem, out CurrentCommitmentApprovalDate, out CurrentCommitmentApprover);
                CurrentCommitmentApprovalStatus = isAvailable ? ApprovalStatus.Approved : ApprovalStatus.NotApproved;
            }
            else
            {
                CurrentCommitmentApprovalStatus = ApprovalStatus.Indeterminate;
            }
        }

        void approvalStatusWorker_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            if (!Globals.ItemManager.IsDiscoveryComplete)
            {
                CommitmentApprovalStatusBox.Text = Globals.CommitmentsApprovalNotDetermined;
                CommitmentApprovalStatusBox.Foreground = Brushes.Black;
            }
            else if (CurrentCommitmentApprovalStatus == ApprovalStatus.Approved)
            {
                CommitmentApprovalStatusBox.Text = Globals.CommitmentsApproved;
                CommitmentApprovalStatusBox.Foreground = Brushes.Green;
            }
            else if (CurrentCommitmentApprovalStatus == ApprovalStatus.NotApproved)
            {
                CommitmentApprovalStatusBox.Text = Globals.CommitmentsNotApproved;
                CommitmentApprovalStatusBox.Foreground = Brushes.Red;
            }
            else if (CurrentCommitmentApprovalStatus == ApprovalStatus.Indeterminate)
            {
                CommitmentApprovalStatusBox.Text = "";
                CommitmentApprovalStatusBox.Foreground = Brushes.Black;
            }
            else
            {
                CommitmentApprovalStatusBox.Text = "";
            }

            ShowHideViewElements(ViewManager.CurrentView.ViewName);
        }

        public bool MembersViewFilterCallback(object viewObject)
        {
            GroupMemberItem memberItem = viewObject as GroupMemberItem;
            if (memberItem != null)
            {
                PillarItem selectedPillar = ViewState.PillarState.CurrentItem;
                if (StoreItem.IsRealItem(selectedPillar))
                {
                    if (memberItem.ParentPillarItem != selectedPillar)
                    {
                        return false;
                    }

                    if (ViewState.ShowOnlyUnassignedMembers && memberItem.TotalWorkRemaining > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        void Handle_DiscoveryComplete(object sender, EventArgs e)
        {
            UpdateUncommittedBacklogItemCount();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Brings up a dialog allowing the user to assign all the selected backlog items to
        /// a scenario.
        /// </summary>
        //------------------------------------------------------------------------------------
        void BacklogAssignToScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            if (BacklogGrid.SelectedItems.Count > 0)
            {
                AsyncObservableCollection<BacklogItem> backlogItems = new AsyncObservableCollection<BacklogItem>();
                foreach (var item in BacklogGrid.SelectedItems)
                {
                    backlogItems.Add(item as BacklogItem);
                }

                PillarItem selectedPillar = ViewState.PillarState.CurrentItem;
                BacklogAssignToScenarioDialog dialog = new BacklogAssignToScenarioDialog(backlogItems, selectedPillar);
                dialog.ShowDialog();
            }
        }

        void BacklogCopyButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<DataGridColumn> columns = BacklogGrid.Columns;
            StringBuilder lines = new StringBuilder();
            StringBuilder line = new StringBuilder();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(BacklogGrid.ItemsSource);

            foreach (object item in view)
            {
                BacklogItem backlogItem = (BacklogItem)item;
                if (backlogItem != null)
                {
                    line.Clear();
                    foreach (DataGridColumn column in columns)
                    {
                        if (column.Visibility == System.Windows.Visibility.Visible)
                        {
                            string prop = column.SortMemberPath;
                            if (!string.IsNullOrWhiteSpace(prop))
                            {
                                System.Reflection.PropertyInfo propInfo = backlogItem.GetType().GetProperty(prop);
                                if (propInfo != null)
                                {
                                    object value = propInfo.GetValue(backlogItem, null);
                                    string text;
                                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                                    {
                                        text = Globals.c_NotSet;
                                    }
                                    else
                                    {
                                        text = value.ToString();
                                    }

                                    line.Append(text);
                                    line.Append('\t');
                                }
                            }
                        }
                    }

                    lines.AppendLine(line.ToString());
                }
            }

            Clipboard.SetText(lines.ToString());
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// User has clicked the button to request creation of a new backlog item.
        /// </summary>
        //------------------------------------------------------------------------------------
        void BacklogCreateButton_Click(object sender, RoutedEventArgs e)
        {
            BacklogItem.RequestNewBacklogItem(ViewState.PillarState.CurrentItem, ViewState.TrainState.CurrentItem);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called when the user hits the button to refresh all views.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Handle_PlannerRefreshStarting()
        {
            BacklogItem.RefreshCommonValues();
        }

        void UpdateUncommittedBacklogItemCount()
        {
            int backlogItemCount = UncommittedBacklogCollection.Count;
            UncommittedBacklogItemCountBox.Text = "(" + backlogItemCount + ")";
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called when the user hits the button to move the selected work items to live under
        /// a different backlog item.
        /// </summary>
        //------------------------------------------------------------------------------------
        void BacklogMoveWorkItemsButton_Click(object sender, RoutedEventArgs e)
        {
            MoveWorkItems();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the heading above the WorkItems grid.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SetWorkItemGridHeading()
        {
            if (CurrentBacklogItem != null)
            {
                WorkItemGridHeading.Text = "Work Items for \"" + CurrentBacklogItem.Title + "\"";
            }
            else
            {
                WorkItemGridHeading.Text = "(Select a backlog item to see work items)";
                WorkItemGrid.ItemsSource = null;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called when the user selects a different cell in the BacklogGrid.
        /// </summary>
        //------------------------------------------------------------------------------------
        void BacklogGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid != null)
            {
                CurrentBacklogItem = grid.CurrentItem as BacklogItem;
                SetWorkItemGridHeading();

                if (CurrentBacklogItem != null)
                {
                    WorkItemGrid.ItemsSource = CurrentBacklogItem.WorkItems;
                }
            }
        }

        void BacklogDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (BacklogGrid.SelectedItems.Count > 1)
            {
                DeleteSelectedBacklogItemsDialog dialog = new DeleteSelectedBacklogItemsDialog(BacklogGrid.SelectedItems);
                dialog.ShowDialog();
                if (dialog.DialogConfirmed)
                {
                    CurrentBacklogItem = null;
                    SetWorkItemGridHeading();
                }
            }

            else if (CurrentBacklogItem != null)
            {
                if (CurrentBacklogItem.RequestDeleteItem())
                {
                    CurrentBacklogItem = null;
                    SetWorkItemGridHeading();
                }
            }
        }

        void BacklogEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentBacklogItem != null)
            {
                CurrentBacklogItem.ShowBacklogItemEditor();
            }
        }

        void SortByBusinessRank(ItemsControl itemsControl)
        {
            CurrentSortColumn = PlannerContent.ColumnHeaderBusinessRank;
            Utils.SetCustomSorting(itemsControl, new BusinessRankSort(), ListSortDirection.Ascending);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the value object associated with the given field is null.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool IsFieldValueNull(ProductStudio.Field field)
        {
            return field.Value is System.DBNull;
        }

        void ShowHideSecondPanel(bool show)
        {
            if (show)
            {
                WorkItemsRow.Height = CurrentWorkItemsPanelHeight;
                WorkItemsSplitterRow.Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto);
                Globals.MainWindow.BacklogSecondPanelRibbonGroup.Visibility = System.Windows.Visibility.Visible;
                Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedShowHideBacklogSecondPanel, true.ToString());
            }
            else
            {
                CurrentWorkItemsPanelHeight = WorkItemsRow.Height;
                WorkItemsRow.Height = new System.Windows.GridLength(0);
                WorkItemsSplitterRow.Height = new System.Windows.GridLength(0);
                Globals.MainWindow.BacklogSecondPanelRibbonGroup.Visibility = System.Windows.Visibility.Collapsed;
                Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedShowHideBacklogSecondPanel, false.ToString());
            }
        }

        void ShowHideWorkItemsPanelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ShowHideSecondPanel(Globals.MainWindow.ShowHideWorkItemsPanelButton.IsChecked == true);
        }

        private void PillarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PillarCanceledBacklogItem != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    ComboBox pillarComboBox = sender as ComboBox;
                    pillarComboBox.SelectedItem = PillarCanceledBacklogItem.ParentPillarItem;
                    PillarCanceledBacklogItem = null;
                    BacklogGrid.CancelEdit();
                }), null);
            }
        }

        private void TrainComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TrainCanceledBacklogItem != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    ComboBox comboBox = sender as ComboBox;
                    comboBox.SelectedItem = TrainCanceledBacklogItem.ParentTrainItem;
                    TrainCanceledBacklogItem = null;
                    BacklogGrid.CancelEdit();
                }), null);
            }

            BacklogItem backlogItem = GetComboBoxBacklogItem(sender, e);
            if (backlogItem != null)
            {
                TrainItem oldTrain = e.RemovedItems[0] as TrainItem;
                TrainItem newTrain = e.AddedItems[0] as TrainItem;
                if (backlogItem.IsCommittedToTrain(oldTrain))
                {
                    if (UserMessage.ShowYesNo(Globals.MainWindow, PlannerContent.BacklogValidationChangeTrainOfCommittedItemMessage, "Change Backlog Item Train"))
                    {

                    }
                }
            }
        }

        private void BacklogGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataGridCell cell = Keyboard.FocusedElement as DataGridCell;
            if (cell != null)
            {
                if (cell.Column is DataGridTemplateColumn)
                {
                    BacklogGrid.BeginEdit();
                    ComboBox combo;
                    GetChildComboBox(cell, out combo);
                    Keyboard.Focus(combo);
                    combo.IsDropDownOpen = true;
                }
            }
        }

        void GetChildComboBox(Visual myVisual, out ComboBox combo)
        {
            combo = null;
            Visual oldVisual = myVisual;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);
                if (childVisual is ComboBox)
                {
                    combo = (ComboBox)childVisual;
                    return;
                }
                GetChildComboBox(childVisual, out combo);
            }
        }

        private void BacklogGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = Keyboard.FocusedElement as DataGridCell;
            if (cell != null)
            {
                if (cell.Column is DataGridTemplateColumn)
                {
                    ComboBox combo;
                    GetChildComboBox(cell, out combo);
                    if (combo != null)
                    {
                        BacklogGrid.BeginEdit();
                        Keyboard.Focus(combo);
                        combo.IsDropDownOpen = true;
                    }
                }
            }
        }

        void Instance_PropertyChangedCanceled(object source, string propName)
        {
            BacklogItem item = source as BacklogItem;
            if (item != null)
            {
                if (propName == "ParentPillarItem")
                {
                    PillarCanceledBacklogItem = item;
                }
                else if (propName == "ParentTrainItem")
                {
                    TrainCanceledBacklogItem = item;
                }
            }
        }

        private void BacklogGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element != null && element is FrameworkElement)
            {
                if (((FrameworkElement)element).Parent is DataGridCell)
                {
                    BacklogItem backlogItem = BacklogGrid.SelectedItem as BacklogItem;
                    if (backlogItem != null)
                    {
                        backlogItem.ShowBacklogItemEditor();
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Allows the user to move the selected work items to a different parent backlog
        /// item.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void MoveWorkItems()
        {
            if (WorkItemGrid.SelectedItems.Count > 0)
            {
                AsyncObservableCollection<WorkItem> workItems = new AsyncObservableCollection<WorkItem>();
                foreach (var item in WorkItemGrid.SelectedItems)
                {
                    workItems.Add(item as WorkItem);
                }

                ScrumTeamItem scrumTeamItem = null;
                MoveWorkItemsDialog dialog = new MoveWorkItemsDialog(workItems, ViewState.PillarState.CurrentItem, ViewState.TrainState.CurrentItem, scrumTeamItem);
                dialog.ShowDialog();
            }
        }

        private void NewWorkItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                BacklogItem backlogItem = menuItem.DataContext as BacklogItem;
                if (backlogItem != null)
                {
                    WorkItem newWorkItem = WorkItem.CreateWorkItem(backlogItem);
                    newWorkItem.SaveNewItem();
                }
            }
        }

        private void SpecStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BacklogItem backlogItem = GetComboBoxBacklogItem(sender, e);
            if (backlogItem != null)
            {
                StoreSpecStatusValue specStatusValue = StoreSpecStatus.GetStoreSpecStatus(backlogItem.StoreSpecStatusText);
                if (specStatusValue == StoreSpecStatusValue.ReadyForCoding || specStatusValue == StoreSpecStatusValue.SpecFinalized)
                {
                    if (!backlogItem.IsSpecSet)
                    {
                        AllowedValue originalSpecStatus = e.RemovedItems[0] as AllowedValue;
                        if (originalSpecStatus != null)
                        {
                            backlogItem.OriginalStoreSpecStatusText = (string)originalSpecStatus.Value;
                        }

                        SetBacklogValidationError(BacklogValidationErrorValue.InvalidSpecSetting, backlogItem);
                    }
                }

            }
        }

        private void SpecComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BacklogItem item = GetComboBoxBacklogItem(sender, e);
            if (item != null)
            {
                if (!item.IsSpecSet)
                {
                    item.StoreSpecStatusText = Globals.c_NotSet;
                    item.NotifyPropertyChanged(() => item.StoreSpecStatusText);
                }
            }
        }

        BacklogItem GetComboBoxBacklogItem(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && (comboBox.IsMouseCaptured || comboBox.IsKeyboardFocusWithin))
            {
                BacklogItem item = comboBox.DataContext as BacklogItem;
                return item;
            }

            return null;
        }

        private void CommitmentSettingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BacklogItem backlogItem = GetComboBoxBacklogItem(sender, e);
            if (backlogItem != null)
            {
                HandleCommitmentSettingChanged(backlogItem);
            }
        }

        private void MemberImage_MouseMove(object sender, MouseEventArgs e)
        {
            StackPanel memberPanel = (StackPanel)sender;
            if (memberPanel != null && e.LeftButton == MouseButtonState.Pressed)
            {
                GroupMemberItem member = memberPanel.DataContext as GroupMemberItem;
                if (member != null)
                {
                    //MemberDragInProgress = true;
                    DragDrop.DoDragDrop(memberPanel, member, DragDropEffects.Link);
                }
            }
        }

        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            var parent = element;
            while (parent != null)
            {
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        bool MemberDragInProgress { get; set; }
        Brush TitleBoxPreviousBackground { get; set; }
        private void BacklogItemTitle_DragEnter(object sender, DragEventArgs e)
        {
            var row = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);
            if (row != null)
            {
                TitleBoxPreviousBackground = row.Background;
                row.Background = Brushes.LightGoldenrodYellow;
            }
        }

        private void BacklogItemTitle_DragLeave(object sender, DragEventArgs e)
        {
            var row = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);
            if (row != null)
            {
                row.Background = TitleBoxPreviousBackground;
            }
        }

        BacklogItem BacklogItemAssignedTo;
        GroupMemberItem GroupMemberItemAssigned;
        private void BacklogItemTitle_Drop(object sender, DragEventArgs e)
        {
            var row = FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);
            if (row != null)
            {
                row.Background = TitleBoxPreviousBackground;
                BacklogItem backlogItemDraggedOnto = row.DataContext as BacklogItem;
                if (backlogItemDraggedOnto != null)
                {
                    if (e.Data.GetDataPresent(typeof(GroupMemberItem)))
                    {
                        GroupMemberItem member = (GroupMemberItem)e.Data.GetData(typeof(GroupMemberItem));
                        if (member != null)
                        {
                            if (member.ActiveBacklogItems.Count > 0)
                            {
                                if (!UserMessage.ShowYesNo(Globals.MainWindow, "This user is already assigned to at least one active backlog item.  Are you sure you want to assign them to another?", "Assign Member to Backlog Item"))
                                {
                                    return;
                                }
                            }

                            AssignMemberToBacklogItem dialog = new AssignMemberToBacklogItem(member, backlogItemDraggedOnto);
                            dialog.ShowDialog();
                            if (dialog.IsConfirmed)
                            {
                                WorkItem newWorkItem = WorkItem.CreateWorkItem(backlogItemDraggedOnto);
                                newWorkItem.Title = "Finalize and check-in my work";
                                newWorkItem.AssignedTo = member.Alias;
                                newWorkItem.Estimate = 4;
                                newWorkItem.SaveNewItem();
                                BacklogItemAssignedTo = backlogItemDraggedOnto;
                                GroupMemberItemAssigned = member;
                                backlogItemDraggedOnto.NotifyPropertyChanged(() => backlogItemDraggedOnto.TotalWorkScheduled);
                                backlogItemDraggedOnto.NotifyPropertyChanged(() => backlogItemDraggedOnto.TotalWorkRemaining);
                                backlogItemDraggedOnto.NotifyPropertyChanged(() => backlogItemDraggedOnto.AssignedDevTeamMembers);
                                backlogItemDraggedOnto.NotifyPropertyChanged(() => backlogItemDraggedOnto.AssignedTestTeamMembers);
                                member.NotifyPropertyChanged(() => member.ActiveBacklogItems);
                            }
                        }
                    }
                }
            }
        }
    }
}
