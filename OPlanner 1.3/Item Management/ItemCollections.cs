using System;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public partial class ItemManager
    {
        public AsyncObservableCollection<ExperienceItem> ExperienceItems;
        public GroupMemberItemCache GroupMemberItems;
        public AsyncObservableCollection<HelpContentItem> HelpItems;
        public AsyncObservableCollection<PersonaItem> PersonaItems;
        public AsyncObservableCollection<PlannerBugItem> PlannerBugItems;
        public AsyncObservableCollection<ProductGroupItem> ProductGroupItems;
        public AsyncObservableCollection<QuarterItem> QuarterItems;
        public AsyncObservableCollection<ScenarioItem> ScenarioItems;
        public AsyncObservableCollection<TrainItem> TrainItems;


        public void InitializeCaches()
        {
            HelpItems = new AsyncObservableCollection<HelpContentItem>();
            GroupMemberItems = new GroupMemberItemCache();
            TrainItems = new AsyncObservableCollection<TrainItem>();

            // Item Caches
            ScenarioItems = new AsyncObservableCollection<ScenarioItem>();
            QuarterItems = new AsyncObservableCollection<QuarterItem>();
            ProductGroupItems = new AsyncObservableCollection<ProductGroupItem>();
            ExperienceItems = new AsyncObservableCollection<ExperienceItem>();
            PersonaItems = new AsyncObservableCollection<PersonaItem>();
            PlannerBugItems = new AsyncObservableCollection<PlannerBugItem>();
        }

        void AddToItemTypeCache(StoreItem item)
        {
            switch (item.StoreItemType)
            {
                case ItemTypeID.Quarter:
                    QuarterItems.Add((QuarterItem)item);
                    break;
                case ItemTypeID.Train:
                    TrainItem trainItem = (TrainItem)item;
                    if (trainItem.EndDate.AddDays(45) >= DateTime.Today)
                    {
                        TrainItems.Add((TrainItem)item);
                    }
                    break;
                case ItemTypeID.ProductGroup:
                    ProductGroupItems.Add((ProductGroupItem)item);
                    break;
                case ItemTypeID.Experience:
                    ExperienceItems.Add((ExperienceItem)item);
                    break;
                case ItemTypeID.Scenario:
                    ScenarioItems.Add((ScenarioItem)item);
                    break;
                case ItemTypeID.Persona:
                    PersonaItems.Add((PersonaItem)item);
                    break;
                case ItemTypeID.HelpContent:
                    HelpItems.Add((HelpContentItem)item);
                    break;
                case ItemTypeID.PlannerBug:
                    PlannerBugItems.Add((PlannerBugItem)item);
                    break;
                case ItemTypeID.GroupMember:
                    GroupMemberItems.Add((GroupMemberItem)item);
                    break;
            }
        }

        void RemoveFromItemTypeCache(StoreItem item)
        {
            switch (item.StoreItemType)
            {
                case ItemTypeID.Quarter:
                    QuarterItems.Remove((QuarterItem)item);
                    break;
                case ItemTypeID.Train:
                    TrainItems.Remove((TrainItem)item);
                    break;
                case ItemTypeID.ProductGroup:
                    ProductGroupItems.Remove((ProductGroupItem)item);
                    break;
                case ItemTypeID.Experience:
                    ExperienceItems.Remove((ExperienceItem)item);
                    break;
                case ItemTypeID.Scenario:
                    ScenarioItems.Remove((ScenarioItem)item);
                    break;
                case ItemTypeID.Persona:
                    PersonaItems.Remove((PersonaItem)item);
                    break;
                case ItemTypeID.HelpContent:
                    HelpItems.Remove((HelpContentItem)item);
                    break;
                case ItemTypeID.PlannerBug:
                    PlannerBugItems.Remove((PlannerBugItem)item);
                    break;
                case ItemTypeID.GroupMember:
                    GroupMemberItems.Remove((GroupMemberItem)item);
                    break;
            }
        }

    }

}
