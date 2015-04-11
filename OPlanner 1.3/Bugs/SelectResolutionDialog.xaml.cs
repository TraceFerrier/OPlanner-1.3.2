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
    /// Interaction logic for SelectResolutionDialog.xaml
    /// </summary>
    public partial class SelectResolutionDialog : Window
    {
        public string ResolutionValue { get; set; }
        public string ResolutionComments { get; set; }
        public SelectResolutionDialog(Window parentWindow, string originalComments)
        {
            InitializeComponent();
            this.Owner = parentWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            ResolutionComboBox.ItemsSource = ResolutionValues.Values;
            CommentsBox.Text = originalComments;
            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;
            UpdateUI();
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ResolutionValue = null;
            Close();
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ResolutionValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            ResolutionValue = combo.SelectedValue as string;
            UpdateUI();
        }

        void UpdateUI()
        {
            OkButton.IsEnabled = string.IsNullOrWhiteSpace(ResolutionValue) ? false : true;
        }

        private void CommentsBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResolutionComments = CommentsBox.Text;
        }
    }
}
