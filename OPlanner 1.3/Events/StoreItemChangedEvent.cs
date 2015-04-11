using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class StoreItemChangedEventArgs : EventArgs
    {
        public StoreItemChange Change;

        public StoreItemChangedEventArgs(StoreItemChange change)
        {
            Change = change;
        }
    }
}
