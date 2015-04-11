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

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for DeleteFeatureTeamDialog.xaml
    /// </summary>
    public partial class DeleteFeatureTeamDialog : Window
    {
        public bool ShouldDelete { get; set; }

        public DeleteFeatureTeamDialog(ScrumTeamItem scrumTeamItem)
        {
            InitializeComponent();
            Globals.ApplicationManager.SetStartupLocation(this);
            BacklogItemGrid.ItemsSource = scrumTeamItem.OwnedBacklogItems;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ShouldDelete = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ShouldDelete = false;
            Close();
        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Globals.ApplicationManager.HandleRightClickContentMenus(this);
        }
    }
}
