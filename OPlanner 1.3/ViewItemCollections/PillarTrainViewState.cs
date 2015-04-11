using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace PlannerNameSpace
{
    public class PillarTrainViewState : ViewState
    {
        public SelectorState<PillarItem> PillarState;
        public SelectorState<TrainItem> TrainState;

        public PillarTrainViewState(ComboBox pillarCombo, ProductPreferences pillarPreference, ComboBox trainCombo, ProductPreferences trainPreference)
        {
            PillarState = new SelectorState<PillarItem>(pillarCombo, pillarPreference);
            TrainState = new SelectorState<TrainItem>(trainCombo, trainPreference);
        }

        public PillarTrainViewState(ComboBox pillarCombo, PillarItem selectedPillar, ComboBox trainCombo, TrainItem selectedTrain)
        {
            PillarState = new SelectorState<PillarItem>(pillarCombo, selectedPillar);
            TrainState = new SelectorState<TrainItem>(trainCombo, selectedTrain);
        }

        public TrainItem NextTrain
        {
            get
            {
                return TrainManager.Instance.GetNextTrain(TrainState.CurrentItem);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Filter that determines whether the given backlog item should appear in the 
        /// standard backlog view, based on the pillar and train settings.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool PillarTrainItemFilter(BacklogItem backlogItem)
        {
            PillarItem filterPillar = PillarState.CurrentItem;
            TrainItem filterTrain = TrainState.CurrentItem;

            if (filterPillar.IsNoneItem)
            {
                if (!backlogItem.ParentPillarItem.IsNoneItem)
                {
                    return false;
                }
            }

            else if (!filterPillar.IsAllItem)
            {
                if (backlogItem.ParentPillarItem != PillarState.CurrentItem)
                {
                    return false;
                }
            }

            if (filterTrain.IsNoneItem)
            {
                if (!backlogItem.ParentTrainItem.IsNoneItem)
                {
                    return false;
                }
            }

            else if (!filterTrain.IsAllItem)
            {
                if (backlogItem.ParentTrainItem != TrainState.CurrentItem)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
