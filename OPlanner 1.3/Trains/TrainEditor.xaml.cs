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
    /// Interaction logic for TrainEditor.xaml
    /// </summary>
    public partial class TrainEditor : Window
    {
        SelectorState<TrainItem> TrainState;

        public TrainEditor()
        {
            InitializeComponent();
            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            TrainState = new SelectorState<TrainItem>(TrainEditorGrid, ProductPreferences.LastSelectedTrainEditorTrain);

            OkButton.Click += OkButton_Click;
            NewButton.Click += NewButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TrainState.CurrentItem != null)
            {
                foreach (BacklogItem item in BacklogItem.Items)
                {
                    if (item.ParentTrainItem == TrainState.CurrentItem)
                    {
                        UserMessage.Show("There are one or more backlog items currently assigned to this train - a train with backlog items assigned to it cannot be deleted.");
                        return;
                    }
                }

                TrainState.CurrentItem.RequestDeleteItem();
            }
        }

        void NewButton_Click(object sender, RoutedEventArgs e)
        {
            TrainItem.CreateTrainItem();
        }
    }
}
