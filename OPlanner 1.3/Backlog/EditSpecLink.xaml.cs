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
    /// Interaction logic for EditSpecLink.xaml
    /// </summary>
    public partial class EditSpecLink : Window
    {
        string originalLink { get; set; }
        BacklogItem TargetBacklogItem { get; set; }
        public EditSpecLink(BacklogItem backlogItem)
        {
            InitializeComponent();
            TargetBacklogItem = backlogItem;
            DialogContext.DataContext = TargetBacklogItem;
            originalLink = backlogItem.SpecLink;
            UrlTextBox.Focus();
        }

        private void OKButton_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            TargetBacklogItem.SpecLink = originalLink;
            Close();
        }
    }
}
