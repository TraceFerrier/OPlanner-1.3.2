using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Collections.Generic;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for ProductGroupEditor.xaml
    /// </summary>
    public partial class ProductGroupEditor : Window
    {
        public WelcomeState WelcomeState { get; set; }
        public bool AliasesChanged { get; set; }
        DispatcherTimer UpdateTimer { get; set; }
        Queue<bool> ResolutionQueue { get; set; }


        ProductGroupItem TargetProductGroupItem { get; set; }
        bool AliasesVerified { get; set; }
        string TestManagerAliasOriginal { get; set; }
        string DevManagerAliasOriginal { get; set; }
        string GPMAliasOriginal { get; set; }
        string TitleOriginal { get; set; }

        public ProductGroupEditor(Window ownerWindow, ProductGroupItem productGroupItem)
        {
            InitializeComponent();

            this.Owner = ownerWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            ResolutionQueue = new Queue<bool>();
            TargetProductGroupItem = productGroupItem;
            DialogContext.DataContext = TargetProductGroupItem;

            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;

            TestManagerAliasOriginal = productGroupItem.TestManagerAlias;
            DevManagerAliasOriginal = productGroupItem.DevManagerAlias;
            GPMAliasOriginal = productGroupItem.GroupPMAlias;
            TitleOriginal = productGroupItem.Title;

            UpdateUI();
            TitleBox.Focus();

            TitleBox.TextChanged += TitleBox_TextChanged;
            GPMAliasBox.TextChanged += GPMAliasBox_TextChanged;
            DevManagerAliasBox.TextChanged += DevManagerAliasBox_TextChanged;
            TestManagerAliasBox.TextChanged += TestManagerAliasBox_TextChanged;

            UpdateTimer = new DispatcherTimer();
            UpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Start();

            QueueAliasResolution();
        }

        void QueueAliasResolution()
        {
            ResolutionQueue.Enqueue(true);
        }

        void StartAliasResolution()
        {
            TestManagerAlias = TestManagerAliasBox.Text;
            DevManagerAlias = DevManagerAliasBox.Text;
            GPMAlias = GPMAliasBox.Text;

            AbortableBackgroundWorker worker = new AbortableBackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
        }

        string TestManagerAlias;
        string DevManagerAlias;
        string GPMAlias;

        string TestManagerDisplayName;
        string DevManagerDisplayName;
        string GPMDisplayName;

        string TestManagerTitle;
        string DevManagerTitle;
        string GPMTitle;

        BitmapSource TestManagerBitmap;
        BitmapSource DevManagerBitmap;
        BitmapSource GPMBitmap;

        bool ResolutionInProgress = false;
        void worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ResolutionInProgress = true;
            if (!string.IsNullOrWhiteSpace(TestManagerAlias))
            {
                TestManagerDisplayName = UserInformation.GetDisplayNameFromAlias(TestManagerAlias);
                TestManagerTitle = UserInformation.GetTitleFromAlias(TestManagerAlias);
                TestManagerBitmap = UserInformation.GetImageFromAlias(TestManagerAlias);
            }

            if (!string.IsNullOrWhiteSpace(DevManagerAlias))
            {
                DevManagerDisplayName = UserInformation.GetDisplayNameFromAlias(DevManagerAlias);
                DevManagerTitle = UserInformation.GetTitleFromAlias(DevManagerAlias);
                DevManagerBitmap = UserInformation.GetImageFromAlias(DevManagerAlias);
            }

            if (!string.IsNullOrWhiteSpace(GPMAlias))
            {
                GPMDisplayName = UserInformation.GetDisplayNameFromAlias(GPMAlias);
                GPMTitle = UserInformation.GetTitleFromAlias(GPMAlias);
                GPMBitmap = UserInformation.GetImageFromAlias(GPMAlias);
            }

            ResolutionInProgress = false;
        }

        void UpdateTimer_Tick(object sender, System.EventArgs e)
        {
            TestManagerDisplayNameBox.Text = TestManagerDisplayName;
            DevManagerDisplayNameBox.Text = DevManagerDisplayName;
            GPMDisplayNameBox.Text = GPMDisplayName;

            if (ResolutionQueue.Count > 0 && !ResolutionInProgress)
            {
                ResolutionQueue.Dequeue();
                StartAliasResolution();
            }

            UpdateUI();
        }

        void TitleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //UpdateUI();
        }

        void TestManagerAliasBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            QueueAliasResolution();
        }

        void DevManagerAliasBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            QueueAliasResolution();
        }

        void GPMAliasBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            QueueAliasResolution();
        }

        void UpdateUI()
        {
            AliasesVerified = true;
            if (string.IsNullOrWhiteSpace(TestManagerDisplayNameBox.Text))
            {
                TestManagerDisplayNameBox.Background = Brushes.LightPink;
                TestManagerImage.Source = Globals.ApplicationManager.GenericProfileBitmap;
                TestManagerTitleBox.Text = "";
                AliasesVerified = false;
            }
            else
            {
                TestManagerDisplayNameBox.Background = Brushes.WhiteSmoke;
                TestManagerImage.Source = TestManagerBitmap;
                TestManagerTitleBox.Text = TestManagerTitle;
            }

            if (string.IsNullOrWhiteSpace(DevManagerDisplayNameBox.Text))
            {
                DevManagerDisplayNameBox.Background = Brushes.LightPink;
                DevManagerImage.Source = Globals.ApplicationManager.GenericProfileBitmap;
                DevManagerTitleBox.Text = "";
                AliasesVerified = false;
            }
            else
            {
                DevManagerDisplayNameBox.Background = Brushes.WhiteSmoke;
                DevManagerImage.Source = DevManagerBitmap;
                DevManagerTitleBox.Text = DevManagerTitle;
            }

            if (string.IsNullOrWhiteSpace(GPMDisplayNameBox.Text))
            {
                GPMDisplayNameBox.Background = Brushes.LightPink;
                GPMImage.Source = Globals.ApplicationManager.GenericProfileBitmap;
                GPMTitleBox.Text = "";
                AliasesVerified = false;
            }
            else
            {
                GPMDisplayNameBox.Background = Brushes.WhiteSmoke;
                GPMImage.Source = GPMBitmap;
                GPMTitleBox.Text = GPMTitle;
            }

            OkButton.IsEnabled = AliasesVerified && (!string.IsNullOrWhiteSpace(TitleBox.Text));

        }

        void UndoChanges()
        {
            TargetProductGroupItem.TestManagerAlias = TestManagerAliasOriginal;
            TargetProductGroupItem.DevManagerAlias = DevManagerAliasOriginal;
            TargetProductGroupItem.GroupPMAlias = GPMAliasOriginal;
            TargetProductGroupItem.Title = TitleOriginal;
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UndoChanges();
            WelcomeState = PlannerNameSpace.WelcomeState.Cancel;
            Close();
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.StringsMatch(TargetProductGroupItem.TestManagerAlias, TestManagerAliasOriginal) &&
                Utils.StringsMatch(TargetProductGroupItem.DevManagerAlias, DevManagerAliasOriginal) &&
                Utils.StringsMatch(TargetProductGroupItem.GroupPMAlias, GPMAliasOriginal))
            {
                AliasesChanged = false;
            }
            else
            {
                AliasesChanged = true;
            }

            if (AliasesChanged && TargetProductGroupItem == ProductGroupManager.Instance.CurrentProductGroup && Globals.ApplicationManager.IsStartupComplete)
            {
                bool proceed = UserMessage.ShowOkCancel(this, "Since you're changing the list of people that are part of your product group, OPlanner needs to re-open your group again to retrieve the backlog and work items for these new people.  Click 'Cancel' if you'd prefer not to make this change at this time.", "Confirm Product Group Changes");
                if (!proceed)
                {
                    return;
                }
            }

            if (ValidateInfo())
            {
                WelcomeState = PlannerNameSpace.WelcomeState.Ok;
                Close();
            }
        }

        bool ValidateInfo()
        {
            string devAlias = TargetProductGroupItem.DevManagerAlias;
            string testAlias = TargetProductGroupItem.TestManagerAlias;
            string gpmAlias = TargetProductGroupItem.GroupPMAlias;

            if (Utils.StringsMatch(devAlias, testAlias) || Utils.StringsMatch(devAlias, gpmAlias) || Utils.StringsMatch(testAlias, gpmAlias))
            {
                UserMessage.Show("The people you select for 'Group PM Alias', 'Dev Manager Alias', and 'Test Manager Alias' must all be different.");
                return false;
            }

            bool isTitleUnique = true;
            if (TargetProductGroupItem.IsNew || !Utils.StringsMatch(TargetProductGroupItem.Title, TitleOriginal))
            {
                if (ProductGroupManager.Instance.GetProductGroupByTitle(TargetProductGroupItem.Title) != null)
                {
                    isTitleUnique = false;
                }
            }

            if (!isTitleUnique)
            {
                UserMessage.Show("The title you've given for this product group is the same as an existing group - please enter a different title.");
                return false;
            }

            return true;
        }

    }
}
