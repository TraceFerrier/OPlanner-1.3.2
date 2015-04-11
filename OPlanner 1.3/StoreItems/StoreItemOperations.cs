using ProductStudio;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PlannerNameSpace
{
    public partial class StoreItem
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reverts all changes in this item to the previously set values.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void UndoChanges()
        {
            List<ItemProperty> changedProperties = m_changedProperties.ToList();
            foreach (ItemProperty itemProperty in changedProperties)
            {
                SetPublicProperty(itemProperty.PublicPropName, itemProperty.BackingValue);
            }

            ItemDocuments.UndoChanges();
            ItemImages.UndoChanges();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called when this item has been refreshed from the backing store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void OnRefreshedFromStore(DatastoreItem dsItem)
        {
            SyncItemFromStore(dsItem);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called by the Change Manager if a merge conflict is detected while trying
        /// to persist this item.  The given dsItem has been re-opened from the backing
        /// store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void MergeChanges(DatastoreItem dsItem)
        {
            SyncItemFromStore(dsItem);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called when this item has been refreshed from the backing store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SyncItemFromStore(DatastoreItem dsItem)
        {
            // First, set the secondary backing store cache to the one just received.
            DSItem = dsItem;

            foreach (KeyValuePair<string, ItemProperty> kvp in ItemProperties)
            {
                // If our value hasn't changed, accept the value from the back-end store
                // if it's different from ours.
                ItemProperty itemProperty = kvp.Value;
                if (!itemProperty.IsValueChanged())
                {
                    SyncPropertyFromStore(itemProperty);
                }

                // Otherwise, put our in-memory changed value into the new backing store cache, so
                // that our value will still get persisted on the next commit operation.
                else
                {
                    Store.SetItemBackingValue(this, itemProperty.DSPropName, itemProperty.PublicPropName, itemProperty.CurrentValue);
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reads the latest value of the given property from the backing store, and if the
        /// value is different what's currently in memory, sets the value of the given 
        /// property to the new value.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SyncPropertyFromStore(ItemProperty itemProperty)
        {
            // Read the latest value from the backing store
            ItemProperty propertyFromStore = new ItemProperty(this, itemProperty.DSPropName, itemProperty.PublicPropName, itemProperty.PropertyType);
            propertyFromStore.ReadFromStore();

            // If the backing store value doesn't match in the in-memory cached value, update
            // the object model with the new value.
            if (!propertyFromStore.IsEqual(itemProperty))
            {
                // First set the backing value of our in-memory property to the value from the store
                itemProperty.BackingValue = propertyFromStore.BackingValue;

                // Use the same value to set the current value (via the object model) so that 
                // backing and current values are sync'd, and all UI side-effects are enacted.
                SetPublicProperty(itemProperty.PublicPropName, propertyFromStore.BackingValue);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the value of the given property through the object model, ensuring that all 
        /// side-effects of setting the property are also applied (just as though the user set 
        /// the property via the UI).
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetPublicProperty(string publicPropertyName, object value)
        {
            PropertyInfo prop = this.GetType().GetProperty(publicPropertyName);
            if (prop == null)
            {
                throw new ApplicationException("Unrecognized Property name: " + publicPropertyName);
            }

            prop.SetValue(this, value);
        }

        public object GetPublicProperty(string publicPropertyName)
        {
            PropertyInfo prop = this.GetType().GetProperty(publicPropertyName);
            if (prop == null)
            {
                throw new ApplicationException("Unrecognized Property name: " + publicPropertyName);
            }

            return prop.GetValue(this);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Ensures that the backing store for the given item is synced with the current
        /// contents of the item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void WriteToBackingStoreBeforeSave(IsOpenedForEdit isOpen)
        {
            foreach (ItemProperty itemProperty in m_changedProperties)
            {
                Store.SetItemBackingValue(this, itemProperty.DSPropName, itemProperty.PublicPropName, itemProperty.CurrentValue, isOpen);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Examines the current backing store values for this item, and if any don't match,
        /// updates the in-memory cache to put it in sync with the back end.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void FinalizePropertyAfterSave(ItemProperty itemProperty)
        {
                // Since the property is now saved, sync the backing value and the current value
                bool isPropertyChanged = IsPropertyOnChangeList(itemProperty);
                itemProperty.BackingValue = itemProperty.CurrentValue;

                // If this isn't a changed property we just saved to the backing store, and the property 
                // doesn't require MustBeOpenToRead, then read the value that's now in the backing store,
                // to pick up any back-end server business rule changes.
                // Note: we don't try to read MustBeOpenToRead properties, since they are expensive to
                // open, and aren't subject to business rule changes.
                if (!isPropertyChanged && !MustPropertyBeOpenToRead(itemProperty))
                {
                    SyncPropertyFromStore(itemProperty);
                }
        }

    }
}
