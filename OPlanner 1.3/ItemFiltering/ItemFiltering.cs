using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public partial class FilteredCollectionManager
    {
        ItemsByParent<ScenarioItem> PillarScenarioItems;

        void InitializeItemFilters()
        {
            PillarScenarioItems = new ItemsByParent<ScenarioItem>();
        }

        void ScenarioItemAdded(ScenarioItem scenarioItem)
        {
            PillarScenarioItems.AddItemByParent(scenarioItem, scenarioItem.ParentPillarItemKey);
        }

        void ScenarioItemRemoved(ScenarioItem scenarioItem)
        {
            PillarScenarioItems.RemoveItemByParent(scenarioItem, scenarioItem.ParentPillarItemKey);
        }
    }
}
