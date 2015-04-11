using System;
using System.Collections;
using System.ComponentModel;

namespace PlannerNameSpace
{
    public class BusinessRankSort : BaseSort
    {
        public override int Compare(object x, object y)
        {
            BacklogItem backlogItemX = (BacklogItem)x;
            BacklogItem backlogItemY = (BacklogItem)y;
            bool isXLater = false;

            // Any item assigned to a train ranks higher than an item 'on the backlog'
            if (StoreItem.IsRealItem(backlogItemX.ParentTrainItem) && !StoreItem.IsRealItem(backlogItemY.ParentTrainItem))
            {
                isXLater = false;
            }
            else if (!StoreItem.IsRealItem(backlogItemX.ParentTrainItem) && StoreItem.IsRealItem(backlogItemY.ParentTrainItem))
            {
                isXLater = true;
            }

            // The item assigned to the earlier train ranks earlier
            else if (backlogItemX.ParentTrainItem.EndDate > backlogItemY.ParentTrainItem.EndDate)
            {
                isXLater = true;
            }
            else if (backlogItemX.ParentTrainItem.EndDate < backlogItemY.ParentTrainItem.EndDate)
            {
                isXLater = false;
            }
            else
            {
                // If both items are assigned to the same train, then rank according to BusinessRank
                if (backlogItemX.BusinessRank == backlogItemY.BusinessRank)
                {
                    return 0;
                }

                isXLater = backlogItemX.BusinessRank > backlogItemY.BusinessRank;
            }

            if (m_direction == ListSortDirection.Ascending)
            {
                return isXLater ? 1 : -1;
            }
            else
            {
                return isXLater ? -1 : 1;
            }
        }
    }
}
