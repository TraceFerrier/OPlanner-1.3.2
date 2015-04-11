using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class ItemsByParent<T> where T : StoreItem, new()
    {
        private Dictionary<string, AsyncObservableCollection<T>> m_itemsByParent;

        public ItemsByParent()
        {
            m_itemsByParent = new Dictionary<string, AsyncObservableCollection<T>>();
        }

        public AsyncObservableCollection<T> GetItemsByParent(StoreItem parentItem)
        {
            if (parentItem == null)
            {
                return new AsyncObservableCollection<T>();
            }

            string parentKey = parentItem.StoreKey;
            EnsureItemsByParent(parentKey);

            return m_itemsByParent[parentKey];
        }

        public virtual void AddItemByParent(T item, StoreItem parentItem)
        {
            if (parentItem != null)
            {
                AddItemByParent(item, parentItem.StoreKey);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Adds the given work item to the cache under the given backlog item parent key.
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual void AddItemByParent(T item, string parentKey)
        {
            lock (m_itemsByParent)
            {
                if (!string.IsNullOrWhiteSpace(parentKey))
                {
                    EnsureItemsByParent(parentKey);

                    if (!m_itemsByParent[parentKey].Contains(item))
                    {
                        m_itemsByParent[parentKey].Add(item);
                    }
                }
            }
        }

        public virtual void RemoveItemByParent(T item, StoreItem parentItem)
        {
            if (parentItem != null)
            {
                RemoveItemByParent(item, parentItem.StoreKey);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Removes the given work item from the cache if it exists under the given backlog
        /// item parent key.
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual void RemoveItemByParent(T item, string parentKey)
        {
            lock (m_itemsByParent)
            {
                if (!string.IsNullOrWhiteSpace(parentKey))
                {
                    if (m_itemsByParent != null && m_itemsByParent.ContainsKey(parentKey))
                    {
                        m_itemsByParent[parentKey].Remove(item);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Ensures that a collection in the cache exists for the given backlog item parent
        /// key.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected void EnsureItemsByParent(string parentKey)
        {
            lock (m_itemsByParent)
            {
                if (!m_itemsByParent.ContainsKey(parentKey))
                {
                    m_itemsByParent.Add(parentKey, new AsyncObservableCollection<T>());
                }
            }
        }

    }
}
