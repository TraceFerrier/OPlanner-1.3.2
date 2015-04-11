using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace PlannerNameSpace
{
    public class TrainCommitmentStatus
    {
        public TrainCommitmentStatusValue StatusValue { get; set; }
        public double TotalCount { get; set; }
        public double ThisValueCount { get; set; }
        public string PillarName { get; set; }
        public string TrainName { get; set; }
        public double Train_Commitment_Status
        {
            get
            {
                return ThisValueCount;
            }
        }

        private String myClass;

        public String Class
        {
            get { return myClass; }
            set
            {
                myClass = value;
            }
        }

    }

    /// <summary>
    /// Interaction logic for TrainStatusChart.xaml
    /// </summary>
    public partial class TrainStatusChart : Window
    {
        PillarItem CurrentPillar { get; set; }
        TrainItem CurrentTrain { get; set; }
        CollectionView ChartView;

        public TrainStatusChart()
        {
            InitializeComponent();
            Utils.FitWindowToScreen(this);

            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            PillarCombo.ItemsSource = PillarManager.PillarsWithAllNone;
            TrainCombo.ItemsSource = TrainManager.Instance.GetTrains(TrainTimeFrame.CurrentOrPast);

            CurrentTrain = TrainManager.Instance.CurrentTrain;

            CurrentPillar = Globals.UserPreferences.GetItemSelectionPreference<PillarItem>(ProductPreferences.LastSelectedTrainCommitmentPillar);
            if (CurrentPillar == null)
            {
                CurrentPillar = Globals.UserPreferences.GetItemSelectionPreference<PillarItem>(ProductPreferences.BacklogViewLastSelectedPillar);
                if (CurrentPillar == null)
                {
                    CurrentPillar = StoreItem.GetDummyItem<PillarItem>(DummyItemType.AllType);
                }
            }

            PillarCombo.SelectedItem = CurrentPillar;

            CurrentTrain = Globals.UserPreferences.GetItemSelectionPreference<TrainItem>(ProductPreferences.LastSelectedTrainCommitmentTrain);
            if (CurrentTrain == null)
            {
                CurrentTrain = Globals.UserPreferences.GetItemSelectionPreference<TrainItem>(ProductPreferences.BacklogViewLastSelectedTrain);
                if (CurrentTrain == null)
                {
                    CurrentTrain = TrainManager.Instance.CurrentTrain;
                }
            }

            TrainCombo.SelectedItem = CurrentTrain;

            PillarCombo.SelectionChanged += PillarCombo_SelectionChanged;
            TrainCombo.SelectionChanged += TrainCombo_SelectionChanged;
            ShowChart();

            Globals.ItemManager.StoreItemChanged += ItemManager_StoreItemChanged;
        }

        void ItemManager_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            StoreItemChange change = e.Change;
            if (change.Item.StoreItemType == ItemTypeID.BacklogItem)
            {
                if (change.ChangeType == ChangeType.Added || change.ChangeType == ChangeType.Removed ||
                    change.PublicPropName == Datastore.PropNameBacklogCommitmentSetting)
                {
                    ShowChart();
                }
            }
        }

        void ShowChart()
        {
            AsyncObservableCollection<BacklogItem> commitmentBacklogItems = new AsyncObservableCollection<BacklogItem>();
            Dictionary<TrainCommitmentStatusValue, TrainCommitmentStatus> statusDictionary = new Dictionary<TrainCommitmentStatusValue, TrainCommitmentStatus>();
            AsyncObservableCollection<TrainCommitmentStatus> commitmentStatusItems = new AsyncObservableCollection<TrainCommitmentStatus>();
            if (!CurrentPillar.IsDummyItem)
            {
                List<BacklogItem> pillarTrainBacklogItems = BacklogItem.GetBacklogItemsByPillarAndTrain(CurrentPillar, CurrentTrain);
                foreach (BacklogItem backlogItem in pillarTrainBacklogItems)
                {
                    if (backlogItem.IsCommittedToTrain(CurrentTrain))
                    {
                        commitmentBacklogItems.Add(backlogItem);
                    }
                }
            }
            else
            {
                foreach (BacklogItem backlogItem in BacklogItem.Items)
                {
                    if (backlogItem.IsCommittedToTrain(CurrentTrain))
                    {
                        commitmentBacklogItems.Add(backlogItem);
                    }
                }
            }

            int totalCount = commitmentBacklogItems.Count;
            foreach (BacklogItem backlogItem in commitmentBacklogItems)
            {
                if (!statusDictionary.ContainsKey(backlogItem.TrainCommitmentStatus))
                {
                    statusDictionary.Add(backlogItem.TrainCommitmentStatus, new TrainCommitmentStatus { StatusValue = backlogItem.TrainCommitmentStatus, ThisValueCount = 0, TotalCount = totalCount, Class = backlogItem.TrainCommitmentStatusText, PillarName = CurrentPillar.Title, TrainName = CurrentTrain.Title });
                    commitmentStatusItems.Add(statusDictionary[backlogItem.TrainCommitmentStatus]);
                }

                TrainCommitmentStatus statusItem = statusDictionary[backlogItem.TrainCommitmentStatus];
                statusItem.ThisValueCount++;
            }

            this.ChartDataContext.DataContext = commitmentStatusItems;
            UpdateUI();

            ChartView = (CollectionView)CollectionViewSource.GetDefaultView(commitmentStatusItems);
            ShowBacklogItems();
            ChartView.CurrentChanged += chartView_CurrentChanged;
        }

        void chartView_CurrentChanged(object sender, EventArgs e)
        {
            ShowBacklogItems();
        }

        void ShowBacklogItems()
        {
            TrainCommitmentStatus statusItem = ChartView.CurrentItem as TrainCommitmentStatus;
            if (statusItem != null)
            {
                AsyncObservableCollection<BacklogItem> commitmentBacklogItems = new AsyncObservableCollection<BacklogItem>();
                List<BacklogItem> pillarTrainBacklogItems = BacklogItem.GetBacklogItemsByPillarAndTrain(CurrentPillar, CurrentTrain);
                foreach (BacklogItem backlogItem in pillarTrainBacklogItems)
                {
                    if (backlogItem.IsCommittedToTrain(CurrentTrain) && backlogItem.TrainCommitmentStatus == statusItem.StatusValue)
                    {
                        commitmentBacklogItems.Add(backlogItem);
                    }
                }

                BacklogGrid.ItemsSource = commitmentBacklogItems;
            }
        }

        void UpdateUI()
        {
            if (CurrentPillar != null)
            {
                TrainReviewPillarBox.Text = CurrentPillar.Title;
            }

            if (CurrentTrain != null)
            {
                TrainReviewTrainBox.Text = CurrentTrain.Title;
            }

            Nullable<DateTime> approvalDate;
            string approver;
            if (Globals.CommitmentStatusManager.IsApprovedSnapshotAvailable(CurrentPillar, CurrentTrain, out approvalDate, out approver))
            {
                TrainReviewSnapshotDateBox.Text = approvalDate.Value.ToLongDateString();
                TrainReviewApproverBox.Text = approver;
            }
        }

        void PillarCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CurrentPillar = PillarCombo.SelectedItem as PillarItem;
            Globals.UserPreferences.SetItemSelectionPreference(ProductPreferences.LastSelectedTrainCommitmentPillar, CurrentPillar);
            ShowChart();
        }

        void TrainCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CurrentTrain = TrainCombo.SelectedItem as TrainItem;
            Globals.UserPreferences.SetItemSelectionPreference(ProductPreferences.LastSelectedTrainCommitmentTrain, CurrentTrain);
            ShowChart();
        }

        private void CopyToClipboardButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Globals.ApplicationManager.HandleRightClickContentMenus(this);
        }

    }
}
