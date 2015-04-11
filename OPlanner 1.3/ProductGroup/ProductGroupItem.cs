using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PlannerNameSpace
{
    public class ProductGroupData
    {
        public List<string> MemberAliases { get; set; }

        public ProductGroupData()
        {
            MemberAliases = new List<string>();
        }
    }

    public class ProductGroupItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.ProductGroup; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        ProductGroupData m_productGroupData;

        public ProductGroupData ProductGroupData
        {
            get 
            {
                if (m_productGroupData == null)
                {
                    m_productGroupData = Utils.UnserializeFromItemProperty<ProductGroupData>(this, Datastore.PropNameProductGroupData);
                    if (m_productGroupData == null)
                    {
                        m_productGroupData = new ProductGroupData();
                    }
                }

                return m_productGroupData; 
            }

            set 
            {
                Utils.SerializeToItemProperty<ProductGroupData>(this, Datastore.PropNameProductGroupData, value);
            }
        }

        public List<string> MemberAliases
        {
            get
            {
                return ProductGroupData.MemberAliases;
            }

            set
            {
                ProductGroupData.MemberAliases.Clear();
                foreach (string alias in value)
                {
                    ProductGroupData.MemberAliases.Add(alias);
                }

                Utils.SerializeToItemProperty<ProductGroupData>(this, Datastore.PropNameProductGroupData, ProductGroupData);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// This is the default 'Team' name to set in order to retrieve the subset of specs 
        /// from the backing store owned by this product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string DefaultSpecTeamName
        {
            get
            {
                string defaultSpecTeamName = GetStringValue(Datastore.PropNameProductGroupComposite);
                if (string.IsNullOrWhiteSpace(defaultSpecTeamName))
                {
                    defaultSpecTeamName = Globals.c_NoneSpecTeamName;
                }

                return defaultSpecTeamName;
            }
            set
            {
                SetStringValue(Datastore.PropNameProductGroupComposite, value);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// A 'group admin' is the alias of a person a product group authorizes to make
        /// group-wide changes, such as committing backlog items for a train, etc.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string GroupAdmin1
        {
            get
            {
                return GetStringValue(Datastore.PropNameProductGroupComposite);
            }
            set
            {
                SetStringValueImmediate(Datastore.PropNameProductGroupComposite, value);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Logs to the back-end store that the given user successfully launched the app.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string GroupAdmin2
        {
            get
            {
                return GetStringValue(Datastore.PropNameProductGroupComposite);
            }
            set
            {
                SetStringValueImmediate(Datastore.PropNameProductGroupComposite, value);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Logs to the back-end store that the given user successfully launched the app.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string GroupAdmin3
        {
            get
            {
                return GetStringValue(Datastore.PropNameProductGroupComposite);
            }
            set
            {
                SetStringValueImmediate(Datastore.PropNameProductGroupComposite, value);
            }
        }

        public string HostItemStoreName
        {
            get
            {
                return GetStringValue(Datastore.PropNameProductGroupComposite);
            }
            set
            {
                SetStringValue(Datastore.PropNameProductGroupComposite, value);
            }
        }

        public bool IsCompatibleWithCurrentStore
        {
            get
            {
                // For compatibility, any product group without a HostItemStoreName is considered to be non-clone
                if (string.IsNullOrWhiteSpace(HostItemStoreName))
                {
                    if (HostItemStore.Instance.IsCloneStore)
                    {
                        return false;
                    }
                }

                else if (!Utils.StringsMatch(HostItemStoreName, HostItemStore.Instance.StoreName))
                {
                    return false;
                }

                return true;
            }
        }

        public void EnsureProductGroupMembers()
        {
            List<string> aliases = MemberAliases;
            if (aliases == null || aliases.Count == 0)
            {
                aliases = Globals.GroupMemberManager.DiscoverProductGroupMemberAliases(this);
                if (aliases != null && aliases.Count > 0)
                {
                    MemberAliases = aliases;
                }
            }
        }

        public string DevManagerDisplayName
        {
            get
            {
                GroupMemberItem member = Globals.GroupMemberManager.GetMemberByAlias(DevManagerAlias);
                if (member != null)
                {
                    return member.DisplayName;
                }
                else
                {
                    return UserInformation.GetDisplayNameFromAlias(DevManagerAlias);
                }
            }
        }

        public string TestManagerDisplayName
        {
            get
            {
                GroupMemberItem member = Globals.GroupMemberManager.GetMemberByAlias(TestManagerAlias);
                if (member != null)
                {
                    return member.DisplayName;
                }
                else
                {
                    return UserInformation.GetDisplayNameFromAlias(TestManagerAlias);
                }
            }
        }

        public string GroupPMDisplayName
        {
            get
            {
                GroupMemberItem member = Globals.GroupMemberManager.GetMemberByAlias(GroupPMAlias);
                if (member != null)
                {
                    return member.DisplayName;
                }
                else
                {
                    return UserInformation.GetDisplayNameFromAlias(GroupPMAlias);
                }
            }
        }

        public BitmapSource DevManagerPicture
        {
            get
            {
                return GetActiveDirectoryImageValue(DevManagerAlias);
            }
        }

        public BitmapSource TestManagerPicture
        {
            get
            {
                return GetActiveDirectoryImageValue(TestManagerAlias);
            }
        }

        public BitmapSource GroupPMPicture
        {
            get
            {
                return GetActiveDirectoryImageValue(GroupPMAlias);
            }
        }

        public string GroupPMAlias
        {
            get { return GetStringValue(Datastore.PropNameGroupPM); }
            set 
            {
                SetStringValue(Datastore.PropNameGroupPM, value);
                NotifyPropertyChanged(() => GroupPMDisplayName);
                NotifyPropertyChanged(() => GroupPMPicture);
            }
        }

        public string DevManagerAlias
        {
            get { return GetStringValue(Datastore.PropNameDevManager); }
            set 
            {
                SetStringValue(Datastore.PropNameDevManager, value);
                NotifyPropertyChanged(() => DevManagerDisplayName);
                NotifyPropertyChanged(() => DevManagerPicture);
            }
        }

        public string TestManagerAlias
        {
            get { return GetStringValue(Datastore.PropNameTestManager); }
            set 
            {
                SetStringValue(Datastore.PropNameTestManager, value);
                NotifyPropertyChanged(() => TestManagerDisplayName);
                NotifyPropertyChanged(() => TestManagerPicture);
            }
        }

        public DateTime MembersLastUpdated
        {
            get 
            {
                return GetDateValue(Datastore.PropNameProductGroupMembersLastUpdated); 
            }

            set 
            {
                SetDateValueImmediate(Datastore.PropNameProductGroupMembersLastUpdated, DateTime.Today);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a ContextMenu suitable for the options available for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void PopulateContextMenu(Window ownerWindow, ContextMenu menu)
        {
            AddContextMenuItem(menu, "Delete Product Group...", "Edit.png", Edit_Click);
        }

        void Edit_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
