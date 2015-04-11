using System;
using System.Collections.Generic;

namespace PlannerNameSpace
{
    public enum DummyItemType
    {
        NoneType,
        AllType,
        AllNoneType
    }

    public partial class StoreItem
    {
        static Dictionary<string, Dictionary<DummyItemType, StoreItem>> DummyItems;
        static readonly object SyncLockDummyObject = new object();

        public bool IsDummyItem { get { return Store == null; } }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an item of the given ItemType, representing a special in-memory-only
        /// dummyType of object, such as "All" or "None".
        /// </summary>
        //------------------------------------------------------------------------------------
        public static T GetDummyItem<T>(DummyItemType dummyType) where T : StoreItem, new()
        {
            lock (SyncLockDummyObject)
            {
                if (DummyItems == null)
                {
                    DummyItems = new Dictionary<string, Dictionary<DummyItemType, StoreItem>>();
                }

                string key = typeof(T).Name;

                if (!DummyItems.ContainsKey(key))
                {
                    DummyItems.Add(key, new Dictionary<DummyItemType, StoreItem>());
                }

                T item = null;
                Dictionary<DummyItemType, StoreItem> dummyItems = DummyItems[key];
                if (!dummyItems.ContainsKey(dummyType))
                {
                    switch (dummyType)
                    {
                        case DummyItemType.AllType:
                            item = new T();
                            item.DummyInitialize(Globals.c_All);
                            break;
                        case DummyItemType.NoneType:
                            item = new T();
                            item.DummyInitialize(Globals.c_None);
                            break;
                        default:
                            throw new ApplicationException();
                    }

                    dummyItems.Add(dummyType, item);
                }

                return dummyItems[dummyType] as T;
            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Initializer that will be called when a dummy instance of this item type, with the
        /// given title, is created.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected virtual void DummyInitialize(string dummyTitle)
        {
            Title = dummyTitle;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if this object is a special in-memory "None" object.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsNoneItem
        {
            get
            {
                if (IsDummyItem)
                {
                    return Title == Globals.c_None;
                }

                return false;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if this object is a special in-memory "All" object.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsAllItem
        {
            get
            {
                if (IsDummyItem)
                {
                    return Title == Globals.c_All;
                }

                return false;
            }
        }

        public static bool IsRealItem(StoreItem item)
        {
            return item != null && !item.IsDummyItem;
        }
    }
}
