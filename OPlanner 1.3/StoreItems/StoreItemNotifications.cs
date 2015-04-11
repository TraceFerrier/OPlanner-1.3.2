using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public partial class StoreItem
    {
        protected virtual void ItemPropertyChanged(NotificationArgs changeArgs) { }

        private void NotifyItemUpdatedWorker(NotificationArgs args)
        {
            ItemPropertyChanged(args);

            if (IsRealItem(ParentItem))
            {
                NotificationArgs parentArgs = args;
                parentArgs.ChangeSource = HierarchicalChangeSource.ChildItem;
                ParentItem.NotifyItemUpdatedWorker(parentArgs);
            }

            if (IsRealItem(OwnerItem))
            {
                NotificationArgs ownerArgs = args;
                ownerArgs.ChangeSource = HierarchicalChangeSource.OwnedItem;
                OwnerItem.NotifyItemUpdatedWorker(ownerArgs);
            }
        }

        private void NotifyItemUpdated(ItemProperty itemProperty)
        {
            if (Globals.ApplicationManager.IsStartupComplete)
            {
                NotificationArgs args = new NotificationArgs(this, ChangeType.Updated, HierarchicalChangeSource.ThisItem, itemProperty);
                NotifyItemUpdatedWorker(args);

                ItemPropertyChanged(args);

                // Also notify children that the parent has changed.
                IStoreItemList childItems = GetChildItems();
                if (childItems != null)
                {
                    args.ChangeSource = HierarchicalChangeSource.ParentItem;
                    foreach (StoreItem childItem in childItems)
                    {
                        childItem.ItemPropertyChanged(args);
                    }
                }
            }

        }
    }
}
