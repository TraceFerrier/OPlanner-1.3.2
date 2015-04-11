using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public class AllowedValue
    {
        public object Value { get; set; }

        public AllowedValue() { }

        public AllowedValue(object value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public bool IsAll
        {
            get
            {
                string value = Value as string;
                if (value == null)
                {
                    return false;
                }

                return value == Globals.c_All;
            }
        }

        public static AllowedValue GetAllowedValueFromList(AsyncObservableCollection<AllowedValue> values, string textValue)
        {
            foreach (AllowedValue value in values)
            {
                if (Utils.StringsMatch(value.Value as string, textValue))
                {
                    return value;
                }
            }

            return null;
        }
    }
}
