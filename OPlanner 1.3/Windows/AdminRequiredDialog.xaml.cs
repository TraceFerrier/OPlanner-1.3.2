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
    /// Interaction logic for AdminRequiredDialog.xaml
    /// </summary>
    public partial class AdminRequiredDialog : Window
    {
        public AdminRequiredDialog(string featureRequiringAdmin)
        {
            InitializeComponent();
            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            this.Title = featureRequiringAdmin;

            ProductGroupAdminListBox.ItemsSource = Globals.ApplicationManager.AdminAliases;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ProductGroupAdminListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
