using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PlannerNameSpace
{
    public class BacklogViewState : PillarTrainViewState
    {
        public event EventHandler CommitmentSettingChanged;
        public event EventHandler ShowOnlyUnassignedMembersSettingChanged;
        public ColumnViewManager ViewManager { get; set; }

        private CommitmentSettingItem m_currentCommitmentSetting;
        private bool m_showOnlyUnassignedMembers;

        public BacklogViewState(ComboBox pillarCombo, ProductPreferences pillarPreference, ComboBox trainCombo, ProductPreferences trainPreference)
            : base(pillarCombo, pillarPreference, trainCombo, trainPreference)
        {

        }

        public bool ShowOnlyUnassignedMembers
        {
            get { return m_showOnlyUnassignedMembers; }
            set 
            {
                m_showOnlyUnassignedMembers = value;
                if (ShowOnlyUnassignedMembersSettingChanged != null)
                {
                    ShowOnlyUnassignedMembersSettingChanged(this, EventArgs.Empty);
                }
            }
        }

        public CommitmentSettingItem CurrentCommitmentSetting
        {
            get { return m_currentCommitmentSetting; }
            set
            {
                m_currentCommitmentSetting = value;
                if (CommitmentSettingChanged != null)
                {
                    CommitmentSettingChanged(this, EventArgs.Empty);
                }
            }
        }

    }
}
