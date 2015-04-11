using ProductStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace PlannerNameSpace
{
    public abstract partial class Datastore 
    {
        public virtual T CreateStoreItem<T>(ItemTypeID typeID, ProductGroupItem productGroupItem = null) where T : StoreItem, new()
        {
            T newItem = (T)CreateItemOfType(typeID);

            newItem.StoreID = StoreID;
            newItem.Status = StatusValues.Active;
            newItem.AssignedTo = Globals.ApplicationManager.CurrentUserAlias;
            newItem.TreeID = GetTreePathID(newItem.DefaultItemPath);
            newItem.PersistState = PersistStates.NewUncommitted;
            InitializeRequiredFieldValues(newItem);

            return newItem;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Detects the item type represented by the given dsItem, and then creates a new
        /// StoreItem of the appropriate class, based on that dsItem.
        /// </summary>
        //------------------------------------------------------------------------------------
        private StoreItem CreateItemOfType(ItemTypeID typeID)
        {
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
                    throw new ApplicationException("No CreateStoreItemOfType handler for the requested item type!");
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Create a new blank bug - the fields can be filled out, and then saved back to PS.
        /// </summary>
        //------------------------------------------------------------------------------------
        public DatastoreItem CreateDSItem()
        {
            DatastoreItemList storeItemList = GetStoreItemList();
            storeItemList.CreateBlank(ProductStudio.PsDatastoreItemTypeEnum.psDatastoreItemTypeBugs);
            return storeItemList.DatastoreItems.Add(null, ProductStudio.PsApplyRulesMask.psApplyRulesAll);
        }
    }
}
