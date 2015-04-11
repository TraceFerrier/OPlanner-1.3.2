using System;
using System.Collections;
using System.ComponentModel;

namespace PlannerNameSpace
{
    class CommitmentNextTrainSort : LandingDateSort
    {
        public override int Compare(object x, object y)
        {
            BacklogItem backlogItemX = (BacklogItem)x;
            BacklogItem backlogItemY = (BacklogItem)y;
            bool isXEarlier = false;

            if (backlogItemX.TrainCommitmentNextTrainStatusText == PlannerContent.TrainCommitmentCarriedOverFromPreviousTrain)
            {
                if (backlogItemY.TrainCommitmentNextTrainStatusText != PlannerContent.TrainCommitmentCarriedOverFromPreviousTrain)
                {
                    isXEarlier = true;
                }
                else
                {
                    return base.Compare(x, y);
                }
            }
            else if (backlogItemX.TrainCommitmentNextTrainStatusText == PlannerContent.TrainCommitmentNewCommitment)
            {
                if (backlogItemY.TrainCommitmentNextTrainStatusText == PlannerContent.TrainCommitmentNewCommitment)
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
