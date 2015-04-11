using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public sealed class ProductGroupManager
    {
        private static readonly ProductGroupManager m_instance = new ProductGroupManager();

        public static ProductGroupManager Instance
        {
            get { return m_instance; }
        }

        public string CurrentProductGroupKey { get; set; }

        private ProductGroupItem m_currentProductGroup;
        public ProductGroupItem CurrentProductGroup
        {
            get
            {
                if (m_currentProductGroup == null && CurrentProductGroupKey != null)
                {
                    m_currentProductGroup = Globals.ItemManager.GetItem<ProductGroupItem>(CurrentProductGroupKey);
                }

                return m_currentProductGroup;
            }

            set
            {
                m_currentProductGroup = value;
            }
        }

        public string CurrentProductGroupName { get { return CurrentProductGroup != null ? CurrentProductGroup.Title : null; } }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Event fired if the default spec team name changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public event EventHandler DefaultSpecTeamNameChanged;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Event fired when the request begun by a call to BeginOpenProductGroup completes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public event EventHandler<ProductGroupOpenedEvent> OpenProductGroupComplete;

        public ProductGroupManager()
        {
            CurrentProductGroupKey = null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Begins the process of discovering and opening the last-opened product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void BeginOpenProductGroup()
        {
            BackgroundTask openProductGroupTask = new BackgroundTask(ProgressDialogOption.StandardProgressNoClose);
            openProductGroupTask.DoWork += openProductGroupTask_DoWork;
            openProductGroupTask.TaskCompleted += openProductGroupTask_TaskCompleted;
            openProductGroupTask.IsProgressDialogIndeterminate = true;
            openProductGroupTask.RunTaskAsync();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Background worker for BeginOpenProductGroup - this routine attempts to read the
        /// item from the backing store that represents the last-opened product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        void openProductGroupTask_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundTask taskWorker = e.Argument as BackgroundTask;
            taskWorker.ReportProgress(0, "", "Starting up...");

            //try
            {
                CurrentProductGroupKey = Globals.UserPreferences.GetGlobalPreference<string>(GlobalPreference.LastOpenProductGroupKey);
                CurrentProductGroup = Datastore.GetStoreItem(CurrentProductGroupKey) as ProductGroupItem;
                if (CurrentProductGroup == null || !CurrentProductGroup.IsActive)
                {
                    CurrentProductGroupKey = null;
                }

                e.Result = new BackgroundTaskResult { ResultType = ResultType.Completed };

            }

            //catch (Exception exception)
            //{
            //    e.Result = new BackgroundTaskResult { ResultType = ResultType.Failed, ResultMessage = exception.Message };
            //}
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when the task kicked off by BeginOpenProductGroup completes.
        /// </summary>
        //------------------------------------------------------------------------------------
        void openProductGroupTask_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            if (OpenProductGroupComplete != null)
            {
                OpenProductGroupComplete(this, new ProductGroupOpenedEvent(CurrentProductGroupKey, CurrentProductGroup, result));
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of all the defined Product Studio paths for all pillars associated
        /// with the currently open ProductGroup.
        /// </summary>
        //------------------------------------------------------------------------------------
        public List<int> GetAllPillarPathIDs()
        {
            List<int> pillarPathIDs = new List<int>();
            AsyncObservableCollection<PillarItem> pillars = PillarManager.PillarItems;
            foreach (PillarItem pillar in pillars)
            {
                pillarPathIDs.Add(pillar.PillarPathID);
            }

            return pillarPathIDs;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// This is the default 'Team' name to set in order to retrieve the subset of specs 
        /// from the backing store owned by this product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string DefaultSpecTeamName
        {
            get
            {
                if (CurrentProductGroup != null)
                {
                    return CurrentProductGroup.DefaultSpecTeamName;
                }

                return Globals.c_NoneSpecTeamName;
            }

            set
            {
                if (CurrentProductGroup != null)
                {
                    CurrentProductGroup.DefaultSpecTeamName = value;
                    if (DefaultSpecTeamNameChanged != null)
                    {
                        DefaultSpecTeamNameChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of the aliases for all the members of the current product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        public List<string> GetMemberAliases()
        {
            if (CurrentProductGroup != null)
            {
                return CurrentProductGroup.MemberAliases;
            }

            return new List<string>();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given the ID of a node in the host store feature tree, return the PillarItem (if
        /// any) that owns that node (via of it's declared paths).  If no owner is found, null
        /// will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public PillarItem FindOwnerPillar(int treeID)
        {
            AsyncObservableCollection<PillarItem> pillars = PillarManager.PillarItems;
            foreach (PillarItem pillar in pillars)
            {
                if (pillar.IsTreeIDUnderPillar(treeID))
                {
                    return pillar;
                }
            }

            return null;
        }

        public ProductGroupItem GetProductGroupByTitle(string title)
        {
            AsyncObservableCollection<ProductGroupItem> groups = Globals.ItemManager.ProductGroupItems;
            foreach (ProductGroupItem group in groups)
            {
                if (Utils.StringsMatch(group.Title, title))
                {
                    return group;
                }
            }

            return null;
        }

    }
}
