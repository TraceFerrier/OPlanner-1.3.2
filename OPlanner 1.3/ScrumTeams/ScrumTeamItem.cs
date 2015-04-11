using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PlannerNameSpace
{
    public enum ScrumTeamStatus
    {
        On_Track,
        Not_On_Track,
        Not_Scheduled,
    }

    public partial class ScrumTeamItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.ScrumTeam; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        public ScrumTeamItem ParentPromotionItem { get; set; }

        public override string ToString()
        {
            return QualifiedTitle;
        }

        public string DisplayName
        {
            get { return Title; }
            set { Title = value; }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the PillarItem that this team is assigned to.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override PillarItem GetParentPillarItem()
        {
            return ParentPillarItem;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a count of the total members assigned to this team.
        /// </summary>
        //------------------------------------------------------------------------------------
        public int MemberCount
        {
            get
            {
                return Members.Count;
            }
        }

        public ScrumTeamStatus ScrumTeamStatus
        {
            get
            {
                return PlannerNameSpace.ScrumTeamStatus.On_Track;
            }
        }

        public string FeatureTeamStatusText
        {
            get
            {
                return Utils.EnumToString<ScrumTeamStatus>(ScrumTeamStatus);
            }
        }

        public Brush FeatureTeamStatusColor
        {
            get
            {
                switch(ScrumTeamStatus)
                {
                    case PlannerNameSpace.ScrumTeamStatus.On_Track:
                        return Brushes.LightGreen;
                    case PlannerNameSpace.ScrumTeamStatus.Not_On_Track:
                        return Brushes.Red;
                    case PlannerNameSpace.ScrumTeamStatus.Not_Scheduled:
                        return Brushes.Yellow;
                    default:
                        return Brushes.Red;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the total number of work hours remaining for all the members of this team.
        /// </summary>
        //------------------------------------------------------------------------------------
        public int TotalWorkRemaining
        {
            get
            {
                int remaining = 0;
                AsyncObservableCollection<GroupMemberItem> members = Members;
                foreach (GroupMemberItem member in members)
                {
                    remaining += 0;// member.WorkHoursRemaining;
                }

                return remaining;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the total number of work hours that have been completed for all the 
        /// members of this team.
        /// </summary>
        //------------------------------------------------------------------------------------
        public int TotalWorkCompleted
        {
            get
            {
                int completed = 0;
                AsyncObservableCollection<GroupMemberItem> members = Members;
                foreach (GroupMemberItem member in members)
                {
                    completed += member.TotalWorkCompleted;
                }

                return completed;
            }
        }

        public bool Equals(ScrumTeamItem featureTeamItem)
        {
            if (featureTeamItem != null && Utils.StringsMatch(StoreKey, featureTeamItem.StoreKey))
            {
                return true;
            }

            return false;
        }

        public static ScrumTeamItem GetDummyNoneTeam()
        {
            return StoreItem.GetDummyItem<ScrumTeamItem>(DummyItemType.NoneType);
        }

        public static ScrumTeamItem GetDummyAllTeam()
        {
            return StoreItem.GetDummyItem<ScrumTeamItem>(DummyItemType.AllType);
        }

        public string QualifiedTitle
        {
            get
            {
                if (ParentPillarItem != null)
                {
                    return ParentPillarItem.Title + " - " + Title;
                }

                return Title;
            }
        }

        public double DevWorkItemDeviation { get; set; }
        public double TestWorkItemDeviation { get; set; }

        public PillarItem ParentPillarItem
        {
            get
            {
                return Globals.ItemManager.GetItem<PillarItem>(ParentPillarKey);
            }

            set
            {
                ParentPillarKey = value.StoreKey;
            }
        }

        public GroupMemberItem ScrumMasterItem
        {
            get
            {
                // DesignChecker: warning if this backlog item does not fall under any pillar
                return Globals.ItemManager.GetItem<GroupMemberItem>(ScrumMasterMemberKey);
            }

            set
            {
                ScrumMasterMemberKey = value.StoreKey;
                NotifyPropertyChanged(() => ScrumMasterImage);
            }
        }

        public BitmapSource ScrumMasterImage
        {
            get
            {
                if (ScrumMasterItem != null)
                {
                    return ScrumMasterItem.UserPicture;
                }

                return null;
            }
        }

        public string PillarName
        {
            get { return ParentPillarItem == null ? Globals.c_None : ParentPillarItem.Title; }
            set { ; }
        }

        public string ScrumMasterDisplayName
        {
            get { return ScrumMasterItem == null ? Globals.c_None : ScrumMasterItem.DisplayName; }
            set { ; }
        }

        public string ParentPillarKey
        {
            get { return GetStringValue(Datastore.PropNameParentPillarKey); }
            set 
            {
                SetStringValue(Datastore.PropNameParentPillarKey, value);
                NotifyPropertyChanged(() => ParentPillarItem);
                NotifyPropertyChanged(() => QualifiedTitle);
                NotifyPropertyChanged(() => PillarName);
            }
        }

        public string ParentTrainKey
        {
            get { return GetStringValue(Datastore.PropNameParentTrainKey); }
            set { SetStringValue(Datastore.PropNameParentTrainKey, value); }
        }

        public string ScrumMasterMemberKey
        {
            get { return GetStringValue(Datastore.PropNameScrumMasterKey); }
            set { SetStringValue(Datastore.PropNameScrumMasterKey, value); }
        }

        public GroupMemberItem ProductOwner1Item
        {
            get
            {
                return Globals.ItemManager.GetItem<GroupMemberItem>(PM1MemberKey);
            }

            set
            {
                PM1MemberKey = value != null ? value.StoreKey : null;
                NotifyPropertyChanged(() => ProductOwner1Image);
            }
        }

        public BitmapSource ProductOwner1Image
        {
            get
            {
                if (ProductOwner1Item != null)
                {
                    return ProductOwner1Item.UserPicture;
                }

                return null;
            }
        }

        public string PM1MemberKey
        {
            get { return GetStringValue(Datastore.PropNamePM1Key); }
            set { SetStringValue(Datastore.PropNamePM1Key, value); }
        }

        public GroupMemberItem ProductOwner2Item
        {
            get
            {
                return Globals.ItemManager.GetItem<GroupMemberItem>(PM2MemberKey);
            }

            set
            {
                PM2MemberKey = value != null ? value.StoreKey : null;
                NotifyPropertyChanged(() => ProductOwner2Image);
            }
        }

        public BitmapSource ProductOwner2Image
        {
            get
            {
                if (ProductOwner2Item != null)
                {
                    return ProductOwner2Item.UserPicture;
                }

                return null;
            }
        }

        public string PM2MemberKey
        {
            get { return GetStringValue(Datastore.PropNamePM2Key); }
            set { SetStringValue(Datastore.PropNamePM2Key, value); }
        }

    }
}
