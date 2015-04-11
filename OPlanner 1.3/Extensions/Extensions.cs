using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public static class Extensions
    {
        public static bool IsStoreProperty(this string str, string propName)
        {
            return Utils.StringsMatch(str, propName);
        }

        public static bool IsDefault(this DateTime dt)
        {
            return default(DateTime).CompareTo(dt) == 0;
        }

        public static List<T> ToList<T>(this Dictionary<string, T> dict)
        {
            List<T> list = new List<T>();
            foreach (KeyValuePair<string, T> kvp in dict)
            {
                T item = kvp.Value;
                list.Add(item);
            }

            return list;
        }

        public static AsyncObservableCollection<T> GetItems<T>(this AsyncObservableCollection<T> collection, DummyItemType collectionType) where T:StoreItem, new()
        {
            AsyncObservableCollection<T> items = collection.ToCollection();
            items.Sort((x, y) => x.Title.CompareTo(y.Title));

            if (collectionType == DummyItemType.NoneType || collectionType == DummyItemType.AllNoneType)
            {
                T item = StoreItem.GetDummyItem<T>(DummyItemType.NoneType);
                items.Insert(0, item);
            }

            if (collectionType == DummyItemType.AllType || collectionType == DummyItemType.AllNoneType)
            {
                T item = StoreItem.GetDummyItem<T>(DummyItemType.AllType);
                items.Insert(0, item);
            }

            // If there are no items at all in the collection, throw an 'All' on the list
            if (items.Count == 0)
            {
                T item = StoreItem.GetDummyItem<T>(DummyItemType.AllType);
                items.Insert(0, item);
            }

            return items;
        }


        public static T GetItem<T>(this AsyncObservableCollection<T> collection, int idx)
        {
            return collection[idx];
        }

        private class Comparer<T> : IComparer<T>
        {
            private readonly Comparison<T> comparison;

            public Comparer(Comparison<T> comparison)
            {
                this.comparison = comparison;
            }

            #region IComparer<T> Members

            public int Compare(T x, T y)
            {
                return comparison.Invoke(x, y);
            }

            #endregion
        }
    }
}
