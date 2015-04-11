using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public partial class BacklogItem
    {
        public string ParentSpec
        {
            get
            {
                string spec = null;
                if (IsSpecdInBacklog)
                {
                    spec = Globals.c_SpecdInBacklog;
                }
                else
                {
                    spec = GetStringValue(Datastore.PropNameParentSpec);
                }

                if (!string.IsNullOrWhiteSpace(spec))
                {
                    if (spec.StartsWith("(No Spec"))
                    {
                        spec = Globals.c_SpecTBD;
                    }
                }
                else
                {
                    spec = Globals.c_SpecTBD;
                }

                return spec;
            }

            set
            {
                if (value == Globals.c_SpecdInBacklog)
                {
                    IsSpecdInBacklog = true;
                }
                else if (value == Globals.c_SpecTBD)
                {
                    IsSpecdInBacklog = false;
                    if (IsSpecSet)
                    {
                        SetStringValue(Datastore.PropNameParentSpec, null);
                    }
                }
                else
                {
                    SetStringValue(Datastore.PropNameParentSpec, value);
                }

                NotifyPropertyChangedByName();
                NotifySpecStatusChanged();
                NotifyPropertyChanged(() => SpecStatusValues);
                NotifyPropertyChanged(() => StoreSpecStatusText);
                NotifyPropertyChanged(() => SpecStatus);
                
                if (ParentScenarioItem != null)
                {
                    ParentScenarioItem.NotifySpecStatusChanged();
                }
            }
        }

        void ProductGroupManager_DefaultSpecTeamNameChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged(() => SpecTeamList);
            NotifyPropertyChanged(() => SpecTeam);
        }

        // This is used only to store the original StoreSpecStatusText value, in
        // case the value needs to be reset.
        public string OriginalStoreSpecStatusText { get; set; }

        public string StoreSpecStatusText
        {
            get
            {
                string specStatus = GetStringValue(Datastore.PropNameParentSpecStatus);
                if (string.IsNullOrWhiteSpace(specStatus))
                {
                    return Globals.c_NotSet;
                }
                else
                {
                    return specStatus;
                }
            }

            set
            {
                if (value != null)
                {
                    StoreSpecStatusValue specStatusValue = StoreSpecStatus.GetStoreSpecStatus(value);
                    if (specStatusValue == StoreSpecStatusValue.ReadyForCoding || specStatusValue == StoreSpecStatusValue.SpecFinalized)
                    {
                        if (!IsSpecSet)
                        {
                            //UserMessage.Show("The Spec Status of a Backlog Item cannot be set to 'Ready for Coding' or 'Spec Finalized' unless the Spec is specified as either a valid spec for your product group, set to 'No Spec Required', or set to 'Spec'd in Backlog'.");
                            //NotifyPropertyChanged();
                            //return;
                        }
                    }

                    if (value == Globals.c_noSpecRequired)
                    {
                        value = Globals.c_SpecTBD;
                    }

                    if (value == Globals.c_NotSet)
                    {
                        value = null;
                    }

                    SetStringValue(Datastore.PropNameParentSpecStatus, value);

                    NotifySpecStatusChanged();
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the value that selects for which team you want to picks specs from.
        /// This value determines what set of values is returned from TeamSpecValues.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string SpecTeam
        {
            get
            {
                string specTeam = GetStringValue(Datastore.PropNameBacklogSpecTeam);
                if (string.IsNullOrWhiteSpace(specTeam))
                {
                    return ProductGroupManager.Instance.DefaultSpecTeamName;
                }

                return specTeam;
            }

            set
            {
                SetStringValue(Datastore.PropNameBacklogSpecTeam, value);
                NotifyPropertyChanged(() => TeamSpecValues);
                NotifyPropertyChanged(() => ParentSpec);
            }
        }

        public static AsyncObservableCollection<AllowedValue> m_SpecTeamListWithNone;
        public static AsyncObservableCollection<AllowedValue> m_SpecTeamListWithoutNone;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the list of team names that allow selection of which team to return a
        /// collection of specs for.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<AllowedValue> SpecTeamList
        {
            get
            {
                if (ProductGroupManager.Instance.DefaultSpecTeamName == Globals.c_NoneSpecTeamName)
                {
                    if (m_SpecTeamListWithNone == null)
                    {
                        BuildSpecTeamLists();
                    }

                    return m_SpecTeamListWithNone;
                }
                else
                {

                }

                if (m_SpecTeamListWithoutNone == null)
                {
                    BuildSpecTeamLists();
                }

                return m_SpecTeamListWithoutNone;
            }
        }

        private void BuildSpecTeamLists()
        {
            m_SpecTeamListWithNone = HostItemStore.Instance.GetFieldAllowedValues(this, Datastore.PropNameBacklogSpecTeam);
            m_SpecTeamListWithoutNone = m_SpecTeamListWithNone.ToCollection();
            foreach (AllowedValue value in m_SpecTeamListWithoutNone)
            {
                if ((string)value.Value == Globals.c_NoneSpecTeamName)
                {
                    m_SpecTeamListWithoutNone.Remove(value);
                    break;
                }
            }

        }

        private static Dictionary<string, AsyncObservableCollection<AllowedValue>> m_specsByTeam;

        public AsyncObservableCollection<AllowedValue> TeamSpecValues
        {
            get
            {
                if (m_specsByTeam == null)
                {
                    m_specsByTeam = new Dictionary<string, AsyncObservableCollection<AllowedValue>>();
                }

                string team = SpecTeam;

                if (!m_specsByTeam.ContainsKey(team))
                {
                    AsyncObservableCollection<AllowedValue> specs;
                    if (team != Globals.c_None)
                    {
                        specs = HostItemStore.Instance.GetDependentFieldAllowedValues(Datastore.PropNameParentSpec, Datastore.PropNameBacklogSpecTeam, team);
                    }
                    else
                    {
                        specs = new AsyncObservableCollection<AllowedValue>();
                    }

                    specs.Insert(0, new AllowedValue(Globals.c_SpecdInBacklog));
                    specs.Insert(1, new AllowedValue(Globals.c_SpecTBD));

                    foreach (AllowedValue specValue in specs)
                    {
                        string spec = specValue.Value as string;
                        if (!string.IsNullOrWhiteSpace(spec))
                        {
                            if (spec.StartsWith("(No Spec"))
                            {
                                specs.Remove(specValue);
                                break;
                            }
                        }
                    }

                    m_specsByTeam.Add(team, specs);

                }

                return m_specsByTeam[team];
            }
        }

        public static void RefreshCommonValues()
        {
            m_specStatusValues = null;
            m_postMortemStatusValues = null;
        }

        static AsyncObservableCollection<AllowedValue> m_specStatusValues;
        public AsyncObservableCollection<AllowedValue> SpecStatusValues
        {
            get
            {
                if (IsSpecTBD)
                {
                    AsyncObservableCollection<AllowedValue> specStatusValues = new AsyncObservableCollection<AllowedValue>();
                    specStatusValues.Add(new AllowedValue(Globals.c_NotSet));
                    return specStatusValues;
                }

                else if (m_specStatusValues == null)
                {
                    //try
                    {
                        m_specStatusValues = new AsyncObservableCollection<AllowedValue>();
                        m_specStatusValues = HostItemStore.Instance.GetFieldAllowedValues(this, Datastore.PropNameParentSpecStatus);
                        m_specStatusValues.Insert(0, new AllowedValue(Globals.c_NotSet));
                        foreach (AllowedValue statusValue in m_specStatusValues)
                        {
                            if (statusValue.Value.ToString().Contains(StoreSpecStatusTextValues.SpecNotNeeded))
                            {
                                m_specStatusValues.Remove(statusValue);
                                break;
                            }
                        }
                    }

                    //catch (Exception exception)
                    //{
                    //    Globals.ApplicationManager.HandleException(exception);
                    //    m_specStatusValues = new AsyncObservableCollection<AllowedValue>();
                    //}
                }

                return m_specStatusValues;
            }
        }

        public override void UpdateSpecStatus(bool updateParent)
        {
            if (IsPostMortemIssue)
            {
                if (Utils.StringsMatch(PostMortemStatus, "PIRComplete"))
                {
                    if (IsPostMortemSpecAttached)
                    {
                        SetSpecStatus(PlannerNameSpace.SpecStatus.All_Specs_Current, updateParent);
                        return;
                    }
                    else
                    {
                        SetSpecStatus(PlannerNameSpace.SpecStatus.Specs_Not_Current, updateParent);
                        return;
                    }
                }
                else
                {
                    SetSpecStatus(PlannerNameSpace.SpecStatus.No_Specs_Required, updateParent);
                    return;
                }
            }

            StoreSpecStatusValue storeSpecStatus = StoreSpecStatus.GetStoreSpecStatus(StoreSpecStatusText);

            TrainItem parentTrainItem = ParentTrainItem;
            // If the finish timeframe is in the past, no further spec work is required.
            if (parentTrainItem != null)
            {
                if (parentTrainItem.TimeFrame == TrainTimeFrame.Past)
                {
                    SetSpecStatus(PlannerNameSpace.SpecStatus.No_Specs_Required, updateParent);
                    return;
                }

                if (storeSpecStatus == StoreSpecStatusValue.NoSpecRequired)
                {
                    SetSpecStatus(PlannerNameSpace.SpecStatus.No_Specs_Required, updateParent);
                    return;
                }

                if (storeSpecStatus == StoreSpecStatusValue.NotSpecified)
                {
                    SetSpecStatus(PlannerNameSpace.SpecStatus.Spec_Status_Not_Set, updateParent);
                    return;
                }
            }

            if (parentTrainItem == null)
            {
                if (storeSpecStatus == StoreSpecStatusValue.ReadyForCoding || storeSpecStatus == StoreSpecStatusValue.SpecFinalized)
                {
                    SetSpecStatus(PlannerNameSpace.SpecStatus.All_Specs_Current, updateParent);
                    return;
                }
                else
                {
                    SetSpecStatus(PlannerNameSpace.SpecStatus.Specs_Not_Current, updateParent);
                    return;
                }
            }

            // If the finish timeframe is the current train, all spec work must be completed.
            else if (parentTrainItem.TimeFrame == TrainTimeFrame.Current)
            {
                if (storeSpecStatus == StoreSpecStatusValue.ReadyForCoding || storeSpecStatus == StoreSpecStatusValue.SpecFinalized)
                {
                    SetSpecStatus(PlannerNameSpace.SpecStatus.All_Specs_Current, updateParent);
                    return;
                }
                else
                {
                    SetSpecStatus(PlannerNameSpace.SpecStatus.Specs_Not_Current, updateParent);
                    return;
                }
            }
            else if (TrainManager.Instance.IsNextTrain(parentTrainItem))
            {
                if (storeSpecStatus == StoreSpecStatusValue.ReadyForCoding || storeSpecStatus == StoreSpecStatusValue.SpecFinalized)
                {
                    SetSpecStatus(PlannerNameSpace.SpecStatus.All_Specs_Current, updateParent);
                    return;
                }
                else
                {
                    SetSpecStatus(PlannerNameSpace.SpecStatus.Specs_Not_Current, updateParent);
                    return;
                }
            }
            else
            {
                SetSpecStatus(PlannerNameSpace.SpecStatus.All_Specs_Current, updateParent);
            }
        }

        public static bool IsSpecValid(string spec)
        {
            if (string.IsNullOrWhiteSpace(spec) || spec.StartsWith("(No Spec"))
            {
                return false;
            }

            return true;
        }

        public bool IsSpecSet
        {
            get
            {
                string parentSpec = ParentSpec;
                return IsSpecValid(parentSpec);
            }
        }

        public bool IsSpecReadyForCoding
        {
            get
            {
                if (IsSpecSet)
                {
                    StoreSpecStatusValue specStatusValue = StoreSpecStatus.GetStoreSpecStatus(StoreSpecStatusText);
                    if (specStatusValue == StoreSpecStatusValue.ReadyForCoding || specStatusValue == StoreSpecStatusValue.SpecFinalized)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        static AsyncObservableCollection<AllowedValue> m_postMortemStatusValues;
        public AsyncObservableCollection<AllowedValue> PostMortemStatusValues
        {
            get
            {
                if (m_postMortemStatusValues == null)
                {
                    m_postMortemStatusValues = HostItemStore.Instance.GetFieldAllowedValues(this, Datastore.PropNamePostMortemStatus);
                    m_postMortemStatusValues.Insert(0, new AllowedValue(Globals.c_NotSet));
                }

                return m_postMortemStatusValues;
            }
        }

        public bool IsPostMortemSpecAttached
        {
            get
            {
                try
                {
                    if (AttachedFilesCount == 0)
                    {
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    Globals.ApplicationManager.HandleException(exception);
                    return false;
                }

                return GetBoolValue(Datastore.PropNameIsPostMortemSpecAttached);
            }

            set { SetBoolValue(Datastore.PropNameIsPostMortemSpecAttached, value); }
        }

        public string PostMortemStatus
        {
            get { return GetNotSetStringValue(Datastore.PropNamePostMortemStatus); }
            set
            {
                SetNotSetStringValue(Datastore.PropNamePostMortemStatus, value);
                NotifyPropertyChanged(() => IsStandardIssue);
                NotifyPropertyChanged(() => IsPostMortemIssueVisibility);
                NotifyPropertyChanged(() => IsPostMortemIssue);
            }
        }

        public string SpecStatusComments
        {
            get { return GetStringValue(Datastore.PropNameParentSpecStatusComments); }
            set { SetStringValue(Datastore.PropNameParentSpecStatusComments, value); }
        }

        public string SpecLink
        {
            get { return GetStringValue(Datastore.PropNameParentSpecLink); }
            set { SetStringValue(Datastore.PropNameParentSpecLink, value); }
        }

        public bool IsSpecTBD
        {
            get
            {
                return Utils.StringsMatch(ParentSpec, Globals.c_SpecTBD);
            }
        }

        public bool IsSpecdInBacklog
        {
            get { return GetBoolValue(Datastore.PropNameSpecdInBacklog); }
            set { SetBoolValue(Datastore.PropNameSpecdInBacklog, value); }
        }


    }
}
