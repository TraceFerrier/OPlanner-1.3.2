using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PlannerNameSpace
{
    public static class Globals
    {
        public static bool IsShuttingDown = false;

        public static void Shutdown()
        {
            IsShuttingDown = true;
            App.Current.Shutdown();
        }

        public static void Startup()
        {
            bool optionsDialogInUse = false;
            bool shouldUseCloneHostStore = false;
            bool shouldClearUserPreferences = false;
            bool shouldClearProductGroup = false;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift))
            {
                StartupOptionsDialog dialog = new StartupOptionsDialog();
                dialog.ShowDialog();

                if (dialog.QuitSelected)
                {
                    Shutdown();
                    return;
                }

                if (!dialog.DialogCancelled)
                {
                    optionsDialogInUse = true;
                    shouldUseCloneHostStore = dialog.ShouldUseClone;
                    shouldClearUserPreferences = dialog.ShouldClearUserPreferences;
                    shouldClearProductGroup = dialog.ShouldClearCurrentProductGroup;
                }
            }

            s_userPreferences = UserPreferences.Unserialize(shouldClearUserPreferences);
            CreateManagers();

            if (optionsDialogInUse)
            {
                s_userPreferences.ShouldUseCloneHostStore = shouldUseCloneHostStore;
            }

            if (shouldUseCloneHostStore)
            {
                shouldClearProductGroup = true;
            }

            s_applicationManager.Startup(shouldClearProductGroup);
        }

        public static void InitializeMainWindow(MainWindow mainWindow)
        {
            g_mainWindow = mainWindow;
            ApplicationManager.InitializeMainWindow();
        }

        static void CreateManagers()
        {
            EventManager = new EventManager();
            s_applicationManager = new ApplicationManager();
            ItemManager = new ItemManager();
            ExperienceManager = new ExperienceManager();
            OPlannerBugsDataSource = new OPlannerBugsDataSource();
            FilterManager = new FilteredCollectionManager();
            GroupMemberManager = new GroupMemberManager();
            CommitmentStatusManager = new CommitmentStatusManager();
            RichTextManager = new RichTextManager();
            StoreItemValueManager = new StoreItemValueManager();
            ProductTreeManager = new ProductTreeManager();
            ScrumTeamManager = new ScrumTeamManager();
        }

        public static void OpenProductGroup(ProductGroupItem productGroupItem)
        {
            string productGroupItemKey = productGroupItem.StoreKey;
            s_applicationManager.Restart(productGroupItemKey);
        }

        public const int MaxShortStringLength = 100;
        public const double AvgCapacityPerDay = 6;
        public const int IdealHoursPerDay = 8;
        public const int BugBufferDaysBetweenBacklogItems = 5;
        public const string c_NotSet = "<Not Set>";
        public const string c_All = "<All>";
        public const string c_NoneSpecTeamName = "None";
        public const string c_None = "<None>";
        public const string c_ExternalTeam = "<External Team>";
        public const string c_noSpecRequired = "(No Spec Required)";
        public const string c_SpecTBD = "(Spec Location TBD)";
        public const string c_SpecdInBacklog = "(Spec'd in Backlog)";
        public const string c_AssignedToFutureTrain = "Assigned to a Future Train";
        public const string OfficeCurrentShipCycle = "Gemini";
        public const string OfficeBacklogFixBy = "TBD (Product Backlog)";
        public const string AcceptanceCriteriaFileName = "AcceptanceCriteria.rtf";
        public const string DescriptionFileName = "Description.rtf";
        public const string PSErrIssueUpdated = "The issue has been updated. Reopen the issue and reapply your changes";
        public const string PSErrAttachmentShare = "The middle tier cannot access the permanent attachment share.";
        public const string PSErrExecuteFailedPleaseRetry = "Please retry the transaction";
        public const string CannotForecast = "Cannot Forecast";
        public const string GoStatus = "Current";
        public const string NoGoStatus = "Not Current";
        public const string CalculatingStatus = "(Calculating...)";
        public const string NotAssigned = "(Not Assigned)";
        public const string AlreadyCompleted = "(Completed)";
        public const string NotCommitted = "Not Committed";
        public const string CommittedNotApproved = "Ready for commitment, not yet approved";
        public const string CommittedAndApproved = "Committed and approved, not yet started";
        public const string NotScheduled = "(No work scheduled)";
        public const string NotInProgress = "(Not Started)";
        public const string DefaultScrumTeamName = "New Scrum Team";
        public const string CommitmentsApprovalNotDetermined = "(Determining commitment approval status...)";
        public const string CommitmentsApproved = "Commitments have been approved for the selected Pillar and Train.";
        public const string CommitmentsNotApproved = "Commitments have not yet been approved for the selected Pillar and Train.";
        public const string CommitmentsNotApplicable = "(Select a specific pillar and train to see commitment status)";

        // Experience View constants
        public const int standardCellHeight = 145;
        public const int experienceRowHeight = standardCellHeight + 12;
        public const int summaryCellHeight = standardCellHeight - 60;
        public const int summaryRowHeight = experienceRowHeight - 60;

        public static GridLength ExpTitleHeight = new GridLength(24);
        public static double ExpCellHeight = standardCellHeight;
        public static double ExpRowHeight = experienceRowHeight;
        public static double ExpSummaryCellHeight = summaryCellHeight;
        public static double ExpSummaryRowHeight = summaryRowHeight;
        public static Thickness BacklogSummaryViewMargin = new Thickness(3);

        // Burndown chart constants
        public static GridLength BurndownHeadingHeight = new GridLength(48);

        public const string ActiveStatusColor = "#33E53D10";
        public const string ResolvedStatusColor = "#33FFFF00";
        public const string ClosedStatusColor = "#3300FF00";

        public static Color LightRed
        {
            get { return Color.FromRgb(255, 100, 100); }
        }

        static SolidColorBrush m_lightRedBrush;
        public static SolidColorBrush LightRedBrush
        {
            get
            {
                if (m_lightRedBrush == null)
                {
                    m_lightRedBrush = new SolidColorBrush();
                    m_lightRedBrush.Color = LightRed;
                }

                return m_lightRedBrush;
            }
        }


        public static MainWindow MainWindow
        {
            get { return g_mainWindow; }
            set { g_mainWindow = value; }
        }

        private static MainWindow g_mainWindow;
        private static UserPreferences s_userPreferences;
        private static ApplicationManager s_applicationManager;

        public static ApplicationManager ApplicationManager
        {
            get { return Globals.s_applicationManager; }
            set { Globals.s_applicationManager = value; }
        }

        public static UserPreferences UserPreferences
        {
            get { return Globals.s_userPreferences; }
            set { Globals.s_userPreferences = value; }
        }

        public static ItemManager ItemManager { get; set; }
        public static EventManager EventManager { get; set; }
        public static ExperienceManager ExperienceManager { get; set; }
        public static OPlannerBugsDataSource OPlannerBugsDataSource { get; set; }
        public static FilteredCollectionManager FilterManager { get; set; }
        public static GroupMemberManager GroupMemberManager { get; set; }
        public static CommitmentStatusManager CommitmentStatusManager { get; set; }
        public static RichTextManager RichTextManager { get; set; }
        public static StoreItemValueManager StoreItemValueManager { get; set; }
        public static ProductTreeManager ProductTreeManager { get; set; }
        public static ScrumTeamManager ScrumTeamManager { get; set; }
        public static GroupMemberItemCache GroupMembers
        {
            get
            {
                return Globals.ItemManager.GroupMemberItems;
            }
        }
    }
}
