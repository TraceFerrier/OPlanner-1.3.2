using ProductStudio;
using System;
using System.Collections.Generic;

namespace PlannerNameSpace
{
    public enum IsOpenedForEdit
    {
        Yes,
        No
    }

    public class CommitItemValueArgs
    {
        public StoreItem Item { get; set; }
        public string PropName { get; set; }
        public object Value { get; set; }
    }

    public abstract partial class Datastore
    {
        public object GetItemValue(DatastoreItem dsItem, string dsPropName, string publicPropName)
        {
            object value;
            if (!CompositeRegistry.GetValueFromComposite(dsItem, dsPropName, publicPropName, out value))
            {
                value = GetBackingValue(dsItem, dsPropName);
            }

            return value;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the value of the given property directly from the backing store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public object GetBackingValue(DatastoreItem dsItem, string dsPropName)
        {
            if (dsItem == null)
            {
                return null;
            }

            //try
            {
                if (MustBeOpenToRead(dsPropName))
                {
                    OpenForRead(dsItem);
                }

                return dsItem.Fields[dsPropName].Value;

            }
            //catch (Exception exception)
            //{
            //    Globals.ApplicationManager.HandleException(exception);
            //    return null;
            //}
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// For the given StoreItem, returns the current value of the specified property.
        /// </summary>
        //------------------------------------------------------------------------------------
        public object GetItemValue(StoreItem item, string dsPropName, string publicPropName)
        {
            if (item == null || item.DSItem == null)
            {
                return null;
            }

            return GetItemValue(item.DSItem, dsPropName, publicPropName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// For the given StoreItem, set the current value of the specified property directly
        /// into the backing store.  If isOpen is true, the item will be assumed to already
        /// be open for edit, and won't be opened again.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetItemBackingValue(StoreItem item, string dsPropName, string publicPropName, object value, IsOpenedForEdit isOpen = IsOpenedForEdit.No)
        {
            DatastoreItem dsItem = item.DSItem;
            if (dsItem != null)
            {
                if (isOpen == IsOpenedForEdit.No && item.ID != 0)
                {
                    OpenForEdit(item.DSItem);
                }

                Field field = dsItem.Fields[dsPropName];
                if (!field.IsReadOnly)
                {
                    object finalValue;
                    if (!CompositeRegistry.GetCompositeFromValue(dsItem, dsPropName, publicPropName, value, out finalValue))
                    {
                        finalValue = value;
                    }

                    field.Value = finalValue;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Internal help function to set a backing value in the given blank dsItem directly.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void SetFieldValue(DatastoreItem dsItem, string dsPropName, object value)
        {
            if (dsItem != null)
            {
                Field field = dsItem.Fields[dsPropName];
                if (!field.IsReadOnly)
                {
                    field.Value = value;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the given property must be open to read (i.e. this may be an
        /// expensive property to pull from the server).
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool MustBeOpenToRead(string propName)
        {
            return !FieldDefs[propName].CanReadWithoutOpen;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Performs a save to the backing store for the given store item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SaveItemWorker(StoreItem item)
        {
            try
            {
                Globals.ApplicationManager.WriteToEventLog("Saving item - " + item.ID.ToString() + ": " + item.Title);

                if (item.ID != 0)
                {
                    OpenForEdit(item.DSItem);
                }

                WriteItemToDS(item);

                // Save all attached document files
                item.ItemDocuments.SaveAll();

                // Save all attached image files
                item.ItemImages.SaveAll();

                // List of standard files to attach
                if (item.AttachedFileNamesToCommit != null)
                {
                    DatastoreItem dsItem = item.DSItem; 
                    foreach (string filename in item.AttachedFileNamesToCommit)
                    {
                        // If a previous file with this filename exists in the store, remove it.
                        RemoveAttachedFile(dsItem, filename);

                        // Add the new file
                        dsItem.Files.AddEx(filename);
                    }
                }

                // An attached file to be removed
                if (item.AttachedFileNameToRemove != null)
                {
                    QueueAttachedFileForRemoval(item, item.AttachedFileNameToRemove);
                }

                item.DSItem.Save();
                ResetItem(item);

                Globals.ApplicationManager.WriteToEventLog("Save complete:" + item.ID.ToString());

                item.LastSavedTime = DateTime.Now;
            }

            catch (Exception exception)
            {
                Globals.ApplicationManager.WriteToEventLog("SaveItemWorker exception: " + exception.Message);

                // Has someone else saved changes to this item since we opened it?
                if (exception.Message.Contains(Globals.PSErrIssueUpdated))
                {
                    if (item.CommitErrorState != CommitErrorStates.MergingChanges)
                    {
                        //  Yes, so re-open the item, merge their changes with ours, then then re-save it.
                        DatastoreItem dsItem = m_storeItemList.Datastore.GetDatastoreItem(PsDatastoreItemTypeEnum.psDatastoreItemTypeBugs, item.ID);
                        OpenForEdit(dsItem);
                        item.CommitErrorState = CommitErrorStates.MergingChanges;
                        item.MergeChanges(dsItem);
                        SaveItemWorker(item);
                    }
                    else
                    {
                        // Should never happen, but we got the same IssueUpdated error again after trying to
                        // re-open and merge changes.  
                    }
                }
                else if (exception.Message.Contains(Globals.PSErrAttachmentShare))
                {
                    item.CommitErrorState = CommitErrorStates.ErrorAccessingAttachmentShare;
                    Globals.ApplicationManager.HandleStoreItemException(item, exception);
                }
                else
                {
                    throw exception;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Performs a resolution for the given store item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ResolveStoreItemWorker(StoreItem item)
        {
            try
            {
                Globals.ApplicationManager.WriteToEventLog("Resolving item - " + item.ID.ToString() + ": " + item.Title);

                OpenForResolve(item.DSItem);
                WriteItemToDS(item);

                item.DSItem.Fields[PropNameResolution].Value = item.Resolution;

                if (item.StoreItemType == ItemTypeID.PlannerBug)
                {
                    PlannerBugItem bugItem = (PlannerBugItem) item;
                    bugItem.DSItem.Fields[PropNameBugComments].Value = bugItem.BugComments;
                }

                item.DSItem.Save();
                ResetItem(item);

                item.LastSavedTime = DateTime.Now;

                Globals.ApplicationManager.WriteToEventLog("Resolve complete:" + item.ID.ToString());
            }

            catch (Exception exception)
            {
                Globals.ApplicationManager.WriteToEventLog("ResolveStoreItemWorker exception: " + exception.Message);
                throw exception;
            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Performs a close operation for the given store item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void CloseStoreItemWorker(StoreItem item)
        {
            try
            {
                Globals.ApplicationManager.WriteToEventLog("Closing item - " + item.ID.ToString() + ": " + item.Title);

                OpenForClose(item.DSItem);
                WriteItemToDS(item);

                if (item.StoreItemType == ItemTypeID.PlannerBug)
                {
                    PlannerBugItem bugItem = (PlannerBugItem)item;
                    bugItem.DSItem.Fields[PropNameBugComments].Value = bugItem.BugComments;
                }

                item.DSItem.Save();
                ResetItem(item);

                item.LastSavedTime = DateTime.Now;

                Globals.ApplicationManager.WriteToEventLog("Close complete:" + item.ID.ToString());
            }

            catch (Exception exception)
            {
                Globals.ApplicationManager.WriteToEventLog("CloseStoreItemWorker exception: " + exception.Message);
                throw exception;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Performs an activate operation for the given store item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ActivateStoreItemWorker(StoreItem item)
        {
            try
            {
                Globals.ApplicationManager.WriteToEventLog("Activating item - " + item.ID.ToString() + ": " + item.Title);

                OpenForActivate(item.DSItem);
                WriteItemToDS(item);

                item.DSItem.Save();
                ResetItem(item);

                item.LastSavedTime = DateTime.Now;

                Globals.ApplicationManager.WriteToEventLog("Activation complete:" + item.ID.ToString());

            }

            catch (Exception exception)
            {
                Globals.ApplicationManager.WriteToEventLog("CloseStoreItemWorker exception: " + exception.Message);
                throw exception;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Transfers the contents of the in-memory cache for this item to the store cache,
        /// in preparation for persisting the contents to the store.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void WriteItemToDS(StoreItem item)
        {
            item.WriteToBackingStoreBeforeSave(IsOpenedForEdit.Yes);
        }


        private void FinalizePropertyAfterSave(StoreItem item, ItemProperty itemProperty)
        {
            item.FinalizePropertyAfterSave(itemProperty);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Ensures that any values that the server updated as a result of business rules
        /// (such as ID for new items, Opened Date, Changed Date, etc) are sync'd back to
        /// the in-memory cache.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void FinalizeAfterSave(StoreItem item)
        {
            item.PersistState = PersistStates.PersistedToStore;
            item.CommitErrorState = CommitErrorStates.NoError;
            item.ItemDocuments.NotifySaveCompleted();
            item.ItemImages.NotifySaveCompleted();
            item.AttachedFileNamesToCommit = null;
            item.AttachedFileNameToRemove = null;

            List<ItemProperty> properties = item.GetItemProperties();
            foreach (ItemProperty itemProperty in properties)
            {
                Globals.ItemManager.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => FinalizePropertyAfterSave(item, itemProperty)));
            }
        }

    }

}
