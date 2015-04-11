using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class ReviewPagesViewItemCollection : ViewItemCollection<BacklogItem>
    {
        ReviewPagesViewState ViewState;

        public ReviewPagesViewItemCollection(ReviewPagesViewState viewState)
        {
            ViewState = viewState;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The standard ItemFilter for this collection.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override bool ItemFilter(object viewObject)
        {
            BacklogItem backlogItem = viewObject as BacklogItem;
            if (ViewState.CurrentReviewPage == AvailableViews.BacklogLastTrainResultRecap)
            {
                return ThisTrainResultsFilter(backlogItem);
            }
            else if (ViewState.CurrentReviewPage == AvailableViews.BacklogThisTrainCommitments)
            {
                return NextTrainCommitmentsFilter(backlogItem);
            }
            else if (ViewState.CurrentReviewPage == AvailableViews.BacklogThisTrainNonCommitments)
            {
                return NextTrainNonCommitmentsFilter(backlogItem);
            }

            return false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the given item represents an item that was committed to the 
        /// currently selected pillar and train.
        /// </summary>
        //------------------------------------------------------------------------------------
        bool ThisTrainResultsFilter(BacklogItem backlogItem)
        {
            if (!IsSelectedPillar(backlogItem))
            {
                return false;
            }

            if (!backlogItem.IsCommittedToTrain(ViewState.TrainState.CurrentItem))
            {
                return false;
            }

            backlogItem.NotifyPropertyChanged(() => backlogItem.TrainCommitmentStatusText);
            backlogItem.NotifyPropertyChanged(() => backlogItem.TrainCommitmentStatusColor);

            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if this item is in the "committed" state for the selected pillar, and
        /// for the train following the currently selected train.
        /// </summary>
        //------------------------------------------------------------------------------------
        bool NextTrainCommitmentsFilter(BacklogItem backlogItem)
        {
            if (!IsNextTrainAndPillar(backlogItem))
            {
                return false;
            }

            if (backlogItem.CommitmentSetting == CommitmentSettingValues.Uncommitted)
            {
                return false;
            }

            backlogItem.NotifyPropertyChanged(() => backlogItem.TrainCommitmentStatusText);
            backlogItem.NotifyPropertyChanged(() => backlogItem.TrainCommitmentStatusColor);
            
            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if this item is in the "uncommitted" state for the selected pillar, 
        /// and for the train following the currently selected train.
        /// </summary>
        //------------------------------------------------------------------------------------
        bool NextTrainNonCommitmentsFilter(BacklogItem backlogItem)
        {
            if (!IsNextTrainAndPillar(backlogItem))
            {
                return false;
            }

            if (backlogItem.CommitmentSetting != CommitmentSettingValues.Uncommitted)
            {
                return false;
            }

            backlogItem.NotifyPropertyChanged(() => backlogItem.TrainCommitmentStatusText);
            backlogItem.NotifyPropertyChanged(() => backlogItem.TrainCommitmentStatusColor);

            return true;
        }

        bool IsThisTrainAndPillar(BacklogItem backlogItem)
        {
            if (!IsSelectedPillar(backlogItem))
            {
                return false;
            }

            return IsGivenTrain(backlogItem, ViewState.TrainState.CurrentItem);
        }

        bool IsNextTrainAndPillar(BacklogItem backlogItem)
        {
            if (!IsSelectedPillar(backlogItem))
            {
                return false;
            }

            return IsGivenTrain(backlogItem, ViewState.NextTrain);
        }

        bool IsSelectedPillar(BacklogItem backlogItem)
        {
            // If this product group has no pillars defined, then treat all
            // items as belonging to the same 'group' pillar.
            if (PillarManager.PillarItems.Count == 0)
            {
                return true;
            }

            if (!StoreItem.IsRealItem(ViewState.PillarState.CurrentItem))
            {
                return false;
            }

            if (backlogItem.ParentPillarItem == null || backlogItem.ParentPillarItem != ViewState.PillarState.CurrentItem)
            {
                return false;
            }

            return true;
        }

        bool IsGivenTrain(BacklogItem backlogItem, TrainItem train)
        {
            if (!StoreItem.IsRealItem(train))
            {
                return false;
            }

            string backlogTitle = backlogItem.Title;
            string backlogTrain = backlogItem.ParentTrainItem == null ? null : backlogItem.ParentTrainItem.Title;

            if (backlogItem.ParentTrainItem == null || backlogItem.ParentTrainItem != train)
            {
                return false;
            }

            return true;
        }
    }
}
