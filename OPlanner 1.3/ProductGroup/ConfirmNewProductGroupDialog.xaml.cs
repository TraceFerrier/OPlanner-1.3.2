using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ConfirmNewProductGroupDialog.xaml
    /// </summary>
    public partial class ConfirmNewProductGroupDialog : Window
    {
        public bool DialogConfirmed { get; set; }

        public ConfirmNewProductGroupDialog(Window owner, ProductGroupItem productGroup, AsyncObservableCollection<MemberDescriptor> members)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            DialogContext.DataContext = productGroup;
            DiscoveredMembersGrid.ItemsSource = members;

            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            DialogConfirmed = false;
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            DialogConfirmed = true;
        }
    }
}
