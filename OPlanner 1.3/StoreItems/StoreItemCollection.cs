using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class StoreItemCollection<T> : AsyncObservableCollection<T>, IStoreItemList where T : StoreItem
    {
        #region Implementation of IStoreItemList

        void IStoreItemList.Add(StoreItem item)
        {
            Add((T)item);
        }

        bool IStoreItemList.Remove(StoreItem item)
        {
            return Remove((T)item);
        }

        bool IStoreItemList.Contains(StoreItem item)
        {
            return Contains((T)item);
        }

        IEnumerator IStoreItemList.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
