﻿#pragma checksum "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A15BFAA0F4FFEF0F03875A5024054569"
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


namespace PlannerNameSpace {
    
    
    /// <summary>
    /// NewOrEditScrumTeamDialog
    /// </summary>
    public partial class NewOrEditScrumTeamDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid DialogContext;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TitleTextBox;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox PillarCombo;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ScrumMasterCombo;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OKButton;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
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
            System.Uri resourceLocater = new System.Uri("/OPlanner 1.0;component/scrumteams/neworeditscrumteamdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
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
            this.TitleTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 44 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
            this.TitleTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TitleTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.PillarCombo = ((System.Windows.Controls.ComboBox)(target));
            
            #line 52 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
            this.PillarCombo.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.PillarCombo_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ScrumMasterCombo = ((System.Windows.Controls.ComboBox)(target));
            
            #line 60 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
            this.ScrumMasterCombo.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ScrumMasterCombo_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.OKButton = ((System.Windows.Controls.Button)(target));
            
            #line 67 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
            this.OKButton.Click += new System.Windows.RoutedEventHandler(this.OKButton_Clicked);
            
            #line default
            #line hidden
            return;
            case 6:
            this.CancelButton = ((System.Windows.Controls.Button)(target));
            
            #line 68 "..\..\..\..\ScrumTeams\NewOrEditScrumTeamDialog.xaml"
            this.CancelButton.Click += new System.Windows.RoutedEventHandler(this.CancelButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

