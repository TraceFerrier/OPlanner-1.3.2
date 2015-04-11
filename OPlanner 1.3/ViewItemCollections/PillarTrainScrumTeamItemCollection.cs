using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class PillarTrainScrumTeamItemCollection : ViewItemCollection<BacklogItem>
    {
        protected PillarTrainScrumTeamViewState ViewState;

        public PillarTrainScrumTeamItemCollection(PillarTrainScrumTeamViewState viewState)
        {
            ViewState = viewState;
            ViewState.ScrumTeamSelector.ScrumTeamItemSelectionChanged += ScrumTeamSelector_ScrumTeamItemSelectionChanged;
        }

        void ScrumTeamSelector_ScrumTeamItemSelectionChanged(object sender, ScrumTeamChangedEventArgs e)
        {
            RefreshCollection();
        }

        public override bool ItemFilter(object viewObject)
        {
            BacklogItem backlogItem = viewObject as BacklogItem;
            if (backlogItem != null)
            {
                if (!ViewState.PillarTrainItemFilter(backlogItem))
                {
                    return false;
                }

                ScrumTeamItem currentScrumTeam = ViewState.ScrumTeamSelector.CurrentScrumTeam;
                if (currentScrumTeam.IsAllItem)
                {
                    return true;
                }
                else if (currentScrumTeam.IsNoneItem)
                {
                    if (backlogItem.ScrumTeamItem != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if (backlogItem.ScrumTeamItem != currentScrumTeam)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
