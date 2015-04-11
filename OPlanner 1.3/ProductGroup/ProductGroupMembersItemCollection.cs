using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class ProductGroupMembersItemCollection : ViewItemCollection<GroupMemberItem>
    {
        protected ProductGroupState ViewState;

        public ProductGroupMembersItemCollection(ProductGroupState viewState)
        {
            ViewState = viewState;
            ViewState.DisciplineChanged += ViewState_DisciplineChanged;
            ViewState.MemberPillarState.CurrentItemChanged += MemberPillarState_CurrentItemChanged;
        }

        void MemberPillarState_CurrentItemChanged(object sender, EventArgs e)
        {
            RefreshCollection();
        }

        void ViewState_DisciplineChanged()
        {
            RefreshCollection();
        }

        public override bool ItemFilter(object viewObject)
        {
            GroupMemberItem memberItem = viewObject as GroupMemberItem;
            if (memberItem != null)
            {
                if (ViewState.MemberPillarState.CurrentItem == null)
                {
                    return true;
                }
                else if (ViewState.MemberPillarState.CurrentItem.IsNoneItem)
                {
                    if (!memberItem.ParentPillarItem.IsNoneItem)
                    {
                        return false;
                    }
                }
                else if (StoreItem.IsRealItem(ViewState.MemberPillarState.CurrentItem))
                {
                    if (memberItem.ParentPillarItem != ViewState.MemberPillarState.CurrentItem)
                    {
                        return false;
                    }
                }

                if (!ViewState.CurrentDiscipline.IsAll)
                {
                    if (!Utils.StringsMatch(memberItem.Discipline, ViewState.CurrentDiscipline.ToString()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
   }
}
