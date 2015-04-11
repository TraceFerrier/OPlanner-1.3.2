using System;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public enum TrainTimeFrame
    {
        Past,
        Current,
        Future,
        CurrentOrFuture,
        CurrentOrPast,
        Unassigned,
    }

    public class TrainItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.Train; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }
        public override bool IsGlobalItem { get { return true; } }

        public TrainItem()
        {
        }

        static TrainItem s_backlogTrainItem;

        public static TrainItem BacklogTrainItem
        {
            get
            {
                if (s_backlogTrainItem == null)
                {
                    s_backlogTrainItem = new TrainItem();
                    s_backlogTrainItem.Title = Globals.OfficeBacklogFixBy;
                    s_backlogTrainItem.TrainFixBy = Globals.OfficeBacklogFixBy;
                    s_backlogTrainItem.TrainShipCycle = Globals.OfficeCurrentShipCycle;
                }

                return s_backlogTrainItem;
            }
        }

        public static void CreateTrainItem()
        {
            TrainItem newItem = ScheduleStore.Instance.CreateStoreItem<TrainItem>(ItemTypeID.Train);
            newItem.Title = "New Train";
            newItem.StartDate = DateTime.Today;
            newItem.EndDate = DateTime.Today;
            newItem.SaveNewItem();
        }

        public static bool operator >(TrainItem item1, TrainItem item2)
        {
            return item1.EndDate > item2.EndDate;
        }

        public static bool operator <(TrainItem item1, TrainItem item2)
        {
            return item1.EndDate < item2.EndDate;
        }

        public static bool operator >=(TrainItem item1, TrainItem item2)
        {
            return item1.EndDate >= item2.EndDate;
        }

        public static bool operator <=(TrainItem item1, TrainItem item2)
        {
            return item1.EndDate <= item2.EndDate;
        }

        public QuarterItem ParentQuarterItem
        {
            get
            {
                string key = ParentQuarterItemKey;
                QuarterItem quarterItem = Globals.ItemManager.GetItem<QuarterItem>(key);
                if (quarterItem == null)
                {
                    return StoreItem.GetDummyItem<QuarterItem>(DummyItemType.NoneType);
                }

                return quarterItem;
            }

            set
            {
                if (value == null || value.IsDummyItem)
                {
                    ParentQuarterItemKey = null;
                }
                else
                {
                    ParentQuarterItemKey = value.StoreKey;
                }

            }
        }

        public string ParentQuarterItemKey
        {
            get
            {
                return GetStringValue(Datastore.PropNameParentQuarterItemKey);
            }

            set
            {
                SetStringValue(Datastore.PropNameParentQuarterItemKey, value);
            }
        }

        public DateTime StartDate
        {
            get { return GetDateValue(Datastore.PropNameStartDate); }
            set { SetDateValue(Datastore.PropNameStartDate, value); }
        }

        public DateTime EndDate
        {
            get { return GetDateValue(Datastore.PropNameEndDate); }
            set { SetDateValue(Datastore.PropNameEndDate, value); }
        }

        public string ShortStartDate
        {
            get { return StartDate.ToShortDateString(); }
            set
            {
                DateTime date;
                if (DateTime.TryParse(value, out date))
                {
                    StartDate = date;
                }
            }
        }

        public string ShortEndDate
        {
            get { return EndDate.ToShortDateString(); }
            set
            {
                DateTime date;
                if (DateTime.TryParse(value, out date))
                {
                    EndDate = date;
                }
            }
        }

        public AsyncObservableCollection<string> AvailableShipCycles
        {
            get
            {
                AsyncObservableCollection<string> stringValues = new AsyncObservableCollection<string>();
                AsyncObservableCollection<AllowedValue> values = HostItemStore.Instance.GetFieldAllowedValues(Datastore.PropNameShipCycle);
                foreach (AllowedValue value in values)
                {
                    stringValues.Add(value.Value.ToString());
                }

                return stringValues;
            }
        }

        public AsyncObservableCollection<string> AvailableFixByValues
        {
            get
            {
                AsyncObservableCollection<string> stringValues = new AsyncObservableCollection<string>();
                stringValues.Add("QR1-01-Jan");
                stringValues.Add("QR1-02-Feb");
                stringValues.Add("QR1-03-Mar");
                stringValues.Add("QR2-04-Apr");
                stringValues.Add("QR2-05-May");
                stringValues.Add("QR2-06-Jun");
                stringValues.Add("QR3-07-Jul");
                stringValues.Add("QR3-08-Aug");
                stringValues.Add("QR3-09-Sep");
                stringValues.Add("QR4-10-Oct");
                stringValues.Add("QR4-11-Nov");
                stringValues.Add("QR4-12-Dec");
                stringValues.Add("QR5-01-Jan");
                stringValues.Add("QR5-02-Feb");
                stringValues.Add("QR5-03-Mar");
                stringValues.Add("QR6-04-Apr");
                stringValues.Add("QR6-05-May");
                stringValues.Add("QR6-06-Jun");
                stringValues.Add("QR7-07-Jul");
                stringValues.Add("QR7-08-Aug");
                stringValues.Add("QR7-09-Sep");
                stringValues.Add("QR8-10-Oct");
                stringValues.Add("QR8-11-Nov");
                stringValues.Add("QR8-12-Dec");
                stringValues.Add(Globals.OfficeBacklogFixBy);

                return stringValues;
            }
        }

        public string TrainShipCycle
        {
            get { return GetStringValue(Datastore.PropNameTrainHostShipCycle); }
            set { SetStringValue(Datastore.PropNameTrainHostShipCycle, value); }
        }

        public string TrainFixBy
        {
            get { return GetStringValue(Datastore.PropNameTrainHostFixBy); }
            set { SetStringValue(Datastore.PropNameTrainHostFixBy, value); }
        }

        public int WorkingHoursAvailable
        {
            get { return Utils.GetNetWorkingHours(StartDate, EndDate); }
        }

        public int WorkingDaysAvailable
        {
            get { return Utils.GetNetWorkingDays(StartDate, EndDate); }
        }

        public static string OfficeTrainNoun { get { return "Train"; } }

        public TrainTimeFrame TimeFrame
        {
            get
            {
                if (!StoreItem.IsRealItem(this))
                {
                    return TrainTimeFrame.Unassigned;
                }

                DateTime startDate = StartDate;
                DateTime endDate = EndDate;
                DateTime unassignedDate = new DateTime();

                if (startDate == unassignedDate || endDate == unassignedDate)
                {
                    return TrainTimeFrame.Unassigned;
                }
                else if (DateTime.Today > endDate)
                {
                    return TrainTimeFrame.Past;
                }
                else if (DateTime.Today >= startDate && DateTime.Today <= endDate)
                {
                    return TrainTimeFrame.Current;
                }
                else
                {
                    return TrainTimeFrame.Future;
                }
            }
        }
    }
}
