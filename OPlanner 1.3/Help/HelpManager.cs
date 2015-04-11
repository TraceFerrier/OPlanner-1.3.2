using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PlannerNameSpace
{
    [Serializable]
    public class UserLog
    {
        public string UserName { get; set; }
        public string LaunchTime { get; set; }
        public string LaunchDate { get; set; }
        public int LaunchCount { get; set; }
    }

    public sealed class HelpManager
    {
        private bool m_ProductLaunchLogged;
        private Dictionary<string, Style> ToolTipStyles;
        private static readonly HelpManager m_instance = new HelpManager();

        private HelpManager()
        {
            ToolTipStyles = new Dictionary<string, Style>();
            m_ProductLaunchLogged = false;
            Globals.EventManager.StoreCommitComplete += Instance_StoreCommitComplete;
        }

        void Instance_StoreCommitComplete(object sender, StoreCommitCompleteEventArgs e)
        {
            // Log the user's product launch on the first user commit operation.
            if (!m_ProductLaunchLogged && e.CommitType == CommitType.UserCommit)
            {
                m_ProductLaunchLogged = true;
                LogProductLaunch(Globals.ApplicationManager.CurrentUserAlias);
            }
        }

        public static HelpManager Instance
        {
            get { return m_instance; }
        }

        public ToolTip GetToolTip(string text, string style)
        {
            if (!ToolTipStyles.ContainsKey(style))
            {
                Style newStyle = (Style)Application.Current.FindResource(style);
                ToolTipStyles.Add(style, newStyle);
            }

            ToolTip toolTip = new System.Windows.Controls.ToolTip();
            toolTip.Style = ToolTipStyles[style];
            toolTip.Content = text;
            return toolTip;
        }

        public HelpContentItem GetHelpContentItem()
        {
            if (Globals.ItemManager.HelpItems.Count == 0)
            {
                return null;
            }

            return Globals.ItemManager.HelpItems.GetItem(0);
        }

        HelpWindow HelpWindow;
        public void ShowHelpWindow(Window owner, string topic)
        {
            if (HelpWindow != null)
            {
                if (!HelpWindow.IsVisible)
                {
                    HelpWindow.Show();
                }

                HelpWindow.BringIntoView();
            }
            else
            {
                HelpWindow = new HelpWindow(owner, topic);
                HelpWindow.Closed += HelpWindow_Closed;
                HelpWindow.Show();
            }
        }

        void HelpWindow_Closed(object sender, EventArgs e)
        {
            HelpWindow = null;
        }

        public Nullable<DateTime> HostProductTreeLastUpdate
        {
            get
            {
                HelpContentItem helpItem = GetHelpContentItem();
                if (helpItem == null)
                {
                    return null;
                }

                return helpItem.HostProductTreeLastUpdate;
            }

            set
            {
                HelpContentItem helpItem = GetHelpContentItem();
                if (helpItem != null)
                {
                    helpItem.BeginSaveImmediate();
                    helpItem.HostProductTreeLastUpdate = value;
                    helpItem.SaveImmediate();
                }
            }
        }

        public List<UserLog> UserLogList
        {
            get
            {
                HelpContentItem helpItem = GetHelpContentItem();
                return Utils.UnserializeFromItemProperty<List<UserLog>>(helpItem, Datastore.PropNameHelpContentUserLog);
            }

            set
            {
                HelpContentItem helpItem = GetHelpContentItem();
                Utils.SerializeToItemProperty(helpItem, Datastore.PropNameHelpContentUserLog, value);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Logs to the back-end store that the given user successfully launched the app.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void LogProductLaunch(string user)
        {
            BackgroundTask logProductLaunchTask = new BackgroundTask(false);
            logProductLaunchTask.DoWork += logProductLaunchTask_DoWork;
            logProductLaunchTask.TaskCompleted += logProductLaunchTask_TaskCompleted;
            logProductLaunchTask.TaskArgs = user;
            logProductLaunchTask.RunTaskAsync();
        }

        void logProductLaunchTask_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundTask task = e.Argument as BackgroundTask;
            string currentUser = task.TaskArgs as string;
            List<UserLog> oldUserLogList = UserLogList;

            // For backward compatibility with the old user log format (which had one entry for
            // every user log-in), handle the case of multiple entries for the same user,
            // consolidating to a single list.
            Dictionary<string, UserLog> newUserLogDict = new Dictionary<string, UserLog>();
            bool userFound = false;
            foreach (UserLog userLog in oldUserLogList)
            {
                if (!newUserLogDict.ContainsKey(userLog.UserName))
                {
                    if (userLog.LaunchCount == 0)
                    {
                        userLog.LaunchCount = 1;
                    }
                    newUserLogDict.Add(userLog.UserName, userLog);
                }
                else
                {
                    newUserLogDict[userLog.UserName].LaunchCount++;
                    newUserLogDict[userLog.UserName].LaunchTime = userLog.LaunchTime;
                    newUserLogDict[userLog.UserName].LaunchDate = userLog.LaunchDate;
                }

                if (Utils.StringsMatch(userLog.UserName, currentUser))
                {
                    userLog.LaunchTime = DateTime.Now.ToLongTimeString();
                    userLog.LaunchDate = DateTime.Now.ToLongDateString();
                    userLog.LaunchCount++;
                    userFound = true;
                }
            }

            if (!userFound)
            {
                UserLog userLog = new UserLog();
                userLog.UserName = currentUser;
                userLog.LaunchTime = DateTime.Now.ToLongTimeString();
                userLog.LaunchDate = DateTime.Now.ToLongDateString();
                userLog.LaunchCount = 1;
                newUserLogDict.Add(currentUser, userLog);
            }

            UserLogList = newUserLogDict.ToList();
        }

        void logProductLaunchTask_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
        }

        MemoryStream GetHelpTopic(string topicName)
        {
            HelpContentItem helpItem = GetHelpContentItem();
            if (helpItem != null)
            {
                return ScheduleStore.Instance.GetRichTextFileAttachmentStream(helpItem, topicName);
            }

            return null;
        }
    }
}
