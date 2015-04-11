using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class StoreItemTypeSort
    {
        public static int Compare(StoreItem x, StoreItem y)
        {
            int rankX = GetItemTypeRank(x.StoreItemType);
            int rankY = GetItemTypeRank(y.StoreItemType);

            if (rankX == rankY)
            {
                return 0;
            }

            return rankX > rankY ? 1 : -1;
        }

        static int GetItemTypeRank(ItemTypeID storeItemType)
        {
            switch (storeItemType)
            {
                case ItemTypeID.GroupMember:
                    return 10;
                case ItemTypeID.Pillar:
                    return 20;
                case ItemTypeID.Experience:
                    return 30;
                case ItemTypeID.Scenario:
                    return 40;
                case ItemTypeID.BacklogItem:
                    return 50;
                case ItemTypeID.WorkItem:
                    return 60;
                default:
                    return 100;
            }
        }

    }
}
