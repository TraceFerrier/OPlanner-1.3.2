using ProductStudio;

namespace PlannerNameSpace
{
    public enum IsRefresh
    {
        Yes,
        No
    }

    public class DeferredStoreItem
    {
        public Datastore ItemStore { get; set; }
        public DatastoreItem DSItem { get; set; }
        public IsRefresh IsRefreshedItem { get; set; }
        int m_id;
        string m_storeKey;

        public DeferredStoreItem(Datastore itemStore, DatastoreItem dsItem, IsRefresh isRefreshedItem)
        {
            m_id = -1;
            m_storeKey = null;
            ItemStore = itemStore;
            DSItem = dsItem;
            IsRefreshedItem = isRefreshedItem;
        }

        public string StoreKey
        {
            get
            {
                if (m_storeKey == null)
                {
                    m_storeKey = StoreItem.GetItemKey(ItemStore.StoreID, ID);
                }

                return m_storeKey;
            }
        }

        public int ID
        {
            get
            {
                if (m_id < 0)
                {
                    m_id = Utils.GetIntValue(ItemStore.GetBackingValue(DSItem, Datastore.PropNameID));
                }

                return m_id;
            }
        }
    }
}
