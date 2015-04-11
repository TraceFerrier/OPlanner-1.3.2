using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class StatusValues
    {
        public const string Active = "Active";
        public const string Resolved = "Resolved";
        public const string Closed = "Closed";

        public static AsyncObservableCollection<string> Values
        {
            get
            {
                AsyncObservableCollection<string> values = new AsyncObservableCollection<string>();
                values.Add(Active);
                values.Add(Resolved);
                values.Add(Closed);

                return values;
            }
        }

        public static AsyncObservableCollection<string> ValuesAll
        {
            get
            {
                AsyncObservableCollection<string> values = Values;
                values.Insert(0, Globals.c_All);
                return values;
            }
        }
    }

}
