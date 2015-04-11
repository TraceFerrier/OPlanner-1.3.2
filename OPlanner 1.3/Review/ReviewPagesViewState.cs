using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;

namespace PlannerNameSpace
{
    public class ReviewPagesViewState : PillarTrainViewState
    {
        public ColumnViewManager ViewManager;
        public DataGrid ReviewPagesDataGrid;
        public event ViewStateChangedEventHandler ReviewPageChanged;
        public TextBlock ReviewPageHeadingBox;
        public TextBlock ReviewPagePillarBox;
        ReviewPagesViewItemCollection ItemCollection;

        public ReviewPagesViewState(DataGrid reviewPagesDataGrid, ComboBox pillarCombo, ProductPreferences pillarPreference, ComboBox trainCombo,
            ProductPreferences trainPreference, TextBlock reviewPageHeadingBox, TextBlock reviewPagePillarBox)
            : base(pillarCombo, pillarPreference, trainCombo, trainPreference)
        {
            ReviewPagesDataGrid = reviewPagesDataGrid;
            ReviewPageHeadingBox = reviewPageHeadingBox;
            ReviewPagePillarBox = reviewPagePillarBox;
            SetUpViewColumns(reviewPagesDataGrid);

            ItemCollection = new ReviewPagesViewItemCollection(this);
            ItemCollection.SetItemsControl(ReviewPagesDataGrid);

            SetReviewPageInformation();
            if (PillarState.CurrentItem != null)
            {
                ReviewPagePillarBox.Text = PillarState.CurrentItem.Title;
            }

            PillarState.CurrentItemChanged += PillarState_CurrentItemChanged;
            TrainState.CurrentItemChanged += TrainState_CurrentItemChanged;
        }

        void PillarState_CurrentItemChanged(object sender, EventArgs e)
        {
            ReviewPagePillarBox.Text = PillarState.CurrentItem.Title;
            ItemCollection.RefreshCollection();
        }

        void TrainState_CurrentItemChanged(object sender, EventArgs e)
        {
            SetReviewPageInformation();
            ItemCollection.RefreshCollection();
        }

        void ReviewPagesViewState_TrainChanged()
        {
            SetReviewPageInformation();
            ItemCollection.RefreshCollection();
        }

        void ViewManager_ViewSelectionChanged(AvailableViews oldViewName, AvailableViews newViewName)
        {
            BacklogItem.CurrentBacklogView = newViewName;
            SetReviewPageInformation();
            ItemCollection.RefreshCollection();

            if (ReviewPageChanged != null)
            {
                ReviewPageChanged();
            }
        }

        public AvailableViews CurrentReviewPage
        {
            get { return ViewManager.CurrentView.ViewName; }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes the settings for the first panel views.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SetUpViewColumns(DataGrid reviewPagesDataGrid)
        {
            // Set up view manager
            ViewManager = new ColumnViewManager(reviewPagesDataGrid, ProductPreferences.ReviewPagesLastSelectedPage, ProductPreferences.LastSelectedBacklogViewTypeValue, AvailableViews.BacklogLastTrainResultRecap, ViewType.DataGridView);
            ViewManager.AddViewControl(AvailableViews.BacklogLastTrainResultRecap, Globals.MainWindow.ReviewLastTrainResultsRadioButton);
            ViewManager.AddViewControl(AvailableViews.BacklogThisTrainCommitments, Globals.MainWindow.ReviewThisTrainCommitmentsRadioButton);
            ViewManager.AddViewControl(AvailableViews.BacklogThisTrainNonCommitments, Globals.MainWindow.ReviewUncommittedItemsRadioButton);

            ViewManager.AddViewColumn(AvailableViews.BacklogLastTrainResultRecap, PlannerContent.ColumnHeaderBacklogTitle);
            ViewManager.AddViewColumn(AvailableViews.BacklogLastTrainResultRecap, "Commitment Recap");
            ViewManager.AddViewColumn(AvailableViews.BacklogLastTrainResultRecap, "PM");
            ViewManager.AddViewColumn(AvailableViews.BacklogLastTrainResultRecap, "Dev");
            ViewManager.AddViewColumn(AvailableViews.BacklogLastTrainResultRecap, "Test");
            ViewManager.AddViewColumn(AvailableViews.BacklogLastTrainResultRecap, PlannerContent.ColumnHeaderLandingDate);

            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainCommitments, PlannerContent.ColumnHeaderBacklogTitle);
            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainCommitments, "Commitment Type");
            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainCommitments, "Commitment Status");
            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainCommitments, "PM");
            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainCommitments, "Dev");
            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainCommitments, "Test");
            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainCommitments, PlannerContent.ColumnHeaderLandingDate);

            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainNonCommitments, PlannerContent.ColumnHeaderBacklogTitle);
            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainNonCommitments, "Commitment");
            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainNonCommitments, "Commitment Status");
            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainNonCommitments, "PM");
            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainNonCommitments, "Spec Status");
            ViewManager.AddViewColumn(AvailableViews.BacklogThisTrainNonCommitments, PlannerContent.ColumnHeaderLandingDate);

            ViewManager.InitializeView();
            ViewManager.ViewSelectionChanged += ViewManager_ViewSelectionChanged;
        }

        void SetReviewPageInformation()
        {
            switch (CurrentReviewPage)
            {
                case AvailableViews.BacklogLastTrainResultRecap:
                    BacklogItem.CurrentBacklogTrain = TrainState.CurrentItem;
                    Utils.SetCustomSorting(ReviewPagesDataGrid, new CommitmentRecapSort(), System.ComponentModel.ListSortDirection.Ascending);
                    ReviewPageHeadingBox.Text = TrainState.CurrentItem.Title + ": Recap and Carry Over";
                    break;
                case AvailableViews.BacklogThisTrainCommitments:
                    BacklogItem.CurrentBacklogTrain = TrainState.CurrentItem;
                    Utils.SetCustomSorting(ReviewPagesDataGrid, new CommitmentNextTrainSort(), System.ComponentModel.ListSortDirection.Ascending);
                    ReviewPageHeadingBox.Text = NextTrain.Title + ": Committed (or carried over from previous train)";
                    break;
                case AvailableViews.BacklogThisTrainNonCommitments:
                    BacklogItem.CurrentBacklogTrain = TrainState.CurrentItem;
                    Utils.SetCustomSorting(ReviewPagesDataGrid, new LandingDateSort(), System.ComponentModel.ListSortDirection.Ascending);
                    ReviewPageHeadingBox.Text = NextTrain.Title + ": (Not Committed)";
                    break;
            }
        }

    }
}
