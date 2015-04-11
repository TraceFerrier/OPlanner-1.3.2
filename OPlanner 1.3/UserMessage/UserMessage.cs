using System.Windows;
using System.Text;

namespace PlannerNameSpace
{
    public static class UserMessage
    {
        public static bool Show(string message, MessageBoxButton buttonType = MessageBoxButton.OK)
        {
            return ShowFourLines(Globals.MainWindow, message, null, null, null, ApplicationManager.AssemblyProduct, buttonType);
        }

        public static bool ShowInfo(string message, MessageBoxButton buttonType = MessageBoxButton.OK)
        {
            return ShowFourLines(Globals.MainWindow, message, null, null, null, ApplicationManager.AssemblyProduct, buttonType, MessageBoxImage.Information);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ShowTwoLines
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool ShowTwoLines(string line1, string line2, MessageBoxButton buttonType = MessageBoxButton.OK)
        {
            return ShowFourLines(Globals.MainWindow, line1, line2, null, null, ApplicationManager.AssemblyProduct, buttonType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ShowTwoLines
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool ShowThreeLines(string line1, string line2, string line3, MessageBoxButton buttonType = MessageBoxButton.OK)
        {
            return ShowFourLines(Globals.MainWindow, line1, line2, line3, null, ApplicationManager.AssemblyProduct, buttonType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ShowTwoLines
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool ShowFourLines(Window owner, string line1, string line2, string line3, string line4, string caption, MessageBoxButton buttonType = MessageBoxButton.OK, MessageBoxImage imageType = MessageBoxImage.Warning)
        {
            StringBuilder message = new StringBuilder();

            if (line1 != null)
            {
                message.AppendLine(line1);
            }

            if (line2 != null)
            {
                message.AppendLine();
                message.AppendLine(line2);
            }

            if (line3 != null)
            {
                message.AppendLine();
                message.AppendLine(line3);
            }

            if (line4 != null)
            {
                message.AppendLine();
                message.AppendLine(line4);
            }

            MessageBoxResult result;
            if (owner == null)
            {
                result = System.Windows.MessageBox.Show(message.ToString(), caption, buttonType, imageType);
            }
            else
            {
                result = System.Windows.MessageBox.Show(owner, message.ToString(), caption, buttonType, imageType);
            }

            if (buttonType == MessageBoxButton.OKCancel)
            {
                return result == MessageBoxResult.OK;
            }
            else
            {
                return result == MessageBoxResult.Yes;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Show(string message, string caption)
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void Show(string message, string caption)
        {
            ShowFourLines(Globals.MainWindow, message, null, null, null, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Show
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void Show(Window owner, string message, string caption)
        {
            ShowFourLines(owner, message, null, null, null, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Show
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void Show(Window owner, string message)
        {
            ShowFourLines(owner, message, null, null, null, ApplicationManager.AssemblyProduct, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ShowYesNo
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool ShowYesNo(Window owner, string message, string caption)
        {
            return ShowFourLines(owner, message, null, null, null, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ShowYesNo
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool ShowOkCancel(Window owner, string message, string caption)
        {
            return ShowFourLines(owner, message, null, null, null, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }

        public static void WaitCursor()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        }

        public static void DefaultCursor()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }
    }
}
