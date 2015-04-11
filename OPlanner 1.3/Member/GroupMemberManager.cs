using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;

namespace PlannerNameSpace
{
    public enum UpdateStages
    {
        Startup,
        DetectingMembersToAddOrDelete,
        DetectingMembersToAddOrDeleteComplete,
        AddingMembersComplete,
        UpdatingProductGroupItemComplete,
        UpdatingGroupMemberInformationComplete,
        UpdateCycleComplete,
    }

    public class GroupMemberManager
    {
        public event GeneralUpdateHandler GroupMemberDiscoveryComplete;
        public event GeneralUpdateHandler CreateDiscoveredGroupMembersComplete;
        public event GeneralUpdateHandler RebuildGroupMembershipComplete;
        Queue<MemberDescriptor> CurrentMembers { get; set; }
        Queue<MemberDescriptor> NewMembers { get; set; }
        List<string> NewMemberAliases { get; set; }
        List<string> AddedAliases { get; set; }
        List<string> DeletedAliases { get; set; }
        UpdateStages UpdateStage;

        AsyncObservableCollection<GroupMemberItem> m_devGroupMembers;
        AsyncObservableCollection<GroupMemberItem> m_testGroupMembers;
        AsyncObservableCollection<GroupMemberItem> m_pmGroupMembers;
        AsyncObservableCollection<GroupMemberItem> m_assignableMembers;

        public GroupMemberManager()
        {
            UpdateStage = UpdateStages.Startup;
            Globals.EventManager.CommitmentStatusComputationComplete += Handle_CommitmentStatusComputationComplete;
            Globals.EventManager.UpdateUI += Handle_UpdateUI;
            CurrentMembers = new Queue<MemberDescriptor>();
            NewMembers = new Queue<MemberDescriptor>();
            NewMemberAliases = null;
            AddedAliases = new List<string>();
            DeletedAliases = new List<string>();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Once other higher priority background tasks are complete, check to see if it's
        /// time to kick off a refresh of the product group membership.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Handle_CommitmentStatusComputationComplete()
        {
            ProductGroupItem productGroup = ProductGroupManager.Instance.CurrentProductGroup;
            DateTime lastUpdate = productGroup.MembersLastUpdated.Date;
            if (productGroup != null && lastUpdate < DateTime.Today)
            {
                UpdateStage = UpdateStages.DetectingMembersToAddOrDelete;
                BackgroundTask updateGroupMembersTask = new BackgroundTask(false);
                updateGroupMembersTask.DoWork += AddOrDeleteGroupMembersTask_DoWork;
                updateGroupMembersTask.TaskCompleted += AddOrDeleteGroupMembersTask_TaskCompleted;
                ProductGroupToUpdate = productGroup;
                updateGroupMembersTask.RunTaskAsync();
            }
        }

        public GroupMemberItem GetMemberByAlias(string alias)
        {
            return Globals.ItemManager.GroupMemberItems.GetMemberByAlias(alias);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a sorted list of all product group members, including an entry for Active
        /// and Closed, appropriate for a list of AssignedTo members.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<GroupMemberItem> GetAssignableGroupMembers()
        {
            if (m_assignableMembers == null)
            {
                m_assignableMembers = Globals.GroupMembers.ToCollection();
                SortMembersByDisplayName(m_assignableMembers);
                m_assignableMembers.Insert(0, GroupMemberItem.GetDummyActiveMember());
                m_assignableMembers.Insert(1, GroupMemberItem.GetDummyExternalTeamMember());
            }

            return m_assignableMembers;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a sorted list of all product group dev members.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<GroupMemberItem> GetDevMembers()
        {
            if (m_devGroupMembers == null)
            {
                m_devGroupMembers = GetSortedMembers(DisciplineValues.Dev);
            }

            return m_devGroupMembers;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a sorted list of all product group Test members.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<GroupMemberItem> GetTestMembers()
        {
            if (m_testGroupMembers == null)
            {
                m_testGroupMembers = GetSortedMembers(DisciplineValues.Test);
            }

            return m_testGroupMembers;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a sorted list of all product group PM members.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<GroupMemberItem> GetPMMembers()
        {
            if (m_pmGroupMembers == null)
            {
                m_pmGroupMembers = GetSortedMembers(DisciplineValues.PM);
            }

            return m_pmGroupMembers;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of all members of the given discipline, sorted by display name.
        /// </summary>
        //------------------------------------------------------------------------------------
        AsyncObservableCollection<GroupMemberItem> GetSortedMembers(string discipline)
        {
            AsyncObservableCollection<GroupMemberItem> members = new AsyncObservableCollection<GroupMemberItem>();
            members.Add(StoreItem.GetDummyItem<GroupMemberItem>(DummyItemType.NoneType));

            foreach (GroupMemberItem member in Globals.ItemManager.GroupMemberItems)
            {
                if (member.Discipline == discipline)
                {
                    members.Add(member);
                }
            }

            SortMembersByDisplayName(members);
            return members;
        }

        void SortMembersByDisplayName(AsyncObservableCollection<GroupMemberItem> members)
        {
            ItemPropertySort<GroupMemberItem> itemComparer = new ItemPropertySort<GroupMemberItem>(Utils.GetPropertyName((GroupMemberItem s) => s.DisplayName), System.ComponentModel.ListSortDirection.Ascending);
            members.Sort((x, y) => itemComparer.Compare(x, y));
        }

        void Handle_UpdateUI()
        {
            if (UpdateStage == UpdateStages.DetectingMembersToAddOrDeleteComplete)
            {
                if (NewMembers.Count > 0)
                {
                    MemberDescriptor newMember = NewMembers.Dequeue();
                    UserInformation uiMember = newMember.Member;
                    if (!GroupMemberExists(uiMember.Alias))
                    {
                        GroupMemberItem newMemberItem = ScheduleStore.Instance.CreateStoreItem<GroupMemberItem>(ItemTypeID.GroupMember);
                        newMemberItem.BeginSaveImmediate();
                        newMemberItem.Alias = uiMember.Alias;
                        newMemberItem.Discipline = newMember.Discipline;
                        newMemberItem.CapacityPerDay = Globals.AvgCapacityPerDay;

                        newMemberItem.InitializeWithUserInformation(uiMember);
                        newMemberItem.SaveImmediate();
                    }
                }
                else
                {
                    UpdateStage = UpdateStages.AddingMembersComplete;
                }
            }

            else if (UpdateStage == UpdateStages.AddingMembersComplete)
            {
                ProductGroupItem productGroup = ProductGroupManager.Instance.CurrentProductGroup;
                if (AddedAliases.Count > 0 || DeletedAliases.Count > 0)
                {
                    if (productGroup != null && NewMemberAliases != null && NewMemberAliases.Count > 0)
                    {
                        productGroup.BeginSaveImmediate();
                        productGroup.MemberAliases = NewMemberAliases;
                        productGroup.SaveImmediate();
                    }
                }

                productGroup.MembersLastUpdated = DateTime.Today;
                UpdateStage = UpdateStages.UpdatingProductGroupItemComplete;
            }

            else if (UpdateStage == UpdateStages.UpdatingProductGroupItemComplete)
            {
                if (CurrentMembers.Count > 0)
                {
                    MemberDescriptor currentMember = CurrentMembers.Dequeue();
                    GroupMemberItem memberItem = Globals.GroupMemberManager.GetMemberByAlias(currentMember.Member.Alias);
                    if (memberItem != null)
                    {
                        Globals.ApplicationManager.WriteToEventLog("Updating GroupMemberItem information: " + memberItem.DisplayName);

                        memberItem.BeginSaveImmediate();
                        memberItem.InitializeWithUserInformation(currentMember.Member);
                        memberItem.SaveImmediate();
                    }
                }
                else
                {
                    UpdateStage = UpdateStages.UpdatingGroupMemberInformationComplete;
                }
            }
            else if (UpdateStage == UpdateStages.UpdatingGroupMemberInformationComplete)
            {
                Globals.ApplicationManager.WriteToEventLog("UpdatingGroupMemberInformationComplete");
                UpdateStage = UpdateStages.UpdateCycleComplete;
            }
        }

        void AddOrDeleteGroupMembersTask_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ProductGroupItem productGroup = ProductGroupManager.Instance.CurrentProductGroup;
            if (productGroup != null)
            {
                List<string> currentMemberAliases = productGroup.MemberAliases;
                Dictionary<string, MemberDescriptor> newMemberDescriptors = DiscoverProductGroupMemberDescriptors(ProductGroupToUpdate);
                NewMemberAliases = GetAliases(newMemberDescriptors);

                CurrentMembers.Clear();
                AddedAliases.Clear();
                NewMembers.Clear();
                foreach (KeyValuePair<string, MemberDescriptor> kvp in newMemberDescriptors)
                {
                    MemberDescriptor member = kvp.Value;
                    CurrentMembers.Enqueue(member);

                    GroupMemberItem groupMemberItem = Globals.GroupMemberManager.GetMemberByAlias(member.Member.Alias);
                    if (!currentMemberAliases.Contains(member.Member.Alias) || groupMemberItem == null)
                    {
                        NewMembers.Enqueue(member);
                        AddedAliases.Add(member.Member.Alias);
                    }
                }

                DeletedAliases.Clear();
                foreach (string alias in currentMemberAliases)
                {
                    if (!NewMemberAliases.Contains(alias))
                    {
                        DeletedAliases.Add(alias);
                    }
                }

            }
        }

        void AddOrDeleteGroupMembersTask_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            UpdateStage = UpdateStages.DetectingMembersToAddOrDeleteComplete;
        }

        public List<string> DiscoverProductGroupMemberAliases(ProductGroupItem productGroup)
        {
            Dictionary<string, MemberDescriptor> memberDescriptors = DiscoverProductGroupMemberDescriptors(productGroup);
            return GetAliases(memberDescriptors);
        }

        public List<string> GetAliases(Dictionary<string, MemberDescriptor> memberDescriptors)
        {
            List<string> aliases = new List<string>();
            foreach (KeyValuePair<string, MemberDescriptor> kvp in memberDescriptors)
            {
                MemberDescriptor member = kvp.Value;
                aliases.Add(member.Member.Alias);
            }

            return aliases;
        }

        ProductGroupItem ProductGroupToUpdate;
        public AsyncObservableCollection<MemberDescriptor> DiscoveredGroupMembers { get; set; }

        public void DiscoverGroupMembers(ProductGroupItem productGroup)
        {
            BackgroundTask DiscoverGroupMembersTask = new BackgroundTask(true);
            DiscoverGroupMembersTask.IsProgressDialogIndeterminate = true;
            DiscoverGroupMembersTask.DoWork += DiscoverGroupMembers_DoWork;
            DiscoverGroupMembersTask.TaskCompleted +=DiscoverGroupMembersTask_TaskCompleted;
            ProductGroupToUpdate = productGroup;
            DiscoverGroupMembersTask.RunTaskAsync();
        }

        void DiscoverGroupMembers_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundTask task = e.Argument as BackgroundTask;
            task.ReportProgress(0, "Creating new Product Group:", "Building the list of all the members of your new team...");
            Dictionary<string, MemberDescriptor> memberDescriptors = DiscoverProductGroupMemberDescriptors(ProductGroupToUpdate);

            DiscoveredGroupMembers = new AsyncObservableCollection<MemberDescriptor>();
            foreach (KeyValuePair<string, MemberDescriptor> kvp in memberDescriptors)
            {
                MemberDescriptor member = kvp.Value;
                DiscoveredGroupMembers.Add(member);
            }
        }

        void DiscoverGroupMembersTask_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            if (GroupMemberDiscoveryComplete != null)
            {
                GroupMemberDiscoveryComplete();
            }
        }

        public void CreateDiscoveredGroupMembers(string parentProductGroupItemKey)
        {
            BackgroundTask CreateDiscoveredGroupMembersTask = new BackgroundTask(true);
            CreateDiscoveredGroupMembersTask.IsProgressDialogIndeterminate = true;
            CreateDiscoveredGroupMembersTask.TaskArgs = parentProductGroupItemKey;
            CreateDiscoveredGroupMembersTask.DoWork += CreateDiscoveredGroupMembers_DoWork;
            CreateDiscoveredGroupMembersTask.TaskCompleted += CreateDiscoveredGroupMembersTask_TaskCompleted;
            CreateDiscoveredGroupMembersTask.RunTaskAsync();
        }

        void CreateDiscoveredGroupMembers_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundTask task = e.Argument as BackgroundTask;
            string parentProductGroupItemKey = task.TaskArgs as string;
            foreach (MemberDescriptor member in DiscoveredGroupMembers)
            {
                UserInformation uiMember = member.Member;
                if (!GroupMemberExists(uiMember.Alias))
                {
                    task.ReportProgress(0, "Adding New Team Member:", uiMember.Alias);
                    GroupMemberItem newMember = ScheduleStore.Instance.CreateStoreItem<GroupMemberItem>(ItemTypeID.GroupMember);
                    newMember.ParentProductGroupKey = parentProductGroupItemKey;
                    newMember.Alias = uiMember.Alias;
                    newMember.Discipline = member.Discipline;
                    newMember.CapacityPerDay = Globals.AvgCapacityPerDay;

                    newMember.InitializeWithUserInformation(uiMember);
                    newMember.SaveNewItem();
                }
            }
        }

        void CreateDiscoveredGroupMembersTask_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            if (CreateDiscoveredGroupMembersComplete != null)
            {
                CreateDiscoveredGroupMembersComplete();
            }
        }

        public AsyncObservableCollection<MemberDescriptor> DiscoverProductGroupMemberObservableDescriptors(string devManager, string testManager, string pmManager)
        {
            AsyncObservableCollection<MemberDescriptor> discriptors = new AsyncObservableCollection<MemberDescriptor>();
            Dictionary<string, MemberDescriptor> dict = DiscoverProductGroupMemberDescriptors(devManager, testManager, pmManager);
            foreach (KeyValuePair<string, MemberDescriptor> kvp in dict)
            {
                MemberDescriptor member = kvp.Value;
                discriptors.Add(member);
            }

            return discriptors;
        }

        public void RebuildProductGroupMembership(ProductGroupItem productGroup)
        {
            BackgroundTask RebuildMembershipTask = new BackgroundTask(true);
            RebuildMembershipTask.IsProgressDialogIndeterminate = true;
            RebuildMembershipTask.DoWork += RebuildMembershipTask_DoWork;
            RebuildMembershipTask.TaskCompleted += RebuildMembershipTask_TaskCompleted;
            ProductGroupToUpdate = productGroup;
            RebuildMembershipTask.RunTaskAsync();
        }

        void RebuildMembershipTask_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundTask task = e.Argument as BackgroundTask;
            task.ReportProgress(0, "Updating Product Group Membership:", "Re-building the list of all the members of your team...");
            Dictionary<string, MemberDescriptor> memberDescriptors = DiscoverProductGroupMemberDescriptors(ProductGroupToUpdate);

            foreach (KeyValuePair<string, MemberDescriptor> kvp in memberDescriptors)
            {
                MemberDescriptor member = kvp.Value;
                UserInformation uiMember = member.Member;
                if (!GroupMemberExists(uiMember.Alias))
                {
                    task.ReportProgress(0, "Adding New Team Member:", uiMember.Alias);
                    GroupMemberItem newMember = ScheduleStore.Instance.CreateStoreItem<GroupMemberItem>(ItemTypeID.GroupMember);
                    newMember.Alias = uiMember.Alias;
                    newMember.Discipline = member.Discipline;
                    newMember.CapacityPerDay = Globals.AvgCapacityPerDay;

                    newMember.InitializeWithUserInformation(uiMember);
                    newMember.SaveNewItem();
                }
            }

            ProductGroupToUpdate.MemberAliases = GetAliases(memberDescriptors);
        }

        void RebuildMembershipTask_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            if (RebuildGroupMembershipComplete != null)
            {
                RebuildGroupMembershipComplete();
            }
        }

        public Dictionary<string, MemberDescriptor> DiscoverProductGroupMemberDescriptors(ProductGroupItem productGroup)
        {
            return DiscoverProductGroupMemberDescriptors(productGroup.DevManagerAlias, productGroup.TestManagerAlias, productGroup.GroupPMAlias);
        }

        public Dictionary<string, MemberDescriptor> DiscoverProductGroupMemberDescriptors(string devManager, string testManager, string pmManager)
        {
            Dictionary<string, MemberDescriptor> groupMemberDescriptors = new Dictionary<string, MemberDescriptor>();
            AddDirectReportMembers(devManager, DisciplineValues.Dev, groupMemberDescriptors);
            AddDirectReportMembers(testManager, DisciplineValues.Test, groupMemberDescriptors);
            AddDirectReportMembers(pmManager, DisciplineValues.PM, groupMemberDescriptors);

            return groupMemberDescriptors;
        }

        public bool GroupMemberExists(string alias)
        {
            GroupMemberItem member = Globals.GroupMemberManager.GetMemberByAlias(alias);
            return member != null;
        }

        void AddDirectReportMembers(string alias, string discipline, Dictionary<string, MemberDescriptor> memberDescriptors)
        {
            if (!string.IsNullOrWhiteSpace(alias))
            {
                UserInformation member = new UserInformation();
                member.InitializeWithAlias(alias);
                if (member.IsValid)
                {
                    AddDirectReportMembersWorker(discipline, member, memberDescriptors);
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, alias);
                }
            }
        }

        void AddDirectReportMembersWorker(string discipline, UserInformation member, Dictionary<string, MemberDescriptor> memberDescriptors)
        {
            if (member != null && !string.IsNullOrWhiteSpace(member.Alias) && !memberDescriptors.ContainsKey(member.Alias))
            {
                MemberDescriptor descriptor = new MemberDescriptor();
                descriptor.Member = member;
                descriptor.Discipline = discipline;
                memberDescriptors.Add(member.Alias, descriptor);
            }

            if (member.IsValid)
            {
                DirectReportCollection directReports = member.DirectReports;
                foreach (UserInformation report in directReports)
                {
                    AddDirectReportMembersWorker(discipline, report, memberDescriptors);
                }
            }

        }
    }
}
