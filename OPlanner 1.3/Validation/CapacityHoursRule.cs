using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PlannerNameSpace
{
    class CapacityHoursRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (!Utils.IsValidNumber(value, 0, 8))
            {
                return new ValidationResult(false, "The entered value for capacity must be a number between 0 and 8");
            }

            return ValidationResult.ValidResult;
        }
    }
}
