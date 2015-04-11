using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PlannerNameSpace.Views
{
    /// <summary>
    /// Interaction logic for BigBoardView.xaml
    /// </summary>
    public partial class BoardView : UserControl
    {
        Brush ItemsContainerPreviousBackground { get; set; }

        ScrumTeamSelector ScrumTeamSelector;
        ScrumTeamItem CurrentScrumTeam;

        public BoardView()
        {
            InitializeComponent();

        }

        public void InitializeData()
        {
            ScrumTeamSelector = new ScrumTeamSelector(Globals.MainWindow.SelectBoardScrumTeamCombo, ProductPreferences.LastSelectedBoardViewScrumTeam,
                Globals.MainWindow.BoardPillarFilterCombo, ProductPreferences.LastSelectedBoardViewPillar);

            SetScrumTeam(ScrumTeamSelector.CurrentScrumTeam);
            ScrumTeamSelector.ScrumTeamItemSelectionChanged += ScrumTeamSelector_ScrumTeamItemSelectionChanged;
        }

        void ScrumTeamSelector_ScrumTeamItemSelectionChanged(object sender, ScrumTeamChangedEventArgs e)
        {
            SetScrumTeam(e.CurrentItem);
        }

        void SetScrumTeam(ScrumTeamItem scrumTeam)
        {
            CurrentScrumTeam = scrumTeam;
            if (scrumTeam != null)
            {
                BoardItemsControl.ItemsSource = CurrentScrumTeam.Members;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called when the user has clicked a WorkItem card.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void WorkItemCard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Border workItemCard = (Border)sender;
                if (workItemCard != null)
                {
                    WorkItem workItem = workItemCard.DataContext as WorkItem;
                    if (workItem != null)
                    {
                        workItem.ShowWorkItemEditor();
                    }
                }
            }
        }

        private void WorkItemCard_MouseMove(object sender, MouseEventArgs e)
        {
            Border workItemCard = (Border)sender;
            if (workItemCard != null && e.LeftButton == MouseButtonState.Pressed)
            {
                WorkItem workItem = workItemCard.DataContext as WorkItem;
                if (workItem != null)
                {
                    DragDrop.DoDragDrop(workItemCard, workItem, DragDropEffects.Move);
                }
            }
        }

        private void ToDoItemsContainer_DragEnter(object sender, DragEventArgs e)
        {
            HandleDragEnter(sender, e, WorkItemStates.NotStarted);
        }

        private void InProgressItemsContainer_DragEnter(object sender, DragEventArgs e)
        {
            HandleDragEnter(sender, e, WorkItemStates.InProgress);
        }

        private void CompletedItemsContainer_DragEnter(object sender, DragEventArgs e)
        {
            HandleDragEnter(sender, e, WorkItemStates.Completed);
        }

        private void ToDoItemsContainer_DragLeave(object sender, DragEventArgs e)
        {
            HandleDragLeave(sender, e);
        }

        private void InProgressItemsContainer_DragLeave(object sender, DragEventArgs e)
        {
            HandleDragLeave(sender, e);
        }

        private void CompletedItemsContainer_DragLeave(object sender, DragEventArgs e)
        {
            HandleDragLeave(sender, e);
        }

        private void ToDoItemsContainer_Drop(object sender, DragEventArgs e)
        {
            HandleDrop(sender, e, WorkItemStates.NotStarted);
        }

        private void InProgressItemsContainer_Drop(object sender, DragEventArgs e)
        {
            HandleDrop(sender, e, WorkItemStates.InProgress);
        }

        private void CompletedItemsContainer_Drop(object sender, DragEventArgs e)
        {
            HandleDrop(sender, e, WorkItemStates.Completed);
        }

        private void HandleDragEnter(object sender, DragEventArgs e, WorkItemStates statusToSet)
        {
            Border itemsContainer = sender as Border;
            if (itemsContainer != null)
            {
                GroupMemberItem teamMemberDraggedOnto = itemsContainer.DataContext as GroupMemberItem;

                if (e.Data.GetDataPresent(typeof(WorkItem)))
                {
                    WorkItem workItem = (WorkItem)e.Data.GetData(typeof(WorkItem));
                    if (workItem.ItemStatus != statusToSet || !Utils.StringsMatch(teamMemberDraggedOnto.Alias, workItem.AssignedTo))
                    {
                        ItemsContainerPreviousBackground = itemsContainer.Background;
                        itemsContainer.Background = Brushes.LightGoldenrodYellow;
                    }
                }
            }
        }

        private void HandleDragLeave(object sender, DragEventArgs e)
        {
            Border itemsContainer = sender as Border;
            if (itemsContainer != null)
            {
                itemsContainer.Background = ItemsContainerPreviousBackground;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has dragged a work item card, and dropped it in a new category.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void HandleDrop(object sender, DragEventArgs e, WorkItemStates statusToSet)
        {
            Border itemsContainer = sender as Border;
            if (itemsContainer != null)
            {
                itemsContainer.Background = ItemsContainerPreviousBackground;

                if (e.Data.GetDataPresent(typeof(WorkItem)))
                {
                    GroupMemberItem droppedOnMember = itemsContainer.DataContext as GroupMemberItem;
                    WorkItem workItem = (WorkItem)e.Data.GetData(typeof(WorkItem));
                    workItem.ProposeStatusChange(statusToSet, droppedOnMember.Alias);
                }
            }
        }

        private void EditBacklogItem_Click(object sender, RoutedEventArgs e)
        {
            BacklogItem backlogItem = GetBacklogItemFromSender(sender);
            if (backlogItem != null)
            {
                backlogItem.ShowBacklogItemEditor();
            }
        }

        private BacklogItem GetBacklogItemFromSender(object sender)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                CollectionViewGroup group = (CollectionViewGroup)element.DataContext;
                string backlogKey = group.Name as string;
                return Globals.ItemManager.GetItem<BacklogItem>(backlogKey);
            }

            return null;
        }

        private void BacklogItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BacklogItem backlogItem = GetBacklogItemFromSender(sender);
            if (backlogItem != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (e.ClickCount == 2)
                    {
                        backlogItem.ShowBacklogItemEditor();
                    }
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    Globals.ApplicationManager.ShowItemContextMenu(backlogItem, Globals.MainWindow);
                }
            }
        }

    }
}
