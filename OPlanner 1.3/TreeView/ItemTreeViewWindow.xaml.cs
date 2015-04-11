using System.Windows;
using System.Windows.Controls.Ribbon;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for ItemTreeViewWindow.xaml
    /// </summary>
    public partial class ItemTreeViewWindow : RibbonWindow
    {
        public static PillarItem SelectedPillarItem { get; set; }
        public static QuarterItem SelectedQuarterItem { get; set; }
        public static string SortProperty { get; set; }

        public ItemTreeViewWindow(ForecastableItem item)
        {
            InitializeComponent();
            Utils.FitWindowToScreen(this);
            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            
            SortProperty = "BusinessRank";
            SelectedPillarItem = null;
            SelectedQuarterItem = null;

            this.Title = "TreeView: " + item.Title;
            TreeRoot.DataContext = item;
            ExperienceHeader.Visibility = System.Windows.Visibility.Collapsed;
            ScenarioHeader.Visibility = System.Windows.Visibility.Collapsed;

            switch (item.StoreItemType)
            {
                case ItemTypeID.Experience:
                    ExperienceItem experienceItem = (ExperienceItem) item;
                    TreeRoot.ItemsSource = experienceItem.GetTreeViewScenarioItems(SelectedPillarItem, SelectedQuarterItem, SortProperty);
                    ExperienceHeader.Visibility = System.Windows.Visibility.Visible;
                    break;
                case ItemTypeID.Scenario:
                    ScenarioItem scenarioItem = (ScenarioItem)item;
                    TreeRoot.ItemsSource = scenarioItem.BacklogItems;
                    ScenarioHeader.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

        private void Grid_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Globals.ApplicationManager.HandleRightClickContentMenus(this);
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            Globals.ApplicationManager.CommitChanges();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Globals.ApplicationManager.Refresh();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            Globals.ItemManager.UndoChanges();
        }
    }
}
