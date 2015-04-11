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
using System.Windows.Shapes;
using System.IO;
using System.Drawing.Imaging;

namespace PlannerNameSpace
{
    public enum ChartType
    {
        FeatureTeamChart,
        ProductGroupChart,
        PillarTrainChart,
    }

    public enum ChartStyle
    {
        Line_Chart,
        Area_Under_Curve,
    }

    /// <summary>
    /// Interaction logic for BurndownChart.xaml
    /// </summary>
    public partial class BurndownChart : Window
    {

        ProductGroupItem CurrentProductGroup { get; set; }
        PillarItem CurrentPillar { get; set; }
        TrainItem CurrentTrain { get; set; }
        string ThisChartName { get; set; }
        ChartType ChartType { get; set; }
        ChartStyle CurrentChartStyle { get; set; }

        const double AxisThickness = 2;
        const double XLegendHeight = 120;
        const double YLegendWidth = 60;
        const double ChartMargin = 10;
        const int YGridlineCount = 20;
        const double YLegendOffsetFromAxis = 50;
        const double YLabelOffsetFromAxis = 30;
        const double LineChartPointSize = 6;

        static Brush RemainingColor = Brushes.Blue;
        static Brush CompletedColor = Brushes.Green;
        static Brush EstimateColor = Brushes.Orange;

        Dictionary<string, WorkSummary> BurndownWorkSummary;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Draws the current burndown chart for the given feature team.
        /// </summary>
        //------------------------------------------------------------------------------------
        public BurndownChart(ScrumTeamItem featureTeamItem)
        {
            ChartType = PlannerNameSpace.ChartType.FeatureTeamChart;
            CurrentPillar = featureTeamItem.ParentPillarItem;
            CurrentTrain = null;//featureTeamItem.ParentTrainItem;
            CommonInitialize();

            TrainNameBox.Visibility = System.Windows.Visibility.Visible;
            PillarComboPanel.Visibility = System.Windows.Visibility.Collapsed;
            TrainComboPanel.Visibility = System.Windows.Visibility.Collapsed;

            ThisChartName = "Feature Team Name: " + featureTeamItem.Title;

            BurndownWorkSummary = Globals.ReportingManager.GetFeatureTeamWorkSummary(featureTeamItem);
            if (BurndownWorkSummary != null)
            {
                RenderChart(BurndownWorkSummary);
            }

        }

        private void CopyToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            CopyChartToClipboard(MainGrid, ChartHeight, ChartWidth);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a burndown chart for the current product and train.
        /// </summary>
        //------------------------------------------------------------------------------------
        public BurndownChart(ProductGroupItem productGroup)
        {
            CurrentProductGroup = productGroup;
            CurrentPillar = StoreItem.GetDummyItem<PillarItem>(ItemTypeID.Pillar, DummyItemType.AllType);
            CurrentTrain = Globals.GlobalItemCache.TrainItems.CurrentTrain;

            CommonInitialize();

            TrainNameBox.Visibility = System.Windows.Visibility.Collapsed;
            PillarComboPanel.Visibility = System.Windows.Visibility.Visible;
            TrainComboPanel.Visibility = System.Windows.Visibility.Visible;

            RenderProductChart(Globals.GlobalItemCache.TrainItems.CurrentTrain);
        }

        public void RenderProductChart(TrainItem trainItem)
        {
            ChartType = PlannerNameSpace.ChartType.ProductGroupChart;
            ThisChartName = "Product Group: " + CurrentProductGroup.Title;
            BurndownWorkSummary = Globals.ReportingManager.GetCurrentProductGroupWorkSummary(trainItem);
            if (BurndownWorkSummary != null)
            {
                RenderChart(BurndownWorkSummary);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Renders a burndown chart based on the specified pillar and train.
        /// </summary>
        //------------------------------------------------------------------------------------
        public BurndownChart(PillarItem pillarItem, TrainItem trainItem)
        {
            ChartType = PlannerNameSpace.ChartType.PillarTrainChart;

            CurrentPillar = pillarItem;

            if (trainItem == null)
            {
                trainItem = Globals.GlobalItemCache.TrainItems.CurrentTrain;
            }

            CurrentTrain = trainItem;

            CommonInitialize();

            TrainNameBox.Visibility = System.Windows.Visibility.Collapsed;
            PillarComboPanel.Visibility = System.Windows.Visibility.Visible;
            TrainComboPanel.Visibility = System.Windows.Visibility.Visible;

            RenderPillarTrainChart(pillarItem, trainItem);
        }

        void RenderPillarTrainChart(PillarItem pillarItem, TrainItem trainItem)
        {
            if (CurrentPillar.IsDummyItem)
            {
                RenderProductChart(CurrentTrain);
            }
            else
            {
                ThisChartName = "Pillar: " + pillarItem.Title;
                BurndownWorkSummary = Globals.ReportingManager.GetPillarWorkSummary(pillarItem, trainItem);
                if (BurndownWorkSummary != null)
                {
                    RenderChart(BurndownWorkSummary);
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Common intialization for all chart types.
        /// </summary>
        //------------------------------------------------------------------------------------
        void CommonInitialize()
        {
            InitializeComponent();
            Utils.FitWindowToScreen(this);

            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            CurrentChartStyle = Utils.StringToEnum<ChartStyle>(Globals.UserPreferences.GetProductPreference(ProductPreferences.LastSelectedBurndownChartStyle));
            CurrentProductGroup = Globals.ApplicationManager.CurrentProductGroup;
            PillarCombo.ItemsSource = Globals.GlobalItemCache.PillarItems.GetItems(DummyItemType.AllType, ItemTypeID.Pillar);
            TrainCombo.ItemsSource = Globals.GlobalItemCache.TrainItems.GetTrains(TrainTimeFrame.CurrentOrPast);
            ChartStyleCombo.ItemsSource = Utils.GetEnumValues<ChartStyle>();
            ChartStyleCombo.SelectedValue = Utils.EnumToString<ChartStyle>(CurrentChartStyle);
            

            PillarCombo.SelectedItem = CurrentPillar == null ? StoreItem.GetDummyItem<PillarItem>(ItemTypeID.Pillar, DummyItemType.AllType) : CurrentPillar;
            TrainCombo.SelectedItem = CurrentTrain;

            PillarCombo.SelectionChanged += PillarCombo_SelectionChanged;
            TrainCombo.SelectionChanged += TrainCombo_SelectionChanged;
            ChartStyleCombo.SelectionChanged += ChartStyleCombo_SelectionChanged;

            this.Loaded += BurndownChart_Loaded;
        }

        void ChartStyleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string styleSelected = ChartStyleCombo.SelectedValue as string;
            CurrentChartStyle = Utils.StringToEnum<ChartStyle>(styleSelected);
            Globals.UserPreferences.SetProductPreference(ProductPreferences.LastSelectedBurndownChartStyle, styleSelected);

            if (BurndownWorkSummary != null)
            {
                RenderChart(BurndownWorkSummary);
            }
        }

        void BurndownChart_Loaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged += BurndownChart_SizeChanged;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has changed the selection in the Train combo box.
        /// </summary>
        //------------------------------------------------------------------------------------
        void TrainCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentTrain = TrainCombo.SelectedItem as TrainItem;
            RenderPillarTrainChart(CurrentPillar, CurrentTrain);

            Globals.UserPreferences.SetItemSelectionPreference(ProductPreferences.LastSelectedBurndownTrainItem, CurrentTrain);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has changed the selection in the Pillar combo box.
        /// </summary>
        //------------------------------------------------------------------------------------
        void PillarCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentPillar = PillarCombo.SelectedItem as PillarItem;
            RenderPillarTrainChart(CurrentPillar, CurrentTrain);

            Globals.UserPreferences.SetItemSelectionPreference(ProductPreferences.LastSelectedBurndownPillarItem, CurrentPillar);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The user has changed the size of the window hosting the burndown chart - in 
        /// response, we'll re-render the chart to fit the new size.
        /// </summary>
        //------------------------------------------------------------------------------------
        void BurndownChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (BurndownWorkSummary != null)
            {
                RenderChart(BurndownWorkSummary);
            }
        }

        double GetSmallestYCoord(List<double> coords)
        {
            double smallest = double.MaxValue;
            foreach (double coord in coords)
            {
                if (coord < smallest)
                {
                    smallest = coord;
                }
            }

            return smallest;
        }

        double ChartHeight { get; set; }
        double ChartWidth { get; set; }

        double WindowWidth
        {
            get
            {
                double width = this.ActualWidth;
                if (width <= 0)
                {
                    width = this.Width;
                }

                return width;
            }
        }

        double WindowHeight
        {
            get
            {
                double height = this.ActualHeight;
                if (height <= 0)
                {
                    height = this.Height;
                }

                return height;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Renders the burndown chart, based on the given directory of work summary data
        /// points.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void RenderChart(Dictionary<string, WorkSummary> workSummary)
        {
            ChartNameBox.Text = ThisChartName;
            TrainNameBox.Text = "Train: " + CurrentTrain.Title;
            bool success = false;

            switch (CurrentChartStyle)
            {
                case ChartStyle.Area_Under_Curve:
                    success = RenderAreaUnderCurveChart(workSummary);
                    break;
                case ChartStyle.Line_Chart:
                    success = RenderLineChart(workSummary);
                    break;
            }

            if (!success)
            {
                UserMessage.Show("Valid burndown data not available for the selected pillar or train.");
            }
        }

        double YGridSpacing
        {
            get { return ChartHeight / YGridlineCount; }
        }

        double XGridSpacing
        {
            get { return ChartWidth / XGridlineCount; }
        }

        double XGridlineCount
        {
            get
            {
                DateTime startDate = CurrentTrain.StartDate;
                DateTime endDate = CurrentTrain.EndDate;
                return Utils.GetNetWorkingDays(startDate, endDate);
            }
        }

        double GetMaxWorkRemaining(Dictionary<string, WorkSummary> workSummary)
        {
            DateTime startDate = CurrentTrain.StartDate;
            DateTime endDate = CurrentTrain.EndDate;
            return Globals.ReportingManager.GetMaxWorkRemaining(workSummary, startDate, endDate);
        }

        double GetMaxWorkEstimate(Dictionary<string, WorkSummary> workSummary)
        {
            DateTime startDate = CurrentTrain.StartDate;
            DateTime endDate = CurrentTrain.EndDate;
            return Globals.ReportingManager.GetMaxWorkEstimate(workSummary, startDate, endDate);
        }

        double MaxChartWorkRemainingValue(Dictionary<string, WorkSummary> workSummary)
        {
            return Math.Ceiling(GetMaxWorkRemaining(workSummary) / 10) * 10;
        }

        double MaxLineChartWorkValue(Dictionary<string, WorkSummary> workSummary)
        {
            return Math.Ceiling(GetMaxWorkEstimate(workSummary) / 10) * 10;
        }

        Point GetChartCoords(double originX, double originY, double workValue, int workDay, double maxChartWorkValue)
        {
            double pixelsPerWorkValue = ChartHeight / maxChartWorkValue;
            double yCoord = originY - (workValue * pixelsPerWorkValue);
            double xCoord = originX + workDay * XGridSpacing;
            return new Point(xCoord, yCoord);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Renders the frame, axes, gridlines, and legends for the chart with the given
        /// parameters.
        /// </summary>
        //------------------------------------------------------------------------------------
        bool DoChartSetup(Dictionary<string, WorkSummary> workSummary, out double originX, out double originY, out double maxChartWorkValue, out double maxWorkRemaining)
        {
            originX = YLegendWidth;
            originY = WindowHeight - XLegendHeight - Globals.BurndownHeadingHeight.Value;
            ChartHeight = originY - ChartMargin;
            ChartWidth = WindowWidth - originX - ChartMargin;

            BurndownCanvas.Children.Clear();

            maxChartWorkValue = MaxLineChartWorkValue(workSummary);
            maxWorkRemaining = MaxChartWorkRemainingValue(workSummary);
            if (maxChartWorkValue > 0 && maxWorkRemaining > 0)
            {
                DrawLineChartAxes(originX, originY);
                DrawGridlines(originX, originY, maxChartWorkValue, XGridlineCount, YGridlineCount);
                DrawLabels(originX, originY, maxChartWorkValue);
                DrawAxisLegends(originX, originY);
                return true;
            }
            else
            {
                return false;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Renders the burndown chart, based on the given directory of work summary data
        /// points.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool RenderLineChart(Dictionary<string, WorkSummary> workSummary)
        {
            double originX;
            double originY;
            double maxChartWorkValue;
            double maxWorkRemaining;
            if (!DoChartSetup(workSummary, out originX, out originY, out maxChartWorkValue, out maxWorkRemaining))
            {
                return false;
            }

            DateTime startDate = CurrentTrain.StartDate;
            DateTime endDate = CurrentTrain.EndDate;
            DateTime currentDate = startDate;
            int workDay = 0;
            Point lastRemainingChartPoint = new Point(0, 0);
            Point lastCompletedChartPoint = new Point(0, 0);
            Point lastEstimateChartPoint = new Point(0, 0);
            while (currentDate <= DateTime.Today && currentDate <= endDate)
            {
                if (Utils.IsWorkDay(currentDate))
                {
                    if (workSummary.ContainsKey(currentDate.ToShortDateString()))
                    {
                        WorkSummary dateSummary = workSummary[currentDate.ToShortDateString()];
                        int workRemaining = dateSummary.WorkRemaining;
                        int workCompleted = dateSummary.WorkCompleted;
                        int workEstimate = workCompleted + workRemaining;

                        Point remainingChartPoint = GetChartCoords(originX, originY, workRemaining, workDay, maxChartWorkValue);
                        Point completedChartPoint = GetChartCoords(originX, originY, workCompleted, workDay, maxChartWorkValue);
                        Point estimateChartPoint = GetChartCoords(originX, originY, workEstimate, workDay, maxChartWorkValue);

                        Ellipse remainingCircle = GetLineChartDataPoint(remainingChartPoint, RemainingColor);
                        Ellipse completedCircle = GetLineChartDataPoint(completedChartPoint, CompletedColor);
                        Ellipse estimateCircle = GetLineChartDataPoint(estimateChartPoint, EstimateColor);

                        if (lastRemainingChartPoint.X > 0 && lastRemainingChartPoint.Y > 0)
                        {
                            Line remainingLine = GetLineChartLine(lastRemainingChartPoint, remainingChartPoint, RemainingColor);
                            BurndownCanvas.Children.Add(remainingLine);

                            Line completedLine = GetLineChartLine(lastCompletedChartPoint, completedChartPoint, CompletedColor);
                            BurndownCanvas.Children.Add(completedLine);

                            Line estimateLine = GetLineChartLine(lastEstimateChartPoint, estimateChartPoint, EstimateColor);
                            BurndownCanvas.Children.Add(estimateLine);


                        }

                        BurndownCanvas.Children.Add(remainingCircle);
                        BurndownCanvas.Children.Add(completedCircle);
                        BurndownCanvas.Children.Add(estimateCircle);

                        lastRemainingChartPoint = remainingChartPoint;
                        lastCompletedChartPoint = completedChartPoint;
                        lastEstimateChartPoint = estimateChartPoint;
                    }

                    workDay++;
                }

                currentDate = currentDate.AddDays(1);
            }

            DrawBurndownLine(BurndownCanvas, originX, originY, maxWorkRemaining, maxChartWorkValue);

            return true;
        }

        public bool RenderAreaUnderCurveChart(Dictionary<string, WorkSummary> workSummary)
        {
            double originX;
            double originY;
            double maxChartWorkValueX;
            double maxWorkRemaining;
            if (!DoChartSetup(workSummary, out originX, out originY, out maxChartWorkValueX, out maxWorkRemaining))
            {
                return false;
            }

            DateTime startDate = CurrentTrain.StartDate;
            DateTime endDate = CurrentTrain.EndDate;
            DateTime currentDate = startDate;
            int workDay = 0;

            Polygon poly = new Polygon();
            poly.Stroke = Brushes.Black;
            poly.Fill = Brushes.LightSteelBlue;
            poly.StrokeThickness = 2;
            PointCollection points = new PointCollection();

            // The first point is at the x,y origin
            Point originPoint = new Point() { X = originX, Y = originY };
            points.Add(originPoint);

            Point finalPoint = new Point();
            while (currentDate <= DateTime.Today && currentDate <= endDate)
            {
                if (Utils.IsWorkDay(currentDate))
                {
                    if (workSummary.ContainsKey(currentDate.ToShortDateString()))
                    {
                        WorkSummary dateSummary = workSummary[currentDate.ToShortDateString()];
                        int workRemaining = dateSummary.WorkRemaining;
                        Point chartPoint = GetChartCoords(originX, originY, workRemaining, workDay, maxWorkRemaining);
                        points.Add(chartPoint);
                        finalPoint = chartPoint;
                    }

                    workDay++;
                }

                currentDate = currentDate.AddDays(1);
            }

            if (finalPoint.X != 0)
            {
                finalPoint.Y = originY;
                points.Add(finalPoint);
            }

            poly.Points = points;
            BurndownCanvas.Children.Add(poly);
            DrawBurndownLine(BurndownCanvas, originX, originY, maxWorkRemaining, maxWorkRemaining);

            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Renders the burndown chart, based on the given directory of work summary data
        /// points.  This chart style will be the 'area under the curve' variety.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void RenderAreaUnderCurveChart2(Dictionary<string, WorkSummary> workSummary)
        {
            BurndownCanvas.Children.Clear();

            double axisThickness = 2;
            double originX = 60;
            double originY = WindowHeight - 200;
            double lengthXAxis = WindowWidth - 110;
            double lengthYAxis = originY - 20;

            ChartHeight = (int)WindowHeight - 60;
            ChartWidth = (int)WindowWidth - 50;

            // Y-Axis
            Line yAxis = new Line();
            yAxis.Stroke = Brushes.SteelBlue;
            yAxis.X1 = originX;
            yAxis.X2 = originX;
            yAxis.Y1 = originY - lengthYAxis;
            yAxis.Y2 = originY;
            yAxis.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            yAxis.StrokeThickness = axisThickness;
            BurndownCanvas.Children.Add(yAxis);

            // Y-Axis legend
            TextBlock yLegend = new TextBlock()
            {
                Text = "Work Remaining",
                FontFamily = new FontFamily("Seqoe UI"),
                FontSize = 12,
                RenderTransformOrigin = new Point(0.5, 0.5),
                LayoutTransform = new RotateTransform(-90)
            };

            Canvas.SetLeft(yLegend, originX - YLegendOffsetFromAxis);
            Canvas.SetTop(yLegend, lengthYAxis / 2);
            BurndownCanvas.Children.Add(yLegend);


            DateTime startDate = CurrentTrain.StartDate;
            DateTime endDate = CurrentTrain.EndDate;

            double xPosition = originX;

            // Draw Y-Axis labels
            List<double> WorkRemainingYCoords = new List<double>();
            int maxWorkRemaining = Globals.ReportingManager.GetMaxWorkRemaining(workSummary, startDate, endDate);
            double ySpacing = 25;
            double yAxisTicks = (int)(lengthYAxis / ySpacing);
            double currentY = originY - ySpacing;
            int workRemainingIterations = (int)Math.Round(maxWorkRemaining / yAxisTicks, MidpointRounding.AwayFromZero);
            int currentWork = 0;
            for (int x = 0; x < yAxisTicks; x++)
            {
                TextBlock yLabel = new TextBlock()
                {
                    Text = currentWork.ToString(),
                    FontFamily = new FontFamily("Seqoe UI"),
                    FontSize = 12,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                };

                WorkRemainingYCoords.Add(currentY);
                Canvas.SetLeft(yLabel, originX - YLabelOffsetFromAxis);
                Canvas.SetTop(yLabel, currentY);
                BurndownCanvas.Children.Add(yLabel);

                currentWork += workRemainingIterations;
                currentY -= ySpacing;
            }

            Dictionary<string, double> DateXCoords = new Dictionary<string, double>();

            // Draw X-Axis dates and gridlines.
            int netWorkingDays = Utils.GetNetWorkingDays(startDate, endDate);
            int spacingX = (int)lengthXAxis / netWorkingDays;

            double smallestYCoord = GetSmallestYCoord(WorkRemainingYCoords);

            DateTime lastDate = new DateTime();
            DateTime currentDate = startDate;
            while (currentDate <= endDate)
            {
                if (Utils.IsWorkDay(currentDate))
                {
                    TextBlock dateText = new TextBlock()
                    {
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 12,
                        TextAlignment = TextAlignment.Center,
                        RenderTransformOrigin = new Point(0.5, 0.5),
                        LayoutTransform = new RotateTransform(60)
                    };

                    dateText.Text = currentDate.ToShortDateString();
                    Canvas.SetLeft(dateText, xPosition);
                    Canvas.SetTop(dateText, originY);
                    BurndownCanvas.Children.Add(dateText);

                    Line gridLine = new Line()
                    {
                        StrokeThickness = 1,
                        Stroke = Brushes.LightGray,
                        Y1 = smallestYCoord,
                        Y2 = originY - axisThickness,
                        X1 = xPosition + spacingX / 2,
                        X2 = xPosition + spacingX / 2,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    };
                    BurndownCanvas.Children.Add(gridLine);
                    DateXCoords.Add(dateText.Text, gridLine.X1);

                    xPosition += spacingX;
                    lastDate = currentDate;
                }

                currentDate = currentDate.AddDays(1);
            }

            foreach (double yCoord in WorkRemainingYCoords)
            {
                Line gridLine = new Line()
                {
                    StrokeThickness = 1,
                    Stroke = Brushes.LightGray,
                    Y1 = yCoord,
                    Y2 = yCoord,
                    X1 = originX,
                    X2 = DateXCoords[lastDate.ToShortDateString()]
                };
                BurndownCanvas.Children.Add(gridLine);
            }

            // Draw the burndown polygon curve
            currentDate = startDate;
            Polygon poly = new Polygon();
            poly.Stroke = Brushes.Black;
            poly.Fill = Brushes.LightSteelBlue;
            poly.StrokeThickness = 2;
            PointCollection points = new PointCollection();

            // The first point is at the x,y origin
            Point originPoint = new Point() { X = originX, Y = originY };
            points.Add(originPoint);

            DateTime curveLastDate = new DateTime();
            while (currentDate <= DateTime.Today && currentDate <= endDate)
            {
                if (Utils.IsWorkDay(currentDate))
                {
                    if (workSummary.ContainsKey(currentDate.ToShortDateString()))
                    {
                        WorkSummary dateSummary = workSummary[currentDate.ToShortDateString()];
                        int workRemaining = dateSummary.WorkRemaining;
                        Point datePoint = new Point();
                        datePoint.X = DateXCoords[currentDate.ToShortDateString()];
                        double pctWorkRemaining = (double)workRemaining / (double)maxWorkRemaining;
                        datePoint.Y = originY - (lengthYAxis * pctWorkRemaining);
                        points.Add(datePoint);
                        curveLastDate = currentDate;
                    }
                }

                currentDate = currentDate.AddDays(1);
            }

            Point finalPoint = new Point();
            finalPoint.X = DateXCoords[curveLastDate.ToShortDateString()];
            finalPoint.Y = originY;
            points.Add(finalPoint);

            poly.Points = points;
            BurndownCanvas.Children.Add(poly);

            // X-Axis
            Line hAxis = new Line();
            hAxis.Stroke = Brushes.SteelBlue;
            hAxis.X1 = originX;
            hAxis.X2 = DateXCoords[lastDate.ToShortDateString()];
            hAxis.Y1 = originY;
            hAxis.Y2 = originY;
            hAxis.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            hAxis.StrokeThickness = axisThickness;
            BurndownCanvas.Children.Add(hAxis);

            // Draw burndown line
            if (lastDate.Year != 0)
            {
                Line burndownLine = new Line();
                burndownLine.StrokeThickness = 2;
                burndownLine.Stroke = Brushes.Black;
                burndownLine.Y1 = originY - lengthYAxis;
                burndownLine.Y2 = originY;
                burndownLine.X1 = originX;
                burndownLine.X2 = DateXCoords[lastDate.ToShortDateString()];
                BurndownCanvas.Children.Add(burndownLine);
            }
        }

        void DrawBurndownLine(Canvas burndownCanvas, double originX, double originY, double maxWorkRemaining, double maxChartWorkValue)
        {
            Line burndownLine = new Line();
            burndownLine.StrokeThickness = 2;
            burndownLine.Stroke = Brushes.Black;

            Point xPoint = GetChartCoords(originX, originY, maxWorkRemaining, 0, maxChartWorkValue);
            Point yPoint = GetChartCoords(originX, originY, 0, (int)(XGridlineCount - 1), maxChartWorkValue);
            burndownLine.Y1 = xPoint.Y;
            burndownLine.Y2 = yPoint.Y;
            burndownLine.X1 = xPoint.X;
            burndownLine.X2 = yPoint.X;
            burndownCanvas.Children.Add(burndownLine);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a small filled circle that can represent a data point on a line chart.
        /// </summary>
        //------------------------------------------------------------------------------------
        Ellipse GetLineChartDataPoint(Point chartPoint, Brush color)
        {
            Ellipse point = new Ellipse();
            point.Fill = color;
            point.StrokeThickness = 1;
            point.Stroke = color;
            point.Width = LineChartPointSize;
            point.Height = LineChartPointSize;

            Canvas.SetLeft(point, chartPoint.X - LineChartPointSize / 2);
            Canvas.SetTop(point, chartPoint.Y);
            return point;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a line segment connecting the given two points.
        /// </summary>
        //------------------------------------------------------------------------------------
        Line GetLineChartLine(Point lastPoint, Point currentPoint, Brush color)
        {
            Line connectorLine = new Line()
            {
                StrokeThickness = 2,
                Stroke = color,
                X1 = lastPoint.X,
                X2 = currentPoint.X,
                Y1 = lastPoint.Y + LineChartPointSize / 2,
                Y2 = currentPoint.Y + LineChartPointSize / 2,
            };

            return connectorLine;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Draws the x and y axis labels for the current burndown chart.
        /// </summary>
        //------------------------------------------------------------------------------------
        void DrawLabels(double originX, double originY, double maxChartWorkValue)
        {
            double currentWork = 0;
            double fontSize = 12;
            double currentY = originY - fontSize;
            double workRemainingIterations = maxChartWorkValue / YGridlineCount;
            for (int x = 0; x < YGridlineCount; x++)
            {
                int currentIntWork = (int)Math.Round(currentWork, MidpointRounding.AwayFromZero);
                TextBlock yLabel = new TextBlock()
                {
                    Text = currentIntWork.ToString(),
                    FontFamily = new FontFamily("Seqoe UI"),
                    FontSize = fontSize,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                };

                Canvas.SetLeft(yLabel, originX - YLabelOffsetFromAxis);
                Canvas.SetTop(yLabel, currentY);
                BurndownCanvas.Children.Add(yLabel);

                currentWork += workRemainingIterations;
                currentY -= YGridSpacing;
            }

            DateTime startDate = CurrentTrain.StartDate;
            DateTime endDate = CurrentTrain.EndDate;
            int netWorkingDays = Utils.GetNetWorkingDays(startDate, endDate);

            DateTime lastDate = new DateTime();
            DateTime currentDate = startDate;
            double currentX = originX - fontSize;
            while (currentDate <= endDate)
            {
                if (Utils.IsWorkDay(currentDate))
                {
                    TextBlock dateText = new TextBlock()
                    {
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = fontSize,
                        TextAlignment = TextAlignment.Center,
                        RenderTransformOrigin = new Point(0.5, 0.5),
                        LayoutTransform = new RotateTransform(60)
                    };

                    dateText.Text = currentDate.ToShortDateString();
                    Canvas.SetLeft(dateText, currentX);
                    Canvas.SetTop(dateText, originY);
                    BurndownCanvas.Children.Add(dateText);

                    currentX += XGridSpacing;
                    lastDate = currentDate;
                }

                currentDate = currentDate.AddDays(1);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Draws the x and y gridlines for the current burndown chart.
        /// </summary>
        //------------------------------------------------------------------------------------
        void DrawGridlines(double originX, double originY, double maxChartWorkValue, double xGridlineCount, double yGridlineCount)
        {
            double workValue = 0;
            for (int y = 0; y < YGridlineCount; y++)
            {
                Point chartPoint = GetChartCoords(originX, originY, workValue, 0, maxChartWorkValue);
                Line gridLine = new Line()
                {
                    StrokeThickness = 1,
                    Stroke = Brushes.LightGray,
                    Y1 = chartPoint.Y,
                    Y2 = chartPoint.Y,
                    X1 = originX,
                    X2 = originX + ChartWidth,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                BurndownCanvas.Children.Add(gridLine);
                workValue += maxChartWorkValue / YGridlineCount;
            }

            for (int x = 0; x < xGridlineCount; x++)
            {
                Line gridLine = new Line()
                {
                    StrokeThickness = 1,
                    Stroke = Brushes.LightGray,
                    Y1 = originY,
                    Y2 = originY - ChartHeight,
                    X1 = originX + XGridSpacing * x,
                    X2 = originX + XGridSpacing * x,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                BurndownCanvas.Children.Add(gridLine);
            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Draws the x and y axis lines for the current burndown chart.
        /// </summary>
        //------------------------------------------------------------------------------------
        void DrawLineChartAxes(double originX, double originY)
        {
            Line yAxis = new Line();
            yAxis.Stroke = Brushes.SteelBlue;
            yAxis.X1 = originX;
            yAxis.X2 = originX;
            yAxis.Y1 = originY - ChartHeight;
            yAxis.Y2 = originY;
            yAxis.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            yAxis.StrokeThickness = AxisThickness;
            BurndownCanvas.Children.Add(yAxis);

            // X-Axis
            Line hAxis = new Line();
            hAxis.Stroke = Brushes.SteelBlue;
            hAxis.X1 = originX;
            hAxis.X2 = originX + ChartWidth;
            hAxis.Y1 = originY;
            hAxis.Y2 = originY;
            hAxis.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            hAxis.StrokeThickness = AxisThickness;
            BurndownCanvas.Children.Add(hAxis);
        }

        void DrawAxisLegends(double originX, double originY)
        {
            // Y-Axis legend
            TextBlock yLegend = new TextBlock()
            {
                Text = "Work Remaining",
                FontFamily = new FontFamily("Seqoe UI"),
                FontSize = 12,
                RenderTransformOrigin = new Point(0.5, 0.5),
                LayoutTransform = new RotateTransform(-90)
            };

            Canvas.SetLeft(yLegend, originX - YLegendOffsetFromAxis);
            Canvas.SetTop(yLegend, ChartHeight / 2);
            BurndownCanvas.Children.Add(yLegend);

            double xPosition = WindowWidth / 2 - 600 / 2;
            DrawYAxisLegend("Work Remaining", RemainingColor, originY, xPosition);
            xPosition += 200;

            DrawYAxisLegend("Completed", CompletedColor, originY, xPosition);
            xPosition += 200;

            DrawYAxisLegend("Estimate", EstimateColor, originY, xPosition);
        }

        void DrawYAxisLegend(string legendText, Brush color, double originY, double xPosition)
        {
            const double LegendLineLength = 40;
            double yPosition = originY + 60;
            const int fontSize = 12;
            const double marginBetweenLineAndText = 10;

            Line legendLine = new Line()
            {
                StrokeThickness = 2,
                Stroke = color,
                X1 = xPosition,
                X2 = xPosition + LegendLineLength,
                Y1 = yPosition + fontSize / 2,
                Y2 = yPosition + fontSize / 2,
            };

            BurndownCanvas.Children.Add(legendLine);
            xPosition += LegendLineLength + marginBetweenLineAndText;

            TextBlock xLegend = new TextBlock()
            {
                Text = legendText,
                FontFamily = new FontFamily("Seqoe UI"),
                FontSize = fontSize,
                Foreground = Brushes.Black,
            };
            Canvas.SetLeft(xLegend, xPosition);
            Canvas.SetTop(xLegend, yPosition);
            BurndownCanvas.Children.Add(xLegend);
        }

        private void CopyChartToClipboard(Grid canvas, double height, double width)
        {
            //FileStream fs = new FileStream(path, FileMode.Create);
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)width, (int)height, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);

            CopyToClipboardButton.Visibility = System.Windows.Visibility.Hidden;
            bmp.Render(canvas);
            CopyToClipboardButton.Visibility = System.Windows.Visibility.Visible;

            //BitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bmp));

            //BitmapImage bitmap = new BitmapImage();
            //MemoryStream stream = new MemoryStream();
            //encoder.Save(stream);
            //bitmap.BeginInit();
            //bitmap.StreamSource = new MemoryStream(stream.ToArray());
            //bitmap.EndInit();
            //Clipboard.SetData(DataFormats.Bitmap, bmp);
            Clipboard.SetImage(bmp);

            //encoder.Save(fs);
            //fs.Close();
        }

    }

}
