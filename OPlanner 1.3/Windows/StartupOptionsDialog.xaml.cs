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
    /// Interaction logic for StartupOptionsDialog.xaml
    /// </summary>
    public partial class StartupOptionsDialog : Window
    {
        public bool DialogCancelled { get; set; }
        public bool QuitSelected { get; set; }
        public bool ShouldUseClone { get; set; }
        public bool ShouldClearCurrentProductGroup { get; set; }
        public bool ShouldClearUserPreferences { get; set; }

        public StartupOptionsDialog()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            ShouldUseClone = false;
            ShouldClearCurrentProductGroup = false;
            ShouldClearUserPreferences = false;
            QuitSelected = false;

            UserPreferencesPathBox.Text = UserPreferences.GetUserPreferencesFullPath();
            TraceFilePathBox.Text = Tracer.GetTraceFilePath();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ShouldUseClone = UseCloneCheckBox.IsChecked == true;
            ShouldClearCurrentProductGroup = ClearProductGroupCheckBox.IsChecked == true;
            ShouldClearUserPreferences = ClearPreferencesCheckBox.IsChecked == true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogCancelled = true;
            Close();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            QuitSelected = true;
            Close();
        }
    }
}
