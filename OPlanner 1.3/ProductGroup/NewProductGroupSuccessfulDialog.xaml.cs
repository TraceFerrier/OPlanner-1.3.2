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
    /// Interaction logic for NewProductGroupSuccessfulDialog.xaml
    /// </summary>
    public enum SuccessResult
    {
        Open,
        Done,
    }

    public partial class NewProductGroupSuccessfulDialog : Window
    {
        public SuccessResult SuccessResult { get; set; }

        public NewProductGroupSuccessfulDialog(Window owner, ProductGroupItem productGroup)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            DialogContext.DataContext = productGroup;

            OpenButton.Click += OpenButton_Click;
            DoneButton.Click += DoneButton_Click;
        }

        void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            SuccessResult = SuccessResult.Done;
        }

        void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            SuccessResult = SuccessResult.Open;
        }
    }
}
