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
    /// Interaction logic for ScenarioMoveToNextQuarterDialog.xaml
    /// </summary>
    public partial class ScenarioMoveToNextQuarterDialog : Window
    {
        public ScenarioItem TargetScenario { get; set; }
        public string ScenarioNewName { get; set; }

        public ScenarioMoveToNextQuarterDialog(ScenarioItem scenario, AsyncObservableCollection<BacklogItem> moveableBacklogItems)
        {
            InitializeComponent();
            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            TargetScenario = scenario;
            ScenarioNameBox.Text = TargetScenario.Title;

            ScenarioNewName = TargetScenario.Title + " (Continued)";
            ScenarioNewNameBox.Text = ScenarioNewName;

            BacklogGrid.ItemsSource = moveableBacklogItems;
            BacklogGrid.SelectionChanged += BacklogGrid_SelectionChanged;
        }

        void BacklogGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BacklogItem selectedItem = BacklogGrid.SelectedItem as BacklogItem;
            if (selectedItem != null)
            {
                WorkItemsGrid.ItemsSource = selectedItem.WorkItems;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
