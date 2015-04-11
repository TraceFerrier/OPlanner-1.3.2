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
    /// Interaction logic for OPlannerNewBugDialog.xaml
    /// </summary>
    public partial class OPlannerBugDialog : Window
    {
        string CurrentTitle { get; set; }
        string CurrentAssignedTo { get; set; }
        string CurrentReproSteps { get; set; }
        string AssignedToDisplayName { get; set; }
        Button OKButton { get; set; }

        PlannerBugItem TargetBug { get; set; }

        public OPlannerBugDialog(Window owner, PlannerBugItem bugItem = null)
        {
            InitializeComponent();
            this.Owner = owner;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            var brushConverter = new BrushConverter();
            if (bugItem == null)
            {
                CurrentAssignedTo = StatusValues.Active;
                TargetBug = PlannerBugItem.CreateItem();
                TargetBug.BugAssignedTo = CurrentAssignedTo;
            }
            else
            {
                TargetBug = bugItem;
                this.Title = "OPlanner Bug: " + TargetBug.ID;
                CurrentTitle = TargetBug.Title;
                CurrentAssignedTo = TargetBug.BugAssignedTo;
                CurrentReproSteps = TargetBug.BugReproSteps;
                TargetBug.BugComments = null;
            }

            DialogContext.DataContext = TargetBug;

            if (TargetBug.IsNew)
            {
                OKButton = OkNewButton;
                OkNewButton.Visibility = System.Windows.Visibility.Visible;
                OkNewButton.Click += OkNewButton_Click;
                OkEditButton.Visibility = System.Windows.Visibility.Collapsed;
                DescriptionHistoryBox.IsEnabled = false;
            }
            else
            {
                OKButton = OkEditButton;
                OkEditButton.Visibility = System.Windows.Visibility.Visible;
                OkNewButton.Visibility = System.Windows.Visibility.Collapsed;
                OkEditButton.Click += OkEditButton_Click;

                if (TargetBug.IsResolvedAnyResolution)
                {
                    DialogContext.Background = (Brush)brushConverter.ConvertFrom(Globals.ResolvedStatusColor);
                    CloseButton.Visibility = System.Windows.Visibility.Visible;
                    ActivateButton.Visibility = System.Windows.Visibility.Visible;
                }

                if (TargetBug.IsActive)
                {
                    DialogContext.Background = (Brush)brushConverter.ConvertFrom(Globals.ActiveStatusColor);
                    ResolveButton.Visibility = System.Windows.Visibility.Visible;
                }

                if (TargetBug.IsClosedAnyResolution)
                {
                    DialogContext.Background = (Brush)brushConverter.ConvertFrom(Globals.ClosedStatusColor);
                    ActivateButton.Visibility = System.Windows.Visibility.Visible;
                }
            }

            ResolveButton.Click += ResolveButton_Click;
            CloseButton.Click += CloseButton_Click;
            ActivateButton.Click += ActivateButton_Click;
            CancelButton.Click += CancelButton_Click;
            UpdateUI();
        }

        void OkEditButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Globals.ItemManager.CommitChanges();
        }

        void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            TargetBug.ActivateItem();
            Globals.ItemManager.CommitChanges();
        }

        void ResolveButton_Click(object sender, RoutedEventArgs e)
        {
            SelectResolutionDialog dialog = new SelectResolutionDialog(this, TargetBug.BugComments);
            dialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(dialog.ResolutionValue))
            {
                Close();

                TargetBug.BugComments = dialog.ResolutionComments;
                TargetBug.ResolveItem(dialog.ResolutionValue);
                Globals.ItemManager.CommitChanges();
            }
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            TargetBug.CloseItem();
            Globals.ItemManager.CommitChanges();
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TargetBug.UndoChanges();
            Close();
        }

        void OkNewButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            TargetBug.SaveNewItem();
            Globals.ItemManager.CommitChanges();
        }

        void ValidateData()
        {
            bool dataValid = false;
            if (!string.IsNullOrWhiteSpace(CurrentTitle))
            {
                if (!string.IsNullOrWhiteSpace(CurrentReproSteps))
                {
                    if (!string.IsNullOrWhiteSpace(TargetBug.BugIssueType))
                    {
                        if (Utils.StringsMatch(CurrentAssignedTo, StatusValues.Active) || !string.IsNullOrWhiteSpace(AssignedToDisplayName))
                        {
                            dataValid = true;
                        }
                    }
                }
            }

            OKButton.IsEnabled = dataValid;
        }

        private void TitleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            CurrentTitle = box.Text;
            ValidateData();
        }

        private void IssueType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            ValidateData();
        }

        private void ReproSteps_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            CurrentReproSteps = box.Text;
            ValidateData();
        }

        private void AssignedTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            CurrentAssignedTo = box.Text;
            UpdateUI();
        }

        void UpdateUI()
        {
            AssignedToDisplayName = UserInformation.GetDisplayNameFromAlias(CurrentAssignedTo);
            DisplayNameBox.Text = AssignedToDisplayName;
            AssignedToImage.Source = UserInformation.GetImageFromAlias(CurrentAssignedTo);

            if (Utils.StringsMatch(CurrentAssignedTo, StatusValues.Active) || !string.IsNullOrWhiteSpace(AssignedToDisplayName))
            {
                AssignedToBox.Background = Brushes.White;
            }
            else
            {
                AssignedToBox.Background = Brushes.LightPink;
            }
            ValidateData();
        }
    }
}
