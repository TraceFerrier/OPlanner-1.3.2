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
    /// Interaction logic for WorkItemBackToActiveDialog.xaml
    /// </summary>
    public partial class WorkItemBackToActiveDialog : Window
    {
        public bool IsConfirmed { get; set; }
        WorkItem TargetWorkItem { get; set; }
        public int Estimate { get; set; }
        public int Completed { get; set; }

        public WorkItemBackToActiveDialog(WorkItem workItem)
        {
            InitializeComponent();

            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            TargetWorkItem = workItem;
            DialogContext.DataContext = workItem;

            this.Loaded += WorkItemBackToActiveDialog_Loaded;
        }

        void WorkItemBackToActiveDialog_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateButtons();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Estimate = Utils.GetIntValue(EstimateBox.Text);
            Completed = Utils.GetIntValue(CompletedBox.Text);
            IsConfirmed = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            Close();
        }

        private void UpdateUI()
        {
            int estimate = Utils.GetIntValue(EstimateBox.Text);
            int completed = Utils.GetIntValue(CompletedBox.Text);
            if (completed > estimate)
            {
                completed = estimate;
                CompletedBox.Text = completed.ToString();
            }

            int workRemaining = estimate - completed;
            if (workRemaining < 0)
            {
                workRemaining = 0;
            }

            RemainingBox.Text = workRemaining.ToString();

            UpdateButtons();
        }

        void UpdateButtons()
        {
            int workRemaining = Utils.GetIntValue(RemainingBox.Text);
            if (workRemaining > 0)
            {
                OkButton.IsEnabled = true;
            }
            else
            {
                OkButton.IsEnabled = false;
            }
        }

        private void EstimateLow_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        private void EstimateHigh_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        private void CompletedBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }
    }
}
