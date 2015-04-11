using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public partial class OffTimeItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.OffTime; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        public string ParentItemKey
        {
            get 
            {
                return GetStringValue(Datastore.PropNameOffTimeParentItemKey); 
            }
            set 
            {
                SetStringValue(Datastore.PropNameOffTimeParentItemKey, value);
                NotifyPropertyChanged(() => ParentItem);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a string that represents the number of off days specified by the given
        /// collection of OffTimeItems, for the given train.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string TrainOffDays(TrainItem trainItem, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            if (trainItem != null && offTimeItems != null)
            {
                DateTime startDate = trainItem.StartDate;
                DateTime endDate = trainItem.EndDate;
                int netDaysOffThisTrain = Utils.GetNetOffDays(startDate, endDate, offTimeItems);
                return netDaysOffThisTrain.ToString() + " Days";
            }

            return "0 Days";
        }

        public DateTime StartDate
        {
            get { return GetDateValue(Datastore.PropNameOffTimeStartDate); }
            set { SetDateValue(Datastore.PropNameOffTimeStartDate, value); }
        }

        public DateTime EndDate
        {
            get { return GetDateValue(Datastore.PropNameOffTimeEndDate); }
            set { SetDateValue(Datastore.PropNameOffTimeEndDate, value); }
        }

        public string Comment
        {
            get { return GetStringValue(Datastore.PropNameOffTimeComment); }
            set { SetStringValue(Datastore.PropNameOffTimeComment, value); }
        }

    }
}
