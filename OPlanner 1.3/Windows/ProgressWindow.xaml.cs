using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace PlannerNameSpace
{
	/// <summary>
	/// Interaction logic for ProgressWindow.xaml
	/// </summary>
	public partial class ProgressWindow : Window
	{
		private bool AllowClose { get; set; }

		public ProgressWindow(Window owner, bool isIndeterminate)
		{
			InitializeComponent();
			AllowClose = false;

			if (owner == null || !owner.IsVisible)
			{
				this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			}
			else
			{
				this.Owner = owner;
			}

			this.Title = ApplicationManager.AssemblyProduct;
			ProgressBarControl.IsIndeterminate = isIndeterminate;
			ProgressBarControl.Minimum = 0;
			ProgressBarControl.Maximum = 100;
		}

		public void CloseProgressWindow()
		{
			AllowClose = true;
			Close();
		}

		public void SetMessage(string message)
		{
			MessageBox.Text = message;
		}

		public void SetProgress(int progress)
		{
			ProgressBarControl.Value = progress;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (!AllowClose)
			{
				e.Cancel = true;
			}
		}
	}
}
