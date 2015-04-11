using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class QuarterItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.Quarter; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }
        public override bool IsGlobalItem { get { return true; } }

        public static void CreateQuarterItem(string title)
        {
            QuarterItem newItem = ScheduleStore.Instance.CreateStoreItem<QuarterItem>(ItemTypeID.Quarter);
            newItem.Title = title;
            newItem.SaveNewItem();
        }

        public AsyncObservableCollection<TrainItem> GetTrains()
        {
            AsyncObservableCollection<TrainItem> quarterTrains = new AsyncObservableCollection<TrainItem>();
            AsyncObservableCollection<TrainItem> trains = TrainManager.TrainItems;
            foreach (TrainItem train in trains)
            {
                if (train.ParentQuarterItem == this)
                {
                    quarterTrains.Add(train);
                }
            }

            return quarterTrains;
        }

        public TrainTimeFrame TimeFrame
        {
            get
            {
                int pastTrains = 0;
                int currentTrains = 0;
                int futureTrains = 0;
                AsyncObservableCollection<TrainItem> quarterTrains = GetTrains();
                foreach (TrainItem train in quarterTrains)
                {
                    if (train.TimeFrame == TrainTimeFrame.Past)
                    {
                        pastTrains++;
                    }
                    else if (train.TimeFrame == TrainTimeFrame.Current)
                    {
                        currentTrains++;
                    }
                    else
                    {
                        futureTrains++;
                    }
                }

                if (currentTrains > 0)
                {
                    return TrainTimeFrame.Current;
                }
                else if (pastTrains > 0)
                {
                    return TrainTimeFrame.Past;
                }
                else
                {
                    return TrainTimeFrame.Future;
                }
            }
        }
    }
}
