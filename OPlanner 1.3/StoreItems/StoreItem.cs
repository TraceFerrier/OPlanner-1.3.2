using ProductStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PlannerNameSpace
{
    public enum PersistStates
    {
        Dummy,
        NewUncommitted,
        NewCommitted,
        PersistedToStore,
    }

    public enum ChildState
    {
        NotApplicable,
        HasChildren,
        HasNoChildren,
    }

    public enum CommitErrorStates
    {
        NoError,
        MergingChanges,
        ErrorAccessingAttachmentShare,
    }

    //------------------------------------------------------------------------------------
    /// <summary>
    /// The base class for all items that are backed by a record in the backing store.
    /// </summary>
    //------------------------------------------------------------------------------------
    public abstract partial class StoreItem : BasePropertyChanged
    {
        protected int m_id;
        protected string m_title;
        
        private Dictionary<string, ItemProperty> ItemProperties;
        private AsyncObservableCollection<ItemProperty> m_changedProperties;
        private Dictionary<string, object> LocalProperties;
        private object SyncLockItemProperties;
        private object SyncLockLocalProperties;

        public DocumentAttachmentCollection ItemDocuments;
        public ImageAttachmentCollection ItemImages;
        public StoreItemCollection<StoreItem> SelfList { get; set; }

        public IList AttachedFileNamesToCommit;
        public string AttachedFileNameToRemove { get; set; }
        public StoreChangeAction ChangeAction { get; set; }
        public PersistStates PersistState { get; set; }

        public bool IsNew { get { return PersistState == PersistStates.NewUncommitted || PersistState == PersistStates.NewCommitted; } }
        public bool IsPersisted { get { return PersistState == PersistStates.PersistedToStore; } }
        public bool IsInImmediateSave { get; set; }
        public CommitErrorStates CommitErrorState { get; set; }

        public Datastore Store { get { return StoreID == null ? null : StoreID.Store; } }
        public DatastoreID StoreID { get; set; }
        public DatastoreItem DSItem;
        Guid UncommittedGuid { get; set; }
        public abstract ItemTypeID StoreItemType { get; }
        public abstract string DefaultItemPath { get; }
        public virtual bool IsGlobalItem { get { return false; } }

        public string StoreItemTypeName { get { return StoreItemType.ToString(); } }
        public string StoreChangeTypeName { get { return ChangeAction.ToString(); } }
        public const string FeatureTeamNotAllowedSelection = Globals.c_None;
        static BitmapSource GenericProfileSource = null;

        public List<ItemProperty> GetItemProperties()
        {
            return ItemProperties.ToList();
        }

        public StoreItem()
        {
            m_id = 0;
            SyncLockItemProperties = new object();
            SyncLockLocalProperties = new object();
            ItemProperties = new Dictionary<string, ItemProperty>();
            m_changedProperties = new AsyncObservableCollection<ItemProperty>();

            ItemDocuments = new DocumentAttachmentCollection(this);
            ItemImages = new ImageAttachmentCollection(this);
            SelfList = new StoreItemCollection<StoreItem>();
            SelfList.Add(this);

            AttachedFileNamesToCommit = null;

            UncommittedGuid = Guid.NewGuid();
            PersistState = PersistStates.Dummy;
            IsInImmediateSave = false;
            CommitErrorState = CommitErrorStates.NoError;
        }

        public AsyncObservableCollection<ItemProperty> ChangedProperties
        {
            get
            {
                return m_changedProperties;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if this property is in the collection of properties that have changed,
        /// and need to be saved to the server.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsPropertyOnChangeList(ItemProperty property)
        {
            return m_changedProperties.Contains(property);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the given property must be open to read (i.e. this may be an
        /// expensive operation in terms of time to retreive from the server).
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool MustPropertyBeOpenToRead(ItemProperty property)
        {
            if (Store == null)
            {
                return false;
            }

            return Store.MustBeOpenToRead(property.DSPropName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called after this item has been completely created or loaded, to allow each 
        /// object to do any custom finalization.  Derived classes should always call the base
        /// implementation before doing any other work.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected virtual void FinalizeItem()
        {
            //m_id = ID;
            //m_title = Title;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Confirms with the user whether the given item should be deleted, and if so,
        /// performs the deletion.
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual bool RequestDeleteItem(Window owner = null)
        {
            Window Owner = owner == null ? Globals.MainWindow : owner;
            RequestDeleteItemDialog dialog = new RequestDeleteItemDialog(Owner, this);
            dialog.ShowDialog();
            if (dialog.DialogConfirmed)
            {
                DeleteItem();
                return true;
            }

            return false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if there are any changes in this item that need to be persisted to
        /// the back-end store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsDirty
        {
            get
            {
                if (ItemDocuments.IsDirty || ItemImages.IsDirty || m_changedProperties.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public string Description
        {
            get { return GetBackgroundStringValue(Datastore.PropNameDescription); }
            set { SetStringValue(Datastore.PropNameDescription, value); }
        }

        public string ShortDescription
        {
            get
            {
                return Utils.GetStandardShortString(Description);
            }
        }

        public void SaveNewItem()
        {
            Globals.ItemManager.NewItemAccepted(this);
        }

        public void SaveAttachment(Attachment attachment)
        {
            Globals.ItemManager.SaveAttachment(this);
        }

        public void BeginSaveImmediate()
        {
            IsInImmediateSave = true;
        }

        public void CancelSaveImmediate()
        {
            IsInImmediateSave = false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Saves this item to the backing store immediately, without sending it to the change
        /// queue.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SaveImmediate()
        {
            Globals.ItemManager.SaveItemImmediate(this);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes this item from the in-memory cache, and sends it to the change queue to
        /// be deleted from the backing store during the next commit operation.
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual void DeleteItem()
        {
            Globals.ItemManager.DeleteItem(this);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Changes this item to the completed (i.e. Fixed) state in the in-memory cache, and
        /// sends it to the change queue to be saved to the backing store during the next 
        /// commit operation.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void FixItem()
        {
            Globals.ItemManager.FixItem(this);
        }

        public void CloseItem()
        {
            Globals.ItemManager.CloseItem(this);
        }

        public void ActivateItem()
        {
            Globals.ItemManager.ActivateItem(this);
        }

        public void ResolveItem(string resolution)
        {
            Globals.ItemManager.ResolveItem(this, resolution);
        }

        public override string ToString()
        {
            return StoreKey + ":" + m_title;
        }

        public virtual string PropToolTip
        {
            get { return "tooltip for this property"; }
        }

        public static string GetStoreNameFromKey(string key)
        {
            return key.Substring(0, key.IndexOf('|'));
        }

        public static int GetIDFromKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                int id;
                string strID = key.Substring(key.IndexOf('|') + 1);
                if (Int32.TryParse(strID, out id))
                {
                    return id;
                }
            }

            return 0;
        }

        public string StoreKey
        {
            get
            {
                if (PersistState == PersistStates.PersistedToStore)
                {
                    return GetItemKey(StoreID, ID);
                }
                else
                {
                    return UncommittedStoreKey;
                }
            }
        }

        public string UncommittedStoreKey
        {
            get { return UncommittedGuid.ToString(); }
        }

        public static string GetHostItemKey(int ID)
        {
            return GetItemKey(HostItemStore.Instance.StoreID, ID);
        }

        public static string GetItemKey(DatastoreID storeID, int ID)
        {
            return storeID.Name + "|" + ID.ToString();
        }


        public DateTime LastSavedTime { get; set; }

        public string IDQualifiedTitle
        {
            get { return ID.ToString() + ": " + Title; }
        }

        public bool IsAssignedToActive
        {
            get
            {
                return Utils.StringsMatch(AssignedTo, StatusValues.Active);
            }
        }

        string m_productTeamAssignedTo = null;
        public string ProductTeamAssignedTo
        {
            get
            {
                if (m_productTeamAssignedTo == null)
                {
                    m_productTeamAssignedTo = AssignedTo;
                    if (!IsAssignedToActive && Globals.GroupMemberManager.GetMemberByAlias(m_productTeamAssignedTo) == null)
                    {
                        m_productTeamAssignedTo = Globals.c_ExternalTeam;
                    }
                }

                return m_productTeamAssignedTo;
            }

            set
            {
                string proposedAssignedTo = value;
                if (proposedAssignedTo != Globals.c_ExternalTeam)
                {
                    AssignedTo = proposedAssignedTo;
                }
            }
        }

        public virtual void OnResolution()
        {

        }

        public string ResolvedBy
        {
            get { return GetStringValue(Datastore.PropNameResolvedBy); }
            set { SetStringValue(Datastore.PropNameResolvedBy, value); }
        }

        public bool SubtypeChanged
        {
            get
            {
                return PreviousSubtype != null && !Utils.StringsMatch(PreviousSubtype, Subtype);
            }
        }

        public string PreviousSubtype { get; set; }

        public virtual Brush ResolutionStatusColor
        {
            get
            {
                var brushConverter = new BrushConverter(); 
                switch (Status)
                {
                    case StatusValues.Active:
                        return (Brush)brushConverter.ConvertFrom(Globals.ActiveStatusColor);
                    case StatusValues.Resolved:
                        return (Brush)brushConverter.ConvertFrom(Globals.ResolvedStatusColor);
                    case StatusValues.Closed:
                        return (Brush)brushConverter.ConvertFrom(Globals.ClosedStatusColor);
                    default:
                        return Brushes.Red;
                }
            }
        }

        public static string GetEffectiveShipCycle(string shipCycle)
        {
            if (Utils.StringsMatch(shipCycle, "Gemini") ||
                Utils.StringsMatch(shipCycle, "Gemini - RTrel") ||
                Utils.StringsMatch(shipCycle, "Gemini - SPOrel"))
            {
                shipCycle = "Gemini";
            }
                
            return shipCycle;
        }

        public AsyncObservableCollection<AllowedValue> AvailableSubtypes
        {
            get { return PropertyAllowedValues.GetAvailableSubtypes(this); }
        }

        public AsyncObservableCollection<PersonaItem> AvailablePersonasAllowNone
        {
            get { return Globals.ItemManager.PersonaItems.GetItems(DummyItemType.NoneType); }
        }

        public AsyncObservableCollection<QuarterItem> AvailableQuarters
        {
            get
            {
                return Globals.ItemManager.QuarterItems;
            }
        }

        public AsyncObservableCollection<GroupMemberItem> AssignableGroupMembers
        {
            get { return Globals.GroupMemberManager.GetAssignableGroupMembers(); }
        }

        public AsyncObservableCollection<GroupMemberItem> AvailableDevMembers
        {
            get
            {
                return Globals.GroupMemberManager.GetDevMembers();
            }
        }

        public AsyncObservableCollection<GroupMemberItem> AvailableTestMembers
        {
            get
            {
                return Globals.GroupMemberManager.GetTestMembers();
            }
        }

        public AsyncObservableCollection<GroupMemberItem> AvailablePMMembers
        {
            get
            {
                return Globals.GroupMemberManager.GetPMMembers();
            }
        }

        public GroupMemberItem AssignedToGroupMember
        {
            get
            {
                return Globals.GroupMemberManager.GetMemberByAlias(AssignedTo);
            }

            set
            {
                AssignedTo = value.Alias;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of the given property as a document represented by a stream.
        /// </summary>
        //------------------------------------------------------------------------------------
        public MemoryStream GetDocumentValue(string propName)
        {
            return ItemDocuments.GetDocumentValue(propName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the value of the given property as a document represented by the given stream.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void SetDocumentValue(string propName, MemoryStream value)
        {
            ItemDocuments.SetAttachmentValue(propName, value);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of the given property as a document represented by a stream.
        /// Note: not used currently - see below.
        /// </summary>
        //------------------------------------------------------------------------------------
        public BitmapSource GetImageValue(string propName, [CallerMemberName] string publicPropName = null)
        {
            return ItemImages.GetFileAttachmentImageValue(propName, publicPropName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the value of the given property as a document represented by the given stream.
        /// Note: we no longer cache images from active directory, but this may be used in the
        /// future to allow setting of user custom images.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetImageValue(string propName, MemoryStream value)
        {
            ItemImages.SetAttachmentValue(propName, value);
        }

        public BitmapSource GetActiveDirectoryImageValue(string alias, [CallerMemberName] string publicPropName = null)
        {
            BitmapSource source = ItemImages.GetActiveDirectoryImageValue(alias, publicPropName);
            if (source == null)
            {
                if (GenericProfileSource == null)
                {
                    var hBitmap = Properties.Resources.GenericProfile.GetHbitmap();
                    GenericProfileSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }

                return GenericProfileSource;
            }

            return source;
        }

        public void LoadDocumentIntoRichTextBox(string propName, string plainPropName, RichTextBox richTextBox)
        {
            Globals.RichTextManager.LoadDocumentIntoRichTextBox(this, propName, plainPropName, richTextBox);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Extracts the current document from the given RichTextBox, and saves it in this
        /// item's document property bag, associated with the given propName.  In addition,
        /// the plain text from the box is also stored in the given plainPropName property.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string SaveDocumentFromRichTextBox(string propName, RichTextBox richTextBox)
        {
            TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            MemoryStream mStream = new MemoryStream();
            range.Save(mStream, DataFormats.Rtf);

            SetDocumentValue(propName, mStream);

            return range.Text;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Attaches the files referenced by the file names in the given list to this item,
        /// and persists the attachments.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void AttachFilesToItem(IList filenames)
        {
            BeginSaveImmediate();
            AttachedFileNamesToCommit = filenames;
            SaveImmediate();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Removes the attached file with the given name from this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void RemoveAttachedFileFromItem(Window owner, string filename)
        {
            if (UserMessage.ShowOkCancel(owner, "Are you sure you want to remove this file: " + filename + "?", "Remove AttachedFile"))
            {
                BeginSaveImmediate();
                AttachedFileNameToRemove = filename;
                SaveImmediate();
            }
        }

        AsyncObservableCollection<FileAttachment> m_attachedFiles;
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the file names of all the user-attached files for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<FileAttachment> AttachedFiles
        {
            get
            {
                if (m_attachedFiles == null)
                {
                    m_attachedFiles = Store.GetAttachedFiles(this);
                }

                return m_attachedFiles;
            }
        }

        public int AttachedFilesCount
        {
            get
            {
                return AttachedFiles.Count;
            }
        }

        public bool IsOneOrMoreFilesAttached
        {
            get
            {
                return AttachedFilesCount > 0;
            }
        }

        public void RefreshAttachedFileInfo()
        {
            m_attachedFiles = null;
            NotifyPropertyChanged(()=> AttachedFiles);
            NotifyPropertyChanged(() => AttachedFilesCount);
        }

        public string GetRefreshedBackingStringValue<T>(Expression<Func<T>> expression)
        {
            return (string)GetRefreshedBackingValue(expression);
        }

        public object GetRefreshedBackingValue<T>(Expression<Func<T>> expression)
        {
            string publicPropName = Utils.GetExpressionName(expression);

            if (ItemProperties.ContainsKey(publicPropName))
            {
                ItemProperty itemProperty = ItemProperties[publicPropName];
                itemProperty.ReadFromStore();
                return itemProperty.BackingValue;
            }

            return null;
        }

        public object GetPreviousValue<T>(Expression<Func<T>> expression)
        {
            string publicPropName = Utils.GetExpressionName(expression);

            if (ItemProperties.ContainsKey(publicPropName))
            {
                return ItemProperties[publicPropName].PreviousValue;
            }

            return null;
        }

        public string GetPreviousStringValue<T>(Expression<Func<T>> expression)
        {
            return Utils.GetStringValue(GetPreviousValue(expression));
        }

        public int GetPreviousIntValue<T>(Expression<Func<T>> expression)
        {
            return Utils.GetIntValue(GetPreviousValue(expression));
        }

        public string GetBackgroundStringValue(string propName, [CallerMemberName] String publicPropName = "")
        {
            //return GetStringValue(propName, publicPropName);
            return Utils.GetStringValue(GetBackgroundValue(propName, publicPropName));
        }

        public object GetBackgroundValue(string propName, [CallerMemberName] String publicPropName = "")
        {
            if (ItemProperties.ContainsKey(propName))
            {
                return ItemProperties[propName];
            }

            return Globals.StoreItemValueManager.GetBackgroundStringValue(this, propName, publicPropName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of all the allowed values for the given Product Studio field, given
        /// the current state of the given item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<AllowedValue> GetFieldAllowedValues(string propName)
        {
            return Store.GetFieldAllowedValues(this, propName);
        }

        public virtual ChildState GetChildState()
        {
            return ChildState.NotApplicable;
        }

        public virtual PillarItem GetParentPillarItem()
        {
            return null;
        }

        public virtual QuarterItem GetParentQuarterItem()
        {
            return null;
        }

        public virtual TrainItem GetParentTrainItem()
        {
            return null;
        }

        public virtual ScenarioItem GetParentScenarioItem()
        {
            return null;
        }

        public virtual PersonaItem GetParentPersonaItem()
        {
            return null;
        }

        public virtual ScrumTeamItem GetParentFeatureTeamItem()
        {
            return null;
        }

        public virtual string GetItemStatusText()
        {
            return Status;
        }

        public virtual CommitmentSettingValues GetCommitmentSetting()
        {
            return CommitmentSettingValues.Uncommitted;
        }

        public virtual int GetStoryPoints()
        {
            return 0;
        }

        public virtual TrainCommitmentStatusValue GetCommitmentStatus()
        {
            return TrainCommitmentStatusValue.NotCommitted;
        }

        public virtual string GetItemIssueType()
        {
            return null;
        }

        public virtual bool MatchesParentPillarItem(PillarItem pillarItem)
        {
            return GetParentPillarItem() == pillarItem;
        }

        public ContextMenu GetContextMenu(Window ownerWindow)
        {
            ContextMenu menu = new ContextMenu();
            PopulateContextMenu(ownerWindow, menu);

            if (Globals.ApplicationManager.IsCurrentUserOPlannerDev())
            {
                AddCopyIDMenu(menu);
            }
            else
            {
                switch (this.StoreItemType)
                {
                    case ItemTypeID.BacklogItem:
                    case ItemTypeID.WorkItem:
                        AddCopyIDMenu(menu);
                        break;
                }
            }

            return menu;
        }

        void AddCopyIDMenu(ContextMenu menu)
        {
            AddContextMenuItem(menu, "Copy ID", "Copy.png", CopyID_Click);
        }

        protected void CopyID_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(ID.ToString());
        }

        public virtual void PopulateContextMenu(Window ownerWindow, ContextMenu menu)
        {

        }

        protected void AddContextMenuItem(ContextMenu menu, string title, string imageName, RoutedEventHandler handler)
        {
            Utils.AddContextMenuItem(menu, title, imageName, handler);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of strings recapping the description change history for the given 
        /// bug.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string ItemDescriptionHistory
        {
            get
            {
                return Store.GetItemDescriptionHistory(this);
            }
        }

    }
}
