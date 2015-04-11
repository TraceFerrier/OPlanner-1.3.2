using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlannerNameSpace
{
    public class HostItemStore : OfficeMainStore
    {
        private static readonly HostItemStore m_instance = new HostItemStore(StoreType.HostStore);

        public static HostItemStore Instance
        {
            get { return m_instance; }
        }

        public HostItemStore(StoreType storeType)
            : base(storeType)
        {
        }


    }
}
