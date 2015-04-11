using System.Collections.Generic;

namespace PlannerNameSpace
{
    // Queries for all items of any of the given types AND (assigned to any of the given members, OR under any of the given tree path IDs)
    public class BurndownQuery : BaseQuery
    {
        List<ItemTypeID> TypeList;
        List<string> GroupMembers;

        public BurndownQuery(Datastore store, List<ItemTypeID> typeList, List<string> groupMemberAliases)
            : base(store)
        {
            TypeList = typeList;
            GroupMembers = groupMemberAliases;
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

            if (GroupMembers.Count > 0)
            {
                BeginOrGroup();
                foreach (string alias in GroupMembers)
                {
                    AddClause(Datastore.PropNameAssignedTo, "Equals", alias);
                    AddClause(Datastore.PropNameResolvedBy, "Equals", alias);
                }
            }

            EndGroup();

            EndGroup();
            EndQuery();
        }
    }
}
