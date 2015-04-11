using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PlannerNameSpace
{
    public class ApplicationManager
    {
        private DispatcherTimer UpdateTimer;

        public string CurrentUserAlias { get; set; }
        public bool IsStartupComplete { get; set; }

        private AsyncObservableCollection<string> m_eventLog;
        private string StoreError { get; set; }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be fired when start-up is fully complete, and the MainWindow is ready.
        /// </summary>
        //------------------------------------------------------------------------------------
        public event EventHandler ApplicationStartupComplete;

        public AsyncObservableCollection<string> AdminAliases { get; set; }
        public AsyncObservableCollection<string> DevAliases { get; set; }

        bool IsOffline { get; set; }

        public ApplicationManager()
        {
            m_eventLog = new AsyncObservableCollection<string>();
            CurrentUserAlias = GetCurrentUserName().ToLower(System.Globalization.CultureInfo.CurrentCulture);

            Globals.EventManager.PlannerRefreshStarting += Instance_PlannerRefreshStarting;
            Globals.EventManager.DiscoveryComplete += Handle_DiscoveryComplete;
            ProductGroupManager.Instance.OpenProductGroupComplete += Handle_OpenProductGroupComplete;
            StoreQueryManager.Instance.PlannerQueryCompleted += Handle_PlannerQueryCompleted;

            IsStartupComplete = false;
            IsOffline = false;

            AdminAliases = new AsyncObservableCollection<string>();
            AdminAliases.Add("tracef");
            AdminAliases.Add("dconger");
            AdminAliases.Add("imranazi");
            AdminAliases.Add("scottmcf");

            DevAliases = new AsyncObservableCollection<string>();
            DevAliases.Add("tracef");

            UpdateTimer = new DispatcherTimer();

            UpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            UpdateTimer.Tick += AppManager_Tick;

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
        }

        void Instance_PlannerRefreshStarting()
        {
            CurrentWarning = null;
        }

        public void InitializeMainWindow()
        {
            Globals.MainWindow.InitializeTabs();
            Globals.MainWindow.KeyDown += HandleGlobalKeyboardShortcuts;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when start-up is complete, and the MainWindow is ready.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void StartupComplete()
        {
            UpdateTimer.Start();

            IsStartupComplete = true;
            Globals.UserPreferences.SetGlobalPreference(GlobalPreference.LastOpenProductGroupKey, ProductGroupManager.Instance.CurrentProductGroupKey);

            if (ApplicationStartupComplete != null)
            {
                ApplicationStartupComplete(this, EventArgs.Empty);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Manages the startup process for the entire application.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Startup(bool shouldClearProductGroup)
        {
            if (IsOffline)
            {
                UserMessage.Show(AssemblyProduct + " is currently offline for maintenance.  Please check back later (ETA is 2:00 pm today (5/15/2013).");
                Globals.Shutdown();
                return;
            }

            Tracer.InitializeTraceFile();

            if (shouldClearProductGroup)
            {
                StoreQueryManager.Instance.BeginProductGroupQuery(null);
            }
            else
            {
                ProductGroupManager.Instance.BeginOpenProductGroup();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Handle the event fired when the attempt to open the current product group has 
        /// completed.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Handle_OpenProductGroupComplete(object sender, ProductGroupOpenedEvent e)
        {
            if (e.Result.ResultType == ResultType.Failed)
            {
                UserMessage.ShowTwoLines("OPlanner encountered an error during startup.", e.Result.ResultMessage);
                Globals.Shutdown();
                return;
            }

            bool IsProductGroupCompatible = true;
            if (e.ProductGroupItem != null && !e.ProductGroupItem.IsCompatibleWithCurrentStore)
            {
                IsProductGroupCompatible = false;
                UserMessage.Show("The Product Group you last opened (" + e.ProductGroupItem.Title + ") is not compatible with the current Store (" + HostItemStore.Instance.StoreName + "). Click 'OK' to pick a different group.");
            }

            // If the user hasn't already selected a product group on a previous run of the app, kick off a product group
            // query so that we can present the user with the current product groups available to be opened.
            if (e.ProductGroupKey == null || !IsProductGroupCompatible)
            {
                StoreQueryManager.Instance.BeginProductGroupQuery(e.Result.Task);
            }
            else
            {
                StoreQueryManager.Instance.BeginPlannerQuery(ShouldRefresh.No, e.ProductGroupKey, e.Result.Task);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Handle the event fired when the PlannerQuery is completed.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Handle_PlannerQueryCompleted(object sender, PlannerQueryCompletedEvent e)
        {
            if (e.Result.ResultType == ResultType.Cancelled || e.Result.ResultType == ResultType.Failed)
            {
                if (!IsStartupComplete)
                {
                    Globals.Shutdown();
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Handle the event fired when the query to pull in all the currently defined product
        /// groups is fully complete.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ProductGroupQueryComplete()
        {
            ApplicationWelcome welcome = new ApplicationWelcome();
            welcome.BeginWelcome();

            if (welcome.WelcomeState == WelcomeState.Cancel)
            {
                Globals.Shutdown();
            }

            else if (welcome.WelcomeState == WelcomeState.Open)
            {
                OpenProductGroup(welcome.SelectedProductGroupItem);
            }
        }

        public void BeginProductGroup(ProductGroupItem productGroupItem)
        {
            ProductGroupManager.Instance.CurrentProductGroup = productGroupItem;
            ProductGroupManager.Instance.CurrentProductGroupKey = productGroupItem.StoreKey;

            UpdateTimer.Stop();
            IsStartupComplete = false;

            StoreQueryManager.Instance.BeginPlannerQuery(ShouldRefresh.No, productGroupItem.StoreKey);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Opens the schedule for the product group represented by the given item, kicking
        /// the currently open group out of memory.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void OpenProductGroup(ProductGroupItem productGroupItem)
        {
            UpdateTimer.Stop();
            Globals.OpenProductGroup(productGroupItem);
        }

        void Handle_MainWindowReady(object sender, EventArgs e)
        {
            Globals.MainWindow.InitializeTabs();
            Globals.MainWindow.KeyDown += HandleGlobalKeyboardShortcuts;
            UpdateTimer.Start();
        }

        void Handle_DiscoveryComplete(object sender, EventArgs e)
        {
            Globals.ProductTreeManager.EnsureHostProductTree();
        }

        public void PlannerQueryCancelled(bool isRefresh)
        {
            StoreQueryManager.Instance.IsRefreshInProgress = false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Restarts the application with the last-opened product group set to the given
        /// product key.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Restart(string newProductGroupKey)
        {
            Globals.UserPreferences.SetGlobalPreference(GlobalPreference.LastOpenProductGroupKey, newProductGroupKey);
            Process.Start(System.Windows.Forms.Application.ExecutablePath);
            Globals.Shutdown();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Generates the app UI update clock
        /// </summary>
        //------------------------------------------------------------------------------------
        void AppManager_Tick(object sender, EventArgs e)
        {
            Globals.EventManager.OnUpdateUI();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Commits all unsaved changes to the backend store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void CommitChanges()
        {
            if (StoreQueryManager.Instance.IsQueryInProgress)
            {
                StoreQueryManager.Instance.CancelQuery();
            }

            if (Globals.ItemManager.ChangeCount > 0)
            {
                Globals.ItemManager.CommitChanges();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sends a refresh query to the back-end store, and syncs the in-memory cache to
        /// any resulting changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Refresh()
        {
            if (!StoreQueryManager.Instance.IsRefreshInProgress)
            {
                if (!Globals.ItemManager.IsDiscoveryComplete)
                {
                    UserMessage.Show("Please wait for item discovery to be completed before performing a Refresh.");
                }
                else if (Globals.ItemManager.ChangeCount > 0)
                {
                    UserMessage.Show("Refresh is not available while changes are pending - save your changes, and then try Refresh again.");
                }
                else
                {
                    Globals.EventManager.OnPlannerRefreshStarting();
                    StoreQueryManager.Instance.BeginRefreshQuery(ProductGroupManager.Instance.CurrentProductGroupKey);
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called when a keyboard event that pertains to the app main window occurs.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void HandleGlobalKeyboardShortcuts(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Handle global shortcut keys
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == System.Windows.Input.Key.S)
                {
                    e.Handled = true;
                    Globals.ApplicationManager.CommitChanges();
                }

                if (e.Key == System.Windows.Input.Key.R)
                {
                    e.Handled = true;
                    Globals.ApplicationManager.Refresh();
                }

                else if (e.Key == System.Windows.Input.Key.G)
                {
                    e.Handled = true;
                    Globals.EventManager.OnGotoItemCommand();
                }

                else if (e.Key == System.Windows.Input.Key.E)
                {
                    e.Handled = true;
                    ShowEventLog();
                }

                else if (e.Key == System.Windows.Input.Key.Z)
                {
                    e.Handled = true;
                    Globals.ItemManager.UndoChanges();
                }

            }
        }

        public void HandleRightClickContentMenus(Window parentWindow)
        {
            CurrentHostWindow = parentWindow;
            IInputElement element = Mouse.DirectlyOver;
            if (element != null && element is FrameworkElement)
            {
                FrameworkElement frameworkElement = (FrameworkElement)element;
                StoreItem storeItem = frameworkElement.DataContext as StoreItem;
                if (storeItem != null)
                {
                    ShowItemContextMenu(storeItem, parentWindow);
                }
            }
        }

        public void ShowItemContextMenu(StoreItem storeItem, Window parentWindow)
        {
            if (storeItem != null)
            {
                ContextMenu menu = storeItem.GetContextMenu(parentWindow);
                if (menu != null)
                {
                    Utils.OpenContextMenu(menu);
                }
            }
        }

        public void SetStartupLocation(Window window)
        {
            Window hostWindow = Globals.MainWindow;
            if (CurrentHostWindow != null)
            {
                hostWindow = CurrentHostWindow;
                CurrentHostWindow = null;
            }

            window.Owner = hostWindow;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
        }
        
        public Window CurrentHostWindow { get; set; }

        OPlannerBugsView BugsViewWindow;
        public void ShowOPlannerBugsWindow()
        {
            if (BugsViewWindow != null)
            {
                if (!BugsViewWindow.IsVisible)
                {
                    BugsViewWindow.Show();
                }

                BugsViewWindow.BringIntoView();
            }
            else
            {
                BugsViewWindow = new OPlannerBugsView();
                BugsViewWindow.Closed +=BugsViewWindow_Closed;
                BugsViewWindow.Show();
            }
        }

        void BugsViewWindow_Closed(object sender, EventArgs e)
        {
            BugsViewWindow = null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// User has clicked on the button to bring up the TrainEdit dialog.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void EditTrains()
        {
            if (ConfirmIsAdmin("Edit Office Trains"))
            {
                TrainEditor dialog = new TrainEditor();
                dialog.ShowDialog();
            }
        }

        public static void GotoItem(DataGrid grid)
        {
            GotoItem gotoDialog = new GotoItem();
            gotoDialog.ShowDialog();
            if (gotoDialog.ItemID > 0)
            {
                StoreItem item = Globals.ItemManager.GetHostItemByID<StoreItem>(gotoDialog.ItemID);
                if (item != null)
                {
                    grid.Focus();
                    grid.ScrollIntoView(item);
                    grid.SelectedItem = item;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the full path to the root storage folder for this application.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetOPlannerFolder()
        {
            string appPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            appPath = Path.Combine(appPath, RootProductName);
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

            return appPath;
        }

        public void ClearEventLog()
        {
            m_eventLog.Clear();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Adds the given text to the event log
        /// </summary>
        //------------------------------------------------------------------------------------
        public void WriteToEventLog(string text)
        {
            try
            {
                if (!text.EndsWith("."))
                    text += ".";

                string entry = DateTime.Now.ToLongTimeString() + ": " + text;
                Tracer.WriteTrace(entry);
                m_eventLog.Add(entry);
            }
            catch (Exception) { }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Add information about the given item to the event log, along with the given 
        /// message.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void AddStoreItemLogEntry(StoreItem item, string message)
        {
            Globals.ApplicationManager.WriteToEventLog(" ID=" + item.ID.ToString() + "; Title=" + item.Title + "; " + message);
        }

        string CurrentWarning = null;
        public string GetCurrentWarning()
        {
            return CurrentWarning;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Add information about the given item to the event log, along with the given 
        /// message.  Returns true if the exceptions was successfully handled.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool HandleException(Exception exception)
        {
            Globals.ApplicationManager.WriteToEventLog(exception.Message);
            Globals.ApplicationManager.WriteToEventLog(exception.StackTrace);
            return false;
        }

        public void HandleStoreItemException(StoreItem item, Exception exception)
        {
            Globals.ApplicationManager.WriteToEventLog(exception.Message);
            Globals.ApplicationManager.WriteToEventLog(exception.StackTrace);

            if (item.CommitErrorState == CommitErrorStates.ErrorAccessingAttachmentShare)
            {
                CurrentWarning = PlannerContent.ProductStudioMiddleTierProblems;
            }
        }

        public AsyncObservableCollection<string> EventLogEntries { get { return m_eventLog; } }

        EventLog EventLogWindow;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Opens the event log window, or brings it to the front if already showing.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ShowEventLog()
        {
            if (EventLogWindow == null)
            {
                EventLogWindow = new EventLog();
                EventLogWindow.Closed += EventLogWindow_Closed;
                EventLogWindow.Show();
            }
            else
            {
                EventLogWindow.Focus();
            }
        }

        void EventLogWindow_Closed(object sender, EventArgs e)
        {
            EventLogWindow = null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetCurrentUserDomainName
        /// 
        /// Gets the domain fieldName of the user currently running the app.
        /// </summary>
        //------------------------------------------------------------------------------------
        private string GetCurrentUserDomainName()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetCurrentUserName
        /// 
        /// Gets the alias (minus domain) of the current user running the app.
        /// </summary>
        //------------------------------------------------------------------------------------
        private string GetCurrentUserName()
        {
            string currentUser = null;
            string domainName = GetCurrentUserDomainName();
            if (!String.IsNullOrEmpty(domainName))
            {
                int userIndex = domainName.LastIndexOf('\\');
                if (userIndex >= 0)
                {
                    currentUser = domainName.Substring(userIndex + 1);
                }
            }

            return currentUser;
        }

        public static string RootProductName
        {
            get { return "OPlanner"; }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the application ProductName FieldName.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        static BitmapSource GenericProfileSource = null;
        public BitmapSource GenericProfileBitmap
        {
            get
            {
                if (GenericProfileSource == null)
                {
                    var hBitmap = Properties.Resources.GenericProfile.GetHbitmap();
                    GenericProfileSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }

                return GenericProfileSource;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Displays an appropriate error message based on the given error result.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void HandleFailedTask(BackgroundTaskResult result)
        {
            StoreErrors error = Datastore.GetStoreErrorFromExceptionMessage(result.ResultMessage);
            if (error == StoreErrors.ProductStudioNotInstalled || error == StoreErrors.ProductStudioNewerVersionRequired)
            {
                ProductStudioNotInstalledDialog dialog = new ProductStudioNotInstalledDialog(error);
                dialog.ShowDialog();
            }
            else
            {
                UserMessage.Show(result.ResultMessage);
            }
        }

        public bool RunningOnUIThread()
        {
            return Globals.MainWindow.Dispatcher.Thread == System.Threading.Thread.CurrentThread;
        }

        public bool IsUserOPlannerDev(string alias)
        {
            return DevAliases.Contains(alias);
        }

        public bool IsCurrentUserOPlannerDev()
        {
            return IsUserOPlannerDev(CurrentUserAlias);
        }

        public bool IsUserAdmin(string alias)
        {
            return AdminAliases.Contains(alias);
        }

        public bool IsCurrentUserAdmin()
        {
            return IsUserAdmin(CurrentUserAlias);
        }

        public bool ConfirmIsAdmin(string featureRequiringAdmin)
        {
            if (!IsCurrentUserAdmin())
            {
                AdminRequiredDialog dialog = new AdminRequiredDialog(featureRequiringAdmin);
                dialog.ShowDialog();

                return false;
            }

            return true;
        }


    }
}
