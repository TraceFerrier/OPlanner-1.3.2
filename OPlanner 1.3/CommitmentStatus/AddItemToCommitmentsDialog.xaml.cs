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
    /// Interaction logic for AddItemToCommitmentsDialog.xaml
    /// </summary>
    public partial class AddItemToCommitmentsDialog : Window
    {
        public bool Confirmed { get; set; }
        public AddItemToCommitmentsDialog(BacklogItem backlogItem)
        {
            InitializeComponent();
            Globals.ApplicationManager.SetStartupLocation(this);

            DialogContext.DataContext = backlogItem;
            OKButton.Click += OKButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }
    }
}
