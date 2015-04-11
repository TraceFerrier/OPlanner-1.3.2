using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public enum CommitmentSettingValues
    {
        aaAllzz,
        aaExclude_Completed_Itemszz,
        Uncommitted,
        Committed,
        In_Progress,
        aaCommitted_or_In_Progresszz,
        aaCommitted_or_In_Progress_or_Completedzz,
        Completed,
    }

    public class CommitmentSettingItem : StoreItem
    {
        static List<CommitmentSettingItem> CommitmentSettingValueItems = null;
        public override ItemTypeID StoreItemType { get { return ItemTypeID.FilterStatus; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        public CommitmentSettingItem(CommitmentSettingValues value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Utils.EnumToString<CommitmentSettingValues>(Value);
        }

        public CommitmentSettingValues Value { get; set; }

        public static List<CommitmentSettingItem> CommitmentSettingValues
        {
            get
            {
                if (CommitmentSettingValueItems == null)
                {
                    CommitmentSettingValueItems = new List<CommitmentSettingItem>();
                    Array enumValues = typeof(CommitmentSettingValues).GetEnumValues();
                    foreach (CommitmentSettingValues value in enumValues)
                    {
                        CommitmentSettingValueItems.Add(new CommitmentSettingItem(value));
                    }
                }

                return CommitmentSettingValueItems;
            }
        }

        public static CommitmentSettingItem GetCommitmentSettingItem(string enumText)
        {
            CommitmentSettingValues enumValue = Utils.StringToEnum<CommitmentSettingValues>(enumText);
            return GetCommitmentSettingItem(enumValue);
        }

        public static CommitmentSettingItem GetCommitmentSettingItem(CommitmentSettingValues enumVal)
        {
            List<CommitmentSettingItem> valueItems = CommitmentSettingValues;
            foreach (CommitmentSettingItem item in valueItems)
            {
                if (item.Value == enumVal)
                {
                    return item;
                }
            }

            throw new ArgumentOutOfRangeException();
        }
    }

}
