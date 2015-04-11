using System;
using System.Collections;
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
    /// Interaction logic for DeleteSelectedBacklogItemsDialog.xaml
    /// </summary>
    public partial class DeleteSelectedBacklogItemsDialog : Window
    {
        public bool DialogConfirmed;
        StoreItemCollection<BacklogItem> SelectedItems;

        public DeleteSelectedBacklogItemsDialog(IList backlogItems)
        {
            InitializeComponent();
            Globals.ApplicationManager.SetStartupLocation(this);

            SelectedItems = ItemManager.GetStoreItemsFromList<BacklogItem>(backlogItems);
            int totalWorkRemaining = ItemManager.GetTotalWorkRemaining(SelectedItems);

            DeleteCountBlock.Text = SelectedItems.Count.ToString();
            TotalWorkRemainingBlock.Text = totalWorkRemaining.ToString() + " hours";
            BacklogItemGrid.ItemsSource = SelectedItems;

            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogConfirmed = true;
            Close();

            ItemManager.DeleteItems<BacklogItem>(SelectedItems);
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogConfirmed = false;
            Close();
        }

    }
}
