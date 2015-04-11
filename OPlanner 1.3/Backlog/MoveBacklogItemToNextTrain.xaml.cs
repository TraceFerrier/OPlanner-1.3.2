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
    /// Interaction logic for MoveBacklogItemToNextTrain.xaml
    /// </summary>
    public partial class MoveBacklogItemToNextTrain : Window
    {
        public bool IsMoveConfirmed { get; set; }
        public string NewBacklogItemName { get; set; }
        public MoveBacklogItemToNextTrain(string newTitle, string nextTrainName)
        {
            InitializeComponent();
            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            NewBacklogItemName = newTitle;
            NewBacklogItemNameBox.Text = NewBacklogItemName;
            NextTrainNameBox.Text = nextTrainName;

            OKButton.Click += OKButton_Click;
            CancelButton.Click += CancelButton_Click;

            UpdateUI();
            NewBacklogItemNameBox.TextChanged += NewBacklogItemNameBox_TextChanged;
        }

        void NewBacklogItemNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NewBacklogItemName = NewBacklogItemNameBox.Text;
            UpdateUI();
        }

        void UpdateUI()
        {
            if (string.IsNullOrWhiteSpace(NewBacklogItemName) || NewBacklogItemName.Length < 2)
            {
                OKButton.IsEnabled = false;
            }
            else
            {
                OKButton.IsEnabled = true;
            }
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsMoveConfirmed = false;
            Close();
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            NewBacklogItemName = NewBacklogItemNameBox.Text;
            IsMoveConfirmed = true;
            Close();
        }
    }
}
