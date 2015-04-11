using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlannerNameSpace.Views
{
    public enum MemberViews
    {
        CommittedWorkView,
        AllWorkView,
    }

    /// <summary>
    /// Interaction logic for MemberHomeView.xaml
    /// </summary>
    public partial class MemberHomeView : UserControl
    {
        GroupMemberItem m_currentMember;

        public GroupMemberItem CurrentMember
        {
            get { return m_currentMember; }
            set
            {
                m_currentMember = value;
                if (m_currentMember != null)
                {
                    Globals.UserPreferences.SetItemSelectionPreference(ProductPreferences.LastSelectedGroupMember, CurrentMember);
                    ViewContext.DataContext = m_currentMember;
                    BeginGetCurrentMemberStats();
                }
            }
        }

        bool ShowOnlyCommittedItems { get; set; }
        bool ShowOnlyActiveWorkItems { get; set; }
        ComboBox SelectTeamMemberCombo { get; set; }
        WorkItem SelectedWorkItem { get; set; }
        DataGrid SelectedWorkItemDataGrid { get; set; }
        DataGrid LastSelectedWorkItemDataGrid { get; set; }
        WorkItem LastCreatedWorkItem { get; set; }

        string TotalOffDaysText { get; set; }
        string TotalWorkRemainingText { get; set; }
        string TotalWorkCompletedText { get; set; }

        public MemberHomeView()
        {
            InitializeComponent();
        }

        public void InitializeData()
        {
            // Set up the last selected view
            string LastSelectedMemberViewValue = Globals.UserPreferences.GetProductPreference(ProductPreferences.LastSelectedMemberViewValue);
            if (LastSelectedMemberViewValue == MemberViews.CommittedWorkView.ToString())
            {
                Globals.MainWindow.MemberCommittedWorkViewRadioButton.IsChecked = true;
                ShowOnlyCommittedItems = true;
            }
            else if (LastSelectedMemberViewValue == MemberViews.AllWorkView.ToString())
            {
                Globals.MainWindow.MemberAllWorkViewRadioButton.IsChecked = true;
                ShowOnlyCommittedItems = false;
            }
            else
            {
                Globals.MainWindow.MemberCommittedWorkViewRadioButton.IsChecked = true;
                ShowOnlyCommittedItems = true;
            }

            // Set up ShowOnly Active WorkItems option
            ShowOnlyActiveWorkItems = Utils.GetBoolValue(Globals.UserPreferences.GetProductPreference(ProductPreferences.LastSelectedMemberViewShowOnlyActiveItemsValue));
            Globals.MainWindow.ShowActiveItemsCheckBox.IsChecked = ShowOnlyActiveWorkItems;
            Globals.MainWindow.ShowActiveItemsCheckBox.Checked += ShowActiveItemsCheckBox_Checked;
            Globals.MainWindow.ShowActiveItemsCheckBox.Unchecked += ShowActiveItemsCheckBox_Unchecked;

            // Set up the current member to be the last selected
            CurrentMember = Globals.UserPreferences.GetItemSelectionPreference<GroupMemberItem>(ProductPreferences.LastSelectedGroupMember);
            if (CurrentMember == null)
            {
                CurrentMember = Globals.GroupMemberManager.GetMemberByAlias(Globals.ApplicationManager.CurrentUserAlias);
            }

            ViewContext.DataContext = CurrentMember;

            SelectTeamMemberCombo = Globals.MainWindow.SelectTeamMemberCombo;
            SelectTeamMemberCombo.ItemsSource = Globals.GroupMembers.GetSortedGroupMembers();

            SelectTeamMemberCombo.SelectedItem = CurrentMember;
            SelectTeamMemberCombo.SelectionChanged += SelectTeamMemberCombo_SelectionChanged;

            Globals.MainWindow.NewMemberWorkItemButton.Click += NewMemberWorkItemButton_Click;
            Globals.MainWindow.DeleteMemberWorkItemButton.Click += DeleteMemberWorkItemButton_Click;
            Globals.MainWindow.MoveMemberWorkItemsButton.Click += MoveMemberWorkItemsButton_Click;

            Globals.MainWindow.MemberCommittedWorkViewRadioButton.Click += MemberCommittedWorkViewRadioButton_Click;
            Globals.MainWindow.MemberAllWorkViewRadioButton.Click += MemberAllWorkViewRadioButton_Click;
            Globals.EventManager.UpdateUI += Handle_UpdateUI;
            BeginGetCurrentMemberStats();

            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;
            Globals.EventManager.DiscoveryComplete += Handle_DiscoveryComplete;

        }

        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
        }

        void MemberAllWorkViewRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyCommittedItems = false;
            Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedMemberViewValue, MemberViews.AllWorkView.ToString());
            UpdateBacklogItems();
        }

        void MemberCommittedWorkViewRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyCommittedItems = true;
            Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedMemberViewValue, MemberViews.CommittedWorkView.ToString());
            UpdateBacklogItems();
        }

        public bool MemberBacklogItemsFilterCallback(object viewObject)
        {
            BacklogItem backlogItem = viewObject as BacklogItem;
            if (backlogItem != null)
            {
                if (ShowOnlyCommittedItems)
                {
                    if (!backlogItem.IsInProgressOrCompleted)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        void UpdateBacklogItems()
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(CurrentMember.BacklogItems);
            view.Refresh();
        }

        void ShowActiveItemsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowOnlyActiveWorkItems = false;
            Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedMemberViewShowOnlyActiveItemsValue, false.ToString());
            UpdateBacklogItemView();
        }

        void ShowActiveItemsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ShowOnlyActiveWorkItems = true;
            Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedMemberViewShowOnlyActiveItemsValue, true.ToString());
            UpdateBacklogItemView();
        }

        void UpdateBacklogItemView()
        {
        }

        public bool BacklogWorkItemsFilterCallback(object viewObject)
        {
            WorkItem workItem = viewObject as WorkItem;
            if (workItem != null)
            {
                if (workItem.AssignedToGroupMember == CurrentMember)
                {
                    if (ShowOnlyActiveWorkItems == true)
                    {
                        if (workItem.IsActive)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        void Handle_DiscoveryComplete(object sender, EventArgs e)
        {

        }

        void BeginGetCurrentMemberStats()
        {
            TotalWorkRemainingText = "(Calculating...)";
            AbortableBackgroundWorker worker = new AbortableBackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (CurrentMember != null)
            {
                TotalOffDaysText = CurrentMember.TotalOffDays;
                TotalWorkRemainingText = CurrentMember.TotalWorkRemainingDisplay;
                TotalWorkCompletedText = CurrentMember.TotalWorkCompleted.ToString();
            }
        }

        void Handle_UpdateUI()
        {
            Globals.MainWindow.NewMemberWorkItemButton.IsEnabled = SelectedWorkItem != null;
            Globals.MainWindow.DeleteMemberWorkItemButton.IsEnabled = SelectedWorkItem != null;

            if (SelectedWorkItemDataGrid != null && SelectedWorkItemDataGrid.SelectedItems.Count > 0)
            {
                Globals.MainWindow.MoveMemberWorkItemsButton.IsEnabled = true;
            }
            else
            {
                Globals.MainWindow.MoveMemberWorkItemsButton.IsEnabled = false;
            }

            if (LastCreatedWorkItem != null && LastSelectedWorkItemDataGrid != null)
            {
                LastSelectedWorkItemDataGrid = null;
                SelectedWorkItem = LastCreatedWorkItem;
            }

            if (CurrentMember != null)
            {
                TotalOffDaysBox.Text = TotalOffDaysText;
                TotalWorkRemainingDisplayBox.Text = TotalWorkRemainingText;
                TotalWorkCompletedBox.Text = TotalWorkCompletedText;
            }
        }

        void NewMemberWorkItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorkItem != null)
            {
                BacklogItem backlogItem = SelectedWorkItem.ParentBacklogItem;
                if (backlogItem != null)
                {
                    string subtype;
                    if (CurrentMember.Discipline == DisciplineValues.Dev)
                    {
                        subtype = SubtypeValues.ProductCoding;
                    }
                    else
                    {
                        subtype = SubtypeValues.Automation;
                    }

                    WorkItem newWorkItem = WorkItem.CreateWorkItem(backlogItem, "New WorkItem", subtype, CurrentMember.Alias);
                    newWorkItem.SaveNewItem();
                    CurrentMember.NotifyPropertyChanged(() => CurrentMember.BacklogItems);
                    LastCreatedWorkItem = newWorkItem;
                    LastSelectedWorkItemDataGrid = SelectedWorkItemDataGrid;
                }
            }
        }

        void MoveMemberWorkItemsButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorkItemDataGrid != null && SelectedWorkItemDataGrid.SelectedItems.Count > 0)
            {
                AsyncObservableCollection<WorkItem> workItems = new AsyncObservableCollection<WorkItem>();
                foreach (object item in SelectedWorkItemDataGrid.SelectedItems)
                {
                    WorkItem workItem = (WorkItem)item;
                    workItems.Add(workItem);
                }

                BacklogItem backlogItem = workItems.GetItem(0).ParentBacklogItem;
                PillarItem selectedPillar = backlogItem == null ? null : backlogItem.ParentPillarItem;
                TrainItem selectedTrain = backlogItem == null ? null : backlogItem.ParentTrainItem;
                MoveWorkItemsDialog dialog = new MoveWorkItemsDialog(workItems, selectedPillar, selectedTrain, null);
                dialog.ShowDialog();

            }
        }

        void DeleteMemberWorkItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorkItem != null)
            {
                SelectedWorkItem.RequestDeleteItem();
            }
        }

        void SelectTeamMemberCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentMember = SelectTeamMemberCombo.SelectedItem as GroupMemberItem;
            if (CurrentMember != null)
            {
                ViewContext.DataContext = CurrentMember;
                Globals.UserPreferences.SetItemSelectionPreference(ProductPreferences.LastSelectedGroupMember, CurrentMember);
                BeginGetCurrentMemberStats();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Attempts to extract a work item reference from the given sender object.
        /// </summary>
        //------------------------------------------------------------------------------------
        private WorkItem GetWorkItemFromSender(object sender)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                return menuItem.DataContext as WorkItem;
            }

            return null;
        }

        private void EditWorkItem_Click(object sender, RoutedEventArgs e)
        {
            WorkItem workItem = GetWorkItemFromSender(sender);
            if (workItem != null)
            {
                workItem.ShowWorkItemEditor();
            }
        }

        private void DeleteWorkItem_Click(object sender, RoutedEventArgs e)
        {
            WorkItem workItem = GetWorkItemFromSender(sender);
            if (workItem != null)
            {
                workItem.RequestDeleteItem();
            }
        }

        private void MoveWorkItem_Click(object sender, RoutedEventArgs e)
        {
            WorkItem workItem = GetWorkItemFromSender(sender);
            if (workItem != null)
            {
                AsyncObservableCollection<WorkItem> workItems = new AsyncObservableCollection<WorkItem>();
                workItems.Add(workItem);
                BacklogItem backlogItem = workItem.ParentBacklogItem;
                if (backlogItem != null)
                {
                    MoveWorkItemsDialog dialog = new MoveWorkItemsDialog(workItems, backlogItem.ParentPillarItem, backlogItem.ParentTrainItem, null);
                    dialog.ShowDialog();
                }
            }
        }

        private void MoveWorkItem(WorkItem workItem)
        {
            if (workItem != null)
            {
                AsyncObservableCollection<WorkItem> workItems = new AsyncObservableCollection<WorkItem>();
                workItems.Add(workItem);
                BacklogItem backlogItem = workItem.ParentBacklogItem;
                if (backlogItem != null)
                {
                    MoveWorkItemsDialog dialog = new MoveWorkItemsDialog(workItems, backlogItem.ParentPillarItem, backlogItem.ParentTrainItem, null);
                    dialog.ShowDialog();
                }
            }
        }

        private void OffDaysClicked(object sender, MouseButtonEventArgs e)
        {
            if (CurrentMember != null)
            {
                CurrentMember.EditOffDays();
            }
        }

        private void EditParentBacklogItem_Click(object sender, RoutedEventArgs e)
        {
            WorkItem workItem = GetWorkItemFromSender(sender);
            if (workItem != null)
            {
                BacklogItem backlogItem = workItem.ParentBacklogItem;
                if (backlogItem != null)
                {
                    backlogItem.ShowBacklogItemEditor();
                }
            }
        }

        private void WorkItemsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
