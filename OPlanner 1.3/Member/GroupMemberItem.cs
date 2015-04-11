using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public delegate void PlannerPropertyChangedEventHandler();

    public partial class GroupMemberItem : StoreItem
    {
        Nullable<DateTime> m_latestLandingDate = null;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Represents the current latest landing date for all backlog or work items that
        /// this member is assigned to.
        /// </summary>
        //------------------------------------------------------------------------------------
        public Nullable<DateTime> LatestLandingDate
        {
            get { return m_latestLandingDate; }
            set { m_latestLandingDate = value; }
        }

        public BitmapSource UserPicture
        {
            get
            {
                return GetActiveDirectoryImageValue(Alias);
            }
        }

        public override ItemTypeID StoreItemType { get { return ItemTypeID.GroupMember; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        public void InitializeWithUserInformation(UserInformation userInfo)
        {
            DisplayName = userInfo.DisplayName;
            OfficeName = userInfo.OfficeName;
            JobTitle = userInfo.JobTitle;
            Telephone = userInfo.Telephone;
            
            // Since we're loading pictures in the background anyway (and a TODO is to cache
            // images locally, let's not bother storing the image in the backing Product Studio bug.
            //SetUserPicture(userInfo.UserPicture);
        }

        protected override void DummyInitialize(string dummyTitle)
        {
            Title = dummyTitle;
            Alias = dummyTitle;
            DisplayName = dummyTitle;
        }

        static GroupMemberItem m_dummyActiveMember;
        public static GroupMemberItem GetDummyActiveMember()
        {
            if (m_dummyActiveMember == null)
            {
                m_dummyActiveMember = new GroupMemberItem();
                m_dummyActiveMember.Title = StatusValues.Active;
                m_dummyActiveMember.Alias = StatusValues.Active;
                m_dummyActiveMember.DisplayName = StatusValues.Active;
            }

            return m_dummyActiveMember;
        }

        static GroupMemberItem m_dummyOutsideTeamMember;
        public static GroupMemberItem GetDummyExternalTeamMember()
        {
            if (m_dummyOutsideTeamMember == null)
            {
                m_dummyOutsideTeamMember = new GroupMemberItem();
                m_dummyOutsideTeamMember.Title = Globals.c_ExternalTeam;
                m_dummyOutsideTeamMember.Alias = Globals.c_ExternalTeam;
                m_dummyOutsideTeamMember.DisplayName = Globals.c_ExternalTeam;
            }

            return m_dummyOutsideTeamMember;
        }

        public bool IsDevManager
        {
            get
            {
                ProductGroupItem productGroup = ProductGroupManager.Instance.CurrentProductGroup;
                if (productGroup != null)
                {
                    if (Utils.StringsMatch(productGroup.DevManagerAlias, Alias))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsTestManager
        {
            get
            {
                ProductGroupItem productGroup = ProductGroupManager.Instance.CurrentProductGroup;
                if (productGroup != null)
                {
                    if (Utils.StringsMatch(productGroup.TestManagerAlias, Alias))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public string Alias
        {
            get { return GetStringValue(Datastore.PropNameGroupMemberAlias); }
            set { SetStringValue(Datastore.PropNameGroupMemberAlias, value); }
        }

        public string Discipline
        {
            get { return GetStringValue(Datastore.PropNameDiscipline); }
            set { SetStringValue(Datastore.PropNameDiscipline, value); }
        }

        public string DisplayName
        {
            get 
            {
                return GetStringValue(Datastore.PropNameGroupMemberDisplayName);
            }

            set 
            {
                SetStringValue(Datastore.PropNameGroupMemberDisplayName, value); 
            }
        }

        public string OfficeName
        {
            get { return GetStringValue(Datastore.PropNameOfficeName); }
            set { SetStringValue(Datastore.PropNameOfficeName, value); }
        }

        public double CapacityPerDay
        {
            get
            {
                return GetDoubleValue(Datastore.PropNameJobTitlePillarAndAvgCapacity);
            }
            set
            {
                SetDoubleValue(Datastore.PropNameJobTitlePillarAndAvgCapacity, value);
            }
        }

        public string JobTitle
        {
            get 
            {
                return GetStringValue(Datastore.PropNameJobTitlePillarAndAvgCapacity);
            }
            set 
            {
                SetStringValue(Datastore.PropNameJobTitlePillarAndAvgCapacity, value);
            }
        }

        public string PillarName
        {
            get { return ParentPillarItem == null ? Globals.c_None : ParentPillarItem.Title; }
        }

        public PillarItem ParentPillarItem
        {
            get
            {
                string key = PillarItemKey;
                if (string.IsNullOrWhiteSpace(key))
                {
                    return StoreItem.GetDummyItem<PillarItem>(DummyItemType.NoneType);
                }

                return Globals.ItemManager.GetItem<PillarItem>(key);
            }

            set
            {
                PillarItem oldParent = ParentPillarItem;

                if (value == null || value.IsDummyItem)
                {
                    PillarItemKey = null;
                }
                else
                {
                    PillarItemKey = value.StoreKey;
                }
            }
        }

        public string PillarItemKey
        {
            get
            {
                return GetStringValue(Datastore.PropNameJobTitlePillarAndAvgCapacity);
            }
            set
            {
                SetStringValue(Datastore.PropNameJobTitlePillarAndAvgCapacity, value);
                NotifyPropertyChanged(() => ParentPillarItem);
                NotifyPropertyChanged(() => PillarName);
            }
        }

        public string Telephone
        {
            get { return GetStringValue(Datastore.PropNameTelephone); }
            set { SetStringValue(Datastore.PropNameTelephone, value); }
        }

        public string TotalWorkRemainingDisplay
        {
            get 
            {
                return TotalWorkRemaining.ToString();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a ContextMenu suitable for the options available for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void PopulateContextMenu(Window ownerWindow, ContextMenu menu)
        {
            AddContextMenuItem(menu, "Edit Off Days...", "PlanningStatus.png", EditOffDays_Click);
        }

        void EditOffDays_Click(object sender, RoutedEventArgs e)
        {
            EditOffDays();
        }

        public void EditOffDays()
        {
            OffTimeEditor editor = new OffTimeEditor(this);
            editor.ShowDialog();

            NotifyPropertyChanged(() => TotalOffDays);
        }

        static AsyncObservableCollection<double> m_capacityValues;
        public AsyncObservableCollection<double> AllowedCapacityValues
        {
            get
            {
                if (m_capacityValues == null)
                {
                    m_capacityValues = new AsyncObservableCollection<double>();
                    for (int x = 0; x <= Globals.IdealHoursPerDay; x++)
                    {
                        m_capacityValues.Add(x);
                    }
                }

                return m_capacityValues;
            }
        }

    }
}
