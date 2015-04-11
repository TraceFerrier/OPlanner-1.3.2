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
    /// Interaction logic for NewFeatureTeamDialog.xaml
    /// </summary>
    public partial class NewOrEditScrumTeamDialog : Window
    {
        public bool Confirmed { get; set; }

        public NewOrEditScrumTeamDialog(ScrumTeamItem featureTeam)
        {
            InitializeComponent();

            Globals.ApplicationManager.SetStartupLocation(this);

            OKButton.IsEnabled = false;

            DialogContext.DataContext = featureTeam;
        }

        private void OKButton_Clicked(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }

        private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ShouldEnable();
        }

        private void PillarCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShouldEnable();
        }

        private void TrainCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShouldEnable();
        }

        private void ScrumMasterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShouldEnable();
        }

        private void ShouldEnable()
        {
            OKButton.IsEnabled = TitleTextBox.Text.Length > 0 &&
                PillarCombo.SelectedItem != null &&
                ScrumMasterCombo.SelectedItem != null;
        }
    }
}
