using System;
using System.Runtime.CompilerServices;

namespace PlannerNameSpace
{
    public partial class StoreItem
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the current value of the given property.
        /// </summary>
        //------------------------------------------------------------------------------------
        private object GetValue(string dsPropName, string publicPropName, PropType type)
        {
            ItemProperty property = EnsureItemProperty(dsPropName, publicPropName, type);
            return property.CurrentValue;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the property with the given name to the given value.  The propertyName value
        /// represents the name of the property as it is exposed by this class (as opposed to
        /// the propName used to write the value to the backing store).  
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        private void SetValue(string propName, object value, String publicPropName, PropType type)
        {
            if (publicPropName == null)
            {
                throw new ApplicationException();
            }

            WriteValueToProperty(propName, value, publicPropName, type);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Writes the value of the given property to local memory, maintaining the collection
        /// of changed properties in the process.
        /// </summary>
        //------------------------------------------------------------------------------------
        private ItemProperty WriteValueToProperty(string dsPropName, object value, string publicPropName, PropType type)
        {
            ItemProperty itemProperty = EnsureItemProperty(dsPropName, publicPropName, type);
            itemProperty.PreviousValue = itemProperty.CurrentValue;
            itemProperty.CurrentValue = value;
            return itemProperty;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when the dirty state of the given property has changed (i.e. when
        /// either the backing or current value changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void PropertyDirtyStateChanged(ItemProperty itemProperty)
        {
            if (itemProperty.IsValueChanged())
            {
                if (!m_changedProperties.Contains(itemProperty))
                {
                    m_changedProperties.Add(itemProperty);
                }
            }
            else
            {
                if (m_changedProperties.Contains(itemProperty))
                {
                    m_changedProperties.Remove(itemProperty);
                }
            }

            Globals.ItemManager.SaveProperty(itemProperty);
            NotifyPropertyChangedByName(itemProperty.PublicPropName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Ensure that the given property is present in the in-memory cache - if not, the
        /// property is populated with its initial value from the backing store (or with a
        /// default empty value if the item is not a persisted type).
        /// </summary>
        //------------------------------------------------------------------------------------
        private ItemProperty EnsureItemProperty(string dsPropName, string publicPropName, PropType type)
        {
            if (publicPropName == null)
            {
                throw new ApplicationException();
            }

            ItemProperty itemProperty = null;
            lock (SyncLockItemProperties)
            {
                if (!ItemProperties.TryGetValue(publicPropName, out itemProperty))
                {
                    itemProperty = new ItemProperty(this, dsPropName, publicPropName, type);
                    if (IsPersisted)
                    {
                        itemProperty.ReadFromStore();
                    }

                    ItemProperties.Add(publicPropName, itemProperty);
                }
            }

            return itemProperty;
        }

        public void SetStringValueImmediate(string propName, string propValue, [CallerMemberName] String publicPropName = "")
        {
            BeginSaveImmediate();
            SetStringValue(propName, propValue, publicPropName);
            SaveImmediate();
        }

        public void SetDateValueImmediate(string propName, DateTime propValue, [CallerMemberName] String publicPropName = "")
        {
            BeginSaveImmediate();
            SetDateValue(propName, propValue, publicPropName);
            SaveImmediate();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the string value of the specified field for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string GetStringValue(string propName, [CallerMemberName] String publicPropName = "")
        {
            string value = (string)GetValue(propName, publicPropName, PropType.StringType);

            // Workaround for PSEvaluation2 Product Studio issue:
            if (propName == "Features" && value == "a")
            {
                value = null;
            }

            return value;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the string value for the specified field for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetStringValue(string propName, string value, [CallerMemberName] String publicPropName = "")
        {
            if (value != null)
            {
                value = value.TrimEnd();
            }

            SetValue(propName, value, publicPropName, PropType.StringType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the string value of the specified field for this item.  If the value in 
        /// the backing store is empty, the None global constant will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string GetNoneStringValue(string propName, [CallerMemberName] String publicPropName = "")
        {
            string value = GetStringValue(propName, publicPropName);

            if (string.IsNullOrWhiteSpace(value))
            {
                value = Globals.c_None;
            }

            return value;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the string value of the specified field for this item.  If the given value
        /// is the None global constant, an empty value will be written to the backing store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetNoneStringValue(string propName, object value, [CallerMemberName] String publicPropName = "")
        {
            string stringValue = value as string;
            if (stringValue != null && stringValue == Globals.c_None)
            {
                stringValue = null;
            }

            SetStringValue(propName, stringValue, publicPropName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the string value of the given property - if the given value is the NotSet
        /// global property, an empty value will be set into the backing store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string GetNotSetStringValue(string propName, [CallerMemberName] String publicPropName = "")
        {
            string value = GetStringValue(propName, publicPropName);
            if (string.IsNullOrWhiteSpace(value))
            {
                return Globals.c_NotSet;
            }
            else
            {
                return value;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the string value of the given property - if the backing value is null or
        /// empty, the NotSet global constant will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetNotSetStringValue(string propName, string value, [CallerMemberName] String publicPropName = "")
        {
            if (value == Globals.c_NotSet)
            {
                value = null;
            }

            SetStringValue(propName, value, publicPropName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the date value of the specified field for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public DateTime GetDateValue(string propName, [CallerMemberName] String publicPropName = "")
        {
            return (DateTime)GetValue(propName, publicPropName, PropType.DateType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the date value for the specified property.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetDateValue(string propName, DateTime value, [CallerMemberName] String publicPropName = "")
        {
            SetValue(propName, value, publicPropName, PropType.DateType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the boolean value of the specified field for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool GetBoolValue(string propName, [CallerMemberName] String publicPropName = "")
        {
            return (bool)GetValue(propName, publicPropName, PropType.BoolType);
        }

        public void SetBoolValue(string propName, bool value, [CallerMemberName] String publicPropName = "")
        {
            SetValue(propName, value, publicPropName, PropType.BoolType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the integer value of the specified field for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public int GetIntValue(string propName, [CallerMemberName] String publicPropName = "")
        {
            return (int)GetValue(propName, publicPropName, PropType.IntType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the integer value for the specified property.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetIntValue(string propName, int value, [CallerMemberName] String publicPropName = "")
        {
            SetValue(propName, value, publicPropName, PropType.IntType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the integer value of the specified field for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public long GetLongValue(string propName, [CallerMemberName] String publicPropName = "")
        {
            return (long)GetValue(propName, publicPropName, PropType.LongType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the long value for the specified property.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetLongValue(string propName, long value, [CallerMemberName] String publicPropName = "")
        {
            SetValue(propName, value, publicPropName, PropType.LongType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the integer value of the specified field for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public double GetDoubleValue(string propName, [CallerMemberName] String publicPropName = "")
        {
            return (double)GetValue(propName, publicPropName, PropType.DoubleType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the double value for the specified property.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetDoubleValue(string propName, double value, [CallerMemberName] String publicPropName = "")
        {
            SetValue(propName, value, publicPropName, PropType.DoubleType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if this property has never been set to any value in the backing
        /// store.  For efficiency, this routine requires that you call the appropriate
        /// Get function for your value before calling IsBackingValueNull.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsBackingValueNull([CallerMemberName] String publicPropName = "")
        {
            return ItemProperties[publicPropName].IsBackingValueNull;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a date from the local cache.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected DateTime GetLocalDateValue([CallerMemberName] String publicPropName = "")
        {
            return (DateTime)GetLocalValue(publicPropName, PropType.DateType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a value from the local cache.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected object GetLocalValue(String publicPropName, PropType type)
        {
            EnsureLocalValue(publicPropName, PropType.DateType);
            return LocalProperties[publicPropName];
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets a date value that will be cached locally (not saved to the backing store).
        /// </summary>
        //------------------------------------------------------------------------------------
        protected void SetLocalDateValue(DateTime value, [CallerMemberName] String publicPropName = "")
        {
            SetLocalValue(value, publicPropName, PropType.DateType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets a value that will be cached locally (not saved to the backing store).
        /// </summary>
        //------------------------------------------------------------------------------------
        private void SetLocalValue(object value, string publicPropName, PropType type)
        {
            EnsureLocalValue(publicPropName, type);
            LocalProperties[publicPropName] = ItemProperty.GetValue(value, type);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets a value that will be cached locally (not saved to the backing store).
        /// </summary>
        //------------------------------------------------------------------------------------
        private void EnsureLocalValue(string publicPropName, PropType type)
        {
            lock (SyncLockLocalProperties)
            {
                if (LocalProperties == null)
                {
                    LocalProperties = new System.Collections.Generic.Dictionary<string, object>();
                }

                if (!LocalProperties.ContainsKey(publicPropName))
                {
                    LocalProperties.Add(publicPropName, ItemProperty.GetValue(null, type));
                }
            }
        }

    }
}
