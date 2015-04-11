using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class ProductGroupOpenedEvent : EventArgs
    {
        public string ProductGroupKey { get; set; }
        public ProductGroupItem ProductGroupItem { get; set; }
        public BackgroundTaskResult Result { get; set; }

        public ProductGroupOpenedEvent(string productGroupKey, ProductGroupItem productGroupItem, BackgroundTaskResult result)
        {
            ProductGroupKey = productGroupKey;
            ProductGroupItem = productGroupItem;
            Result = result;
        }
    }
}
