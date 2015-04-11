using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlannerNameSpace
{
    public class RichTextContext
    {
        public static Window CurrentItemEditorWindow { get; set; }

        public StoreItem StoreItem { get; set; }
        public string HeaderText { get; set; }
    }

}
