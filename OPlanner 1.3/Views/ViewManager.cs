using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Ribbon;
using System.Windows;

namespace PlannerNameSpace
{
    public enum ViewSelection
    {
        Selected,
        Unselected,
    }

    public enum ViewType
    {
        DataGridView,
        CategoryView,
    }

    public delegate void ViewManagerEventHandler(AvailableViews oldViewName, AvailableViews newViewName);

    public enum AvailableViews
    {
        None,
        BacklogStandardView,
        BacklogPlanningColumnsView,
        BacklogTrainReviewView,
        BacklogLastTrainResultRecap,
        BacklogThisTrainCommitments,
        BacklogThisTrainNonCommitments,
        ExperienceForecastView,
        ExperienceSpecStatusView,
        ExperienceSummaryView,
    }

    public class View
    {
        public AvailableViews ViewName { get; set; }
        public ViewType ViewType { get; set; }
    }

    public abstract class ViewManager
    {
        public static AvailableViews CurrentGlobalView { get; set; } 
        protected Dictionary<View, RibbonRadioButton> ViewControls { get; set; }
        public View CurrentView { get; set; }
        protected Dictionary<AvailableViews, List<string>> Views { get; set; }
        protected ProductPreferences InitialViewPreference { get; set; }
        protected ProductPreferences InitialViewTypePreference { get; set; }
        protected Dictionary<AvailableViews, List<FrameworkElement>> CategoryVisibilityElements { get; set; }
        protected List<FrameworkElement> DataGridVisibilityElements { get; set; }

        public event ViewManagerEventHandler ViewSelectionChanged;

        public ViewManager(ProductPreferences initialView, ProductPreferences initialViewType, AvailableViews defaultViewName, ViewType defaultViewType)
        {
            InitialViewPreference = initialView;
            InitialViewTypePreference = initialViewType;
            Views = new Dictionary<AvailableViews, List<string>>();
            ViewControls = new Dictionary<View, RibbonRadioButton>();
            CategoryVisibilityElements = new Dictionary<AvailableViews, List<FrameworkElement>>();
            DataGridVisibilityElements = new List<FrameworkElement>();

            View defaultView = null;
            string initialViewText = Globals.UserPreferences.GetProductPreference(InitialViewPreference);
            string initialViewTypeText = Globals.UserPreferences.GetProductPreference(InitialViewTypePreference);
            if (initialViewText != null && initialViewTypeText != null)
            {
                AvailableViews viewName = Utils.StringToEnum<AvailableViews>(initialViewText);
                if (viewName != AvailableViews.None)
                {
                    defaultView = new View();
                    defaultView.ViewName = Utils.StringToEnum<AvailableViews>(initialViewText);
                    defaultView.ViewType = Utils.StringToEnum<ViewType>(initialViewTypeText);
                    CurrentView = defaultView;
                }
            }

            if (defaultView == null)
            {
                defaultView = new View();
                defaultView.ViewName = defaultViewName;
                defaultView.ViewType = defaultViewType;
                CurrentView = defaultView;
            }
        }

        protected View IdentifyView(AvailableViews viewName)
        {
            foreach (KeyValuePair<View, RibbonRadioButton> kvp in ViewControls)
            {
                View view = kvp.Key;
                if (view.ViewName == viewName)
                {
                    return view;
                }
            }

            return null;
        }

        public void AddViewControl(AvailableViews viewName, RibbonRadioButton button, ViewType viewType = ViewType.DataGridView)
        {
            View view = new View();
            view.ViewName = viewName;
            view.ViewType = viewType;

            button.Tag = view;
            button.Checked += ViewButton_Clicked;
            ViewControls.Add(view, button);
        }

        public void AddDataGridVisibilityElement(FrameworkElement visibilityElement)
        {
            DataGridVisibilityElements.Add(visibilityElement);
        }

        public void AddCategoryVisibilityElement(AvailableViews viewName, FrameworkElement visibilityElement)
        {
            if (!CategoryVisibilityElements.ContainsKey(viewName))
            {
                CategoryVisibilityElements.Add(viewName, new List<FrameworkElement>());
            }

            List<FrameworkElement> elements = CategoryVisibilityElements[viewName];
            elements.Add(visibilityElement);
        }

        public void InitializeView()
        {
            UpdateViewInternal(null, CurrentView);
            UpdateViewControls();
        }

        void ViewButton_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            View oldView = CurrentView;
            RibbonRadioButton button = sender as RibbonRadioButton;
            CurrentView = (View)button.Tag;
            Globals.UserPreferences.SetProductPreference(InitialViewPreference, Utils.EnumToString<AvailableViews>(CurrentView.ViewName));
            Globals.UserPreferences.SetProductPreference(InitialViewTypePreference, Utils.EnumToString<ViewType>(CurrentView.ViewType));

            UpdateViewInternal(oldView, CurrentView);
            InternalViewSelectionChanged(CurrentView.ViewName);
        }

        private void UpdateViewInternal(View oldView = null, View newView = null)
        {
            if (newView == null || newView.ViewType == ViewType.DataGridView)
            {
                UpdateView();
            }

            if (oldView != null)
            {
                if (oldView.ViewType == ViewType.DataGridView)
                {
                    foreach (FrameworkElement element in DataGridVisibilityElements)
                    {
                        element.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (CategoryVisibilityElements.ContainsKey(oldView.ViewName))
                    {
                        List<FrameworkElement> elements = CategoryVisibilityElements[oldView.ViewName];
                        foreach (FrameworkElement element in elements)
                        {
                            element.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }

            if (newView != null)
            {
                if (newView.ViewType == ViewType.DataGridView)
                {
                    foreach (FrameworkElement element in DataGridVisibilityElements)
                    {
                        element.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    foreach (FrameworkElement element in DataGridVisibilityElements)
                    {
                        element.Visibility = Visibility.Collapsed;
                    }

                    if (CategoryVisibilityElements.ContainsKey(newView.ViewName))
                    {
                        List<FrameworkElement> elements = CategoryVisibilityElements[newView.ViewName];
                        foreach (FrameworkElement element in elements)
                        {
                            element.Visibility = Visibility.Visible;
                        }
                    }
                }
            }

            if (ViewSelectionChanged != null)
            {
                ViewSelectionChanged(oldView.ViewName, newView.ViewName);
            }
        }

        protected virtual void UpdateView()
        {
            UpdateViewControls();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Handler that's called when the current view selection is changed by the user.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected virtual void InternalViewSelectionChanged(AvailableViews currentView)
        {
            CurrentGlobalView = currentView;
        }

        void UpdateViewControls()
        {
            foreach (KeyValuePair<View, RibbonRadioButton> kvp in ViewControls)
            {
                View view = kvp.Key;
                RibbonRadioButton button = kvp.Value;

                if (CurrentView != null && view.ViewName == CurrentView.ViewName)
                {
                    button.IsChecked = true;
                }
                else
                {
                    button.IsChecked = false;
                }
            }
        }

    }
}
