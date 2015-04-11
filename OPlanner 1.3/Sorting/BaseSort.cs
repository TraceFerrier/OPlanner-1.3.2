using System;
using System.Collections;
using System.ComponentModel;

namespace PlannerNameSpace
{
    public abstract class BaseSort : IComparer
    {
        protected Nullable<ListSortDirection> m_direction;
        public void SetSortDirection(ListSortDirection direction)
        {
            m_direction = direction;
        }

        public virtual int Compare(object x, object y)
        {
            return 0;
        }
    }
}
