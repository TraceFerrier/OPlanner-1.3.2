using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public class ColumnViewManager : ViewManager
    {
        DataGrid ViewDataGrid { get; set; }
        List<string> AllColumns { get; set; }

        public ColumnViewManager(DataGrid dataGrid, ProductPreferences initialViewPreference, ProductPreferences initialViewTypePreference, AvailableViews defaultView, ViewType defaultViewType)
            : base(initialViewPreference, initialViewTypePreference, defaultView, defaultViewType)
        {
            ViewDataGrid = dataGrid;
            AllColumns = new List<string>();
        }

        public void AddViewColumn(AvailableViews view, string columnHeader)
        {
            if (!Views.ContainsKey(view))
            {
                Views.Add(view, new List<string>());
            }

            List<string> viewCollection = Views[view];
            if (!viewCollection.Contains(columnHeader))
            {
                viewCollection.Add(columnHeader);
            }

            if (!AllColumns.Contains(columnHeader))
            {
                AllColumns.Add(columnHeader);
            }
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            UpdateViewColumns();
        }

        void UpdateViewColumns()
        {
            // First, hide all the view-specific columns
            foreach (string columnHeader in AllColumns)
            {
                DataGridColumn column = GetColumn(ViewDataGrid, columnHeader);
                column.Visibility = System.Windows.Visibility.Hidden;
            }

            foreach (KeyValuePair<AvailableViews, List<string>> kvp in Views)
            {
                // Then show only those called out for the current view.
                AvailableViews viewName = kvp.Key;
                List<string> viewColumns = kvp.Value;
                foreach (string columnHeader in viewColumns)
                {
                    DataGridColumn column = GetColumn(ViewDataGrid, columnHeader);
                    View view = IdentifyView(viewName);
                    if (view.ViewName == CurrentView.ViewName)
                    {
                        column.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
        }

        DataGridColumn GetColumn(DataGrid dataGrid, string columnHeader)
        {
            ObservableCollection<DataGridColumn> columns = dataGrid.Columns;
            foreach (DataGridColumn column in columns)
            {
                if (Utils.StringsMatch(columnHeader, column.Header as string))
                {
                    return column;
                }
            }

            return null;
        }
    }
}
