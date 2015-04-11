using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class CollectionCountChangedEventArgs : EventArgs
    {
        public int OldCount { get; set; }
        public int NewCount { get; set; }
    }

    public delegate void ViewItemCollectionCountChangedEventHandler(object sender, CollectionCountChangedEventArgs e);
}
