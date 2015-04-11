using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Windows.Controls;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for EditOPlannerBugsView.xaml
    /// </summary>
    public partial class OPlannerBugsView : RibbonWindow
    {
        ItemsControlFilter<PlannerBugItem> Filter;

        public OPlannerBugsView()
        {
            InitializeComponent();
            Utils.FitWindowToScreen(this);

            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            Filter = new ItemsControlFilter<PlannerBugItem>(BugsGrid, ItemTypeID.PlannerBug);
            Filter.AddFilter(BugsStatusFilterCombo, ProductPreferences.LastSelectedBugsStatusFilterValue, ItemTypeID.FilterBugStatus);
            Filter.AddFilter(BugsAssignedToFilterCombo, ProductPreferences.LastSelectedBugsAssignedToValue, ItemTypeID.FilterBugAssignedTo);
            Filter.AddFilter(BugsIssueTypeFilterCombo, ProductPreferences.LastSelectedBugsIssueTypeValue, ItemTypeID.FilterIssueType);
            Filter.AddFilter(BugsResolutionFilterCombo, ProductPreferences.LastSelectedBugsResolutionValue, ItemTypeID.FilterResolution);
            Filter.UpdateFilter();

        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
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

        private void BacklogGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element != null && element is FrameworkElement)
            {
                if (((FrameworkElement)element).Parent is DataGridCell)
                {
                    PlannerBugItem bugItem = BugsGrid.SelectedItem as PlannerBugItem;
                    if (bugItem != null)
                    {
                        bugItem.ShowEditor(this);
                    }
                }
            }
        }
    }
}
