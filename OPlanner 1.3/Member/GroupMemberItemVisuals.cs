using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Reflection;


namespace PlannerNameSpace
{
    public partial class GroupMemberItem
    {
        public double WorkBarWidth { get { return 190; } }
        public double WorkFillWidth
        {
            get
            {
                double available = CurrentTrainHoursRemaining;
                double remaining = CurrentTrainWorkRemaining;
                if (available == 0)
                {
                    return WorkBarWidth;
                }

                double width = (remaining / available) * WorkBarWidth;
                if (width > WorkBarWidth)
                {
                    width = WorkBarWidth;
                }

                return width;
            }
        }

        public SolidColorBrush WorkFillColor
        {
            get
            {
                return Utils.GetWorkCapacityFillColor(CurrentTrainWorkRemaining, CurrentTrainHoursRemaining);
            }
        }

        public string DisplayHoursOfTotal
        {
            get
            {
                return "(" + CurrentTrainWorkRemaining.ToString() + " of " + CurrentTrainHoursRemaining.ToString() + " hours remaining this train";
            }
        }

    }
}
