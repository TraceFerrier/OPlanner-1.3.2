﻿#pragma checksum "..\..\..\..\WorkItems\WorkItemBackToActiveDialog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "522D3C57DEF1FFB294D72C87C5B62760"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace PlannerNameSpace.WorkItems {
    
    
    /// <summary>
    /// WorkItemBackToActiveDialog
    /// </summary>
    public partial class WorkItemBackToActiveDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\..\..\WorkItems\WorkItemBackToActiveDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid DialogContext;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\..\WorkItems\WorkItemBackToActiveDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock EstimateBox;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\WorkItems\WorkItemBackToActiveDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CompletedBox;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\..\WorkItems\WorkItemBackToActiveDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox RemainingBox;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\..\WorkItems\WorkItemBackToActiveDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OkButton;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\..\WorkItems\WorkItemBackToActiveDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CancelButton;
        
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
            System.Uri resourceLocater = new System.Uri("/OPlanner 1.0;component/workitems/workitembacktoactivedialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\WorkItems\WorkItemBackToActiveDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
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
            this.DialogContext = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.EstimateBox = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.CompletedBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 43 "..\..\..\..\WorkItems\WorkItemBackToActiveDialog.xaml"
            this.CompletedBox.LostFocus += new System.Windows.RoutedEventHandler(this.CompletedBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 4:
            this.RemainingBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.OkButton = ((System.Windows.Controls.Button)(target));
            
            #line 49 "..\..\..\..\WorkItems\WorkItemBackToActiveDialog.xaml"
            this.OkButton.Click += new System.Windows.RoutedEventHandler(this.OkButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.CancelButton = ((System.Windows.Controls.Button)(target));
            
            #line 50 "..\..\..\..\WorkItems\WorkItemBackToActiveDialog.xaml"
            this.CancelButton.Click += new System.Windows.RoutedEventHandler(this.CancelButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

