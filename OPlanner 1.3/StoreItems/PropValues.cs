
namespace PlannerNameSpace
{
    public class SubStatusValues
    {
        public const string Investigating = "Investigating";
        public const string WorkingOnFix = "Working on a fix";
        public const string ActionPending = "Action Pending";
        public const string Blocked = "Blocked";
    }

    public class SubtypeValues
    {
        public const string ProductCoding = "Product Coding";
        public const string Automation = "Automation";
    }

    class ResolutionType
    {
        public const string ByDesign = "By Design";
        public const string Duplicate = "Duplicate";
        public const string External = "External";
        public const string Fixed = "Fixed";
        public const string NotRepro = "Not Repro";
        public const string Postponed = "Postponed";
        public const string WontFix = "Won't Fix";
        public const string None = "None";
    }

    public class DisciplineValues
    {
        public const string Dev = "Dev";
        public const string Test = "Test";
        public const string PM = "PM";
    }

    public class WorkItemDisplayStates
    {
        public const string NotStarted = "Not Started";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";
        public const string CompletedAndResolved = "Completed and Resolved";
        public const string CompletedAndClosed = "Completed and Closed";
        public const string Delete = "Delete";
    }

    public enum WorkItemStates
    {
        NotSet,
        NotStarted,
        InProgress,
        Completed,
        Delete,
    }
}
