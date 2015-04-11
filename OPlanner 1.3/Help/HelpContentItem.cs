using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class HelpContentItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.HelpContent; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }
        public override bool IsGlobalItem { get { return true; } }

        public Nullable<DateTime> HostProductTreeLastUpdate
        {
            get 
            {
                long binaryDateTime = GetLongValue(Datastore.PropNameHelpContentHostProductTreeLastUpdateDate);
                if (binaryDateTime == 0)
                {
                    return null;
                }

                return DateTime.FromBinary(binaryDateTime);
            }

            set 
            {
                long binaryDateTime = 0;
                if (value != null)
                {
                    binaryDateTime = value.Value.ToBinary();
                }

                SetLongValue(Datastore.PropNameHelpContentHostProductTreeLastUpdateDate, binaryDateTime); 
            }
        }

    }
}
