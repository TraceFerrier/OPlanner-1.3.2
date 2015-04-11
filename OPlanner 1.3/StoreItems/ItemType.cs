
namespace PlannerNameSpace
{
    public enum ItemTypeID
    {
        // Schedule types
        ProductGroup,
        GroupMember,
        OffTime,
        Pillar,
        ScrumTeam,
        Train,
        Scenario,
        Experience,
        Quarter,
        Persona,
        PlannerBug,
        HelpContent,

        // Host item types
        BacklogItem,
        WorkItem,

        // Special nonitem types for filtering purposes
        FilterStatus,
        FilterBugStatus,
        FilterBugAssignedTo,
        FilterIssueType,
        FilterResolution,
        FilterForecastable,

        Unknown,
    }

}
