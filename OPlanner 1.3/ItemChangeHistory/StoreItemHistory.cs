using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public enum SearchMode
    {
        StartAtCurrentIndex,
        StartAtNextIndex,
    }

    public partial class StoreItem
    {
        ItemChangeHistoryCollection ChangeHistory { get; set; }

        public ItemChangeHistoryCollection GetItemChangeHistory()
        {
            if (ChangeHistory == null)
            {
                ChangeHistory = CalculateItemChangeHistory();
            }

            return ChangeHistory;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of strings recapping the change history for the given bug.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected ItemChangeHistoryCollection CalculateItemChangeHistory()
        {
            ProductStudio.DatastoreItem psItem = DSItem;

            ItemChangeHistoryCollection bugChanges = new ItemChangeHistoryCollection();
            if (psItem != null)
            {
                ItemHistoryChange originalChange = GetBugOriginalChange();
                if (originalChange == null)
                {
                    return bugChanges;
                }

                bugChanges.Add(originalChange);

                Store.OpenForRead(psItem);
                ProductStudio.DatastoreItemHistory history = psItem.History;
                const int c_originalChange = 1;
                for (int i = c_originalChange; i <= history.Count; ++i)
                {
                    ItemHistoryChange bugChange = new ItemHistoryChange();
                    bugChange.ChangeType = ChangeTypes.Edited;
                    bugChange.ChangedBy = GetChangedFieldValue(Datastore.PropNameChangedBy, history, i, SearchMode.StartAtNextIndex) as string;

                    ProductStudio.DatastoreItem item = history.get_ItemByRevision(i);
                    ProductStudio.Fields fields = item.Fields;
                    foreach (ProductStudio.Field field in fields)
                    {
                        if (!Utils.IsFieldValueNull(field))
                        {
                            if (field.Name == Datastore.PropNameChangedDate)
                            {
                                bugChange.ChangedDate = (DateTime)field.Value;
                            }
                            else if (field.Name != Datastore.PropNamePersonID && field.Name != Datastore.PropNameRevisedDate &&
                                field.Name != Datastore.PropNamePersonName && field.Name != Datastore.PropNameRev &&
                                field.Name != Datastore.PropNameLatestHistoryDescription && field.Name != Datastore.PropNameChangedBy)
                            {
                                ItemHistoryChangedField changedField = new ItemHistoryChangedField();
                                changedField.FieldName = field.Name;

                                changedField.OldValue = field.Value;
                                changedField.NewValue = GetChangedFieldValue(field.Name, history, i, SearchMode.StartAtNextIndex);
                                if (changedField.FieldName == Datastore.PropNameStatus)
                                {
                                    if (changedField.NewValue.ToString() == StatusValues.Resolved)
                                    {
                                        bugChange.ChangeType = StatusValues.Resolved;
                                    }
                                    else if (changedField.NewValue.ToString() == StatusValues.Active)
                                    {
                                        bugChange.ChangeType = "Activated";
                                    }
                                    else if (changedField.NewValue.ToString() == StatusValues.Closed)
                                    {
                                        bugChange.ChangeType = StatusValues.Closed;
                                    }
                                }

                                bugChange.ChangedFields.Add(changedField);
                            }
                        }
                    }
                    if (bugChange != null)
                    {
                        bugChanges.Add(bugChange);
                    }
                }

                return bugChanges;
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of strings recapping the change history for the given bug.
        /// </summary>
        //------------------------------------------------------------------------------------
        public ItemHistoryChange GetBugOriginalChange()
        {
            Store.OpenForRead(DSItem);
            ProductStudio.DatastoreItem psItem = DSItem;
            if (psItem != null)
            {
                ProductStudio.DatastoreItemHistory history = psItem.History;
                ItemHistoryChange bugChange = new ItemHistoryChange();
                bugChange.ChangedBy = GetChangedFieldValue(Datastore.PropNameChangedBy, history, 1, SearchMode.StartAtCurrentIndex) as string;
                bugChange.ChangeType = ChangeTypes.Opened;
                bugChange.ChangedDate = OpenedDate;

                ProductStudio.DatastoreItem item;
                try
                {
                    item = history.get_ItemByRevision(1);
                }

                catch
                {
                    return null;
                }

                ProductStudio.Fields fields = item.Fields;
                foreach (ProductStudio.Field field in fields)
                {
                    if (field.Name != Datastore.PropNamePersonID && field.Name != Datastore.PropNameRevisedDate &&
                        field.Name != Datastore.PropNamePersonName && field.Name != Datastore.PropNameRev &&
                        field.Name != Datastore.PropNameLatestHistoryDescription && field.Name != Datastore.PropNameChangedBy)
                    {
                        ItemHistoryChangedField changedField = new ItemHistoryChangedField();
                        changedField.FieldName = field.Name;

                        changedField.OldValue = "";
                        changedField.NewValue = GetChangedFieldValue(field.Name, history, 1, SearchMode.StartAtCurrentIndex);
                        if (changedField.NewValue != null && changedField.NewValue.ToString().Length > 0)
                        {
                            bugChange.ChangedFields.Add(changedField);
                        }
                    }
                }

                return bugChange;
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a changed field from the given bug history, returns the new value for that
        /// item.
        /// </summary>
        //------------------------------------------------------------------------------------
        private object GetChangedFieldValue(string fieldName, ProductStudio.DatastoreItemHistory history, int currentIndex, SearchMode mode)
        {
            if (mode == SearchMode.StartAtNextIndex)
            {
                currentIndex++;
            }

            while (currentIndex < history.Count)
            {
                ProductStudio.DatastoreItem item = history.get_ItemByRevision(currentIndex);
                ProductStudio.Fields fields = item.Fields;
                if (!Utils.IsFieldValueNull(fields[fieldName]))
                {
                    object returnValue = fields[fieldName].Value;
                    if (fieldName == Datastore.PropNameChangedBy && !Utils.IsFieldValueNull(fields[Datastore.PropNamePersonName]))
                    {
                        string personName = fields[Datastore.PropNamePersonName].Value as string;
                        if (personName != returnValue.ToString())
                        {
                            returnValue = returnValue.ToString() + " (" + personName + ")";
                        }
                    }
                    return returnValue;
                }
                currentIndex++;
            }

            return DSItem.Fields[fieldName].Value;
        }

    }
}
