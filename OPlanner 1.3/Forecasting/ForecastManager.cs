using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;

namespace PlannerNameSpace
{
    public enum CurrentTrainForecast
    {
        Committed,
        Targeted,
        OutOfScope,
        NotSet,
    }

    public enum CantForecastReasons
    {
        NoBusinessRank,
        NoStoryPoints,
        NoStoryPointsAndNoBusinessRank,
        NoAssignedTrain,
        BacklogItemNotForecastable,
        ScenarioItemNotForecastable,
        ScenarioItemHasNoBacklogItems,
        ExperienceHasNoScenarioItems,
    }

    public class ForecastMode
    {
        public const string Forecastable = "Forecastable";
        public const string NotForecastable = "Not Forecastable";
        public const string ForecastToolTip = "This is the estimated train in which all the dependencies of this item are forecast to be fully complete and shippable.";
    }

    public sealed class ForecastManager
    {
        private static readonly ForecastManager m_instance = new ForecastManager();

        public static ForecastManager Instance
        {
            get { return m_instance; }
        }

        public Dictionary<CantForecastReasons, string> QuarterReasons { get; set; }
        public Dictionary<CantForecastReasons, string> TrainReasons { get; set; }
        bool hasInitialForecastStarted { get; set; }
        bool IsForecastInProgress { get; set; }
        bool HasForecastingComputationCompleted { get; set; }
        bool IsForecastComputationInProgress { get; set; }

        public ForecastManager()
        {
            hasInitialForecastStarted = false;
            IsForecastComputationInProgress = false;
            Globals.EventManager.UpdateUI += Handle_UpdateUI;
            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;

            QuarterReasons = new Dictionary<CantForecastReasons, string>();
            TrainReasons = new Dictionary<CantForecastReasons, string>();

            TrainReasons.Add(CantForecastReasons.NoBusinessRank, "Forecasting for this item cannot be completed because a Business Rank has not been assigned.");
            TrainReasons.Add(CantForecastReasons.NoStoryPoints, "Forecasting for this item cannot be completed because a Story Points value greater than zero has not been assigned.");
            TrainReasons.Add(CantForecastReasons.NoStoryPointsAndNoBusinessRank, "Before forecasting for this item can be completed, values for Business Rank and Story Points must be assigned.");
            TrainReasons.Add(CantForecastReasons.NoAssignedTrain, "Before forecasting can be completed, this item must be assigned to a Train.");
            TrainReasons.Add(CantForecastReasons.BacklogItemNotForecastable, "Forecasting cannot be completed, because at least one of the backlog items assigned to this scenario is not forecastable.");
            TrainReasons.Add(CantForecastReasons.ScenarioItemNotForecastable, "Forecasting cannot be completed, because at least one of the scenarios assigned to this experience is not forecastable.");
            TrainReasons.Add(CantForecastReasons.ScenarioItemHasNoBacklogItems, "Forecasting cannot be completed, because no backlog items have been assigned to this scenario.");
            TrainReasons.Add(CantForecastReasons.ExperienceHasNoScenarioItems, "Forecasting cannot be completed, because no scenarios have been assigned to this experience.");
        }

        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            if (!Globals.ItemManager.IsCommitInProgress)
            {
                // TODO
                // StartForecastOnItemChange(e.Change.Item);
            }
        }

        void StartForecastOnItemChange(StoreItem item)
        {
            switch (item.StoreItemType)
            {
                case ItemTypeID.Experience:
                case ItemTypeID.Scenario:
                case ItemTypeID.BacklogItem:
                    StartForecastStatusComputation();
                    StartForecast();
                    break;
            }
        }

        void Handle_UpdateUI()
        {
            if (!hasInitialForecastStarted && Globals.ItemManager.IsDiscoveryComplete)
            {
                hasInitialForecastStarted = true;

                StartForecastStatusComputation();
                StartForecast();
            }

            if (HasForecastingComputationCompleted)
            {
                HasForecastingComputationCompleted = false;
                Globals.EventManager.OnForecastingComputationComplete();
            }
        }

        void StartForecast()
        {
            if (!IsForecastInProgress)
            {
                IsForecastInProgress = true;
                BackgroundWorker forecastWorker = new BackgroundWorker();
                forecastWorker.DoWork += forecastWorker_DoWork;
                forecastWorker.RunWorkerCompleted += forecastWorker_RunWorkerCompleted;
                forecastWorker.RunWorkerAsync();
            }
        }

        void StartForecastStatusComputation()
        {
            if (!IsForecastComputationInProgress)
            {
                IsForecastComputationInProgress = true;
                HasForecastingComputationCompleted = false;
                BackgroundWorker forecastComputationWorker = new BackgroundWorker();
                forecastComputationWorker.DoWork += forecastComputationWorker_DoWork;
                forecastComputationWorker.RunWorkerCompleted += forecastComputationWorker_RunWorkerCompleted;
                forecastComputationWorker.RunWorkerAsync();
            }
        }

        void forecastComputationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsForecastComputationInProgress = false;
            HasForecastingComputationCompleted = true;
        }

        void forecastComputationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Update the various status values for all items in the forecast tree
            List<BacklogItem> backlogItems = BacklogItem.Items.ToList();
            foreach (BacklogItem backlogItem in backlogItems)
            {
                backlogItem.UpdateSpecStatus(false);
                backlogItem.UpdateDesignStatus(false);
                backlogItem.UpdateCompletionStatus(false);
            }

            foreach (BacklogItem backlogItem in backlogItems)
            {
                backlogItem.NotifySpecStatusChanged(false);
                backlogItem.NotifyDesignStatusChanged(false);
                backlogItem.NotifyCompletionStatusChanged(false);
            }

            AsyncObservableCollection<ScenarioItem> scenarioItems = Globals.ItemManager.ScenarioItems;
            foreach (ScenarioItem scenarioItem in scenarioItems)
            {
                scenarioItem.UpdateSpecStatus(false);
                scenarioItem.NotifySpecStatusChanged(false);

                scenarioItem.UpdateDesignStatus(false);
                scenarioItem.NotifyDesignStatusChanged(false);

                scenarioItem.UpdateCompletionStatus(false);
                scenarioItem.NotifyCompletionStatusChanged(false);

                scenarioItem.UpdateChildCounts();
            }

            AsyncObservableCollection<ExperienceItem> experienceItems = Globals.ItemManager.ExperienceItems;
            foreach (ExperienceItem experienceItem in experienceItems)
            {
                experienceItem.UpdateSpecStatus(false);
                experienceItem.NotifySpecStatusChanged(false);

                experienceItem.UpdateDesignStatus(false);
                experienceItem.NotifyDesignStatusChanged(false);

                experienceItem.UpdateCompletionStatus(false);
                experienceItem.NotifyCompletionStatusChanged(false);

                experienceItem.UpdateChildCounts();
            }

        }

        void forecastWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsForecastInProgress = false;
        }

        void forecastWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // First, gather all forecastable backlog items (all those items in future trains that are fully groomed)
            Dictionary<string, AsyncObservableCollection<BacklogItem>> forecastableBacklogItems = new Dictionary<string, AsyncObservableCollection<BacklogItem>>();
            List<BacklogItem> allBacklogItems = BacklogItem.Items.ToList();
            foreach (BacklogItem backlogItem in allBacklogItems)
            {
                TrainItem trainItem = backlogItem.ParentTrainItem;
                if (trainItem != null)
                {
                    if (trainItem.TimeFrame == TrainTimeFrame.Past)
                    {
                        backlogItem.ForecastedFinishTrain = trainItem;
                    }
                    else if (trainItem.TimeFrame == TrainTimeFrame.Current)
                    {
                        //if (backlogItem.CurrentDevTrainForecast != CurrentTrainForecast.OutOfScope)
                        //{
                        //    if (backlogItem.CurrentTestTrainForecast != CurrentTrainForecast.OutOfScope)
                        //    {
                        //        backlogItem.ForecastedFinishTrain = Globals.GlobalItemCache.TrainItems.CurrentTrain;
                        //    }
                        //}
                    }
                    else if (trainItem.TimeFrame == TrainTimeFrame.Future || trainItem.TimeFrame == TrainTimeFrame.Unassigned)
                    {
                        if (backlogItem.StoryPoints > 0 && backlogItem.BusinessRank > 0 && backlogItem.ParentPillarItem != null)
                        {
                            string key = backlogItem.ParentPillarItem.StoreKey;
                            if (!forecastableBacklogItems.ContainsKey(key))
                            {
                                forecastableBacklogItems.Add(key, new AsyncObservableCollection<BacklogItem>());
                            }

                            AsyncObservableCollection<BacklogItem> pillarBacklogItems = forecastableBacklogItems[key];
                            pillarBacklogItems.Add(backlogItem);
                        }
                        else
                        {
                            backlogItem.ForecastedFinishTrain = null;
                            if (backlogItem.StoryPoints == 0 && backlogItem.BusinessRank > 0)
                            {
                                backlogItem.CantForecastReason = CantForecastReasons.NoStoryPoints;
                            }
                            else if (backlogItem.StoryPoints > 0 && backlogItem.BusinessRank == 0)
                            {
                                backlogItem.CantForecastReason = CantForecastReasons.NoStoryPointsAndNoBusinessRank;
                            }
                            else
                            {
                                backlogItem.CantForecastReason = CantForecastReasons.NoStoryPointsAndNoBusinessRank;
                            }
                        }
                    }
                    else
                    {
                        backlogItem.ForecastedFinishTrain = null;
                        backlogItem.CantForecastReason = CantForecastReasons.NoAssignedTrain;
                    }
                }
                else
                {
                    backlogItem.ForecastedFinishTrain = null;
                    backlogItem.CantForecastReason = CantForecastReasons.NoAssignedTrain;
                }
            }

            // Perform forecasting of each backlog item, based on the average story point velocity per pillar
            foreach (KeyValuePair<string, AsyncObservableCollection<BacklogItem>> kvp in forecastableBacklogItems)
            {
                string pillarKey = kvp.Key;
                AsyncObservableCollection<BacklogItem> pillarBacklogItems = kvp.Value;

                // Make sure all forecastable items are in business rank order
                pillarBacklogItems.Sort((x, y) => x.BusinessRank.CompareTo(y.BusinessRank));

                PillarItem pillarItem = Globals.ItemManager.GetItem<PillarItem>(pillarKey);
                if (pillarItem != null)
                {
                    int velocityPerTrain = pillarItem.AverageTrainVelocity;
                    if (velocityPerTrain > 0)
                    {
                        int storyPointsUsedThisTrain = 0;
                        TrainItem forecastTrain = TrainManager.Instance.GetNextTrain(TrainManager.Instance.CurrentTrain);
                        foreach (BacklogItem backlogItem in pillarBacklogItems)
                        {
                            storyPointsUsedThisTrain += backlogItem.StoryPoints;
                            if (storyPointsUsedThisTrain <= velocityPerTrain)
                            {
                                backlogItem.ForecastedFinishTrain = forecastTrain;
                            }
                            else
                            {
                                storyPointsUsedThisTrain = 0;
                                forecastTrain = TrainManager.Instance.GetNextTrain(forecastTrain);
                                backlogItem.ForecastedFinishTrain = forecastTrain;
                            }

                        }
                    }
                }
            }

            // Forecasting for Scenarios
            AsyncObservableCollection<ScenarioItem> scenarioItems = Globals.ItemManager.ScenarioItems.ToCollection();
            foreach (ScenarioItem scenarioItem in scenarioItems)
            {
                string scenarioTitle = scenarioItem.Title;
                AsyncObservableCollection<BacklogItem> backlogItems = scenarioItem.BacklogItems.ToCollection();
                if (backlogItems.Count == 0)
                {
                    scenarioItem.CantForecastReason = CantForecastReasons.ScenarioItemHasNoBacklogItems;
                }
                else
                {
                    foreach (BacklogItem backlogItem in backlogItems)
                    {
                        if (!backlogItem.IsForecastable)
                        {
                            scenarioItem.ForecastedFinishTrain = null;
                            scenarioItem.CantForecastReason = CantForecastReasons.BacklogItemNotForecastable;
                            break;
                        }
                        else
                        {
                            if (scenarioItem.ForecastedFinishTrain == null || backlogItem.ForecastedFinishTrain > scenarioItem.ForecastedFinishTrain)
                            {
                                scenarioItem.ForecastedFinishTrain = backlogItem.ForecastedFinishTrain;
                            }
                        }
                    }
                }
            }

            // Forecasting for Experiences
            AsyncObservableCollection<ExperienceItem> experienceItems = Globals.ItemManager.ExperienceItems.ToCollection();
            foreach (ExperienceItem experienceItem in experienceItems)
            {
                AsyncObservableCollection<ScenarioItem> experienceScenarioItems = experienceItem.ScenarioItems.ToCollection();
                if (experienceScenarioItems.Count == 0)
                {
                    experienceItem.CantForecastReason = CantForecastReasons.ExperienceHasNoScenarioItems;
                }
                else
                {
                    foreach (ScenarioItem scenarioItem in experienceScenarioItems)
                    {
                        if (!scenarioItem.IsForecastable)
                        {
                            experienceItem.ForecastedFinishTrain = null;
                            experienceItem.CantForecastReason = CantForecastReasons.ScenarioItemNotForecastable;
                            break;
                        }
                        else
                        {
                            if (experienceItem.ForecastedFinishTrain == null || scenarioItem.ForecastedFinishTrain > experienceItem.ForecastedFinishTrain)
                            {
                                experienceItem.ForecastedFinishTrain = scenarioItem.ForecastedFinishTrain;
                            }
                        }
                    }
                }
            }
        }
    }
}
