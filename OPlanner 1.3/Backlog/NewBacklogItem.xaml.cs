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
    /// Interaction logic for NewBacklogItem.xaml
    /// </summary>
    public partial class NewBacklogItemDialog : Window
    {
        public bool DialogConfirmed { get; set; }
        public string BacklogItemTitle { get; set; }
        public PillarItem SelectedPillarItem { get; set; }
        public TrainItem SelectedTrainItem { get; set; }
        public string SelectedSpec { get; set; }
        public BacklogItem TargetBacklogItem;

        public NewBacklogItemDialog(BacklogItem backlogItem, PillarItem pillarItem, TrainItem trainItem)
        {
            InitializeComponent();
            Globals.ApplicationManager.SetStartupLocation(this);

            // Set up title
            TargetBacklogItem = backlogItem;
            BacklogItemTitle = backlogItem.Title;
            TitleBox.Text = BacklogItemTitle;
            TitleBox.TextChanged += TitleBox_TextChanged;
            BindSpecTeamListCombo();

            // Set up Pillar combo
            PillarCombo.ItemsSource = PillarManager.PillarItems;
            if (!StoreItem.IsRealItem(pillarItem))
            {
                PillarCombo.SelectedIndex = 0;
            }
            else
            {
                PillarCombo.SelectedItem = pillarItem;
            }

            SelectedPillarItem = PillarCombo.SelectedItem as PillarItem;

            TrainCombo.ItemsSource = TrainManager.Instance.GetTrains(TrainTimeFrame.CurrentOrFuture);
            SpecCombo.ItemsSource = backlogItem.TeamSpecValues;

            if (!StoreItem.IsRealItem(trainItem) || trainItem.TimeFrame == TrainTimeFrame.Past)
            {
                trainItem = TrainManager.Instance.CurrentTrain;
            }

            TrainCombo.SelectedItem = trainItem;
            SelectedTrainItem = trainItem;

            PillarCombo.SelectionChanged += PillarCombo_SelectionChanged;
            TrainCombo.SelectionChanged += TrainCombo_SelectionChanged;
            SpecCombo.SelectionChanged += SpecCombo_SelectionChanged;

            ToolTip toolTip = new System.Windows.Controls.ToolTip();
            toolTip.Content = "Tip: Select '" + Globals.c_SpecTBD + "' if you intend to create a spec for this backlog item in your product group's spec library, but haven't done so yet.";
            SpecCombo.ToolTip = toolTip;

            UpdateUI();
        }

        void BindSpecTeamListCombo()
        {
            SpecTeamListCombo.ItemsSource = TargetBacklogItem.SpecTeamList;
            SpecTeamListCombo.SelectedValuePath = "Value";
            SpecTeamListCombo.DisplayMemberPath = "Value";

            foreach (AllowedValue allowedValue in TargetBacklogItem.SpecTeamList)
            {
                if (Utils.StringsMatch(allowedValue.Value as string, TargetBacklogItem.SpecTeam))
                {
                    SpecTeamListCombo.SelectedValue = allowedValue;
                    break;
                }
            }

            SpecTeamListCombo.SelectionChanged += SpecTeamListCombo_SelectionChanged;
        }

        void SpecTeamListCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AllowedValue specTeamValue = SpecTeamListCombo.SelectedItem as AllowedValue;
            if (specTeamValue != null)
            {
                TargetBacklogItem.SpecTeam = specTeamValue.Value as string;
                SpecCombo.ItemsSource = TargetBacklogItem.TeamSpecValues;
                UpdateUI();
            }
        }

        void TitleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BacklogItemTitle = TitleBox.Text;
            UpdateUI();
        }

        void TrainCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTrainItem = TrainCombo.SelectedItem as TrainItem;
            UpdateUI();
        }

        void PillarCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPillarItem = PillarCombo.SelectedItem as PillarItem;
            UpdateUI();
        }

        void SpecCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AllowedValue value = SpecCombo.SelectedItem as AllowedValue;
            if (value != null)
            {
                SelectedSpec = value.Value.ToString();
            }
            else
            {
                SelectedSpec = null;
            }

            UpdateUI();
        }

        void UpdateUI()
        {
            bool isSpecValid = BacklogItem.IsSpecValid(SelectedSpec);

            if (!string.IsNullOrWhiteSpace(BacklogItemTitle) && SelectedPillarItem != null && SelectedTrainItem != null && isSpecValid)
            {
                OKButton.IsEnabled = true;
            }
            else
            {
                OKButton.IsEnabled = false;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogConfirmed = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogConfirmed = false;
            SelectedPillarItem = null;
            this.Close();
        }
    }
}
