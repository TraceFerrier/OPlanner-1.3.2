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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlannerNameSpace.UserControls
{
    /// <summary>
    /// Interaction logic for PlannerStatusBar.xaml
    /// </summary>
    public partial class PlannerStatusBar : UserControl
    {
        Brush OriginalStatusTextBackground;
        bool FirstLoadComplete = false;
        TrainItem CurrentTrain { get; set; }
        DateTime Today { get; set; }
        TextBlock TodayTextBlock { get; set; }

        public PlannerStatusBar()
        {
            InitializeComponent();

            OriginalStatusTextBackground = StatusMessageGrid.Background;
            Globals.EventManager.UpdateUI += HandleUpdateUI;
            Globals.EventManager.TabLoadStarting += Instance_TabLoadStarting;
        }

        void BacklogItemsNotOnTrack_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateNotificationsPanel();
        }

        void BacklogItemsOnTrack_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateNotificationsPanel();
        }

        void Instance_TabLoadStarting()
        {
            Utils.ShowWaitCursor();
            ShowStatusMessage("Loading View...", Brushes.LightYellow);
        }

        void ShowProductName()
        {
            ProductGroupItem CurrentProduct = ProductGroupManager.Instance.CurrentProductGroup;
            if (CurrentProduct != null)
            {
                string title = CurrentProduct.Title;
                if (Globals.UserPreferences.ShouldUseCloneHostStore)
                {
                    title += " (Clone)";
                }

                ProductNameBox.Text = title;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// UI timer handler, to regularly update the status bar.
        /// </summary>
        //------------------------------------------------------------------------------------
        void HandleUpdateUI()
        {
            if (!FirstLoadComplete)
            {
                ShowProductName();

                //TrainArea.Visibility = System.Windows.Visibility.Visible;
                StatusBarExpander.IsExpanded = Globals.UserPreferences.GetProductPreference<bool>(ProductPreferences.LastSelectedStatusBarPositionValue);
                FirstLoadComplete = true;

                Globals.CommitmentStatusManager.BacklogItemsOnTrack.CollectionChanged += BacklogItemsOnTrack_CollectionChanged;
                Globals.CommitmentStatusManager.BacklogItemsNotOnTrack.CollectionChanged += BacklogItemsNotOnTrack_CollectionChanged;
            }

            UpdateStatusBar();

            Utils.ShowDefaultCursor();
        }

        public void UpdateStatusBar2()
        {
            BackgroundTask updateTask = new BackgroundTask(false);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Redraws the status bar to reflect the most current app status.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void UpdateStatusBar()
        {
            bool showRefreshStatus = false;

            // If the status bar hasn't been displayed yet, or we've advanced to a new day since
            // the app was booted, render the status bar.
            if (CurrentTrain != TrainManager.Instance.CurrentTrain || Today != DateTime.Today)
            {
                CurrentTrain = TrainManager.Instance.CurrentTrain;
                Today = DateTime.Today;

                if (CurrentTrain != null)
                {
                    TrainNameBox.Text = CurrentTrain.Title;

                    int totalDays = Utils.GetTotalDays(CurrentTrain.StartDate, CurrentTrain.EndDate);
                    DateTime trainDay = CurrentTrain.StartDate;
                    double timelineWidth = TimeLineCanvas.ActualWidth;

                    double dayMargins = 1;
                    double dayTopMargin = 15;
                    double dayWidth = timelineWidth / totalDays;
                    double dayHeight = TimeLineCanvas.ActualHeight - dayMargins * 2 - dayTopMargin;

                    double leftSide = dayMargins;
                    for (int x = 0; x < totalDays; x++)
                    {
                        Rectangle dayRect = new Rectangle();
                        dayRect.Width = dayWidth - dayMargins * 2;
                        dayRect.Height = dayHeight;

                        if (trainDay == DateTime.Today)
                        {
                            // If we've advanced forward a day, remove the display of the previous day's date
                            if (TodayTextBlock != null)
                            {
                                TimeLineCanvas.Children.Remove(TodayTextBlock);
                            }

                            TodayTextBlock = new TextBlock();
                            TodayTextBlock.Text = trainDay.ToShortDateString();
                            TodayTextBlock.Foreground = Brushes.White;
                            Canvas.SetLeft(TodayTextBlock, leftSide);
                            Canvas.SetTop(TodayTextBlock, dayMargins);
                            //TimeLineCanvas.Children.Add(TodayTextBlock);
                        }

                        StringBuilder toolTipText = new StringBuilder();
                        toolTipText.AppendLine(trainDay.ToLongDateString());

                        if (!Utils.IsWorkDay(trainDay))
                        {
                            dayRect.Fill = Brushes.LightGray;

                            if (Utils.IsHoliday(trainDay))
                            {
                                toolTipText.AppendLine("Company Holiday");
                            }
                            else
                            {
                                toolTipText.AppendLine("Weekend Day");
                            }
                        }
                        else if (trainDay < DateTime.Today)
                        {
                            dayRect.Fill = Brushes.LightPink;
                            toolTipText.AppendLine("Past Train WorkDay");
                        }
                        else if (trainDay == DateTime.Today)
                        {
                            dayRect.Fill = Brushes.Green;
                            toolTipText.AppendLine("Today");
                        }
                        else
                        {
                            dayRect.Fill = Brushes.LightGreen;
                            toolTipText.AppendLine("Upcoming Train WorkDay");
                        }

                        Canvas.SetLeft(dayRect, leftSide);
                        Canvas.SetTop(dayRect, dayMargins + dayTopMargin);

                        TextBlock dayName = new TextBlock();
                        dayName.Text = trainDay.DayOfWeek.ToString().Substring(0, 2);
                        dayName.Foreground = Brushes.Black;
                        dayName.FontSize = 9;
                        Canvas.SetLeft(dayName, leftSide + 5);
                        Canvas.SetTop(dayName, dayMargins + dayTopMargin);

                        ToolTip toolTip = HelpManager.Instance.GetToolTip(toolTipText.ToString(), "TimelineStandardToolTipStyle");
                        dayRect.ToolTip = toolTip;
                        dayName.ToolTip = toolTip;

                        TimeLineCanvas.Children.Add(dayRect);
                        TimeLineCanvas.Children.Add(dayName);

                        leftSide += dayWidth;
                        trainDay = trainDay.AddDays(1);
                    }

                    TrainStartBox.Text = CurrentTrain.StartDate.ToShortDateString();
                    TrainEndBox.Text = CurrentTrain.EndDate.ToShortDateString();
                }
            }

            TodayTimeBox.Text = DateTime.Now.ToLongTimeString();

            string currentCount = Globals.ItemManager.DeferredItemCurrent.ToString();
            string totalCount = Globals.ItemManager.DeferredItemCount.ToString();

            if (StoreQueryManager.Instance.IsRefreshInProgress)
            {
                ShowStatusMessage("Refreshing items: " + currentCount + " of " + totalCount, Brushes.Yellow);
            }
            else if (!Globals.ItemManager.IsDiscoveryComplete)
            {
                ShowStatusMessage("Discovering items: " + currentCount + " of " + totalCount, Brushes.LightYellow);
            }
            else if (Globals.ApplicationManager.GetCurrentWarning() != null)
            {
                ShowWarningMessage(Globals.ApplicationManager.GetCurrentWarning());
            }
            else
            {
                StatusMessageGrid.Background = OriginalStatusTextBackground;
                StatusMessageBox.Text = "";
            }

            if (StoreQueryManager.Instance.IsRefreshInProgress && showRefreshStatus)
            {
                ChangeStatusGrid.Background = Brushes.Yellow;
                ChangeStatusBox.Text = "Refresh in progress...";
                ChangeStatusBox.Foreground = Brushes.Black;
            }
            else
            {
                int pendingChanges = Globals.ItemManager.ChangeCount;
                if (pendingChanges > 0)
                {
                    ChangeStatusGrid.Background = Brushes.Red;
                    ChangeStatusBox.Foreground = Brushes.White;
                    ChangeStatusBox.Text = "Changes to save: " + pendingChanges.ToString();
                }
                else
                {
                    ChangeStatusGrid.Background = Brushes.Green;
                    ChangeStatusBox.Foreground = Brushes.White;
                    ChangeStatusBox.Text = "Changes to save: " + pendingChanges.ToString();
                }
            }

            UpdateNotificationsPanel();
        }

        void ShowStatusMessage(string message, Brush messageColor)
        {
            StatusMessageBox.Visibility = System.Windows.Visibility.Visible;
            WarningImage.Visibility = System.Windows.Visibility.Collapsed;
            StatusMessageGrid.Background = messageColor;
            StatusMessageBox.Text = message;
        }

        void ClearStatusMessage()
        {
            StatusMessageGrid.Background = OriginalStatusTextBackground;
            StatusMessageBox.Text = "";
        }

        void ShowWarningMessage(string warning)
        {
            StatusMessageGrid.Background = OriginalStatusTextBackground;
            StatusMessageBox.Visibility = System.Windows.Visibility.Collapsed;
            WarningImage.Visibility = System.Windows.Visibility.Visible;
            WarningImage.ToolTip = warning;
        }

        void UpdateNotificationsPanel()
        {
            int onTrackBacklogItems = Globals.CommitmentStatusManager.BacklogItemsOnTrack.Count;
            int notOnTrackBacklogItems = Globals.CommitmentStatusManager.BacklogItemsNotOnTrack.Count;

            BacklogItemsOnTrackBox.Text = onTrackBacklogItems.ToString();
            BacklogItemsNotOnTrackBox.Text = notOnTrackBacklogItems.ToString();
            DevelopmentBackground.Color = Utils.GetStatusColor(onTrackBacklogItems, notOnTrackBacklogItems);

            int onTrackSpecs = TrainManager.Instance.NextTrainSpecsOnTrackCount;
            int notOnTrackSpecs = TrainManager.Instance.NextTrainSpecsNotOnTrackCount;
            SpecsOnTrackBox.Text = onTrackSpecs.ToString();
            SpecsNotOnTrackBox.Text = notOnTrackSpecs.ToString();
            SpecStatusBackground.Color = Utils.GetStatusColor(onTrackSpecs, notOnTrackSpecs);
        }

        private void DevelopmentPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        void FeatureTeamDetails_Click(object sender, RoutedEventArgs e)
        {
        }

        private void StatusBarExpander_Expanded(object sender, RoutedEventArgs e)
        {
            Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedStatusBarPositionValue, true);
        }

        private void StatusBarExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedStatusBarPositionValue, false);
        }

        private void ChangePanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 2)
                {
                    Globals.ItemManager.ShowChangeListWindow();
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                ContextMenu menu = new ContextMenu();
                Utils.AddContextMenuItem(menu, "ChangeList Viewer...", "SaveAndUpdate.png", ShowChangeListWindow_Click);
                Utils.OpenContextMenu(menu);
            }
        }

        void ShowChangeListWindow_Click(object sender, RoutedEventArgs e)
        {
            Globals.ItemManager.ShowChangeListWindow();
        }

    }
}
