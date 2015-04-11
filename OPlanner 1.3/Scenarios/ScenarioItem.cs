using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public class GradeValues
    {
        public const string Green = "Green";
        public const string Yellow = "Yellow";
        public const string Red = "Red";
    }

    [Serializable]
    public class ScenarioStatusFields
    {
        public string TestGrade { get; set; }
        public string DesignerGrade { get; set; }
        public string ResearchGrade { get; set; }

        public string TestComments { get; set; }
        public string DesignerComments { get; set; }
        public string ResearchComments { get; set; }

        public string TestOwnerAlias { get; set; }
        public string DesignerAlias { get; set; }
        public string ResearcherAlias { get; set; }

        public int TestHoursAllocated { get; set; }
        public int DesignerHoursAllocated { get; set; }
        public int ResearchHoursAllocated { get; set; }
        public int BusinessRank { get; set; }
    }

    public partial class ScenarioItem : ForecastableItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.Scenario; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        private ScenarioStatusFields m_scenarioStatus;

        public static ScenarioItem GetDummyAllItem()
        {
            return StoreItem.GetDummyItem<ScenarioItem>(DummyItemType.AllType);
        }

        ScenarioStatusFields ScenarioStatus
        {
            get
            {
                if (m_scenarioStatus == null)
                {
                    string statusText = GetStringValue(Datastore.PropNameOldScenarioStatus);
                    if (!string.IsNullOrWhiteSpace(statusText))
                    {
                        if (statusText.Contains("<ScenarioStatusFields"))
                        {
                            StringReader stringReader = new StringReader(statusText);

                            try
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(ScenarioStatusFields));
                                m_scenarioStatus = (ScenarioStatusFields)serializer.Deserialize(stringReader);
                            }

                            catch
                            {
                                m_scenarioStatus = null;
                            }
                        }
                    }

                    if (m_scenarioStatus == null)
                    {
                        m_scenarioStatus = new ScenarioStatusFields();
                    }
                }

                return m_scenarioStatus;
            }
        }

        void RefreshStatusProperties()
        {
                m_scenarioStatus = null;
                PropertyInfo[] props = typeof(ScenarioStatusFields).GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    NotifyPropertyChangedByName(prop.Name);
                }

                NotifyPropertyChanged(() => TestGradeColor);
                NotifyPropertyChanged(() => TestBackgroundColor);
                NotifyPropertyChanged(() => ResearchGradeColor);
                NotifyPropertyChanged(() => ResearchBackgroundColor);
                NotifyPropertyChanged(() => DesignerGradeColor);
                NotifyPropertyChanged(() => DesignerBackgroundColor);
        }

        void WriteScenarioStatus([CallerMemberName] String publicPropName = "")
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ScenarioStatusFields));
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, m_scenarioStatus);
            string statusText = stringWriter.ToString();
            SetStringValue(Datastore.PropNameOldScenarioStatus, statusText, "ScenarioStatus");
            NotifyPropertyChangedByName(publicPropName);
        }

        public static ScenarioItem CreateScenarioItem(PillarItem parentPillarItem, ExperienceItem parentExperienceItem, QuarterItem parentQuarterItem)
        {
            ScenarioItem newItem = ScheduleStore.Instance.CreateStoreItem<ScenarioItem>(ItemTypeID.Scenario);
            newItem.BeginSaveImmediate();
            newItem.Title = "New Scenario";
            newItem.ParentPillarItem = parentPillarItem;
            newItem.ParentExperienceItem = parentExperienceItem;
            newItem.ParentQuarterItem = parentQuarterItem;
            newItem.SaveNewItem();
            newItem.SaveImmediate();

            return newItem;
        }

        public string TargetShipQuarter
        {
            get
            {
                if (ParentQuarterItem == null)
                {
                    return Globals.c_None;
                }

                return ParentQuarterItem.Title;
            }
        }

        public int BacklogCount
        {
            get
            {
                return BacklogItems.Count;
            }
        }

        public string QualifiedTitle
        {
            get
            {
                string title = null;
                if (ParentPillarItem != null && !ParentPillarItem.IsDummyItem)
                {
                    title = ParentPillarItem.Title + ": ";
                    title += Title;
                }
                else
                {
                    title = Title;
                }

                return title;
            }
        }

        public AsyncObservableCollection<ExperienceItem> AvailableExperiences
        {
            get { return Globals.ItemManager.ExperienceItems.GetItems(DummyItemType.NoneType); }
        }

        public override AsyncObservableCollection<ForecastableItem> ForecastableChildren
        {
            get
            {
                AsyncObservableCollection<ForecastableItem> items = new AsyncObservableCollection<ForecastableItem>();
                foreach (BacklogItem item in BacklogItems)
                {
                    items.Add(item);
                }

                return items;
            }
        }

        public override PillarItem GetParentPillarItem()
        {
            return ParentPillarItem;
        }

        public override QuarterItem GetParentQuarterItem()
        {
            return ParentQuarterItem;
        }

        public string ParentPillarTitle { get { return ParentPillarItem == null ? Globals.c_None : ParentPillarItem.Title; } }
        public string ParentQuarterTitle { get { return ParentQuarterItem == null ? Globals.c_None : ParentQuarterItem.Title; } }

        public PillarItem ParentPillarItem
        {
            get
            {
                string key = ParentPillarItemKey;
                PillarItem pillarItem = Globals.ItemManager.GetItem<PillarItem>(key);
                if (pillarItem == null)
                {
                    return StoreItem.GetDummyItem<PillarItem>(DummyItemType.NoneType);
                }

                return pillarItem;
            }

            set
            {
                PillarItem oldParent = ParentPillarItem;

                if (value == null || value.IsDummyItem)
                {
                    ParentPillarItemKey = null;
                }
                else
                {
                    ParentPillarItemKey = value.StoreKey;
                }

            }
        }

        public string ParentPillarItemKey
        {
            get
            {
                return GetStringValue(Datastore.PropNameOldScenarioParentPillarKey);
            }

            set
            {
                SetStringValue(Datastore.PropNameOldScenarioParentPillarKey, value);
                NotifyPropertyChanged(() => ParentPillarItem);
            }
        }

        public string ParentExperienceTitle { get { return ParentExperienceItem == null ? Globals.c_None : ParentExperienceItem.Title; } }

        public override ForecastableItem ParentForecastableItem
        {
            get { return ParentExperienceItem; }
        }


        public ExperienceItem ParentExperienceItem
        {
            get
            {
                string key = ParentExperienceItemKey;
                if (string.IsNullOrWhiteSpace(key))
                {
                    return StoreItem.GetDummyItem<ExperienceItem>(DummyItemType.NoneType);
                }

                return Globals.ItemManager.GetItem<ExperienceItem>(ParentExperienceItemKey);
            }

            set
            {
                ExperienceItem oldParent = ParentExperienceItem;

                if (value == null || value.IsDummyItem)
                {
                    ParentExperienceItemKey = null;
                }
                else
                {
                    ParentExperienceItemKey = value.StoreKey;
                }

                NotifyPropertyChangedByName();

            }
        }

        public string ParentExperienceItemKey
        {
            get
            {
                return GetStringValue(Datastore.PropNameOldParentExperienceItemKey);
            }

            set
            {
                SetStringValue(Datastore.PropNameOldParentExperienceItemKey, value);
                NotifyPropertyChanged(() => ParentExperienceItem);
                NotifyPropertyChanged(() => ParentExperienceTitle);
            }
        }

        public QuarterItem ParentQuarterItem
        {
            get
            {
                string key = ParentQuarterItemKey;
                QuarterItem quarterItem = Globals.ItemManager.GetItem<QuarterItem>(key);
                if (quarterItem == null)
                {
                    return StoreItem.GetDummyItem<QuarterItem>(DummyItemType.NoneType);
                }

                return quarterItem;
            }

            set
            {
                if (value == null || value.IsDummyItem)
                {
                    ParentQuarterItemKey = null;
                }
                else
                {
                    ParentQuarterItemKey = value.StoreKey;
                }

            }
        }

        public string ParentQuarterItemKey
        {
            get
            {
                return GetStringValue(Datastore.PropNameOldScenarioParentQuarterItemKey);
            }

            set
            {
                SetStringValue(Datastore.PropNameOldScenarioParentQuarterItemKey, value);
                NotifyPropertyChanged(() => ParentQuarterItem);
                NotifyPropertyChanged(() => TargetShipQuarter);
            }
        }

        public int TotalWorkRemaining
        {
            get
            {
                int remaining = 0;
                //AsyncObservableCollection<BacklogItem> items = BacklogItems;
                //foreach (BacklogItem backlogItem in items)
                //{
                //    remaining += backlogItem.TotalWorkRemaining;
                //}

                return remaining;
            }
        }

        public string TotalWorkRemainingDisplay
        {
            get { return TotalWorkRemaining.ToString() + " hours"; }
        }

        public AsyncObservableCollection<string> StatusGradeValues
        {
            get
            {
                AsyncObservableCollection<string> values = new AsyncObservableCollection<string>();
                values.Add(GradeValues.Green);
                values.Add(GradeValues.Yellow);
                values.Add(GradeValues.Red);

                return values;
            }
        }

        public void ShowScenarioEditor()
        {
            ScenarioItemEditorDialog dialog = new ScenarioItemEditorDialog(this);
            dialog.ShowDialog();
        }

        public string TestOwnerAlias
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return null;
                }

                return ScenarioStatus.TestOwnerAlias;
            }

            set
            {
                ScenarioStatus.TestOwnerAlias = value;
                WriteScenarioStatus();
            }
        }

        public string TestGrade
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return null;
                }

                return ScenarioStatus.TestGrade;
            }

            set
            {
                ScenarioStatus.TestGrade = value;
                NotifyPropertyChanged(() => TestGradeColor);
                NotifyPropertyChanged(() => TestBackgroundColor);
                WriteScenarioStatus();
            }
        }

        public Brush TestGradeColor
        {
            get
            {
                switch (TestGrade)
                {
                    case GradeValues.Green:
                        return Brushes.Green;
                    case GradeValues.Yellow:
                        return Brushes.Yellow;
                    case GradeValues.Red:
                        return Brushes.Red;
                    default:
                        return Brushes.Yellow;
                }
            }
        }

        public Color TestBackgroundColor
        {
            get
            {
                switch (TestGrade)
                {
                    case GradeValues.Green:
                        return Colors.Green;
                    case GradeValues.Yellow:
                        return Colors.Yellow;
                    case GradeValues.Red:
                        return Colors.Red;
                    default:
                        return Colors.Yellow;
                }
            }
        }

        public string TestComments
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return null;
                }

                return ScenarioStatus.TestComments;
            }

            set
            {
                ScenarioStatus.TestComments = value;
                WriteScenarioStatus();
            }
        }

        public int TestHoursAllocated
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return 0;
                }

                return ScenarioStatus.TestHoursAllocated;
            }

            set
            {
                ScenarioStatus.TestHoursAllocated = value;
                WriteScenarioStatus();
            }
        }

        public string ResearcherAlias
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return null;
                }

                return ScenarioStatus.ResearcherAlias;
            }

            set
            {
                ScenarioStatus.ResearcherAlias = value;
                WriteScenarioStatus();
            }
        }

        public string ResearchGrade
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return null;
                }

                return ScenarioStatus.ResearchGrade;
            }

            set
            {
                ScenarioStatus.ResearchGrade = value;
                WriteScenarioStatus();
                NotifyPropertyChanged(() => ResearchGradeColor);
                NotifyPropertyChanged(() => ResearchBackgroundColor);
            }
        }
        public Brush ResearchGradeColor
        {
            get
            {
                switch (ResearchGrade)
                {
                    case GradeValues.Green:
                        return Brushes.Green;
                    case GradeValues.Yellow:
                        return Brushes.Yellow;
                    case GradeValues.Red:
                        return Brushes.Red;
                    default:
                        return Brushes.Green;
                }
            }
        }

        public Color ResearchBackgroundColor
        {
            get
            {
                switch (ResearchGrade)
                {
                    case GradeValues.Green:
                        return Colors.Green;
                    case GradeValues.Yellow:
                        return Colors.Yellow;
                    case GradeValues.Red:
                        return Colors.Red;
                    default:
                        return Colors.Green;
                }
            }
        }

        public string ResearchComments
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return null;
                }

                return ScenarioStatus.ResearchComments;
            }

            set
            {
                ScenarioStatus.ResearchComments = value;
                WriteScenarioStatus();
            }
        }

        public int ResearchHoursAllocated
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return 0;
                }

                return ScenarioStatus.ResearchHoursAllocated;
            }

            set
            {
                ScenarioStatus.ResearchHoursAllocated = value;
                WriteScenarioStatus();
            }
        }

        public override int BusinessRank
        {
            get { return GetIntValue(Datastore.PropNameOldScenarioBusinessRank); }
            set { SetIntValue(Datastore.PropNameOldScenarioBusinessRank, value); }

        }

        public string DesignerAlias
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return null;
                }

                return ScenarioStatus.DesignerAlias;
            }

            set
            {
                ScenarioStatus.DesignerAlias = value;
                WriteScenarioStatus();
            }
        }
        public string DesignerGrade
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return null;
                }

                return ScenarioStatus.DesignerGrade;
            }

            set
            {
                ScenarioStatus.DesignerGrade = value;
                WriteScenarioStatus();
                NotifyPropertyChanged(() => DesignerGradeColor);
                NotifyPropertyChanged(() => DesignerBackgroundColor);
            }
        }

        public Brush DesignerGradeColor
        {
            get
            {
                switch (DesignerGrade)
                {
                    case GradeValues.Green:
                        return Brushes.Green;
                    case GradeValues.Yellow:
                        return Brushes.Yellow;
                    case GradeValues.Red:
                        return Brushes.Red;
                    default:
                        return Brushes.Green;
                }
            }
        }

        public Color DesignerBackgroundColor
        {
            get
            {
                switch (DesignerGrade)
                {
                    case GradeValues.Green:
                        return Colors.Green;
                    case GradeValues.Yellow:
                        return Colors.Yellow;
                    case GradeValues.Red:
                        return Colors.Red;
                    default:
                        return Colors.Green;
                }
            }
        }

        public string DesignerComments
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return null;
                }

                return ScenarioStatus.DesignerComments;
            }

            set
            {
                ScenarioStatus.DesignerComments = value;
                WriteScenarioStatus();
            }
        }

        public int DesignerHoursAllocated
        {
            get
            {
                if (ScenarioStatus == null)
                {
                    return 0;
                }

                return ScenarioStatus.DesignerHoursAllocated;
            }

            set
            {
                ScenarioStatus.DesignerHoursAllocated = value;
                WriteScenarioStatus();
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
            AddContextMenuItem(menu, "Delete...", "DeleteScenario.png", Delete_Click);
            AddContextMenuItem(menu, "Move Scenario to the Next Quarter...", "MoveToNextTrain.png", MoveToNextQuarter_Click);
        }

        void TreeView_Click(object sender, RoutedEventArgs e)
        {
            ShowTreeView();
        }

        void Edit_Click(object sender, RoutedEventArgs e)
        {
            ShowScenarioEditor();
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            RequestDeleteItem();
        }

        void MoveToNextQuarter_Click(object sender, RoutedEventArgs e)
        {
            QuarterItem quarter = ParentQuarterItem;
            if (!StoreItem.IsRealItem(quarter))
            {
                UserMessage.Show("This Scenario is currently not assigned to a quarter, and therefore can't be moved.");
                return;
            }

            if (quarter.TimeFrame == TrainTimeFrame.Future)
            {
                UserMessage.Show("This Scenario is already assigned to a future quarter (" + quarter.Title + "), and therefore can't be moved.");
                return;
            }

            if (ForecastableChildren.Count == 0)
            {
                UserMessage.Show("This Scenario doesn't have any Backlog Items assigned to it, and therefore can't be moved.");
                return;
            }

            AsyncObservableCollection<BacklogItem> moveableBacklogItems = new AsyncObservableCollection<BacklogItem>();
            foreach (ForecastableItem item in ForecastableChildren)
            {
                BacklogItem backlogItem = item as BacklogItem;
                if (backlogItem != null)
                {
                    if (backlogItem.CanMoveToNextTrain == MoveToNextTrainStatus.Movable)
                    {
                        moveableBacklogItems.Add(backlogItem);
                    }
                }
            }

            if (moveableBacklogItems.Count == 0)
            {
                UserMessage.Show("This Scenario doesn't have any Backlog Items that are eligible to be moved to the next quarter (Backlog Items can't be moved if they have no work remaining to be completed, or are already assigned to a future train).");
                return;
            }

            ScenarioMoveToNextQuarterDialog dialog = new ScenarioMoveToNextQuarterDialog(this, moveableBacklogItems);
            dialog.ShowDialog();
        }
    }
}
