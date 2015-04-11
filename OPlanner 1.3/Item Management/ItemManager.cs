using ProductStudio;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace PlannerNameSpace
{
    [Serializable]
    class LocalProperty
    {
        string PublicPropName{get;set;}
        object value{get;set;}
    }

    public partial class ItemManager : DispatcherObject
    {
        private Dictionary<string, StoreItem> m_items;
        private StoreItemCollection<StoreItem> m_changedItems;
        private List<DeferredStoreItem> m_deferredStoreItems { get; set; }

        public bool AreDeferredItemsAvailable { get { return m_deferredStoreItems != null && m_deferredStoreItems.Count > 0; } }
        public bool IsDiscoveryComplete { get; set; }
        public int DeferredItemCount { get; set; }
        public int DeferredItemCurrent { get; set; }
        public bool IsCommitInProgress { get; set; }

        public ItemManager()
        {
            m_items = new Dictionary<string, StoreItem>();
            m_changedItems = new StoreItemCollection<StoreItem>();
            m_deferredStoreItems = new List<DeferredStoreItem>();
            InitializeCaches();

            StoreQueryManager.Instance.PlannerQueryCompleted += Handle_PlannerQueryCompleted;
            Globals.ApplicationManager.ApplicationStartupComplete += Handle_ApplicationStartupComplete;
        }

        // Events fired by the ItemManager
        public event EventHandler<StoreItemChangedEventArgs> StoreItemChanged;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// List of all items that currently have unsaved changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<StoreItem> ChangeList
        {
            get
            {
                return m_changedItems;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Count of items that have unsaved changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public int ChangeCount { get { return m_changedItems.Count; } }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called when any property of an item changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SaveProperty(ItemProperty itemProperty)
        {
            StoreItem item = itemProperty.Item;
            ChangeType changeType = ChangeType.Updated;

            // Items in these persist states by definition have no changes to be saved.
            if (item.PersistState == PersistStates.Dummy || item.PersistState == PersistStates.NewUncommitted)
            {
                return;
            }

            // Maintain the change list
            UpdateChangeList(item);

            // If the Resolution property has changed, the item is moving to or from the deleted state
            if (itemProperty.PublicPropName == Utils.GetPropertyName((StoreItem p) => p.Resolution))
            {
                // Item is being deleted
                if (item.Resolution == ResolutionType.WontFix)
                {
                    // If the item isn't dirty, then the resolution has been completed, and the property
                    // change above is the backing value catching up to the current value.
                    if (itemProperty.IsValueChanged())
                    {
                        if (RemoveFromCache(item))
                        {
                            changeType = ChangeType.Removed;
                            item.ChangeAction = StoreChangeAction.ResolveAndCloseItem;

                            // If the item has never been saved, just remove from the change list
                            if (item.IsNew)
                            {
                                m_changedItems.Remove(item);
                            }
                        }
                    }
                }

                // If item was undone back to active, bring it back into the cache.
                else if (string.IsNullOrWhiteSpace(item.Resolution))
                {
                    if (AddToCache(item))
                    {
                        changeType = ChangeType.Added;
                        item.ChangeAction = StoreChangeAction.Default;
                    }
                }
            }

            if (changeType == ChangeType.Updated)
            {
                item.OnUpdate(itemProperty);
                SendItemChangedEvent(item, ChangeType.Updated, itemProperty);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Accepts a new item that has never been saved to the store, and prepares it
        /// to be saved on the next Commit operation.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void NewItemAccepted(StoreItem item)
        {
            System.Diagnostics.Debug.Assert(item.IsNew);
            item.PersistState = PersistStates.NewCommitted;
            AddToCache(item);
            m_changedItems.Add(item);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// An attachment to the given item has changed.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SaveAttachment(StoreItem item)
        {
            UpdateChangeList(item);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Updates the change list based upon the current state of the given item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void UpdateChangeList(StoreItem item)
        {
            // Changes for items in immediate save mode are being batched for a direct save
            // operation, and don't go on the changelist.
            if (item.IsInImmediateSave)
            {
                return;
            }

            bool isDirty = item.IsDirty;
            if (!m_changedItems.Contains(item))
            {
                if (isDirty)
                {
                    m_changedItems.Add(item);
                }
            }
            else
            {
                if (!isDirty)
                {
                    m_changedItems.Remove(item);
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Called when an item has been loaded via query from the backing store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void AddItemFromStore(StoreItem item)
        {
            AddToCache(item);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Adds the given item to the cache, and sends a StoreItemAdded event to all
        /// listeners.
        /// </summary>
        //------------------------------------------------------------------------------------
        private bool AddToCache(StoreItem item)
        {
            string storeKey = item.StoreKey;

            System.Diagnostics.Debug.Assert(!m_items.ContainsKey(storeKey), "Item Cache Add: the given item is already present");

            //if (!m_items.ContainsKey(storeKey))
            {
                m_items.Add(item.StoreKey, item);
                AddToItemTypeCache(item);
                item.OnAdd();
                SendItemChangedEvent(item, ChangeType.Added, null);
                return true;
            }

            //return false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Removes the given item from the cache, and sends a StoreItemRemoved event to all
        /// listeners.
        /// </summary>
        //------------------------------------------------------------------------------------
        private bool RemoveFromCache(StoreItem item)
        {
            string storeKey = item.StoreKey;

            System.Diagnostics.Debug.Assert(m_items.ContainsKey(storeKey), "Item Cache Remove: the given item is not present");
            Globals.ApplicationManager.WriteToEventLog("RemoveFromCache call recived on the UI thread: " + item.StoreKey);

            if (m_items.ContainsKey(storeKey))
            {
                Globals.ApplicationManager.WriteToEventLog("Item removed: " + item.ID + ": " + item.Title);
                m_items.Remove(storeKey);
                RemoveFromItemTypeCache(item);
                item.OnRemove();
                SendItemChangedEvent(item, ChangeType.Removed, null);
                return true;
            }

            return false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Deletes the item from memory, and prepares it for deletion from the backing
        ///  store on the next Commit operation.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void DeleteItem(StoreItem item)
        {
            item.Resolution = ResolutionType.WontFix;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Resolves and closes the given item, with a resolution of Fixed.  The change will
        ///  be saved to the backing store on the next commit operation.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void FixItem(StoreItem item)
        {
            item.OnResolution();
            item.Resolution = ResolutionType.Fixed;
            item.Status = StatusValues.Closed;
            item.SubStatus = null;
            item.ChangeAction = StoreChangeAction.ResolveAndCloseItem;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Resolves the given item, with the given resolution.  The change will be saved to
        ///  the backing store on the next commit operation.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ResolveItem(StoreItem item, string resolution)
        {
            item.OnResolution();
            item.Resolution = resolution;
            item.Status = StatusValues.Resolved;
            item.ChangeAction = StoreChangeAction.ResolveItem;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Closes the given item.  The change will be saved to the backing store on the next 
        ///  commit operation.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void CloseItem(StoreItem item)
        {
            item.ChangeAction = StoreChangeAction.CloseItem;
            item.Status = StatusValues.Closed;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Activates the given item.  The change will be saved to the backing store on the 
        ///  next commit operation.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ActivateItem(StoreItem item)
        {
            item.Status = StatusValues.Active;
            item.Resolution = null;
            item.ChangeAction = StoreChangeAction.ActivateItem;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Returns a copy of all the items in the cache.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<StoreItem> GetItems()
        {
            AsyncObservableCollection<StoreItem> items = new AsyncObservableCollection<StoreItem>();
            foreach (KeyValuePair<string, StoreItem> kvp in m_items)
            {
                items.Add(kvp.Value);
            }
            
            return items;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Commits any changes in the given item to the backing store immediately, without 
        /// committing any other items that are on the current change list.
        /// 
        /// To use this method, you must first call Item.BeginSaveImmediate before making any
        /// changes to the item you want to save.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SaveItemImmediate(StoreItem item)
        {
            if (!item.IsDirty || !item.IsInImmediateSave)
            {
                return;
            }

            item.IsInImmediateSave = false;
            if (item.IsNew)
            {
                item.PersistState = PersistStates.NewCommitted;
                AddToCache(item);
            }

            CommitChangesWorker commitWorker = new CommitChangesWorker();
            commitWorker.CommitItem(item);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Starts a background task to attempt to commit all currently queued changes to the
        /// back-end store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void CommitChanges(bool showProgress = true)
        {
            if (m_changedItems.Count > 0)
            {
                if (ChangeListWindow != null)
                {
                    ChangeListWindow.Close();
                    ChangeListWindow = null;
                }

                IsCommitInProgress = true;
                CommitChangesWorker commitWorker = new CommitChangesWorker();

                List<StoreItem> commitItems = m_changedItems.ToList();
                commitWorker.CommitItems(commitItems, showProgress);
            }
        }

        private bool UndoInProgress = false;
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Undoes all changes that are currently queued up to be saved.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void UndoChanges()
        {
            AsyncObservableCollection<StoreItem> changedItems = m_changedItems.ToCollection();
            changedItems.Sort((x, y) => StoreItemTypeSort.Compare(x, y));

            UndoInProgress = true;
            foreach (StoreItem item in changedItems)
            {
                if (item.IsNew)
                {
                    m_changedItems.Remove(item);
                    RemoveFromCache(item);
                }
                else
                {
                    item.UndoChanges();
                }
            }

            UndoInProgress = false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sends a StoreItemChanged event out to all listeners.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void SendItemChangedEvent(StoreItem item, ChangeType changeType, ItemProperty itemProperty)
        {
            if (Globals.ApplicationManager.IsStartupComplete)
            {
                if (StoreItemChanged != null)
                {
                    StoreItemChange change = null;
                    ChangeSource source = UndoInProgress ? ChangeSource.Undo : ChangeSource.Default;
                    switch (changeType)
                    {
                        case ChangeType.Added:
                            change = new StoreItemChange() { Item = item, ChangeType = changeType, PublicPropName = null, ChangeSource = source };
                            break;
                        case ChangeType.Removed:
                            change = new StoreItemChange() { Item = item, ChangeType = ChangeType.Removed, PublicPropName = null, ChangeSource = source };
                            break;
                        case ChangeType.Updated:
                            change = new StoreItemChange() { Item = item, ChangeType = ChangeType.Updated, PublicPropName = itemProperty.PublicPropName, OldValue = itemProperty.PreviousValue, NewValue = itemProperty.CurrentValue, ChangeSource = source };
                            break;
                    }

                    if (change != null)
                    {
                        StoreItemChanged(this, new StoreItemChangedEventArgs(change));
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Once a new item gets persisted, the temporary StoreKey is replaced with the actual
        /// backing store ID, so remove the item with the temporary key from the cache, and
        /// put it back in using the ID as the key.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void OnStoreItemIDChange(StoreItem item, int oldID, int newID)
        {
            if (Globals.ApplicationManager.IsStartupComplete)
            {
                string uncommittedKey = item.UncommittedStoreKey;
                if (m_items.ContainsKey(uncommittedKey))
                {
                    m_items.Remove(uncommittedKey);
                }

                string key = item.StoreKey;
                if (!m_items.ContainsKey(key))
                {
                    m_items.Add(key, item);
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the item in this collection with the given storeID and item ID.
        /// </summary>
        //------------------------------------------------------------------------------------
        public ItemType GetHostItemByID<ItemType>(int ID) where ItemType : StoreItem
        {
            return GetItem<ItemType>(StoreItem.GetHostItemKey(ID));
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the item in this collection with the given storeID and item ID.
        /// </summary>
        //------------------------------------------------------------------------------------
        public StoreItem GetItem(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            if (m_items.ContainsKey(key))
            {
                return m_items[key];
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the item in this collection with the given storeID and item ID.
        /// </summary>
        //------------------------------------------------------------------------------------
        public ItemType GetItem<ItemType>(string key) where ItemType : StoreItem
        {
            return GetItem(key) as ItemType;
        }

        #region ItemDiscovery

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called any time there's a set of DSItems to be deferred during a normal
        /// planning query.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ReceiveStoreItemsToDefer(Datastore store, DatastoreItems items)
        {
            foreach (DatastoreItem dsItem in items)
            {
                DeferStoreItem(store, dsItem, IsRefresh.No);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called any time there's a set of DSItems to be deferred during a Refresh
        /// query. In this case, we're only interested in items that have changed since the 
        /// last refresh.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ReceiveRefreshStoreItemsToDefer(Datastore store, DatastoreItems items)
        {
            DateTime lastRefreshTime = StoreQueryManager.Instance.LastRefreshTime;
            foreach (DatastoreItem dsItem in items)
            {
                DateTime changedDateTime = Utils.GetValueAsLocalTime(store.GetBackingValue(dsItem, Datastore.PropNameChangedDate));
                if (changedDateTime >= lastRefreshTime)
                {
                    DeferStoreItem(store, dsItem, IsRefresh.Yes);
                }
            }
        }

        private void DeferStoreItem(Datastore store, DatastoreItem dsItem, IsRefresh isRefreshedItem)
        {
            DeferredStoreItem deferredItem = new DeferredStoreItem(store, dsItem, isRefreshedItem);
            m_deferredStoreItems.Add(deferredItem);
            DeferredItemCount++;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// We'll start initial item discovery only after startup is fully complete.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Handle_ApplicationStartupComplete(object sender, EventArgs e)
        {
            DiscoverDeferredItems();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Handle the event fired when a PlannerQuery is completed, to handle discovery after
        /// a refresh.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Handle_PlannerQueryCompleted(object sender, PlannerQueryCompletedEvent e)
        {
            if (e.Result.ResultType == ResultType.Completed)
            {
                DiscoverDeferredItems();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Discover any items that were deferred as a result of the last planner query,
        /// and instantiate and cache those items in the background now.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void DiscoverDeferredItems()
        {
            if (AreDeferredItemsAvailable)
            {
                BackgroundWorker DeferredItemIndexWorker = new BackgroundWorker();
                DeferredItemIndexWorker.DoWork += BackgroundDiscoveryWorker;
                DeferredItemIndexWorker.RunWorkerCompleted += DiscoverDeferredItems_WorkCompleted;
                DeferredItemIndexWorker.RunWorkerAsync();
            }
            else
            {
                IsDiscoveryComplete = true;
                Globals.EventManager.OnDiscoveryComplete(this);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Builds the primary items cache in the background.
        /// </summary>
        //------------------------------------------------------------------------------------
        void BackgroundDiscoveryWorker(object sender, DoWorkEventArgs e)
        {
            DeferredStoreItem deferredItem = m_deferredStoreItems[0];
            if (deferredItem.IsRefreshedItem == IsRefresh.No)
            {
                UndeferStoreItems();
            }
            else
            {
                UndeferRefreshStoreItems();
            }

            m_deferredStoreItems.Clear();
            DeferredItemCount = 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Pulls each DatastoreItem that was placed on the deferred list during querying,
        /// creates a new StoreItem from it, and adds the item to the cache.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void UndeferStoreItems()
        {
            if (m_deferredStoreItems != null)
            {
                foreach (DeferredStoreItem deferredItem in m_deferredStoreItems)
                {
                    StoreItem storeItem = deferredItem.ItemStore.CreateAndInitializeItemFromDS(deferredItem.DSItem);
                    BackgroundAddToCache(storeItem);
                    DeferredItemCurrent++;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  If any changes were detected during the last refresh, update all the in-memory
        ///  store items with the changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void UndeferRefreshStoreItems()
        {
            if (m_deferredStoreItems == null)
            {
                return;
            }

            Globals.ApplicationManager.ClearEventLog();

            // First pass
            foreach (DeferredStoreItem deferredItem in m_deferredStoreItems)
            {
                string storeKey = deferredItem.StoreKey;
                Globals.ApplicationManager.WriteToEventLog("Item to refresh found: " + storeKey);

                // If the item is in our cache, check whether refresh indicates the item has
                // been deleted since the last refresh.
                if (m_items.ContainsKey(storeKey))
                {
                    StoreItem existingItem = m_items[storeKey];
                    existingItem.DSItem = deferredItem.DSItem;
                    if (existingItem.IsDeletedOnRefresh)
                    {
                        BackgroundRemoveFromCache(existingItem);
                    }
                    else
                    {
                        // Not deleted, so update any properties we haven't changed with any
                        // new values from the backing store.
                        List<ItemProperty> itemProperties = existingItem.GetItemProperties();
                        foreach (ItemProperty itemProperty in itemProperties)
                        {
                            if (!itemProperty.IsValueChanged())
                            {
                                BackgroundSyncRefreshedProperty(existingItem, itemProperty);
                            }
                        }
                    }
                }

                // Not in the cache - if active or fixed, the item was added since the last refresh.
                else
                {
                    StoreItem storeItem = deferredItem.ItemStore.CreateAndInitializeItemFromDS(deferredItem.DSItem);
                    if (storeItem.IsActive || storeItem.IsFixed)
                    {
                        BackgroundAddToCache(storeItem);
                    }
                }
            }
        }

        void SyncRefreshedProperty(StoreItem item, ItemProperty itemProperty)
        {
            item.SyncPropertyFromStore(itemProperty);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Cache entry point for use by background operations.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void BackgroundAddToCache(StoreItem item)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => AddToCache(item)));
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Cache entry point for use by background operations.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void BackgroundRemoveFromCache(StoreItem item)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => RemoveFromCache(item)));
        }

        private void BackgroundSyncRefreshedProperty(StoreItem item, ItemProperty itemProperty)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => SyncRefreshedProperty(item, itemProperty)));
        }

        private bool DeferredListContains(List<DeferredStoreItem> deferredStoreItems, StoreItem item)
        {
            foreach (DeferredStoreItem deferredItem in deferredStoreItems)
            {
                if (deferredItem.StoreKey == item.StoreKey)
                {
                    return true;
                }
            }

            return false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  A discovery of deferred items has occurred for a standard or refresh query.
        /// </summary>
        //------------------------------------------------------------------------------------
        void DiscoverDeferredItems_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!IsDiscoveryComplete)
            {
                IsDiscoveryComplete = true;
                Globals.EventManager.OnDiscoveryComplete(this);
            }
        }

        #endregion


        static ChangeListWindow ChangeListWindow = null;

        public void ShowChangeListWindow()
        {
            if (ChangeListWindow != null)
            {
                if (!ChangeListWindow.IsVisible)
                {
                    ChangeListWindow.Show();
                }
                ChangeListWindow.BringIntoView();
            }
            else
            {
                ChangeListWindow = new ChangeListWindow();
                ChangeListWindow.Closed += ChangeListWindow_Closed;
                ChangeListWindow.Show();
            }
        }

        void ChangeListWindow_Closed(object sender, EventArgs e)
        {
            ChangeListWindow = null;
        }

    }
}
