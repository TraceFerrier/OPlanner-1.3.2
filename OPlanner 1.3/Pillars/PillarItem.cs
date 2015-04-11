using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;

namespace PlannerNameSpace
{
    public partial class PillarItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.Pillar; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        List<int> PillarNodes;
        public static PillarItem CreatePillarItem(string title = "New Pillar")
        {
            PillarItem newPillar = ScheduleStore.Instance.CreateStoreItem<PillarItem>(ItemTypeID.Pillar);
            newPillar.Title = title;
            return newPillar;
        }

        public bool IsTreeIDUnderPillar(int treeID)
        {
            if (PillarNodes == null)
            {
                PillarNodes = HostItemStore.Instance.GetTreeIDChildNodes(PillarPathID);
            }

            return PillarNodes.Contains(treeID);
        }

        public int AverageTrainVelocity
        {
            get
            {
                return 0;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a ContextMenu suitable for the options available for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void PopulateContextMenu(Window ownerWindow, ContextMenu menu)
        {
            AddContextMenuItem(menu, "Edit Pillar...", "Edit.png", Edit_Click);
            AddContextMenuItem(menu, "Delete Pillar...", "Delete.png", Delete_Click);
        }

        void Edit_Click(object sender, RoutedEventArgs e)
        {
            PillarManager.Instance.ShowPillarEditItemEditor(this);
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            RequestDeleteItem();
        }

        public string PillarPath
        {
            get
            {
                return HostItemStore.Instance.GetTreePath(PillarPathID, ProductTreeFormat.ExcludeProduct);
            }
        }

        public int PillarPathID
        {
            get 
            {
                return GetIntValue(Datastore.PropNamePillarTreeID1); 
            }
            set
            {
                int lastID = GetIntValue(Datastore.PropNamePillarTreeID1);
                if (value != lastID)
                {
                    PillarNodes = null;
                }

                SetIntValue(Datastore.PropNamePillarTreeID1, value);
            }
        }

        public string PMOwner
        {
            get { return GetNoneStringValue(Datastore.PropNamePillarPMOwner); }
            set { SetNoneStringValue(Datastore.PropNamePillarPMOwner, value); }
        }

        public string TestOwner
        {
            get { return GetNoneStringValue(Datastore.PropNamePillarTestOwner); }
            set { SetNoneStringValue(Datastore.PropNamePillarTestOwner, value); }
        }

        public string DevOwner
        {
            get { return GetNoneStringValue(Datastore.PropNamePillarDevOwner); }
            set { SetNoneStringValue(Datastore.PropNamePillarDevOwner, value); }
        }

    }
}
