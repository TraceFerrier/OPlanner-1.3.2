using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using PlannerNameSpace.Views;

namespace PlannerNameSpace
{
    public partial class ExperienceItem : ForecastableItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.Experience; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        public static void CreateExperienceItem(PillarItem parentPillarItem, PersonaItem personaItem)
        {
            ExperienceItem newItem = ScheduleStore.Instance.CreateStoreItem<ExperienceItem>(ItemTypeID.Experience);
            newItem.Title = "New Experience";
            newItem.ParentPillarItem1 = parentPillarItem;
            newItem.ExperiencePersonaItem = personaItem;
            newItem.BeginSaveImmediate();
            newItem.SaveImmediate();
        }

        public void ShowExperienceEditor()
        {
            ExperienceItemEditorDialog dialog = new ExperienceItemEditorDialog(this);
            dialog.ShowDialog();
        }

        public override ForecastableItem ParentForecastableItem
        {
            get { return null; }
        }


        public override AsyncObservableCollection<ForecastableItem> ForecastableChildren
        {
            get
            {
                AsyncObservableCollection<ForecastableItem> items = new AsyncObservableCollection<ForecastableItem>();
                foreach (ScenarioItem item in ScenarioItems)
                {
                    items.Add(item);
                }

                return items;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the collection of all ScenarioItems for this Experience, filtered by the
        /// current filter settings for the ExperiencesView.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<ScenarioItem> ExperienceViewScenarioItems
        {
            get
            {
                AsyncObservableCollection<ScenarioItem> filteredItems = new AsyncObservableCollection<ScenarioItem>();
                AsyncObservableCollection<ScenarioItem> items = ScenarioItems.ToCollection();
                foreach (ScenarioItem item in items)
                {
                    if (ExperiencesView.SelectedQuarterItem == null ||
                        item.ParentQuarterItem == ExperiencesView.SelectedQuarterItem ||
                        ExperiencesView.SelectedQuarterItem.IsDummyItem)
                    {
                        if (ExperiencesView.SelectedPillarItem == null ||
                            item.ParentPillarItem == ExperiencesView.SelectedPillarItem ||
                            ExperiencesView.SelectedPillarItem.IsDummyItem)
                        {
                            filteredItems.Add(item);
                        }
                    }
                }

                if (Utils.StringsMatch(ExperiencesView.SortProperty, "Title"))
                {
                    filteredItems.Sort((x, y) => x.Title.CompareTo(y.Title));
                }
                else if (Utils.StringsMatch(ExperiencesView.SortProperty, "BusinessRank"))
                {
                    filteredItems.Sort((x, y) => x.BusinessRank.CompareTo(y.BusinessRank));
                }

                return filteredItems;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the collection of all ScenarioItems for this Experience, filtered by the
        /// current filter settings for the ExperiencesView.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<ScenarioItem> GetTreeViewScenarioItems(PillarItem pillarItem, QuarterItem quarterItem, string sortProperty)
        {
            {
                AsyncObservableCollection<ScenarioItem> filteredItems = new AsyncObservableCollection<ScenarioItem>();
                AsyncObservableCollection<ScenarioItem> items = ScenarioItems.ToCollection();
                foreach (ScenarioItem item in items)
                {
                    if (quarterItem == null ||
                        item.ParentQuarterItem == quarterItem ||
                        quarterItem.IsDummyItem)
                    {
                        if (pillarItem == null ||
                            item.ParentPillarItem == pillarItem ||
                            pillarItem.IsDummyItem)
                        {
                            filteredItems.Add(item);
                        }
                    }
                }

                if (Utils.StringsMatch(sortProperty, "Title"))
                {
                    filteredItems.Sort((x, y) => x.Title.CompareTo(y.Title));
                }
                else if (Utils.StringsMatch(sortProperty, "BusinessRank"))
                {
                    filteredItems.Sort((x, y) => x.BusinessRank.CompareTo(y.BusinessRank));
                }

                return filteredItems;
            }
        }

        public override int BusinessRank
        {
            get { return GetIntValue(Datastore.PropNameExperienceBusinessRank); }
            set { SetIntValue(Datastore.PropNameExperienceBusinessRank, value); }
        }

        public override ChildState GetChildState()
        {
            AsyncObservableCollection<ScenarioItem> items = ExperienceViewScenarioItems;
            return items.Count == 0 ? ChildState.HasNoChildren : ChildState.HasChildren;
        }

        public override bool MatchesParentPillarItem(PillarItem pillarItem)
        {
            if (pillarItem == ParentPillarItem1 || pillarItem == ParentPillarItem2 || pillarItem == ParentPillarItem3)
            {
                return true;
            }

            return false;
        }

        public override PersonaItem GetParentPersonaItem()
        {
            return ExperiencePersonaItem;
        }

        public string PersonaTitle { get { return ExperiencePersonaItem == null ? Globals.c_None : ExperiencePersonaItem.Title; } }

        public PersonaItem ExperiencePersonaItem
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ExperiencePersonaItemKey))
                {
                    return StoreItem.GetDummyItem<PersonaItem>(DummyItemType.NoneType);
                }
                else
                {
                    return Globals.ItemManager.GetItem<PersonaItem>(ExperiencePersonaItemKey);
                }
            }

            set
            {
                if (value == null || value.IsDummyItem)
                {
                    ExperiencePersonaItemKey = null;
                }
                else
                {
                    ExperiencePersonaItemKey = value.StoreKey;
                }
            }
        }

        public string ExperiencePersonaItemKey
        {
            get
            {
                return GetStringValue(Datastore.PropNameExperiencePersonaKey);
            }

            set
            {
                SetStringValue(Datastore.PropNameExperiencePersonaKey, value);
                NotifyPropertyChanged(() => ExperiencePersonaItem);
            }
        }

        public string ParentPillarItem1Key
        {
            get
            {
                return GetStringValue(Datastore.PropNameParentPillar1);
            }

            set
            {
                SetStringValue(Datastore.PropNameParentPillar1, value);
                NotifyPropertyChanged(() => ParentPillarItem1);
                NotifyPropertyChanged(() => ParentPillar1Title);
            }
        }

        public string ParentPillarItem2Key
        {
            get
            {
                return GetStringValue(Datastore.PropNameParentPillar2);
            }

            set
            {
                SetStringValue(Datastore.PropNameParentPillar2, value);
                NotifyPropertyChanged(() => ParentPillarItem2);
                NotifyPropertyChanged(() => ParentPillar2Title);
            }
        }

        public string ParentPillarItem3Key
        {
            get
            {
                return GetStringValue(Datastore.PropNameParentPillar3);
            }

            set
            {
                SetStringValue(Datastore.PropNameParentPillar3, value);
                NotifyPropertyChanged(() => ParentPillarItem3);
                NotifyPropertyChanged(() => ParentPillar3Title);
            }
        }

        public string ParentPillar1Title { get { return ParentPillarItem1 == null ? Globals.c_None : ParentPillarItem1.Title; } }
        public string ParentPillar2Title { get { return ParentPillarItem2 == null ? Globals.c_None : ParentPillarItem2.Title; } }
        public string ParentPillar3Title { get { return ParentPillarItem3 == null ? Globals.c_None : ParentPillarItem3.Title; } }

        public PillarItem ParentPillarItem1
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ParentPillarItem1Key))
                {
                    return StoreItem.GetDummyItem<PillarItem>(DummyItemType.NoneType);
                }
                else
                {
                    return Globals.ItemManager.GetItem<PillarItem>(ParentPillarItem1Key);
                }
            }

            set
            {
                if (value == null || value.IsDummyItem)
                {
                    ParentPillarItem1Key = null;
                }
                else
                {
                    ParentPillarItem1Key = value.StoreKey;
                }
            }
        }

        public PillarItem ParentPillarItem2
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ParentPillarItem2Key))
                {
                    return StoreItem.GetDummyItem<PillarItem>(DummyItemType.NoneType);
                }
                else
                {
                    return Globals.ItemManager.GetItem<PillarItem>(ParentPillarItem2Key);
                }
            }

            set
            {
                if (value == null || value.IsDummyItem)
                {
                    ParentPillarItem2Key = null;
                }
                else
                {
                    ParentPillarItem2Key = value.StoreKey;
                }
            }
        }

        public PillarItem ParentPillarItem3
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ParentPillarItem3Key))
                {
                    return StoreItem.GetDummyItem<PillarItem>(DummyItemType.NoneType);
                }
                else
                {
                    return Globals.ItemManager.GetItem<PillarItem>(ParentPillarItem3Key);
                }
            }

            set
            {
                if (value == null || value.IsDummyItem)
                {
                    ParentPillarItem3Key = null;
                }
                else
                {
                    ParentPillarItem3Key = value.StoreKey;
                }
            }
        }

        public AsyncObservableCollection<PillarItem> ParentPillarItems
        {
            get
            {
                AsyncObservableCollection<PillarItem> pillarItems = new AsyncObservableCollection<PillarItem>();
                PillarItem pillarItem = Globals.ItemManager.GetItem<PillarItem>(ParentPillarItem1Key);
                if (pillarItem != null)
                {
                    pillarItems.Add(pillarItem);
                }

                pillarItem = Globals.ItemManager.GetItem<PillarItem>(ParentPillarItem2Key);
                if (pillarItem != null)
                {
                    pillarItems.Add(pillarItem);
                }

                pillarItem = Globals.ItemManager.GetItem<PillarItem>(ParentPillarItem3Key);
                if (pillarItem != null)
                {
                    pillarItems.Add(pillarItem);
                }

                return pillarItems;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a ContextMenu suitable for the options available for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void PopulateContextMenu(Window ownerWindow, ContextMenu menu)
        {
            AddContextMenuItem(menu, "Edit...", "Edit.png", Edit_Click);
            AddForecastableContextMenuItems(menu);
            AddContextMenuItem(menu, "Tree View...", "TreeView.png", TreeView_Click);
            AddContextMenuItem(menu, "Delete...", "Delete.png", Delete_Click);
            AddContextMenuItem(menu, "New Scenario", "NewScenario.png", NewScenario_Click);
        }

        void TreeView_Click(object sender, RoutedEventArgs e)
        {
            ShowTreeView();
        }

        void Edit_Click(object sender, RoutedEventArgs e)
        {
            ShowExperienceEditor();
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            RequestDeleteItem();
        }

        void NewScenario_Click(object sender, RoutedEventArgs e)
        {
            PillarItem parentPillarItem = ParentPillarItem1;
            if (parentPillarItem == null)
            {
                parentPillarItem = ParentPillarItem2;
                if (parentPillarItem == null)
                {
                    parentPillarItem = ParentPillarItem3;
                }
            }

            QuarterItem parentQuarterItem = PlannerNameSpace.Views.ExperiencesView.SelectedQuarterItem;
            if (parentQuarterItem.IsDummyItem)
            {
                parentQuarterItem = null;
            }

            ScenarioItem.CreateScenarioItem(parentPillarItem, this, parentQuarterItem);
        }

    }
}
