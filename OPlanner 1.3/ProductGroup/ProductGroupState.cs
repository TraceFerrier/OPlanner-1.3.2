using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public class ProductGroupState : ViewState
    {
        public event ViewStateChangedEventHandler DisciplineChanged;

        private AllowedValue m_currentDiscipline;
        private PillarItem m_currentPillarGridPillar;
        public SelectorState<PillarItem> MemberPillarState;

        public ProductGroupState(ComboBox pillarCombo, ComboBox disciplineCombo )
        {
            // Set up Discipline combo
            AsyncObservableCollection<AllowedValue> disciplineValues = ScheduleStore.Instance.GetFieldAllowedValues(Datastore.PropNameDiscipline, new AllowedValue { Value = Globals.c_All });
            disciplineCombo.ItemsSource = disciplineValues;
            string currentDiscipline = Globals.UserPreferences.GetProductPreference(ProductPreferences.LastSelectedProductGroupDisciplineValue);
            if (string.IsNullOrWhiteSpace(currentDiscipline))
            {
                currentDiscipline = Globals.c_All;
            }

            disciplineCombo.SelectedItem = AllowedValue.GetAllowedValueFromList(disciplineValues, currentDiscipline);
            m_currentDiscipline = disciplineCombo.SelectedItem as AllowedValue;
            disciplineCombo.SelectionChanged += disciplineCombo_SelectionChanged;

            // Set up MemberPillar combo
            MemberPillarState = new SelectorState<PillarItem>(pillarCombo, ProductPreferences.LastSelectedProductGroupMembersPillar);
        }

        void disciplineCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            CurrentDiscipline = combo.SelectedItem as AllowedValue;
        }

        public PillarItem CurrentPillarGridPillar
        {
            get { return m_currentPillarGridPillar; }
            set { m_currentPillarGridPillar = value; }
        }

        public AllowedValue CurrentDiscipline
        {
            get { return m_currentDiscipline; }
            set 
            {
                m_currentDiscipline = value;
                Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedProductGroupDisciplineValue, value.ToString());

                if (DisciplineChanged != null)
                {
                    DisciplineChanged();
                }
            }
        }
    }
}
