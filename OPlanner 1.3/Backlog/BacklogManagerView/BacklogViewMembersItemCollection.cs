
namespace PlannerNameSpace
{
    public class BacklogViewMembersItemCollection : ViewItemCollection<GroupMemberItem>
    {
        protected BacklogViewState ViewState;

        public BacklogViewMembersItemCollection(BacklogViewState viewState)
        {
            ViewState = viewState;
            ViewState.ShowOnlyUnassignedMembersSettingChanged += ViewState_ShowOnlyUnassignedMembersSettingChanged;
            ViewState.PillarState.CurrentItemChanged += PillarState_CurrentItemChanged;
            ViewState.TrainState.CurrentItemChanged += TrainState_CurrentItemChanged;
        }

        void PillarState_CurrentItemChanged(object sender, System.EventArgs e)
        {
            RefreshCollection();
        }

        void TrainState_CurrentItemChanged(object sender, System.EventArgs e)
        {
            foreach (GroupMemberItem member in Items)
            {
                member.NotifyPropertyChanged(() => member.ActiveBacklogItems);
                member.NotifyPropertyChanged(() => member.CurrentTrainWorkRemaining);
            }
        }

        void ViewState_ShowOnlyUnassignedMembersSettingChanged(object sender, System.EventArgs e)
        {
            RefreshCollection();
        }

        public override bool ItemFilter(object viewObject)
        {
            GroupMemberItem memberItem = viewObject as GroupMemberItem;
            if (memberItem != null)
            {
                if (StoreItem.IsRealItem(ViewState.PillarState.CurrentItem))
                {
                    if (memberItem.ParentPillarItem != ViewState.PillarState.CurrentItem)
                    {
                        return false;
                    }

                    // Show only members that work items can be assigned to
                    if (memberItem.Discipline == DisciplineValues.PM)
                    {
                        return false;
                    }

                    if (ViewState.ShowOnlyUnassignedMembers && memberItem.CurrentTrainWorkRemaining > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
