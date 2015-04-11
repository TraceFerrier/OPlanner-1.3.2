using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public partial class BacklogItem
    {
        public static List<BacklogItem> GetPillarBacklogItems(PillarItem pillar)
        {
            List<BacklogItem> backlogItems = new List<BacklogItem>();
            foreach (BacklogItem backlogItem in BacklogItem.Items)
            {
                if (backlogItem.ParentPillarItem == pillar)
                {
                    backlogItems.Add(backlogItem);
                }
            }

            return backlogItems;
        }

        public static List<BacklogItem> GetBacklogItemsByPillarAndTrain(PillarItem pillar, TrainItem train)
        {
            List<BacklogItem> backlogItems = new List<BacklogItem>();
            foreach (BacklogItem backlogItem in BacklogItem.Items)
            {
                if (backlogItem.ParentPillarItem == pillar && backlogItem.ParentTrainItem == train)
                {
                    backlogItems.Add(backlogItem);
                }
            }

            return backlogItems;
        }


    }

}
