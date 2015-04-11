using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public enum ChangeType
    {
        Added,
        Removed,
        Updated,
    }

    public enum ChangeSource
    {
        Default,
        Undo,
    }

    public class StoreItemChange
    {
        public StoreItem Item { get; set; }
        public ChangeType ChangeType { get; set; }
        public ChangeSource ChangeSource { get; set; }
        public string PublicPropName { get; set; }
        public string DSPropName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public StoreItemChange()
        {
            ChangeSource = PlannerNameSpace.ChangeSource.Default;
        }
    }
}
