using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class MemberDescriptor
    {
        public UserInformation Member { get; set; }
        public string Discipline { get; set; }
        public string Alias
        {
            get
            {
                if (Member != null)
                {
                    return Member.Alias;
                }

                return null;
            }
        }

        public string DisplayName
        {
            get
            {
                if (Member != null)
                {
                    return Member.DisplayName;
                }

                return null;
            }
        }

        public string Title
        {
            get
            {
                if (Member != null)
                {
                    return Member.JobTitle;
                }

                return null;
            }
        }
    }

}
