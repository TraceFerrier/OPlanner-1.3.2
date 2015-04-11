using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for GotoItem.xaml
    /// </summary>
    public partial class GotoItem : Window
    {
        public int ItemID { get; set; }
        public GotoItem()
        {
            InitializeComponent();

            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            OkButton.IsEnabled = false;
            ItemID = 0;

            this.KeyDown += GotoItem_KeyDown;

            ItemIDBox.Focus();
        }

        void GotoItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && OkButton.IsEnabled)
            {
                Close();
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ItemID = 0;
            Close();
        }

        private void ItemIDBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ItemIDBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string idString = ItemIDBox.Text;
            ItemID = Utils.GetIntValue(ItemIDBox.Text);

            OkButton.IsEnabled = ItemID > 0;
        }
    }
}
