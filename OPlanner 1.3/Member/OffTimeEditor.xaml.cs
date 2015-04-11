using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for OffTimeEditor.xaml
    /// </summary>
    public partial class OffTimeEditor : Window
    {
        private GroupMemberItem ParentItem;
        private Dictionary<string, int> m_workDaysInUse;

        const int ControlMarginSize = 2;
        const int StartEndDateGridRowHeight = 28;

        private DateTime EarliestAllowedDate { get; set; }

        public OffTimeEditor(GroupMemberItem groupMemberItem)
        {
            InitializeComponent();

            Globals.ApplicationManager.SetStartupLocation(this);

            this.Title = "Set Off Time for: " + groupMemberItem.DisplayName;
            MemberImage.Source = groupMemberItem.UserPicture;
            MemberNameBox.Text = groupMemberItem.DisplayName;
            CancelButton.Click += CancelButton_Click;
            OKButton.Click += OKButton_Click;

            EarliestAllowedDate = new DateTime(2013, 4, 1);

            ParentItem = groupMemberItem;
            m_workDaysInUse = new Dictionary<string, int>();

            if (groupMemberItem.OffTimeItems.Count == 0)
            {
                AddOffTimeRow(null);
            }
            else
            {
                foreach (OffTimeItem offTimeItem in groupMemberItem.OffTimeItems)
                {
                    AddOffTimeRow(offTimeItem);
                }
            }

        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private List<int> DetectOverlappingRows()
        {
            List<int> rows = new List<int>();
            m_workDaysInUse.Clear();

            foreach (RowDefinition row in StartEndDateGrid.RowDefinitions)
            {
                OffTimeControls offTimeControls = row.Tag as OffTimeControls;
                if (offTimeControls != null)
                {
                    int thisRow = Grid.GetRow(offTimeControls.StartDatePicker);
                    bool thisRowOverlaps = false;
                    DateTime startDate = offTimeControls.StartDatePicker.SelectedDate.Value;
                    DateTime endDate = offTimeControls.EndDatePicker.SelectedDate.Value;
                    DateTime date = startDate;
                    while (date <= endDate)
                    {
                        if (Utils.IsWorkDay(date))
                        {
                            if (!thisRowOverlaps && m_workDaysInUse.ContainsKey(date.ToShortDateString()))
                            {
                                thisRowOverlaps = true;
                                rows.Add(thisRow);
                                int overlappingWith = m_workDaysInUse[date.ToShortDateString()];
                                if (!rows.Contains(overlappingWith))
                                {
                                    rows.Add(overlappingWith);
                                }
                            }

                            if (!m_workDaysInUse.ContainsKey(date.ToShortDateString()))
                            {
                                m_workDaysInUse.Add(date.ToShortDateString(), thisRow);
                            }
                        }

                        date = date.AddDays(1);
                    }
                }
            }

            return rows;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has clicked the Ok button.
        /// </summary>
        //------------------------------------------------------------------------------------
        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            CommitChanges();
            Close();
        }

        void CommitChanges()
        {
            int existingCount = ParentItem.OffTimeItems.Count;
            int newIndex = 0;

            // For each OffTime row currently in the window, save the definition as an OffTimeItem
            // to the backing store:
            //
            // If there are now more OffTime definitions than there were when the dialog opened, 
            // save the new definitions back to the original OffItemItems in the backing store 
            // until they run out, and then create new items to save the overflow.
            //
            // If there are now fewer OffTime definitions, save all the new definitions back into
            // the existing items until the new definitions run out, and then delete the remaining
            // existing definitions.
            foreach (RowDefinition row in StartEndDateGrid.RowDefinitions)
            {
                OffTimeControls offTimeControls = row.Tag as OffTimeControls;
                if (offTimeControls != null)
                {
                    OffTimeItem offTimeItem;

                    // Re-use an existing definition
                    if (newIndex < existingCount)
                    {
                        offTimeItem = ParentItem.OffTimeItems.GetItem(newIndex);
                    }

                    // No more existing definitions left, so create a new item in the backing store.
                    else
                    {
                        offTimeItem = ScheduleStore.Instance.CreateStoreItem<OffTimeItem>(ItemTypeID.OffTime);
                        offTimeItem.ParentItemKey = ParentItem.StoreKey;
                        offTimeItem.Title = ParentItem.DisplayName + ": OffTime";
                    }

                    offTimeItem.StartDate = offTimeControls.StartDatePicker.SelectedDate.Value;
                    offTimeItem.EndDate = offTimeControls.EndDatePicker.SelectedDate.Value;

                    if (offTimeItem.IsNew)
                    {
                        offTimeItem.SaveNewItem();
                    }

                    newIndex++;
                }
            }

            // If there are now existing definitions that are no longer needed, delete them (copy them
            // into a delete list first, since the original collection could change as we delete items.
            List<StoreItem> itemsToDelete = new List<StoreItem>();
            if (newIndex < existingCount)
            {
                for (int indexToDelete = newIndex; indexToDelete < existingCount; indexToDelete++)
                {
                    OffTimeItem offTimeItem = ParentItem.OffTimeItems.GetItem(indexToDelete);
                    itemsToDelete.Add(offTimeItem);
                }
            }

            foreach (StoreItem item in itemsToDelete)
            {
                item.DeleteItem();
            }
        }

        private void AdditionalDatesLinkClick(object sender, MouseButtonEventArgs e)
        {
            AddOffTimeRow(null);
        }

        private DateTime GetNextUnusedWorkingDay()
        {
            m_workDaysInUse.Clear();

            foreach (RowDefinition row in StartEndDateGrid.RowDefinitions)
            {
                OffTimeControls offTimeControls = row.Tag as OffTimeControls;
                if (offTimeControls != null)
                {
                    int thisRow = Grid.GetRow(offTimeControls.StartDatePicker);
                    DateTime startDate = offTimeControls.StartDatePicker.SelectedDate.Value;
                    DateTime endDate = offTimeControls.EndDatePicker.SelectedDate.Value;
                    DateTime date = startDate;
                    while (date <= endDate)
                    {
                        if (Utils.IsWorkDay(date))
                        {
                            if (!m_workDaysInUse.ContainsKey(date.ToShortDateString()))
                            {
                                m_workDaysInUse.Add(date.ToShortDateString(), thisRow);
                            }
                        }

                        date = date.AddDays(1);
                    }
                }
            }

            DateTime nextDate = DateTime.Today;
            for (; ; )
            {
                if (Utils.IsWorkDay(nextDate) && !m_workDaysInUse.ContainsKey(nextDate.ToShortDateString()))
                {
                    return nextDate;
                }

                nextDate = nextDate.AddDays(1);
            }
        }

        private void AddOffTimeRow(OffTimeItem offTimeItem)
        {
            RowDefinition row = new RowDefinition();
            GridLength gridRowHeight = StartEndDateGridRow.Height;
            StartEndDateGridRow.Height = new GridLength(gridRowHeight.Value + StartEndDateGridRowHeight);
            row.Height = new GridLength(StartEndDateGridRowHeight);
            StartEndDateGrid.RowDefinitions.Add(row);

            int lastRow = StartEndDateGrid.RowDefinitions.Count - 1;

            // Add Delete Button
            Button button = new Button();
            button.Margin = new Thickness(ControlMarginSize);
            button.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            button.Style = (Style)Application.Current.FindResource("DeleteButtonStyle");
            button.Click += DeleteRowButton_Click;
            Grid.SetRow(button, lastRow);
            Grid.SetColumn(button, 0);
            StartEndDateGrid.Children.Add(button);

            //int gridRow = Grid.GetRow(button);
            //StartEndDateGrid.RowDefinitions.RemoveAt(gridRow);
            //RowDefinition thisRow = StartEndDateGrid.RowDefinitions[gridRow];

            // Add the StartDate DatePicker
            DatePicker startDatePicker = new DatePicker();
            startDatePicker.Height = 24;
            startDatePicker.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            startDatePicker.SelectedDate = offTimeItem == null ? GetNextUnusedWorkingDay() : offTimeItem.StartDate;
            startDatePicker.SelectedDateChanged += StartDatePicker_SelectedDateChanged;
            startDatePicker.Margin = new Thickness(ControlMarginSize);
            Grid.SetRow(startDatePicker, lastRow);
            Grid.SetColumn(startDatePicker, 1);
            StartEndDateGrid.Children.Add(startDatePicker);

            // Add the EndDate DatePicker
            DatePicker endDatePicker = new DatePicker();
            endDatePicker.Height = 24;
            endDatePicker.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            endDatePicker.SelectedDate = offTimeItem == null ? startDatePicker.SelectedDate : offTimeItem.EndDate;
            endDatePicker.SelectedDateChanged += EndDatePicker_SelectedDateChanged;
            endDatePicker.Margin = new Thickness(ControlMarginSize);

            Grid.SetRow(endDatePicker, lastRow);
            Grid.SetColumn(endDatePicker, 2);
            StartEndDateGrid.Children.Add(endDatePicker);

            // Add the NetDays text control
            int netWorkDays = Utils.GetNetWorkingDays(startDatePicker.SelectedDate.Value, endDatePicker.SelectedDate.Value);
            TextBlock netDaysTextBlock = new TextBlock();
            netDaysTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            netDaysTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            netDaysTextBlock.Text = netWorkDays.ToString();
            Grid.SetRow(netDaysTextBlock, lastRow);
            Grid.SetColumn(netDaysTextBlock, 3);
            StartEndDateGrid.Children.Add(netDaysTextBlock);

            // For convenience, let the controls on each row know about the others
            OffTimeControls offTimeControls = new OffTimeControls();
            offTimeControls.DeleteButton = button;
            offTimeControls.StartDatePicker = startDatePicker;
            offTimeControls.EndDatePicker = endDatePicker;
            offTimeControls.NetDaysTextBlock = netDaysTextBlock;
            row.Tag = offTimeControls;

            endDatePicker.Tag = offTimeControls;
            startDatePicker.Tag = offTimeControls;
            button.Tag = offTimeControls;

            TotalNetDaysOffBlock.Text = GetTotalNetDaysOff().ToString();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has clicked the button to delete a row from the grid.
        /// </summary>
        //------------------------------------------------------------------------------------
        void DeleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;
            if (deleteButton != null)
            {
                int gridRow = Grid.GetRow(deleteButton);
                DeleteGridRow(StartEndDateGrid, gridRow);

                GridLength gridRowHeight = StartEndDateGridRow.Height;
                StartEndDateGridRow.Height = new GridLength(gridRowHeight.Value - StartEndDateGridRowHeight);

                TotalNetDaysOffBlock.Text = GetTotalNetDaysOff().ToString();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the given row from the given Grid (including deleting or updating all
        /// the child controls of the DataGrid accordingly).
        /// </summary>
        //------------------------------------------------------------------------------------
        void DeleteGridRow(Grid grid, int rowIndex)
        {
            List<UIElement> elementsToRemove = new List<UIElement>();
            List<UIElement> elementsToUpdate = new List<UIElement>();
            foreach (UIElement element in grid.Children)
            {
                if (Grid.GetRow(element) == rowIndex)
                {
                    elementsToRemove.Add(element);
                }
                else if (Grid.GetRow(element) > rowIndex)
                {
                    elementsToUpdate.Add(element);
                }
            }

            foreach (UIElement element in elementsToRemove)
            {
                grid.Children.Remove(element);
            }

            grid.RowDefinitions.RemoveAt(rowIndex);

            foreach (UIElement element in elementsToUpdate)
            {
                int oldRow = Grid.GetRow(element);
                Grid.SetRow(element, oldRow - 1);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a count of the total number of off days the user has designated across
        /// all OffTime rows.
        /// </summary>
        //------------------------------------------------------------------------------------
        int GetTotalNetDaysOff()
        {
            int totalNetDaysOff = 0;
            foreach (RowDefinition row in StartEndDateGrid.RowDefinitions)
            {
                OffTimeControls offTimeControls = row.Tag as OffTimeControls;
                if (offTimeControls != null)
                {
                    totalNetDaysOff += Utils.GetNetWorkingDays(offTimeControls.StartDatePicker.SelectedDate.Value, offTimeControls.EndDatePicker.SelectedDate.Value);
                }
            }

            return totalNetDaysOff;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has changed the StartDate for one of the OffTime rows.
        /// </summary>
        //------------------------------------------------------------------------------------
        void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker startDatePicker = sender as DatePicker;
            if (startDatePicker != null)
            {
                if (startDatePicker.SelectedDate < EarliestAllowedDate)
                {
                    startDatePicker.SelectedDate = Utils.GetNextWorkingDay(EarliestAllowedDate);
                }

                OffTimeControls offTimeControls = startDatePicker.Tag as OffTimeControls;
                if (offTimeControls != null)
                {
                    DatePicker endDatePicker = offTimeControls.EndDatePicker;
                    if (endDatePicker.SelectedDate < startDatePicker.SelectedDate)
                    {
                        endDatePicker.SelectedDate = startDatePicker.SelectedDate;
                    }

                    DetectAndCorrectOverlappingDateRanges();

                    offTimeControls.NetDaysTextBlock.Text = Utils.GetNetWorkingDays(startDatePicker.SelectedDate.Value, endDatePicker.SelectedDate.Value).ToString();
                    TotalNetDaysOffBlock.Text = GetTotalNetDaysOff().ToString();
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has changed the EndDate for one of the OffTime rows.
        /// </summary>
        //------------------------------------------------------------------------------------
        void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker endDatePicker = sender as DatePicker;
            if (endDatePicker != null)
            {
                if (endDatePicker.SelectedDate < EarliestAllowedDate)
                {
                    endDatePicker.SelectedDate = Utils.GetNextWorkingDay(EarliestAllowedDate);
                }

                OffTimeControls offTimeControls = endDatePicker.Tag as OffTimeControls;
                if (offTimeControls != null)
                {
                    DatePicker startDatePicker = offTimeControls.StartDatePicker;
                    if (endDatePicker.SelectedDate < startDatePicker.SelectedDate)
                    {
                        endDatePicker.SelectedDate = startDatePicker.SelectedDate;
                    }

                    DetectAndCorrectOverlappingDateRanges();

                    offTimeControls.NetDaysTextBlock.Text = Utils.GetNetWorkingDays(startDatePicker.SelectedDate.Value, endDatePicker.SelectedDate.Value).ToString();
                    TotalNetDaysOffBlock.Text = GetTotalNetDaysOff().ToString();
                }
            }
        }

        private bool InOverlapCorrectionMode = false;
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Detects whether any of the current off-time date ranges overlap, and if so, puts
        /// the dialog in "correction" mode (directing the user to correct the problem). If
        /// the dialog was already in correction mode, and the user has fixed the problem,
        /// normal mode is restored.
        /// </summary>
        //------------------------------------------------------------------------------------
        void DetectAndCorrectOverlappingDateRanges()
        {
            List<int> overlappingRows = DetectOverlappingRows();
            if (overlappingRows.Count > 0)
            {
                InOverlapCorrectionMode = true;
                NewDatesButton.IsEnabled = false;
                NewDatesLink.IsEnabled = false;
                OKButton.IsEnabled = false;
                ErrorBlock.Text = "One or more of the date ranges that you specified overlap with each other - please adjust the dates so that they don't overlap.";
                ErrorBlock.Visibility = System.Windows.Visibility.Visible;

                foreach (UIElement element in StartEndDateGrid.Children)
                {
                    element.IsEnabled = false;
                    int elementRow = Grid.GetRow(element);
                    foreach (int row in overlappingRows)
                    {
                        if (row == elementRow)
                        {
                            element.IsEnabled = true;
                            break;
                        }
                    }
                }
            }
            else if (InOverlapCorrectionMode)
            {
                InOverlapCorrectionMode = false;
                ErrorBlock.Visibility = System.Windows.Visibility.Collapsed;
                ErrorBlock.Text = "";

                NewDatesButton.IsEnabled = true;
                NewDatesLink.IsEnabled = true;
                OKButton.IsEnabled = true;

                foreach (UIElement element in StartEndDateGrid.Children)
                {
                    element.IsEnabled = true;
                }
            }
        }

        class OffTimeControls
        {
            public Button DeleteButton { get; set; }
            public DatePicker StartDatePicker { get; set; }
            public DatePicker EndDatePicker { get; set; }
            public TextBlock NetDaysTextBlock { get; set; }
        }
    }
}
