using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public class OPlannerBugsDataSource
    {
        public AsyncObservableCollection<string> GetAllBugAssignedToValues()
        {
            AsyncObservableCollection<string> assignedToValues = new AsyncObservableCollection<string>();
            assignedToValues.Add(Globals.c_All);

            AsyncObservableCollection<PlannerBugItem> bugs = Globals.ItemManager.PlannerBugItems;
            foreach (PlannerBugItem bug in bugs)
            {
                if (!assignedToValues.Contains(bug.BugAssignedTo))
                {
                    assignedToValues.Add(bug.BugAssignedTo);
                }
            }

            return assignedToValues;
        }
    }
}
