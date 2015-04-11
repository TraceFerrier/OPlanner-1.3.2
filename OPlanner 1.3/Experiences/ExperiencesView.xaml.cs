using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlannerNameSpace.Views
{
    /// <summary>
    /// Interaction logic for ExperiencesView.xaml
    /// </summary>
    public partial class ExperiencesView : UserControl
    {
        public static PillarItem SelectedPillarItem { get; set; }
        public static QuarterItem SelectedQuarterItem { get; set; }
        public static string SortProperty { get; set; }

        ItemsControlFilter<ExperienceItem> Filter;

        public ExperiencesView()
        {
            InitializeComponent();
        }

        public void InitializeData()
        {
            ViewContext.DataContext = Globals.ExperienceManager;

            Globals.MainWindow.ExperiencesQuarterFilterCombo.SelectionChanged += ExperiencesFilterCombo_SelectionChanged;

            Filter = new ItemsControlFilter<ExperienceItem>(ExperienceItemsControl, ItemTypeID.Experience);
            Filter.AddFilter(Globals.MainWindow.ExperiencesPillarFilterCombo, ProductPreferences.LastSelectedExperienceViewPillarItem, ItemTypeID.Pillar);
            Filter.AddFilter(Globals.MainWindow.ExperiencesQuarterFilterCombo, ProductPreferences.LastSelectedExperienceViewQuarterItem, ItemTypeID.Quarter, false);
            Filter.AddFilter(Globals.MainWindow.ExperiencesPersonaFilterCombo, ProductPreferences.LastSelectedExperienceViewPersonaItem, ItemTypeID.Persona);
            Filter.AddHideChildlessItemsCheckbox(Globals.MainWindow.HideExperiencesCheckBox);
            Filter.AddSortProperty(Globals.MainWindow.ExperienceSortingCombo, "Title", ProductPreferences.LastSelectedExperienceSortPropertyValue, true);
            Filter.AddSortProperty(Globals.MainWindow.ExperienceSortingCombo, "BusinessRank", ProductPreferences.LastSelectedExperienceSortPropertyValue, true);
            Filter.CustomSort += Filter_CustomSort;
            Filter.UpdateFilter();

            ExperienceViewManager viewManager = new ExperienceViewManager(ProductPreferences.LastSelectedExperienceViewValue, ProductPreferences.LastSelectedExperienceViewTypeValue, AvailableViews.ExperienceSummaryView);
            viewManager.AddViewControl(AvailableViews.ExperienceSummaryView, Globals.MainWindow.ExperienceSummaryViewRadioButton);
            viewManager.AddViewControl(AvailableViews.ExperienceSpecStatusView, Globals.MainWindow.ExperienceSpecStatusViewRadioButton);
            viewManager.AddViewControl(AvailableViews.ExperienceForecastView, Globals.MainWindow.ExperienceForecastingViewRadioButton);
            viewManager.InitializeView();

            Globals.MainWindow.ExperienceCreateButton.Click += ExperienceCreateButton_Click;
            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;
        }

        void Filter_CustomSort(string sortProperty)
        {
            SortProperty = sortProperty;
        }

        void ExperiencesFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPillarItem = Globals.MainWindow.ExperiencesPillarFilterCombo.SelectedItem as PillarItem;
            SelectedQuarterItem = Globals.MainWindow.ExperiencesQuarterFilterCombo.SelectedItem as QuarterItem;
            foreach (ExperienceItem item in Globals.ItemManager.ExperienceItems)
            {
                item.NotifyPropertyChanged(() => item.ExperienceViewScenarioItems);
            }
        }

        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            if (e.Change.ChangeType == ChangeType.Added || e.Change.ChangeType == ChangeType.Removed)
            {
                Filter.UpdateFilter();
            }
        }

        void ExperienceCreateButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            PillarItem parentPillarItem = Globals.MainWindow.ExperiencesPillarFilterCombo.SelectedItem as PillarItem;
            if (parentPillarItem != null && parentPillarItem.IsDummyItem)
            {
                parentPillarItem = null;
            }

            PersonaItem personaItem = Globals.MainWindow.ExperiencesPersonaFilterCombo.SelectedItem as PersonaItem;
            if (personaItem != null && personaItem.IsDummyItem)
            {
                personaItem = null;
            }

            ExperienceItem.CreateExperienceItem(parentPillarItem, personaItem);
            Filter.UpdateFilter();

        }

        private void EditExperience_Click(object sender, RoutedEventArgs e)
        {
            IInputElement element = Mouse.DirectlyOver;
            if (element != null && element is FrameworkElement)
            {
                FrameworkElement frameworkElement = (FrameworkElement)element;
                ScenarioItem scenarioItem = frameworkElement.DataContext as ScenarioItem;
                if (scenarioItem != null)
                {
                    scenarioItem.ShowScenarioEditor();
                }
                else
                {
                    ExperienceItem experienceItem = frameworkElement.DataContext as ExperienceItem;
                    if (experienceItem != null)
                    {
                        experienceItem.ShowExperienceEditor();
                    }
                }
            }
        }

        private void DeleteExperience_Click(object sender, RoutedEventArgs e)
        {
            IInputElement element = Mouse.DirectlyOver;
            if (element != null && element is FrameworkElement)
            {
                FrameworkElement frameworkElement = (FrameworkElement)element;
                ScenarioItem scenarioItem = frameworkElement.DataContext as ScenarioItem;
                if (scenarioItem != null)
                {
                    scenarioItem.RequestDeleteItem();
                    Filter.UpdateFilter();
                }
                else
                {
                    ExperienceItem experienceItem = frameworkElement.DataContext as ExperienceItem;
                    if (experienceItem != null)
                    {
                        experienceItem.RequestDeleteItem();
                        Filter.UpdateFilter();
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Attempts to extract a work item reference from the given sender object.
        /// </summary>
        //------------------------------------------------------------------------------------
        private ExperienceItem GetExperienceFromSender(object sender)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                return menuItem.DataContext as ExperienceItem;
            }

            return null;
        }

        private void ScenarioItemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element != null && element is FrameworkElement)
            {
                FrameworkElement frameworkElement = (FrameworkElement)element;
                ScenarioItem scenarioItem = frameworkElement.DataContext as ScenarioItem;
                if (scenarioItem != null)
                {
                    scenarioItem.ShowScenarioEditor();
                }
            }
        }

        private void ExperienceItemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element != null && element is FrameworkElement)
            {
                FrameworkElement frameworkElement = (FrameworkElement)element;
                ExperienceItem experienceItem = frameworkElement.DataContext as ExperienceItem;
                if (experienceItem != null)
                {
                    experienceItem.ShowExperienceEditor();
                }
            }
        }

    }
}
