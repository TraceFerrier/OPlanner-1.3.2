using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for BacklogItemEditorDialog.xaml
    /// </summary>
    public partial class BacklogItemEditorDialog : Window
    {
        BacklogItem TargetBacklogItem;
        bool AcceptanceCriteriaChanged = false;
        bool DescriptionChanged = false;
        ItemChangeHistoryCollection Changes;
        bool IsHistoryWaiting = false;
        AssignedToTextBoxHandler AssignedAliasTextBox;

        public static RoutedCommand CreateHyperlinkCommand = new RoutedCommand();

        public BacklogItemEditorDialog(BacklogItem backlogItem, int selectedTabIndex)
        {
            InitializeComponent();

            RichTextContext.CurrentItemEditorWindow = this;
            Globals.ApplicationManager.SetStartupLocation(this);

            TargetBacklogItem = backlogItem;
            DialogContext.DataContext = backlogItem;
            AssignedAliasTextBox = new PlannerNameSpace.AssignedToTextBoxHandler(AssignedToTextBox);

            AcceptanceCriteriaControl.ParentWindow = this;
            DescriptionControl.ParentWindow = this;

            PostMortemHelpBox.Text = PlannerContent.BacklogEditorPostMortemHelp;
            TargetBacklogItem.LoadDocumentIntoRichTextBox(Datastore.PropNameAcceptanceCriteriaDocument, null, AcceptanceCriteriaControl.PlannerRichTextBox);
            TargetBacklogItem.LoadDocumentIntoRichTextBox(Datastore.PropNameDescriptionDocument, null, DescriptionControl.PlannerRichTextBox);

            AcceptanceCriteriaControl.PlannerRichTextBox.Tag = new RichTextContext { StoreItem = TargetBacklogItem, HeaderText="Acceptance Criteria" };
            AcceptanceCriteriaControl.PlannerRichTextBox.TextChanged += AcceptanceCriteriaBox_TextChanged;

            DescriptionControl.PlannerRichTextBox.Tag = new RichTextContext { StoreItem = TargetBacklogItem, HeaderText = "Backlog Item Description" };
            DescriptionControl.PlannerRichTextBox.TextChanged += DescriptionCriteriaBox_TextChanged;

            OpenSpecButton.Click += OpenSpecButton_Click;
            EditLinkButton.Click += EditLinkButton_Click;

            OpenAttachedFileButton.Click += OpenAttachedFileButton_Click;
            AddAttachedFileButton.Click += AddAttachedFileButton_Click;
            RemoveAttachedFileButton.Click += RemoveAttachedFileButton_Click;

            AttachedFilesGrid.SelectionChanged += AttachedFilesGrid_SelectionChanged;

            Globals.EventManager.StoreCommitComplete += Instance_StoreCommitComplete;
            Globals.ItemManager.StoreItemChanged += ItemManager_StoreItemChanged;
            Globals.EventManager.UpdateUI += Instance_UpdateUI;
            BacklogTabControl.SelectedIndex = selectedTabIndex;

            Changes = null;
            ChangesGrid.SelectionChanged += ChangesGrid_SelectionChanged;
            BackgroundWorker historyWorker = new BackgroundWorker();
            historyWorker.DoWork += historyWorker_DoWork;
            historyWorker.RunWorkerCompleted += historyWorker_RunWorkerCompleted;
            historyWorker.RunWorkerAsync();

            UpdateUI();
        }

        void Instance_StoreCommitComplete(object sender, StoreCommitCompleteEventArgs e)
        {
            TargetBacklogItem.RefreshAttachedFileInfo();
            UpdateUI();
        }

        void ItemManager_StoreItemChanged(object sender, StoreItemChangedEventArgs e)
        {
            if (e.Change.ChangeType == ChangeType.Added || e.Change.ChangeType == ChangeType.Removed)
            {
                UpdateUI();
            }
        }

        private void OKButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (!AssignedAliasTextBox.IsValid())
            {
                UserMessage.Show(PlannerContent.BacklogValidationAssignedToAliasInvalid);
                return;
            }

            if (AcceptanceCriteriaChanged)
            {
                TargetBacklogItem.SaveDocumentFromRichTextBox(Datastore.PropNameAcceptanceCriteriaDocument, AcceptanceCriteriaControl.PlannerRichTextBox);
            }

            if (DescriptionChanged)
            {
                TargetBacklogItem.SaveDocumentFromRichTextBox(Datastore.PropNameDescriptionDocument, DescriptionControl.PlannerRichTextBox);
            }

            CloseDialog();
        }

        private void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            TargetBacklogItem.UndoChanges();
            CloseDialog();
        }

        private void CloseDialog()
        {
            Globals.EventManager.StoreCommitComplete -= Instance_StoreCommitComplete;
            Globals.ItemManager.StoreItemChanged -= ItemManager_StoreItemChanged;
            Globals.EventManager.UpdateUI -= Instance_UpdateUI;
            Close();
        }

        void Instance_UpdateUI()
        {
            if (IsHistoryWaiting)
            {
                IsHistoryWaiting = false;
                if (Changes != null)
                {
                    ChangesGrid.ItemsSource = Changes;
                    if (Changes.Count > 0)
                    {
                        ItemHistoryChange latestChange = Changes.GetItem(Changes.Count - 1);
                        ChangesGrid.SelectedItem = latestChange;
                        ChangesGrid.ScrollIntoView(latestChange);
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
            Changes = TargetBacklogItem.GetItemChangeHistory();
        }

        void ChangesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemHistoryChange change = ChangesGrid.SelectedValue as ItemHistoryChange;
            if (change != null)
            {
                ChangedFieldsGrid.ItemsSource = change.ChangedFields;
            }
        }

        void RemoveAttachedFileButton_Click(object sender, RoutedEventArgs e)
        {
            FileAttachment file = AttachedFilesGrid.SelectedItem as FileAttachment;
            if (file != null)
            {
                TargetBacklogItem.RemoveAttachedFileFromItem(this, file.FileName);
            }
        }

        void AttachedFilesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }

        void OpenAttachedFileButton_Click(object sender, RoutedEventArgs e)
        {
            FileAttachment file = AttachedFilesGrid.SelectedItem as FileAttachment;
            if (file != null)
            {
                TargetBacklogItem.Store.ShowFile(TargetBacklogItem, file.FileName);
            }
        }

        void AddAttachedFileButton_Click(object sender, RoutedEventArgs e)
        {
            IList filenames = Utils.SelectFiles();
            if (filenames != null && filenames.Count > 0)
            {
                TargetBacklogItem.AttachFilesToItem(filenames);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the dialog UI based on current conditions.
        /// </summary>
        //------------------------------------------------------------------------------------
        void UpdateUI()
        {
            OpenSpecButton.IsEnabled = string.IsNullOrWhiteSpace(TargetBacklogItem.SpecLink) ? false : true;
            OpenAttachedFileButton.IsEnabled = AttachedFilesGrid.SelectedItem == null ? false : true;
            RemoveAttachedFileButton.IsEnabled = OpenAttachedFileButton.IsEnabled;

            int attachedFileCount = TargetBacklogItem.AttachedFilesCount;
            AttachedFilesTabName.Text = "Attached Files (" + attachedFileCount.ToString() + ")";
        }

        void EditLinkButton_Click(object sender, RoutedEventArgs e)
        {
            EditSpecLink dialog = new EditSpecLink(TargetBacklogItem);
            dialog.ShowDialog();
            UpdateUI();
        }

        void OpenSpecButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TargetBacklogItem.SpecLink))
            {
                Process.Start(TargetBacklogItem.SpecLink);
            }
        }

        void DescriptionCriteriaBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DescriptionChanged = true;
        }

        void AcceptanceCriteriaBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AcceptanceCriteriaChanged = true;
        }
    }
}
