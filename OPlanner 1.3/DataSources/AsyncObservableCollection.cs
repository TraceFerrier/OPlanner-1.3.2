using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace PlannerNameSpace
{
    public class AsyncObservableCollection<T> : IList<T>, IList, INotifyCollectionChanged
    {
        private IList<T> _list = new List<T>();

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<T>

        public void Clear()
        {
            //lock (SyncObject)
            {
                _list.Clear();

                OnCollectionChanged(NotifyCollectionChangedAction.Reset, null);
            }
        }

        public bool Contains(T item)
        {
            //lock (SyncObject)
            {
                return _list.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (SyncObject)
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get 
            {
                return _list.Count; 
            }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        #endregion

        #region Implementation of IList<T>

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (SyncObject)
            {
                _list.Insert(index, item);

                OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (SyncObject)
            {
                T item = _list[index];
                _list.RemoveAt(index);

                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            }
        }

        public T this[int index]
        {
            get
            {
                return _list[index];
            }

            set
            {
                //lock (SyncObject)
                {
                    T item = value;
                    _list[index] = item;

                    OnCollectionChanged(NotifyCollectionChangedAction.Replace, item);
                }
            }
        }

        #endregion

        #region Implementation of ICollection

        public void CopyTo(Array array, int arrayIndex)
        {
            lock (SyncObject)
            {
                int i = arrayIndex;
                foreach (T item in _list)
                {
                    array.SetValue(item, i);
                    i++;
                }
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region Implementation of IList

        object IList.this[int index]
        {
            get
            {
                return _list[index];
            }

            set
            {
                //lock (SyncObject)
                {
                    T item = (T)value;
                    _list[index] = item;

                    OnCollectionChanged(NotifyCollectionChangedAction.Replace, item);
                }
            }
        }

        int IList.Add(object item)
        {
            return _Add((T)item);
        }

        bool IList.Contains(object item)
        {
            return Contains((T)item);
        }

        int IList.IndexOf(object item)
        {
            if (item == null)
            {
                return -1;
            }

            return IndexOf((T)item);
        }

        void IList.Insert(int index, object item)
        {
            Insert(index, (T)item);
        }

        public bool IsFixedSize
        {
            get
            {
                return true;
            }
        }

        void IList.Remove(object item)
        {
            Remove((T)item);
        }

        #endregion

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public object SyncObject;

        public AsyncObservableCollection()
        {
            SyncObject = new object();
        }

        public virtual void Add(T item)
        {
            _Add(item);
        }

        private int _Add(T item)
        {
            lock (SyncObject)
            {
                _list.Add(item);

                OnCollectionChanged(NotifyCollectionChangedAction.Add, item);

                return _list.Count - 1;
            }
        }

        public virtual bool Remove(T item)
        {
            return _Remove(item);
        }

        private bool _Remove(T item)
        {
            lock (SyncObject)
            {
                bool isRemoved = _list.Remove(item);
                if (isRemoved)
                {
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
                }

                return isRemoved;
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            lock (SyncObject)
            {
                _list[newIndex] = _list[oldIndex];
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            lock (SyncObject)
            {
                var comparer = new Comparer<T>(comparison);
                _list = _list.OrderBy(x => x, comparer).ToList<T>();
            }
        }

        public List<T> ToList()
        {
            List<T> newList = new List<T>();

            lock (SyncObject)
            {
                int index = 0;
                while (index < _list.Count)
                {
                    newList.Add(_list[index]);
                    index++;
                }
            }

            return newList;
        }

        public AsyncObservableCollection<T> ToCollection()
        {
            AsyncObservableCollection<T> newList = new AsyncObservableCollection<T>();

            lock (SyncObject)
            {
                int index = 0;
                while (index < _list.Count)
                {
                    newList.Add(_list[index]);
                    index++;
                }
            }

            return newList;
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, T item)
        {
            List<T> changedItems = new List<T>();
            changedItems.Add(item);
            OnCollectionChanged(action, changedItems);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, IList changedItems)
        {
            NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;
            if (eventHandler == null)
            {
                return;
            }

            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action, changedItems);

            Delegate[] delegates = eventHandler.GetInvocationList();
            // Walk thru invocation list
            foreach (NotifyCollectionChangedEventHandler handler in delegates)
            {
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                {
                    handler(this, e);
                }
            }
        }

        private class Comparer<CT> : IComparer<CT>
        {
            private readonly Comparison<CT> comparison;

            public Comparer(Comparison<CT> comparison)
            {
                this.comparison = comparison;
            }

            #region IComparer<CT> Members

            public int Compare(CT x, CT y)
            {
                return comparison.Invoke(x, y);
            }

            #endregion
        }

    }
}
