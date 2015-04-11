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
    /// Interaction logic for EditHyperlink.xaml
    /// </summary>
    public partial class EditHyperlink : Window
    {
        public bool IsCancelled { get; set; }
        public string HyperlinkText
        {
            get { return HyperlinkTextBox.Text; }
            set { HyperlinkTextBox.Text = value; }
        }

        public string HyperlinkAddress
        {
            get { return HyperlinkAddressBox.Text; }
            set { HyperlinkAddressBox.Text = value; }
        }

        public EditHyperlink(Window ownerWindow, string hyperlinkText)
        {
            InitializeComponent();

            Globals.ApplicationManager.SetStartupLocation(this);


            HyperlinkText = hyperlinkText;
            if (!string.IsNullOrWhiteSpace(hyperlinkText))
            {
                HyperlinkTextBox.IsEnabled = false;
            }

            HyperlinkTextBox.TextChanged += HyperlinkTextBox_TextChanged;
            HyperlinkAddressBox.TextChanged += HyperlinkAddressBox_TextChanged;
            OKButton.IsEnabled = false;
        }

        void HyperlinkAddressBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OKButton.IsEnabled = HyperlinkAddressBox.Text.Length > 0 && HyperlinkTextBox.Text.Length > 0;
        }

        void HyperlinkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OKButton.IsEnabled = HyperlinkAddressBox.Text.Length > 0 && HyperlinkTextBox.Text.Length > 0;
        }

        private void OKButton_Clicked(object sender, RoutedEventArgs e)
        {
            IsCancelled = false;
            Close();
        }

        private void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            IsCancelled = true;
            Close();
        }
    }
}
