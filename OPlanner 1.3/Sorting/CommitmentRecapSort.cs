using System;
using System.Collections;
using System.ComponentModel;

namespace PlannerNameSpace
{
    class CommitmentRecapSort : LandingDateSort
    {
        public override int Compare(object x, object y)
        {
            BacklogItem backlogItemX = (BacklogItem)x;
            BacklogItem backlogItemY = (BacklogItem)y;
            bool isXEarlier = false;

            if (backlogItemX.TrainCommitmentRecapStatusText == PlannerContent.TrainCommitmentCompleted)
            {
                if (backlogItemY.TrainCommitmentRecapStatusText != PlannerContent.TrainCommitmentCompleted)
                {
                    isXEarlier = true;
                }
                else
                {
                    return base.Compare(x, y);
                }
            }
            else if (backlogItemX.TrainCommitmentRecapStatusText == PlannerContent.RecapTrainCommitmentChangedToLaterTrain)
            {
                if (backlogItemY.TrainCommitmentRecapStatusText == PlannerContent.RecapTrainCommitmentChangedToLaterTrain)
                {
                    return base.Compare(x, y);
                }
            }
            else
            {
                return base.Compare(x, y);
            }

            if (m_direction == ListSortDirection.Ascending)
            {
                return isXEarlier ? -1 : 1;
            }
            else
            {
                return isXEarlier ? 1 : -1;
            }
        }
    }
}
