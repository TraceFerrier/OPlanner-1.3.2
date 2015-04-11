using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PlannerNameSpace
{
    public class ScrumTeamSelector
    {
        public ComboBox ScrumTeamCombo { get; set; }
        public ScrumTeamItem CurrentScrumTeam { get; set; }
        public SelectorState<PillarItem> PillarState;
        public event ScrumTeamItemEventHandler ScrumTeamItemSelectionChanged;

        public ScrumTeamSelector(ComboBox scrumTeamCombo, ProductPreferences initialScrumTeamSelectionKey, ComboBox pillarCombo, ProductPreferences initialPillarSelectionKey)
        {
            SelectorState<PillarItem> pillarState = new SelectorState<PillarItem>(pillarCombo, initialPillarSelectionKey);
            ScrumTeamItem initialScrumTeam = Globals.UserPreferences.GetItemSelectionPreference<ScrumTeamItem>(initialScrumTeamSelectionKey);
            InitializeSelector(scrumTeamCombo, initialScrumTeam, pillarState);
        }

        public ScrumTeamSelector(ComboBox scrumTeamCombo, ProductPreferences initialScrumTeamSelectionKey, SelectorState<PillarItem> pillarState)
        {
            ScrumTeamItem initialScrumTeam = Globals.UserPreferences.GetItemSelectionPreference<ScrumTeamItem>(initialScrumTeamSelectionKey);
            InitializeSelector(scrumTeamCombo, initialScrumTeam, pillarState);
        }

        public ScrumTeamSelector(ComboBox scrumTeamCombo, ScrumTeamItem initialScrumTeamSelection, SelectorState<PillarItem> pillarState)
        {
            InitializeSelector(scrumTeamCombo, initialScrumTeamSelection, pillarState);
        }

        private void InitializeSelector(ComboBox scrumTeamCombo, ScrumTeamItem initialScrumTeamSelection, SelectorState<PillarItem> pillarState)
        {
            PillarState = pillarState;
            PillarState.CurrentItemChanged += PillarState_CurrentItemChanged;
            ScrumTeamCombo = scrumTeamCombo;

            CurrentScrumTeam = Globals.UserPreferences.GetItemSelectionPreference<ScrumTeamItem>(ProductPreferences.LastSelectedFeatureTeamInEditor);
            SetScrumTeamItems();

            if (CurrentScrumTeam != null)
            {
                ScrumTeamCombo.SelectedItem = CurrentScrumTeam;
                SetSelectedScrumTeam(CurrentScrumTeam);
            }

            Globals.EventManager.ScrumTeamCollectionChanged += Handle_ScrumTeamCollectionChanged;
            ScrumTeamCombo.SelectionChanged += ScrumTeamCombo_SelectionChanged;

        }

        public PillarItem CurrentPillar
        {
            get { return PillarState.CurrentItem; }
        }

        void PillarState_CurrentItemChanged(object sender, EventArgs e)
        {
            SetScrumTeamItems();
        }

        void Handle_ScrumTeamCollectionChanged(object sender, EventArgs e)
        {
            SetScrumTeamItems();
        }

        void SetScrumTeamItems()
        {
            AsyncObservableCollection<ScrumTeamItem> scrumTeamItems = Globals.ScrumTeamManager.GetScrumTeams(PillarState.CurrentItem);
            ScrumTeamCombo.ItemsSource = scrumTeamItems;
            if (CurrentScrumTeam == null || !scrumTeamItems.Contains(CurrentScrumTeam))
            {
                if (scrumTeamItems.Count > 0)
                {
                    CurrentScrumTeam = scrumTeamItems.GetItem(0);
                    ScrumTeamCombo.SelectedItem = CurrentScrumTeam;
                }
            }
        }

        public void SetSelectedScrumTeam(ScrumTeamItem featureTeamItem)
        {
            CurrentScrumTeam = featureTeamItem;
            if (!ScrumTeamCombo.Items.Contains(featureTeamItem))
            {
                ScrumTeamCombo.ItemsSource = Globals.ScrumTeamManager.GetScrumTeams(PillarState.CurrentItem);
            }

            ScrumTeamCombo.SelectedItem = CurrentScrumTeam;

            Globals.UserPreferences.SetItemSelectionPreference(ProductPreferences.LastSelectedFeatureTeamInEditor, CurrentScrumTeam);
            OnScrumTeamItemSelectionChanged(CurrentScrumTeam);
        }

        void ScrumTeamCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScrumTeamItem featureTeamItem = ScrumTeamCombo.SelectedItem as ScrumTeamItem;
            if (featureTeamItem != null)
            {
                CurrentScrumTeam = featureTeamItem;
                SetSelectedScrumTeam(featureTeamItem);
            }
        }

        public void OnScrumTeamItemSelectionChanged(ScrumTeamItem item)
        {
            if (ScrumTeamItemSelectionChanged != null)
            {
                ScrumTeamItemSelectionChanged(this, new ScrumTeamChangedEventArgs(item));
            }
        }
    }
}
