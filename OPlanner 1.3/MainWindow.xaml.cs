using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Controls.Ribbon;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        bool FirstOpened;
        public const string DefaultTabName = "ProductGroupTab";

        public MainWindow()
        {
            // Add resources for all classes that we're binding to in MainWindow xaml.
            Application.Current.Resources.Add("PillarManager", PillarManager.Instance);
            Application.Current.Resources.Add("TrainManager", TrainManager.Instance);
            Application.Current.Resources.Add("ProductGroupManager", ProductGroupManager.Instance);

            InitializeComponent();
            Utils.FitWindowToScreen(this);
            this.Title = ApplicationManager.AssemblyProduct;

            Closing += MainWindow_Closing;
            FirstOpened = true;

        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Globals.ItemManager != null && Globals.ItemManager.ChangeCount > 0)
            {
                if (!UserMessage.ShowOkCancel(this, "Are you sure you want to quit without saving changes?", ApplicationManager.AssemblyProduct))
                {
                    e.Cancel = true;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Switches the ribbon to the tab associated with the given layoutMode.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SwitchToTab(RibbonTab tab)
        {
            if ((RibbonTab)Ribbon.SelectedItem != tab)
            {
                Ribbon.SelectedValue = tab;
            }
        }

        public void InitializeTabs()
        {
            if (FirstOpened)
            {
                    RibbonTab lastSelectedTab = GetTabFromGlobalPreferences();
                    if (lastSelectedTab != null)
                    {
                        SwitchToTab(lastSelectedTab);
                    }

                FirstOpened = false;
            }
        }

        RibbonTab GetTabFromGlobalPreferences()
        {
            string lastSelectedTabName = Globals.UserPreferences.GetGlobalPreference<string>(GlobalPreference.LastSelectedRibbonTab);
            if (lastSelectedTabName == null)
            {
                lastSelectedTabName = DefaultTabName;
            }
         
            return (RibbonTab)this.FindName(lastSelectedTabName);
        }

        Views.ProductGroupView ProductGroupView;
        Views.ExperiencesView ExperiencesView;
        Views.ScenarioView ScenarioView;
        Views.BacklogManagerView BacklogManagerView;
        Views.ReviewPagesView ReviewPagesView;
        Views.ScrumTeamsView ScrumTeamsView;
        Views.BoardView BoardView;
        Views.MemberHomeView MemberHomeView;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called each time the user selects a different tab in the ribbon.
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ribbon ribbon = (Ribbon)sender;
            RibbonTab tab = (RibbonTab)ribbon.SelectedItem;
            ShowTab(tab);
        }

        private void RibbonWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void ShowTab(RibbonTab tab)
        {
            if (!FirstOpened)
            {
                Globals.UserPreferences.SetGlobalPreference(GlobalPreference.LastSelectedRibbonTab, tab.Name);
            }

            if (ProductGroupView != null)
            {
                ProductGroupView.Visibility = System.Windows.Visibility.Hidden;
            }

            if (ExperiencesView != null)
            {
                ExperiencesView.Visibility = System.Windows.Visibility.Hidden;
            }

            if (ScenarioView != null)
            {
                ScenarioView.Visibility = System.Windows.Visibility.Hidden;
            }

            if (BacklogManagerView != null)
            {
                BacklogManagerView.Visibility = System.Windows.Visibility.Hidden;
            }

            if (ReviewPagesView != null)
            {
                ReviewPagesView.Visibility = System.Windows.Visibility.Hidden;
            }

            if (ScrumTeamsView != null)
            {
                ScrumTeamsView.Visibility = System.Windows.Visibility.Hidden;
            }

            if (BoardView != null)
            {
                BoardView.Visibility = System.Windows.Visibility.Hidden;
            }

            if (MemberHomeView != null)
            {
                MemberHomeView.Visibility = System.Windows.Visibility.Hidden;
            }


            if (tab == ProductGroupTab)
            {
                if (ProductGroupView == null)
                {
                    Globals.EventManager.OnTabLoadStarting();
                    ProductGroupView = new Views.ProductGroupView();
                    ProductGroupView.InitializeData();
                    ProductGroupViewContent.Content = ProductGroupView;
                }

                ProductGroupView.Visibility = System.Windows.Visibility.Visible;
            }
            else if (tab == BacklogTab)
            {
                if (BacklogManagerView == null)
                {
                    Globals.EventManager.OnTabLoadStarting();
                    BacklogManagerView = new Views.BacklogManagerView();
                    BacklogManagerView.InitializeData();
                    BacklogManagerViewContent.Content = BacklogManagerView;
                }

                BacklogManagerView.Visibility = System.Windows.Visibility.Visible;
                BacklogManagerView.TabViewActivated();
            }
            else if (tab == ReviewPagesTab)
            {
                if (ReviewPagesView == null)
                {
                    Globals.EventManager.OnTabLoadStarting();
                    ReviewPagesView = new Views.ReviewPagesView();
                    ReviewPagesView.InitializeData();
                    ReviewPagesViewContent.Content = ReviewPagesView;
                }

                ReviewPagesView.Visibility = System.Windows.Visibility.Visible;
                ReviewPagesView.TabViewActivated();
            }
            else if (tab == ExperiencesTab)
            {
                if (ExperiencesView == null)
                {
                    Globals.EventManager.OnTabLoadStarting();
                    ExperiencesView = new Views.ExperiencesView();
                    ExperiencesView.InitializeData();
                    ExperiencesViewContent.Content = ExperiencesView;
                }

                ExperiencesView.Visibility = System.Windows.Visibility.Visible;

            }
            else if (tab == ScenarioTab)
            {
                if (ScenarioView == null)
                {
                    Globals.EventManager.OnTabLoadStarting();
                    ScenarioView = new Views.ScenarioView();
                    ScenarioView.InitializeData();
                    ScenarioViewContent.Content = ScenarioView;
                }

                ScenarioView.Visibility = System.Windows.Visibility.Visible;
            }
            else if (tab == ScrumTeamsTab)
            {
                if (ScrumTeamsView == null)
                {
                    Globals.EventManager.OnTabLoadStarting();
                    ScrumTeamsView = new Views.ScrumTeamsView();
                    ScrumTeamsView.InitializeData();
                    ScrumTeamsViewContent.Content = ScrumTeamsView;
                }

                ScrumTeamsView.Visibility = System.Windows.Visibility.Visible;
            }
            else if (tab == BoardTab)
            {
                if (BoardView == null)
                {
                    Globals.EventManager.OnTabLoadStarting();
                    BoardView = new Views.BoardView();
                    BoardView.InitializeData();
                    BoardViewContent.Content = BoardView;
                }

                BoardView.Visibility = System.Windows.Visibility.Visible;
            }
            else if (tab == MemberHomeTab)
            {
                if (MemberHomeView == null)
                {
                    Globals.EventManager.OnTabLoadStarting();
                    MemberHomeView = new Views.MemberHomeView();
                    MemberHomeView.InitializeData();
                    MemberHomeViewContent.Content = MemberHomeView;
                }

                MemberHomeView.Visibility = System.Windows.Visibility.Visible;
            }
        }


        // Ribbon Click handling
        private void NewWorkItemButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Globals.EventManager.OnCreateNewWorkItem();
        }

        private void DeleteWorkItemButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Globals.EventManager.OnDeleteWorkItem();
        }

        private void BacklogCreateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Globals.EventManager.OnCreateBacklogItem();
        }

        private void EventLogButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Globals.ApplicationManager.ShowEventLog();
        }

        private void ScheduleCheckerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Globals.ScheduleChecker.ShowScheduleChecker();
        }

        private void SaveChanges_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Globals.ApplicationManager.CommitChanges();
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Globals.ApplicationManager.Refresh();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            Globals.ItemManager.UndoChanges();
        }

        private void Main_RightMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            Globals.ApplicationManager.HandleRightClickContentMenus(this);
        }
    }
}
