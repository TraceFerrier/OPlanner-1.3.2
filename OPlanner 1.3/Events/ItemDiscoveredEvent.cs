using System;

namespace PlannerNameSpace
{
    public enum DiscoveryAction
    {
        Add,
        Remove,
    }

    public class ItemDiscoveredEventArgs : EventArgs
    {
        public StoreItem DiscoveredItem { get; set; }
        public DiscoveryAction Action { get; set; }

        public ItemDiscoveredEventArgs(StoreItem discoveredItem, DiscoveryAction action)
        {
            DiscoveredItem = discoveredItem;
            Action = action;
        }
    }
}
