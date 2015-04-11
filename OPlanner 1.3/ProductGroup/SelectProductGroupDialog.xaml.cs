using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PlannerNameSpace
{
    public enum ProductGroupStages
    {
        NotStarted,
        ShowCreateProductGroupDialog,
        ShowEditProductGroupDialog,
        EditProductGroupDialogActive,
        CreateProductGroupDialogActive,
        GroupMemberDiscoveryCompleted,
        CreatingProductGroupItem,
        CreatingProductGroupItemCompleted,
        CreateDiscoveredGroupMembersCompleted,
        CommittingDiscoveredGroupMembers,
        CommittingDiscoveredGroupMembersCompleted,
        UpdateExistingProductGroupCompleted,
        RebuildingProductGroupMembership,
        ProductGroupMembershipRebuildComplete,
        EditProductGroupCompleted,
        EndOfCycle,
    }

    /// <summary>
    /// Interaction logic for SelectProductGroupDialog.xaml
    /// </summary>
    public partial class SelectProductGroupDialog : Window
    {
        public WelcomeState WelcomeState { get; set; }
        public ProductGroupItem SelectedProductGroupItem { get; set; }
        public ProductGroupItem NewProductGroupItem { get; set; }
        DispatcherTimer UpdateTimer { get; set; }
        ProductGroupStages ProductGroupStage { get; set; }
        bool CreateProductGroupOperationInProgress { get; set; }
        bool ExistingProductGroupAliasesChanged { get; set; }

        public SelectProductGroupDialog()
        {
            InitializeComponent();

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            ExistingProductGroupAliasesChanged = false;
            CreateProductGroupOperationInProgress = false;
            ProductGroupStage = ProductGroupStages.NotStarted;

            EnsureProductGroupGridItems();

            ProductGroupGrid.SelectionChanged += ProductGroupGrid_SelectionChanged;
            Globals.GroupMemberManager.GroupMemberDiscoveryComplete += GroupMemberManager_GroupMemberDiscoveryComplete;
            Globals.GroupMemberManager.RebuildGroupMembershipComplete += GroupMemberManager_RebuildGroupMembershipComplete;
            Globals.GroupMemberManager.CreateDiscoveredGroupMembersComplete += GroupMemberManager_CreateDiscoveredGroupMembersComplete;
            Globals.EventManager.StoreCommitComplete += Handle_StoreCommitComplete;

            OpenButton.Click += OpenButton_Click;
            EditButton.Click += EditButton_Click;
            NewButton.Click += NewButton_Click;
            CancelButton.Click += CancelButton_Click;

            UpdateUI();

            UpdateTimer = new DispatcherTimer();
            UpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Start();
        }

        void EnsureProductGroupGridItems()
        {
            AsyncObservableCollection<ProductGroupItem> allGroups = Globals.ItemManager.ProductGroupItems;
            AsyncObservableCollection<ProductGroupItem> compatibleGroups = new AsyncObservableCollection<ProductGroupItem>();
            foreach (ProductGroupItem productGroup in allGroups)
            {
                if (productGroup.IsCompatibleWithCurrentStore)
                {
                    compatibleGroups.Add(productGroup);
                }
            }

            compatibleGroups.Sort((x, y) => y.LastChangedDate.CompareTo(x.LastChangedDate));
            ProductGroupGrid.ItemsSource = compatibleGroups;
        }

        void Handle_StoreCommitComplete(object sender, StoreCommitCompleteEventArgs e)
        {
            CreateProductGroupOperationInProgress = false;
            if (ProductGroupStage == ProductGroupStages.CreatingProductGroupItem)
            {
                ProductGroupStage = ProductGroupStages.CreatingProductGroupItemCompleted;
            }
            else if (ProductGroupStage == ProductGroupStages.CommittingDiscoveredGroupMembers)
            {
                ProductGroupStage = ProductGroupStages.CommittingDiscoveredGroupMembersCompleted;
            }
            else if (ProductGroupStage == ProductGroupStages.EditProductGroupDialogActive)
            {
                ProductGroupStage = ProductGroupStages.UpdateExistingProductGroupCompleted;
            }
            else if (ProductGroupStage == ProductGroupStages.ProductGroupMembershipRebuildComplete)
            {
                ProductGroupStage = ProductGroupStages.EditProductGroupCompleted;
            }
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTimer.Stop();
            WelcomeState = PlannerNameSpace.WelcomeState.Cancel;
            Close();
        }

        void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SelectedProductGroupItem.IsCompatibleWithCurrentStore)
            {
                UserMessage.Show("The product group you selected is not compatible with the current Store (" + HostItemStore.Instance.StoreName + "). Please pick another group.");
                return;
            }

            UpdateTimer.Stop();
            WelcomeState = PlannerNameSpace.WelcomeState.Open;
            Close();
        }

        void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupStage = ProductGroupStages.ShowCreateProductGroupDialog;
            CreateProductGroupOperationInProgress = false;
        }

        void GroupMemberManager_GroupMemberDiscoveryComplete()
        {
            CreateProductGroupOperationInProgress = false;
            ProductGroupStage = ProductGroupStages.GroupMemberDiscoveryCompleted;
        }

        void GroupMemberManager_CreateDiscoveredGroupMembersComplete()
        {
            CreateProductGroupOperationInProgress = false;
            ProductGroupStage = ProductGroupStages.CreateDiscoveredGroupMembersCompleted;
        }

        void GroupMemberManager_RebuildGroupMembershipComplete()
        {
            CreateProductGroupOperationInProgress = false;
            ProductGroupStage = ProductGroupStages.ProductGroupMembershipRebuildComplete;
        }

        void UpdateTimer_Tick(object sender, System.EventArgs e)
        {
            if (!CreateProductGroupOperationInProgress)
            {
                // Show ProductGroupEditor with a newly created product group
                if (ProductGroupStage == ProductGroupStages.ShowCreateProductGroupDialog)
                {
                    CreateProductGroupOperationInProgress = true; 
                    ProductGroupStage = ProductGroupStages.CreateProductGroupDialogActive;

                    if (NewProductGroupItem == null)
                    {
                        NewProductGroupItem = ScheduleStore.Instance.CreateStoreItem<ProductGroupItem>(ItemTypeID.ProductGroup);
                    }

                    ProductGroupEditor dialog = new ProductGroupEditor(this, NewProductGroupItem);
                    dialog.ShowDialog();

                    if (dialog.WelcomeState == PlannerNameSpace.WelcomeState.Ok)
                    {
                        Globals.GroupMemberManager.DiscoverGroupMembers(NewProductGroupItem);
                    }
                }

                // Show ProductGroupEditor to edit an existing group
                else if (ProductGroupStage == ProductGroupStages.ShowEditProductGroupDialog)
                {
                    CreateProductGroupOperationInProgress = true;
                    ProductGroupStage = ProductGroupStages.EditProductGroupDialogActive;

                    ProductGroupEditor dialog = new ProductGroupEditor(this, SelectedProductGroupItem);
                    dialog.ShowDialog();

                    if (dialog.WelcomeState == PlannerNameSpace.WelcomeState.Ok)
                    {
                        SelectedProductGroupItem.BeginSaveImmediate();
                        ExistingProductGroupAliasesChanged = dialog.AliasesChanged;
                        SelectedProductGroupItem.SaveImmediate();
                    }
                    else
                    {
                        CreateProductGroupOperationInProgress = false;
                    }
                }

                // The discovery of members for a new product group has completed
                else if (ProductGroupStage == ProductGroupStages.GroupMemberDiscoveryCompleted)
                {
                    AsyncObservableCollection<MemberDescriptor> discoveredGroupMembers = Globals.GroupMemberManager.DiscoveredGroupMembers;
                    ConfirmNewProductGroupDialog dialog = new ConfirmNewProductGroupDialog(this, NewProductGroupItem, discoveredGroupMembers);
                    dialog.ShowDialog();

                    if (dialog.DialogConfirmed)
                    {
                        CreateProductGroupOperationInProgress = true;
                        NewProductGroupItem.HostItemStoreName = HostItemStore.Instance.StoreName;
                        NewProductGroupItem.SaveNewItem();
                        ProductGroupStage = ProductGroupStages.CreatingProductGroupItem;
                        Globals.ItemManager.CommitChanges(true);
                    }
                    else
                    {
                        ProductGroupStage = ProductGroupStages.ShowCreateProductGroupDialog;
                        CreateProductGroupOperationInProgress = false;
                    }
                }

                // The commit of the new ProductGroupItem has completed.
                else if (ProductGroupStage == ProductGroupStages.CreatingProductGroupItemCompleted)
                {
                    NewProductGroupItem.ParentProductGroupKey = NewProductGroupItem.StoreKey;
                    EnsureProductGroupGridItems();
                    Globals.GroupMemberManager.CreateDiscoveredGroupMembers(NewProductGroupItem.StoreKey);
                }

                // The creation of previously discovered members for a new product group has completed.
                else if (ProductGroupStage == ProductGroupStages.CreateDiscoveredGroupMembersCompleted)
                {
                    // Now commit all the newly discovered members
                    CreateProductGroupOperationInProgress = true;
                    ProductGroupStage = ProductGroupStages.CommittingDiscoveredGroupMembers;
                    Globals.ItemManager.CommitChanges(true);
                }

                // The creation of a new product group is fully completed
                else if (ProductGroupStage == ProductGroupStages.CommittingDiscoveredGroupMembersCompleted)
                {
                    CreateProductGroupOperationInProgress = true;
                    ProductGroupStage = ProductGroupStages.EndOfCycle;

                    NewProductGroupSuccessfulDialog dialog = new NewProductGroupSuccessfulDialog(this, NewProductGroupItem);
                    dialog.ShowDialog();

                    if (dialog.SuccessResult == SuccessResult.Open)
                    {
                        SelectedProductGroupItem = NewProductGroupItem;
                        WelcomeState = PlannerNameSpace.WelcomeState.Open;
                        Close();
                    }
                }

                else if (ProductGroupStage == ProductGroupStages.UpdateExistingProductGroupCompleted)
                {
                    ProductGroupStage = ProductGroupStages.RebuildingProductGroupMembership;
                    if (ExistingProductGroupAliasesChanged)
                    {
                        CreateProductGroupOperationInProgress = true;
                        Globals.GroupMemberManager.RebuildProductGroupMembership(SelectedProductGroupItem);
                    }
                    else
                    {
                        ProductGroupStage = ProductGroupStages.EditProductGroupCompleted;
                    }
                }

                else if (ProductGroupStage == ProductGroupStages.ProductGroupMembershipRebuildComplete)
                {
                    CreateProductGroupOperationInProgress = true;
                    Globals.ItemManager.CommitChanges(true);

                }

                else if (ProductGroupStage == ProductGroupStages.EditProductGroupCompleted)
                {
                    ProductGroupStage = ProductGroupStages.EndOfCycle;
                    if (ExistingProductGroupAliasesChanged && SelectedProductGroupItem == ProductGroupManager.Instance.CurrentProductGroup && Globals.ApplicationManager.IsStartupComplete)
                    {
                        WelcomeState = PlannerNameSpace.WelcomeState.Restart;
                        Close();
                    }
                }
            }
        }

        void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupStage = ProductGroupStages.ShowEditProductGroupDialog;
        }

        void ProductGroupGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedProductGroupItem = ProductGroupGrid.SelectedItem as ProductGroupItem;
            UpdateUI();
        }

        void UpdateUI()
        {
            if (SelectedProductGroupItem != null && SelectedProductGroupItem != ProductGroupManager.Instance.CurrentProductGroup)
            {
                OpenButton.IsEnabled = true;
            }
            else
            {
                OpenButton.IsEnabled = false;
            }

            EditButton.IsEnabled = SelectedProductGroupItem == null ? false : true;
        }
    }
}
