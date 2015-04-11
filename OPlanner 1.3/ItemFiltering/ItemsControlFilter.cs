using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public delegate void FilterEventHandler();
    public delegate void FilterPopulateStatusEventHandler();
    public delegate void SortEventHandler(string sortProperty);

    public class FilterInfo
    {
        public ComboBox Combo { get; set; }
        public bool UsesSelectionEnum { get; set; }
        public ProductPreferences ItemSelectionEnum { get; set; }
        public ItemTypeID ItemTypeID { get; set; }
        public bool FiltersItemsControl { get; set; }
        public string EnumValueText { get; set; }
    }

    public class SortInfo
    {
        public ComboBox Combo { get; set; }
        public ProductPreferences ItemSelectionEnum { get; set; }
        public AsyncObservableCollection<string> SortProperties { get; set; }
        public bool IsCustomSort { get; set; }
    }

    public class ItemsControlFilter<T> where T : StoreItem, new()
    {
        public event FilterEventHandler ItemsFilterUpdated;
        public event SortEventHandler CustomSort;

        AsyncObservableCollection<T> ItemsControlItems { get; set; }
        ItemsControl ItemsControl { get; set; }
        ItemTypeID ItemTypeID { get; set; }
        FilterInfoCollection FilterInfoCollection { get; set; }
        List<SortInfo> SortInfoCollection { get; set; }
        CheckBox HideChildlessItemsCheckbox { get; set; }
        public bool HideChildlessItems { get; set; }
        int UpdateFilterRequestCount { get; set; }

        public ProductGroupItem CurrentProductGroup { get; set; }
        ComboBox CurrentProductGroupCombo { get; set; }

        public ItemsControlFilter(ItemsControl itemsControl, ItemTypeID itemTypeID)
        {
            ItemsControl = itemsControl;

            ItemTypeID = itemTypeID;
            FilterInfoCollection = new PlannerNameSpace.FilterInfoCollection();
            SortInfoCollection = new List<SortInfo>();
            HideChildlessItems = false;

            RegisterStoreItemEventHandlers();
            Globals.EventManager.UpdateUI += Instance_UpdateUI;
        }

        void Instance_UpdateUI()
        {
            if (UpdateFilterRequestCount > 0)
            {
                UpdateFilterRequestCount = 0;
                UpdateFilteredItems();
            }
        }

        void RegisterStoreItemEventHandlers()
        {
            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;
        }

        public int ItemsCount
        {
            get
            {
                if (ItemsControl == null)
                {
                    return 0;
                }

                return ItemsControl.Items.Count;
            }
        }

        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            StoreItem item = e.Change.Item;
            UpdateFiltersOnItemChanges(item);

            switch(e.Change.ChangeType)
            {
                case ChangeType.Added:
                case ChangeType.Removed:
                    UpdateFilteredItems();
                    break;
                case ChangeType.Updated:
                    UpdateFiltersOnItemChanges(item);

                    if (item.StoreItemType == PlannerNameSpace.ItemTypeID.Experience)
                    {
                        if (e.Change.DSPropName.IsStoreProperty(Datastore.PropNameExperienceBusinessRank))
                        {
                            UpdateFilteredItems();
                        }
                    }
                    break;
            }
        }

        void UpdateFiltersOnItemChanges(StoreItem item)
        {
            if (!Globals.ItemManager.IsCommitInProgress)
            {
                foreach (FilterInfo filterInfo in FilterInfoCollection)
                {
                    if (filterInfo.ItemTypeID == item.StoreItemType)
                    {
                        PopulateFilters(filterInfo, null, true);
                    }
                }

                SetSortingCriteria();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a filter, using the given combo box as the selector for the filter.  The
        /// ProductPreferences value will be used to persist the last setting across 
        /// sessions.  Set filtersItemsControl to false if you don't want changes to this
        /// filter's combo box to actually change the filtered values.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void AddFilter(ComboBox comboBox, ProductPreferences itemSelectionEnum, ItemTypeID itemTypeID, bool filtersItemsControl = true)
        {
            FilterInfo filterInfo = new FilterInfo { Combo = comboBox, ItemSelectionEnum = itemSelectionEnum, ItemTypeID = itemTypeID, FiltersItemsControl = filtersItemsControl };
            filterInfo.UsesSelectionEnum = true;
            FilterInfoCollection.Add(filterInfo);

            StoreItem selectedItem = null;
            switch (itemTypeID)
            {
                default:
                    selectedItem = Globals.UserPreferences.GetItemSelectionPreference<StoreItem>(itemSelectionEnum);
                    break;
            }

            AddFilter(filterInfo, selectedItem);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a filter, using the given combo box as the selector for the filter.  The
        /// ProductPreferences value will be used to persist the last setting across 
        /// sessions.  Set filtersItemsControl to false if you don't want changes to this
        /// filter's combo box to actually change the filtered values.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void AddFilter(ComboBox comboBox, StoreItem selectedItem, ItemTypeID itemTypeID, bool filtersItemsControl = true)
        {
            FilterInfo filterInfo = new FilterInfo { Combo = comboBox, ItemTypeID = itemTypeID, FiltersItemsControl = filtersItemsControl };
            filterInfo.UsesSelectionEnum = false;
            AddFilter(filterInfo, selectedItem);
        }

        void AddFilter(FilterInfo filterInfo, StoreItem selectedItem)
        {
            if (filterInfo.ItemTypeID == PlannerNameSpace.ItemTypeID.ProductGroup)
            {
                CurrentProductGroupCombo = filterInfo.Combo;
            }

            FilterInfoCollection.Add(filterInfo);

            PopulateFilters(filterInfo, selectedItem);

            if (selectedItem != null)
            {
                filterInfo.Combo.SelectedItem = selectedItem;
            }

            filterInfo.Combo.SelectionChanged += FilterComboBox_SelectionChanged;
        }

        public void AddSortProperty(ComboBox comboBox, string sortProperty, ProductPreferences itemSelectionEnum, bool isCustomSort = false)
        {
            SortInfo sortInfo = null;
            foreach (SortInfo existingSortInfo in SortInfoCollection)
            {
                if (existingSortInfo.Combo == comboBox)
                {
                    sortInfo = existingSortInfo;
                    break;
                }
            }

            if (sortInfo == null)
            {
                sortInfo = new SortInfo();
                sortInfo.SortProperties = new AsyncObservableCollection<string>();
                sortInfo.Combo = comboBox;
                sortInfo.ItemSelectionEnum = itemSelectionEnum;
                sortInfo.IsCustomSort = isCustomSort;
                SortInfoCollection.Add(sortInfo);
            }

            sortInfo.SortProperties.Add(sortProperty);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a checkbox that when checked, will hide all items in the filtered list that
        /// have no children.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void AddHideChildlessItemsCheckbox(CheckBox checkBox)
        {
            HideChildlessItemsCheckbox = checkBox;
            checkBox.Checked += checkBox_Checked;
            checkBox.Unchecked += checkBox_Checked;
        }

        void checkBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            HideChildlessItems = checkBox.IsChecked == true;
            UpdateFilteredItems();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Internal function to populate all the values in the filter dropdowns.
        /// </summary>
        //------------------------------------------------------------------------------------
        void PopulateFilters(FilterInfo filterInfo, StoreItem selectedItem = null, bool noSelectedItemChange = false)
        {
            if (noSelectedItemChange)
            {
                selectedItem = new PillarItem();
            }

            ComboBox comboBox = filterInfo.Combo;
            ItemTypeID itemTypeID = filterInfo.ItemTypeID;
            switch (itemTypeID)
            {
                case PlannerNameSpace.ItemTypeID.Pillar:
                    comboBox.ItemsSource = PillarManager.PillarsWithAllNone;
                    if (selectedItem == null)
                    {
                        comboBox.SelectedItem = StoreItem.GetDummyItem<PillarItem>(DummyItemType.AllType);
                    }
                    break;

                case PlannerNameSpace.ItemTypeID.Quarter:
                    comboBox.ItemsSource = Globals.ItemManager.QuarterItems.GetItems(DummyItemType.AllType);
                    if (selectedItem == null)
                    {
                        comboBox.SelectedItem = StoreItem.GetDummyItem<QuarterItem>(DummyItemType.AllType);
                    }
                    break;

                case PlannerNameSpace.ItemTypeID.Train:
                    comboBox.ItemsSource = TrainManager.Instance.GetAvailableTrains();
                    if (selectedItem == null)
                    {
                        comboBox.SelectedItem = StoreItem.GetDummyItem<TrainItem>(DummyItemType.AllType);
                    }
                    break;

                case PlannerNameSpace.ItemTypeID.Scenario:
                    comboBox.ItemsSource = Globals.FilterManager.GetScenarioItemsSortedByPillar(DummyItemType.AllNoneType);
                    if (selectedItem == null)
                    {
                        comboBox.SelectedItem = StoreItem.GetDummyItem<ScenarioItem>(DummyItemType.AllType);
                    }
                    break;

                case PlannerNameSpace.ItemTypeID.Persona:
                    comboBox.ItemsSource = Globals.ItemManager.PersonaItems.GetItems(DummyItemType.AllNoneType);
                    if (selectedItem == null)
                    {
                        comboBox.SelectedItem = StoreItem.GetDummyItem<PersonaItem>(DummyItemType.AllType);
                    }
                    break;

                case PlannerNameSpace.ItemTypeID.FilterBugStatus:
                    comboBox.ItemsSource = StatusValues.ValuesAll;
                    if (selectedItem == null)
                    {
                        comboBox.SelectedItem = BacklogItemStates.All;
                    }
                    break;

                case PlannerNameSpace.ItemTypeID.FilterIssueType:
                    comboBox.ItemsSource = PlannerBugItem.IssueTypesAll;
                    if (selectedItem == null)
                    {
                        comboBox.SelectedItem = Globals.c_All;
                    }
                    break;

                case PlannerNameSpace.ItemTypeID.FilterBugAssignedTo:
                    comboBox.ItemsSource = Globals.OPlannerBugsDataSource.GetAllBugAssignedToValues();
                    if (selectedItem == null)
                    {
                        comboBox.SelectedItem = Globals.c_All;
                    }
                    break;

                case PlannerNameSpace.ItemTypeID.FilterResolution:
                    comboBox.ItemsSource = ResolutionValues.ValuesAllNone;
                    if (selectedItem == null)
                    {
                        comboBox.SelectedItem = Globals.c_All;
                    }
                    break;

                case PlannerNameSpace.ItemTypeID.FilterForecastable:
                    AsyncObservableCollection<string> forecastModeValues = new AsyncObservableCollection<string>();
                    forecastModeValues.Add(ForecastMode.Forecastable);
                    forecastModeValues.Add(ForecastMode.NotForecastable);
                    forecastModeValues.Add(Globals.c_All);
                    comboBox.ItemsSource = forecastModeValues;
                    if (selectedItem == null)
                    {
                        comboBox.SelectedItem = ForecastMode.Forecastable;
                    }
                    break;
                    
            }

            if (selectedItem != null && !noSelectedItemChange)
            {
                comboBox.SelectedItem = selectedItem;
            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has changed the selection in one of the filter combo boxes.
        /// </summary>
        //------------------------------------------------------------------------------------
        void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            foreach (FilterInfo filterInfo in FilterInfoCollection)
            {
                if (filterInfo.Combo == comboBox)
                {
                    if (filterInfo.UsesSelectionEnum)
                    {
                        switch (filterInfo.ItemTypeID)
                        {
                            default:
                                Globals.UserPreferences.SetItemSelectionPreference(filterInfo.ItemSelectionEnum, comboBox.SelectedItem as StoreItem);
                                break;
                        }
                    }

                }
            }

            UpdateFilteredItems();
        }

        public void UpdateFilter()
        {
            InitializeSortProperties();
            UpdateFilteredItems();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Internal function to update the values in the filter dropdowns based on
        /// changing conditions and dependencies.
        /// </summary>
        //------------------------------------------------------------------------------------
        void UpdateFilters()
        {
            PillarItem selectedPillarItem = null;
            TrainItem selectedTrainItem = null;
            ComboBox scenarioComboBox = null;
            StoreItem initialScenarioItem = null;

            foreach (FilterInfo filterInfo in FilterInfoCollection)
            {
                if (filterInfo.ItemTypeID == PlannerNameSpace.ItemTypeID.Pillar)
                {
                    selectedPillarItem = filterInfo.Combo.SelectedItem as PillarItem;
                }
                else if (filterInfo.ItemTypeID == PlannerNameSpace.ItemTypeID.Train)
                {
                    selectedTrainItem = filterInfo.Combo.SelectedItem as TrainItem;
                }
                else if (filterInfo.ItemTypeID == PlannerNameSpace.ItemTypeID.Scenario)
                {
                    scenarioComboBox = filterInfo.Combo;
                    initialScenarioItem = Globals.UserPreferences.GetItemSelectionPreference<ScenarioItem>(filterInfo.ItemSelectionEnum);
                }
            }

            if (scenarioComboBox != null)
            {
                StoreItem selectedItem = scenarioComboBox.SelectedItem as StoreItem;
                if (selectedPillarItem == PillarItem.GetDummyItem<PillarItem>(DummyItemType.AllType))
                {
                    scenarioComboBox.ItemsSource = Globals.FilterManager.GetScenarioItemsSortedByPillar(DummyItemType.AllNoneType);
                }
                else
                {
                    scenarioComboBox.ItemsSource = Globals.FilterManager.GetPillarScenarioItems(selectedPillarItem, DummyItemType.AllNoneType);
                }

                if (selectedItem == null)
                {
                    scenarioComboBox.SelectedItem = initialScenarioItem == null ? ScenarioItem.GetDummyAllItem() : initialScenarioItem;
                }
            }

        }

        void InitializeSortProperties()
        {
            foreach (SortInfo sortInfo in SortInfoCollection)
            {
                sortInfo.Combo.ItemsSource = sortInfo.SortProperties;
                string selectedValue = Globals.UserPreferences.GetProductPreference(sortInfo.ItemSelectionEnum);
                if (string.IsNullOrWhiteSpace(selectedValue))
                {
                    sortInfo.Combo.SelectedIndex = 0;
                }
                else
                {
                    sortInfo.Combo.SelectedValue = selectedValue;
                }

                sortInfo.Combo.SelectionChanged += SortPropertyCombo_SelectionChanged;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Takes the current selected values of all sorting combos, and sets the overall
        /// sort order of the underlying collection in the ItemsControl.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SetSortingCriteria()
        {
            if (!Globals.ApplicationManager.RunningOnUIThread())
            {
                return;
            }

            if (!Globals.ItemManager.IsCommitInProgress)
            {
                if (ItemsControl.ItemsSource != null && ItemsControl.Items.Count > 0)
                {
                    if (SortInfoCollection.Count > 0)
                    {
                        Utils.ClearSortCriteria(ItemsControl);
                        foreach (SortInfo sortInfo in SortInfoCollection)
                        {
                            if (sortInfo.IsCustomSort)
                            {
                                if (CustomSort != null)
                                {
                                    CustomSort(sortInfo.Combo.SelectedItem as string);
                                }
                            }
                            else
                            {
                                Utils.AddSortCriteria(ItemsControl, sortInfo.Combo.SelectedValue as string);
                            }
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has changed the selection in one of the sort property combo boxes.
        /// </summary>
        //------------------------------------------------------------------------------------
        void SortPropertyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo != null)
            {
                foreach (SortInfo sortInfo in SortInfoCollection)
                {
                    if (sortInfo.Combo == combo)
                    {
                        Globals.UserPreferences.SetProductPreference(sortInfo.ItemSelectionEnum, combo.SelectedValue as string);
                        break;
                    }
                }

                SetSortingCriteria();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the list of items displayed in the ItemsControl, based upon all the 
        /// current filtering/sorting criteria.
        /// </summary>
        //------------------------------------------------------------------------------------
        void UpdateFilteredItems()
        {
            UpdateFilters();

            bool filteringRequired = true;
            IList items = null;
            switch (ItemTypeID)
            {
                case PlannerNameSpace.ItemTypeID.Pillar:
                    items = PillarItem.Items;
                    break;
                case PlannerNameSpace.ItemTypeID.Scenario:
                    items = Globals.ItemManager.ScenarioItems;
                    break;
                case PlannerNameSpace.ItemTypeID.BacklogItem:
                    throw new NotImplementedException();
                case PlannerNameSpace.ItemTypeID.Experience:
                    AsyncObservableCollection<ExperienceItem> experienceItems = Globals.ItemManager.ExperienceItems.ToCollection();
                    experienceItems.Sort((x, y) => x.BusinessRank.CompareTo(y.BusinessRank));
                    items = experienceItems;
                    break;
                case PlannerNameSpace.ItemTypeID.PlannerBug:
                    items = Globals.ItemManager.PlannerBugItems;
                    break;
            }

            if (filteringRequired)
            {
                ItemsControlItems = new AsyncObservableCollection<T>();

                foreach (T item in items)
                {
                    T filteredItem = item;
                    foreach (FilterInfo filterInfo in FilterInfoCollection)
                    {
                        if (!filterInfo.FiltersItemsControl)
                        {
                            continue;
                        }

                        if (filteredItem != null)
                        {
                            if (HideChildlessItems && filteredItem.GetChildState() != ChildState.NotApplicable)
                            {
                                if (filteredItem.GetChildState() == ChildState.HasNoChildren)
                                {
                                    filteredItem = null;
                                    continue;
                                }
                            }

                            switch (filterInfo.ItemTypeID)
                            {
                                case PlannerNameSpace.ItemTypeID.Pillar:
                                    PillarItem pillarItem = filterInfo.Combo.SelectedItem as PillarItem;
                                    if (pillarItem != null && !pillarItem.IsDummyItem)
                                    {
                                        if (!filteredItem.MatchesParentPillarItem(pillarItem))
                                        {
                                            filteredItem = null;
                                        }
                                    }
                                    break;

                                case PlannerNameSpace.ItemTypeID.Quarter:
                                    QuarterItem quarterItem = filterInfo.Combo.SelectedItem as QuarterItem;
                                    if (quarterItem != null && !quarterItem.IsDummyItem)
                                    {
                                        if (filteredItem.GetParentQuarterItem() != quarterItem)
                                        {
                                            filteredItem = null;
                                        }
                                    }
                                    break;

                                case PlannerNameSpace.ItemTypeID.Train:
                                    TrainItem trainItem = filterInfo.Combo.SelectedItem as TrainItem;
                                    if (trainItem == TrainItem.BacklogTrainItem)
                                    {
                                        if (filteredItem.GetParentTrainItem() != TrainItem.BacklogTrainItem)
                                        {
                                            filteredItem = null;
                                        }
                                    }
                                    else
                                    {
                                        if (trainItem != null && !trainItem.IsDummyItem)
                                        {
                                            if (filteredItem.GetParentTrainItem() != trainItem)
                                            {
                                                filteredItem = null;
                                            }
                                        }
                                    }
                                    break;

                                case PlannerNameSpace.ItemTypeID.Scenario:
                                    ScenarioItem scenarioItem = filterInfo.Combo.SelectedItem as ScenarioItem;
                                    if (scenarioItem != null && scenarioItem != ScenarioItem.GetDummyAllItem())
                                    {
                                        if (filteredItem.GetParentScenarioItem() != scenarioItem)
                                        {
                                            filteredItem = null;
                                        }
                                    }
                                    break;

                                case PlannerNameSpace.ItemTypeID.Persona:
                                    PersonaItem personaItem = filterInfo.Combo.SelectedItem as PersonaItem;
                                    if (personaItem != null && personaItem != PersonaItem.GetDummyAllTeam())
                                    {
                                        if (filteredItem.GetParentPersonaItem() != personaItem)
                                        {
                                            filteredItem = null;
                                        }
                                    }
                                    break;

                                case PlannerNameSpace.ItemTypeID.FilterBugStatus:
                                    string bugStatus = filterInfo.Combo.SelectedItem as string;
                                    if (!string.IsNullOrWhiteSpace(bugStatus) && bugStatus != Globals.c_All)
                                    {
                                        if (!Utils.StringsMatch(filteredItem.GetItemStatusText(), bugStatus))
                                        {
                                            filteredItem = null;
                                        }
                                    }
                                    break;

                                case PlannerNameSpace.ItemTypeID.FilterIssueType:
                                    string isseType = filterInfo.Combo.SelectedItem as string;
                                    if (!string.IsNullOrWhiteSpace(isseType) && isseType != Globals.c_All)
                                    {
                                        if (!Utils.StringsMatch(filteredItem.GetItemIssueType(), isseType))
                                        {
                                            filteredItem = null;
                                        }
                                    }
                                    break;

                                case PlannerNameSpace.ItemTypeID.FilterBugAssignedTo:
                                    string assignedTo = filterInfo.Combo.SelectedItem as string;
                                    if (!string.IsNullOrWhiteSpace(assignedTo) && assignedTo != Globals.c_All)
                                    {
                                        StoreItem fItem = filteredItem;
                                        PlannerBugItem bugItem = (PlannerBugItem)fItem;
                                        if (bugItem != null)
                                        {
                                            if (!Utils.StringsMatch(bugItem.BugAssignedTo, assignedTo))
                                            {
                                                filteredItem = null;
                                            }
                                        }
                                    }
                                    break;

                                case PlannerNameSpace.ItemTypeID.FilterResolution:
                                    string resolution = filterInfo.Combo.SelectedItem as string;
                                    if (resolution == Globals.c_None)
                                    {
                                        if (!string.IsNullOrWhiteSpace(filteredItem.Resolution))
                                        {
                                            filteredItem = null;
                                        }
                                    }
                                    else if (!string.IsNullOrWhiteSpace(resolution) && resolution != Globals.c_All)
                                    {
                                        if (!Utils.StringsMatch(filteredItem.Resolution, resolution))
                                        {
                                            filteredItem = null;
                                        }
                                    }
                                    break;

                                case PlannerNameSpace.ItemTypeID.FilterForecastable:
                                    string forecastMode = filterInfo.Combo.SelectedValue as string;
                                    if (forecastMode != Globals.c_All)
                                    {
                                        ForecastableItem forecastableItem = (ForecastableItem)(StoreItem)filteredItem;
                                        if (forecastMode == ForecastMode.Forecastable)
                                        {
                                            if (!forecastableItem.IsForecastable)
                                            {
                                                filteredItem = null;
                                            }
                                        }
                                        else
                                        {
                                            if (forecastableItem.IsForecastable)
                                            {
                                                filteredItem = null;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    if (filteredItem != null)
                    {
                        ItemsControlItems.Add(filteredItem);
                    }
                }

                ItemsControl.ItemsSource = ItemsControlItems;
            }

            SetSortingCriteria();

            // Call the host to notify them that the filtering for items
            // in the view has been updated.
            if (ItemsFilterUpdated != null)
            {
                ItemsFilterUpdated();
            }
        }
    }
}
