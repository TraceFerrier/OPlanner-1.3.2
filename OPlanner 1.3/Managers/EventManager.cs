using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Runtime.CompilerServices;

namespace PlannerNameSpace
{
    public delegate void RibbonButtonEventHandler();
    public delegate void StoreEventHandler();
    public delegate void UpdateProductGroupMembersEventHandler();
    public delegate void StoreItemEventHandler(StoreItem item);
    public delegate void PropertyEventHandler(object source, string propName);
    public delegate void GeneralUpdateHandler();

    public sealed class EventManager : DispatcherObject
    {
        // Events that any listener can suscribe to
        public event EventHandler DiscoveryComplete;

        public event GeneralUpdateHandler UpdateUI;
        public event GeneralUpdateHandler CommitmentStatusComputationComplete;
        public event GeneralUpdateHandler ForecastingComputationComplete;
        public event StoreItemEventHandler NewItemCommittedToStore;
        public event ScrumTeamItemEventHandler ScrumTeamViewTeamSelectionChanged;

        public event RibbonButtonEventHandler CreateWorkItem;
        public event RibbonButtonEventHandler DeleteWorkItem;
        public event RibbonButtonEventHandler CreateBacklogItem;

        public event StoreEventHandler PlannerRefreshStarting;

        public event UpdateProductGroupMembersEventHandler ProductGroupMembersUpdateComplete;
        public event PropertyEventHandler PropertyChangedCanceled;
        public event GeneralUpdateHandler GotoItemCommand;
        public event GeneralUpdateHandler TabLoadStarting;

        public event EventHandler ScrumTeamCollectionChanged;
        public event EventHandler<StoreCommitCompleteEventArgs> StoreCommitComplete;

        public void OnDiscoveryComplete(object sender)
        {
            if (DiscoveryComplete != null)
            {
                DiscoveryComplete(sender, EventArgs.Empty);
            }
        }

        public void OnScrumTeamCollectionChanged()
        {
            if (ScrumTeamCollectionChanged != null)
            {
                ScrumTeamCollectionChanged(this, EventArgs.Empty);
            }
        }

        public void OnTabLoadStarting()
        {
            if (TabLoadStarting != null)
            {
                TabLoadStarting();
            }
        }

        // The Store Commit operation has completed
        public void OnStoreCommitComplete(object sender, StoreCommitCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => DispatchOnStoreCommitComplete(sender, e)));
        }

        private void DispatchOnStoreCommitComplete(object sender, StoreCommitCompleteEventArgs e)
        {
            if (StoreCommitComplete != null)
            {
                StoreCommitComplete(sender, e);
            }
        }

        // A refresh of all planner data is starting
        public void OnPlannerRefreshStarting()
        {
            if (PlannerRefreshStarting != null)
            {
                PlannerRefreshStarting();
            }
        }

        // An async update of the current product group's members has been completed
        public void OnProductGroupMembersUpdateCompleted()
        {
            if (ProductGroupMembersUpdateComplete != null)
            {
                ProductGroupMembersUpdateComplete();
            }
        }

        // Create new work item
        public void OnCreateNewWorkItem()
        {
            if (CreateWorkItem != null)
            {
                CreateWorkItem();
            }
        }

        public void OnDeleteWorkItem()
        {
            if (DeleteWorkItem != null)
            {
                DeleteWorkItem();
            }
        }

        public void OnCreateBacklogItem()
        {
            if (CreateBacklogItem != null)
            {
                CreateBacklogItem();
            }
        }

        public void OnUpdateUI()
        {
            if (UpdateUI != null)
            {
                UpdateUI();
            }
        }

        public void OnForecastingComputationComplete()
        {
            if (ForecastingComputationComplete != null)
            {
                ForecastingComputationComplete();
            }
        }

        public void OnCommitmentStatusComputationComplete()
        {
            if (CommitmentStatusComputationComplete != null)
            {
                CommitmentStatusComputationComplete();
            }
        }

        public void OnNewItemCommittedToStore(StoreItem item)
        {
            if (NewItemCommittedToStore != null)
            {
                NewItemCommittedToStore(item);
            }
        }

        public void OnScrumTeamViewTeamSelectionChanged(object sender, ScrumTeamItem currentItem)
        {
            if (ScrumTeamViewTeamSelectionChanged != null)
            {
                ScrumTeamViewTeamSelectionChanged(sender, new ScrumTeamChangedEventArgs(currentItem));
            }
        }

        public void OnPropertyChangedCanceled(object source, [CallerMemberName] String publicPropName = "")
        {
            if (PropertyChangedCanceled != null)
            {
                PropertyChangedCanceled(source, publicPropName);
            }
        }

        public void OnGotoItemCommand()
        {
            if (GotoItemCommand != null)
            {
                GotoItemCommand();
            }
        }

    }
}
