using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace PlannerNameSpace
{
    // Queries for all items of any of the given types AND (assigned to any of the given members, OR under any of the given tree path IDs)
    public class HostItemQuery : BaseQuery
    {
        List<ItemTypeID> TypeList;
        List<string> GroupMembers;
        List<int> TreeIDs;
        AsyncObservableCollection<TrainItem> Trains;
        ShouldRefresh ShouldRefresh { get; set; }
        RefreshType RefreshType { get; set; }

        public HostItemQuery(Datastore store, List<ItemTypeID> typeList, List<string> groupMemberAliases, List<int> treeIDs, 
            AsyncObservableCollection<TrainItem> trains, ShouldRefresh isRefresh, RefreshType refreshType = RefreshType.QueryForChangedItems)
            : base(store)
        {
            TypeList = typeList;
            GroupMembers = groupMemberAliases;
            TreeIDs = treeIDs;
            Trains = trains;
            ShouldRefresh = isRefresh;
            RefreshType = refreshType;
        }

        protected override void BuildQueryXML()
        {
            BeginQuery();
            BeginAndGroup();
            
            BeginOrGroup();
            foreach (ItemTypeID itemType in TypeList)
            {
                AddClause(Store.PropNameType, "Equals", Store.GetItemTypeName(itemType));
            }
            EndGroup();

            if (Trains != null)
            {
                BeginOrGroup();
                foreach (TrainItem train in Trains)
                {
                    AddClause(Datastore.PropNameFixBy, "Equals", train.Title);
                }
                EndGroup();
            }


            if (GroupMembers.Count > 0)
            {
                BeginOrGroup();
                foreach (string alias in GroupMembers)
                {
                    AddClause(Datastore.PropNameAssignedTo, "Equals", alias);
                    AddClause(Datastore.PropNameResolvedBy, "Equals", alias);
                }

                if (TreeIDs != null && TreeIDs.Count > 0)
                {
                    foreach (int treeID in TreeIDs)
                    {
                        AddTreeIDUnderClause(treeID);
                    }
                }

                EndGroup();
            }

            // If this is a standard refresh, query only for items that have changed since
            // the last time the query was executed.
            if (!AddRefreshDateFilterClause(ShouldRefresh, RefreshType))
            {
                BeginOrGroup();
                AddClause(Datastore.PropNameStatus, "Equals", StatusValues.Active);
                AddClause("Resolution", "Equals", "Fixed");
                EndGroup();
            }

            EndGroup();
            EndQuery();
        }
    }
}
