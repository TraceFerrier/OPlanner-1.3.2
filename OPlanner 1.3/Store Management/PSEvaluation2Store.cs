using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductStudio;

namespace PlannerNameSpace
{
    public abstract class PSEvaluation2Store : Datastore
    {
        public static class GroupMemberFields
        {
            public static readonly int IdxJobTitle = 0;
            public static readonly int IdxPillarKey = 1;
            public static readonly int IdxAvgCapacity = 2;
        }

        public PSEvaluation2Store(StoreType storeType)
            : base(storeType)
        {
        }

        public override string StoreName { get { return "PSEvaluation2"; } }
        public override int TeamRootNode { get { return 0; } }
        public override int TeamRootDepth { get { return 1; } }

        public override string PropNameType { get { return "Source ID"; } }

        protected override void InitializeProperties()
        {
            // Notes: Scenario field is bound to a list of allowed values if the Milestone field is set
            // so it can't be used as a freeform field if the milestone field is used.

            AddItemType(ItemTypeID.ProductGroup, "{81653854-355E-43e8-8F1E-89AC8165CF36}");
            AddItemType(ItemTypeID.Train, "{426247A7-A844-45BB-8972-D332CFD2EE7C}");
            AddItemType(ItemTypeID.GroupMember, "{4B06A238-EBB6-457f-A105-4A085CF5261A}");
            AddItemType(ItemTypeID.OffTime, "{57BA61BD-D51E-4A92-B571-A2ABB08C5B79}");
            AddItemType(ItemTypeID.Pillar, "{8676B0C9-F000-4a07-AB3E-578DCD36C876}");
            AddItemType(ItemTypeID.ScrumTeam, "{8DA2B78D-A6C9-41c0-BFBE-A27F83F292B4}");
            AddItemType(ItemTypeID.Scenario, "{6B125C65-ECDD-4FFE-B416-47F7D4F5DBA3}");
            AddItemType(ItemTypeID.Experience, "{1917D5C4-9240-4D8C-8E02-6D078C910CEE}");
            AddItemType(ItemTypeID.Quarter, "{26568D19-E1AB-43CE-A801-168EB6A43C33}");
            AddItemType(ItemTypeID.Persona, "{A7408746-F262-4876-A03F-858743A1A77E}");
            AddItemType(ItemTypeID.PlannerBug, "{56E286FA-A624-4416-BBD9-A4058E04CAD7}");
            AddItemType(ItemTypeID.HelpContent, "{BF1E0494-9F02-4C7D-B8C3-9C78481BC855}");
            AddItemType(ItemTypeID.BacklogItem, "{AEA1731B-9F1E-4316-B430-371D0D0AD969}");
            AddItemType(ItemTypeID.WorkItem, "{9E1DD697-7A16-419A-B734-629107074920}");

            // Until we have a Schedule Database with a schema that defines the allowed
            // values for the following properties, set the proposed allowed values directly.
            SetFieldAllowedValues(Datastore.PropNameDiscipline, new AllowedValue { Value = DisciplineValues.Dev });
            SetFieldAllowedValues(Datastore.PropNameDiscipline, new AllowedValue { Value = DisciplineValues.Test });
            SetFieldAllowedValues(Datastore.PropNameDiscipline, new AllowedValue { Value = DisciplineValues.PM });
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Thanks to the small number of fields in this store, we persist some values for
        /// some StoreItems as composite values.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected override void RegisterCompositeValues(CompositeValueRegistry registry)
        {
            // GroupMemberItem
            registry.RegisterCompositeValue(Datastore.PropNameJobTitlePillarAndAvgCapacity, Utils.GetPropertyName((GroupMemberItem s) => s.JobTitle), GroupMemberFields.IdxJobTitle);
            registry.RegisterCompositeValue(Datastore.PropNameJobTitlePillarAndAvgCapacity, Utils.GetPropertyName((GroupMemberItem s) => s.PillarItemKey), GroupMemberFields.IdxPillarKey);
            registry.RegisterCompositeValue(Datastore.PropNameJobTitlePillarAndAvgCapacity, Utils.GetPropertyName((GroupMemberItem s) => s.CapacityPerDay), GroupMemberFields.IdxAvgCapacity);

            // ProductGroupItem
            registry.RegisterCompositeValue(Datastore.PropNameProductGroupComposite, Utils.GetPropertyName((ProductGroupItem s) => s.DefaultSpecTeamName), 0);
            registry.RegisterCompositeValue(Datastore.PropNameProductGroupComposite, Utils.GetPropertyName((ProductGroupItem s) => s.GroupAdmin1), 1);
            registry.RegisterCompositeValue(Datastore.PropNameProductGroupComposite, Utils.GetPropertyName((ProductGroupItem s) => s.GroupAdmin2), 2);
            registry.RegisterCompositeValue(Datastore.PropNameProductGroupComposite, Utils.GetPropertyName((ProductGroupItem s) => s.GroupAdmin3), 3);
            registry.RegisterCompositeValue(Datastore.PropNameProductGroupComposite, Utils.GetPropertyName((ProductGroupItem s) => s.HostItemStoreName), 4);

        }

        public override string DefaultTeamTreePath { get { return "\\test\\"; } }
        public override string DefaultMemberListTreePath { get { return "\\test\\"; } }
        public override string DefaultMilestoneTreePath { get { return "\\test\\"; } }
        public override string DefaultSprintTreePath { get { return "\\test\\"; } }
        public override string DefaultTaskFolderTreePath { get { return "\\test\\"; } }

        public override void InitializeRequiredFieldValues(StoreItem item)
        {
            item.SetStringValue("Issue type", "Spec Issue", "Issue type");
            item.Severity = "2";
            item.OpenedBy = Globals.ApplicationManager.CurrentUserAlias;
            item.OpenedDate = DateTime.Now;
        }
    }
}
