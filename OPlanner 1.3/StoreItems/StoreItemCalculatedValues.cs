using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public partial class GroupMemberItem
    {
        #region Off-time Management

        private string m_totalOffDays;
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a string representing the total number of off days over the next 2 months
        /// for this member.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string TotalOffDays
        {
            get
            {
                if (m_totalOffDays == null)
                {
                    DateTime startDate = DateTime.Today;
                    DateTime endDate = startDate.AddMonths(2);
                    int netDaysOff = Utils.GetNetOffDays(startDate, endDate, OffTimeItems);
                    m_totalOffDays = netDaysOff.ToString() + " Days";
                }

                return m_totalOffDays;
            }
        }

        #endregion

        private int m_totalWorkRemaining = -1;
        private int m_totalWorkCompleted = -1;
        private int m_currentTrainWorkRemaining = -1;
        private int m_currentTrainHoursRemaining = -1;
        public int TotalWorkRemaining
        {
            get
            {
                if (m_totalWorkRemaining < 0)
                {
                    m_totalWorkRemaining = 0;
                    if (m_workItems.Count > 0)
                    {
                        int idx = 0;
                        do
                        {
                            m_totalWorkRemaining += m_workItems[idx++].WorkRemaining;
                        } while (idx < m_workItems.Count);
                    }
                }

                return m_totalWorkRemaining;
            }
        }

        public int CurrentTrainWorkRemaining
        {
            get
            {
                if (m_currentTrainWorkRemaining < 0)
                {
                    m_currentTrainWorkRemaining = 0;
                    if (m_workItems.Count > 0)
                    {
                        TrainItem thisTrain = TrainManager.Instance.CurrentTrain;
                        int idx = 0;
                        do
                        {
                            WorkItem workItem = m_workItems[idx++];
                            if (!StoreItem.IsRealItem(thisTrain) || workItem.GetParentTrainItem() == thisTrain)
                            {
                                m_currentTrainWorkRemaining += workItem.WorkRemaining;
                            }
                        } while (idx < m_workItems.Count);
                    }
                }

                return m_currentTrainWorkRemaining;
            }
        }

        public int CurrentTrainHoursRemaining
        {
            get
            {
                if (m_currentTrainHoursRemaining < 0)
                {
                    m_currentTrainHoursRemaining = Utils.GetNetWorkingHours(DateTime.Today, TrainManager.Instance.CurrentTrain.EndDate, OffTimeItems);
                }

                return m_currentTrainHoursRemaining;
            }
        }

        public int TotalWorkCompleted
        {
            get
            {
                if (m_totalWorkCompleted < 0)
                {
                    m_totalWorkCompleted = 0;
                    if (m_workItems.Count > 0)
                    {
                        int idx = 0;
                        do
                        {
                            WorkItem workItem = m_workItems[idx++];
                            m_totalWorkCompleted += workItem.Completed;
                        } while (idx < m_workItems.Count);
                    }
                }

                return m_totalWorkRemaining;
            }
        }

    }
}
