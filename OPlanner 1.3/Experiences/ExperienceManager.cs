using System.Windows;

namespace PlannerNameSpace
{
    public class ExperienceManager : BasePropertyChanged
    {
        public ExperienceManager()
        {
        }

        public Visibility IsExperienceSummaryView { get { return ViewManager.CurrentGlobalView == AvailableViews.ExperienceSummaryView ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility IsNotExperienceSummaryView { get { return ViewManager.CurrentGlobalView != AvailableViews.ExperienceSummaryView ? Visibility.Visible : Visibility.Collapsed; } }

    }
}
