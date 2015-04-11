using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace PlannerNameSpace
{
    public abstract class ViewItemCollection<T> where T : StoreItem
    {
        public event ViewItemCollectionCountChangedEventHandler CollectionCountChanged;

        private Selector CollectionItemsControl;
        private object SyncCollectionCountChangedEvent;
        private IList<T> m_sourceItems;
        private AsyncObservableCollection<T> m_items;

        public ViewItemCollection()
        {
            SyncCollectionCountChangedEvent = new object();
            m_items = new AsyncObservableCollection<T>();

            if (typeof(T) == typeof(BacklogItem))
            {
                m_sourceItems = (IList<T>)BacklogItem.Items;
                BacklogItem.Items.CollectionChanged += SourceItems_CollectionChanged;
            }
            else if (typeof(T) == typeof(GroupMemberItem))
            {
                m_sourceItems = (IList<T>)GroupMemberItem.Items;
                GroupMemberItem.Items.CollectionChanged += SourceItems_CollectionChanged;
            }

            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called whenever items in the backlog view need to be filtered.  Makes use of the
        /// various backlog filter combo settings to determine whether the given item should
        /// be excluded from the view.
        /// </summary>
        //------------------------------------------------------------------------------------
        public abstract bool ItemFilter(object viewObject);

        public int Count
        {
            get
            {
                return m_items.Count;
            }
        }

        public AsyncObservableCollection<T> Items
        {
            get
            {
                return m_items;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Binds your itemsControl to the item collection maintained by this ViewItemCollection.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetItemsControl(Selector itemsControl)
        {
            CollectionItemsControl = itemsControl;
            CollectionItemsControl.ItemsSource = m_items;

            RefreshCollection();
        }

        public void SelectItem(object item)
        {
            T viewItem = item as T;
            if (viewItem != null && m_items.Contains(viewItem))
            {
                // If the items control for this collection happens to be a DataGrid,
                // scroll the selected item into view.
                DataGrid dataGridControl = CollectionItemsControl as DataGrid;
                if (dataGridControl != null)
                {
                    dataGridControl.Focus();
                    dataGridControl.ScrollIntoView(viewItem);
                    dataGridControl.SelectedItem = viewItem;
                }
                else
                {
                    CollectionItemsControl.SelectedItem = viewItem;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Selects the given item in the view, if the view contains that item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ClickSelectItem(object item)
        {
            T viewItem = item as T;
            if (viewItem != null && m_items.Contains(viewItem))
            {
                // If the items control for this collection happens to be a DataGrid,
                // scroll the selected item into view.
                DataGrid dataGridControl = CollectionItemsControl as DataGrid;
                if (dataGridControl != null)
                {
                    dataGridControl.Focus();
                    dataGridControl.ScrollIntoView(viewItem);
                    DataGridRow row = (DataGridRow)dataGridControl.ItemContainerGenerator.ContainerFromItem(viewItem);
                    Point screenPoint = row.PointToScreen(new Point(25, 10));
                    int left = (int) Globals.MainWindow.Left;
                    if (screenPoint.X <= left)
                    {
                        screenPoint.X = left + 25;
                    }

                    MouseUtils.DoMouseClick(screenPoint);
                }
                else
                {
                    CollectionItemsControl.SelectedItem = viewItem;
                }
            }
        }

        void SourceItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int oldCount = m_items.Count;

            if (e.NewItems != null)
            {
                foreach (T item in e.NewItems)
                {
                    if (ItemFilter(item))
                    {
                        m_items.Add(item);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (T item in e.OldItems)
                {
                    m_items.Remove(item);
                }
            }

            OnCollectionCountChanged(oldCount);
        }

        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            StoreItemChange change = e.Change;
            if (change.ChangeType == ChangeType.Updated)
            {
                int oldCount = m_items.Count;

                object item = change.Item;
                if (typeof(T) == item.GetType())
                {
                    if (ItemFilter(item))
                    {
                        if (!m_items.Contains((T)item))
                        {
                            m_items.Add((T)item);
                        }
                    }
                    else
                    {
                        if (m_items.Contains((T)item))
                        {
                            m_items.Remove((T)item);
                        }
                    }
                }

                OnCollectionCountChanged(oldCount);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Refilters all the items in this collection's source, retaining only those that
        /// meet the current filter criteria.  Derived classes should call this each time
        /// their filter criteria changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void RefreshCollection()
        {
            int oldCount = m_items.Count;
            m_items.Clear();

            int sourceCount = m_sourceItems.Count;
            if (sourceCount > 0)
            {
                for (int index = 0; index < sourceCount; index++)
                {
                    T item = m_sourceItems[index];
                    if (ItemFilter(item))
                    {
                        m_items.Add((T)item);
                    }
                }
            }

            OnCollectionCountChanged(oldCount);
        }

        void OnCollectionCountChanged(int oldCount)
        {
            lock (SyncCollectionCountChangedEvent)
            {
                if (CollectionCountChanged != null)
                {
                    int newCount = m_items.Count;
                    if (newCount != oldCount)
                    {
                        CollectionCountChangedEventArgs args = new CollectionCountChangedEventArgs();
                        args.OldCount = oldCount;
                        args.NewCount = newCount;

                        // Invoke registered event handlers on the appropriate thread
                        Delegate[] delegates = CollectionCountChanged.GetInvocationList();
                        foreach (ViewItemCollectionCountChangedEventHandler handler in delegates)
                        {
                            DispatcherObject dispatcherObject = handler.Target as DispatcherObject;
                            if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                            {
                                dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, args);
                            }
                            else
                            {
                                handler(this, args);
                            }
                        }
                    }
                }
            }
        }


    }
}
