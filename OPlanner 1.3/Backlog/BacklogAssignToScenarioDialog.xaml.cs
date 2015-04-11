using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for BacklogAssignToScenarioDialog.xaml
    /// </summary>
    public partial class BacklogAssignToScenarioDialog : Window
    {
        ItemsControlFilter<ScenarioItem> Filter;

        public BacklogAssignToScenarioDialog(AsyncObservableCollection<BacklogItem> backlogItems, PillarItem selectedPillar)
        {
            InitializeComponent();

            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            OkButton.IsEnabled = false;
            BacklogGrid.ItemsSource = backlogItems;

            Filter = new ItemsControlFilter<ScenarioItem>(ScenarioGrid, ItemTypeID.Scenario);
            Filter.AddFilter(PillarCombo, selectedPillar, ItemTypeID.Pillar);
            Filter.AddFilter(QuarterCombo, ProductPreferences.LastSelectedBacklogAssignToScenarioQuarterItem, ItemTypeID.Quarter);
            Filter.UpdateFilter();

            ScenarioGrid.SelectionChanged += ScenarioGrid_SelectionChanged;
        }

        void ScenarioGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScenarioItem selectedScenario = ScenarioGrid.SelectedItem as ScenarioItem;
            if (selectedScenario != null)
            {
                OkButton.IsEnabled = true;
            }
            else
            {
                OkButton.IsEnabled = false;
            }
        }

        private void BacklogGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();

            ScenarioItem parentScenarioItem = ScenarioGrid.SelectedItem as ScenarioItem;
            if (parentScenarioItem != null)
            {
                foreach (BacklogItem backlogItem in BacklogGrid.Items)
                {
                    backlogItem.ParentScenarioItem = parentScenarioItem;
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
