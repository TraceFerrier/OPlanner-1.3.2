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
    /// Interaction logic for RequestDeleteItemDialog.xaml
    /// </summary>

    public partial class RequestDeleteItemDialog : Window
    {
        public bool DialogConfirmed { get; set; }

        public RequestDeleteItemDialog(Window owner, StoreItem itemToDelete)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            Title = "Delete " + itemToDelete.StoreItemTypeName;
            HeadingBox.Text = "Are you sure you want to delete this " + itemToDelete.StoreItemTypeName + "?";

            DialogContext.DataContext = itemToDelete;

            OKButton.Click += OKButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            DialogConfirmed = true;
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            DialogConfirmed = false;
        }
    }
}
