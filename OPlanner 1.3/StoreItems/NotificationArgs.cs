
namespace PlannerNameSpace
{
    public enum HierarchicalChangeSource
    {
        ThisItem,
        ChildItem,
        OwnedItem,
        ParentItem,
        NotSet,
    }

    public class NotificationArgs
    {
        public StoreItem Item { get; set; }
        public ChangeType ChangeType { get; set; }
        public HierarchicalChangeSource ChangeSource { get; set; }
        public ItemProperty ChangedProperty { get; set; }

        public NotificationArgs(StoreItem item, ChangeType type, HierarchicalChangeSource changeSource, ItemProperty changedProperty)
        {
            Item = item;
            ChangeType = type;
            ChangedProperty = changedProperty;
            ChangeSource = changeSource;
        }
    }
}
