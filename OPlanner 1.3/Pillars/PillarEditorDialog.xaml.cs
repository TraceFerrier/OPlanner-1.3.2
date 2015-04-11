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
using System.Windows.Threading;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for PillarEditorDialog.xaml
    /// </summary>
    public partial class PillarEditorDialog : Window
    {
        PillarItem TargetPillarItem { get; set; }
        public int SelectedFeatureAreaID { get; set; }

        public PillarEditorDialog(PillarItem pillarItem)
        {
            InitializeComponent();
            Globals.ApplicationManager.SetStartupLocation(this);

            this.Closing += PillarEditorDialog_Closing;
            Globals.ProductTreeManager.PopulateTree(PillarAreaTreeView);

            if (pillarItem == null)
            {
                pillarItem = PillarItem.CreatePillarItem();
                pillarItem.PillarPathID = Globals.UserPreferences.GetProductPreference<int>(ProductPreferences.LastSelectedNewPillarPathID);
            }

            TargetPillarItem = pillarItem;
            DialogContext.DataContext = TargetPillarItem;
            SelectedFeatureAreaID = TargetPillarItem.PillarPathID;
            Globals.ProductTreeManager.SelectFeatureAreaTreeNode(PillarAreaTreeView, SelectedFeatureAreaID);
        }

        void PillarEditorDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
        }

        public void CloseDialog()
        {
            Close();
        }

        private void FeatureAreaTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedFeatureAreaID = Globals.ProductTreeManager.GetSelectedAreaTreeID(PillarAreaTreeView);
            Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedNewPillarPathID, SelectedFeatureAreaID);
        }

        private void OKButton_Clicked(object sender, RoutedEventArgs e)
        {
            TargetPillarItem.PillarPathID = SelectedFeatureAreaID;

            if (IsPillarValid(TargetPillarItem))
            {
                if (TargetPillarItem.IsNew)
                {
                    TargetPillarItem.SaveNewItem();
                }

                CloseDialog();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!TargetPillarItem.IsNew)
            {
                TargetPillarItem.UndoChanges();
            }

            CloseDialog();
        }

        bool IsPillarValid(PillarItem pillarItem)
        {
            if (string.IsNullOrWhiteSpace(pillarItem.Title))
            {
                UserMessage.Show(PlannerContent.PillarValidationNoTitle);
                return false;
            }

            if (!PillarManager.Instance.IsPillarValid(pillarItem, PillarValidationType.Title))
            {
                UserMessage.Show(PlannerContent.PillarValidationInvalidTitle);
                return false;
            }

            if (!PillarManager.Instance.IsPillarValid(pillarItem, PillarValidationType.PillarPathID))
            {
                UserMessage.Show(PlannerContent.PillarValidationInvalidPillarPath);
                return false;
            }

            return true;
        }
    }
}
