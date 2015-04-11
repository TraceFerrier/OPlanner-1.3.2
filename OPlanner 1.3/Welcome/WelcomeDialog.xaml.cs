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

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for WelcomeDialog.xaml
    /// </summary>
    public partial class WelcomeDialog : Window
    {
        WelcomeState WelcomeState { get; set; }
        public WelcomeDialog()
        {
            InitializeComponent();

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            WelcomeState = PlannerNameSpace.WelcomeState.Cancel;
            NextButton.Click += NextButton_Click;
            CancelButton.Click += CancelButton_Click;
            MoreInfo.Click += MoreInfo_Click;

        }

        void MoreInfo_Click(object sender, RoutedEventArgs e)
        {
            HelpManager.Instance.ShowHelpWindow(this, "Introduction to OPlanner");
        }

        public WelcomeState ShowWelcomeDialog()
        {
            this.ShowDialog();
            return WelcomeState;
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            WelcomeState = PlannerNameSpace.WelcomeState.Cancel;
            Close();
        }

        void NextButton_Click(object sender, RoutedEventArgs e)
        {
            WelcomeState = PlannerNameSpace.WelcomeState.Next;
            Close();
        }
    }
}
