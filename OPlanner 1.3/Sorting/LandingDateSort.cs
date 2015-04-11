using System;
using System.Collections;
using System.ComponentModel;

namespace PlannerNameSpace
{
    public class LandingDateSort : BaseSort
    {
        public override int Compare(object x, object y)
        {
            BacklogItem backlogItemX = (BacklogItem)x;
            BacklogItem backlogItemY = (BacklogItem)y;
            Nullable<DateTime> landingDateX = Utils.GetNullableDateTimeValue(backlogItemX.LandingDateText);
            Nullable<DateTime> landingDateY = Utils.GetNullableDateTimeValue(backlogItemY.LandingDateText);
            bool isXLater = false;


            if (backlogItemX.IsActive && !backlogItemY.IsActive)
            {
                isXLater = true;
            }
            else if (!backlogItemX.IsActive && backlogItemY.IsActive)
            {
                isXLater = false;
            }

            // Always treat non-date text as greater than a date value, so they'll sort to the end of the list
            else if (landingDateX == null && landingDateY == null)
            {
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
            }

            else if (landingDateX != null && landingDateY == null)
            {
                isXLater = false;
            }

            else if (landingDateX == null && landingDateY != null)
            {
                isXLater = true;
            }
            if (landingDateX != null && landingDateY != null)
            {
                if (landingDateX == landingDateY)
                {
                    return 0;
                }
                else
                {
                    isXLater = landingDateX.Value > landingDateY;
                }
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
