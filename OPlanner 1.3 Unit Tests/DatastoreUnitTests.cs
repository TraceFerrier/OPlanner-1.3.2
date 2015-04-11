using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlannerNameSpace;

namespace ScheduleManager20UnitTests
{
    [TestClass]
    public class DatastoreUnitTests
    {
        [TestMethod]
        public void TestSetSubstring()
        {
            string result = Utils.SetSubstring("foo^bar^cap", null, 0, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "^bar^cap"));

            result = Utils.SetSubstring("foo^bar^cap", null, 1, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "foo^^cap"));

            result = Utils.SetSubstring("foo^bar^cap", null, 2, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "foo^bar^"));

            result = Utils.SetSubstring("foo^bar^cap", "dream", 0, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "dream^bar^cap"));

            result = Utils.SetSubstring("foo^bar^cap", "dream", 1, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "foo^dream^cap"));

            result = Utils.SetSubstring("foo^bar^cap", "dream", 2, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "foo^bar^dream"));

            result = Utils.SetSubstring(null, "dream", 0, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "dream"));

            result = Utils.SetSubstring(null, "dream", 1, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "^dream"));

            result = Utils.SetSubstring(null, "dream", 2, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "^^dream"));

            result = Utils.SetSubstring("foo^^cap", "dream", 1, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "foo^dream^cap"));

            result = Utils.SetSubstring("^^^^^", "dream", 0, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "dream^^^^^"));

            result = Utils.SetSubstring("^^^^^", "dream", 1, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "^dream^^^^"));

            result = Utils.SetSubstring("^^^^^", "dream", 2, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "^^dream^^^"));


        }

        [TestMethod]
        public void TestGetSubstring()
        {
            string result = Utils.GetSubstring(null, 2, '^');
            Assert.IsTrue(result == null);

            result = Utils.GetSubstring("PRINCIPAL DEVELOPMENT LEAD^PSEvaluation2|11032", 0, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "PRINCIPAL DEVELOPMENT LEAD"));

            result = Utils.GetSubstring("PRINCIPAL DEVELOPMENT LEAD^PSEvaluation2|11032", 1, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "PSEvaluation2|11032"));

            result = Utils.GetSubstring("PRINCIPAL DEVELOPMENT LEAD^PSEvaluation2|11032", 2, '^');
            Assert.IsTrue(result == null);

            result = Utils.GetSubstring("PRINCIPAL DEVELOPMENT LEAD^PSEvaluation2|11032^2", 2, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "2"));

        }

        [TestMethod]
        public void TestFindSubstring()
        {
            Assert.IsTrue(Utils.FindSubstring("fools^walk^in", "fools", '^') == 0);
            Assert.IsTrue(Utils.FindSubstring("fools^walk^in", "Walk", '^') == 1);
            Assert.IsTrue(Utils.FindSubstring("fools^walk^in", "in", '^') == 2);

            Assert.IsTrue(Utils.FindSubstring(null, "anything", '^') < 0);
            Assert.IsTrue(Utils.FindSubstring("", "in", '^') < 0);
            Assert.IsTrue(Utils.FindSubstring("Train1^Train2^Train3^Train4", "Train5", '^') < 0);

            Assert.IsTrue(Utils.FindSubstring("Train 1^Train 2^Train 3^Train 4", "Train 3", '^') == 2);

        }

        [TestMethod]
        public void TestClearSubstring()
        {
            string result = Utils.ClearSubstring("falling^into^infinity", 0, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "into^infinity"));

            result = Utils.ClearSubstring("falling^into^infinity", 1, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "falling^infinity"));

            result = Utils.ClearSubstring("falling^into^infinity", 2, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "falling^into"));

            result = Utils.ClearSubstring("falling^into^infinity", 5, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "falling^into^infinity"));

            result = Utils.ClearSubstring("falling^into^infinity", -1, '^');
            Assert.IsTrue(Utils.StringsMatch(result, "falling^into^infinity"));

            result = Utils.ClearSubstring(null, 0, '^');
            Assert.IsTrue(result == null);

        }

        [TestMethod]
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Ensure that all Datastore-derived classes define values for every ItemTypeID.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void TestShouldFailIfAnyTypeIDsAreUndefined()
        {
            //ScheduleStore scheduleStore = ScheduleStore.Instance;

            //// Ensure that any unrecognized typeID name returns the expected 'error' type enum value
            //Assert.IsTrue(scheduleStore.GetItemTypeID("foo") == ItemTypeID.Unknown);
            //Assert.IsTrue(scheduleStore.GetItemTypeID("bar") == ItemTypeID.Unknown);

            //foreach (ItemTypeID typeID in Enum.GetValues(typeof(ItemTypeID)))
            //{
            //    // 'Unknown' marks the end of the enum
            //    if (typeID != ItemTypeID.Unknown)
            //    {
            //        string itemTypeName = scheduleStore.GetItemTypeName(typeID);
            //        Assert.IsTrue(itemTypeName != ItemTypeID.Unknown.ToString());
            //    }
            //}
        }

        [TestMethod]
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Ensure that all Datastore-derived classes define values for every PropID.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void TestShouldFailIfAnyPropIDsAreUndefined()
        {
            //foreach (PropID prop in Enum.GetValues(typeof(PropID)))
            //{
            //    // 'StartCustom' marks the end of props that are required to be set.
            //    if (prop == PropID.StartCustom)
            //    {
            //        break;
            //    }

            //    string propertyName = ScheduleStore.Instance.GetPropertyName(prop);
            //    Assert.IsTrue(propertyName != PropID.Unknown.ToString(), "PropID '" + prop.ToString() + "' undefined by store '" + ScheduleStore.Instance.StoreID.Name + "'.");
            //}
        }
    }
}
