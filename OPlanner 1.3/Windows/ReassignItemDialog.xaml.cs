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
    /// Interaction logic for ReassignItemDialog.xaml
    /// </summary>
    public partial class ReassignItemDialog : Window
    {
        public GroupMemberItem SelectedMember { get; set; }
        public ReassignItemDialog(ScrumTeamItem featureTeam)
        {
            InitializeComponent();

            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            OkButton.IsEnabled = false;

            TeamMemberCombo.SelectionChanged += TeamMemberCombo_SelectionChanged;
            TeamMemberCombo.ItemsSource = featureTeam.Members;
            TeamMemberCombo.DisplayMemberPath = "Title";
        }

        void TeamMemberCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedMember = TeamMemberCombo.SelectedItem as GroupMemberItem;
            OkButton.IsEnabled = SelectedMember != null ? true : false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedMember = null;
            Close();
        }
    }
}
