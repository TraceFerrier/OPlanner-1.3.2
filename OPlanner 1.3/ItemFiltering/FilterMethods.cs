using System.Collections.Generic;

namespace PlannerNameSpace
{
    public partial class FilteredCollectionManager
    {
        public AsyncObservableCollection<ScenarioItem> GetPillarScenarioItems(PillarItem pillarItem)
        {
            return PillarScenarioItems.GetItemsByParent(pillarItem);
        }

        public AsyncObservableCollection<ScenarioItem> GetPillarScenarioItems(PillarItem pillarItem, DummyItemType collectionType)
        {
            AsyncObservableCollection<ScenarioItem> items = PillarScenarioItems.GetItemsByParent(pillarItem).ToCollection();
            if (collectionType == DummyItemType.NoneType || collectionType == DummyItemType.AllNoneType)
            {
                ScenarioItem item = StoreItem.GetDummyItem<ScenarioItem>(DummyItemType.NoneType);
                items.Insert(0, item);
            }

            if (collectionType == DummyItemType.AllType || collectionType == DummyItemType.AllNoneType)
            {
                ScenarioItem item = StoreItem.GetDummyItem<ScenarioItem>(DummyItemType.AllType);
                items.Insert(0, item);
            }

            return items;
        }

        public AsyncObservableCollection<ScenarioItem> GetScenarioItemsSortedByPillar(DummyItemType collectionType)
        {
            AsyncObservableCollection<ScenarioItem> sortedItems = new AsyncObservableCollection<ScenarioItem>();

            List<PillarItem> pillarItems = PillarManager.PillarItems.ToList();
            foreach (PillarItem pillarItem in pillarItems)
            {
                AsyncObservableCollection<ScenarioItem> pillarScenarioItems = GetPillarScenarioItems(pillarItem);
                pillarScenarioItems.Sort((x, y) => x.Title.CompareTo(y.Title));
                foreach (ScenarioItem item in pillarScenarioItems)
                {
                    sortedItems.Add(item);
                }
            }

            if (collectionType == DummyItemType.NoneType || collectionType == DummyItemType.AllNoneType)
            {
                ScenarioItem item = StoreItem.GetDummyItem<ScenarioItem>(DummyItemType.NoneType);
                sortedItems.Insert(0, item);
            }

            if (collectionType == DummyItemType.AllType || collectionType == DummyItemType.AllNoneType)
            {
                ScenarioItem item = StoreItem.GetDummyItem<ScenarioItem>(DummyItemType.AllType);
                sortedItems.Insert(0, item);
            }

            return sortedItems;
        }
    }
}
