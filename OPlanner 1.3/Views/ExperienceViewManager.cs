using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public class ExperienceViewManager : ViewManager
    {
        public ExperienceViewManager(ProductPreferences initialViewPreference, ProductPreferences initialViewTypePreference, AvailableViews defaultView)
            : base(initialViewPreference, initialViewTypePreference, defaultView, ViewType.DataGridView)
        {
        }

        protected override void InternalViewSelectionChanged(AvailableViews currentView)
        {
            base.InternalViewSelectionChanged(currentView);

            AsyncObservableCollection<ExperienceItem> experiences = Globals.ItemManager.ExperienceItems;
            foreach (ExperienceItem experience in experiences)
            {
                experience.SetCurrentView(currentView);
            }
        }
    }
}
