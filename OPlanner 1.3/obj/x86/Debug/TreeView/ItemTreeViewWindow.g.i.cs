﻿#pragma checksum "..\..\..\..\TreeView\ItemTreeViewWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A196348DE40CF6A883BD71D7FDBE94FE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using PlannerNameSpace;
using PlannerNameSpace.UserControls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace PlannerNameSpace {
    
    
    /// <summary>
    /// ItemTreeViewWindow
    /// </summary>
    public partial class ItemTreeViewWindow : System.Windows.Controls.Ribbon.RibbonWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\..\TreeView\ItemTreeViewWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Ribbon.RibbonButton GeneralHelpButton;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\TreeView\ItemTreeViewWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ViewContext;
        
        #line default
        #line hidden
        
        
        #line 297 "..\..\..\..\TreeView\ItemTreeViewWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView TreeViewControl;
        
        #line default
        #line hidden
        
        
        #line 298 "..\..\..\..\TreeView\ItemTreeViewWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeViewItem TreeRoot;
        
        #line default
        #line hidden
        
        
        #line 301 "..\..\..\..\TreeView\ItemTreeViewWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ExperienceHeader;
        
        #line default
        #line hidden
        
        
        #line 370 "..\..\..\..\TreeView\ItemTreeViewWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ScenarioHeader;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/OPlanner 1.0;component/treeview/itemtreeviewwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\TreeView\ItemTreeViewWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\..\TreeView\ItemTreeViewWindow.xaml"
            ((System.Windows.Controls.Grid)(target)).MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Grid_MouseRightButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 22 "..\..\..\..\TreeView\ItemTreeViewWindow.xaml"
            ((System.Windows.Controls.Ribbon.RibbonButton)(target)).Click += new System.Windows.RoutedEventHandler(this.SaveChanges_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 23 "..\..\..\..\TreeView\ItemTreeViewWindow.xaml"
            ((System.Windows.Controls.Ribbon.RibbonButton)(target)).Click += new System.Windows.RoutedEventHandler(this.Refresh_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 24 "..\..\..\..\TreeView\ItemTreeViewWindow.xaml"
            ((System.Windows.Controls.Ribbon.RibbonButton)(target)).Click += new System.Windows.RoutedEventHandler(this.Undo_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.GeneralHelpButton = ((System.Windows.Controls.Ribbon.RibbonButton)(target));
            return;
            case 6:
            this.ViewContext = ((System.Windows.Controls.Grid)(target));
            return;
            case 7:
            this.TreeViewControl = ((System.Windows.Controls.TreeView)(target));
            return;
            case 8:
            this.TreeRoot = ((System.Windows.Controls.TreeViewItem)(target));
            return;
            case 9:
            this.ExperienceHeader = ((System.Windows.Controls.Grid)(target));
            return;
            case 10:
            this.ScenarioHeader = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

