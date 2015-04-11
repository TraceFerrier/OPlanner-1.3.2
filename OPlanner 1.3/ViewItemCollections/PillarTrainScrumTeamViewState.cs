using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace PlannerNameSpace
{
    public class PillarTrainScrumTeamViewState : PillarTrainViewState
    {
        public ScrumTeamSelector ScrumTeamSelector;

        public PillarTrainScrumTeamViewState(ComboBox pillarCombo, PillarItem selectedPillar, ComboBox trainCombo, TrainItem selectedTrain,
            ComboBox scrumTeamCombo, ScrumTeamItem selectedScrumTeam)
            : base(pillarCombo, selectedPillar, trainCombo, selectedTrain)
        {
            ScrumTeamSelector = new PlannerNameSpace.ScrumTeamSelector(scrumTeamCombo, selectedScrumTeam, PillarState);
        }
    }
}
