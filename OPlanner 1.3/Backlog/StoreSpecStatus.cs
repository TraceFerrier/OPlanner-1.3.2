using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public enum ComparisonResult
    {
        GreaterThan,
        Equals,
        LessThan,
        Undefined,
    }

    public class StoreSpecStatusTextValues
    {
        public const string Placeholder = "0. Placeholder";
        public const string PageOne = "1. Page One";
        public const string ReadyForReview = "2. Ready for Review";
        public const string CoreDesignApproved = "3. Core Design Approved";
        public const string ReadyForFinalReview = "4. Ready for Final Review";
        public const string ReadyForCoding = "5. Ready for Coding";
        public const string SpecFinalized = "6. Spec Finalized";
        public const string SpecNotNeeded = "7. Not Needed";
    }

    public enum StoreSpecStatusValue
    {
        NotSpecified,
        NoSpecRequired,
        Placeholder,
        PageOne,
        ReadyForReview,
        CoreDesignApproved,
        ReadyForFinalReview,
        ReadyForCoding,
        SpecFinalized,
    }

    public class StoreSpecStatus
    {
        public static StoreSpecStatusValue GetStoreSpecStatus(string statusText)
        {
            if (Utils.StringsMatch(statusText, Globals.c_noSpecRequired))
            {
                return StoreSpecStatusValue.NotSpecified;
            }

            if (Utils.StringsMatch(statusText, Globals.c_NotSet))
            {
                return StoreSpecStatusValue.NotSpecified;
            }

            switch (statusText)
            {
                case StoreSpecStatusTextValues.Placeholder:
                    return StoreSpecStatusValue.Placeholder;

                case StoreSpecStatusTextValues.PageOne:
                    return StoreSpecStatusValue.PageOne;

                case StoreSpecStatusTextValues.ReadyForReview:
                    return StoreSpecStatusValue.ReadyForReview;

                case StoreSpecStatusTextValues.CoreDesignApproved:
                    return StoreSpecStatusValue.CoreDesignApproved;

                case StoreSpecStatusTextValues.ReadyForFinalReview:
                    return StoreSpecStatusValue.ReadyForFinalReview;

                case StoreSpecStatusTextValues.ReadyForCoding:
                    return StoreSpecStatusValue.ReadyForCoding;

                case StoreSpecStatusTextValues.SpecFinalized:
                    return StoreSpecStatusValue.SpecFinalized;
                default:
                    return StoreSpecStatusValue.NotSpecified;

            }
        }

        public static ComparisonResult CompareStoreSpecStatus(StoreSpecStatusValue s1, StoreSpecStatusValue s2)
        {
            if (s1 == StoreSpecStatusValue.NoSpecRequired || s1 == StoreSpecStatusValue.NotSpecified)
            {
                return ComparisonResult.Undefined;
            }

            if (s2 == StoreSpecStatusValue.NoSpecRequired || s2 == StoreSpecStatusValue.NotSpecified)
            {
                return ComparisonResult.Undefined;
            }

            if (s1 == s2)
            {
                return ComparisonResult.Equals;
            }

            int s1Position = -1;
            int s2Position = -1;
            int position = 0;
            Array enumValues = typeof(StoreSpecStatusValue).GetEnumValues();
            foreach (StoreSpecStatusValue enumValue in enumValues)
            {
                if (enumValue == s1)
                {
                    s1Position = position;
                }

                if (enumValue == s2)
                {
                    s2Position = position;
                }

                position++;
            }

            if (s1Position > s2Position)
            {
                return ComparisonResult.GreaterThan;
            }
            else
            {
                return ComparisonResult.LessThan;
            }
        }
    }
}
