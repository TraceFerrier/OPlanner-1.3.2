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
    /// Interaction logic for AssignMemberToBacklogItem.xaml
    /// </summary>
    public partial class AssignMemberToBacklogItem : Window
    {
        public bool IsConfirmed { get; set; }
        public AssignMemberToBacklogItem(GroupMemberItem member, BacklogItem backlogItem)
        {
            InitializeComponent();
            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            DialogContext.DataContext = member;
            BacklogItemTitle.Text = backlogItem.Title;

            OKButton.Click += OKButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            Close();
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            Close();
        }
    }
}
