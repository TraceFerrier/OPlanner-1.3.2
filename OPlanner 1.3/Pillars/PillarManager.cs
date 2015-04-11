using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public enum PillarValidationType
    {
        Title,
        PillarPathID,
    }

    public sealed class PillarManager
    {
        private static readonly PillarManager m_instance = new PillarManager();

        private PillarManager() 
        {
            m_pillarsWithNone = null;
            m_pillarsWithAllNone = null;
            RefreshBasedOnPillarChangesPending = false;
            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;
        }

        public static PillarManager Instance
        {
            get { return m_instance; }
        }

        static AsyncObservableCollection<PillarItem> m_sortedPillars;
        static AsyncObservableCollection<PillarItem> m_pillarsWithNone;
        static AsyncObservableCollection<PillarItem> m_pillarsWithAllNone;

        // Triggering these static events will magically update the binding for
        // any controls bound to the respective collections.
        public static event EventHandler PillarItemsChanged;
        public static event EventHandler PillarsWithNoneChanged;
        public static event EventHandler PillarsWithAllNoneChanged;

        public bool RefreshBasedOnPillarChangesPending { get; set; }

        void Handle_ApplicationStartupComplete(object sender, EventArgs e)
        {
            Globals.ItemManager.StoreItemChanged += Handle_StoreItemChanged;
        }

        public static AsyncObservableCollection<PillarItem> PillarItems
        {
            get
            {
                if (m_sortedPillars == null)
                {
                    m_sortedPillars = PillarItem.Items.ToCollection();
                    m_sortedPillars.Sort((x, y) => x.Title.CompareTo(y.Title));
                }

                return m_sortedPillars;
            }
        }

        public static AsyncObservableCollection<PillarItem> PillarsWithNone
        {
            get
            {
                if (m_pillarsWithNone == null)
                {
                    m_pillarsWithNone = Utils.GetItems<PillarItem>(PillarItem.Items, DummyItemType.NoneType, Utils.GetPropertyName((PillarItem p) => p.Title));
                }

                return m_pillarsWithNone;
            }
        }

        public static AsyncObservableCollection<PillarItem> PillarsWithAllNone
        {
            get
            {
                if (m_pillarsWithAllNone == null)
                {
                    m_pillarsWithAllNone = Utils.GetItems<PillarItem>(PillarItem.Items, DummyItemType.AllNoneType, Utils.GetPropertyName((PillarItem p) => p.Title));
                }

                return m_pillarsWithAllNone;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Keeps the pillar specialty cache collections up-to-date, based on changes in the 
        /// primary pillar cache.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Handle_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            StoreItemChange change = e.Change;
            if (change.Item.StoreItemType == ItemTypeID.Pillar)
            {
                PillarItem pillarItem = (PillarItem)change.Item;
                switch (change.ChangeType)
                {
                    case ChangeType.Added:
                        RefreshBasedOnPillarChangesPending = true;
                        UpdatePillarCollections();
                        break;
                    case ChangeType.Removed:
                        RefreshBasedOnPillarChangesPending = true;
                        UpdatePillarCollections();
                        break;
                    case ChangeType.Updated:
                        if (change.PublicPropName.IsStoreProperty(Datastore.PropNameTitle))
                        {
                            UpdatePillarCollections();
                        }
                        break;
                }
            }
        }

        void UpdateDependentItems(StoreItemChange change)
        {
            AsyncObservableCollection<StoreItem> allItems = Globals.ItemManager.GetItems();
            foreach (StoreItem item in allItems)
            {
                IDependsOnPillarItem dependsItem = item as IDependsOnPillarItem;
                if (dependsItem != null)
                {
                    dependsItem.NotifyPillarChanged(change);
                }
            }
        }

        void UpdatePillarCollections()
        {
            m_sortedPillars = null;
            m_pillarsWithNone = null;
            m_pillarsWithAllNone = null;

            // Fire UI binding events
            if (PillarItemsChanged != null)
            {
                PillarItemsChanged(null, EventArgs.Empty);
            }

            if (PillarsWithNoneChanged != null)
            {
                PillarsWithNoneChanged(null, EventArgs.Empty);
            }

            if (PillarsWithAllNoneChanged != null)
            {
                PillarsWithAllNoneChanged(null, EventArgs.Empty);
            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Since adding or deleting pillars can (and probably will) change the set of 
        /// StoreItems that are valid for the current product group, kick off a refresh to
        /// re-sync the StoreItem set.
        /// </summary>
        //------------------------------------------------------------------------------------
        void RefreshBasedOnPillarChanges()
        {

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the property associated with the given validation type is set to
        /// a valid value in the given pillar (i.e. the title isn't in use by any other
        /// pillar, etc).
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsPillarValid(PillarItem pillarToValidate, PillarValidationType validationType)
        {
            foreach (PillarItem pillarItem in PillarItem.Items)
            {
                if (pillarItem != pillarToValidate)
                {
                    switch (validationType)
                    {
                        case PillarValidationType.PillarPathID:
                            if (pillarItem.PillarPathID == pillarToValidate.PillarPathID)
                            {
                                return false;
                            }
                            break;

                        case PillarValidationType.Title:
                            if (Utils.StringsMatch(pillarItem.Title, pillarToValidate.Title))
                            {
                                return false;
                            }
                            break;
                    }
                }
            }

            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Pulls up a dialog that allows the user to edit or create a pillar item (including
        /// setting the back-end store path for the item).
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ShowPillarEditItemEditor(PillarItem pillarItem)
        {
            // If the product tree isn't already available, we'll wait for it.
            if (!Globals.ProductTreeManager.IsTreeItemCollectionAvailable)
            {
                Globals.ProductTreeManager.EnsureHostProductTree();

                BackgroundTask waitForProductTreeTask = new BackgroundTask(true);
                waitForProductTreeTask.DoWork += waitForProductTreeTask_DoWork;
                waitForProductTreeTask.TaskCompleted += waitForProductTreeTask_TaskCompleted;
                waitForProductTreeTask.IsProgressDialogIndeterminate = true;
                waitForProductTreeTask.TaskArgs = pillarItem;
                waitForProductTreeTask.RunTaskAsync();
            }
            else
            {
                ShowPillarDialog(pillarItem);
            }
        }

        void waitForProductTreeTask_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundTask task = e.Argument as BackgroundTask;
            task.ReportProgress(0, "Building the Pillar Path Tree View (this may take a minute or so the first time you edit a pillar)...", "");
            do
            {
                if (task.CancellationPending)
                {
                    e.Cancel = true;
                    e.Result = new BackgroundTaskResult { ResultType = ResultType.Cancelled };
                    return;
                }

            } while (!Globals.ProductTreeManager.IsTreeItemCollectionAvailable);
        }

        void waitForProductTreeTask_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            if (result != null && result.ResultType == ResultType.Cancelled)
            {
                return;
            }

            ShowPillarDialog((PillarItem)TaskArgs);
        }

        void ShowPillarDialog(PillarItem pillarItem)
        {
            PillarEditorDialog dialog = new PillarEditorDialog(pillarItem);
            dialog.ShowDialog();
        }

    }
}
