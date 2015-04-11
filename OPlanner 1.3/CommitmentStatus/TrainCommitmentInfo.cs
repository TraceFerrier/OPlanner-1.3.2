using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PlannerNameSpace
{
    public enum TrainCommitmentStatusValue
    {
        NotCalculated,
        NotCommitted,
        CommittedNotApproved,
        CommittedAndApproved,
        Completed,
        ProjectedPastDue,
        PastDue,
        MovedToLaterTrain,
        MovedToEarlierTrain,
        AssignedToFutureTrain,
        CarriedOverFromPreviousTrain,
        OnTrack,
    }

    public class TrainCommitmentInfo
    {
        public TrainCommitmentStatusValue StatusValue { get; set; }
        public string StatusText { get; set; }
        public Brush StatusColor { get; set; }

        public static void CreateInfoValues(Dictionary<TrainCommitmentStatusValue, TrainCommitmentInfo> infoValues)
        {
            infoValues.Add(TrainCommitmentStatusValue.NotCalculated, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.NotCalculated,
                StatusText = Globals.CalculatingStatus,
                StatusColor = Brushes.DarkGray
            });

            infoValues.Add(TrainCommitmentStatusValue.NotCommitted, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.NotCommitted,
                StatusText = Globals.NotCommitted,
                StatusColor = Brushes.DarkGoldenrod,
            });

            infoValues.Add(TrainCommitmentStatusValue.CommittedNotApproved, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.CommittedNotApproved,
                StatusText = Globals.CommittedNotApproved,
                StatusColor = Brushes.Red,
            });

            infoValues.Add(TrainCommitmentStatusValue.CommittedAndApproved, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.CommittedAndApproved,
                StatusText = Globals.CommittedAndApproved,
                StatusColor = Brushes.DarkGreen
            });

            infoValues.Add(TrainCommitmentStatusValue.MovedToLaterTrain, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.MovedToLaterTrain,
                StatusText = PlannerContent.TrainCommitmentChangedToLaterTrain,
                StatusColor = Brushes.DarkRed
            });

            infoValues.Add(TrainCommitmentStatusValue.MovedToEarlierTrain, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.MovedToEarlierTrain,
                StatusText = PlannerContent.TrainCommitmentChangedToEarlierTrain,
                StatusColor = Brushes.DarkSeaGreen
            });

            infoValues.Add(TrainCommitmentStatusValue.OnTrack, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.OnTrack,
                StatusText = PlannerContent.TrainCommitmentOnTrack,
                StatusColor = Brushes.Green
            });

            infoValues.Add(TrainCommitmentStatusValue.PastDue, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.PastDue,
                StatusText = PlannerContent.TrainCommitmentPastDue,
                StatusColor = Brushes.Red
            });

            infoValues.Add(TrainCommitmentStatusValue.ProjectedPastDue, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.ProjectedPastDue,
                StatusText = PlannerContent.TrainCommitmentProjectedPastDue,
                StatusColor = Brushes.IndianRed
            });

            infoValues.Add(TrainCommitmentStatusValue.AssignedToFutureTrain, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.AssignedToFutureTrain,
                StatusText = PlannerContent.TrainCommitmentAssignedToFutureTrain,
                StatusColor = Brushes.Purple
            });

            infoValues.Add(TrainCommitmentStatusValue.Completed, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.Completed,
                StatusText = Globals.AlreadyCompleted,
                StatusColor = Brushes.DarkGreen
            });

            infoValues.Add(TrainCommitmentStatusValue.CarriedOverFromPreviousTrain, new TrainCommitmentInfo
            {
                StatusValue = TrainCommitmentStatusValue.CarriedOverFromPreviousTrain,
                StatusText = PlannerContent.TrainCommitmentCarriedOverFromPreviousTrain,
                StatusColor = Brushes.LightPink
            });

        }
    }
}
