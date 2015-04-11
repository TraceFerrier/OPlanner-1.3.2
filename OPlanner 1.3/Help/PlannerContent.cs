using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public static class PlannerContent
    {
        // DataGrid column headings
        public const string ColumnHeaderBusinessRank = "Business Rank";
        public const string ColumnHeaderLandingDate = "Expected Landing Date";
        public const string ColumnHeaderBacklogTitle = "Title";
        public const string ColumnHeaderBacklogCommitmentSetting = "Commitment Setting";
        public const string ColumnHeaderBacklogCommitmentStatus = "Commitment Status";
        public const string ColumnHeaderPrimaryPillar = "Primary Pillar";

        // Tooltips
        public const string ColumnHeaderStoryPointsToolTip = "Story Points allow you to provide a rough early estimate for how much time (in team working days) your team feels it will take to complete a particular backlog item.\r\rIf you provide Story Points to an item that has been 'Committed', but is not yet 'In Progress', OPlanner will use that value to forecast a landing date for your backlog item. You can then use these estimated landing dates to help determine how many backlog items your team can commit to for a particular train.\r\rOnce your team has created all the work items that describe and estimate the work for a backlog item, and you set the Commitment Setting to 'In Progress', OPlanner will use the work item estimates from then on to calculate the landing date.";
        public const string ColumnHeaderCommitmentSettingToolTip = "The 'Commitment Setting' for a backlog item indicates whether your team or pillar has 'committed' to delivering the work described by the backlog item by the end of a specific train.  The possible values are:\r\rUncommitted: the team has not committed to working on this item yet.\r\rCommitted: the team has committed to fully finishing all the work described by the backlog item before the end of the train that the item is assigned to.  Note that you cannot set an item to 'Committed' until the spec for the item is in the 'Ready for Coding' state.\r\rIn Progress: the team has created all the work items that fully describe and estimate the work required to finish a backlog item, and is ready to (or has begun) the work.\r\rComplete: The work for the backlog item is fully complete, including all coding, unit testing, automation, and bug fixing.";
        public const string ColumnHeaderLandingDateToolTip = "The 'Landing Date' for a backlog item is OPlanner's estimate for when all the work for that item is expected to be fully completed, taking into account all the work items that your team has created to describe and estimate the work required.\r\rIf the backlog item is in the 'Committed' state (typically before detailed work items have been created), you can assign Story Points (in units of working days) to the item, in which case OPlanner will use that value as a rough estimate to calculate a landing date.";
        public const string LandingDateTooltipCompleted = "All work for this item has been completed";

        public const string RibbonNewPillarToolTip = "New Pillar\r\rCreates a new Pillar for your product group.  Once you've created one or more pillars, you can then group your backlog items by assigning them to one of your pillars (using the Backlog tab).\r\rAll backlog items assigned to a pillar (as well as the work items under that backlog item) will be placed under that pillar's Product Path in Product Studio.";
        public const string RibbonEditPillarToolTip = "Edit Pillar\r\rOpens the editor for the pillar currently selected in the list below.";
        public const string RibbonNewExperienceToolTip = "New Experience\r\rCreates a new Experience, which allows you to group collections of related scenarios that describe the work needed to deliver an end-to-end user experience.";
        public const string RibbonNewBacklogToolTip = "New Backlog Item\r\rOnce you've created a backlog item, you can then create any number of work items parented to that backlog item, enabling your team members to detail and estimate the work required to fully complete and deliver the features described by the backlog item.";
        public const string RibbonApproveBacklogCommitmentsToolTip = "Approve Backlog Commitments\r\rApproves all the currently displayed committed backlog items for the selected pillar and train.  This signifies that the pillar is committing to completing all these items for this train.";

        public const string PillarOverviewToolTip = "Pillar Overview\r\rIn OPlanner, 'Pillars' represent a way for you to create broad 'sub-teams' or areas of ownership within your product group.\r\rOnce you create a pillar, you can group your Backlog Items (see the Backlog tab in the ribbon) by assigning them to one of your pillars, and then track and report on the progress of the work for each of your pillars individually.";
        public const string PeopleOverviewToolTip = "People Overview\r\rThis section shows you a summary of all the people that are part of your product group.\r\rThe members of your group consist of all the people that report to your team's Dev Manager, Test Manager, and GPM, and OPlanner automatically keeps this membership up-to-date daily.";

        public const string TrainCommitmentCompleted = "Commitment Completed";
        public const string TrainCommitmentOnTrack = "Commitment On Track";
        public const string TrainCommitmentPastDue = "Commitment Past Due";
        public const string TrainCommitmentAssignedToFutureTrain = Globals.c_AssignedToFutureTrain;
        public const string TrainCommitmentProjectedPastDue = "Commitment projected to finish late";
        public const string TrainCommitmentCompletedInLaterTrain = "Commitment was completed in a later train than the one it was originally committed to";
        public const string TrainCommitmentCompletedInEarlierTrain = "Commitment was completed in a train earlier than the one it was originally committed to";
        public const string TrainCommitmentChangedToLaterTrain = "Commitment has been moved to train later than the one it was originally committed to";
        public const string RecapTrainCommitmentChangedToLaterTrain = "Not Completed - work will continue during the next train";
        public const string TrainCommitmentChangedToEarlierTrain = "Commitment has been moved to train earlier than the one it was originally committed to";
        public const string TrainCommitmentCarriedOverFromPreviousTrain = "Carried over from previous train";
        public const string TrainCommitmentNewCommitment = "New Commitment";

        // Backlog validation messages
        public const string BacklogValidationInvalidSpecMessage = "The Spec Status of a Backlog Item cannot be set to 'Ready for Coding' or 'Spec Finalized' unless the Spec is specified as either a valid spec for your product group, set to 'No Spec Required', or set to 'Spec'd in Backlog'.";
        public const string BacklogValidationInvalidCommitmentMessage = "The Commitment Status of a Backlog Item cannot be changed to 'Committed' or 'In Progress' until the Spec for this backlog item is in the'Ready for Coding' or 'Spec Finalized state.";
        public const string BacklogValidationInvalidNoWorkCommitmentMessage = "The Commitment Status of a Backlog Item cannot be changed to 'In Progress' until the team has created and assigned work items describing the work required to complete this backog item.";
        public const string BacklogValidationCommitmentChangedOnClosedMessage = "This item has already been closed as 'Completed' - are you sure you want to re-activate it?";
        public const string BacklogValidationChangeTrainOfCommittedItemMessage = "This backlog item is committed to the train it is currently assigned to - by changing the assigned train for this item, do you want to commit this item to the new train?";
        public const string BacklogValidationAssignedToAliasInvalid = "Hold on - the alias you've entered in the 'Assigned To (Alias)' box wasn't recognized - please enter a valid alias.";

        // Backlog Editor xaml strings
        public const string BacklogEditorPostMortemHelp = "This backlog Item represents a 'Post Mortem Issue'. Spec Status is determined by the setting of the 'Post Mortem Status' attribute - if this attribute is set to 'PIR Complete', then a presentation spec is required to be attached to this item (using the 'Attached Files' tab on this dialog).  Select the Checkbox below only if you've attached an appropriate presentation.";

        // Pillar validation messages
        public const string PillarValidationNoTitle = "Please enter a title for this pillar.";
        public const string PillarValidationInvalidTitle = "The title you've given for this pillar is already in use by another pillar.  Please enter a different title.";
        public const string PillarValidationInvalidPillarPath = "The path you've selected for this pillar is already in use by another pillar.  Please select a different path in the Tree.";

        // Warning messages
        public const string ProductStudioMiddleTierProblems = "OPlanner is currently having difficulty communicating with Product Studio.  There may be some fields in your schedule that aren't populated, or cannot be changed. If you click Refresh, OPlanner will try again to read any missing data.";

    }

}
