using System.Collections.Generic;
using System;

namespace PlannerNameSpace
{
    public class ActiveItemTypeQuery : BaseQuery
    {
        List<ItemTypeID> TypeList;
        string m_productGroupKey;
        ShouldRefresh ShouldRefresh;
        RefreshType RefreshType;

        // If productGroupKey is non-empty, only those items that have their ProductGroupKey property equal
        // to this value will be returned.
        public ActiveItemTypeQuery(Datastore store, string productGroupKey, List<ItemTypeID> typeList, ShouldRefresh shouldRefresh, RefreshType refreshType = PlannerNameSpace.RefreshType.QueryForChangedItems)
            : base(store)
        {
            TypeList = typeList;
            m_productGroupKey = productGroupKey;
            ShouldRefresh = shouldRefresh;
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

            if (!string.IsNullOrEmpty(m_productGroupKey))
            {
                AddClause(Datastore.PropNameParentProductGroupKey, "Equals", m_productGroupKey);
            }

            if (!AddRefreshDateFilterClause(ShouldRefresh, RefreshType))
            {
                AddClause(Datastore.PropNameStatus, "Equals", StatusValues.Active);
            }
            
            EndGroup();
            EndQuery();
        }
    }
}
