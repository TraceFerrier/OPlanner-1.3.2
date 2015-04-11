using System.Windows.Controls;

namespace PlannerNameSpace
{
    public class BacklogViewItemCollection : ViewItemCollection<BacklogItem>
    {
        protected BacklogViewState ViewState;

        public BacklogViewItemCollection(ViewState viewState)
        {
            ViewState = (BacklogViewState) viewState;
            ViewState.CommitmentSettingChanged += ViewState_CommitmentSettingChanged;
            ViewState.PillarState.CurrentItemChanged += PillarState_CurrentItemChanged;
            ViewState.TrainState.CurrentItemChanged += TrainState_CurrentItemChanged;
        }

        void TrainState_CurrentItemChanged(object sender, System.EventArgs e)
        {
            RefreshCollection();
        }

        void PillarState_CurrentItemChanged(object sender, System.EventArgs e)
        {
            RefreshCollection();
        }

        void ViewState_CommitmentSettingChanged(object sender, System.EventArgs e)
        {
            RefreshCollection();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called whenever items in the backlog view need to be filtered.  Makes use of the
        /// various backlog filter combo settings to determine whether the given item should
        /// be excluded from the view.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override bool ItemFilter(object viewObject)
        {
            BacklogItem backlogItem = viewObject as BacklogItem;
            if (backlogItem != null)
            {
                switch (ViewState.ViewManager.CurrentView.ViewName)
                {
                    case AvailableViews.BacklogTrainReviewView:
                        return TrainReviewItemFilter(backlogItem);
                    default:
                        return StandardItemFilter(backlogItem);
                }
            }

            return false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Filter that determines whether the given backlog item should appear in the 
        /// standard backlog view.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool StandardItemFilter(BacklogItem backlogItem)
        {
            if (!ViewState.PillarTrainItemFilter(backlogItem))
            {
                return false;
            }

            if (ViewState.CurrentCommitmentSetting.Value == CommitmentSettingValues.aaExclude_Completed_Itemszz)
            {
                if (!backlogItem.IsActive)
                {
                    return false;
                }
            }
            else if (ViewState.CurrentCommitmentSetting.Value == CommitmentSettingValues.aaCommitted_or_In_Progresszz)
            {
                if (backlogItem.GetCommitmentSetting() != CommitmentSettingValues.Committed && backlogItem.GetCommitmentSetting() != CommitmentSettingValues.In_Progress)
                {
                    return false;
                }
            }
            else if (ViewState.CurrentCommitmentSetting.Value == CommitmentSettingValues.aaCommitted_or_In_Progress_or_Completedzz)
            {
                if (backlogItem.GetCommitmentSetting() != CommitmentSettingValues.Committed && 
                    backlogItem.GetCommitmentSetting() != CommitmentSettingValues.In_Progress &&
                    backlogItem.GetCommitmentSetting() != CommitmentSettingValues.Completed)
                {
                    return false;
                }
            }
            else if (ViewState.CurrentCommitmentSetting.Value == CommitmentSettingValues.Completed)
            {
                if (backlogItem.IsActive)
                {
                    return false;
                }
            }
            else if (ViewState.CurrentCommitmentSetting.Value != CommitmentSettingValues.aaAllzz)
            {
                if (backlogItem.GetCommitmentSetting() != ViewState.CurrentCommitmentSetting.Value || !backlogItem.IsActive)
                {
                    return false;
                }
            }

            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Filter that determines whether the given backlog item should appear in the 
        /// 'Train Review' backlog view.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool TrainReviewItemFilter(BacklogItem backlogItem)
        {
            if (!StoreItem.IsRealItem(ViewState.PillarState.CurrentItem) || !StoreItem.IsRealItem(ViewState.TrainState.CurrentItem))
            {
                return false;
            }


            if (backlogItem.ParentPillarItem != ViewState.PillarState.CurrentItem)
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


    }
}
