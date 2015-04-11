using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class ItemChangeHistoryCollection : AsyncObservableCollection<ItemHistoryChange>
    {
        public ItemChangeHistoryCollection()
            : base()
        {
        }
    }

    public class ItemChangeHistoryFieldCollection : AsyncObservableCollection<ItemHistoryChangedField>
    {
        public ItemChangeHistoryFieldCollection()
            : base()
        {
        }
    }

    public class ItemHistoryChange
    {
        public ItemHistoryChange()
        {
            ChangedFields = new ItemChangeHistoryFieldCollection();
        }

        private DateTime m_changedDate;
        public DateTime ChangedDate
        {
            get { return m_changedDate; }
            set { m_changedDate = value; }
        }
        public string ChangeType { get; set; }
        public string ChangedBy { get; set; }

        private ItemChangeHistoryFieldCollection m_changedFields;

        public ItemChangeHistoryFieldCollection ChangedFields
        {
            get { return m_changedFields; }
            set { m_changedFields = value; }
        }
    }

    public class ItemHistoryChangedField
    {
        public ItemHistoryChangedField()
        {
        }

        public string FieldName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
