using System.Collections.Generic;

namespace PlannerNameSpace
{
    public class FilterInfoCollection : List<FilterInfo>
    {
        public T GetSelectedFilterItem<T>(ItemTypeID itemType) where T : StoreItem
        {
            FilterInfo filterInfo = GetFilterInfo(itemType);
            if (filterInfo != null)
            {
                return filterInfo.Combo.SelectedItem as T;
            }

            return null;
        }

        public string GetSelectedFilterString(ItemTypeID itemType)
        {
            FilterInfo filterInfo = GetFilterInfo(itemType);
            if (filterInfo != null)
            {
                return filterInfo.Combo.SelectedValue as string;
            }

            return null;
        }

        public bool AreAllFiltersUnset()
        {
            foreach (FilterInfo filterInfo in this)
            {
                if (IsFilterSet(filterInfo))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsOnlyThisFilterSet(ItemTypeID itemType)
        {
            foreach (FilterInfo filterInfo in this)
            {
                if (filterInfo.ItemTypeID == itemType)
                {
                    if (!IsFilterSet(filterInfo))
                    {
                        return false;
                    }
                }
                else
                {
                    if (IsFilterSet(filterInfo))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool AreTheseTheOnlyFiltersSet(ItemTypeID itemType1, ItemTypeID itemType2)
        {
            foreach (FilterInfo filterInfo in this)
            {
                if (filterInfo.ItemTypeID == itemType1 || filterInfo.ItemTypeID == itemType2)
                {
                    if (!IsFilterSet(filterInfo))
                    {
                        return false;
                    }
                }
                else
                {
                    if (IsFilterSet(filterInfo))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsFilterSet(FilterInfo filterInfo)
        {
            switch (filterInfo.ItemTypeID)
            {
                case ItemTypeID.FilterStatus:
                    string filterValue = filterInfo.Combo.SelectedValue as string;
                    return !string.IsNullOrWhiteSpace(filterValue) && filterValue != BacklogItemStates.All;
                default:
                    StoreItem selectedItem = filterInfo.Combo.SelectedItem as StoreItem;
                    return StoreItem.IsRealItem(selectedItem);
            }
        }

        public FilterInfo GetFilterInfo(ItemTypeID itemType)
        {
            foreach (FilterInfo filterInfo in this)
            {
                if (filterInfo.ItemTypeID == itemType)
                {
                    return filterInfo;
                }
            }

            return null;
        }
    }
}
