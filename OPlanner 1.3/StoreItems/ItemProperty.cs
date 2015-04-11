using System;
using ProductStudio;
using System.Runtime.CompilerServices;

namespace PlannerNameSpace
{
    public enum PropType
    {
        StringType,
        IntType,
        LongType,
        BoolType,
        DateType,
        DoubleType,
    }

    public class ItemProperty : BasePropertyChanged
    {
        private StoreItem m_item;
        private PropType m_propertyType;
        private string m_publicPropName;
        private string m_DSPropName;
        private object m_backingValue;
        private object m_previousValue;
        private object m_currentValue;
        private bool m_isBackingValueNull;

        enum DirtyStateChanged
        {
            Yes,
            No,
        }

        public ItemProperty(StoreItem item, string dsPropName, string publicPropName, PropType type)
        {
            m_item = item;
            m_propertyType = type;
            m_DSPropName = dsPropName;
            m_publicPropName = publicPropName;
            m_isBackingValueNull = true;
            SetValue(null);
        }

        public StoreItem Item
        {
            get { return m_item; }
        }

        public int ID
        {
            get { return m_item.ID; }
        }

        public PropType PropertyType
        {
            get
            {
                return m_propertyType; 
            }

            set 
            {
                m_propertyType = value;
                NotifyPropertyChangedByName();
            }
        }

        public string PublicPropName
        {
            get 
            {
                return m_publicPropName; 
            }

            set
            {
                m_publicPropName = value;
                NotifyPropertyChangedByName();
            }
        }

        public string DSPropName
        {
            get
            {
                return m_DSPropName; 
            }

            set
            {
                m_DSPropName = value;
                NotifyPropertyChangedByName();
            }
        }

        public object PreviousValue
        {
            get
            {
                return m_previousValue;
            }

            set
            {
                m_previousValue = value;
                NotifyPropertyChangedByName();
            }
        }

        public object BackingValue
        {
            get 
            {
                return m_backingValue; 
            }

            set
            {
                object originalValue = m_backingValue;
                m_backingValue = value;
                SendDirtyStateChange(originalValue, value);
                NotifyPropertyChangedByName();
            }
        }

        public object CurrentValue
        {
            get
            {
                return m_currentValue;
            }

            set 
            {
                object currentValue = m_currentValue;
                m_currentValue = value;
                SendDirtyStateChange(currentValue, value);
                NotifyPropertyChangedByName();
            }
        }

        private void SendDirtyStateChange(object currentValue, object newValue)
        {
            if (!Item.IsDummyItem)
            {
                if (!AreValuesEqual(m_propertyType, currentValue, newValue))
                {
                    m_item.PropertyDirtyStateChanged(this);
                }
            }
        }

        public bool IsBackingValueNull
        {
            get
            {
                return m_isBackingValueNull;
            }
        }

        public void ReadFromStore()
        {
            object backingValue = m_item.Store.GetItemValue(m_item.DSItem, DSPropName, PublicPropName);
            m_isBackingValueNull = backingValue == null;
            SetValue(backingValue);
        }

        private void SetValue(object initialValue)
        {
            m_backingValue = GetValue(initialValue, PropertyType);
            m_previousValue = m_backingValue;
            m_currentValue = m_backingValue;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the backing value and the current value of this property are
        /// different.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsValueChanged()
        {
            return AreValuesEqual(PropertyType, BackingValue, CurrentValue) == false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the current values of this object and the given object are equal.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsEqual(ItemProperty itemProperty)
        {
            if (itemProperty.PropertyType != PropertyType)
            {
                return false;
            }

            return AreValuesEqual(PropertyType, itemProperty.CurrentValue, CurrentValue);
        }

        private static bool AreValuesEqual(PropType propertyType, object backingValue, object currentValue)
        {
            switch (propertyType)
            {
                case PropType.StringType:
                    string strBackingValue = (string)backingValue;
                    string strCurrentValue = (string)currentValue;
                    if (string.IsNullOrWhiteSpace(strBackingValue) && string.IsNullOrWhiteSpace(strCurrentValue))
                    {
                        return true;
                    }

                    return String.Compare(strBackingValue, strCurrentValue) == 0;

                case PropType.IntType:
                    int intBackingValue = (int)backingValue;
                    int intCurrentValue = (int)currentValue;
                    return intBackingValue == intCurrentValue;

                case PropType.LongType:
                    long longBackingValue = (long)backingValue;
                    long longCurrentValue = (long)currentValue;
                    return longBackingValue == longCurrentValue;

                case PropType.DoubleType:
                    double doubleBackingValue = (double)backingValue;
                    double doubleCurrentValue = (double)currentValue;
                    return doubleBackingValue == doubleCurrentValue;

                case PropType.DateType:
                    DateTime dateBackingValue = (DateTime)backingValue;
                    DateTime dateCurrentValue = (DateTime)currentValue;
                    return dateBackingValue.CompareTo(dateCurrentValue) == 0;

                case PropType.BoolType:
                    bool boolBackingValue = (bool)backingValue;
                    bool boolCurrentValue = (bool)currentValue;
                    return boolBackingValue == boolCurrentValue;

                default:
                    throw new ApplicationException("Property Type Unrecognized");
            }
        }

        public static object GetValue(object initialValue, PropType type)
        {
            object finalValue = null;

            switch (type)
            {
                case PropType.StringType:
                    finalValue = Utils.GetStringValue(initialValue);
                    break;

                case PropType.IntType:
                    finalValue = Utils.GetIntValue(initialValue);
                    break;

                case PropType.LongType:
                    finalValue = Utils.GetLongValue(initialValue);
                    break;

                case PropType.DoubleType:
                    finalValue = Utils.GetDoubleValue(initialValue);
                    break;

                case PropType.DateType:
                    finalValue = Utils.GetDateTimeValue(initialValue);
                    break;
                case PropType.BoolType:
                    finalValue = Utils.GetBoolValue(initialValue);
                    break;

                default:
                    throw new ApplicationException("Unrecognized property type in SetValue:" + type.ToString());
            }

            return finalValue;
        }

    }
}
