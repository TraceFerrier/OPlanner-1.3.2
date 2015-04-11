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
    /// Interaction logic for ScenarioView.xaml
    /// </summary>
    public partial class ScenarioView : UserControl
    {
        public static QuarterItem SelectedQuarterItem { get; set; }
        ItemsControlFilter<ScenarioItem> Filter;

        public ScenarioView()
        {
            InitializeComponent();
        }

        public void InitializeData()
        {
            ViewContext.DataContext = Globals.ItemManager.ScenarioItems;

            Globals.MainWindow.ScenarioQuarterFilterCombo.SelectionChanged += ScenarioQuarterFilterCombo_SelectionChanged;
            Globals.MainWindow.ScenarioTabCreateButton.Click += ScenarioCreateButton_Click;

            Filter = new ItemsControlFilter<ScenarioItem>(ScenarioItemsControl, ItemTypeID.Scenario);
            Filter.AddFilter(Globals.MainWindow.ScenarioPillarFilterCombo, ProductPreferences.LastSelectedScenarioViewPillarItem, ItemTypeID.Pillar);
            Filter.AddFilter(Globals.MainWindow.ScenarioQuarterFilterCombo, ProductPreferences.LastSelectedScenarioViewQuarterItem, ItemTypeID.Quarter);
            Filter.AddSortProperty(Globals.MainWindow.ScenarioBusinessRankSortingCombo, "Title", ProductPreferences.LastSelectedScenarioSortPropertyValue);
            Filter.AddSortProperty(Globals.MainWindow.ScenarioBusinessRankSortingCombo, "BusinessRank", ProductPreferences.LastSelectedScenarioSortPropertyValue);
            Filter.UpdateFilter();

            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;

        }

        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            if (e.Change.ChangeType == ChangeType.Added || e.Change.ChangeType == ChangeType.Removed)
            {
                Filter.UpdateFilter();
            }
        }

        void ScenarioQuarterFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedQuarterItem = Globals.MainWindow.ScenarioQuarterFilterCombo.SelectedItem as QuarterItem;
        }

        void Handle_StoreItemChanges(StoreItem item)
        {
            Filter.UpdateFilter();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user would like to create a new scenario.
        /// </summary>
        //------------------------------------------------------------------------------------
        void ScenarioCreateButton_Click(object sender, RoutedEventArgs e)
        {
            PillarItem parentPillarItem = Globals.MainWindow.ScenarioPillarFilterCombo.SelectedItem as PillarItem;
            if (parentPillarItem == null || parentPillarItem.IsDummyItem)
            {
                parentPillarItem = null;
            }

            QuarterItem parentQuarterItem = Globals.MainWindow.ScenarioQuarterFilterCombo.SelectedItem as QuarterItem;
            ScenarioItem scenarioItem = ScenarioItem.CreateScenarioItem(parentPillarItem, null, parentQuarterItem);
            Filter.UpdateFilter();

            // Scroll to the end to display the newly created item
            ScenarioScroller.ScrollToEnd();
        }


        private void BacklogGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void BacklogItemEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BacklogItemClose_ContextClick(object sender, RoutedEventArgs e)
        {

        }

        private void BacklogItemDelete_ContextClick(object sender, RoutedEventArgs e)
        {

        }

        private void BacklogItemMoveToNextTrain_ContextClick(object sender, RoutedEventArgs e)
        {

        }

        private void PillarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TrainComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ScenarioComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
