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

    /// <summary>
    /// Interaction logic for FeatureTeamWorkBarChart.xaml
    /// </summary>
    public partial class ScrumTeamWorkBarChart : UserControl
    {
        ScrumTeamItem CurrentScrumTeam = null;
        public ScrumTeamWorkBarChart()
        {
            InitializeComponent();
            BarChartListView.ItemTemplate = this.FindResource("MemberBar") as DataTemplate;
            Globals.EventManager.ScrumTeamViewTeamSelectionChanged += Instance_ScrumTeamViewTeamSelectionChanged;
        }

        void Instance_ScrumTeamViewTeamSelectionChanged(object sender, ScrumTeamChangedEventArgs e)
        {
            CurrentScrumTeam = e.CurrentItem;
            //BarChartListView.ItemsSource = CurrentScrumTeam.Members;
        }

    }
}
