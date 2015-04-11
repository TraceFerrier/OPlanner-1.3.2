using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class PersonaItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.Persona; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }
        public override bool IsGlobalItem { get { return true; } }

        public static void CreateItem()
        {
            PersonaItem newItem = ScheduleStore.Instance.CreateStoreItem<PersonaItem>(ItemTypeID.Persona);
            newItem.Title = "New Persona";
            newItem.SaveNewItem();
        }

        public static PersonaItem GetDummyNoneTeam()
        {
            return StoreItem.GetDummyItem<PersonaItem>(DummyItemType.NoneType);
        }

        public static PersonaItem GetDummyAllTeam()
        {
            return StoreItem.GetDummyItem<PersonaItem>(DummyItemType.AllType);
        }

        public string PersonaDescription
        {
            get { return GetStringValue(Datastore.PropNamePersonaDescription); }
            set { SetStringValue(Datastore.PropNamePersonaDescription, value); }
        }
    }
}
