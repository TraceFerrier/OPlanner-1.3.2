using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PlannerNameSpace
{
    public class SelectorState<T> where T : StoreItem, new()
    {
        public event EventHandler CurrentItemChanged;

        private T m_currentItem;
        ProductPreferences UserPreferenceKey;
        List<DummyItemType> m_dummyTypesFound;
        Selector SelectorControl;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Give a SelectorState object your items control, and a user preference key, and it will
        /// take care of setting the initial selected item in the combo, keep track of the
        /// currently selected item, and notify you when the current item changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public SelectorState(Selector selectorControl, ProductPreferences userPreferenceKey)
        {
            UserPreferenceKey = userPreferenceKey;
            T initialSelectedItem = Globals.UserPreferences.GetItemSelectionPreference<T>(userPreferenceKey);
            InitializeSelector(selectorControl, initialSelectedItem);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Give a SelectorState object your items control, and the item to initally select, and
        /// it will take care of setting the initial selected item in the combo, keep track of
        /// the currently selected item, and notify you when the current item changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public SelectorState(Selector selectorControl, T initialSelectedItem)
        {
            UserPreferenceKey = ProductPreferences.None;
            InitializeSelector(selectorControl, initialSelectedItem);
        }

        void InitializeSelector(Selector selectorControl, T initialSelectedItem)
        {
            SelectorControl = selectorControl;
            m_dummyTypesFound = new List<DummyItemType>();
            FindDummyItems(selectorControl.ItemsSource);

            T currentItem = initialSelectedItem;
            if (currentItem == null)
            {
                currentItem = GetCurrentItemOnNullInitialSelection();
            }

            CurrentItem = currentItem;
            selectorControl.SelectedItem = currentItem;
            selectorControl.SelectionChanged += filterComboBox_SelectionChanged;
            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;
        }

        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            switch (e.Change.ChangeType)
            {
                case ChangeType.Removed:
                    if (e.Change.Item == CurrentItem)
                    {

                    }
                    break;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an appropriate item in the selector that can be used as the initial
        /// selection in case the currently selected item is null.
        /// </summary>
        //------------------------------------------------------------------------------------
        T GetCurrentItemOnNullInitialSelection()
        {
            T currentItem = null;
            if (m_dummyTypesFound.Contains(DummyItemType.AllType))
            {
                currentItem = StoreItem.GetDummyItem<T>(DummyItemType.AllType);
            }
            else if (m_dummyTypesFound.Contains(DummyItemType.NoneType))
            {
                currentItem = StoreItem.GetDummyItem<T>(DummyItemType.NoneType);
            }
            else
            {
                if (SelectorControl.HasItems)
                {
                    currentItem = SelectorControl.Items[0] as T;
                }
            }

            return currentItem;
        }

        void filterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentItem = SelectorControl.SelectedItem as T;
        }

        public T CurrentItem
        {
            get { return m_currentItem; }
            set
            {
                m_currentItem = value;

                // The current item can go null if it is deleted by the user (for example, this selector is presenting a
                // list of pillars or trains, and the user deletes the currently selected pillar or train).  In that
                // case, try to select another appropriate item in the list.
                if (m_currentItem == null)
                {
                    SelectorControl.SelectedItem = GetCurrentItemOnNullInitialSelection();
                }
                else
                {
                    if (UserPreferenceKey != ProductPreferences.None)
                    {
                        Globals.UserPreferences.SetItemSelectionPreference(UserPreferenceKey, m_currentItem);
                    }

                    if (CurrentItemChanged != null)
                    {
                        CurrentItemChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        void FindDummyItems(IEnumerable collection)
        {
            foreach (object item in collection)
            {
                StoreItem storeItem = item as StoreItem;
                if (storeItem != null)
                {
                    if (storeItem.IsAllItem)
                    {
                        m_dummyTypesFound.Add(DummyItemType.AllType);
                    }
                    else if (storeItem.IsNoneItem)
                    {
                        m_dummyTypesFound.Add(DummyItemType.NoneType);
                    }
                }
            }
        }

    }

}
