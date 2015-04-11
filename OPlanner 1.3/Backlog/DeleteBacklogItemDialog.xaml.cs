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
    /// Interaction logic for DeleteBacklogItemDialog.xaml
    /// </summary>
    public partial class DeleteBacklogItemDialog : Window
    {
        public bool ShouldDelete { get; set; }

        public DeleteBacklogItemDialog(BacklogItem backlogItem)
        {
            InitializeComponent();
            Globals.ApplicationManager.SetStartupLocation(this);

            BacklogTitleBlock.Text = backlogItem.Title;
            BacklogPillarBlock.Text = backlogItem.PillarName;
            WorkItemGrid.ItemsSource = backlogItem.WorkItems;
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
    }
}
