using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public partial class WorkItem
    {
        public static StoreItemCollection<WorkItem> Items = new StoreItemCollection<WorkItem>();
        protected override IStoreItemList GetCache() { return Items; }
    }

    public partial class BacklogItem
    {
        public static StoreItemCollection<BacklogItem> Items = new StoreItemCollection<BacklogItem>();
        protected override IStoreItemList GetCache() { return Items; }
    }

    public partial class GroupMemberItem
    {
        public static StoreItemCollection<GroupMemberItem> Items = new StoreItemCollection<GroupMemberItem>();
        protected override IStoreItemList GetCache() { return Items; }
    }

    public partial class OffTimeItem
    {
        public static StoreItemCollection<OffTimeItem> Items = new StoreItemCollection<OffTimeItem>();
        protected override IStoreItemList GetCache() { return Items; }
    }

    public partial class PillarItem
    {
        public static StoreItemCollection<PillarItem> Items = new StoreItemCollection<PillarItem>();
        protected override IStoreItemList GetCache() { return Items; }
    }

    public partial class ScrumTeamItem
    {
        public static StoreItemCollection<ScrumTeamItem> Items = new StoreItemCollection<ScrumTeamItem>();
        protected override IStoreItemList GetCache() { return Items; }
    }

}
