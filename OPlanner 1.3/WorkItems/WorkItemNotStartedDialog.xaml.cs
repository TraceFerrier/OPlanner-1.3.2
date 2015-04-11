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

namespace PlannerNameSpace.WorkItems
{
    /// <summary>
    /// Interaction logic for WorkItemNotStartedDialog.xaml
    /// </summary>
    public partial class WorkItemNotStartedDialog : Window
    {
        public bool IsConfirmed { get; set; }

        public WorkItemNotStartedDialog(WorkItem workItem)
        {
            InitializeComponent();
            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            DialogContext.DataContext = workItem;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            Close();
        }
    }
}
