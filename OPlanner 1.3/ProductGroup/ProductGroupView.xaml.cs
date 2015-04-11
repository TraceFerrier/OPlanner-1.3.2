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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PlannerNameSpace.Views
{
    /// <summary>
    /// Interaction logic for ProductGroup.xaml
    /// </summary>
    public partial class ProductGroupView : UserControl
    {
        ProductGroupState ViewState;
        ProductGroupMembersItemCollection MemberItemCollection;

        public ProductGroupView()
        {
            InitializeComponent();

            Globals.EventManager.ProductGroupMembersUpdateComplete += PopulateProductGroupMemberList;
        }

        public void InitializeData()
        {

            PillarGrid.ItemsSource = PillarItem.Items;
            Utils.AddSortCriteria(PillarGrid, "Title");

            ViewState = new ProductGroupState(MemberPillarCombo, DisciplineCombo);
            MemberItemCollection = new ProductGroupMembersItemCollection(ViewState);
            MemberItemCollection.SetItemsControl(PeopleGrid);
            Utils.SetSortCriteria(PeopleGrid, "DisplayName");

            Context.DataContext = ProductGroupManager.Instance.CurrentProductGroup;

            PillarGrid.SelectionChanged += PillarGrid_SelectionChanged;
            Globals.MainWindow.EditProductGroupsButton.Click += EditProductGroupsButton_Click;
            Globals.MainWindow.NewPillarButton.Click += NewPillarButton_Click;
            Globals.MainWindow.EditPillarButton.Click += EditPillarButton_Click;
            Globals.MainWindow.DeletePillarButton.Click += DeletePillarButton_Click;
            Globals.MainWindow.TrainEditButton.Click += TrainEditButton_Click;
            Globals.MainWindow.PersonaEditButton.Click += PersonaEditButton_Click;
            Globals.MainWindow.FileBugButton.Click += FileBugButton_Click;
            Globals.MainWindow.EditBugsButton.Click += EditBugsButton_Click;

        }

        void EditBugsButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.ApplicationManager.ShowOPlannerBugsWindow();
        }

        void FileBugButton_Click(object sender, RoutedEventArgs e)
        {
            OPlannerBugDialog dialog = new OPlannerBugDialog(Globals.MainWindow);
            dialog.ShowDialog();
        }

        void EditProductGroupsButton_Click(object sender, RoutedEventArgs e)
        {
            SelectProductGroupDialog dialog = new SelectProductGroupDialog();
            dialog.ShowDialog();

            if (dialog.WelcomeState == WelcomeState.Open || dialog.WelcomeState == WelcomeState.Restart)
            {
                Globals.ApplicationManager.OpenProductGroup(dialog.SelectedProductGroupItem);
            }
        }

        void Instance_ProductGroupMembersUpdateComplete()
        {
            throw new NotImplementedException();
        }

        void DeletePillarButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewState.CurrentPillarGridPillar != null)
            {
                ViewState.CurrentPillarGridPillar.RequestDeleteItem();
            }
        }

        void EditPillarButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewState.CurrentPillarGridPillar != null)
            {
                ShowPillarEditDialog(ViewState.CurrentPillarGridPillar);
            }
        }

        void NewPillarButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPillarEditDialog(null);
        }

        void ShowPillarEditDialog(PillarItem pillarItem)
        {
            PillarManager.Instance.ShowPillarEditItemEditor(pillarItem);
        }

        void PillarGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewState.CurrentPillarGridPillar = PillarGrid.SelectedItem as PillarItem;
            Globals.MainWindow.EditPillarButton.IsEnabled = ViewState.CurrentPillarGridPillar != null;
            Globals.MainWindow.DeletePillarButton.IsEnabled = ViewState.CurrentPillarGridPillar != null;

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// User has clicked on the button to bring up the TrainEdit dialog.
        /// </summary>
        //------------------------------------------------------------------------------------
        void TrainEditButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.ApplicationManager.EditTrains();
        }

        public void PopulateProductGroupMemberList()
        {
            //GroupMemberFilter = new ItemsControlFilter<GroupMemberItem>(PeopleGrid, ItemTypeID.GroupMember);
            //GroupMemberFilter.AddFilter(DisciplineCombo, ProductPreferences.LastSelectedProductGroupDisciplineValue, ItemTypeID.FilterDiscipline);
            //GroupMemberFilter.UpdateFilter();

            //Utils.SetSortCriteria(PeopleGrid, "DisplayName");
        }

        void PersonaEditButton_Click(object sender, RoutedEventArgs e)
        {
            PersonaEditor dialog = new PersonaEditor();
            dialog.ShowDialog();
        }

        private void PeopleGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            DataGridColumn column = e.Column;

            if ((string)column.Header == PlannerContent.ColumnHeaderPrimaryPillar)
            {
                Utils.SetCustomSortingForColumn(PeopleGrid, new ItemPropertySort<GroupMemberItem>(Utils.GetPropertyName((GroupMemberItem s) => s.Title), System.ComponentModel.ListSortDirection.Ascending), e);
            }
        }

    }
}
