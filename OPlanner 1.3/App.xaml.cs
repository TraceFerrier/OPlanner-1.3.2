using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Planner_Startup(object sender, StartupEventArgs e)
        {
            // We're going to put up a start-up dialog before showing the main window, so set 
            // ShutdownMode so the app doesn't close when the start-up dialog is taken down.
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            App.Current.Exit += new ExitEventHandler(Current_Exit);

            // Kick off the startup process
            Globals.Startup();

            if (!Globals.IsShuttingDown)
            {
                this.MainWindow = new MainWindow();
                Globals.InitializeMainWindow((MainWindow)this.MainWindow);

                // Now re-set ShutdownMode so that the app will close normally when the main window is closed.
                this.ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;
                this.MainWindow.Show();

                Globals.ApplicationManager.StartupComplete();
            }
        }

        public void CreateMainWindow()
        {
            this.MainWindow = new MainWindow();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Event handler to enable us to take action when the user shuts down the app.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Current_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            Globals.Shutdown();
        }
    }
}
