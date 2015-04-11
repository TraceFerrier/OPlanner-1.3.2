using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class DatastoreID
    {
        public StoreType StoreType { get; set; }
        public string Name { get; set; }

        public Datastore Store
        {
            get
            {
                return StoreType == StoreType.ScheduleStore ? (Datastore) ScheduleStore.Instance : HostItemStore.Instance;
            }
        }

        public DatastoreID(string storeName, StoreType storeType)
        {
            StoreType = storeType;
            Name = storeName;
        }
    }
}
