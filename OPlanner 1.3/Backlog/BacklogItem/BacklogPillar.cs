using System.Windows;

namespace PlannerNameSpace
{
    public partial class BacklogItem
    {
        private PillarItem m_parentPillarItem;

        public string ParentPillarTitle
        {
            get 
            {
                return ParentPillarItem.Title; 
            } 
        }

        public string ParentPillarPath 
        {
            get 
            {
                if (ParentPillarItem.IsNoneItem)
                {
                    return Globals.c_None;
                }

                return ParentPillarItem.PillarPath; 
            }
        }

        public void NotifyPillarChanged(StoreItemChange change)
        {
            m_parentPillarItem = null;
            NotifyPropertyChanged(() => ParentPillarItem);
            NotifyPropertyChanged(() => PillarName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the PillarItem that this backlog item is assigned to.
        /// </summary>
        //------------------------------------------------------------------------------------
        public PillarItem ParentPillarItem
        {
            get
            {
                if (m_parentPillarItem == null)
                {
                    m_parentPillarItem = ProductGroupManager.Instance.FindOwnerPillar(this.TreeID);
                    if (m_parentPillarItem == null)
                    {
                        m_parentPillarItem = GetDummyItem<PillarItem>(DummyItemType.NoneType);
                    }
                }

                return m_parentPillarItem;
            }

            set
            {
                // If this backlog item is attached to a scrum team, and this is an attempt
                // to move this item under a pillar not associated with the scrum team, verify
                // with the user whether they're OK with that.
                if (value != null)
                {
                    PillarItem currentPillarItem = m_parentPillarItem;
                    if (currentPillarItem != null && currentPillarItem.IsDummyItem)
                    {
                        currentPillarItem = null;
                    }

                    PillarItem proposedPillarItem = value;
                    if (currentPillarItem == null || proposedPillarItem.StoreKey != currentPillarItem.StoreKey)
                    {
                        bool changeValidated = true;
                        bool removeFromFeatureTeam = false;
                        if (!this.IsNew)
                        {
                            if (ScrumTeamItem != null && !ScrumTeamItem.IsDummyItem && ScrumTeamItem.ParentPillarKey != proposedPillarItem.StoreKey)
                            {
                                bool userClickedYes = UserMessage.Show("This backlog item is currently assigned to the " + ScrumTeamItem.Title + " Scrum Team - moving this backlog item to a different pillar will" +
                                    " remove the Scrum Team assignment.  Click OK if you are sure this is what you want to do:", MessageBoxButton.OKCancel);

                                if (!userClickedYes)
                                {
                                    changeValidated = false;
                                    Globals.EventManager.OnPropertyChangedCanceled(this);
                                }
                                else
                                {
                                    removeFromFeatureTeam = true;
                                }
                            }
                        }

                        if (changeValidated)
                        {
                            m_parentPillarItem = proposedPillarItem;
                            this.TreeID = m_parentPillarItem.PillarPathID;
                            NotifyPropertyChanged(() => ParentPillarItem);
                            NotifyPropertyChanged(() => PillarName);

                            if (removeFromFeatureTeam)
                            {
                                ScrumTeamItem = null;
                            }

                            foreach (WorkItem workItem in WorkItems)
                            {
                                workItem.NotifyParentPillarChanged();
                            }

                            NotifyPropertyChanged(() => ValidScrumTeams);
                        }
                    }
                }
            }
        }

        public override PillarItem GetParentPillarItem()
        {
            return ParentPillarItem;
        }

        public string PillarName
        {
            get
            {
                return ParentPillarItem == null ? Globals.c_None : ParentPillarItem.Title;
            }
        }

    }
}
