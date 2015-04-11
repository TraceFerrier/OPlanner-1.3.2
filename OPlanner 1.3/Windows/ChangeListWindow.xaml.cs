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
    /// Interaction logic for ChangeListWindow.xaml
    /// </summary>
    public partial class ChangeListWindow : Window
    {
        public ChangeListWindow()
        {
            InitializeComponent();

            Utils.FitWindowToScreen(this);
            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            ChangeListItemsControl.ItemsSource = Globals.ItemManager.ChangeList;
            Globals.ItemManager.ChangeList.CollectionChanged += ChangeList_CollectionChanged;
            UpdateChangeUI();
        }

        void ChangeList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateChangeUI();
        }

        void UpdateChangeUI()
        {
            ChangeCountBox.Text = "Changes to save: " + Globals.ItemManager.ChangeList.Count;

            if (Globals.ItemManager.ChangeList.Count > 0)
            {
                TitlePanel.Background = Brushes.Red;
            }
            else
            {
                TitlePanel.Background = Brushes.Green;
            }
        }
    }
}
