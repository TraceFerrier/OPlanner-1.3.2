using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for BacklogWorkItemsWindow.xaml
    /// </summary>
    public partial class BacklogWorkItemsWindow : RibbonWindow
    {
        BacklogItem TargetBacklogItem { get; set; }
        public BacklogWorkItemsWindow(BacklogItem backlogItem)
        {
            InitializeComponent();

            Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            Loaded += Window_Loaded;
            SizeChanged += Window_SizeChanged;

            TargetBacklogItem = backlogItem;
            WorkItemGrid.DataContext = TargetBacklogItem;
            WorkItemDataGrid.ItemsSource = TargetBacklogItem.WorkItems;
        }

        void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePanelSize();
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePanelSize();
        }

        void UpdatePanelSize()
        {
            //WorkItemsPanel.Width = this.ActualWidth - 20;
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
