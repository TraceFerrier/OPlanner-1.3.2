using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class ScrumTeamChangedEventArgs : EventArgs
    {
        public ScrumTeamChangedEventArgs(ScrumTeamItem currentItem)
        {
            CurrentItem = currentItem;
        }

        public ScrumTeamItem CurrentItem { get; set; }
    }

    public delegate void ScrumTeamItemEventHandler(Object sender, ScrumTeamChangedEventArgs e);
}
