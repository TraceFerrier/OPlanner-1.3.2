using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public class PlannerBugItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.PlannerBug; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        public static PlannerBugItem CreateItem()
        {
            PlannerBugItem newItem = ScheduleStore.Instance.CreateStoreItem<PlannerBugItem>(ItemTypeID.PlannerBug);
            newItem.Title = "";
            return newItem;
        }

        static AsyncObservableCollection<string> m_issueTypes;
        public static AsyncObservableCollection<string> IssueTypes
        {
            get
            {
                if (m_issueTypes == null)
                {
                    m_issueTypes = new AsyncObservableCollection<string>();
                    m_issueTypes.Add("Feature Request");
                    m_issueTypes.Add("Creating a new Product Group");
                    m_issueTypes.Add("Missing or incorrect people for your team");
                    m_issueTypes.Add("Backlog - creating or editing");
                    m_issueTypes.Add("Board View");
                    m_issueTypes.Add("Burndown Charts");
                    m_issueTypes.Add("Capacity Planning");
                    m_issueTypes.Add("Experiences - creating or editing");
                    m_issueTypes.Add("Feature Teams - creating or editing");
                    m_issueTypes.Add("Member View");
                    m_issueTypes.Add("Personas - creating or editing");
                    m_issueTypes.Add("Pillars - creating or editing");
                    m_issueTypes.Add("Refreshing");
                    m_issueTypes.Add("Saving or undoing changes");
                    m_issueTypes.Add("Scenarios - creating or editing");
                    m_issueTypes.Add("Trains - creating or editing");
                    m_issueTypes.Add("Other Issues");
                }

                return m_issueTypes;
            }
        }

        public static AsyncObservableCollection<string> IssueTypesAll
        {
            get
            {
                AsyncObservableCollection<string> issueTypes = IssueTypes;
                issueTypes.Insert(0, Globals.c_All);

                return issueTypes;
            }
        }

        public string BugAssignedTo
        {
            get { return GetStringValue(Datastore.PropNameBugAssignedTo); }
            set { SetStringValue(Datastore.PropNameBugAssignedTo, value); }
        }

        public string BugIssueType
        {
            get { return GetStringValue(Datastore.PropNameBugIssueType); }
            set { SetStringValue(Datastore.PropNameBugIssueType, value); }
        }

        public string BugComments
        {
            get { return GetStringValue(Datastore.PropNameBugComments); }
            set { SetStringValue(Datastore.PropNameBugComments, value); }
        }

        public string BugReproSteps
        {
            get { return GetStringValue(Datastore.PropNameBugReproSteps); }
            set { SetStringValue(Datastore.PropNameBugReproSteps, value); }
        }

        public override string GetItemIssueType()
        {
            return BugIssueType;
        }

        public Brush BugStatusColor
        {
            get
            {
                switch (Status)
                {
                    case StatusValues.Active:
                        return Brushes.LightPink;
                    case StatusValues.Resolved:
                        return Brushes.LightYellow;
                    case StatusValues.Closed:
                        return Brushes.Green;
                    default:
                        return Brushes.LightYellow;
                }
            }
        }

        public override string Status
        {
            get
            {
                return base.Status;
            }
            set
            {
                base.Status = value;
                NotifyPropertyChanged(()=> BugStatusColor);
            }
        }
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a ContextMenu suitable for the options available for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void PopulateContextMenu(Window ownerWindow, ContextMenu menu)
        {
            ContextWindowOwner = ownerWindow;
            AddContextMenuItem(menu, "Edit...", "Edit.png", Edit_Click);
        }

        Window ContextWindowOwner { get; set; }
        void Edit_Click(object sender, RoutedEventArgs e)
        {
            ShowEditor(ContextWindowOwner);
        }

        OPlannerBugDialog EditorDialog;
        public void ShowEditor(Window owner)
        {
            if (EditorDialog != null)
            {
                if (!EditorDialog.IsVisible)
                {
                    EditorDialog.Show();
                }

                EditorDialog.BringIntoView();
            }
            else
            {
                EditorDialog = new OPlannerBugDialog(owner, this);
                EditorDialog.Closed += EditorDialog_Closed;
                EditorDialog.Show();
            }
        }

        void EditorDialog_Closed(object sender, System.EventArgs e)
        {
            EditorDialog = null;
        }

    }
}
