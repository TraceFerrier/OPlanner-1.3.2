using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PlannerNameSpace
{
    public class MouseUtils
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        public static void DoMouseClickOnControl(Control control)
        {
            Point screenPoint = control.PointToScreen(new Point(25, 10));

            Globals.ApplicationManager.WriteToEventLog("Point X: " + screenPoint.X.ToString() + "Point Y: " + screenPoint.Y.ToString());
            DoMouseClick(screenPoint);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the cursor position to the given absolute screen coordinates.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void SetCursorPosition(System.Windows.Point screenPoint)
        {
            int x = (int)screenPoint.X;
            int y = (int)screenPoint.Y;
            SetCursorPos(x, y);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Simulates a mouse left-click at the given absolute screen coordinates, without 
        /// changing the actual cursor position.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void DoMouseClick(System.Windows.Point screenPoint)
        {
            MousePoint currPosition;
            GetCursorPos(out currPosition);
            SetCursorPos((int)screenPoint.X, (int)screenPoint.Y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE, (uint)screenPoint.X, (uint)screenPoint.Y, 0, 0);
            SetCursorPos(currPosition.X, currPosition.Y);
        }

    }
}
