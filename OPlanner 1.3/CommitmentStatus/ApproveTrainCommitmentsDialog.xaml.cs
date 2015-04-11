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
    /// Interaction logic for ApproveTrainCommitmentsDialog.xaml
    /// </summary>
    public partial class ApproveTrainCommitmentsDialog : Window
    {
        public bool IsDialogConfirmed { get; set; }
        public ApproveTrainCommitmentsDialog(PillarItem pillar, TrainItem train, int backlogItemCount)
        {
            InitializeComponent();
            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            PillarBox.Text = pillar.Title;
            TrainBox.Text = train.Title;
            ItemCountBox.Text = backlogItemCount.ToString();

            OKButton.Click += OKButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsDialogConfirmed = false;
            Close();
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            IsDialogConfirmed = true;
            Close();
        }
    }
}
