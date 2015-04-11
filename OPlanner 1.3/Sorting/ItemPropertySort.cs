using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace PlannerNameSpace
{
    class ItemPropertySort<T> : BaseSort
    {
        PropertyInfo SortProp;
        public ItemPropertySort(string propName, ListSortDirection direction)
        {
            Init(propName, direction);
        }

        public ItemPropertySort(string propName)
        {
            Init(propName, ListSortDirection.Ascending);
        }

        void Init(string propName, ListSortDirection direction)
        {
            m_direction = direction;
            SortProp = typeof(T).GetProperty(propName);
        }

        public override int Compare(object x, object y)
        {
            StoreItem itemX = (StoreItem)x;
            StoreItem itemY = (StoreItem)y;
            bool isXEarlier = false;

            if (itemX == itemY)
            {
                return 0;
            }
            else if (itemX.IsDummyItem && itemY.IsDummyItem)
            {
                isXEarlier = itemX.IsAllItem;
            }
            else if (itemX.IsDummyItem)
            {
                isXEarlier = true;
            }
            else if (itemY.IsDummyItem)
            {
                isXEarlier = false;
            }
            else
            {
                string xValue = (string)SortProp.GetValue(itemX);
                string yValue = (string)SortProp.GetValue(itemY);

                int compare = xValue.CompareTo(yValue);
                if (compare == 0)
                {
                    return 0;
                }

                isXEarlier = compare < 0;
            }

            if (m_direction == ListSortDirection.Ascending)
            {
                return isXEarlier ? -1 : 1;
            }
            else
            {
                return isXEarlier ? 1 : -1;
            }

        }
    }
}
