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

namespace PlannerNameSpace.Views
{
    /// <summary>
    /// Interaction logic for ReviewPagesView.xaml
    /// </summary>
    public partial class ReviewPagesView : UserControl
    {
        ReviewPagesViewState ViewState;

        public ReviewPagesView()
        {
            InitializeComponent();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called immediately after the view is created, allowing population of the data
        /// presented by the view.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void InitializeData()
        {
            ViewState = new ReviewPagesViewState(LastTrainResultsDataGrid, Globals.MainWindow.ReviewPillarFilterCombo, ProductPreferences.ReviewPagesReviewLastSelectedPillar,
                Globals.MainWindow.ReviewTrainFilterCombo, ProductPreferences.ReviewPagesReviewLastSelectedTrain, ReviewPageHeadingBox, ReviewPagePillarBox);
        }

        public void TabViewActivated()
        {
            if (ViewState != null)
            {
                BacklogItem.CurrentBacklogTrain = ViewState.TrainState.CurrentItem;
                BacklogItem.CurrentBacklogView = ViewState.CurrentReviewPage;
            }
        }

    }
}
