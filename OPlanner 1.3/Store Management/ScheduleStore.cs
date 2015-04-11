using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlannerNameSpace
{
    public class ScheduleStore : PSEvaluation2Store
    {
        private static readonly ScheduleStore m_instance = new ScheduleStore(StoreType.ScheduleStore);

        public static ScheduleStore Instance
        {
            get { return m_instance; }
        }

        public ScheduleStore(StoreType storeType)
            : base(storeType)
        {
        }

        public override T CreateStoreItem<T>(ItemTypeID typeID, ProductGroupItem productGroupItem = null)
        {
            T item = base.CreateStoreItem<T>(typeID, productGroupItem);
            if (!item.IsGlobalItem)
            {
                if (productGroupItem == null)
                {
                    productGroupItem = ProductGroupManager.Instance.CurrentProductGroup;
                }

                item.ParentProductGroupKey = productGroupItem != null ? productGroupItem.StoreKey : null;
            } 

            return item;
        }
    }
}
