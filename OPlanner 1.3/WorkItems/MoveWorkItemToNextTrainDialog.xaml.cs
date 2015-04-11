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
    /// Interaction logic for MoveWorkItemToNextTrainDialog.xaml
    /// </summary>
    public partial class MoveWorkItemToNextTrainDialog : Window
    {
        public bool IsMoveConfirmed { get; set; }
        public string NewBacklogItemName { get; set; }

        public MoveWorkItemToNextTrainDialog(WorkItem workItem, string nextTrainTitle, string newBacklogItemTitle)
        {
            InitializeComponent();

            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            IsMoveConfirmed = false;
            DialogContext.DataContext = workItem;

            NewBacklogItemName = newBacklogItemTitle;
            NextTrainNameBox.Text = nextTrainTitle;
            NewBacklogItemNameBox.Text = NewBacklogItemName;
            NewBacklogItemNameBox.TextChanged += NewBacklogItemNameBox_TextChanged;
            OKButton.Click += OKButton_Click;
            CancelButton.Click += CancelButton_Click;
            UpdateUI();
        }

        void NewBacklogItemNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NewBacklogItemName = NewBacklogItemNameBox.Text;
            UpdateUI();
        }

        void UpdateUI()
        {
            if (string.IsNullOrWhiteSpace(NewBacklogItemName) || NewBacklogItemName.Length < 2)
            {
                OKButton.IsEnabled = false;
            }
            else
            {
                OKButton.IsEnabled = true;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            NewBacklogItemName = NewBacklogItemNameBox.Text;
            IsMoveConfirmed = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
