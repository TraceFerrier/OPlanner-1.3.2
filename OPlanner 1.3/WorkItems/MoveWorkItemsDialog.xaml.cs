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

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for MoveWorkItemsDialog.xaml
    /// </summary>
    public partial class MoveWorkItemsDialog : Window
    {
        public MoveWorkItemsDialog(AsyncObservableCollection<WorkItem> workItems, PillarItem selectedPillar, TrainItem selectedTrain, ScrumTeamItem selectedScrumTeam)
        {
            InitializeComponent();

            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            OkButton.IsEnabled = false;
            WorkItemsGrid.ItemsSource = workItems;

            PillarTrainScrumTeamViewState ViewState = new PillarTrainScrumTeamViewState(BacklogPillarCombo, selectedPillar, BacklogTrainCombo, selectedTrain, ScrumTeamCombo, selectedScrumTeam);
            PillarTrainScrumTeamItemCollection backlogItemCollection = new PillarTrainScrumTeamItemCollection(ViewState);

            backlogItemCollection.SetItemsControl(BacklogGrid);
            BacklogGrid.SelectionChanged += BacklogGrid_SelectionChanged;

        }

        void BacklogGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BacklogItem selectedBacklogItem = BacklogGrid.SelectedItem as BacklogItem;
            OkButton.IsEnabled = selectedBacklogItem != null;
        }

        private void BacklogGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();

            BacklogItem parentBacklogItem = BacklogGrid.SelectedItem as BacklogItem;
            if (parentBacklogItem != null)
            {
                foreach (WorkItem workItem in WorkItemsGrid.Items)
                {
                    workItem.ParentBacklogItem = parentBacklogItem;
                }
            }

            Globals.ItemManager.CommitChanges();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
