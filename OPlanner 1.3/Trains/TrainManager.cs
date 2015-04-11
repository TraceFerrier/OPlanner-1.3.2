using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public sealed class TrainManager
    {
        private static readonly TrainManager m_instance = new TrainManager();

        public static TrainManager Instance
        {
            get { return m_instance; }
        }

        private TrainManager()
        {
            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;
        }

        static AsyncObservableCollection<TrainItem> m_sortedTrainItems;
        static AsyncObservableCollection<TrainItem> m_trainsWithNone;
        static AsyncObservableCollection<TrainItem> m_trainsWithAllNone;

        // Triggering these static events will magically update the binding for
        // any controls bound to the respective collections.
        public static event EventHandler TrainItemsChanged;
        public static event EventHandler TrainsWithNoneChanged;
        public static event EventHandler TrainsWithAllNoneChanged;

        private TrainItem m_currentTrain;
        DateTime m_currentTrainDate;

        public static AsyncObservableCollection<TrainItem> TrainItems
        {
            get 
            {
                if (m_sortedTrainItems == null)
                {
                    m_sortedTrainItems = Globals.ItemManager.TrainItems.ToCollection();
                    m_sortedTrainItems.Sort((x, y) => x.EndDate.CompareTo(y.EndDate));
                }

                return m_sortedTrainItems; 
            }
        }

        public static AsyncObservableCollection<TrainItem> TrainsWithNone
        {
            get
            {
                if (m_trainsWithNone == null)
                {
                    m_trainsWithNone = TrainItems.ToCollection();
                    m_trainsWithNone.Insert(0, StoreItem.GetDummyItem<TrainItem>(DummyItemType.NoneType));
                    m_trainsWithNone.Insert(1, TrainItem.BacklogTrainItem);
                }

                return m_trainsWithNone;
            }
        }

        public static AsyncObservableCollection<TrainItem> TrainsWithAllNone
        {
            get
            {
                if (m_trainsWithAllNone == null)
                {
                    m_trainsWithAllNone = TrainItems.ToCollection();
                    m_trainsWithAllNone.Insert(0, StoreItem.GetDummyItem<TrainItem>(DummyItemType.AllType));
                    m_trainsWithAllNone.Insert(1, StoreItem.GetDummyItem<TrainItem>(DummyItemType.NoneType));
                    m_trainsWithAllNone.Insert(2, TrainItem.BacklogTrainItem);
                }

                return m_trainsWithAllNone;
            }
        }

        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            if (e.Change.Item.StoreItemType == ItemTypeID.Train)
            {
                UpdateTrainCollections();
            }
        }

        void UpdateTrainCollections()
        {
            m_currentTrain = null;
            m_sortedTrainItems = null;
            m_trainsWithNone = null;
            m_trainsWithAllNone = null;

            if (TrainItemsChanged != null)
            {
                TrainItemsChanged(null, EventArgs.Empty);
            }

            if (TrainsWithNoneChanged != null)
            {
                TrainsWithNoneChanged(null, EventArgs.Empty);
            }

            if (TrainsWithAllNoneChanged != null)
            {
                TrainsWithAllNoneChanged(null, EventArgs.Empty);
            }
        }

        public AsyncObservableCollection<TrainItem> GetAvailableTrains(DummyItemType populationMode = DummyItemType.AllType)
        {
            AsyncObservableCollection<TrainItem> items = TrainItems.ToCollection();
            items.Insert(0, TrainItem.BacklogTrainItem);

            if (populationMode == DummyItemType.NoneType || populationMode == DummyItemType.AllNoneType)
            {
                TrainItem item = StoreItem.GetDummyItem<TrainItem>(DummyItemType.NoneType);
                items.Insert(0, item);
            }

            if (populationMode == DummyItemType.AllType || populationMode == DummyItemType.AllNoneType)
            {
                TrainItem item = StoreItem.GetDummyItem<TrainItem>(DummyItemType.AllType);
                items.Insert(0, item);
            }

            return items;
        }

        public AsyncObservableCollection<TrainItem> GetQueryableTrains()
        {
            AsyncObservableCollection<TrainItem> trainItems = TrainItems.ToCollection();
            trainItems.Insert(0, TrainItem.BacklogTrainItem);
            return trainItems;
        }

        public TrainItem CurrentTrain
        {
            get
            {
                if (m_currentTrain == null || m_currentTrainDate != null && m_currentTrainDate.Day != DateTime.Now.Day)
                {
                    AsyncObservableCollection<TrainItem> items = TrainItems;
                    foreach (TrainItem train in items)
                    {
                        if (train.TimeFrame == TrainTimeFrame.Current && Utils.StringsMatch(train.TrainShipCycle, "Gemini"))
                        {
                            if (train.EndDate > train.StartDate)
                            {
                                m_currentTrain = train;
                                m_currentTrainDate = DateTime.Now;
                                break;
                            }
                        }
                    }
                }

                return m_currentTrain;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the given train item is currently the next train chronologically.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsNextTrain(TrainItem trainItem)
        {
            TrainItem nextTrain = GetNextTrain(CurrentTrain);
            return nextTrain == trainItem;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a train, return the next active train chronologically in time.  If no train
        /// is found, null will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public TrainItem GetNextTrain(TrainItem oldTrain)
        {
            int oldTrainIndex = TrainItems.IndexOf(oldTrain);
            if (oldTrainIndex < 0 || oldTrainIndex >= TrainItems.Count)
            {
                return null;
            }

            return TrainItems[oldTrainIndex + 1];
        }

        public AsyncObservableCollection<TrainItem> GetTrains(TrainTimeFrame timeFrame)
        {
            AsyncObservableCollection<TrainItem> trains = new AsyncObservableCollection<TrainItem>();
            foreach (TrainItem trainItem in TrainItems)
            {
                if (timeFrame == TrainTimeFrame.CurrentOrFuture)
                {
                    if (trainItem.TimeFrame == TrainTimeFrame.Current || trainItem.TimeFrame == TrainTimeFrame.Future)
                    {
                        trains.Add(trainItem);
                    }
                }
                else if (timeFrame == TrainTimeFrame.CurrentOrPast)
                {
                    if (trainItem.TimeFrame == TrainTimeFrame.Current || trainItem.TimeFrame == TrainTimeFrame.Past)
                    {
                        trains.Add(trainItem);
                    }
                }
                else
                {
                    if (trainItem.TimeFrame == timeFrame)
                    {
                        trains.Add(trainItem);
                    }
                }
            }

            return trains;
        }

        public TrainItem GetTrainByTitle(string trainText)
        {
            if (string.IsNullOrWhiteSpace(trainText))
            {
                return null;
            }

            foreach (TrainItem train in TrainItems)
            {
                if (Utils.StringsMatch(train.Title, trainText))
                {
                    return train;
                }
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given shipCycle and fixBy values, returns the TrainItem object that represents
        /// the corresponding train.
        /// </summary>
        //------------------------------------------------------------------------------------
        public TrainItem FindTrain(string shipCycle, string fixBy)
        {
            if (Utils.StringsMatch(TrainItem.BacklogTrainItem.TrainShipCycle, shipCycle) && Utils.StringsMatch(TrainItem.BacklogTrainItem.TrainFixBy, fixBy))
            {
                return TrainItem.BacklogTrainItem;
            }

            foreach (TrainItem train in TrainItems)
            {
                if (Utils.StringsMatch(train.TrainShipCycle, shipCycle) && Utils.StringsMatch(train.TrainFixBy, fixBy))
                {
                    return train;
                }
            }

            return null;
        }

        public int NextTrainSpecsOnTrackCount;
        public int NextTrainSpecsNotOnTrackCount;

        bool IsInitialNextTrainSpecStatusCalculationComplete = false;
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the count of FeatureTeams in the current train that meet the specified
        /// team status (either on track, or not on track).
        /// </summary>
        //------------------------------------------------------------------------------------
        void CalculateNextTrainSpecStatusCounts()
        {
            if (IsInitialNextTrainSpecStatusCalculationComplete)
            {
                NextTrainSpecsOnTrackCount = 0;
                NextTrainSpecsNotOnTrackCount = 0;
                List<BacklogItem> items = BacklogItem.Items.ToList();
                foreach (BacklogItem backlogItem in items)
                {
                    if (backlogItem.IsAssignedToNextTrain)
                    {
                        if (backlogItem.SpecStatus == SpecStatus.All_Specs_Current || backlogItem.SpecStatus == SpecStatus.No_Specs_Required)
                        {
                            NextTrainSpecsNotOnTrackCount++;
                        }
                        else
                        {
                            NextTrainSpecsNotOnTrackCount++;
                        }
                    }
                }
            }
        }
    }
}
