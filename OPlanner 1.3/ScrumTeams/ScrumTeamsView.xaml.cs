using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PlannerNameSpace.Views
{
    /// <summary>
    /// Interaction logic for CapacityPlannerView.xaml
    /// </summary>
    public partial class ScrumTeamsView : UserControl
    {
        private CollectionView PlannerView;
        private delegate void BackgroundAccumulateWorkDelegate();
        ScrumTeamSelector ScrumTeamSelector;
        private BacklogItem CurrentBacklogItem;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// CapacityPlannerView Constructor
        /// </summary>
        //------------------------------------------------------------------------------------
        public ScrumTeamsView()
        {
            InitializeComponent();

            PlannerView = null;
            LastAssignedTo = null;
            LastWorkItemSubtype = null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Handler that's called only when the first full load of items from the host store
        /// has completed loading.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void InitializeData()
        {

            ScrumTeamSelector = new ScrumTeamSelector(Globals.MainWindow.SelectScrumTeamCombo, ProductPreferences.LastSelectedScrumTeamsViewTeam,
                Globals.MainWindow.ScrumTeamPillarFilterCombo, ProductPreferences.LastSelectedScrumTeamsViewPillar);
            ScrumTeamSelector.ScrumTeamItemSelectionChanged += ScrumTeamSelector_ScrumTeamItemSelectionChanged;

            Globals.EventManager.CreateWorkItem += CreateWorkItemCommand;
            Globals.EventManager.DeleteWorkItem += DeleteWorkItemCommand;
            Globals.EventManager.NewItemCommittedToStore += Handle_NewItemCommittedToStore;

            // Set up ribbon button handlers
            Globals.MainWindow.CreateScrumTeamButton.Click += CreateScrumTeamButton_Click;
            Globals.MainWindow.DeleteScrumTeamButton.Click += DeleteScrumTeamButton_Click;
            Globals.MainWindow.EditScrumTeamButton.Click += EditScrumTeamButton_Click;
            Globals.MainWindow.ScrumTeamBacklogCreateButton.Click += ScrumTeamBacklogCreateButton_Click;
            Globals.MainWindow.ScrumTeamBacklogDeleteButton.Click += CapacityBacklogDeleteButton_Click;
            Globals.MainWindow.ScrumTeamMoveWorkItemsButton.Click += MoveWorkItemsButton_Click;
            BacklogGrid.SelectionChanged += BacklogGrid_SelectionChanged;
            WorkItemsGrid.SelectedCellsChanged += WorkItemsGrid_SelectedCellsChanged;

            ScrumTeamItem currentScrumTeam = ScrumTeamSelector.CurrentScrumTeam;
            if (currentScrumTeam != null)
            {
                BacklogGrid.ItemsSource = currentScrumTeam.OwnedBacklogItems;
                WorkBarChart.DataContext = currentScrumTeam;
            }

            SetPlannerSortOrder();
            Globals.EventManager.UpdateUI += Handle_UpdateUI;
        }

        void ScrumTeamSelector_ScrumTeamItemSelectionChanged(object sender, ScrumTeamChangedEventArgs e)
        {
            ScrumTeamItem currentScrumTeam = e.CurrentItem;
            BacklogGrid.ItemsSource = currentScrumTeam.OwnedBacklogItems;
            WorkBarChart.DataContext = currentScrumTeam;

            SetPlannerSortOrder();
            WorkItemsGrid.ItemsSource = null;
            Globals.EventManager.OnScrumTeamViewTeamSelectionChanged(this, currentScrumTeam);
        }

        void EditScrumTeamButton_Click(object sender, RoutedEventArgs e)
        {
            ScrumTeamItem currentTeam = ScrumTeamSelector.CurrentScrumTeam;
            NewOrEditScrumTeamDialog dialog = new NewOrEditScrumTeamDialog(currentTeam);
            dialog.ShowDialog();
            if (dialog.Confirmed)
            {
                currentTeam.BeginSaveImmediate();
                currentTeam.SaveImmediate();
            }
            else
            {
                currentTeam.UndoChanges();
            }
        }

        void CreateScrumTeamButton_Click(object sender, RoutedEventArgs e)
        {
            ScrumTeamItem scrumTeam = new ScrumTeamItem();
            scrumTeam.Title = Globals.DefaultScrumTeamName;
            scrumTeam.ParentPillarItem = ScrumTeamSelector.CurrentPillar;
            NewOrEditScrumTeamDialog dialog = new NewOrEditScrumTeamDialog(scrumTeam);
            dialog.ShowDialog();

            if (dialog.Confirmed)
            {
                ScrumTeamItem newItem = ScheduleStore.Instance.CreateStoreItem<ScrumTeamItem>(ItemTypeID.ScrumTeam);
                newItem.Title = scrumTeam.Title;
                newItem.ParentPillarItem = scrumTeam.ParentPillarItem;
                newItem.ScrumMasterItem = scrumTeam.ScrumMasterItem;
                newItem.SaveNewItem();
                ScrumTeamSelector.SetSelectedScrumTeam(newItem);
            }
        }

        void DeleteScrumTeamButton_Click(object sender, RoutedEventArgs e)
        {
            ScrumTeamItem scrumTeam = ScrumTeamSelector.CurrentScrumTeam;
            if (scrumTeam != null)
            {
                scrumTeam.RequestDeleteItem();
            }
        }

        void Handle_NewItemCommittedToStore(StoreItem item)
        {
            if (item.StoreItemType == ItemTypeID.BacklogItem)
            {
                BacklogItem backlogItem = (BacklogItem)item;
                BacklogGrid.SelectedItem = backlogItem;
                BacklogGrid.ScrollIntoView(backlogItem);
            }
        }

        void MoveWorkItemsButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MoveWorkItems();
        }

        void ScrumTeamBacklogCreateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ScrumTeamItem currentScrumTeam = ScrumTeamSelector.CurrentScrumTeam;
            if (currentScrumTeam != null)
            {
                PillarItem pillarItem = currentScrumTeam.ParentPillarItem;
                TrainItem trainItem = TrainManager.Instance.CurrentTrain;
                BacklogItem.RequestNewBacklogItem(pillarItem, trainItem, currentScrumTeam);
            }
        }

        void CapacityBacklogDeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BacklogItem item = BacklogGrid.CurrentItem as BacklogItem;
            if (item == null)
            {
                item = BacklogGrid.SelectedItem as BacklogItem;
            }

            if (item != null)
            {
                item.DeleteItem();
            }
        }

        void WorkItemsGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
        }

        WorkItem DeletedWorkItemInProgress = null;
        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has clicked the command to delete the currently selected work item.
        /// </summary>
        //------------------------------------------------------------------------------------
        void DeleteWorkItemCommand()
        {
            WorkItem item = WorkItemsGrid.CurrentItem as WorkItem;
            if (item != null)
            {
                if (item.RequestDeleteItem())
                {
                    DeletedWorkItemInProgress = item;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Regular timer job that runs on the UI thread, which allows us to make updates to
        /// our UI collection views as the user adds and removes items.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Handle_UpdateUI()
        {
            // Update Ribbon
            Globals.MainWindow.ScrumTeamMoveWorkItemsButton.IsEnabled = WorkItemsGrid.SelectedItems.Count > 0 ? true : false;
            Globals.MainWindow.NewCapacityWorkItemButton.IsEnabled = BacklogGrid.SelectedItems.Count == 1 ? true : false;
            Globals.MainWindow.DeleteCapacityWorkItemButton.IsEnabled = WorkItemsGrid.SelectedItems.Count == 1 && DeletedWorkItemInProgress == null ? true : false;
            Globals.MainWindow.ScrumTeamBacklogCreateButton.IsEnabled = ScrumTeamSelector.CurrentScrumTeam != null ? true : false;
            Globals.MainWindow.ScrumTeamBacklogDeleteButton.IsEnabled = BacklogGrid.SelectedItems.Count == 1 ? true : false;
       }

        string LastAssignedTo { get; set; }
        string LastWorkItemSubtype { get; set; }

        void CreateWorkItemCommand()
        {
            if (CurrentBacklogItem != null)
            {
                if (LastAssignedTo == null)
                {
                    foreach (GroupMemberItem member in ScrumTeamSelector.CurrentScrumTeam.Members)
                    {
                        if (Utils.StringsMatch(member.Discipline, "Dev"))
                        {
                            LastAssignedTo = member.Alias;
                            break;
                        }
                    }
                }

                if (LastWorkItemSubtype == null)
                {
                    LastWorkItemSubtype = SubtypeValues.ProductCoding;
                }

                WorkItem workItem = WorkItem.CreateWorkItem(CurrentBacklogItem, "New WorkItem", LastWorkItemSubtype, LastAssignedTo);
                workItem.SaveNewItem();
            }

        }

        void SetPlannerSortOrder()
        {
            PlannerView = (CollectionView)CollectionViewSource.GetDefaultView(BacklogGrid.ItemsSource);

            if (PlannerView != null && PlannerView.CanSort == true)
            {
                PlannerView.SortDescriptions.Clear();
                PlannerView.SortDescriptions.Add(new SortDescription("BusinessRank", ListSortDirection.Ascending));
            }
        }

        void BacklogGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentBacklogItem = BacklogGrid.SelectedItem as BacklogItem;
            if (CurrentBacklogItem != null)
            {
                WorkItemsGrid.ItemsSource = CurrentBacklogItem.WorkItems;
                WorkItemsGridHeader.Text = "Work Items for: " + CurrentBacklogItem.ID.ToString() + ":" + CurrentBacklogItem.Title;
            }
        }

        private void BacklogGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IInputElement element = e.MouseDevice.DirectlyOver;
            System.Windows.Controls.Primitives.RepeatButton scrollButton = element as System.Windows.Controls.Primitives.RepeatButton;
            if (scrollButton == null)
            {
                BacklogItem backlogItem = BacklogGrid.SelectedItem as BacklogItem;
                if (backlogItem != null)
                {
                    backlogItem.ShowBacklogItemEditor();
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The work item grid context menu was used to request editing of the selected work
        /// item.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void EditWorkItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WorkItem workItem = GetWorkItemFromSender(sender);
            if (workItem != null)
            {
                workItem.ShowWorkItemEditor();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The work item grid context menu was used to request deletion of the selected work
        /// item.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void DeleteWorkItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WorkItem workItem = GetWorkItemFromSender(sender);
            if (workItem != null)
            {
                workItem.RequestDeleteItem();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Allows the user to move the selected work items to a different parent backlog
        /// item.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void MoveWorkItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MoveWorkItems();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Allows the user to move the selected work items to a different parent backlog
        /// item.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void MoveWorkItems()
        {
            if (WorkItemsGrid.SelectedItems.Count > 0)
            {
                AsyncObservableCollection<WorkItem> workItems = new AsyncObservableCollection<WorkItem>();
                foreach (var item in WorkItemsGrid.SelectedItems)
                {
                    workItems.Add(item as WorkItem);
                }

                PillarItem pillarItem = Globals.MainWindow.ScrumTeamPillarFilterCombo.SelectedItem as PillarItem;
                ScrumTeamItem featureTeamItem = Globals.MainWindow.SelectScrumTeamCombo.SelectedItem as ScrumTeamItem;
                MoveWorkItemsDialog dialog = new MoveWorkItemsDialog(workItems, pillarItem, null, featureTeamItem);
                dialog.ShowDialog();
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

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Attempts to extract a work item reference from the given sender object.
        /// </summary>
        //------------------------------------------------------------------------------------
        private BacklogItem GetBacklogItemFromSender(object sender)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                return menuItem.DataContext as BacklogItem;
            }

            return null;
        }

        private void BacklogItemEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BacklogItem backlogItem = GetBacklogItemFromSender(sender);
            if (backlogItem != null)
            {
                backlogItem.ShowBacklogItemEditor();
            }
        }

        private void BacklogItemClose_ContextClick(object sender, System.Windows.RoutedEventArgs e)
        {
            BacklogItem backlogItem = GetBacklogItemFromSender(sender);
            if (backlogItem != null)
            {
                backlogItem.RequestClose();
            }
        }

        private void BacklogItemDelete_ContextClick(object sender, System.Windows.RoutedEventArgs e)
        {
            BacklogItem backlogItem = GetBacklogItemFromSender(sender);
            if (backlogItem != null)
            {
                backlogItem.DeleteItem();
            }
        }

        private void NewWorkItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WorkItem workItem = GetWorkItemFromSender(sender);
            if (workItem != null)
            {
                BacklogItem parentBacklogItem = workItem.ParentBacklogItem;
                if (parentBacklogItem != null)
                {
                    WorkItem newWorkItem = WorkItem.CreateWorkItem(parentBacklogItem);
                    newWorkItem.SaveNewItem();
                }
            }
        }

    }
}
