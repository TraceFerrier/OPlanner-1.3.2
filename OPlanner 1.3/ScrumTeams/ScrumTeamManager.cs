using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace PlannerNameSpace
{
    public class ScrumTeamManager
    {
        private Dictionary<string, StoreItemCollection<ScrumTeamItem>> m_pillarScrumTeams;

        public ScrumTeamManager()
        {
            m_pillarScrumTeams = null;
            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Keep the scrum team collections up-to-date.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            StoreItemChange change = e.Change;
            if (change.Item.StoreItemType == ItemTypeID.ScrumTeam)
            {
                ScrumTeamItem scrumTeamItem = (ScrumTeamItem)change.Item;
                switch (change.ChangeType)
                {
                    case ChangeType.Added:
                        HandleScrumTeamAdded(scrumTeamItem);
                        break;
                    case ChangeType.Removed:
                        HandleScrumTeamRemoved(scrumTeamItem);
                        break;
                    case ChangeType.Updated:
                        break;
                }
            }
        }

        void HandleScrumTeamAdded(ScrumTeamItem scrumTeamItem)
        {
            if (scrumTeamItem.ParentPillarItem != null)
            {
                string pillarKey = scrumTeamItem.ParentPillarItem.StoreKey;
                if (!m_pillarScrumTeams.ContainsKey(pillarKey))
                {
                    m_pillarScrumTeams.Add(pillarKey, new StoreItemCollection<ScrumTeamItem>());
                }

                StoreItemCollection<ScrumTeamItem> pillarScrumTeams = m_pillarScrumTeams[scrumTeamItem.ParentPillarItem.StoreKey];
                if (!pillarScrumTeams.Contains(scrumTeamItem))
                {
                    pillarScrumTeams.Add(scrumTeamItem);
                }
            }
        }

        void HandleScrumTeamRemoved(ScrumTeamItem scrumTeamItem)
        {
            if (scrumTeamItem.ParentPillarItem != null)
            {
                StoreItemCollection<ScrumTeamItem> pillarScrumTeams = m_pillarScrumTeams[scrumTeamItem.ParentPillarItem.StoreKey];
                if (pillarScrumTeams.Contains(scrumTeamItem))
                {
                    pillarScrumTeams.Remove(scrumTeamItem);
                }
            }
        }

        public AsyncObservableCollection<ScrumTeamItem> GetScrumTeams(PillarItem pillarItem)
        {
            if (m_pillarScrumTeams == null)
            {
                m_pillarScrumTeams = new Dictionary<string, StoreItemCollection<ScrumTeamItem>>();
            }

            if (!m_pillarScrumTeams.ContainsKey(pillarItem.StoreKey))
            {
                m_pillarScrumTeams.Add(pillarItem.StoreKey, new StoreItemCollection<ScrumTeamItem>());

                StoreItemCollection<ScrumTeamItem> pillarScrumTeams = m_pillarScrumTeams[pillarItem.StoreKey];
                foreach (ScrumTeamItem scrumTeam in ScrumTeamItem.Items)
                {
                    if (scrumTeam.ParentPillarItem == pillarItem)
                    {
                        pillarScrumTeams.Add(scrumTeam);
                    }
                }

                pillarScrumTeams.Sort((x, y) => x.Title.CompareTo(y.Title));
                pillarScrumTeams.Insert(0, ScrumTeamItem.GetDummyNoneTeam());
            }

            return m_pillarScrumTeams[pillarItem.StoreKey];

        }

    }
}
