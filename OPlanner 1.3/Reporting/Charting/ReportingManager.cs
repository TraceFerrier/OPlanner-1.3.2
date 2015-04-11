using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace PlannerNameSpace
{
    public enum BurndownItemType
    {
        Product,
        Pillar,
        FeatureTeam,
    }

    [Serializable]
    public class WorkSummary
    {
        public BurndownItemType BurndownType { get; set; }
        public string CurrentDateKey { get; set; }
        public string TrainKey { get; set; }
        public string ItemKey { get; set; }
        public int WorkCompleted { get; set; }
        public int WorkRemaining { get; set; }
        public string WorkStatus { get; set; }

        public int WorkEstimate
        {
            get { return WorkCompleted + WorkRemaining; }
        }

        public DateTime CurrentDate
        {
            get
            {
                DateTime thisDate;
                if (DateTime.TryParse(CurrentDateKey, out thisDate))
                {
                    return thisDate;
                }

                return new DateTime();
            }
        }
    }

    public class ReportingManager
    {
        public bool BurndownAvailable { get; set; }
        DateTime LastDateAccumulated { get; set; }
        DateTime FirstDateToAccumulate { get; set; }

        List<WorkItem> BurndownItems;
        Dictionary<string, Dictionary<string, WorkSummary>> ProductGroupWorkSummary;
        Dictionary<string, Dictionary<string, Dictionary<string, WorkSummary>>> PillarWorkSummary;
        Dictionary<string, Dictionary<string, WorkSummary>> FeatureTeamWorkSummary;
        bool BurndownAccumulationStarted { get; set; }

        public ReportingManager()
        {
            FirstDateToAccumulate = new DateTime(2013, 3, 30);
            LastDateAccumulated = FirstDateToAccumulate;
            EventManager.Instance.IndexingComplete += Handle_AllStartupActivitiesComplete;
            BurndownAvailable = false;

            BurndownItems = new List<WorkItem>();
            ProductGroupWorkSummary = new Dictionary<string, Dictionary<string, WorkSummary>>();
            PillarWorkSummary = new Dictionary<string, Dictionary<string, Dictionary<string, WorkSummary>>>();
            FeatureTeamWorkSummary = new Dictionary<string, Dictionary<string, WorkSummary>>();
            BurndownAccumulationStarted = false;
        }

        void LoadCachedBurndownSummary()
        {
            ProductGroupItem productGroup = Globals.ProductGroupManager.GetCurrentProductGroup();
            string burndownFile = productGroup.Store.GetFullPathToFileAttachment(productGroup, "Burndown.xml");
            if (burndownFile != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<WorkSummary>));
                TextReader reader = new StreamReader(burndownFile);
                List<WorkSummary> burndownList = (List<WorkSummary>)serializer.Deserialize(reader);
                reader.Close();

                foreach (WorkSummary workSummary in burndownList)
                {
                    DateTime thisDate;
                    if (DateTime.TryParse(workSummary.CurrentDateKey, out thisDate))
                    {
                        if (thisDate > LastDateAccumulated)
                        {
                            LastDateAccumulated = thisDate;
                        }
                    }

                    switch (workSummary.BurndownType)
                    {
                        case BurndownItemType.Product:
                            if (!ProductGroupWorkSummary.ContainsKey(workSummary.TrainKey))
                            {
                                ProductGroupWorkSummary.Add(workSummary.TrainKey, new Dictionary<string,WorkSummary>());
                            }

                            Dictionary<string, WorkSummary> trainDict = ProductGroupWorkSummary[workSummary.TrainKey];
                            trainDict.Add(workSummary.CurrentDateKey, workSummary);
                            break;

                        case BurndownItemType.Pillar:
                            if (!PillarWorkSummary.ContainsKey(workSummary.ItemKey))
                            {
                                PillarWorkSummary.Add(workSummary.ItemKey, new Dictionary<string,Dictionary<string,WorkSummary>>());
                            }

                            Dictionary<string, Dictionary<string, WorkSummary>> pillarTrainDict = PillarWorkSummary[workSummary.ItemKey];
                            if (!pillarTrainDict.ContainsKey(workSummary.TrainKey))
                            {
                                pillarTrainDict.Add(workSummary.TrainKey, new Dictionary<string,WorkSummary>());
                            }

                            Dictionary<string, WorkSummary> dict = pillarTrainDict[workSummary.TrainKey];
                            dict.Add(workSummary.CurrentDateKey, workSummary);
                            break;

                        case BurndownItemType.FeatureTeam:
                            if (!FeatureTeamWorkSummary.ContainsKey(workSummary.ItemKey))
                            {
                                FeatureTeamWorkSummary.Add(workSummary.ItemKey, new Dictionary<string, WorkSummary>());
                            }

                            Dictionary<string, WorkSummary> ftdict = FeatureTeamWorkSummary[workSummary.ItemKey];
                            ftdict.Add(workSummary.CurrentDateKey, workSummary);
                            break;
                    }
                }

            }
        }

        void Handle_AllStartupActivitiesComplete()
        {
            //if (LastDateAccumulated < DateTime.Today)
            //{
            //    Globals.StoreQueryManager.BeginBurndownQuery();
            //}
        }


        public void OnHostDatastoreQueryComplete(bool isRefresh)
        {
            //if (!isRefresh)
            //{
            //    LoadCachedBurndownSummary();

            //    if (LastDateAccumulated == DateTime.Today)
            //    {
            //        BurndownAvailable = true;
            //    }
            //    else
            //    {
            //        if (LastDateAccumulated > FirstDateToAccumulate)
            //        {
            //            BurndownAvailable = true;
            //        }
            //    }
            //}
        }

        public void OnReportingQueryItem(StoreItem item)
        {
            WorkItem workItem = item as WorkItem;
            if (workItem != null)
            {
                BurndownItems.Add(workItem);
            }
        }

        public void OnReportingQueryComplete()
        {
            StartBurndownAccumulation();
        }

        private void StartBurndownAccumulation()
        {
            if (!BurndownAccumulationStarted)
            {
                BurndownAccumulationStarted = true;
                BackgroundWorker accumulateWorker = new BackgroundWorker();
                accumulateWorker.DoWork += accumulateWorker_DoWork;
                accumulateWorker.RunWorkerCompleted += accumulateWorker_RunWorkerCompleted;
                accumulateWorker.RunWorkerAsync();
            }
        }

        void accumulateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBurndownAccumulating = false;

            BurndownAvailable = true;

            List<WorkSummary> summaryList = new List<WorkSummary>();

            string fullFilePath = Utils.GetFullPathToTempFile("Burndown.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<WorkSummary>));
            TextWriter writer = new StreamWriter(fullFilePath);

            foreach (KeyValuePair<string, Dictionary<string, WorkSummary>> kvp in FeatureTeamWorkSummary)
            {
                Dictionary<string, WorkSummary> dict = kvp.Value;
                foreach (KeyValuePair<string, WorkSummary> innerKvp in dict)
                {
                    WorkSummary workSummary = innerKvp.Value;
                    summaryList.Add(workSummary);
                    //serializer.Serialize(writer, workSummary);
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, WorkSummary>>> kvp in PillarWorkSummary)
            {
                Dictionary<string, Dictionary<string, WorkSummary>> trainDict = kvp.Value;
                foreach (KeyValuePair<string, Dictionary<string, WorkSummary>> innerKvp in trainDict)
                {
                    Dictionary<string, WorkSummary> dayDict = innerKvp.Value;
                    foreach (KeyValuePair<string, WorkSummary> inner2Kvp in dayDict)
                    {
                        WorkSummary workSummary = inner2Kvp.Value;
                        summaryList.Add(workSummary);
                    }
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, WorkSummary>> kvp in ProductGroupWorkSummary)
            {
                Dictionary<string, WorkSummary> trainDict = kvp.Value;
                foreach (KeyValuePair<string, WorkSummary> innerKvp in trainDict)
                {
                    WorkSummary workSummary = innerKvp.Value;
                    summaryList.Add(workSummary);
                }
            }

            serializer.Serialize(writer, summaryList);
            writer.Close();
            writer.Dispose();

            ProductGroupItem productGroup = Globals.ProductGroupManager.GetCurrentProductGroup();
            if (productGroup != null)
            {
                productGroup.Store.SaveFileAttachment(productGroup, fullFilePath);
            }
        }

        void SerializeWorkSummary(WorkSummary workSummary)
        {

        }

        public DateTime CurrenBurndownDateProcessing { get; set; }
        public bool IsBurndownAccumulating { get; set; }
        public int TotalItemsForCurrentDate { get; set; }
        public int CurrentItemForCurrentDate { get; set; }

        void accumulateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            IsBurndownAccumulating = true;
            TotalItemsForCurrentDate = BurndownItems.Count;
            DateTime currentDate = DateTime.Today;

            try
            {
                while (currentDate > LastDateAccumulated)
                {
                    CurrenBurndownDateProcessing = currentDate;
                    string currentDateKey = currentDate.ToShortDateString();
                    CurrentItemForCurrentDate = 0; 
                    foreach (WorkItem workItem in BurndownItems)
                    {
                        CurrentItemForCurrentDate++;
                        if (workItem.HistoryIndex == 0)
                        {
                            int ID = workItem.ID;
                            workItem.LastChangedDate = Utils.GetValueAsLocalDate(workItem.GetValue(Datastore.PropNameChangedDate));
                            workItem.CurrentBacklogKey = workItem.ParentBacklogItemKey;
                            BacklogItem backlogItem = workItem.ParentBacklogItem;
                            if (backlogItem != null)
                            {
                                workItem.CurrentFeatureTeamKey = backlogItem.ScrumTeamKey;
                                TrainItem trainItem = backlogItem.ParentTrainItem;
                                if (trainItem != null)
                                {
                                    workItem.CurrentTrainKey = trainItem.StoreKey;
                                }
                            }

                            workItem.CurrentCompleted = workItem.Completed;
                            workItem.CurrentEstimate = workItem.Estimate;
                            workItem.CurrentStatus = workItem.Status;
                            PillarItem pillarItem = Globals.ProductGroupManager.FindOwnerPillar(workItem.TreeID);
                            if (pillarItem != null)
                            {
                                workItem.CurrentPillarKey = pillarItem.StoreKey;
                            }

                            workItem.Store.OpenForRead(workItem.DSItem);
                            ProductStudio.DatastoreItemHistory history = workItem.DSItem.History;
                            workItem.HistoryIndex = history.Count;
                        }
                        else if (workItem.LastChangedDate == currentDate)
                        {
                            workItem.Store.OpenForRead(workItem.DSItem);
                            ProductStudio.DatastoreItemHistory history = workItem.DSItem.History;
                            ProductStudio.DatastoreItem item = history.get_ItemByRevision(workItem.HistoryIndex);
                            ProductStudio.Fields fields = item.Fields;

                            if (!Utils.IsFieldValueNull(fields[Datastore.PropNameParentBacklogItemID]))
                            {
                                int parentID = Utils.GetIntValue(fields[Datastore.PropNameParentBacklogItemID].Value);
                                if (parentID != 0)
                                {
                                    workItem.CurrentBacklogKey = StoreItem.GetHostItemKey(parentID);
                                }
                            }

                            if (!Utils.IsFieldValueNull(fields[Datastore.PropNameCompleted]))
                            {
                                workItem.CurrentCompleted = Utils.GetIntValue(fields[Datastore.PropNameCompleted].Value);
                            }

                            if (!Utils.IsFieldValueNull(fields[Datastore.PropNameEstimate]))
                            {
                                workItem.CurrentEstimate = Utils.GetIntValue(fields[Datastore.PropNameEstimate].Value);
                            }

                            if (!Utils.IsFieldValueNull(fields[Datastore.PropNameStatus]))
                            {
                                workItem.CurrentStatus = Utils.GetStringValue(fields[Datastore.PropNameStatus].Value);
                            }

                            workItem.LastChangedDate = Utils.GetValueAsLocalDate(fields[Datastore.PropNameChangedDate].Value);
                            workItem.HistoryIndex--;

                            while (workItem.HistoryIndex >= 1 && workItem.LastChangedDate == currentDate)
                            {
                                item = history.get_ItemByRevision(workItem.HistoryIndex);
                                fields = item.Fields;
                                workItem.LastChangedDate = Utils.GetValueAsLocalDate(fields[Datastore.PropNameChangedDate].Value);

                                if (!Utils.IsFieldValueNull(fields[Datastore.PropNameStatus]))
                                {
                                    workItem.CurrentStatus = Utils.GetStringValue(fields[Datastore.PropNameStatus].Value);
                                }

                                workItem.HistoryIndex--;
                            }

                        }

                        // Add an entry for the overall product burndown
                        if (!string.IsNullOrWhiteSpace(workItem.CurrentTrainKey))
                        {
                            if (!ProductGroupWorkSummary.ContainsKey(workItem.CurrentTrainKey))
                            {
                                ProductGroupWorkSummary.Add(workItem.CurrentTrainKey, new Dictionary<string, WorkSummary>());
                            }

                            Dictionary<string, WorkSummary> productDict = ProductGroupWorkSummary[workItem.CurrentTrainKey];

                            if (!productDict.ContainsKey(currentDateKey))
                            {
                                productDict.Add(currentDateKey, new WorkSummary());
                            }

                            WorkSummary productWorkSummary = productDict[currentDateKey];
                            SetWorkSummary(productWorkSummary, workItem, BurndownItemType.Product, workItem.CurrentTrainKey, currentDateKey, null);
                        }

                        // Pillar burndown
                        if (workItem.CurrentPillarKey != null && workItem.CurrentTrainKey != null)
                        {
                            if (!PillarWorkSummary.ContainsKey(workItem.CurrentPillarKey))
                            {
                                PillarWorkSummary.Add(workItem.CurrentPillarKey, new Dictionary<string, Dictionary<string, WorkSummary>>());
                            }

                            Dictionary<string, Dictionary<string, WorkSummary>> trainDict = PillarWorkSummary[workItem.CurrentPillarKey];
                            if (!trainDict.ContainsKey(workItem.CurrentTrainKey))
                            {
                                trainDict.Add(workItem.CurrentTrainKey, new Dictionary<string, WorkSummary>());
                            }

                            Dictionary<string, WorkSummary> dayDict = trainDict[workItem.CurrentTrainKey];
                            if (!dayDict.ContainsKey(currentDateKey))
                            {
                                dayDict.Add(currentDateKey, new WorkSummary());
                            }

                            WorkSummary workSummary = dayDict[currentDateKey];
                            SetWorkSummary(workSummary, workItem, BurndownItemType.Pillar, workItem.CurrentTrainKey, currentDateKey, workItem.CurrentPillarKey);
                        }

                        // Feature Team burndown
                        if (workItem.CurrentFeatureTeamKey != null)
                        {
                            if (!FeatureTeamWorkSummary.ContainsKey(workItem.CurrentFeatureTeamKey))
                            {
                                FeatureTeamWorkSummary.Add(workItem.CurrentFeatureTeamKey, new Dictionary<string, WorkSummary>());
                            }

                            if (!FeatureTeamWorkSummary[workItem.CurrentFeatureTeamKey].ContainsKey(currentDateKey))
                            {
                                FeatureTeamWorkSummary[workItem.CurrentFeatureTeamKey].Add(currentDateKey, new WorkSummary());
                            }

                            WorkSummary workSummary = FeatureTeamWorkSummary[workItem.CurrentFeatureTeamKey][currentDateKey];
                            SetWorkSummary(workSummary, workItem, BurndownItemType.FeatureTeam, null, currentDateKey, workItem.CurrentFeatureTeamKey);

                        }

                    }

                    currentDate = currentDate.Subtract(new TimeSpan(1, 0, 0, 0));
                }
            }

            catch (Exception)
            {

            }

        }

        void SetWorkSummary(WorkSummary workSummary, WorkItem workItem, BurndownItemType burndownType, string currentTrainKey, string currentDateKey, string itemKey)
        {
            workSummary.BurndownType = burndownType;
            workSummary.TrainKey = currentTrainKey;
            workSummary.CurrentDateKey = currentDateKey;
            workSummary.ItemKey = itemKey;
            workSummary.WorkCompleted += workItem.CurrentCompleted;

            int workRemaining = 0;
            if (workItem.CurrentStatus == StatusValues.Active)
            {
                workRemaining = workItem.CurrentEstimate - workItem.CurrentCompleted;
                if (workRemaining < 0)
                {
                    workRemaining = 0;
                }
                workSummary.WorkRemaining += workRemaining;
            }

        }

        public void ShowPillarBurndown(PillarItem pillarItem, TrainItem trainItem)
        {
            BurndownChart chart = new BurndownChart(pillarItem, trainItem);
            chart.ShowDialog();
        }

        public void ShowFeatureTeamBurndown(ScrumTeamItem featureTeamItem)
        {
            BurndownChart chart = new BurndownChart(featureTeamItem);
            chart.ShowDialog();
        }

        public void ShowCurrentProductGroupBurndown()
        {
            ProductGroupItem productGroup = Globals.ProductGroupManager.GetCurrentProductGroup();
            BurndownChart chart = new BurndownChart(productGroup);
            chart.ShowDialog();
        }

        public int GetMaxWorkRemaining(Dictionary<string, WorkSummary> workSummary, DateTime startDate, DateTime endDate)
        {
            int maxWorkRemaining = 0;
            foreach (KeyValuePair<string, WorkSummary> kvp in workSummary)
            {
                WorkSummary summary = kvp.Value;
                DateTime summaryDate;
                DateTime.TryParse(summary.CurrentDateKey, out summaryDate);
                if (summaryDate >= startDate && summaryDate <= endDate)
                {
                    if (summary.WorkRemaining > maxWorkRemaining)
                    {
                        maxWorkRemaining = summary.WorkRemaining;
                    }
                }
            }

            return maxWorkRemaining;
        }

        public int GetMaxWorkEstimate(Dictionary<string, WorkSummary> workSummary, DateTime startDate, DateTime endDate)
        {
            int maxWorkEstimate = 0;
            foreach (KeyValuePair<string, WorkSummary> kvp in workSummary)
            {
                WorkSummary summary = kvp.Value;
                DateTime summaryDate;
                DateTime.TryParse(summary.CurrentDateKey, out summaryDate);
                if (summaryDate >= startDate && summaryDate <= endDate)
                {
                    if (summary.WorkEstimate > maxWorkEstimate)
                    {
                        maxWorkEstimate = summary.WorkEstimate;
                    }
                }
            }

            return maxWorkEstimate;
        }

        public Dictionary<string, WorkSummary> GetPillarWorkSummary(PillarItem pillarItem, TrainItem trainItem)
        {
            if (PillarWorkSummary.ContainsKey(pillarItem.StoreKey))
            {
                Dictionary<string, Dictionary<string, WorkSummary>> trainDict = PillarWorkSummary[pillarItem.StoreKey];
                if (trainDict.ContainsKey(trainItem.StoreKey))
                {
                    return trainDict[trainItem.StoreKey];
                }
            }

            return null;
        }

        public Dictionary<string, WorkSummary> GetFeatureTeamWorkSummary(ScrumTeamItem featureTeam)
        {
            if (FeatureTeamWorkSummary.ContainsKey(featureTeam.StoreKey))
            {
                return FeatureTeamWorkSummary[featureTeam.StoreKey];
            }

            return null;
        }

        public Dictionary<string, WorkSummary> GetCurrentProductGroupWorkSummary(TrainItem trainItem)
        {
            if (ProductGroupWorkSummary.ContainsKey(trainItem.StoreKey))
            {
                return ProductGroupWorkSummary[trainItem.StoreKey];
            }

            return null;
        }

        public int GetAveragePillarVelocity(PillarItem pillarItem)
        {
            List<int> velocities = new List<int>();
            AsyncObservableCollection<TrainItem> trains = Globals.GlobalItemCache.TrainItems.GetTrains(TrainTimeFrame.PastValidVelocity);
            foreach (TrainItem train in trains)
            {
                DateTime latestDate = new DateTime();
                WorkSummary latestWorkSummary = null;
                Dictionary<string, WorkSummary> work = GetPillarWorkSummary(pillarItem, train);
                if (work != null && work.Count > 0)
                {
                    foreach (KeyValuePair<string, WorkSummary> kvp in work)
                    {
                        WorkSummary ws = kvp.Value;
                        if (ws.CurrentDate > latestDate)
                        {
                            latestDate = ws.CurrentDate;
                            latestWorkSummary = ws;
                        }
                    }

                    int trainVelocity = latestWorkSummary.WorkCompleted / Globals.IdealHoursPerDay;
                    velocities.Add(trainVelocity);
                }
            }

            if (velocities.Count == 0)
            {
                return 0;
            }

            int velocityTotal = 0;
            foreach (int velocity in velocities)
            {
                velocityTotal += velocity;
            }

            return velocityTotal / velocities.Count;
        }
    }


}
