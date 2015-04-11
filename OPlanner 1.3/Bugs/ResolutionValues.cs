using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    class ResolutionValues
    {
        public const string ByDesign = "By Design";
        public const string Duplicate = "Duplicate";
        public const string External = "External";
        public const string Fixed = "Fixed";
        public const string NotRepro = "Not Repro";
        public const string Postponed = "Postponed";
        public const string WontFix = "Won't Fix";

        public static AsyncObservableCollection<string> Values
        {
            get
            {
                AsyncObservableCollection<string> resolutionValues = new AsyncObservableCollection<string>();
                resolutionValues.Add(ByDesign);
                resolutionValues.Add(Duplicate);
                resolutionValues.Add(External);
                resolutionValues.Add(Fixed);
                resolutionValues.Add(NotRepro);
                resolutionValues.Add(Postponed);
                resolutionValues.Add(WontFix);

                return resolutionValues;
            }
        }

        public static AsyncObservableCollection<string> ValuesAllNone
        {
            get
            {
                AsyncObservableCollection<string> resolutionValues = Values;
                resolutionValues.Insert(0, Globals.c_All);
                resolutionValues.Insert(0, Globals.c_None);
                return resolutionValues;
            }
        }
    }
}
