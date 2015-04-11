using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace PlannerNameSpace
{
    public enum ResultValue
    {
        Running,
        Completed,
        Cancelled,
        Failed
    }

    public class QueryResults
    {
        public ResultValue Result { get; set; }
        public string ResultsMessage { get; set; }
        public object UserResults { get; set; }
    }
}
