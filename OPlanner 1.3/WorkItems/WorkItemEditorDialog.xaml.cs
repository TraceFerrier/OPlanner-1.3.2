using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlannerNameSpace.WorkItems
{
    /// <summary>
    /// Interaction logic for WorkItemEditorDialog.xaml
    /// </summary>
    public partial class WorkItemEditorDialog : Window
    {
        WorkItem TargetWorkItem;
        ItemChangeHistoryCollection Changes;
        bool IsHistoryWaiting = false;

        public WorkItemEditorDialog(WorkItem workItem)
        {
            InitializeComponent();

            Globals.ApplicationManager.SetStartupLocation(this);
            Globals.EventManager.UpdateUI += Handle_UpdateUI;

            TargetWorkItem = workItem;
            this.Closed += WorkItemEditorDialog_Closed;

            DialogContext.DataContext = TargetWorkItem;
            Changes = null;
            ChangesGrid.SelectionChanged += ChangesGrid_SelectionChanged;
            BackgroundWorker historyWorker = new BackgroundWorker();
            historyWorker.DoWork += historyWorker_DoWork;
            historyWorker.RunWorkerCompleted += historyWorker_RunWorkerCompleted;
            historyWorker.RunWorkerAsync();
        }

        void WorkItemEditorDialog_Closed(object sender, System.EventArgs e)
        {
        }

        void Handle_UpdateUI()
        {
            if (IsHistoryWaiting)
            {
                ChangesLabel.Content = "Changes:";
                IsHistoryWaiting = false;
                if (Changes != null)
                {
                    ChangesGrid.ItemsSource = Changes;
                    if (Changes.Count > 0)
                    {
                        ItemHistoryChange latestChange = Changes.GetItem(Changes.Count - 1);
                        if (latestChange != null)
                        {
                            ChangesGrid.SelectedItem = latestChange;
                            ChangesGrid.ScrollIntoView(latestChange);
                        }
                    }
                }
            }
        }

        void historyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsHistoryWaiting = true;
        }

        void historyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Changes = TargetWorkItem.GetItemChangeHistory();
        }

        void ChangesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemHistoryChange change = ChangesGrid.SelectedValue as ItemHistoryChange;
            if (change != null)
            {
                ChangedFieldsGrid.ItemsSource = change.ChangedFields;
            }
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TargetWorkItem.UndoChanges();
            Close();

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TargetWorkItem.RequestDeleteItem())
            {
                Close();
            }
        }
    }
}
