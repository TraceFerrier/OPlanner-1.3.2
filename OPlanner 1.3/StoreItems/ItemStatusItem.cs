using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public class BacklogItemStates
    {
        public const string All = "<All>";
        public const string AllExceptCompleted = "<Exclude Completed Items>";
        public const string NotStarted = "Not Started";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";
        public const string PastDue = "Past Due";
        public const string NotScheduled = "No Work Scheduled";
        public const string FutureWork = Globals.c_AssignedToFutureTrain;
        public const string OnTheBacklog = "On the Backlog";
        public const string PostponedToFutureTrain = "Postponed to a future train";
    }

    public class ItemStatusItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.FilterStatus; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        public string ItemStatus { get; set; }

        public ItemStatusItem(string itemStatus)
        {
            ItemStatus = itemStatus;
        }

        public static List<ItemStatusItem> ItemStateValues
        {
            get
            {
                List<ItemStatusItem> values = new List<ItemStatusItem>();
                values.Add(new ItemStatusItem(BacklogItemStates.All));
                values.Add(new ItemStatusItem(BacklogItemStates.AllExceptCompleted));
                AsyncObservableCollection<string> fsValues = Utils.GetEnumValues<CommitmentSettingValues>();
                foreach (string fsValue in fsValues)
                {
                    values.Add(new ItemStatusItem(fsValue));
                }

                //values.Add(BacklogItemStates.NotStarted);
                //values.Add(BacklogItemStates.InProgress);
                //values.Add(BacklogItemStates.Completed);
                //values.Add(BacklogItemStates.PastDue);
                //values.Add(BacklogItemStates.NotScheduled);
                //values.Add(BacklogItemStates.FutureWork);
                //values.Add(BacklogItemStates.PostponedToFutureTrain);
                //values.Add(BacklogItemStates.OnTheBacklog);

                return values;
            }
        }
    }
}
