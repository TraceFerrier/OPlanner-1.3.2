using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public class GroupMemberItemCache : AsyncObservableCollection<GroupMemberItem>
    {
        Dictionary<string, GroupMemberItem> m_groupMembersByAlias;

        public GroupMemberItemCache()
        {
            m_groupMembersByAlias = new Dictionary<string, GroupMemberItem>();
        }

        public override void Add(GroupMemberItem item)
        {
            List<string> aliases = ProductGroupManager.Instance.GetMemberAliases();
            string alias = item.Alias;
            if (aliases.Contains(alias))
            {
                if (!m_groupMembersByAlias.ContainsKey(alias))
                {
                    base.Add(item);
                    m_groupMembersByAlias.Add(alias, item);
                }
                else
                {
                    Globals.ApplicationManager.WriteToEventLog("Duplicate group member found: " + alias + "(" + item.ID + ")");
                }
            }
        }

        public override bool Remove(GroupMemberItem item)
        {
            string alias = item.Alias;
            if (m_groupMembersByAlias.ContainsKey(alias))
            {
                m_groupMembersByAlias.Remove(alias);
            }

            return base.Remove(item);
        }

        public AsyncObservableCollection<GroupMemberItem> GetSortedGroupMembers()
        {
            AsyncObservableCollection<GroupMemberItem> items = this.ToCollection();
            items.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));
            return items;
        }

        public GroupMemberItem GetMemberByAlias(string alias)
        {
            if (!string.IsNullOrWhiteSpace(alias))
            {
                if (m_groupMembersByAlias.ContainsKey(alias))
                {
                    return m_groupMembersByAlias[alias];
                }
            }

            return null;
        }

        public List<GroupMemberItem> GetMembersByPillar(PillarItem pillar, string discipline)
        {
            List<GroupMemberItem> filteredMembers = new List<GroupMemberItem>();
            foreach (GroupMemberItem member in this)
            {
                if (member.ParentPillarItem == pillar)
                {
                    if (member.Discipline == discipline || discipline == Globals.c_All)
                    {
                        if (!member.IsDevManager && !member.IsTestManager)
                        {
                            filteredMembers.Add(member);
                        }
                    }
                }
            }

            return filteredMembers;
        }
    }
}
