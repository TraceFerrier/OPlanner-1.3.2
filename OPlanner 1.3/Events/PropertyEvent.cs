using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class PropertyEventArgs : EventArgs
    {
        public StoreItem Item { get; set; }
        public ItemProperty Property { get; set; }

        public PropertyEventArgs(StoreItem item, ItemProperty property)
        {
            Item = item;
            Property = property;
        }
    }
}
