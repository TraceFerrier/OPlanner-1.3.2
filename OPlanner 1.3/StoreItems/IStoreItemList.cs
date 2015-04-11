using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace PlannerNameSpace
{
    public interface IStoreItemList
    {
        void Add(StoreItem item);
        bool Remove(StoreItem item);
        bool Contains(StoreItem item);
        IEnumerator GetEnumerator();
    }
}
