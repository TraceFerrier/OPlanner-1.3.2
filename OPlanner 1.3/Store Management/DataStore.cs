using ProductStudio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PlannerNameSpace
{
    public enum StoreType
    {
        ScheduleStore,
        HostStore,
    }

    class ChangeTypes
    {
        public const string Opened = "Opened";
        public const string Edited = "Edited";
        public const string Activated = "Activated";
        public const string Resolved = "Resolved";
        public const string Closed = "Closed";
    }

    public enum StoreErrors
    {
        ProductStudioNotInstalled,
        ProductStudioNewerVersionRequired,
        UnrecognizedError,
    }

    public abstract partial class Datastore
    {
        public abstract string PropNameType { get; }
        protected abstract void InitializeProperties();

        // Standard properties
        public const string PropNameID = "ID";
        public const string PropNameTreeID = "TreeID";
        public const string PropNameTitle = "Title";
        public const string PropNameShipCycle = "Ship Cycle";
        public const string PropNameFixBy = "Fix By";
        public const string PropNameAssignedTo = "Assigned To";
        public const string PropNameStatus = "Status";
        public const string PropNameSeverity = "Severity";
        public const string PropNamePriority = "Priority";
        public const string PropNameResolvedBy = "Resolved By";
        public const string PropNameResolution = "Resolution";
        public const string PropNameSubtype = "Subtype";
        public const string PropNameDescription = "Repro Steps";
        public const string PropNameOpenedDate = "Opened Date";
        public const string PropNameOpenedBy = "Opened By";
        public const string PropNameChangedDate = "Changed Date";
        public const string PropNameChangedBy = "Changed By";
        public const string PropNameClosedBy = "Closed By";
        public const string PropNameClosedDate = "Closed Date";
        public const string PropNameBugComments = "Description";
        public const string PropNamePersonName = "Person Name";
        public const string PropNamePersonID = "PersonID";
        public const string PropNameRevisedDate = "Revised Date";
        public const string PropNameRev = "Rev";
        public const string PropNameLatestHistoryDescription = "Description";

        // ProductGroupItem
        public const string PropNameGroupPM = "Environment";
        public const string PropNameDevManager = "Features";
        public const string PropNameTestManager = "NotifyList";
        public const string PropNameProductGroupMembersLastUpdated = "Milestone";
        public const string PropNameProductGroupComposite = "Scenario"; // Capacity: 260
        public const string PropNameProductGroupData = "Repro Steps";

        // ForecastableItem
        public const string PropNameLandingDate = "Custom8";

        // BacklogItem
        public const string PropNamePM_Owner = "PM Owner";
        public const string PropNameTest_Owner = "Test Owner";
        public const string PropNameDev_Owner = "Dev Owner";
        public const string PropNameBusiness_Rank = "Business Rank";
        public const string PropNameStoryPoints = "Estimate";
        public const string PropNameCommittedToTrains = "Custom3";
        public const string PropNameCommittedToDates = "Custom4";
        public const string PropNameCommittedToApprovers = "Custom5";
        public const string PropNameBacklogScrumTeamKey = "Custom6"; 
        public const string PropNameBacklogCommitmentSetting = "Custom7"; 
        public const string PropNameIsPostMortemSpecAttached = "Custom10"; 
        public const string PropNameBacklogDesignStatus = "Custom11";
        public const string PropNameSpecdInBacklog = "Custom13";
        public const string PropNameParentScenarioItemKey = "Custom14";
        public const string PropNameParentSpec = "Parent Spec";
        public const string PropNameParentSpecStatus = "Spec Status";
        public const string PropNameParentSpecStatusComments = "Custom15";
        public const string PropNameParentSpecLink = "Test Notes";
        public const string PropNamePostMortemStatus = "Post Mortem Status";
        public const string PropNameBacklogSpecTeam = "Team";

        public const string PropNameDescriptionDocument = "Description";
        public const string PropNameAcceptanceCriteriaDocument = "AcceptanceCriteria";

        // WorkItem
        public const string PropNameOriginalEstimate = "Original Estimate";
        public const string PropNameEstimate = "Estimate";
        public const string PropNameCompleted = "Completed";
        public const string PropNameParentBacklogItemID = "Parent Feature Crew Id"; // Temp mapping
        public const string PropNameAlternateParentBacklogItemRef = "FeatureScrum";
        public const string PropNameSubStatus = "SubStatus";
        public const string PropNameWorkItemFeatureTeamMemberItemKey = "Custom7";
        public const string PropNameWorkItemWorkAssignedToKey = "Custom8";

        // Schedule properties
        public const string PropNameParentProductGroupKey = PropNameFixBy;

        // TrainCommitment
        public const string PropNameCommitmentBacklogItemKey = "NotifyList";
        public const string PropNameCommitmentTrainItemKey = "Features";
        public const string PropNameCommitmentSnapshotDate = "Environment";
        public const string PropNameCommitmentApprover = "Scenario";

        // TrainItem
        public const string PropNameStartDate = "Environment";
        public const string PropNameEndDate = "Features";
        public const string PropNameTrainHostShipCycle = PropNameFixBy;
        public const string PropNameTrainHostFixBy = "Milestone";
        public const string PropNameTrainHostSubFixBy = "Scenario";
        public const string PropNameParentQuarterItemKey = "NotifyList";

            // PillarItem
        public const string PropNamePillarTreeID1 = "Environment";
        public const string PropNamePillarTreeID2 = "Features";
        public const string PropNamePillarPMOwner = "NotifyList";
        public const string PropNamePillarTestOwner = "Scenario";
        public const string PropNamePillarDevOwner = "Milestone";

        // GroupMemberItem
        public const string PropNameGroupMemberAlias = PropNameTitle;
        public const string PropNameDiscipline = "NotifyList";
        public const string PropNameGroupMemberDisplayName = "Features";
        public const string PropNameJobTitlePillarAndAvgCapacity = "Environment";
        public const string PropNameOfficeName = "Milestone";
        public const string PropNameTelephone = "Scenario";
        public const string PropNameGroupMemberAvgCapacity = "Repro Steps";

        // ScrumTeamItem
        public const string PropNameParentPillarKey = "NotifyList";
        public const string PropNameParentTrainKey = "Features";
        public const string PropNameScrumMasterKey = "Environment";
        public const string PropNamePM1Key = "Milestone";
        public const string PropNamePM2Key = "Scenario";

        // HelpContentItem
        public const string PropNameHelpContentUserLog = "Repro Steps";
        public const string PropNameHelpContentHostProductTreeLastUpdateDate = "Environment";

        // Old Scenario
        public const string PropNameOldScenarioStatus = "Repro Steps";
        public const string PropNameOldParentExperienceItemKey = "Features";
        public const string PropNameOldScenarioParentPillarKey = "NotifyList";
        public const string PropNameOldScenarioParentQuarterItemKey = "Scenario";
        public const string PropNameOldScenarioBusinessRank = "Environment";

        // Scenario
        public const string PropNameScenarioDescription = "User Scenario";
        public const string PropNameParentExperienceItemKey = "Custom1";
        public const string PropNameScenarioParentPillarKey = "Custom2";
        public const string PropNameScenarioTestGrade = "Custom3";
        public const string PropNameScenarioDesignerGrade = "Custom4";
        public const string PropNameScenarioResearchGrade = "Custom5";
        public const string PropNameScenarioTestComments = "Explain Defect";
        public const string PropNameScenarioDesignerComments = "Explain Fix";
        public const string PropNameScenarioDesignerAlias = "Custom7";
        public const string PropNameScenarioResearcherAlias = "Custom8";
        public const string PropNameScenarioTestHoursAllocated = "Original Estimate";
        public const string PropNameScenarioDesignerHoursAllocated = "Estimate";
        public const string PropNameScenarioResearchHoursAllocated = "Completed";
        public const string PropNameScenarioBusinessRank = "Business Rank";

        // ExperienceItem
        public const string PropNameExperienceBusinessRank = "Scenario";
        public const string PropNameParentPillar1 = "Milestone";
        public const string PropNameParentPillar2 = "Environment";
        public const string PropNameParentPillar3 = "Features";
        public const string PropNameExperiencePersonaKey = "NotifyList";

        // PersonaItem
        public const string PropNamePersonaDescription = "Repro Steps";

        // OPlannerBugItem
        public const string PropNameBugAssignedTo = "NotifyList";
        public const string PropNameBugReproSteps = "Repro Steps";
        public const string PropNameBugIssueType = "Scenario";


        // OffTimeItem
        public const string PropNameOffTimeParentItemKey = "NotifyList";
        public const string PropNameOffTimeComment = "Milestone";
        public const string PropNameOffTimeStartDate = "Environment";
        public const string PropNameOffTimeEndDate = "Features";

        private DatastoreID m_storeID;
        public DatastoreID StoreID { get { return m_storeID; } }
        public StoreType StoreType { get { return m_storeID.StoreType; } }

        public bool IsLoaded { get; set; }
        private Stopwatch ConnectionTime { get; set; }
        private const int MaxConnectionMinutes = 45;
        public abstract string StoreName { get; }
        public abstract int TeamRootNode { get; }
        public abstract int TeamRootDepth { get; }

        private CompositeValueRegistry CompositeRegistry;

        public Datastore(StoreType storeType)
        {
            IsLoaded = false;
            ConnectionTime = new Stopwatch();
            m_storeID = new DatastoreID(StoreName, storeType);
            CompositeRegistry = new CompositeValueRegistry(this);

            InitializeProperties();
            RegisterCompositeValues(CompositeRegistry);
        }

        protected virtual void RegisterCompositeValues(CompositeValueRegistry registry) { }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the given query against this data store.  Returns false if a cancellation
        /// request is detected via the given taskWorker.
        /// </summary>
        //------------------------------------------------------------------------------------
        public BackgroundTaskResult ExecuteQuery(BaseQuery query, ShouldRefresh shouldRefresh, BackgroundTask taskWorker, bool deferItemCreation)
        {
            DatastoreItemList storeItemList = GetStoreItemList();
            FieldDefinitions fieldDefs = storeItemList.Datastore.FieldDefinitions;

            // Set up a query, using QueryDefinition to define the query XML
            ProductStudio.Query psQuery = new ProductStudio.Query();
            psQuery.CountOnly = false;
            psQuery.DatastoreItemType = PsDatastoreItemTypeEnum.psDatastoreItemTypeBugs;
            psQuery.SelectionCriteria = query.QueryXml;
            Tracer.WriteTrace("ExecuteQuery: " + psQuery.SelectionCriteria);

            psQuery.QueryFields.Clear();
            psQuery.QuerySortFields.Clear();
            psQuery.QuerySortFields.Add(fieldDefs["ID"], PsSortTypeEnum.psSortTypeDescending);

            // Execute the query
            try
            {
                storeItemList.Query = psQuery;
                storeItemList.Execute();
            }

            catch (Exception e)
            {
                return new BackgroundTaskResult { ResultType = ResultType.Failed, ResultMessage = e.Message };
            }

            Globals.ApplicationManager.WriteToEventLog(StoreName + ": Query results count: " + storeItemList.DatastoreItems.Count.ToString());

            int newItemCount = 0;
            if (shouldRefresh == ShouldRefresh.Yes)
            {
                Globals.ItemManager.ReceiveRefreshStoreItemsToDefer(this, storeItemList.DatastoreItems);
            }
            else if (deferItemCreation)
            {
                Globals.ItemManager.ReceiveStoreItemsToDefer(this, storeItemList.DatastoreItems);
            }
            else
            {
                foreach (DatastoreItem dsItem in storeItemList.DatastoreItems)
                {
                    if (taskWorker.CancellationPending)
                    {
                        return new BackgroundTaskResult { ResultType = ResultType.Cancelled };
                    }

                    StoreItem storeItem = CreateAndInitializeItemFromDS(dsItem);

                    // Every item loaded from the back-end store is placed into
                    // the global item cache for immediate access.
                    Globals.ItemManager.AddItemFromStore(storeItem);

                    newItemCount++;

                }
            }

            return new BackgroundTaskResult { ResultType = ResultType.Completed };

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Detects the item type represented by the given dsItem, and then creates a new
        /// StoreItem of the appropriate class, based on that dsItem.
        /// </summary>
        //------------------------------------------------------------------------------------
        public StoreItem CreateAndInitializeItemFromDS(DatastoreItem dsItem)
        {
            StoreItem storeItem = CreateItemOfTypeFromDS(dsItem);
            storeItem.PersistState = PersistStates.PersistedToStore;
            storeItem.DSItem = dsItem;
            storeItem.StoreID = StoreID;

            return storeItem;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Detects the item type represented by the given dsItem, and then creates a new
        /// StoreItem of the appropriate class, based on that dsItem.
        /// </summary>
        //------------------------------------------------------------------------------------
        private StoreItem CreateItemOfTypeFromDS(DatastoreItem dsItem)
        {
            ItemTypeID typeID = GetDSItemType(dsItem);
            switch (typeID)
            {
                case ItemTypeID.ProductGroup:
                    return new ProductGroupItem();
                case ItemTypeID.Train:
                    return new TrainItem();
                case ItemTypeID.ScrumTeam:
                    return new ScrumTeamItem();
                case ItemTypeID.Pillar:
                    return new PillarItem();
                case ItemTypeID.GroupMember:
                    return new GroupMemberItem();
                case ItemTypeID.BacklogItem:
                    return new BacklogItem();
                case ItemTypeID.WorkItem:
                    return new WorkItem();
                case ItemTypeID.OffTime:
                    return new OffTimeItem();
                case ItemTypeID.Scenario:
                    return new ScenarioItem();
                case ItemTypeID.Experience:
                    return new ExperienceItem();
                case ItemTypeID.Quarter:
                    return new QuarterItem();
                case ItemTypeID.Persona:
                    return new PersonaItem();
                case ItemTypeID.PlannerBug:
                    return new PlannerBugItem();
                case ItemTypeID.HelpContent:
                    return new HelpContentItem();
                default:
                    throw new ApplicationException("No CreateStoreItemOfTypeFromDS handler for the requested item type!");
            }
        }

        public bool OpenAndReadField(StoreItem item, string propName, out object value)
        {
            if (OpenForRead(item.DSItem))
            {
                value = item.DSItem.Fields[propName].Value;
                return true;
            }

            value = null;
            return false;
        }

        protected Dictionary<string, bool> ReadOnlyPropStates { get; set; }

        public bool IsPropertyReadOnly(string propName)
        {
            if (ReadOnlyPropStates == null)
            {
                ReadOnlyPropStates = new Dictionary<string,bool>();
            }

            if (!ReadOnlyPropStates.ContainsKey(propName))
            {
                bool isReadOnly = false;
                DatastoreItem dsItem = CreateDSItem();
                Field field = dsItem.Fields[propName];
                if (field != null && field.IsReadOnly)
                {
                    isReadOnly = true;
                }

                ReadOnlyPropStates.Add(propName, isReadOnly);
            }

            return ReadOnlyPropStates[propName];
        }

        // The ItemTypes dictionary contains the mappings between ItemTypeID identifiers
        // and the string value used to represent that type for a StoreItem in this
        // Datastore.
        protected Dictionary<string, ItemTypeID> ItemTypes { get; set; }

        protected void AddItemType(ItemTypeID typeID, string type)
        {
            type = type.ToLower();
            if (ItemTypes == null)
            {
                ItemTypes = new Dictionary<string, ItemTypeID>();
            }

            if (!ItemTypes.ContainsKey(type) && !ItemTypes.ContainsValue(typeID))
            {
                ItemTypes.Add(type, typeID);
            }
            else
            {
                UserMessage.ShowTwoLines("Duplicate type provided to AddItemType!", "ItemTypeID: " + typeID.ToString() + "; Type: " + type);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// returns the ItemTypeID that is represented by the given string, as defined for
        /// this Datastore.
        /// </summary>
        //------------------------------------------------------------------------------------
        public ItemTypeID GetItemTypeID(string itemType)
        {
            itemType = itemType.ToLower();
            if (ItemTypes.ContainsKey(itemType))
            {
                return ItemTypes[itemType];
            }

            return ItemTypeID.Unknown;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// returns the string used to represent the given ItemTypeID, as defined for this
        /// Datastore.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string GetItemTypeName(ItemTypeID typeID)
        {
            foreach (KeyValuePair<string, ItemTypeID> kvp in ItemTypes)
            {
                if (kvp.Value == typeID)
                {
                    return kvp.Key;
                }
            }

            return ItemTypeID.Unknown.ToString();
        }

        //------------------------------------------------------------------------------------
        private ItemTypeID GetDSItemType(DatastoreItem dsItem)
        {
            object typeValue = dsItem.Fields[PropNameType].Value;
            return GetItemTypeID(typeValue.ToString());
        }

         //------------------------------------------------------------------------------------
        /// <summary>
        /// Put item in edit mode such that changes can be saved back to the bug database.
        /// </summary>
        //------------------------------------------------------------------------------------
        private bool OpenForEdit(ProductStudio.DatastoreItem dsItem)
        {
            return OpenDSItem(dsItem, PsItemEditActionEnum.psDatastoreItemEditActionEdit);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Open item for read.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool OpenForRead(ProductStudio.DatastoreItem dsItem)
        {
            return OpenDSItem(dsItem, PsItemEditActionEnum.psDatastoreItemEditActionReadOnly);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Open item to resolve the bug.
        /// </summary>
        //------------------------------------------------------------------------------------
        private bool OpenForResolve(ProductStudio.DatastoreItem dsItem)
        {
            return OpenDSItem(dsItem, PsItemEditActionEnum.psBugEditActionResolve);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Open item to prepare for closure.
        /// </summary>
        //------------------------------------------------------------------------------------
        private bool OpenForClose(ProductStudio.DatastoreItem dsItem)
        {
            return OpenDSItem(dsItem, PsItemEditActionEnum.psBugEditActionClose);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Open item to prepare to activate.
        /// </summary>
        //------------------------------------------------------------------------------------
        private bool OpenForActivate(ProductStudio.DatastoreItem dsItem)
        {
            return OpenDSItem(dsItem, PsItemEditActionEnum.psBugEditActionActivate);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Open item for the given action.
        /// </summary>
        //------------------------------------------------------------------------------------
        private bool OpenDSItem(ProductStudio.DatastoreItem dsItem, PsItemEditActionEnum action)
        {
                dsItem.Edit(action, null, PsApplyRulesMask.psApplyRulesAll);
                return true;
        }


        private void ResetItem(StoreItem item)
        {
            ResetDSItem(item.DSItem);
        }

        private void ResetDSItem(DatastoreItem dsItem)
        {
            if (dsItem.ID > 0 && dsItem.IsOpenForEdit)
            {
                dsItem.Reset(true);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of all the allowed values for the given Product Studio field, given
        /// the current state of the given item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual AsyncObservableCollection<AllowedValue> GetFieldAllowedValues(StoreItem item, string propName)
        {
            DatastoreItem dsItem = null;
            if (item.DSItem == null)
            {
                if (item.StoreItemType == ItemTypeID.WorkItem)
                {
                    dsItem = GetDSItemOfType(ItemTypeID.WorkItem);
                }
                else if (item.StoreItemType == ItemTypeID.BacklogItem)
                {
                    dsItem = GetDSItemOfType(ItemTypeID.BacklogItem);
                }
                else
                {
                    dsItem = CreateDSItem();
                }
            }
            else
            {
                dsItem = item.DSItem;
            }

            OpenForRead(dsItem);
            return GetFieldAllowedValues(dsItem, propName);
        }

        private DatastoreItem GetDSItemOfType(ItemTypeID itemType)
        {
            DatastoreItem dsItem = CreateDSItem();
            string strItemType = GetItemTypeName(itemType);
            dsItem.Fields[PropNameType].Value = strItemType;
            return dsItem;
        }

        private Dictionary<string, AsyncObservableCollection<AllowedValue>> FieldAllowedValues = null;
        public virtual void SetFieldAllowedValues(string propName, AllowedValue value)
        {
            if (FieldAllowedValues == null)
            {
                FieldAllowedValues = new Dictionary<string, AsyncObservableCollection<AllowedValue>>();
            }

            if (!FieldAllowedValues.ContainsKey(propName))
            {
                FieldAllowedValues.Add(propName, new AsyncObservableCollection<AllowedValue>());
            }

            AsyncObservableCollection<AllowedValue> propValues = FieldAllowedValues[propName];
            propValues.Add(value);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of all the allowed values for the given Product Studio field.  This
        /// assumes that this property does not depend on the value of any other property for
        /// its state.
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual AsyncObservableCollection<AllowedValue> GetFieldAllowedValues(string propName, AllowedValue firstValue = null)
        {
            AsyncObservableCollection<AllowedValue> allowedValues = GetFieldAllowedValues(CreateDSItem(), propName);
            if (allowedValues.Count == 0)
            {
                if (FieldAllowedValues.ContainsKey(propName))
                {
                    allowedValues = FieldAllowedValues[propName];
                }
            }

            if (firstValue != null)
            {
                allowedValues.Insert(0, firstValue);
            }

            return allowedValues;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of all the allowed values for the given Product Studio field, given
        /// the current state of the given item.
        /// </summary>
        //------------------------------------------------------------------------------------
        private AsyncObservableCollection<AllowedValue> GetFieldAllowedValues(DatastoreItem dsItem, string propName)
        {
            AsyncObservableCollection<AllowedValue> valueList = new AsyncObservableCollection<AllowedValue>();

            if (dsItem != null)
            {
                ProductStudio.Field field = dsItem.Fields[propName];
                ProductStudio.Values values = field.ValidValues;
                foreach (object value in values)
                {
                    valueList.Add(new AllowedValue { Value = value });
                }

                ResetDSItem(dsItem);
            }

            return valueList;
        }

        public AsyncObservableCollection<AllowedValue> GetDependentFieldAllowedValues(string dependentProp, string dependsOnProp, object dependsOnValue)
        {
            DatastoreItem dsItem = CreateDSItem();
            SetFieldValue(dsItem, dependsOnProp, dependsOnValue);
            return GetFieldAllowedValues(dsItem, dependentProp);
        }

        public HashSet<string> GetFieldAllowedValuesSet(string propName)
        {
            HashSet<string> valueSet = new HashSet<string>();
            DatastoreItem dsItem = CreateDSItem();
            ProductStudio.Field field = dsItem.Fields[propName];
            ProductStudio.Values values = field.ValidValues;
            foreach (object value in values)
            {
                valueSet.Add(value as string);
            }

            return valueSet;
        }

        public abstract string DefaultTeamTreePath { get; }
        public abstract string DefaultMemberListTreePath { get; }
        public abstract string DefaultMilestoneTreePath { get; }
        public abstract string DefaultSprintTreePath { get; }
        public abstract string DefaultTaskFolderTreePath { get; }
        public abstract void InitializeRequiredFieldValues(StoreItem item);

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Pulls the DatastoreItem with the given ID from the backing store.
        /// </summary>
        //------------------------------------------------------------------------------------
        private DatastoreItem DSItemByID(int itemID)
        {
            DatastoreItemList storeItemList = GetStoreItemList(false);
            return storeItemList.Datastore.GetDatastoreItem(PsDatastoreItemTypeEnum.psDatastoreItemTypeBugs, itemID);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Pulls the StoreItem with the given store key from the backing store represented
        /// by that key.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static StoreItem GetStoreItem(string storeKey)
        {
            if (string.IsNullOrWhiteSpace(storeKey))
            {
                return null;
            }

            string store = StoreItem.GetStoreNameFromKey(storeKey);
            int id = StoreItem.GetIDFromKey(storeKey);

            Datastore datastore = null;
            if (!string.IsNullOrWhiteSpace(store) && id > 0)
            {
                if (Utils.StringsMatch(store, ScheduleStore.Instance.StoreName))
                {
                    datastore = ScheduleStore.Instance;
                }
                else if (Utils.StringsMatch(store, HostItemStore.Instance.StoreName))
                {
                    datastore = HostItemStore.Instance;
                }
            }

            if (datastore != null)
            {
                DatastoreItem dsItem = datastore.DSItemByID(id);
                if (dsItem != null)
                {
                    StoreItem item = datastore.CreateAndInitializeItemFromDS(dsItem);
                    item.ID = id;
                    return item;
                }
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a treeID for this product's feature tree, returns the full path for that
        /// tree node. 
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual string GetTreePath(int treeID, ProductTreeFormat treeFormat)
        {
            DatastoreItemList storeItemList = GetStoreItemList();
            if (storeItemList != null)
            {
                ProductStudio.PsCoreTreeTypeEnum treeType = ProductStudio.PsCoreTreeTypeEnum.psCoreTreeTypeProduct;
                ProductStudio.Node rootNode = storeItemList.Datastore.get_RootNodeEx(treeType);
                ProductStudio.Node currNode = rootNode.FindNodeInSubtree(treeID);

                List<string> nodeNames = new List<string>();
                while (currNode != null && currNode.ID > 1)
                {
                    nodeNames.Insert(0, currNode.Name);
                    currNode = currNode.Parent;
                }

                string fullPath = treeFormat == ProductTreeFormat.IncludeProduct ? StoreID.Name : "";
                foreach (string name in nodeNames)
                {
                    fullPath += "\\" + name;
                }

                return fullPath;
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// If an attached file exists in the given item with the filename given in the
        /// specified full path, this routine removes it.
        /// </summary
        //------------------------------------------------------------------------------------
        private void RemoveAttachedFile(DatastoreItem dsItem, string fullFilePath)
        {
            string fileName = Path.GetFileName(fullFilePath);

            if (dsItem != null)
            {
                ProductStudio.Files files = dsItem.Files;

                int indexToRemove = 0;
                foreach (ProductStudio.File file in files)
                {
                    if (file.FileName == fileName)
                    {
                        dsItem.Files.Remove(indexToRemove);
                        return;
                    }

                    indexToRemove++;
                }
            }
        }

        private void SaveFileAttachment(DatastoreItem dsItem, string fileName, MemoryStream stream)
        {
            string fullFilePath = Utils.GetFullPathToTempFile(fileName);
            using (FileStream fStream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(fStream);
                fStream.Close();
            }

            RemoveAttachedFile(dsItem, fullFilePath);
            dsItem.Files.AddEx(fullFilePath);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the file names of all the attached files for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<string> GetAttachedFileNames(StoreItem storeItem)
        {
            AsyncObservableCollection<string> filenames = new AsyncObservableCollection<string>();
            DatastoreItem dsItem = storeItem.DSItem;
            OpenForRead(dsItem);
            foreach (ProductStudio.File file in dsItem.Files)
            {
                // Exclude internal-use-only attached files.
                if (!IsInternalAttachedFile(file.FileName))
                {
                    filenames.Add(file.FileName);
                }
            }

            return filenames;
        }

        public AsyncObservableCollection<FileAttachment> GetAttachedFiles(StoreItem storeItem)
        {
            AsyncObservableCollection<FileAttachment> files = new AsyncObservableCollection<FileAttachment>();
            DatastoreItem dsItem = storeItem.DSItem;
            OpenForRead(dsItem);
            foreach (ProductStudio.File file in dsItem.Files)
            {
                if (!IsInternalAttachedFile(file.FileName))
                {
                    files.Add(new FileAttachment(file));
                }
            }

            return files;
        }

        public bool IsInternalAttachedFile(string filename)
        {
            if (Utils.StringsMatch(filename, PropNameDescriptionDocument + ".rtf") ||
            Utils.StringsMatch(filename, PropNameAcceptanceCriteriaDocument + ".rtf"))
            {
                return true;
            }

            return false;
        }

        public void QueueAttachedFileForRemoval(StoreItem item, string filename)
        {
            DatastoreItem dsItem = item.DSItem;
            int index = 0;
            foreach (ProductStudio.File file in dsItem.Files)
            {
                if (Utils.StringsMatch(filename, file.FileName))
                {
                    dsItem.Files.Remove(index);
                    break;
                }

                index++;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Opens the associated file using the OS shell.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ShowFile(StoreItem item, string filename)
        {
            DatastoreItem dsItem = item.DSItem;
            string tempFullPath = Path.Combine(Path.GetTempPath(), filename);

            try
            {
                OpenForRead(dsItem);
                foreach (ProductStudio.File file in dsItem.Files)
                {
                    if (Utils.StringsMatch(filename, file.FileName))
                    {
                        file.SaveToFile(tempFullPath, true);
                        if (System.IO.File.Exists(tempFullPath))
                        {
                            System.Diagnostics.Process.Start(tempFullPath);
                            break;
                        }
                    }
                }
            }

            catch (Exception e)
            {
                UserMessage.Show("Couldn't access the attached file: " + tempFullPath + " - " + e.Message);
            }
        }

        public void SaveRichTextFileAttachment(DatastoreItem dsItem, string propName, MemoryStream stream)
        {
            SaveFileAttachment(dsItem, propName + ".rtf", stream);
        }

        public void SaveImageFileAttachment(DatastoreItem dsItem, string propName, MemoryStream stream)
        {
            if (stream != null)
            {
                SaveFileAttachment(dsItem, propName + ".jpg", stream);
            }
        }

        public BitmapSource GetImageFileAttachment(StoreItem item, string propName)
        {
            string fullPath = GetFullPathToFileAttachment(item, propName + ".jpg");
            if (fullPath != null)
            {
                FileStream fStream = new FileStream(fullPath, FileMode.OpenOrCreate);
                return Utils.GetBitmapSourceFromStream(fStream);
            }

            return null;
        }

        public MemoryStream GetFileAttachmentStream(StoreItem item, string fileName)
        {
            MemoryStream mStream = null;
            string fullPath = GetFullPathToFileAttachment(item, fileName);
            if (fullPath != null)
            {
                FileStream fStream = new FileStream(fullPath, FileMode.OpenOrCreate);
                if (fStream != null)
                {
                    mStream = new MemoryStream();
                    fStream.CopyTo(mStream);
                    fStream.Close();
                }
            }

            return mStream;
        }

        public MemoryStream GetRichTextFileAttachmentStream(StoreItem item, string propName)
        {
            return GetFileAttachmentStream(item, propName + ".rtf");
        }

        public string GetFullPathToFileAttachment(StoreItem item, string fileName)
        {
            try
            {
                DatastoreItem dsItem = item.DSItem;
                OpenForRead(dsItem);
                ProductStudio.Files files = dsItem.Files;
                foreach (ProductStudio.File file in files)
                {
                    if (Utils.StringsMatch(fileName, file.FileName))
                    {
                        string tempFile = Utils.GetFullPathToTempFile(file.FileName);
                        file.SaveToFile(tempFile, true);
                        return tempFile;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                item.CommitErrorState = CommitErrorStates.ErrorAccessingAttachmentShare;
                Globals.ApplicationManager.HandleStoreItemException(item, e);
                return null;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the root node of the Product Tree for this product.
        /// </summary>
        //------------------------------------------------------------------------------------
        public Node GetProductTreeRootNode()
        {
            DatastoreItemList storeItemList = GetStoreItemList();
            if (storeItemList != null)
            {
                ProductStudio.PsCoreTreeTypeEnum treeType = ProductStudio.PsCoreTreeTypeEnum.psCoreTreeTypeProduct;
                return storeItemList.Datastore.get_RootNodeEx(treeType);
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the complete product tree for the current product. The root will have 
        /// parent id = -200. The returned tree is a depth-first representation of the product 
        /// hierarchy.
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual void PopulateTree(TreeView treeView, int startingTreeID)
        {
            DatastoreItemList storeItemList = GetStoreItemList();
            if (storeItemList != null)
            {
                ProductStudio.PsCoreTreeTypeEnum treeType = ProductStudio.PsCoreTreeTypeEnum.psCoreTreeTypeProduct;
                ProductStudio.Node rootNode = storeItemList.Datastore.get_RootNodeEx(treeType);

                ProductAreaTree tree = new ProductAreaTree(rootNode);
                tree.PopulateTree(treeView, startingTreeID);
            }
        }

        public List<int> GetTreeIDChildNodes(int treeID)
        {
            List<int> nodes = new List<int>();
            nodes.Add(treeID);
            DatastoreItemList storeItemList = GetStoreItemList();
            if (storeItemList != null)
            {
                ProductStudio.Node rootNode = storeItemList.Datastore.get_RootNodeEx(ProductStudio.PsCoreTreeTypeEnum.psCoreTreeTypeProduct);
                Node startingNode = rootNode.FindNodeInSubtree(treeID);
                if (startingNode != null)
                {
                    AddTreeIDChildNode(startingNode, nodes);
                }
            }

            return nodes;
        }

        private void AddTreeIDChildNode(Node node, List<int> nodeList)
        {
            foreach (Node childNode in node.Nodes)
            {
                nodeList.Add(childNode.ID);
                AddTreeIDChildNode(childNode, nodeList);
            }
        }

        public bool IsItemUnderTreeID(int treeID, int itemTreeID)
        {
            if (treeID == itemTreeID)
            {
                return true;
            }

            DatastoreItemList storeItemList = GetStoreItemList();
            if (storeItemList != null)
            {
                ProductStudio.Node rootNode = storeItemList.Datastore.get_RootNodeEx(ProductStudio.PsCoreTreeTypeEnum.psCoreTreeTypeProduct);
                Node startingNode = rootNode.FindNodeInSubtree(treeID);
                if (startingNode != null)
                {
                    return IsItemUnderTreeNode(startingNode, itemTreeID);
                }
            }

            return false;
        }

        private bool IsItemUnderTreeNode(Node node, int itemTreeID)
        {
            foreach (Node childNode in node.Nodes)
            {
                if (childNode.ID == itemTreeID)
                {
                    return true;
                }

                IsItemUnderTreeNode(childNode, itemTreeID);
            }

            return false;
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a path in this store's tree hierarchy, returns the treeID for that path.  If
        /// the path isn't found, zero will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public int GetTreePathID(string treePath)
        {
            if (treePath == null)
            {
                return 0;
            }

            DatastoreItemList storeItemList = GetStoreItemList();
            if (storeItemList == null)
            {
                return 0;
            }

            char[] trim = { '\\' };
            treePath = treePath.Trim(trim);
            ProductStudio.Node currentNode = storeItemList.Datastore.get_RootNodeEx(ProductStudio.PsCoreTreeTypeEnum.psCoreTreeTypeProduct);
            string[] pathNodes = treePath.Split('\\');
            foreach (string pathNode in pathNodes)
            {
                bool foundNode = false;
                foreach (Node node in currentNode.Nodes)
                {
                    if (node.Name == pathNode)
                    {
                        currentNode = node;
                        foundNode = true;
                        break;
                    }
                }

                if (!foundNode)
                {
                    return 0;
                }
            }

            return currentNode.ID;
        }

        protected FieldDefinitions FieldDefs;
        protected DatastoreItemList m_storeItemList;
        private DatastoreItemList GetStoreItemList(bool cacheStoreItemList = true)
        {
            try
            {
                if (m_storeItemList == null || ConnectionTime.Elapsed.Minutes > MaxConnectionMinutes)
                {
                    Tracer.WriteTrace("GetStoreItemList");
                    ConnectionTime.Reset();
                    ProductStudio.Directory psDir = new ProductStudio.Directory();
                    psDir.Connect();

                    Tracer.WriteTrace("psDir.Connect() completed");

                    ProductStudio.Product psProduct = psDir.GetProductByName(StoreID.Name);
                    DatastoreItemList storeItemList = new DatastoreItemList();
                    storeItemList.Datastore = psProduct.Connect();
                    Tracer.WriteTrace("psProduct.Connect() completed");

                    // Simulate outdated Product Studio error
                    //if (storeItemList.Datastore == null || storeItemList.Datastore != null)
                    //{
                    //    throw new Exception("[Product Studio] 80044005. Access to this product requires updated client components.");
                    //}

                    ConnectionTime.Start();

                    FieldDefs = storeItemList.Datastore.FieldDefinitions;

                    if (cacheStoreItemList)
                    {
                        m_storeItemList = storeItemList;
                    }
                    else
                    {
                        return storeItemList;
                    }

                }

                return m_storeItemList;
            }

            catch (Exception exception)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Properties.Resources.DatastoreUnableToConnectError + StoreID.Name);
                sb.AppendLine();
                sb.AppendLine(exception.Message);
                sb.AppendLine();
                Globals.ApplicationManager.WriteToEventLog(sb.ToString());
                Tracer.WriteTrace("GetStoreItemList exception: " + sb.ToString());
                Tracer.WriteTrace("Stack Trace: " + exception.StackTrace);

                throw new Exception(sb.ToString());
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Parses the given exception message to determine whether a Store error is
        /// recognized.  If not UnrecognizedError is returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static StoreErrors GetStoreErrorFromExceptionMessage(string message)
        {
            if (message.Contains("REGDB_E_CLASSNOTREG"))
            {
                return StoreErrors.ProductStudioNotInstalled;
            }
            else if (message.Contains("[Product Studio] 80044005"))
            {
                return StoreErrors.ProductStudioNewerVersionRequired;
            }

            return StoreErrors.UnrecognizedError;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of strings recapping the description change history for the given 
        /// bug.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string GetItemDescriptionHistory(StoreItem storeItem)
        {
            if (storeItem.IsNew)
            {
                return null;
            }

            DatastoreItem dsItem = storeItem.DSItem;
            OpenForRead(dsItem);
            StringBuilder historyBuilder = new StringBuilder(2048);

            if (dsItem != null)
            {
                historyBuilder.Append(String.Format("{0}\n{1}", dsItem.TagLine, dsItem.TagContent));
                ProductStudio.DatastoreItemHistory history = dsItem.History;

                for (int i = 0; i < history.Count; ++i)
                {
                    historyBuilder.Append(String.Format("\n{0}\n{1}", history[i].TagLine, history[i].TagContent));
                }
            }

            return historyBuilder.ToString();
        }


    }
}
