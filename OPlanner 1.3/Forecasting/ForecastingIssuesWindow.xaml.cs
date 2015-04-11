using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for ForecastingIssuesWindow.xaml
    /// </summary>
    public partial class ForecastingIssuesWindow : RibbonWindow
    {
        ForecastableItem TargetItem { get; set; }
        public ForecastingIssuesWindow(ForecastableItem item, PlanningIssue initialIssue)
        {
            InitializeComponent();
            Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            Loaded += ForecastingIssuesWindow_Loaded;
            SizeChanged += ForecastingIssuesWindow_SizeChanged;

            TargetItem = item;
            ForecastingIssuesGrid.DataContext = TargetItem;
            ChangeIssue(initialIssue);

            AsyncObservableCollection<string> filterItems = new AsyncObservableCollection<string>();
            filterItems = Utils.GetEnumValues<PlanningIssue>();
            IssueCombo.ItemsSource = filterItems;
            IssueCombo.SelectedValue = Utils.EnumToString<PlanningIssue>(initialIssue);
            IssueCombo.SelectionChanged += IssueCombo_SelectionChanged;
        }

        void IssueCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filterSelection = IssueCombo.SelectedValue as string;
            PlanningIssue issue = Utils.StringToEnum<PlanningIssue>(filterSelection);
            ChangeIssue(issue);
        }

        public void ChangeIssue(PlanningIssue issue)
        {
            BacklogItemsControl.ItemsSource = TargetItem.GetItemsWithPlanningIssues(issue);
        }

        void ForecastingIssuesWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePanelSize();
        }

        void ForecastingIssuesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePanelSize();
        }

        void UpdatePanelSize()
        {
            BacklogItemsPanel.Width = this.ActualWidth - 20;
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

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Globals.ApplicationManager.HandleRightClickContentMenus(this);
        }
    }
}
