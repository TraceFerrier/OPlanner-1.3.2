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

namespace PlannerNameSpace.Windows
{
    /// <summary>
    /// Interaction logic for CommitChangesProgressDialog.xaml
    /// </summary>
    /// 

    public delegate void CancelRequestedEventHandler();

    public partial class BackgroundTaskProgressDialog : Window
    {
        public event CancelRequestedEventHandler CancelRequested;
        bool m_closeRequested;

        public BackgroundTaskProgressDialog()
        {
            InitializeComponent();

            m_closeRequested = false;

            if (Globals.MainWindow != null && Globals.MainWindow.IsVisible)
            {
                this.Owner = Globals.MainWindow;
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            }
            else
            {
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }

            Closing += BackgroundTaskProgressDialog_Closing;
        }

        void BackgroundTaskProgressDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!m_closeRequested)
            {
                e.Cancel = true;

                if (CancelRequested != null)
                {
                    CancelRequested();
                }
            }
        }

        public void CloseDialog()
        {
            m_closeRequested = true;
            Close();
        }

        public void SetProgressValue(int progress)
        {
            ProgressBarControl.Value = progress;
        }

        public bool IsCancelButtonEnabled
        {
            get { return CancelButton.IsEnabled; }

            set { CancelButton.IsEnabled = value; }
        }

        public bool IsIndeterminate
        {
            get { return ProgressBarControl.IsIndeterminate; }

            set { ProgressBarControl.IsIndeterminate = value; }
        }

        public void SetProgressDescription(string description)
        {
            ItemDescriptionBox.Text = description;
        }

        public void SetProgressMessage(string message)
        {
            ProgressMessageBox.Text = message;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (CancelRequested != null)
            {
                CancelRequested();
            }
        }
    }
}
