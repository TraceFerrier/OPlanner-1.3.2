using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class UncommittedBacklogViewItemCollection : BacklogViewItemCollection
    {
        public UncommittedBacklogViewItemCollection(BacklogViewState viewState)
            : base(viewState)
        {

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

                if (!backlogItem.IsActive)
                {
                    return false;
                }
                else if (backlogItem.CommitmentSetting != CommitmentSettingValues.Uncommitted)
                {
                    return false;
                }
                else if (!backlogItem.IsSpecReadyForCoding)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
